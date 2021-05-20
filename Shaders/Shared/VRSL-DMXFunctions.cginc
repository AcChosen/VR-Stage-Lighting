#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

// float blockCenter = compatSampleYAxis;
// float blockTraversal = 0.03846;
uint getChannelSectorX()
{
    return (uint) round(UNITY_ACCESS_INSTANCED_PROP(Props, _Sector));  
}

uint checkPanInvertY()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _PanInvert);
}
uint checkTiltInvertZ()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _TiltInvert);
}

float2 getSectorCoordinates(float x, float y, uint sector)
{
    // say we were on sector 6
    // we need to move over 2 sectors
    // and we need to move up 3 sectors

    //1 sector is every 13 channels
    //the grid is 26x26 aka 2 sectors per row
        float ymod = 0;
        float xmod = 0;
        float originalx = x;
        float originaly = y;

        //TRAVERSING THE Y AXIS OF THE OSC GRID
        ymod = floor(sector / 2);       

        //TRAVERSING THE X AXIS OF THE OSC GRID
        xmod = sector % 2;

        //x += (xmod * 0.052);
        //0.498573
        //0.036343
        x+= (xmod * 0.498573);
        y+= (ymod * 0.03846);
        //y += (ymod * 0.006);
        //originaly = originaly + (0.0147 * sector);
        originaly = IF(sector == 0, originaly, originaly + (0.0147 * (sector+1)));

        return IF(_EnableCompatibilityMode == 1, 
        float2 (x, y), 
        float2(originalx, originaly));
        // if(_EnableCompatibilityMode == 1)
        // {
        //     return float2(x,y);
        // }
        // else
        // {
        //     return float2(originalx, originaly);
        // }

}



//function for getting the value on the OSC Grid in the bottom right corner configuration
float getValueAtCoords(float x, float y, uint sector, sampler2D _Tex)
{
    float2 recoords = getSectorCoordinates(x, y, sector);
    float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    float3 cRGB = float3(c.r, c.g, c.b);
    float value = LinearRgbToLuminance(cRGB);
    value = LinearToGammaSpaceExact(value);
    
	return value;
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


uint getOSCGoboSelection(uint sector)
{
    uint value = IF(_EnableCompatibilityMode == 1,
    round(((getValueAtCoords(0.442291,compatSampleYAxis, sector, _OSCGridRenderTextureRAW))*255)/42.5),
    round(((getValueAtCoords(0.883659,compatSampleYAxis, sector, _OSCGridRenderTextureRAW))*255)/42.5));
    return value;
}

uint isOSC()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableOSC);
}

uint isStrobe()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableStrobe);
}

uint instancedGOBOSelection()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_ProjectionSelection);
}

float getOffsetX()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_FixtureRotationX);
}

float getOffsetY()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_FixtureBaseRotationY);
}

float getStrobeFreq()

{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_StrobeFreq);
}
float4 getEmissionColor()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
}

float getConeWidth()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_ConeWidth) - 1.5;
}

uint isGOBOSpin()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
}

float getConeLength()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _ConeLength);
}

float getGlobalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity);
}

float getFinalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
}

//function for getting the Fine Pan Value (Channel 2)
float GetFinePanValue(uint sector)
{
    float rawValue = IF(_EnableCompatibilityMode == 1, 
    clamp(getValueAtCoords(0.057691, compatSampleYAxis, sector, _OSCGridRenderTexture),0.0,0.9999), 
    clamp(getValueAtCoords(0.115183, standardSampleYAxis, sector, _OSCGridRenderTexture),0.0,0.9999));
    float result = IF(isOSC() == 1, rawValue/1000, 0.0);
    return result;

}

//function for getting the Fine Tilt Value (Channel 4)
float GetFineTiltValue(uint sector)
{
    float rawValue = IF(_EnableCompatibilityMode == 1,
    clamp(getValueAtCoords(0.134611, compatSampleYAxis, sector, _OSCGridRenderTexture),0.0,0.9999),
    clamp(getValueAtCoords(0.268205, standardSampleYAxis, sector, _OSCGridRenderTexture),0.0,0.9999));
    float result = IF(isOSC() == 1, rawValue/1000, 0.0);
    return result;

}
//function for getting the Strobe Value (Channel 7)
float GetStrobeValue(uint sector)
{
    float rawValue = getValueAtCoords(0.250075, 0.020, sector, _OSCGridRenderTextureRAW);
    float finalValue = IF(rawValue <= 0.35, 0.0, rawValue);
    uint remappedvalue = clamp(floor(finalValue * 255),10,255);
    float frequency = clamp(remappedvalue * 0.0980392156862745, 1.0, 25.0);//hz
    frequency = IF(frequency <= 2.0, 0.0, frequency);
    return frequency;

}

