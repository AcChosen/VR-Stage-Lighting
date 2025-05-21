#ifndef VRSL_STANDARD_SURF_FUNCTIONS
#define VRSL_STANDARD_SURF_FUNCTIONS

#if defined(UNIVERSAL_PIPELINE_CORE_INCLUDED)

#include "../Shared/VRSL-DMXFunctions-URP.hlsl"

float GetSurfaceStrobe(uint DMXChannel)
{
    half strobe = getValueAtCoords(DMXChannel + 4, TEXTURE2D_ARGS(_Udon_DMXGridStrobeOutput, sampler_Udon_DMXGridStrobeOutput));
    
    //check if we should even be strobing at all.
    strobe = IF(isDMX() == 1, strobe, 1);
    strobe = IF(isStrobe() == 1, strobe, 1);
    
    return strobe;
}

float4 GetDMXRGB(uint DMXChannel, float intensity, uint channelOffset)
{
    float redchannel = getValueAtCoords(DMXChannel + 1 + channelOffset, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture));
    float greenchannel = getValueAtCoords(DMXChannel + 2 + channelOffset, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture));
    float bluechannel = getValueAtCoords(DMXChannel + 3 + channelOffset, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture));

    #if defined(PROJECTION_YES)
        redchannel = redchannel * _RedMultiplier;
        bluechannel = bluechannel * _BlueMultiplier;
        greenchannel = greenchannel * _GreenMultiplier;
    #endif
    
    return lerp(float4(0,0,0,1), float4(redchannel,greenchannel,bluechannel,1), intensity);
}

float GetDMXAlpha(uint DMXChannel, float enableDMXControl)
{
    return IF(isDMX() == 1 && enableDMXControl == 1, getValueAtCoords(DMXChannel, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture)), 0.0);
}

float GetDMX12CH(uint DMXChannel, int dimmerChannel)
{
    int startChannel = (int)DMXChannel + 2 + dimmerChannel;
    float dimmer = getValueAtCoords(startChannel, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture));
    return dimmer;
}

float4 GetDMX12CHRGB(uint DMXChannel, int dimmerChannel, out float intensity)
{
    int startChannel = (int)DMXChannel + (dimmerChannel-1);
    intensity = getValueAtCoords(startChannel, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture));
    return GetDMXRGB((uint) startChannel, intensity, 0);
}

#if _1CH_MODE
    float4 GetDMXEmission1CH(float2 EmissionUV)
    {
        uint dmx = getDMXChannel();
        float dmxIntensity = IF(isDMX()==1, getValueAtCoords(dmx, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture)), 1.0);
        float4 DMXcol = getEmissionColor() * dmxIntensity;
        float4 col = IF(isDMX() == 1, DMXcol * (_CurveMod), getEmissionColor());
        float4 e = col;
        #ifndef LENS_FLARE
            e *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, EmissionUV).r;
        #endif
        float sat = (e.r + e.g + e.b)/3.0;
        e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
        e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
        e *= _UniversalIntensity;
        return lerp(float4(0,0,0,0),e, dmxIntensity) * (lerp(1,_CurveMod, dmxIntensity)) * getEmissionColor();
    }
#endif

#if _4CH_MODE
    float4 GetDMXEmission4CH(float2 EmissionUV)
    {
        uint dmx = getDMXChannel();
        float dmxIntensity = IF(isDMX()==1, getValueAtCoords(dmx, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture)), 1.0);
        float4 DMXcol = GetDMXRGB(dmx, dmxIntensity,0) * getEmissionColor();
        float4 col = IF(isDMX() == 1, DMXcol * (_CurveMod), getEmissionColor());
        float4 e = col;
        #ifndef LENS_FLARE
            e *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, EmissionUV).r;
        #endif
        float sat = (e.r + e.g + e.b)/3.0;
        e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
        e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
        e *= _UniversalIntensity;
        return lerp(float4(0,0,0,0),e, dmxIntensity) * (lerp(1,_CurveMod, dmxIntensity)) * getEmissionColor();
    }
#endif

#if _5CH_MODE
    float4 GetDMXEmission5CH(float2 EmissionUV)
    {
        uint dmx = getDMXChannel();
        float dmxIntensity = IF(isDMX()==1, getValueAtCoords(dmx, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture)), 1.0);
        float strobe = IF(isStrobe() == 1, GetSurfaceStrobe(dmx), 1);
        float4 DMXcol = GetDMXRGB(dmx, dmxIntensity,0) * getEmissionColor();
        float4 col = IF(isDMX() == 1, DMXcol * (_CurveMod), getEmissionColor());
        float4 e = col * strobe;
        #ifndef LENS_FLARE
            e *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, EmissionUV).r;
        #endif
        float sat = (e.r + e.g + e.b)/3.0;
        e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
        e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
        e *= _UniversalIntensity;
        return lerp(float4(0,0,0,0),e, dmxIntensity) * (lerp(1,_CurveMod, dmxIntensity)) * getEmissionColor();
    }
