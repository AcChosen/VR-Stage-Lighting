//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
inline float CorrectedLinearEyeDepth(float z, float B)
{
	return 1.0 / (z/UNITY_MATRIX_P._34 + B);
}


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

//Huge, huge thanks and shoutout to Uncomfy on the VRC Shader Discord for helping me figure this out <3
        float4 InvertRotations (float4 input, float panValue, float tiltValue)
        {
            float sX, cX, sY, cY;

            #ifdef VRSL_DMX
                float angleY = radians(getOffsetY() + (panValue));
            #endif
            #ifdef VRSL_AUDIOLINK
                float angleY = radians(0);
            #endif

            sY = sin(angleY);
            cY = cos(angleY);
            float4x4 rotateYMatrix = float4x4(cY, sY, 0, 0,
                -sY, cY, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
            float4 BaseAndFixturePos = input;

            	//INVERSION CHECK
            rotateYMatrix = IF(checkPanInvertY() == 1, transpose(rotateYMatrix), rotateYMatrix);

            //float4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);
            //LOCALROTY IS NEW ROTATION


            float tiltOffset = 90.0;
            tiltOffset = IF(checkTiltInvertZ() == 1, -tiltOffset, tiltOffset);
            //set new origin to do transform
            float4 newOrigin = input.w * _FixtureRotationOrigin;
            input.xyz -= newOrigin;

            #ifdef VRSL_DMX
                float angleX = radians(getOffsetX() + (tiltValue + tiltOffset));
            #endif
            #ifdef VRSL_AUDIOLINK
                float angleX = radians(0 + (tiltOffset));
            #endif
            sX = sin((angleX));
            cX = cos((angleX));

            float4x4 rotateXMatrix = float4x4(1, 0, 0, 0,
                0, cX, sX, 0,
                0, -sX, cX, 0,
                0, 0, 0, 1);

            //float4 fixtureVertexPos = input;

            	//INVERSION CHECK
            rotateXMatrix = IF(checkTiltInvertZ() == 1, transpose(rotateXMatrix), rotateXMatrix);
            //float4 localRotX = mul(rotateXMatrix, fixtureVertexPos);

            float4x4 rotateXYMatrix = mul(rotateXMatrix, rotateYMatrix);
            float4 localRotXY = mul(rotateXYMatrix, input);

            input.xyz = localRotXY;
            input.xyz += newOrigin;
            return input;
        }

        float2 RotateUV(float2 input, float angle)
        {
            float2 newOrigin = float2(0.5, 0.5);
            input -= newOrigin;
            float sinX = sin (radians(angle));
            float cosX = cos (radians(angle));
            float sinY = sin (radians(angle));
            float2x2 rotationMatrix = float2x2( cosX, -sinX, sinY, cosX);
            input = mul(input, rotationMatrix);
            input += newOrigin;
            return input;

        }


        float4 ChooseProjection(float2 uv, float projChooser)
        {
            float2 addition = float2(0.0, 0.0);
            uv*= float2(0.25, 0.5);

            #ifdef WASH
            projChooser = 1.0;
            #endif
            
            addition = IF(projChooser == 1.0, float2(0.0, 0.5) , addition);
            #if !defined(WASH)
                addition = IF(projChooser == 2.0, float2(0.25, 0.5), addition);
                addition = IF(projChooser == 3.0, float2(0.5, 0.5), addition);
                addition = IF(projChooser == 4.0, float2(0.75, 0.5), addition);
                addition = IF(projChooser == 5.0, float2(0.0, 0.0) , addition);
                addition = IF(projChooser == 6.0, float2(0.25, 0.0), addition);
                addition = IF(projChooser == 7.0, float2(0.5, 0.0), addition);
                addition = IF(projChooser == 8.0, float2(0.75, 0.0), addition);
            #endif
            uv.x += addition.x;
            uv.y += addition.y;
            return tex2D(_ProjectionMainTex, uv);
        }
        float ChooseProjectionScalar(float coneWidth, float projChooser)
        {
            //float chooser = IF(isDMX() == 1, selection, instancedGOBOSelection());
            float result = _ProjectionUVMod;
            result = IF((projChooser) == 1.0, (_ProjectionUVMod * _MinimumBeamRadius), result);
            #if !defined(WASH)
            result = IF((projChooser) == 2.0, _ProjectionUVMod2 * _MinimumBeamRadius, result);
            result = IF((projChooser) == 3.0, _ProjectionUVMod3 * _MinimumBeamRadius, result);
            result = IF((projChooser) == 4.0, _ProjectionUVMod4 * _MinimumBeamRadius, result);
            result = IF((projChooser) == 5.0, _ProjectionUVMod5 * _MinimumBeamRadius, result);
            result = IF((projChooser) == 6.0, _ProjectionUVMod6 * _MinimumBeamRadius, result);
            result = IF((projChooser) == 7.0, _ProjectionUVMod7 * _MinimumBeamRadius, result);
            result = IF((projChooser) == 8.0, _ProjectionUVMod8 * _MinimumBeamRadius, result);
            #endif


            // float a = 1.8;
            // #ifdef WASH
            //     a = 3.0;
            // #endif
            // return result * (clamp(coneWidth, -2.0, 4) + a);
            float conewidthControl = coneWidth/4.25;
            #ifndef WASH
                return result * lerp(0.325, 1, (conewidthControl));
            #else
                return result * lerp(0.4, 1, (conewidthControl));
            #endif
        }  





        fixed4 ProjectionFrag(v2f i) : SV_Target
        {
            //UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            UNITY_SETUP_INSTANCE_ID(i);
            if(i.color.g > 0.5)
            {

                float4 emissionTint = i.emissionColor;

                #ifdef VRSL_DMX
                    float gi = getGlobalIntensity();
                    float fi = getFinalIntensity();
                    float coneWidth = i.intensityStrobeWidth.z;
                    if(((all(i.rgbColor <= float4(0.01,0.01,0.01,1)) || i.intensityStrobeWidth.x <= 0.01) && isDMX() == 1) || gi <= 0.005 || fi <= 0.005 || all(emissionTint <= float4(0.005, 0.005, 0.005, 1)))
                    {
                        return half4(0,0,0,0);
                    }
                #endif
                #ifdef VRSL_AUDIOLINK
                    float audioReaction = i.audioGlobalFinalConeIntensity.x;
                    float gi = i.audioGlobalFinalConeIntensity.y;
                    float fi = i.audioGlobalFinalConeIntensity.z;
                    float coneWidth = i.audioGlobalFinalConeIntensity.w;
                    // if((all(i.rgbColor <= float4(0.01,0.01,0.01,1)) || i.intensityStrobeWidth.x <= 0.01) && isOSC() == 1)
                    // {
                    //     return (0,0,0,0);
                    // }
                    if(audioReaction <= 0.005 || gi <= 0.005 || fi <= 0.005 || all(emissionTint<= float4(0.005, 0.005, 0.005, 1.0)))
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


                #ifdef VRSL_DMX
                    float panValue = i.goboPlusSpinPanTilt.z;
                    float tiltValue = i.goboPlusSpinPanTilt.w;
                    uint selection = round(i.goboPlusSpinPanTilt.x);
                #endif

                 //Calculating projection
                i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
                float2 screenposUV = i.screenPos.xy / i.screenPos.w;


                //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
                float perspectiveDivide = 1.0f / i.pos.w;
                float4 depthdirect = i.worldDirection * perspectiveDivide;
                //float2 altScreenPos = i.screenPos.xy * perspectiveDivide;


                //
               // #if _MULTISAMPLEDEPTH
                    float2 texelSize = _CameraDepthTexture_TexelSize.xy;
                    float d1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(1.0, 0.0) * texelSize);
                    float d2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(-1.0, 0.0) * texelSize);
                    float d3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(0.0, 1.0) * texelSize);
                    float d4 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV + float2(0.0, -1.0) * texelSize);
                    float sceneZ = min(d1, min(d2, min(d3, d4)));
             //   #else
              //      float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV);
               // #endif

                #if UNITY_REVERSED_Z
                    if (sceneZ == 0)
                #else
                    if (sceneZ == 1)
                #endif
                        return float4(0,0,0,1);

                
                float depth = CorrectedLinearEyeDepth(sceneZ, depthdirect.w);

                //Convert from Corrected Linear Eye Depth to Linear01Depth
                //Credit: https://www.cyanilux.com/tutorials/depth/#eye-depth
                depth = (1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z);
                depth = Linear01Depth(depth);

                 
                //lienarize the depth

                float3 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) ).xyz;
                //get object origin in world space.

                float3 projectionOriginWorld = mul(unity_ObjectToWorld,_ProjectionRangeOrigin);
                
                float3 wpos = getWpos(depth, i.ray);
                //convert view space coordinate to world space coordinate. 
                //Wpos is now coordinates for intersection.

                //get the projection in object space
                float3 oPos = mul(unity_WorldToObject, float4(wpos,1)).xyz;
                if((distance(oPos, _FixtureRotationOrigin) < _ProjectionCutoff) || distance(oPos, float4(0,0,0,0)) < _ProjectionOriginCutoff)
                {
                    //check the distance of rotation origin to the set cutoff value.
                    //if distance is less that the set value, discard the pixel.
                    //this is used to prevent the projection from bleeding on to the source fixture mesh.
                    discard;
                }

                float distanceFromOrigin = abs(distance(objectOrigin , wpos));


                #ifdef VRSL_DMX
                    float projChooser = IF(isDMX() == 1, selection, instancedGOBOSelection());
                #endif
                #ifdef VRSL_AUDIOLINK
                    float projChooser = round(instancedGOBOSelection());
                #endif


                //Get distance of intersection from the origin in world space
                #ifdef VRSL_DMX
                    float UVscale = 1/(0 + (distanceFromOrigin * (ChooseProjectionScalar(coneWidth, projChooser)) + (0 * (distanceFromOrigin * distanceFromOrigin))));
                    distanceFromOrigin = lerp(distanceFromOrigin*0.6 +0.65,distanceFromOrigin, saturate(coneWidth));
                #endif
                #ifdef VRSL_AUDIOLINK
                    distanceFromOrigin = lerp(distanceFromOrigin*0.6 +0.65,distanceFromOrigin, saturate(coneWidth));
                    float UVscale = 1/(0 + ((distanceFromOrigin) * ChooseProjectionScalar(coneWidth, projChooser)));
                #endif
                // inverse that distance so that it gets smaller as it gets closer, 
                // multiply it by modifier parameter incase things get wonky.
                float3 projPos = getProjPos(wpos);


                //position of the intersection fragment in the cone's object space
                #ifdef VRSL_DMX
                    projPos = InvertRotations(float4(projPos,1.0), panValue, tiltValue);
                #endif
                #ifdef VRSL_AUDIOLINK
                    projPos = InvertRotations(float4(projPos,1.0), 0, 0);
                #endif



                float2 uvCoords = (((float2((projPos.x), projPos.y) * UVscale)));

                uvCoords.x += 0.5;
                uvCoords.y += 0.5;
                //Get coordinate plane

                half2 uvOrigin = half2(0,0);
                uvOrigin = (half2(0,0) * UVscale) * (clamp(coneWidth+0.5, -1.0, 4) + 1.6) + 0.5;
                
                #ifdef VRSL_DMX
                    _SpinSpeed = IF(checkPanInvertY() == 1, -_SpinSpeed, _SpinSpeed);
                    _SpinSpeed = IF(isDMX() == 1, _SpinSpeed, _SpinSpeed);
                    uvCoords = IF(isGOBOSpin() == 1 && projChooser > 1.0, RotateUV(uvCoords,  degrees(i.goboPlusSpinPanTilt.y)), RotateUV(uvCoords, _ProjectionRotation));
                #endif
                #ifdef VRSL_AUDIOLINK
                    float goboSpinSpeed = IF(checkPanInvertY() == 1, -getGoboSpinSpeed(), getGoboSpinSpeed());
                    uvCoords = IF(isGOBOSpin() == 1 && projChooser > 1.0, RotateUV(uvCoords, _Time.w * ( 10* goboSpinSpeed)), RotateUV(uvCoords, _ProjectionRotation));
                #endif

               // uvCoords = IF(isGOBOSpin() == 1 && projChooser > 1.0, RotateUV(uvCoords, _Time.w * ( 10* _SpinSpeed)), RotateUV(uvCoords, _ProjectionRotation));
                
                clip(uvCoords);

                //Discard any pixels that are outside of the traditional 0-1 UV bounds.
                float4 tex = ChooseProjection(uvCoords, projChooser);
                float distFromUVOrigin = (abs(distance(uvCoords, uvOrigin)));
                // Create create xy coordinate plane based on object space, make sure it scales based on the 
                // distance from the intersection


                //Discard any pixels that are outside of the traditional 0-1 UV bounds in the negative range.
                clip(1.0 - uvCoords);
                float4 col = tex;

                clip(projPos.z);
                float projectionDistance = abs(distance(i.projectionorigin.xyz, projPos.xyz));

                //Projection Fade
                #ifdef _ALPHATEST_ON
                    col = lerp(col, float4(0,0,0,0), clamp(pow(distFromUVOrigin * (_ProjectionFade-1.0),_ProjectionFadeCurve),0.0,1.0));
                #else
                    col = lerp(col, float4(0,0,0,0), clamp(pow(distFromUVOrigin * _ProjectionFade,_ProjectionFadeCurve),0.0,1.0));
                #endif


                #ifdef VRSL_DMX
                    float strobe = IF(isStrobe() == 1, i.intensityStrobeWidth.y, 1);
                    col = IF(isDMX() == 1 & _EnableStaticEmissionColor == 0, col * i.rgbColor, col);
                #endif
                #ifdef VRSL_AUDIOLINK
                    float strobe = 1.0;
                #endif

                
                //col = IF(_EnableStaticEmissionColor == 1, col * float4(_StaticEmission.r * _RedMultiplier,_StaticEmission.g * _GreenMultiplier,_StaticEmission.b * _BlueMultiplier,_StaticEmission.a), col);
                
                  
                

                // project plane on to the world normals in object space in the z direction of the object origin.
                float projectionIntesnity = _ProjectionIntensity;
                #ifdef _ALPHATEST_ON
                    projectionIntesnity +=4.0;
                #endif
                col = ((col * emissionTint * UVscale * projectionIntesnity)) * strobe; 
                col = col * (1/(_ProjectionDistanceFallOff * (distanceFromOrigin * distanceFromOrigin)));
                #ifdef VRSL_AUDIOLINK
                     col = col * audioReaction;
                #endif
                col = lerp(half4(0,0,0,col.w), col, gi);
                col = lerp(half4(0,0,0,col.w), col, fi);
                //float saturation = saturate(RGB2HSV(col)).y;  
                //col = IF(_EnableStaticEmissionColor == 1, lerp(float4(0,0,0,0), col, saturation), col);
                col = IF( _EnableStaticEmissionColor == 1, float4(col.r * _RedMultiplier, col.g * _GreenMultiplier, col.b * _BlueMultiplier, col.a), col);
                col *= _UniversalIntensity;
                #ifdef _ALPHATEST_ON
                    clip(col.a - DITHER_THRESHOLDS[index]);
                    clip((((col.r + col.g + col.b)/3) * (_ClippingThreshold)) - DITHER_THRESHOLDS[index]);
                    return col;
                #else
                    return col;
                #endif
            }
            else
            {
                return float4(0,0,0,0);
            }
                
        }