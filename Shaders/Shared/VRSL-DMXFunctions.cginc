#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

uint getDMXChannel()
{
    return (uint) round(UNITY_ACCESS_INSTANCED_PROP(Props, _DMXChannel));  
}

uint checkPanInvertY()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _PanInvert);
}
uint checkTiltInvertZ()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _TiltInvert);
}

float2 LegacyRead(int channel, int sector)
{
    // say we were on sector 6
    // we need to move over 2 sectors
    // and we need to move up 3 sectors

    //1 sector is every 13 channels
        float x = 0.02000;
        float y = 0.02000;
        //TRAVERSING THE Y AXIS OF THE OSC GRID
        float ymod = floor(sector / 2.0);       

        //TRAVERSING THE X AXIS OF THE OSC GRID
        float xmod = sector % 2.0;

        x+= (xmod * 0.50);
        y+= (ymod * 0.04);

        x+= (channel * 0.04);
        //we are now on the correct
        return float2(x,y);

}

float2 IndustryRead(int x, int y, uint DMXChannel)
{
    
    float resMultiplierX = (_OSCGridRenderTextureRAW_TexelSize.z / 13);
    float2 xyUV = float2(0.0,0.0);
    
    xyUV.x = ((x * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.x);
    xyUV.y = (y * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.y;
    xyUV.y -= 0.001;
    xyUV.x -= 0.015;
   // xyUV.x = DMXChannel == 15 ? xyUV.x + 0.0769 : xyUV.x;
    return xyUV;
}

//function for getting the value on the OSC Grid in the bottom right corner configuration
float getValueAtCoords(uint DMXChannel, sampler2D _Tex)
{
    //DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
    uint x = DMXChannel % 13; // starts at 1 ends at 13
    x = x == 0.0 ? 13.0 : x;
    float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
    y = frac(y)== 0.00000 ? y - 1 : y;
    if(x == 13.0) //for the 13th channel of each sector... Go down a sector for these DMX Channel Ranges...
    {
    
        //I don't know why, but we need this for some reason otherwise the 13th channel gets shifted around improperly.
        //I"m not sure how to express these exception ranges mathematically. Doing so would be much more cleaner though.
        y = DMXChannel >= 90 && DMXChannel <= 404 ? y - 1 : y;
        y = DMXChannel >= 676 && DMXChannel <= 819 ? y - 1 : y;
        y = DMXChannel >= 1339 ? y - 1 : y;
    }

    // y = (y > 6 && y < 31) && x == 13.0 ? y - 1 : y;

    float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0), DMXChannel);
        
    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    float3 cRGB = float3(c.r, c.g, c.b);
    float value = LinearRgbToLuminance(cRGB);
    value = LinearToGammaSpaceExact(value);
    return value;
}

float getValueAtCoordsRaw(uint DMXChannel, sampler2D _Tex)
{
   // DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
    uint x = DMXChannel % 13; // starts at 1 ends at 13
    x = x == 0.0 ? 13.0 : x;
    float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
    y = frac(y)== 0.00000 ? y - 1 : y;

    float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0), DMXChannel);

    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    
	return c.r;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if defined(VOLUMETRIC_YES) || defined(PROJECTION_YES) || defined(FIXTURE_EMIT)
    float getMinMaxPan()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props,_MaxMinPanAngle);
    }
    float getMinMaxTilt()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props,_MaxMinTiltAngle);
    }
#endif

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
    return UNITY_ACCESS_INSTANCED_PROP(Props,_ConeWidth) - 1.0;
}

uint isGOBOSpin()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
}

float getConeLength()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _ConeLength);
}
float getMaxConeLength(uint DMXChannel)
{
    #ifdef VOLUMETRIC_YES
    float mcl = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxConeLength);
    return isOSC() == 1 && _EnableExtraChannels == 1 ? mcl + (getValueAtCoords(DMXChannel+1, _OSCGridRenderTextureRAW) * 4) : mcl;
    #else
    return UNITY_ACCESS_INSTANCED_PROP(Props, _MaxConeLength);
    #endif
}

float getGlobalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity);
}

float getFinalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
}

float GetStrobeOutput(uint DMXChannel)
{
    
    float phase = getValueAtCoordsRaw(DMXChannel + 6, _OSCGridStrobeTimer);
    float status = getValueAtCoords(DMXChannel + 6, _OSCGridRenderTextureRAW);

    half strobe = (sin(phase));//Get sin wave
    strobe = IF(strobe > 0.0, 1.0, 0.0);//turn to square wave
    //strobe = saturate(strobe);

    strobe = IF(status > 0.2, strobe, 1); //minimum channel threshold set
    
    //check if we should even be strobing at all.
    strobe = IF(isOSC() == 1, strobe, 1);
    strobe = IF(isStrobe() == 1, strobe, 1);
    
    return strobe;

}
float GetStrobeOutputFiveCH(uint DMXChannel)
{
    
    float phase = getValueAtCoordsRaw(DMXChannel + 4, _OSCGridStrobeTimer);
    float status = getValueAtCoords(DMXChannel + 4, _OSCGridRenderTextureRAW);

    half strobe = (sin(phase));//Get sin wave
    strobe = IF(strobe > 0.0, 1.0, 0.0);//turn to square wave
    //strobe = saturate(strobe);

    strobe = IF(status > 0.2, strobe, 1); //minimum channel threshold set
    
    //check if we should even be strobing at all.
    strobe = IF(isOSC() == 1, strobe, 1);
    strobe = IF(isStrobe() == 1, strobe, 1);
    
    return strobe;

}
float getOSCGoboSelection(uint DMXChannel)
{
    float goboSelect = 30.0;

    #if defined(PROJECTION_MOVER) || defined (VOLUMETRIC_YES) 
        goboSelect = IF(UNITY_ACCESS_INSTANCED_PROP(Props, _LegacyGoboRange) > 0, 42.5, goboSelect);
    #endif

    uint value = round(((getValueAtCoords(DMXChannel + 11, _OSCGridRenderTextureRAW))*255)/goboSelect);
    value = isOSC() > 0.0 ? value : instancedGOBOSelection();
    return clamp(value, 1, 8) -0.1;
}