#endif

#if _13CH_MODE
    float4 GetDMXEmission13CH(float2 EmissionUV)
    {
        uint dmx = getDMXChannel();
        float dmxIntensity = IF(isDMX()==1, getValueAtCoords(dmx+5, TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture)), 1.0);
        float strobe = IF(isStrobe() == 1, GetSurfaceStrobe(dmx+2), 1);
        float4 DMXcol = GetDMXRGB(dmx, dmxIntensity,6) * getEmissionColor();
        float4 col = IF(isDMX() == 1, DMXcol * (_CurveMod), getEmissionColor());
        float4 e = col * strobe;
        #ifndef LENS_FLARE
            e *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, EmissionUV).r;
        #endif
        float sat = (e.r + e.g + e.b)/3.0;
        e.rgb = lerp(float3(sat,sat,sat), e, _Saturation);
        e = ((e * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
        e *= _UniversalIntensity;
        return lerp(float4(0,0,0,0),e, dmxIntensity) * (lerp(1,_CurveMod, dmxIntensity)) * getEmissionColor();
    }
#endif

int GetCurrent12CH(float2 uv)
{
    if(uv.y > 0.22 && uv.y < 0.3)
    {
        if(uv.x > 0.027581 && uv.x < 0.079706)
        {
            #ifndef _CHANNEL_MODE
            return 1;
            #else
            return 1;
            #endif
        }
        else if(uv.x > 0.119042 && uv.x < 0.150821)
        {
            #ifndef _CHANNEL_MODE
            return 2;
            #else
            return 5;
            #endif
        }
        else if(uv.x > 0.19694 && uv.x < 0.237864)
        {
            #ifndef _CHANNEL_MODE
            return 3;
            #else
            return 9;
            #endif
        }
        else if(uv.x > 0.27888 && uv.x < 0.318998)
        {
            #ifndef _CHANNEL_MODE
            return 4;
            #else
            return 13;
            #endif
        }
        else if(uv.x > 0.354643 && uv.x < 0.407866)
        {
            #ifndef _CHANNEL_MODE
            return 5;
            #else
            return 17;
            #endif
        }
        else if(uv.x > 0.441313 && uv.x < 0.470366)
        {
            #ifndef _CHANNEL_MODE
            return 6;
            #else
            return 21;
            #endif
        }
        else if(uv.x > 0.522612 && uv.x < 0.559965)
        {
            #ifndef _CHANNEL_MODE
            return 7;
            #else
            return 25;
            #endif
        }
        else if(uv.x > 0.60 && uv.x < 0.68)
        {
            #ifndef _CHANNEL_MODE
            return 8;
            #else
            return 29;
            #endif
        }
        else if(uv.x > 0.69 && uv.x < 0.75)
        {
            #ifndef _CHANNEL_MODE
            return 9;
            #else
            return 33;
            #endif
        }
        else if(uv.x > 0.76 && uv.x < 0.80)
        {
            #ifndef _CHANNEL_MODE
            return 10;
            #else
            return 37;
            #endif
        }
        else if(uv.x > 0.81 && uv.x < 0.92)
        {
            #ifndef _CHANNEL_MODE
            return 11;
            #else
            return 41;
            #endif
        }
        else
        {
            #ifndef _CHANNEL_MODE
            return 12;
            #else
            return 45;
            #endif
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
    #ifndef _CHANNEL_MODE
        float dmxIntensity = GetDMX12CH(dmx, GetCurrent12CH(EmissionUV));  
        float4 DMXcol = GetDMXRGB(dmx-1, dmxIntensity,0) * getEmissionColor();
    #else
        float dmxIntensity = 0.0;
        float4 DMXcol = GetDMX12CHRGB(dmx, GetCurrent12CH(EmissionUV), dmxIntensity) * getEmissionColor();
    #endif
    
    float4 col = IF(isDMX() == 1, DMXcol * _CurveMod, getEmissionColor());
    half4 e = col;
    
    #ifndef LENS_FLARE
        e *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, EmissionUV).r;
    #endif
    
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
    return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableDMXAlpha) == 1 && isDMX() == 1 ? 
        getValueAtCoords((getDMXChannel() + 5), TEXTURE2D_ARGS(_Udon_DMXGridRenderTexture, sampler_Udon_DMXGridRenderTexture)) : 1.0;
}
#endif

#endif
#endif