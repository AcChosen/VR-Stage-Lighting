//MOVER LIGHT SYSTEM DEFINES
float _BlindingAngleMod;
sampler2D _MainTex; 

#ifndef VRSL_SURFACE
    float4 _MainTex_ST;
// #else
//     float _MaxMinPanAngle;
//     float _MaxMinTiltAngle;
#endif

#ifdef VRSL_DMX
    uint _UseRawGrid, _EnableExtraChannels;
    uniform float4 _Udon_DMXGridRenderTexture_TexelSize;
    sampler2D _Udon_DMXGridRenderTexture, _Udon_DMXGridRenderTextureMovement, _Udon_DMXGridStrobeOutput, _Udon_DMXGridSpinTimer;
    float _SpinSpeed;

    #ifdef FIXTURE_EMIT
        Texture2D   _Udon_VRSL_GI_LightTexture;
        uniform float4  _Udon_VRSL_GI_LightTexture_TexelSize;
        SamplerState    VRSL_PointClampSampler;
        int     _Udon_VRSL_GI_LightCount;
        float _VRSLSpecularStrength;
        float _VRSLGlossiness;
    #endif

#endif
#ifdef VRSL_AUDIOLINK
    uniform float4 _AudioSpectrum_TexelSize; 
    uniform sampler2D _AudioSpectrum;
    sampler2D _SamplingTexture;
#endif

//SamplerState sampler_point_repeat;
int _IsEven;
#if !defined(VOLUMETRIC_YES) && !defined(PROJECTION_YES)
    sampler2D _MetallicGlossMap;
    sampler2D _BumpMap, _InsideConeNormalMap, _SceneAlbedo;
#endif

#ifdef FIXTURE_EMIT
    sampler2D _DecorativeEmissiveMap;
    half _DecorativeEmissiveMapStrength;
    float4 _DecorativeEmissiveMap_ST;
#endif


float4 _Color;
float _Metallic;
float _Glossiness;
float _BumpScale;
float _XOffset, _YOffset, _Fade, _FeatherOffset;
uint _PureEmissiveToggle;
float _RealtimeGIStrength;

float _StripeSplit, _StripeSplit2, _StripeSplit3, _StripeSplit4, _StripeSplit5, _StripeSplit6, _StripeSplit7;
float _StripeSplitScroll;
float _StripeSplitStrength, _StripeSplitStrength2, _StripeSplitStrength3, _StripeSplitStrength4, _StripeSplitStrength5, _StripeSplitStrength6, _StripeSplitStrength7;
float4 _FixtureLensOrigin;
//float _ProjectionNormalBlur;

float4x4 _viewToWorld;
float _MinimumBeamRadius;


#if defined(VOLUMETRIC_YES)

    #ifdef _HQ_MODE
        sampler2D _NoiseTexHigh;
    #else
        sampler2D _NoiseTex;
    #endif

    float _Noise2StretchInside;
    float _Noise2Stretch;
    float _Noise2X;
    float _Noise2Y;
    float _Noise2Z;
    float _Noise2Power;

    float _Noise2StretchInsideDefault;
    float _Noise2StretchDefault;
    float _Noise2XDefault;
    float _Noise2YDefault;
    float _Noise2ZDefault;
    float _Noise2PowerDefault;

    float _Noise2StretchInsidePotato;
    float _Noise2StretchPotato;
    float _Noise2XPotato;
    float _Noise2YPotato;
    float _Noise2ZPotato;
    float _Noise2PowerPotato;
    
    
#endif

    #ifdef _HQ_MODE
        float4 _NoiseTexHigh_ST;
    #else
        float4 _NoiseTex_ST;
    #endif

float _NoisePower, _NoiseSeed;
uint _ToggleMagicNoise;


float _SpecularLMOcclusion;
float _SpecLMOcclusionAdjust;
float _TriplanarFalloff;
float _LMStrength;
float _RTLMStrength;
int _TextureSampleMode;
int _LightProbeMethod;
half _Saturation, _SaturationLength, _LensMaxBrightness, _UniversalIntensity;
uint _EnableCompatibilityMode, _EnableVerticalMode;
uint _GoboBeamSplitEnable;


