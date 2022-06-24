Shader "VRSL/DMX CRTs/Strobe Timer"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE STROBE
    Properties
    {
        [NoScaleOffset]_OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
        _MaxStrobeFreq("Maximum Strobe Frequency", Range(1,100)) = 25
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

            sampler2D   _Tex;
            Texture2D _OSCGridRenderTexture;
            uniform float4 _OSCGridRenderTexture_TexelSize;
            SamplerState sampler_point_repeat;
            half _MaxStrobeFreq;

            

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));



            // float calculateStrobe(float2 coords)
            // {
            //     float freq = GetStrobeValue(coords);
            //     return 2.0;
            // }

            float GetDMXValue(float4 c)
            {
                float3 cRGB = float3(c.r, c.g, c.b);
                float value = LinearRgbToLuminance(cRGB);
                value = LinearToGammaSpaceExact(value);
                
                return value;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR
            //NOTE
            //THIS IS OUTPUTTING THE PHASE OF A SINE WAVE
            //IT READS THE INPUT TEXTURE TO GET THE FREQUENCY AND ADJUSTS THE PHASE TO THAT FREQUENCY FOR THAT CHANNEL
            //IT IS ASSUMING ALL CHANNELS ARE STROBE CHANNELS
            //THE ACTUAL SINE WAVE IS CALCULATED ON THE FIXTURE AS SIN(PHASE)
            //DON'T FORGET NEXT TIME PLZ THX
            //SERIOUSLY, TRIG IS IMPORTANT

            {
                //CHILL FOR 1 SECOND TO ALLOW DATA TO COME IN
                if (_Time.y > 1.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);

                    float dt = clamp(unity_DeltaTime.x, 0.0, 2.0);
                    //T = CURRENT PHASE
                    float t = previousFrame.r;
                    //INCREMENT CURRENT PHASE CLOSER TO 2PI
                    t = t + (dt * (GetDMXValue(currentFrame) * _MaxStrobeFreq));

                    //IF PHASE IS GREATER THAN OR EQUAL TO 2PI, RETURN TO 0, CAUSE SIN(2PI) == SIN(0)
                    if (t >= 2*3.14159265) 
                    {
                        t -= 2*3.14159265; 
                    }
                    //EZ CLAP
                    return clamp(t, 0.0, 1000000.0);
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
