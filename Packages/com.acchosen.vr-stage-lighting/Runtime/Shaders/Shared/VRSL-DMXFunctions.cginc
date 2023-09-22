#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

uint getDMXChannel()
{
    return (uint) round(UNITY_ACCESS_INSTANCED_PROP(Props, _DMXChannel));  
}

uint getNineUniverseMode()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _NineUniverseMode);
}

#ifndef LASER
uint checkPanInvertY()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _PanInvert);
}
uint checkTiltInvertZ()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _TiltInvert);
}
#endif

float2 LegacyRead(int channel, int sector)
{
    // say we were on sector 6
    // we need to move over 2 sectors
    // and we need to move up 3 sectors

    //1 sector is every 13 channels
        float x = 0.02000;
        float y = 0.02000;
        //TRAVERSING THE Y AXIS OF THE DMX GRID
        float ymod = floor(sector / 2.0);       

        //TRAVERSING THE X AXIS OF THE DMX GRID
        float xmod = sector % 2.0;

        x+= (xmod * 0.50);
        y+= (ymod * 0.04);
        y-= sector >= 23 ? 0.025 : 0.0;
        x+= (channel * 0.04);
        x-= sector >= 40 ? 0.01 : 0.0;
        //we are now on the correct
        return float2(x,y);

}

float2 IndustryRead(int x, int y)
{
    
    float resMultiplierX = (_Udon_DMXGridRenderTexture_TexelSize.z / 13);
    float2 xyUV = float2(0.0,0.0);
    
    xyUV.x = ((x * resMultiplierX) * _Udon_DMXGridRenderTexture_TexelSize.x);
    xyUV.y = (y * resMultiplierX) * _Udon_DMXGridRenderTexture_TexelSize.y;
    xyUV.y -= 0.001915;
    xyUV.x -= 0.015;
   // xyUV.x = DMXChannel == 15 ? xyUV.x + 0.0769 : xyUV.x;
    return xyUV;
}
int getTargetRGBValue(uint universe)
{
    universe -=1;
    return floor((int)(universe / 3));
    //returns 0 for red, 1 for green, 2, for blue
}

//function for getting the value on the DMX Grid in the bottom right corner configuration
float getValueAtCoords(uint DMXChannel, sampler2D _Tex)
{
    uint universe = ceil(((int) DMXChannel)/512.0);
    int targetColor = getTargetRGBValue(universe);
    
    //DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
    universe-=1;
    DMXChannel = targetColor > 0 ? DMXChannel - (((universe - (universe % 3)) * 512)) - (targetColor * 24) : DMXChannel;

    uint x = DMXChannel % 13; // starts at 1 ends at 13
    x = x == 0.0 ? 13.0 : x;
    float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
    y = frac(y)== 0.00000 ? y - 1 : y;
    if(x == 13.0) //for the 13th channel of each sector... Go down a sector for these DMX Channel Ranges...
    {
    
        //I don't know why, but we need this for some reason otherwise the 13th channel gets shifted around improperly.
        //I"m not sure how to express these exception ranges mathematically. Doing so would be much more cleaner though.
        y = DMXChannel >= 90 && DMXChannel <= 101 ? y - 1 : y;
        y = DMXChannel >= 160 && DMXChannel <= 205 ? y - 1 : y;
        y = DMXChannel >= 326 && DMXChannel <= 404 ? y - 1 : y;
        y = DMXChannel >= 676 && DMXChannel <= 819 ? y - 1 : y;
        y = DMXChannel >= 1339 ? y - 1 : y;
    }

    // y = (y > 6 && y < 31) && x == 13.0 ? y - 1 : y;
    
    float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0));
        
    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    float value = 0.0;
    
   if(getNineUniverseMode() && _EnableCompatibilityMode != 1)
   {
    value = c.r;
    value = IF(targetColor > 0, c.g, value);
    value = IF(targetColor > 1, c.b, value);
   }
   else
   {
        float3 cRGB = float3(c.r, c.g, c.b);
        value = LinearRgbToLuminance(cRGB);
    }
    value = LinearToGammaSpaceExact(value);
    return value;
}

