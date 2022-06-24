//MOVER LIGHT SYSTEM DEFINES
sampler2D _MainTex;
sampler2D _OSCGridRenderTexture, _OSCGridRenderTextureRAW, _OSCGridStrobeTimer;
//SamplerState sampler_point_repeat;
int _IsEven;
sampler2D _MetallicGlossMap;
sampler2D _BumpMap, _InsideConeNormalMap, _SceneAlbedo;
float4 _Color;
float _Metallic;
float _Glossiness;
float _BumpScale;
float _XOffset, _YOffset, _Fade, _FeatherOffset;
uint _PureEmissiveToggle;
float _RealtimeGIStrength;

float _Divide;
float _DivideScroll;
float _DividePower;
//float _ProjectionNormalBlur;

float4x4 _viewToWorld;

sampler2D _NoiseTex;
float4 _NoiseTex_ST;
float _NoisePower, _NoiseSeed;


float _SpecularLMOcclusion;
float _SpecLMOcclusionAdjust;
float _TriplanarFalloff;
float _LMStrength;
float _RTLMStrength;
int _TextureSampleMode;
int _LightProbeMethod;
uint _UseRawGrid;
half _Saturation, _SaturationLength, _LensMaxBrightness, _UniversalIntensity;

//float _FixtureRotationX;
//float _FixtureBaseRotationY;
float4 _FixtureRotationOrigin;
float _FixtureMaxIntensity;
float _MaxMinPanAngle;
float _MaxMinTiltAngle;
float _ProjectionIntensity;
float _ProjectionRange;
float4 _ProjectionRangeOrigin;
float _ProjectionFade, _ProjectionFadeCurve, _ProjectionDistanceFallOff;

uniform const half compatSampleYAxis = 0.019231;
uniform const half standardSampleYAxis = 0.00762;
uniform float4 _OSCGridRenderTextureRAW_TexelSize;

//float _FinalStrobeFreq, _NewTimer;

// int _EnableOSC;
// int _EnableStrobe;
uint _EnableCompatibilityMode, _EnableVerticalMode;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

sampler2D _LightMainTex, _ProjectionMainTex;
float4 _LightMainTex_ST;
float _ProjectionUVMod, _UseWorldNorm, _ProjectionRotation, _SpinSpeed, _ProjectionUVMod2, _ProjectionUVMod3, _ProjectionUVMod4, _ProjectionUVMod5, _ProjectionUVMod6;
//half _ProjectionSelection;
float4 _ProjectionMainTex_ST;
float _ModX;
float _ModY;
half  _ConeSync, _ProjectionShadowHarshness;
//float _StrobeFreq;

float _PulseSpeed, _BlendSrc, _BlendDst, _BlendOp;
float _FadeStrength, _FadeAmt, _DistFade, _ProjectionMaxIntensity;
float _InnerFadeStrength, _InnerIntensityCurve;

//Instanced Properties

UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
    UNITY_DEFINE_INSTANCED_PROP(uint, _Channel)
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
    UNITY_DEFINE_INSTANCED_PROP(float4, _Emission2)
    UNITY_DEFINE_INSTANCED_PROP(float4, _Emission3)
    UNITY_DEFINE_INSTANCED_PROP(float, _ConeWidth)
    UNITY_DEFINE_INSTANCED_PROP(float, _ConeLength)
    UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
    UNITY_DEFINE_INSTANCED_PROP(float, _MaxConeLength)
UNITY_INSTANCING_BUFFER_END(Props)