uniform const half compatSampleYAxis = 0.019231;
uniform const half standardSampleYAxis = 0.00762;
//float _FixtureRotationX;
//float _FixtureBaseRotationY;
float4 _FixtureRotationOrigin;
float _FixtureMaxIntensity;
//float _MaxMinPanAngle;
//float _MaxMinTiltAngle;
float _ProjectionIntensity;
float _ProjectionRange;
float4 _ProjectionRangeOrigin;
float _ProjectionFade, _ProjectionFadeCurve, _ProjectionDistanceFallOff;
float _AlphaProjectionIntensity;

//float _FinalStrobeFreq, _NewTimer;

// int _EnableDMX;
// int _EnableStrobe;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
uniform float4 _CameraDepthTexture_TexelSize;
sampler2D _LightMainTex, _ProjectionMainTex;
float4 _LightMainTex_ST;
float _ProjectionUVMod, _UseWorldNorm, _ProjectionRotation, _ProjectionUVMod2, _ProjectionUVMod3, _ProjectionUVMod4, _ProjectionUVMod5, _ProjectionUVMod6, _ProjectionUVMod7, _ProjectionUVMod8;
//half _ProjectionSelection;
float4 _ProjectionMainTex_ST;
float _ModX;
float _ModY;
half  _ConeSync, _ProjectionShadowHarshness, _BlindingStrength;
//float _StrobeFreq;

float _PulseSpeed, _BlendSrc, _BlendDst, _BlendOp;
float _FadeStrength, _FadeAmt, _DistFade, _ProjectionMaxIntensity, _IntensityCutoff;
float _InnerFadeStrength, _InnerIntensityCurve, _FixutreIntensityMultiplier;
half _RedMultiplier, _GreenMultiplier, _BlueMultiplier;

int _EnableStaticEmissionColor;
float4 _StaticEmission;
half _ProjectionCutoff, _ProjectionOriginCutoff, _GradientMod, _GradientModGOBO;
half _ClippingThreshold, _RenderTextureMultiplier;
//Instanced Properties

UNITY_INSTANCING_BUFFER_START(Props)
    #ifdef VRSL_DMX
        UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
        UNITY_DEFINE_INSTANCED_PROP(uint, _NineUniverseMode)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableDMX)
        UNITY_DEFINE_INSTANCED_PROP(uint, _LegacyGoboRange)
    #endif
    #ifdef VRSL_AUDIOLINK
        UNITY_DEFINE_INSTANCED_PROP(float, _EnableAudioLink)
        UNITY_DEFINE_INSTANCED_PROP(float, _EnableColorChord)
        UNITY_DEFINE_INSTANCED_PROP(float, _NumBands)
        UNITY_DEFINE_INSTANCED_PROP(float, _Band)
        UNITY_DEFINE_INSTANCED_PROP(float, _BandMultiplier)
        UNITY_DEFINE_INSTANCED_PROP(float, _Delay)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
        UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleX)
        UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleY)
        UNITY_DEFINE_INSTANCED_PROP(float, _SpinSpeed)
        UNITY_DEFINE_INSTANCED_PROP(float, _ThemeColorTarget)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableThemeColorSampling)
    #endif
    UNITY_DEFINE_INSTANCED_PROP(uint, _PanInvert)
    UNITY_DEFINE_INSTANCED_PROP(uint, _TiltInvert)
    UNITY_DEFINE_INSTANCED_PROP(uint, _EnableStrobe)
    UNITY_DEFINE_INSTANCED_PROP(uint, _EnableSpin)
    UNITY_DEFINE_INSTANCED_PROP(float, _StrobeFreq)
    UNITY_DEFINE_INSTANCED_PROP(float, _FixtureRotationX)
    UNITY_DEFINE_INSTANCED_PROP(float, _FixtureBaseRotationY)
    UNITY_DEFINE_INSTANCED_PROP(uint, _ProjectionSelection)
    UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
    UNITY_DEFINE_INSTANCED_PROP(float, _ConeWidth)
    UNITY_DEFINE_INSTANCED_PROP(float, _ConeLength)
    UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensityBlend)
    UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxConeLength)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxMinPanAngle)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxMinTiltAngle)
UNITY_INSTANCING_BUFFER_END(Props)