float getValueAtCoordsRaw(uint DMXChannel, sampler2D _Tex)
{
   // DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
    uint universe = ceil(((int) DMXChannel)/512.0);
    int targetColor = getTargetRGBValue(universe);

    universe-=1;
    DMXChannel = targetColor > 0 ? DMXChannel - (((universe - (universe % 3)) * 512)) - (targetColor * 24) : DMXChannel;


    uint x = DMXChannel % 13; // starts at 1 ends at 13
    x = x == 0.0 ? 13.0 : x;
    float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
    y = frac(y)== 0.00000 ? y - 1 : y;
    
    float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0));

    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    float value = c.r;
    value = IF(targetColor > 0, c.g, value);
    value = IF(targetColor > 1, c.b, value);
	return value;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if defined(VOLUMETRIC_YES) || defined(PROJECTION_YES) || defined(FIXTURE_EMIT) || defined(FIXTURE_SHADOWCAST) || defined(VRSL_SURFACE) || defined(VRSL_FLARE)
    float getMinMaxPan()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props,_MaxMinPanAngle);
    }
    float getMinMaxTilt()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props,_MaxMinTiltAngle);
    }
#endif

uint isDMX()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableDMX);
}
#ifndef LASER
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
#endif
float4 getEmissionColor()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
}
#ifndef LASER
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
    return isDMX() == 1 && _EnableExtraChannels == 1 ? mcl + (getValueAtCoords(DMXChannel+1, _Udon_DMXGridRenderTexture) * 4) : mcl;
    #else
    return UNITY_ACCESS_INSTANCED_PROP(Props, _MaxConeLength);
    #endif
}
#endif
float getGlobalIntensity()
{
    return lerp(1.0,UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity), UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensityBlend));
}

float getFinalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
}
#ifndef LASER
float GetStrobeOutput(uint DMXChannel)
{
    
    // float phase = getValueAtCoordsRaw(DMXChannel + 6, _Udon_DMXGridStrobeTimer);
    // float status = getValueAtCoords(DMXChannel + 6, _Udon_DMXGridRenderTexture);

    half strobe = getValueAtCoords(DMXChannel + 6, _Udon_DMXGridStrobeOutput);
    // half strobe = (sin(phase));//Get sin wave
    // strobe = IF(strobe > 0.0, 1.0, 0.0);//turn to square wave
    //strobe = saturate(strobe);

    // strobe = IF(status > 0.2, strobe, 1); //minimum channel threshold set
    
    //check if we should even be strobing at all.
    strobe = IF(isDMX() == 1, strobe, 1);
    strobe = IF(isStrobe() == 1, strobe, 1);
    
    return strobe;

}
float GetStrobeOutputFiveCH(uint DMXChannel)
{
    
    // float phase = getValueAtCoordsRaw(DMXChannel + 4, _Udon_DMXGridStrobeTimer);
    // float status = getValueAtCoords(DMXChannel + 4, _Udon_DMXGridRenderTexture);

    half strobe = getValueAtCoords(DMXChannel + 4, _Udon_DMXGridStrobeOutput);
    // strobe = IF(strobe > 0.0, 1.0, 0.0);//turn to square wave
    //strobe = saturate(strobe);

    // strobe = IF(status > 0.2, strobe, 1); //minimum channel threshold set
    
    //check if we should even be strobing at all.
    strobe = IF(isDMX() == 1, strobe, 1);
    strobe = IF(isStrobe() == 1, strobe, 1);
    
    return strobe;

}
float getDMXGoboSelection(uint DMXChannel)
{
    float goboSelect = 30.0;

    #if defined(PROJECTION_MOVER) || defined (VOLUMETRIC_YES) 
        goboSelect = IF(UNITY_ACCESS_INSTANCED_PROP(Props, _LegacyGoboRange) > 0, 42.5, goboSelect);
    #endif

    uint value = round(((getValueAtCoords(DMXChannel + 11, _Udon_DMXGridRenderTexture))*255)/goboSelect);
    value = isDMX() > 0.0 ? value : instancedGOBOSelection();
    return clamp(value, 1, 8) -0.1;
}

