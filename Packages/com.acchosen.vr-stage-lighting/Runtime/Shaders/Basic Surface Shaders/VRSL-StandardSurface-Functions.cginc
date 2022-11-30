float GetSurfaceStrobe(uint DMXChannel)
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
float4 GetDMXRGB(uint DMXChannel, float intensity)
{
    float redchannel = getValueAtCoords(DMXChannel + 1, _OSCGridRenderTextureRAW);
    float greenchannel = getValueAtCoords(DMXChannel + 2, _OSCGridRenderTextureRAW);
    float bluechannel = getValueAtCoords(DMXChannel + 3, _OSCGridRenderTextureRAW);

    #if defined(PROJECTION_YES)
        redchannel = redchannel * _RedMultiplier;
        bluechannel = bluechannel * _BlueMultiplier;
        greenchannel = greenchannel * _GreenMultiplier;
    #endif


    //return IF(isOSC() == 1,lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetOSCIntensity(DMXChannel, _FixtureMaxIntensity)), float4(redchannel,greenchannel,bluechannel,1) * GetOSCIntensity(DMXChannel, _FixtureMaxIntensity));
    return lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), intensity);
}


float GetDMX12CH(uint DMXChannel, int dimmerChannel)
{
    int startChannel = (int)DMXChannel + 2 + dimmerChannel;
    float dimmer = getValueAtCoords(startChannel, _OSCGridRenderTextureRAW);
    return dimmer;
    // float redchannel = getValueAtCoords(DMXChannel + 1, _OSCGridRenderTextureRAW);
    // float greenchannel = getValueAtCoords(DMXChannel + 2, _OSCGridRenderTextureRAW);
    // float bluechannel = getValueAtCoords(DMXChannel + 3, _OSCGridRenderTextureRAW);
    // #if defined(PROJECTION_YES)
    //     redchannel = redchannel * _RedMultiplier;
    //     bluechannel = bluechannel * _BlueMultiplier;
    //     greenchannel = greenchannel * _GreenMultiplier;
    // #endif
    //return IF(isOSC() == 1,lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), GetOSCIntensity(DMXChannel, _FixtureMaxIntensity)), float4(redchannel,greenchannel,bluechannel,1) * GetOSCIntensity(DMXChannel, _FixtureMaxIntensity));
    // return lerp(fixed4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), intensity);

}



float4 GetDMXEmission(float2 EmissionUV)
{
    uint dmx = getDMXChannel();
    float dmxIntensity = getValueAtCoords(dmx, _OSCGridRenderTextureRAW);
    float strobe = IF(isStrobe() == 1, GetSurfaceStrobe(dmx), 1);
    float4 OSCcol = GetDMXRGB(dmx, dmxIntensity) * getEmissionColor();
    //OSCcol *= GetOSCColor(dmx);
    float4 col = IF(isOSC() == 1, OSCcol * (_CurveMod), getEmissionColor());
    half4 e = col * strobe;
    // e = IF(isOSC() == 1,lerp(half4(-_CurveMod,-_CurveMod,-_CurveMod,1), e, pow(GetOSCIntensity(dmx, 1.0), 1.0)), e);
    // e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
    e *= tex2D(_EmissionMask, EmissionUV).r;
    //e*= _FixutreIntensityMultiplier;
    float sat = (e.r + e.g + e.b)/3.0;
    e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
    e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
    e *= _UniversalIntensity;
    //dmxIntensity = clamp(dmxIntensity-0.1, 0.0, 1.0);
    dmxIntensity = dmxIntensity * dmxIntensity * dmxIntensity * dmxIntensity;
    return lerp(float4(0,0,0,0),e, dmxIntensity) * (lerp(1,_CurveMod, dmxIntensity));
}
int GetCurrent12CH(float2 uv)
{
    if(uv.y > 0.22 && uv.y < 0.3)
    {
        if(uv.x > 0.027581 && uv.x < 0.079706)
        {
            return 1;
        }
        else if(uv.x > 0.119042 && uv.x < 0.150821)
        {
            return 2;
        }
        else if(uv.x > 0.19694 && uv.x < 0.237864)
        {
            return 3;
        }
        else if(uv.x > 0.27888 && uv.x < 0.318998)
        {
            return 4;
        }
        else if(uv.x > 0.354643 && uv.x < 0.407866)
        {
            return 5;
        }
        else if(uv.x > 0.441313 && uv.x < 0.470366)
        {
            return 6;
        }
        else if(uv.x > 0.522612 && uv.x < 0.559965)
        {
            return 7;
        }
        else if(uv.x > 0.60 && uv.x < 0.68)
        {
            return 8;
        }
        else if(uv.x > 0.69 && uv.x < 0.75)
        {
            return 9;
        }
        else if(uv.x > 0.76 && uv.x < 0.80)
        {
            return 10;
        }
        else if(uv.x > 0.81 && uv.x < 0.92)
        {
            return 11;
        }
        else
        {
            return 12;
        }
    }
    else
    {
        return 0;
    }
}

float4 GetDMXEmission12Ch(float2 EmissionUV)
{
    uint dmx = getDMXChannel();
    //float dmxIntensity = getValueAtCoords(dmx, _OSCGridRenderTextureRAW);
    float dmxIntensity = GetDMX12CH(dmx, GetCurrent12CH(EmissionUV));


    //float strobe = IF(isStrobe() == 1, GetSurfaceStrobe(dmx), 1);
    float4 OSCcol = GetDMXRGB(dmx-1, dmxIntensity) * getEmissionColor();
    //OSCcol *= GetOSCColor(dmx);
    float4 col = IF(isOSC() == 1, OSCcol * _CurveMod, getEmissionColor());
    half4 e = col;
    // e = IF(isOSC() == 1,lerp(half4(-_CurveMod,-_CurveMod,-_CurveMod,1), e, pow(GetOSCIntensity(dmx, 1.0), 1.0)), e);
    // e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
    e *= tex2D(_EmissionMask, EmissionUV).r;
    //e*= _FixutreIntensityMultiplier;
    float sat = (e.r + e.g + e.b)/3.0;
    e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
    e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
    e *= _UniversalIntensity;
    dmxIntensity = dmxIntensity * dmxIntensity * dmxIntensity * dmxIntensity;
    return lerp(float4(0,0,0,0),e, dmxIntensity) * (lerp(1,_CurveMod, dmxIntensity));

}


#ifdef SURF_ALPHA
float GetDMXAlpha()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableDMXAlpha) == 1 && isOSC() == 1 ? getValueAtCoords((getDMXChannel() + 5), _OSCGridRenderTextureRAW) : 1.0;
}

#endif