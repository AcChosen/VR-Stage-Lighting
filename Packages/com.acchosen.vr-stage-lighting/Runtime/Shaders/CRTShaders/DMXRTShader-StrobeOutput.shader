Shader "VRSL/DMX CRTs/Strobe Output"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE STROBE
    Properties
    {
        [NoScaleOffset]_DMXTexture("DMX Grid Render Texture (To Control Lights)", 2D) = "white" {}
        _MaxStrobeFreq("Maximum Strobe Frequency", Range(1,100)) = 25
        [Toggle]_EnableCompatibilityMode("Compatibility Mode", Float) = 0
        [Toggle]_NineUniverseMode("Nine Universe Mode", Float) = 0
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Pass
        {
            Name "Strobe Pass"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 4.5

            #define VRSL_DMX

       // #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/VRSLDMX.cginc"
            Texture2D _Udon_DMXGridRenderTexture;
            Texture2D _Udon_DMXGridStrobeTimer;
            SamplerState VRSL_PointClampSampler;
            float _NineUniverseMode, _EnableCompatibilityMode;

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float3 getValue(float3 c)
            {
                    float3 value = float3(0,0,0);
                        
                    if(_NineUniverseMode == 1 && _EnableCompatibilityMode != 1)
                    {
                        value.r = c.r;
                        value.g = c.g;
                        value.b = c.b;
                    }
                    else
                    {
                        float3 cRGB = float3(c.r, c.g, c.b);
                        float v = LinearRgbToLuminance(cRGB);
                        value = float3(v,v,v);
                    }
                        value = float3(LinearToGammaSpaceExact(value.r),LinearToGammaSpaceExact(value.g),LinearToGammaSpaceExact(value.b));
                        return value;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR


            {
                //CHILL FOR 1 SECOND TO ALLOW DATA TO COME IN
                // if (_Time.y > 1.0)
                // {
                    float4 p = _Udon_DMXGridStrobeTimer.SampleLevel(VRSL_PointClampSampler, IN.localTexcoord.xy, 0);
                    float4 s = _Udon_DMXGridRenderTexture.SampleLevel(VRSL_PointClampSampler, IN.localTexcoord.xy, 0);
                    float phase = p.r;
                    float status = getValue(s).r;
                    half strobe = (sin(phase));//Get sin wave
                    strobe = IF(strobe > 0.0, 1.0, 0.0);//turn to square wave
                    //strobe = saturate(strobe);

                    strobe = IF(status > 0.2, strobe, 1); //minimum channel threshold set
                    return strobe;
                // }

                // else
                // {
                //     return float4(0,0,0,0);
                // }
            }
            ENDCG
         }
    }
    //CustomEditor "VRSLInspector"
}
