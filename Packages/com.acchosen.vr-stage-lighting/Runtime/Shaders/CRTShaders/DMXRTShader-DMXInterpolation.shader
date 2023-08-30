Shader "VRSL/DMX CRTs/Interpolation"
{
    Properties
    {
        _DMXChannel ("DMX Channel (for legacy global movement speed)", Int) = 0
        [Toggle] _EnableLegacyGlobalMovementSpeedChannel ("Enable Legacy Global Movement Speed Channel (disables individiual movement speed per sector)", Int) = 0
        [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
        [Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
        [Toggle] _EnableCompatibilityMode ("Enable Stream DMX/DMX Control", Int) = 0
        [Toggle(_OLD_SCHOOL_SMOOTHING)] _UseOldSchoolSmoothing ("Use Old School Smoothing", Int) = 0
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

            #pragma multi_compile_local _ _OLD_SCHOOL_SMOOTHING



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

            float getSmoothnessValuePreSet()
            {
                return _SmoothValue;
            }

            float3 getSmoothnessValueRGBPreSet()
            {
                return float3(_SmoothValue, _SmoothValue, _SmoothValue);
            }

            float getSmoothnessValue(float2 uv, out float dmxSmoothness)
            {
                if(_EnableLegacyGlobalMovementSpeedChannel == 1)
                {
                    dmxSmoothness = IF(_EnableCompatibilityMode == 1, getValueAtCoords(0.096151, 0.019231, _DMXChannel), getValueAtCoords(0.189936, 0.00762, _DMXChannel));
                }
                else
                {
                    dmxSmoothness = IF(_EnableCompatibilityMode == 1, getValueAtCoords(0.096151, 0.019231, _DMXChannel), getValueAtUV(float2(0.960, uv.y)));
                }
                float oscSmoothness = lerp(saturate(_MaximumSmoothnessDMX), saturate(_MinimumSmoothnessDMX), dmxSmoothness);
                return oscSmoothness;
               // return IF(_EnableDMX == 1, oscSmoothness, _SmoothValue);  
            }


            float3 getSmoothnessValueRGB(float2 uv, out float3 dmxSmoothness)
            {

                //rgbSmoothnessRaw = float3(0,0,0);
                if(_EnableLegacyGlobalMovementSpeedChannel == 1)
                {
                    dmxSmoothness = getValueAtCoordsRGB(0.189936, 0.00762, _DMXChannel);
                }
                else
                {
                    dmxSmoothness = getValueAtUVRGB(float2(0.960, uv.y));
                }

                float3 rgbSmoothness = float3(lerp( saturate(_MaximumSmoothnessDMX), saturate(_MinimumSmoothnessDMX), dmxSmoothness.r), 
                lerp(saturate(_MaximumSmoothnessDMX), saturate(_MinimumSmoothnessDMX), dmxSmoothness.g), 
                lerp(saturate(_MaximumSmoothnessDMX), saturate(_MinimumSmoothnessDMX), dmxSmoothness.b));
                return rgbSmoothness;
               // return IF(_EnableDMX == 1, rgbSmoothness, float3(_SmoothValue, _SmoothValue, _SmoothValue));  
            }

            float SmoothDamp (float current, float target, inout float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
            {
                //smoothTime = 1-pow(smoothTime, deltaTime);

                float multiplier = target < current ? 0.4 : 1.0;
                smoothTime *= multiplier;
                smoothTime = max(0.0001, smoothTime);
                float num = 2.0 / smoothTime;
                float num2 = num * deltaTime;
                float num3 = 1.0 / (1.0 + num2 + 0.48 * num2 * num2 + 0.235 * num2 * num2 * num2);
                float num4 = current - target;
                float num5 = target;
                float num6 = maxSpeed * smoothTime;
                num4 = clamp(num4, -num6, num6);
                target = current - num4;
                float num7 = (currentVelocity + num * num4) * deltaTime;
                currentVelocity = (currentVelocity - num * num7) * num3;
                float num8 = target + (num4 + num7) * num3;
                if (num5 - current > 0.0 == num8 > num5)
                {
                    num8 = num5;
                    currentVelocity = (num8 - num5) / deltaTime;
                }
                return num8;
            }


            // float invLerp(float from, float to, float value){
            // return (value - from) / (to - from);
            // }

            // float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
            // float rel = invLerp(origFrom, origTo, value);
            // return lerp(targetFrom, targetTo, rel);
            // }



            float DampComplex(float source, float target, float smoothing, float dt, float dmxMod)
            {
                float multiplier = target < source ? 0.02-(0.01 * dmxMod) : 1.0;
                smoothing *= multiplier;
                return lerp(source, target, 1 - pow(saturate(smoothing), dt));
            }

            float Damp(float source, float target, float smoothing, float dt)
            {
                return lerp(source, target, 1 - pow(saturate(smoothing), dt));
            }
            // float3 Damp(float3 source, float3 target, float smoothing, float dt)
            // {
            //     return lerp(source, target, 1 - pow(smoothing, dt))
            // }


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

                        float3 dmxSmoothness = float3(1.0, 1.0, 1.0);
                        #if _OLD_SCHOOL_SMOOTHING
                            float3 s = getSmoothnessValueRGB(IN.localTexcoord.xy, dmxSmoothness);
                            s = lerp(clamp(lerp(previousFrame.rgb, currentFrame.rgb,smoothstep(0.0, 1.0, clamp(unity_DeltaTime.z,0.0,100.0))) , 0.0, 100.0), currentFrame.rgb, s);
                        #else
                            
                            float3 smoothing = _EnableDMX == 1 ? getSmoothnessValueRGB(IN.localTexcoord.xy, dmxSmoothness) : getSmoothnessValueRGBPreSet() ;




                            float3 s = float3(
                                lerp(DampComplex(previousFrame.r, currentFrame.r, smoothing.r, unity_DeltaTime.x, dmxSmoothness.r), currentFrame.r, dmxSmoothness.r * 0.1),
                                                lerp(DampComplex(previousFrame.g, currentFrame.g, smoothing.g, unity_DeltaTime.x, dmxSmoothness.g), currentFrame.g, dmxSmoothness.g * 0.1),
                                                    (DampComplex(previousFrame.b, currentFrame.b, smoothing.b, unity_DeltaTime.x, dmxSmoothness.b), currentFrame.b, dmxSmoothness.b * 0.1)
                                                    );

                        #endif
                        
                        return float4(s, currentFrame.a); 
                    }
                    else
                    {
                        float dmxSmoothness = 1.0;
                        float smoothing = 0.0;
                        #if _OLD_SCHOOL_SMOOTHING
                         float smoothness = _EnableDMX == 1 ? getSmoothnessValue(IN.localTexcoord.xy, dmxSmoothness) : getSmoothnessValuePreSet();         
                            return lerp(clamp(lerp(previousFrame, currentFrame,smoothstep(0.0, 1.0, clamp(unity_DeltaTime.x,0.0,100.0))) , 0.0, 100.0), currentFrame, smoothness);
                        #else
                            //float currentVelocity = previousFrame.a;
                            if(_EnableDMX == 1)
                            {
                                smoothing = getSmoothnessValue(IN.localTexcoord.xy, dmxSmoothness);
                            }
                            else
                            {
                                smoothing = getSmoothnessValuePreSet();
                            }
                            return lerp(DampComplex(previousFrame.r, currentFrame.r, smoothing, unity_DeltaTime.x, dmxSmoothness), currentFrame.r , dmxSmoothness * 0.1);
                            //float output = SmoothDamp(previousFrame.r, currentFrame.r, currentVelocity, smoothing,100000.0,unity_DeltaTime.x);
                           // return (float4(output, output, output, currentVelocity));
                        #endif
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
