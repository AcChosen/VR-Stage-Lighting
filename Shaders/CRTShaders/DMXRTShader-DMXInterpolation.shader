Shader "VRSL/DMX CRTs/Interpolation"
{
    Properties
    {
        _DMXChannel ("DMX Channel (for legacy global movement speed)", Int) = 0
        [Toggle] _EnableLegacyGlobalMovementSpeedChannel ("Enable Legacy Global Movement Speed Channel (disables individiual movement speed per sector)", Int) = 0
        [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
        [Toggle] _EnableCompatibilityMode ("Enable Stream OSC/DMX Control", Int) = 0
        [NoScaleOffset]_OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
        _SmoothValue ("Smoothness Level (0 to 1, 0 = max)", Range(0,1)) = 0.5
        _MinimumSmoothnessOSC ("Minimum Smoothness Value for OSC", Float) = 0
        _MaximumSmoothnessOSC ("Maximum Smoothness Value for OSc", Float) = 0
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Name "Interpolation Pass"
        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float _SmoothValue, _MinimumSmoothnessOSC, _MaximumSmoothnessOSC;
            sampler2D   _Tex;
            sampler2D _OSCGridRenderTexture;
            SamplerState sampler_point_repeat;
            int _IsEven, _DMXChannel, _EnableOSC, _EnableLegacyGlobalMovementSpeedChannel;
            uint _EnableCompatibilityMode;
            float oscSmoothnessRAW;
            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));



            float2 getSectorCoordinates(float x, float y, uint sector)
            {
                // say we were on sector 6
                // we need to move over 2 sectors
                // and we need to move up 3 sectors

                //1 sector is every 13 channels
                //the grid is 26x26 aka 2 sectors per row
                //TRAVERSING THE Y AXIS OF THE OSC GRID

                float ymod = floor(sector / 2);
                float originalx = x;
                float originaly = y;       

                //TRAVERSING THE X AXIS OF THE OSC GRID
                float xmod = sector % 2;


                //x += (xmod * 0.052);
                //0.498573
                //0.036343
                x+= (xmod * 0.498573);
                y+= (ymod * 0.03846);
                //y += (ymod * 0.006);
                originaly = IF(sector == 0, originaly, originaly + (0.0147 * (sector)));

                return IF(_EnableCompatibilityMode == 1, 
                float2 (x, y), 
                float2(originalx, originaly));

            }

            float getValueAtCoords(float x, float y, uint sector)
            {
            float2 recoords = getSectorCoordinates(x, y, sector);
            float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
            float4 c = tex2D(_OSCGridRenderTexture, uvcoords);
            float3 cRGB = float3(c.r, c.g, c.b);
            float value = LinearRgbToLuminance(cRGB);
            value = LinearToGammaSpaceExact(value);

            return value;
            }

            float getValueAtUV(float2 uv)
            {
                float4 c = tex2D(_OSCGridRenderTexture, uv);
                float3 cRGB = float3(c.r, c.g, c.b);
                float value = LinearRgbToLuminance(cRGB);
                value = LinearToGammaSpaceExact(value);

                return value;
            }

            float2 getCh13coords(float2 uv)
            {
                return float2(0.911, uv.y);
            }
            float getSmoothnessValue(float2 uv)
            {
                if(_EnableLegacyGlobalMovementSpeedChannel == 1)
                {
                    oscSmoothnessRAW = IF(_EnableCompatibilityMode == 1, getValueAtCoords(0.096151, 0.019231, _DMXChannel), getValueAtCoords(0.189936, 0.00762, _DMXChannel));
                }
                else
                {
                    oscSmoothnessRAW = IF(_EnableCompatibilityMode == 1, getValueAtCoords(0.096151, 0.019231, _DMXChannel), getValueAtUV(float2(0.960, uv.y)));
                }
                float oscSmoothness = lerp(_MinimumSmoothnessOSC, _MaximumSmoothnessOSC, oscSmoothnessRAW);
                return IF(_EnableOSC == 1, oscSmoothness, _SmoothValue);  
            }


            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                if (_Time.y > 3.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = tex2D(_OSCGridRenderTexture, IN.localTexcoord.xy);
                    // if(IN.localTexcoord.y > 0.90)
                    // {
                    //     oscSmoothnessRAW = IF(_EnableCompatibilityMode == 1, getValueAtCoords(0.096151, 0.019231, _DMXChannel), getValueAtCoords(0.189936, 0.00762, _DMXChannel));
                    //     return oscSmoothnessRAW;
                    // }
                    float smoothness = getSmoothnessValue(IN.localTexcoord.xy);
                    return lerp(clamp(lerp(previousFrame, currentFrame,smoothstep(0.0, 1.0, clamp(unity_DeltaTime.z,0.0,1.0))) , 0.0, 400.0), currentFrame, smoothness);
                }
                else
                {
                    return tex2D(_OSCGridRenderTexture, IN.localTexcoord.xy);
                }
            }
            ENDCG
         }
    }
    CustomEditor "VRSLInspector"
}
