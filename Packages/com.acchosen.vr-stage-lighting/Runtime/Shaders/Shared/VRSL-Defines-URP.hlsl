#ifndef VRSL_DEFINES
#define VRSL_DEFINES

#if defined(UNIVERSAL_RENDER_PIPELINE)

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//MOVER LIGHT SYSTEM DEFINES
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_ST;

half _BlindingAngleMod;

#ifdef VRSL_DMX
    uint _UseRawGrid, _EnableExtraChannels;
    uniform half4 _Udon_DMXGridRenderTexture_TexelSize;
    TEXTURE2D(_Udon_DMXGridRenderTexture);
    SAMPLER(sampler_Udon_DMXGridRenderTexture);
    TEXTURE2D(_Udon_DMXGridRenderTextureMovement);
    SAMPLER(sampler_Udon_DMXGridRenderTextureMovement);
    TEXTURE2D(_Udon_DMXGridStrobeOutput);
    SAMPLER(sampler_Udon_DMXGridStrobeOutput);
    TEXTURE2D(_Udon_DMXGridSpinTimer);
    SAMPLER(sampler_Udon_DMXGridSpinTimer);
    half _SpinSpeed;

#ifdef FIXTURE_EMIT
            TEXTURE2D(_Udon_VRSL_GI_LightTexture);
            uniform half4 _Udon_VRSL_GI_LightTexture_TexelSize;
            SAMPLER(VRSL_PointClampSampler);
            int _Udon_VRSL_GI_LightCount;
            half _VRSLSpecularStrength;
            half _VRSLGlossiness;
#endif
#endif

#ifdef VRSL_AUDIOLINK
    uniform half4 _AudioSpectrum_TexelSize; 
    TEXTURE2D(_AudioSpectrum);
    SAMPLER(sampler_AudioSpectrum);
    TEXTURE2D(_SamplingTexture);
    SAMPLER(sampler_SamplingTexture);
#endif

int _IsEven;
#if !defined(VOLUMETRIC_YES) && !defined(PROJECTION_YES)
TEXTURE2D(_MetallicGlossMap);
SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_InsideConeNormalMap);
SAMPLER(sampler_InsideConeNormalMap);
TEXTURE2D(_SceneAlbedo);
SAMPLER(sampler_SceneAlbedo);
#endif

#if defined(VRSL_SURFACE)
    TEXTURE2D(_EmissionMask);
    SAMPLER(sampler_EmissionMask);
    TEXTURE2D(_NormalMap);
    SAMPLER(sampler_NormalMap);
    TEXTURE2D(_MetallicSmoothness);
    SAMPLER(sampler_MetallicSmoothness);
    float4 _EmissionMask_ST, _NormalMap_ST, _MetallicSmoothness_ST;
#endif

#ifdef FIXTURE_EMIT
    TEXTURE2D(_DecorativeEmissiveMap);
    SAMPLER(sampler_DecorativeEmissiveMap);
    half _DecorativeEmissiveMapStrength;
    float4 _DecorativeEmissiveMap_ST;
#endif

half4 _Color;
half _Metallic;
half _Glossiness;
half _BumpScale;
half _XOffset, _YOffset, _Fade, _FeatherOffset;
uint _PureEmissiveToggle;
half _RealtimeGIStrength;

half _StripeSplit, _StripeSplit2, _StripeSplit3, _StripeSplit4, _StripeSplit5, _StripeSplit6, _StripeSplit7;
half _StripeSplitScroll;
half _StripeSplitStrength, _StripeSplitStrength2, _StripeSplitStrength3, _StripeSplitStrength4, _StripeSplitStrength5,
     _StripeSplitStrength6, _StripeSplitStrength7;
half4 _FixtureLensOrigin;

float4x4 _viewToWorld;
half _MinimumBeamRadius;

#if defined(VOLUMETRIC_YES)
#ifdef _HQ_MODE
            TEXTURE2D(_NoiseTexHigh);
            SAMPLER(sampler_NoiseTexHigh);
#else
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
#endif

    half _Noise2StretchInside;
    half _Noise2Stretch;
    half _Noise2X;
    half _Noise2Y;
    half _Noise2Z;
    half _Noise2Power;

    half _Noise2StretchInsideDefault;
    half _Noise2StretchDefault;
    half _Noise2XDefault;
    half _Noise2YDefault;
    half _Noise2ZDefault;
    half _Noise2PowerDefault;

    half _Noise2StretchInsidePotato;
    half _Noise2StretchPotato;
    half _Noise2XPotato;
    half _Noise2YPotato;
    half _Noise2ZPotato;
    half _Noise2PowerPotato;
#endif

#ifdef _HQ_MODE
    half4 _NoiseTexHigh_ST;
#else
half4 _NoiseTex_ST;
#endif

half _NoisePower, _NoiseSeed;
uint _ToggleMagicNoise;

half _SpecularLMOcclusion;
half _SpecLMOcclusionAdjust;
half _TriplanarFalloff;
half _LMStrength;
half _RTLMStrength;
int _TextureSampleMode;
int _LightProbeMethod;
half _Saturation, _SaturationLength, _LensMaxBrightness, _UniversalIntensity;
uint _EnableCompatibilityMode, _EnableVerticalMode;
uint _GoboBeamSplitEnable;