float GetStrobeOutput(uint sector)
{
    float2 recoords = IF(_EnableCompatibilityMode == 1, 
    getSectorCoordinates(compatSampleYAxis + (6.0 * 0.03846), compatSampleYAxis, sector),
    getSectorCoordinates(0.498959, standardSampleYAxis, sector)); // this is important i think: 0.498959
    float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
    float4 c = tex2Dlod(_OSCGridStrobeTimer, uvcoords);
    half freq = c.r;
    half multiplier = 4.0;
    half strobe = IF(sin(freq*multiplier) > 0.0, 1, 0);
    strobe = IF(freq <= 000.1, 1, strobe);
    //half output = IF(freq < 370000.0, 1.0, strobe);
    strobe = IF(isOSC() == 1, strobe, 1);
    
    return strobe;

}

float getGoboSpinSpeed (uint sector)
{
    //return getValueAtCoords(0.478864,0.023718, sector, _OSCGridRenderTextureRAW);

    float2 recoords = IF(_EnableCompatibilityMode == 1,
    getSectorCoordinates(0.480751, compatSampleYAxis, sector),
    getSectorCoordinates(0.960572, compatSampleYAxis, sector));
    float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
    //float4 c = tex2Dlod(_OSCGridRenderTexture, uvcoords);
    //float4 c = _OSCGridStrobeTimer.SampleLevel(sampler_point_repeat, uvcoords, 0);
    float4 c = tex2Dlod(_OSCGridStrobeTimer, uvcoords);
    float speed = c.r;
    float actualSpeed = lerp(0,1, (speed/100)-1);
    actualSpeed = actualSpeed/255;
    //actualSpeed = abs(actualSpeed);
    //actualSpeed = actualSpeed * 0.0980392156862745;
    //actualSpeed = IF(actualSpeed < 0.04, 0, actualSpeed);
    return actualSpeed;

}

float CalculateStrobe(uint sector)
{
    float value = GetStrobeValue(sector);
    uint remappedvalue = clamp(floor(value * 255),10,255);
    float frequency = clamp(remappedvalue * 0.0980392156862745, 1.0, 25.0); //hz
    //float finalValue = sign(sin(frequency * _Time.y));
    float finalValue = sign(sin(frequency));
    finalValue = IF(value == 0.0 , 1.0, finalValue);
    return finalValue;
}

//function for getting the Intensity Value (Channel 6)
float GetOSCIntensity(uint sector, float multiplier)
{
    //float value = getValueAtCoords(0.9177, 0.004, sector, _OSCGridRenderTexture);
    //float value = getValueAtCoords(0.208301, 0.02471, sector, _OSCGridRenderTexture);
    float value = 0.0;
//      = IF(_EnableCompatibilityMode == 1, 
// ,

    if(_EnableCompatibilityMode == 1)
    {
        value = IF(_UseRawGrid == 1, //compatible mode
        getValueAtCoords(0.211531, compatSampleYAxis, sector, _OSCGridRenderTextureRAW), 
        getValueAtCoords(0.208301, 0.02471, sector, _OSCGridRenderTexture))
    }
    else
    {
        value = IF(_UseRawGrid == 1, //non compatible mode
        getValueAtCoords(0.423595, standardSampleYAxis, sector, _OSCGridRenderTextureRAW), 
        getValueAtCoords(0.423595, standardSampleYAxis, sector, _OSCGridRenderTexture));
    }

    return value * multiplier;
}

//function for getting the Pan Value (Channel 1)
float GetPanValue(uint sector)
{
    //float rawValue = getValueAtCoords(0.898, 0.004, sector);
    float inputValue = IF(_EnableCompatibilityMode == 1, 
    getValueAtCoords(compatSampleYAxis, compatSampleYAxis, sector, _OSCGridRenderTexture), 
    getValueAtCoords(0.038017, standardSampleYAxis, sector, _OSCGridRenderTexture));
    inputValue += GetFinePanValue(sector);
    return IF(isOSC() == 1, ((_MaxMinPanAngle * 2) * (inputValue)) - _MaxMinPanAngle, 0.0);
}