float getGoboSpinSpeed (uint DMXChannel)
{
    // float speed = getValueAtCoords(DMXChannel + 10, _OSCGridRenderTextureRAW);
    // speed = speed > 0.5 ? -(speed - 0.5) : speed;
    // speed = abs(speed) < 0.05 ? 0 : speed;
    // return speed * 8;
    #if defined(PROJECTION_YES) || defined(VOLUMETRIC_YES)
        float status = getValueAtCoords(DMXChannel + 10, _OSCGridRenderTextureRAW);
        float phase = getValueAtCoordsRaw(DMXChannel + 10, _OSCGridSpinTimer);
        phase = checkPanInvertY() == 1 ? -phase : phase;
        return status > 0.5 ? -phase : phase;
    #endif
    return 0.0;
}

//function for getting the Intensity Value (Channel 6)
float GetOSCIntensity(uint DMXChannel, float multiplier)
{
    return getValueAtCoords(DMXChannel + 5, _OSCGridRenderTextureRAW) * multiplier;
}

//function for getting the Pan Value (Channel 2)
float GetFinePanValue(uint DMXChannel)
{
    return getValueAtCoords(DMXChannel+1, _OSCGridRenderTexture);
}
float GetPanValue(uint DMXChannel)
{
    float inputValue = getValueAtCoords(DMXChannel, _OSCGridRenderTexture);
    //inputValue = (inputValue + (GetFinePanValue(DMXChannel) * 0.01));
    #if defined(VOLUMETRIC_YES) || defined(PROJECTION_YES) || defined(FIXTURE_EMIT)
        return IF(isOSC() == 1, ((getMinMaxPan() * 2) * (inputValue)) - getMinMaxPan(), 0.0);
    #else
        return IF(isOSC() == 1, ((_MaxMinPanAngle * 2) * (inputValue)) - _MaxMinPanAngle, 0.0);
    #endif
}


float GetFineTiltValue(uint DMXChannel)
{
    return getValueAtCoords(DMXChannel+3, _OSCGridRenderTexture);
}

//function for getting the Tilt Value (Channel 3)
float GetTiltValue(uint DMXChannel)
{
    float inputValue = getValueAtCoords(DMXChannel + 2, _OSCGridRenderTexture);
    //inputValue = (inputValue + (GetFineTiltValue(DMXChannel) * 0.01));
    #if defined(VOLUMETRIC_YES) || defined(PROJECTION_YES) || defined(FIXTURE_EMIT)
        return IF(isOSC() == 1, ((getMinMaxTilt() * 2) * (inputValue)) - getMinMaxTilt(), 0.0);
    #else
        return IF(isOSC() == 1, ((_MaxMinTiltAngle * 2) * (inputValue)) - _MaxMinTiltAngle, 0.0);
    #endif
     
}

//Function for getting the RGB Color Value (Channels 4, 5, and 6)
float4 GetOSCColor(uint DMXChannel)
{
    float redchannel = getValueAtCoords(DMXChannel + 7, _OSCGridRenderTextureRAW);
    float greenchannel = getValueAtCoords(DMXChannel + 8, _OSCGridRenderTextureRAW);
    float bluechannel = getValueAtCoords(DMXChannel + 9, _OSCGridRenderTextureRAW);

    #if defined(PROJECTION_YES)
        redchannel = redchannel * _RedMultiplier;
        bluechannel = bluechannel * _BlueMultiplier;
        greenchannel = greenchannel * _GreenMultiplier;
    #endif


    //return IF(isOSC() == 1,lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetOSCIntensity(DMXChannel, _FixtureMaxIntensity)), float4(redchannel,greenchannel,bluechannel,1) * GetOSCIntensity(DMXChannel, _FixtureMaxIntensity));
    return lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetOSCIntensity(DMXChannel, _FixtureMaxIntensity));
}

float getOSCConeWidth(uint DMXChannel) //Motor Speed Channel// CHANNEL 5
{
    float inputvalue = getValueAtCoords(DMXChannel + 4, _OSCGridRenderTextureRAW);
    float OSCWidth = lerp(0, 5.5, inputvalue) - 1.5;
    return IF(isOSC() == 1, OSCWidth, getConeWidth());


}