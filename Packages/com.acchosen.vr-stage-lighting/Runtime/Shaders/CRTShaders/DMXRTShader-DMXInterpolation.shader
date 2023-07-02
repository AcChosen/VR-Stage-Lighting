Shader "VRSL/DMX CRTs/Interpolation"
{
    Properties
    {
        _DMXChannel ("DMX Channel (for legacy global movement speed)", Int) = 0
        [Toggle] _EnableLegacyGlobalMovementSpeedChannel ("Enable Legacy Global Movement Speed Channel (disables individiual movement speed per sector)", Int) = 0
        [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
        [Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
        [Toggle] _EnableCompatibilityMode ("Enable Stream DMX/DMX Control", Int) = 0
        [NoScaleOffset]_DMXTexture("DMX Grid Render Texture (To Control Lights)", 2D) = "white" {}
        _SmoothValue ("Smoothness Level (0 to 1, 0 = max)", Range(0,1)) = 0.5
        _MinimumSmoothnessDMX ("Minimum Smoothness Value for DMX", Float) = 0
        _MaximumSmoothnessDMX ("Maximum Smoothness Value for OSc", Float) = 0
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

            #define VRSL_DMX

            float _SmoothValue, _MinimumSmoothnessDMX, _MaximumSmoothnessDMX;
            sampler2D   _Tex;
            sampler2D _DMXTexture;
            SamplerState sampler_point_repeat;
            int _IsEven, _DMXChannel, _EnableDMX, _EnableLegacyGlobalMovementSpeedChannel;
            uint _EnableCompatibilityMode, _NineUniverseMode;
            float oscSmoothnessRAW;
            float3 rgbSmoothnessRaw;
            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));



            float2 getSectorCoordinates(float x, float y, uint sector)
            {
                // say we were on sector 6
                // we need to move over 2 sectors
                // and we need to move up 3 sectors

                //1 sector is every 13 channels
                //the grid is 26x26 aka 2 sectors per row
                //TRAVERSING THE Y AXIS OF THE DMX GRID

                float ymod = floor(sector / 2);
                float originalx = x;
                float originaly = y;       

                //TRAVERSING THE X AXIS OF THE DMX GRID
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
            float4 c = tex2D(_DMXTexture, uvcoords);
            float3 cRGB = float3(c.r, c.g, c.b);
            float value = LinearRgbToLuminance(cRGB);
            value = LinearToGammaSpaceExact(value);

            return value;
            }

            float3 getValueAtCoordsRGB(float x, float y, uint sector)
            {
            float2 recoords = getSectorCoordinates(x, y, sector);
            float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
            float4 c = tex2D(_DMXTexture, uvcoords);
            float3 col = float3(LinearToGammaSpaceExact(c.r), LinearToGammaSpaceExact(c.g), LinearToGammaSpaceExact(c.b));
            return col;
            }

            float getValueAtUV(float2 uv)
            {
                float4 c = tex2D(_DMXTexture, uv);
                float3 cRGB = float3(c.r, c.g, c.b);
                float value = LinearRgbToLuminance(cRGB);
                value = LinearToGammaSpaceExact(value);

                return value;
            }

            float3 getValueAtUVRGB(float2 uv)
            {
                float4 c = tex2D(_DMXTexture, uv);
                float3 col = float3(LinearToGammaSpaceExact(c.r), LinearToGammaSpaceExact(c.g), LinearToGammaSpaceExact(c.b));
                return col;
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
                float oscSmoothness = lerp(_MinimumSmoothnessDMX, _MaximumSmoothnessDMX, oscSmoothnessRAW);
                return IF(_EnableDMX == 1, oscSmoothness, _SmoothValue);  
            }


            float3 getSmoothnessValueRGB(float2 uv)
            {

                rgbSmoothnessRaw = float3(0,0,0);
                if(_EnableLegacyGlobalMovementSpeedChannel == 1)
                {
                    rgbSmoothnessRaw = getValueAtCoordsRGB(0.189936, 0.00762, _DMXChannel);
                }
                else
                {
                    rgbSmoothnessRaw = getValueAtUVRGB(float2(0.960, uv.y));
                }

                float3 rgbSmoothness = float3(lerp(_MinimumSmoothnessDMX, _MaximumSmoothnessDMX, rgbSmoothnessRaw.r), 
                lerp(_MinimumSmoothnessDMX, _MaximumSmoothnessDMX, rgbSmoothnessRaw.g), 
                lerp(_MinimumSmoothnessDMX, _MaximumSmoothnessDMX, rgbSmoothnessRaw.b));

                return IF(_EnableDMX == 1, rgbSmoothness, float3(_SmoothValue, _SmoothValue, _SmoothValue));  
            }


            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                if (_Time.y > 3.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = tex2D(_DMXTexture, IN.localTexcoord.xy);
                    // if(IN.localTexcoord.y > 0.90)
                    // {
                    //     oscSmoothnessRAW = IF(_EnableCompatibilityMode == 1, getValueAtCoords(0.096151, 0.019231, _DMXChannel), getValueAtCoords(0.189936, 0.00762, _DMXChannel));
                    //     return oscSmoothnessRAW;
                    // }
                    if(_NineUniverseMode && _EnableDMX)
                    {
                        float3 s = getSmoothnessValueRGB(IN.localTexcoord.xy);
                        s = lerp(clamp(lerp(previousFrame.rgb, currentFrame.rgb,smoothstep(0.0, 1.0, clamp(unity_DeltaTime.z,0.0,1.0))) , 0.0, 400.0), currentFrame.rgb, s);
                        return float4(s, currentFrame.a); 
                    }
                    else
                    {
                        float smoothness = getSmoothnessValue(IN.localTexcoord.xy);
                        return lerp(clamp(lerp(previousFrame, currentFrame,smoothstep(0.0, 1.0, clamp(unity_DeltaTime.z,0.0,1.0))) , 0.0, 400.0), currentFrame, smoothness);
                    }
                }
                else
                {
                    return tex2D(_DMXTexture, IN.localTexcoord.xy);
                }
            }
            ENDCG
         }
    }
    CustomEditor "VRSLInspector"
}
