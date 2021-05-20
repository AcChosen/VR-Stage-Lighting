Shader "VRSL/DMX CRTs/Strobe Timer"
{
    Properties
    {
        [NoScaleOffset]_OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            sampler2D   _Tex;
            Texture2D _OSCGridRenderTexture;
            SamplerState sampler_point_repeat;

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float getValueAtCoords(float2 coords)
            {
                //float2 recoords = getSectorCoordinates(x, y, sector);
                //float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
                //float4 c = tex2Dlod(_OSCGridRenderTexture, uvcoords);
                float4 c = _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, coords, 0);
                float3 cRGB = float3(c.r, c.g, c.b);
                float value = LinearRgbToLuminance(cRGB);
                value = LinearToGammaSpaceExact(value);
                
                return value;
            }

            float GetStrobeValue(float2 coords)
            {
                float rawValue = getValueAtCoords(coords);
                float finalValue = IF(rawValue <= 0.03515625, 0.0, rawValue);
                uint remappedvalue = clamp(floor(finalValue * 255),10,255);
                float frequency = clamp(remappedvalue * 0.0980392156862745, 1.0, 25.0);//hz
                //frequency = IF(finalValue == 0.0, 0.0, frequency);
                //frequency = IF(frequency <= 2.0, 0.0, frequency);
                //float result = IF(_EnableOSC == 1, finalValue, 0.0);
                //_FinalStrobeFreq = frequency;
                return frequency;
            }



            // float calculateStrobe(float2 coords)
            // {
            //     float freq = GetStrobeValue(coords);
            //     return 2.0;
            // }

            float4 frag(v2f_customrendertexture IN) : COLOR
            
            {
                if (_Time.y > 3.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);
                    //return lerp(previousFrame, currentFrame, (_SmoothValue * 100) * unity_DeltaTime.x);
                    float4 output = previousFrame + clamp(unity_DeltaTime.z,0.0,2.0) * GetStrobeValue(IN.localTexcoord.xy);
                    if(currentFrame.r <= 0.05)
                    {
                        return float4(0,0,0,0);
                    }
                    else
                    {
                        return clamp(output, 0.0, 300.0);
                    }

                }
                else
                {
                    return _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);
                }
            }
            ENDCG
         }
    }
    CustomEditor "VRSLInspector"
}