float getGoboSpinSpeed (uint DMXChannel)
{
    #if defined(PROJECTION_YES) || defined(VOLUMETRIC_YES)
        float status = getValueAtCoords(DMXChannel + 10, _Udon_DMXGridRenderTexture);
        float phase = getValueAtCoordsRaw(DMXChannel + 10, _Udon_DMXGridSpinTimer);
        phase = checkPanInvertY() == 1 ? -phase : phase;
        return status > 0.5 ? -phase : phase;
    #endif
    return 0.0;
}

//function for getting the Intensity Value (Channel 6)
float GetDMXIntensity(uint DMXChannel, float multiplier)
{
    return getValueAtCoords(DMXChannel + 5, _Udon_DMXGridRenderTexture) * multiplier;
}

float GetDMXChannel(uint DMXChannel)
{
    return getValueAtCoords(DMXChannel, _Udon_DMXGridRenderTexture);
}

//function for getting the Pan Value (Channel 2)
float GetFinePanValue(uint DMXChannel)
{
    return getValueAtCoords(DMXChannel+1, _Udon_DMXGridRenderTextureMovement);
}
float GetPanValue(uint DMXChannel)
{
    float inputValue = getValueAtCoords(DMXChannel, _Udon_DMXGridRenderTextureMovement);
    //inputValue = (inputValue + (GetFinePanValue(DMXChannel) * 0.01));
    #if defined(VOLUMETRIC_YES) || defined(PROJECTION_YES) || defined(FIXTURE_EMIT) || defined(FIXTURE_SHADOWCAST) || defined(VRSL_SURFACE) || defined(VRSL_FLARE)
        return IF(isDMX() == 1, ((getMinMaxPan() * 2) * (inputValue)) - getMinMaxPan(), 0.0);
    #else
        return IF(isDMX() == 1, ((_MaxMinPanAngle * 2) * (inputValue)) - _MaxMinPanAngle, 0.0);
    #endif
}


float GetFineTiltValue(uint DMXChannel)
{
    return getValueAtCoords(DMXChannel+3, _Udon_DMXGridRenderTextureMovement);
}

//function for getting the Tilt Value (Channel 3)
float GetTiltValue(uint DMXChannel)
{
    float inputValue = getValueAtCoords(DMXChannel + 2, _Udon_DMXGridRenderTextureMovement);
    //inputValue = (inputValue + (GetFineTiltValue(DMXChannel) * 0.01));
    #if defined(VOLUMETRIC_YES) || defined(PROJECTION_YES) || defined(FIXTURE_EMIT) || defined(FIXTURE_SHADOWCAST) || defined(VRSL_SURFACE) || defined(VRSL_FLARE)
        return IF(isDMX() == 1, ((getMinMaxTilt() * 2) * (inputValue)) - getMinMaxTilt(), 0.0);
    #else
        return IF(isDMX() == 1, ((_MaxMinTiltAngle * 2) * (inputValue)) - _MaxMinTiltAngle, 0.0);
    #endif
     
}

//Function for getting the RGB Color Value (Channels 4, 5, and 6)
float4 GetDMXColor(uint DMXChannel)
{
    float redchannel = getValueAtCoords(DMXChannel + 7, _Udon_DMXGridRenderTexture);
    float greenchannel = getValueAtCoords(DMXChannel + 8, _Udon_DMXGridRenderTexture);
    float bluechannel = getValueAtCoords(DMXChannel + 9, _Udon_DMXGridRenderTexture);

    #if defined(PROJECTION_YES)
        redchannel = redchannel * _RedMultiplier;
        bluechannel = bluechannel * _BlueMultiplier;
        greenchannel = greenchannel * _GreenMultiplier;
    #endif


    //return IF(isDMX() == 1,lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetDMXIntensity(DMXChannel, _FixtureMaxIntensity)), float4(redchannel,greenchannel,bluechannel,1) * GetDMXIntensity(DMXChannel, _FixtureMaxIntensity));
    return lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetDMXIntensity(DMXChannel, _FixtureMaxIntensity));
}

float getDMXConeWidth(uint DMXChannel) //Motor Speed Channel// CHANNEL 5
{
    float inputvalue = getValueAtCoords(DMXChannel + 4, _Udon_DMXGridRenderTexture);
    float DMXWidth = lerp(0, 5.5, inputvalue) - 1.5;
    return IF(isDMX() == 1, DMXWidth, getConeWidth());


}
#endif