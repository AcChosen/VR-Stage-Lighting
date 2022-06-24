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

float4 GetDMXEmission(float2 EmissionUV)
{
    uint dmx = getDMXChannel();
    float dmxIntensity = getValueAtCoords(dmx, _OSCGridRenderTextureRAW);
    float strobe = IF(isStrobe() == 1, GetSurfaceStrobe(dmx), 1);
    float4 OSCcol = GetDMXRGB(dmx, dmxIntensity) * getEmissionColor();
    //OSCcol *= GetOSCColor(dmx);
    float4 col = IF(isOSC() == 1, lerp(float4(0,0,0,0), OSCcol * _CurveMod, dmxIntensity), getEmissionColor());
    half4 e = col * strobe;
    // e = IF(isOSC() == 1,lerp(half4(-_CurveMod,-_CurveMod,-_CurveMod,1), e, pow(GetOSCIntensity(dmx, 1.0), 1.0)), e);
    // e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
    e *= tex2D(_EmissionMask, EmissionUV).r;
    //e*= _FixutreIntensityMultiplier;
    float sat = (e.r + e.g + e.b)/3.0;
    e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
    e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
    return e * _UniversalIntensity;
}

#ifdef SURF_ALPHA
float GetDMXAlpha()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableDMXAlpha) == 1 && isOSC() == 1 ? getValueAtCoords((getDMXChannel() + 5), _OSCGridRenderTextureRAW) : 1.0;
}

#endif