// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

        float3 getWpos(float depth, float3 ray)
        {
                float4 vpos = float4(ray * depth, 1);
                // scale ray by linearized depth, which gives us the position of the ray 
                // intersection with the depth buffer in view space. This is the point of intersection in view space.
                
                float3 wpos = (mul(unity_CameraToWorld, vpos).xyz);
                //convert view space coordinate to world space coordinate. 
                //Wpos is now coordinates for intersection.
                return wpos;
        }

        float3 getProjPos(float3 wpos)
        {
            return (mul(unity_WorldToObject,float4(wpos.x, wpos.y, wpos.z, 1)));
        }

        inline float CorrectedLinearEyeDepth(float z, float B)
        {
            return 1.0 / (z/UNITY_MATRIX_P._34 + B);
        }



        fixed4 ProjectionFrag(v2f i) : SV_Target
        {
            //UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            UNITY_SETUP_INSTANCE_ID(i);
            float4 emissionTint = i.emissionColor;
            #ifdef VRSL_DMX
                float gi = i.globalFinalIntensity.x;
                float fi = i.globalFinalIntensity.y;
                #ifdef FIVECH
                    if(((all(i.rgbColor <= float4(0.01,0.01,0.01,1)) || i.intensityStrobe.x <= 0.01) && isDMX() == 1) || gi <= 0.005 || fi <= 0.005 || all(emissionTint <= float4(0.005, 0.005, 0.005, 1.0)))
                    {
                        return float4(0,0,0,0);
                    }
                #else
                    if(((all(i.rgbColor <= float4(0.05,0.05,0.05,1)) || i.intensityStrobe.x <= 0.05) && isDMX() == 1) || gi <= 0.005 || fi <= 0.005 || all(emissionTint <= float4(0.005, 0.005, 0.005, 1.0)))
                    {
                        return float4(0,0,0,0);
                    }
                #endif
            #endif
            #ifdef VRSL_AUDIOLINK
                float audioReaction = i.audioGlobalFinalIntensity.x;
                float gi = i.audioGlobalFinalIntensity.y;
                float fi = i.audioGlobalFinalIntensity.z;
                if(audioReaction <= 0.005 || gi <= 0.005 || fi <= 0.005 || all(emissionTint <= float4(0.005, 0.005, 0.005, 1.0)))
                {
                    return half4(0,0,0,0);
                }
            #endif

            #if _ALPHATEST_ON
                float2 pos = i.screenPos.xy / i.screenPos.w;
                pos *= _ScreenParams.xy;
                float DITHER_THRESHOLDS[16] =
                {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };
                int index = (int)((uint(pos.x) % 4) * 4 + uint(pos.y) % 4);
            #endif
            
            if(i.color.g != 0)
            {
                
                i.ray = i.ray * (_ProjectionParams.z / i.ray.z);

                float2 screenposUV = i.screenPos.xy / i.screenPos.w;

                //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
                float perspectiveDivide = 1.0f / i.pos.w;
                float4 direction = i.worldDirection * perspectiveDivide;
                float2 altScreenPos = i.screenPos.xy * perspectiveDivide;


                #if _MULTISAMPLEDEPTH
                    float2 texelSize = _CameraDepthTexture_TexelSize.xy;
                    float d1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(1.0, 0.0) * texelSize);
                    float d2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(-1.0, 0.0) * texelSize);
                    float d3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(0.0, 1.0) * texelSize);
                    float d4 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(0.0, -1.0) * texelSize);
                    float sceneZ = min(d1, min(d2, min(d3, d4)));
                #else
                    float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV);
                #endif

                #if UNITY_REVERSED_Z
                    if (sceneZ == 0)
                #else
                    if (sceneZ == 1)
                #endif
                        return float4(0,0,0,1);
                //Convert to Corrected LinearEyeDepth by DJ Lukis
                float depth = CorrectedLinearEyeDepth(sceneZ, direction.w);

                //Convert from Corrected Linear Eye Depth to Linear01Depth
                //Credit: https://www.cyanilux.com/tutorials/depth/#eye-depth
                depth = (1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z);
                //Convert to Linear01Depth
                depth = Linear01Depth(depth);
                float3 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) ).xyz;
                //get object origin in world space.
                //float3 fragViewPos = float4(i.ray * depth, 1);

                float3 wpos = getWpos(depth, i.ray);
                float3 projPos = getProjPos(wpos);
                float distanceFromOrigin = length(objectOrigin - wpos);
                float attenuationDist = length(objectOrigin - wpos);
                float f = _Fade;
                #if _ALPHATEST_ON
                    f += 1.0;
                #endif
                float attenuation = 1.0 / (_ProjectionDistanceFallOff + f * attenuationDist + _FeatherOffset * (attenuationDist * attenuationDist));

                float UVscale = 1/(_ProjectionDistanceFallOff + (distanceFromOrigin * _ProjectionUVMod) + (_FeatherOffset * (distanceFromOrigin * distanceFromOrigin)));

                //float3 calculatedWorldNormal = getCalculatedWorldNormal(projPos);

                float2 uvCoords = (((float2((projPos.x), projPos.y) * UVscale)));
                //uvCoords = mul(uvCoords, projPos.z);
                float2 oldUVcoords = uvCoords;
                //Get coordinate plane in object space

                uvCoords.x += _XOffset;
                uvCoords.y += _YOffset;
                uvCoords.x *= _ModX;
                uvCoords.y *= _ModY;
                //uvCoords = normalize(mul(float4(uvCoords, 0.0, 0.0), unity_ObjectToWorld)).xy;

                clip(uvCoords);
                //Discard any pixels that are outside of the traditional 0-1 UV bounds.

                float4 tex = tex2D(_ProjectionMainTex, uvCoords); 
                //tex = float4(tex.x, tex.y, tex.z, pow(tex.w * distanceFromOrigin, -1));
                //tex = pow(tex * distanceFromOrigin, 1);
                float distFromUVOrigin = (abs(distance(uvCoords, half2(0,0))));
                //calculatedWorldNormal = UnpackNormal(tex2D(_SceneNormals, oldUVcoords));
                // Create create xy coordinate plane based on object space, make sure it scales based on the 
                // distance from the intersection

                clip(1.0 - uvCoords);
                float4 col = tex;
                 //float4 col = tex * float4(n,1);

                //clip(projPos.z);
                #ifdef VRSL_AUDIOLINK
                    float strobe = 1.0;
                    col *= audioReaction;
                #endif
                #ifdef VRSL_DMX
                    float strobe = IF(isStrobe() == 1, i.intensityStrobe.y, 1);    
                    float4 DMXcol = col;
                    DMXcol *= i.rgbColor;
                    col = IF(isDMX() == 1, DMXcol, col);
                #endif


                
                float4 result = ((col * UVscale  * _ProjectionMaxIntensity) * emissionTint) * strobe;
                float fadeRange = (saturate(1-(pow(10, distanceFromOrigin - 2))));
                col = (((lerp(result,float4(0,0,0,0), smoothstep(distanceFromOrigin, 0, f))) * gi) * fi) * _UniversalIntensity;
                
                #ifdef _ALPHATEST_ON
                    col *= _AlphaProjectionIntensity;
                    clip(col.a - DITHER_THRESHOLDS[index]);
                    clip((((col.r + col.g + col.b)/3) * (_ClippingThreshold)) - DITHER_THRESHOLDS[index]);
                    return col;
                #else
                    return col;
                #endif
            }
            else
            {
                clip(i.pos);
                discard;
                return float4(0,0,0,0);
            }
                
        }