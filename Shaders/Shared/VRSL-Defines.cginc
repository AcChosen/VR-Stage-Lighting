//MOVER LIGHT SYSTEM DEFINES
sampler2D _MainTex; float4 _MainTex_ST;
sampler2D _OSCGridRenderTexture, _OSCGridRenderTextureRAW, _OSCGridStrobeTimer, _OSCGridSpinTimer;
//SamplerState sampler_point_repeat;
int _IsEven;
#if !defined(VOLUMETRIC_YES) && !defined(PROJECTION_YES)
    sampler2D _MetallicGlossMap;
    sampler2D _BumpMap, _InsideConeNormalMap, _SceneAlbedo;
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

#if defined(VOLUMETRIC_YES)
    sampler2D _NoiseTex;
    float _Noise2StretchInside;
#endif
float4 _NoiseTex_ST;
float _NoisePower, _NoiseSeed, _Noise2Stretch, _Noise2X, _Noise2Y, _Noise2Z, _Noise2Power;
uint _ToggleMagicNoise;


float _SpecularLMOcclusion;
float _SpecLMOcclusionAdjust;
float _TriplanarFalloff;
float _LMStrength;
float _RTLMStrength;
int _TextureSampleMode;
int _LightProbeMethod;
uint _UseRawGrid, _EnableExtraChannels;
half _Saturation, _SaturationLength, _LensMaxBrightness, _UniversalIntensity;
uint _EnableCompatibilityMode, _EnableVerticalMode;
uint _GoboBeamSplitEnable;

uniform const half compatSampleYAxis = 0.019231;
uniform const half standardSampleYAxis = 0.00762;
uniform float4 _OSCGridRenderTextureRAW_TexelSize;
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

//float _FinalStrobeFreq, _NewTimer;

// int _EnableOSC;
// int _EnableStrobe;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
sampler2D _LightMainTex, _ProjectionMainTex;
float4 _LightMainTex_ST;
float _ProjectionUVMod, _UseWorldNorm, _ProjectionRotation, _SpinSpeed, _ProjectionUVMod2, _ProjectionUVMod3, _ProjectionUVMod4, _ProjectionUVMod5, _ProjectionUVMod6, _ProjectionUVMod7, _ProjectionUVMod8;
//half _ProjectionSelection;
float4 _ProjectionMainTex_ST;
float _ModX;
float _ModY;
half  _ConeSync, _ProjectionShadowHarshness;
//float _StrobeFreq;

float _PulseSpeed, _BlendSrc, _BlendDst, _BlendOp;
float _FadeStrength, _FadeAmt, _DistFade, _ProjectionMaxIntensity, _IntensityCutoff;
float _InnerFadeStrength, _InnerIntensityCurve, _FixutreIntensityMultiplier;
half _RedMultiplier, _GreenMultiplier, _BlueMultiplier;

int _EnableStaticEmissionColor;
float4 _StaticEmission;

//Instanced Properties

UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
    UNITY_DEFINE_INSTANCED_PROP(uint, _PanInvert)
    UNITY_DEFINE_INSTANCED_PROP(uint, _TiltInvert)
    UNITY_DEFINE_INSTANCED_PROP(uint, _EnableOSC)
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
    UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxConeLength)
    UNITY_DEFINE_INSTANCED_PROP(uint, _LegacyGoboRange)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxMinPanAngle)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxMinTiltAngle)
UNITY_INSTANCING_BUFFER_END(Props)