//function for getting the Tilt Value (Channel 3)
float GetTiltValue(uint sector)
{
    //float rawValue =  getValueAtCoords(0.906,0.004, sector);
    // float xmod = 1 * blockTraversal;
    // float xDestination = blockCenter + xmod;
    float inputValue =  IF(_EnableCompatibilityMode == 1,
    getValueAtCoords(0.096151, compatSampleYAxis, sector, _OSCGridRenderTexture),
    getValueAtCoords(0.189936, standardSampleYAxis, sector, _OSCGridRenderTexture));
    inputValue += GetFineTiltValue(sector);
    return IF(isOSC() == 1, ((_MaxMinTiltAngle * 2) * (inputValue)) - _MaxMinTiltAngle, 0.0);
     
}

//Function for getting the RGB Color Value (Channels 4, 5, and 6)
float4 GetOSCColor(uint sector)
{
    // float redchannel = getValueAtCoords(0.925,0.004,sector, _OSCGridRenderTexture);
    // float greenchannel = getValueAtCoords(0.929,0.004,sector, _OSCGridRenderTexture);
    // float bluechannel = getValueAtCoords(0.934,0.004,sector, _OSCGridRenderTexture);

    // float redchannel = getValueAtCoords(0.288911,0.023718,sector, _OSCGridRenderTexture);
    // float greenchannel = getValueAtCoords(0.326116,0.023718,sector, _OSCGridRenderTexture);
    // float bluechannel = getValueAtCoords(0.362577,0.023718,sector, _OSCGridRenderTexture);
    float redchannel = 0.0;
    float greenchannel = 0.0;
    float bluechannel = 0.0;
    if(_EnableCompatibilityMode == 1)
    {
        redchannel = IF(_UseRawGrid == 1, getValueAtCoords(0.296911,compatSampleYAxis,sector, _OSCGridRenderTextureRAW), getValueAtCoords(0.288911,0.023718,sector, _OSCGridRenderTexture));

        greenchannel = IF(_UseRawGrid == 1, getValueAtCoords(0.326911,compatSampleYAxis,sector, _OSCGridRenderTextureRAW), getValueAtCoords(0.326116,0.023718,sector, _OSCGridRenderTexture));

        bluechannel = IF(_UseRawGrid == 1, getValueAtCoords(0.365371,compatSampleYAxis,sector, _OSCGridRenderTextureRAW), getValueAtCoords(0.362577,0.023718,sector, _OSCGridRenderTexture));

    }
    else
    {
        redchannel = IF(_UseRawGrid == 1, getValueAtCoords(0.577023,standardSampleYAxis,sector, _OSCGridRenderTextureRAW), getValueAtCoords(0.577023,standardSampleYAxis,sector, _OSCGridRenderTexture));

        greenchannel = IF(_UseRawGrid == 1, getValueAtCoords(0.653579,standardSampleYAxis,sector, _OSCGridRenderTextureRAW), getValueAtCoords(0.653579,standardSampleYAxis,sector, _OSCGridRenderTexture));

        bluechannel = IF(_UseRawGrid == 1, getValueAtCoords(0.731426,standardSampleYAxis,sector, _OSCGridRenderTextureRAW), getValueAtCoords(0.731426,standardSampleYAxis,sector, _OSCGridRenderTexture));
    }
    // float redchannel = IF(, 
    // (,
    // );


    // float greenchannel = IF(_EnableCompatibilityMode == 1, 
    // ,
    // );


    // float bluechannel = IF(_EnableCompatibilityMode == 1, 



    #if defined(PROJECTION_YES)
        redchannel = redchannel * _RedMultiplier;
        bluechannel = bluechannel * _BlueMultiplier;
        greenchannel = greenchannel * _GreenMultiplier;
    #endif


    return IF(isOSC() == 1,lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetOSCIntensity(sector, _FixtureMaxIntensity)), float4(redchannel,greenchannel,bluechannel,1) * GetOSCIntensity(sector, _FixtureMaxIntensity));
}

float getOSCConeWidth(uint sector) //Motor Speed Channel// CHANNEL 5
{
    float inputvalue = IF(_EnableCompatibilityMode == 1, 
    getValueAtCoords(0.173071,compatSampleYAxis,sector, _OSCGridRenderTexture),
    getValueAtCoords(0.344893,standardSampleYAxis,sector, _OSCGridRenderTexture));
    float OSCWidth = lerp(0, 5.5, inputvalue) - 1.5;
    return IF(isOSC() == 1, OSCWidth, getConeWidth());


}