uniform const half compatSampleYAxis = 0.019231;
uniform const half standardSampleYAxis = 0.00762;
half4 _FixtureRotationOrigin;
half _FixtureMaxIntensity;
half _ProjectionIntensity;
half _ProjectionRange;
half4 _ProjectionRangeOrigin;
half _ProjectionFade, _ProjectionFadeCurve, _ProjectionDistanceFallOff;
half _AlphaProjectionIntensity;

TEXTURE2D_X_FLOAT(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);
uniform half4 _CameraDepthTexture_TexelSize;

TEXTURE2D(_LightMainTex);
SAMPLER(sampler_LightMainTex);
float4 _LightMainTex_ST;

TEXTURE2D(_ProjectionMainTex);
SAMPLER(sampler_ProjectionMainTex);
float4 _ProjectionMainTex_ST;

half _ProjectionUVMod, _UseWorldNorm, _ProjectionRotation, _ProjectionUVMod2, _ProjectionUVMod3, _ProjectionUVMod4,
     _ProjectionUVMod5, _ProjectionUVMod6, _ProjectionUVMod7, _ProjectionUVMod8;
half _ModX;
half _ModY;
half _ConeSync, _ProjectionShadowHarshness, _BlindingStrength;

half _PulseSpeed, _BlendSrc, _BlendDst, _BlendOp;
half _FadeStrength, _FadeAmt, _DistFade, _ProjectionMaxIntensity, _IntensityCutoff;
half _InnerFadeStrength, _InnerIntensityCurve, _FixutreIntensityMultiplier;
half _RedMultiplier, _GreenMultiplier, _BlueMultiplier;

int _EnableStaticEmissionColor;
half4 _StaticEmission;
half _ProjectionCutoff, _ProjectionOriginCutoff, _GradientMod, _GradientModGOBO;
half _ClippingThreshold, _RenderTextureMultiplier;

// Instanced Properties
UNITY_INSTANCING_BUFFER_START(Props)
    #ifdef VRSL_DMX
        UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
        UNITY_DEFINE_INSTANCED_PROP(uint, _NineUniverseMode)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableDMX)
        UNITY_DEFINE_INSTANCED_PROP(uint, _LegacyGoboRange)
    #endif
    #ifdef VRSL_AUDIOLINK
        UNITY_DEFINE_INSTANCED_PROP(half, _EnableAudioLink)
        UNITY_DEFINE_INSTANCED_PROP(half, _EnableColorChord)
        UNITY_DEFINE_INSTANCED_PROP(half, _NumBands)
        UNITY_DEFINE_INSTANCED_PROP(half, _Band)
        UNITY_DEFINE_INSTANCED_PROP(half, _BandMultiplier)
        UNITY_DEFINE_INSTANCED_PROP(half, _Delay)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
        UNITY_DEFINE_INSTANCED_PROP(half, _TextureColorSampleX)
        UNITY_DEFINE_INSTANCED_PROP(half, _TextureColorSampleY)
        UNITY_DEFINE_INSTANCED_PROP(half, _SpinSpeed)
        UNITY_DEFINE_INSTANCED_PROP(half, _ThemeColorTarget)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableThemeColorSampling)
        UNITY_DEFINE_INSTANCED_PROP(uint, _UseTraditionalSampling)
    #endif
    #ifdef VRSL_SURFACE
        UNITY_DEFINE_INSTANCED_PROP(half, _CurveMod)
    #endif
    UNITY_DEFINE_INSTANCED_PROP(uint, _PanInvert)
    UNITY_DEFINE_INSTANCED_PROP(uint, _TiltInvert)
    UNITY_DEFINE_INSTANCED_PROP(uint, _EnableStrobe)
    UNITY_DEFINE_INSTANCED_PROP(uint, _EnableSpin)
    UNITY_DEFINE_INSTANCED_PROP(half, _StrobeFreq)
    UNITY_DEFINE_INSTANCED_PROP(half, _FixtureRotationX)
    UNITY_DEFINE_INSTANCED_PROP(half, _FixtureBaseRotationY)
    UNITY_DEFINE_INSTANCED_PROP(uint, _ProjectionSelection)
    UNITY_DEFINE_INSTANCED_PROP(half4, _Emission)
    UNITY_DEFINE_INSTANCED_PROP(half, _ConeWidth)
    UNITY_DEFINE_INSTANCED_PROP(half, _ConeLength)
    UNITY_DEFINE_INSTANCED_PROP(half, _GlobalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(half, _GlobalIntensityBlend)
    UNITY_DEFINE_INSTANCED_PROP(half, _FinalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(half, _MaxConeLength)
    UNITY_DEFINE_INSTANCED_PROP(half, _MaxMinPanAngle)
    UNITY_DEFINE_INSTANCED_PROP(half, _MaxMinTiltAngle)
UNITY_INSTANCING_BUFFER_END(Props)

#endif

#endif
