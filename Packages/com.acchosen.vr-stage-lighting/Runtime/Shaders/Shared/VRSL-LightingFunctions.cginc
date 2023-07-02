//This file contains all of the neccisary functions for lighting to work a'la standard shading.
//Feel free to add to this.
//GGX STANDARD SHADER LIGHTING MODEL
#ifdef _LIGHTING_MODEL
    #include "UnityPBSLighting.cginc"
    sampler2D   _OcclusionMap;
    half        _OcclusionStrength;
	    //-------------------------------------------------------------------------------------
    // counterpart for NormalizePerPixelNormal
    // skips normalization per-vertex and expects normalization to happen per-pixel
    half3 NormalizePerVertexNormal (float3 n) // takes float to avoid overflow
    {
        #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
            return normalize(n);
        #else
            return n; // will normalize per-pixel instead
        #endif
    }

    float3 NormalizePerPixelNormal (float3 n)
    {
        #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
            return n;
        #else
            return normalize((float3)n); // takes float to avoid overflow
        #endif
    }

    //-------------------------------------------------------------------------------------

    UnityLight MainLight ()
    {
        UnityLight l;

        l.color = _LightColor0.rgb;
        l.dir = _WorldSpaceLightPos0.xyz;
        return l;
    }

    UnityLight AdditiveLight (half3 lightDir, half atten)
    {
        UnityLight l;

        l.color = _LightColor0.rgb;
        l.dir = lightDir;
        #ifndef USING_DIRECTIONAL_LIGHT
            l.dir = NormalizePerPixelNormal(l.dir);
        #endif

        // shadow the light
        l.color *= atten;
        return l;
    }

    UnityLight DummyLight ()
    {
        UnityLight l;
        l.color = 0;
        l.dir = half3 (0,1,0);
        return l;
    }

    UnityIndirect ZeroIndirect ()
    {
        UnityIndirect ind;
        ind.diffuse = 0;
        ind.specular = 0;
        return ind;
    }
    half3 NormalInTangentSpace(float2 texcoords)
    {
        half3 normalTangent = UnpackScaleNormal(tex2D (_BumpMap, texcoords.xy), _BumpScale);
        return normalTangent;
    }
    float3 PerPixelWorldNormal(float2 i_tex, float3 tangentToWorld[3])
    {
        half3 binormal = tangentToWorld[0].xyz;
        half3 tangent = tangentToWorld[1].xyz;
        half3 normal = tangentToWorld[2].xyz;

        #if UNITY_TANGENT_ORTHONORMALIZE
            normal = NormalizePerPixelNormal(normal);

            // ortho-normalize Tangent
            tangent = normalize (tangent - normal * dot(tangent, normal));

            // recalculate Binormal
            half3 newB = cross(normal, tangent);
            binormal = newB * sign (dot (newB, binormal));
        #endif

        half3 normalTangent = NormalInTangentSpace(i_tex);
        float3 normalWorld = NormalizePerPixelNormal(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z); // @TODO: see if we can squeeze this normalize on SM2.0 as well

        return normalWorld;
    }
    #define IN_LIGHTDIR_FWDADD(i) half3(i.tangentToWorldAndLightDir[0].w, i.tangentToWorldAndLightDir[1].w, i.tangentToWorldAndLightDir[2].w)
    #define IN_WORLDPOS(i) i.worldPos

        struct FragmentCommonData
    {
        half3 diffColor, specColor;
        // Note: smoothness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
        // Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
        half oneMinusReflectivity, smoothness;
        float3 normalWorld;
        float3 eyeVec;
        half alpha;
        float3 posWorld;
    };
    half2 MetallicRough(float2 uv)
    {
        half2 mg;
        mg.r = tex2D(_MetallicGlossMap, uv).r * _Metallic;
        mg.g = _Glossiness;
        return mg;
    }
    half3 Albedo(float2 texcoords)
    {
        half3 albedo = _Color.rgb * tex2D (_MainTex, texcoords.xy).rgb;
        return albedo;
    }
    half Alpha(float2 uv)
    {
        return tex2D(_MainTex, uv).a * _Color.a;
    }
    half4 SpecularGloss(float2 uv)
    {
        half4 sg;
        sg = tex2D(_MetallicGlossMap, uv);
        sg.a = _Glossiness;
        return sg;
    }
    half Occlusion(float2 uv)
    {
        half occ = tex2D(_OcclusionMap, uv).g;
        return LerpOneTo (occ, _OcclusionStrength);
    }

    #ifndef UNITY_SETUP_BRDF_INPUT
        #define UNITY_SETUP_BRDF_INPUT RoughnessSetup
    #endif


    inline FragmentCommonData SpecularSetup (float2 i_tex)
    {
        half4 specGloss = SpecularGloss(i_tex.xy);
        half3 specColor = specGloss.rgb;
        half smoothness = specGloss.a;

        half oneMinusReflectivity;
        half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);

        FragmentCommonData o = (FragmentCommonData)0;
        o.diffColor = diffColor;
        o.specColor = specColor;
        o.oneMinusReflectivity = oneMinusReflectivity;
        o.smoothness = smoothness;
        return o;
    }
    inline FragmentCommonData RoughnessSetup(float2 i_tex)
    {
        half2 metallicGloss = MetallicRough(i_tex.xy);
        half metallic = metallicGloss.x;
        half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

        half oneMinusReflectivity;
        half3 specColor;
        half3 diffColor = DiffuseAndSpecularFromMetallic(Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

        FragmentCommonData o = (FragmentCommonData)0;
        o.diffColor = diffColor;
        o.specColor = specColor;
        o.oneMinusReflectivity = oneMinusReflectivity;
        o.smoothness = smoothness;
        return o;
    }
    // parallax transformed texcoord is used to sample occlusion
    inline FragmentCommonData FragmentSetup (inout float2 i_tex, float3 i_eyeVec, float3 tangentToWorld[3], float3 i_posWorld)
    {

        half alpha = Alpha(i_tex.xy);

        FragmentCommonData o = UNITY_SETUP_BRDF_INPUT (i_tex);
        o.normalWorld = PerPixelWorldNormal(i_tex, tangentToWorld);
        o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
        o.posWorld = i_posWorld;

        // NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
        o.diffColor = PreMultiplyAlpha (o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);
        return o;
    }

    inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light, bool reflections)
    {
        UnityGIInput d;
        d.light = light;
        d.worldPos = s.posWorld;
        d.worldViewDir = -s.eyeVec;
        d.atten = atten;
        #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
            d.ambient = 0;
            d.lightmapUV = i_ambientOrLightmapUV;
        #else
            d.ambient = i_ambientOrLightmapUV.rgb;
            d.lightmapUV = 0;
        #endif

        d.probeHDR[0] = unity_SpecCube0_HDR;
        d.probeHDR[1] = unity_SpecCube1_HDR;
        #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
        d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
        #endif
        #ifdef UNITY_SPECCUBE_BOX_PROJECTION
        d.boxMax[0] = unity_SpecCube0_BoxMax;
        d.probePosition[0] = unity_SpecCube0_ProbePosition;
        d.boxMax[1] = unity_SpecCube1_BoxMax;
        d.boxMin[1] = unity_SpecCube1_BoxMin;
        d.probePosition[1] = unity_SpecCube1_ProbePosition;
        #endif

        if(reflections)
        {
            Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.smoothness, -s.eyeVec, s.normalWorld, s.specColor);
            // Replace the reflUVW if it has been compute in Vertex shader. Note: the compiler will optimize the calcul in UnityGlossyEnvironmentSetup itself
            #if UNITY_STANDARD_SIMPLE
                g.reflUVW = s.reflUVW;
            #endif

            return UnityGlobalIllumination (d, occlusion, s.normalWorld, g);
        }
        else
        {
            return UnityGlobalIllumination (d, occlusion, s.normalWorld);
        }
    }

    inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light)
    {
        return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, true);
    }
    //-------------------------------------------------------------------------------------
    half4 OutputForward (half4 output, half alphaFromSurface)
    {
        UNITY_OPAQUE_ALPHA(output.a);
        return output;
    }
    inline half4 VertexGIForward(appdata v, float3 posWorld, half3 normalWorld)
    {
        half4 ambientOrLightmapUV = 0;
        // Static lightmaps
        #ifdef LIGHTMAP_ON
            ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            ambientOrLightmapUV.zw = 0;
        // Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
        #elif UNITY_SHOULD_SAMPLE_SH
            #ifdef VERTEXLIGHT_ON
                // Approximated illumination from non-important point lights
                ambientOrLightmapUV.rgb = Shade4PointLights (
                    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                    unity_4LightAtten0, posWorld, normalWorld);
            #endif

            ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, ambientOrLightmapUV.rgb);
        #endif

        #ifdef DYNAMICLIGHTMAP_ON
            ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        #endif

        return ambientOrLightmapUV;
    }


    #define FRAGMENT_SETUP(x) FragmentCommonData x = \
        FragmentSetup(i.uv, i.eyeVec.xyz, i.btn, IN_WORLDPOS(i));



        
//LEGACY LIGHTING MODEL
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



#else
    half4 getMetallicSmoothness(float4 metallicGlossMap)
    {
        half roughness = 1-(_Glossiness * metallicGlossMap.a);
        roughness *= 1.7 - 0.7 * roughness;
        half metallic = metallicGlossMap.r * _Metallic;
        return half4(metallic, 0, 0, roughness);
    }

    //Reflection direction, worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax
    float3 getReflectionUV(float3 direction, float3 position, float4 cubemapPosition, float3 boxMin, float3 boxMax) 
    {
        #if UNITY_SPECCUBE_BOX_PROJECTION
            if (cubemapPosition.w > 0) {
                float3 factors = ((direction > 0 ? boxMax : boxMin) - position) / direction;
                float scalar = min(min(factors.x, factors.y), factors.z);
                direction = direction * scalar + (position - cubemapPosition);
            }
        #endif
        return direction;
    }

    half3 getIndirectSpecular(float3 worldPos, float3 diffuseColor, float vdn, float4 metallicSmoothness, half3 reflDir, half3 indirectLight, float3 viewDir, float3 lighting)
    {	//This function handls Unity style reflections, Matcaps, and a baked in fallback cubemap.
        half3 spec = half3(0,0,0);
        #if defined(UNITY_PASS_FORWARDBASE)
            float3 reflectionUV1 = getReflectionUV(reflDir, worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
            half4 probe0 = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectionUV1, metallicSmoothness.w * 6);
            half3 probe0sample = DecodeHDR(probe0, unity_SpecCube0_HDR);

            float3 indirectSpecular;
            float interpolator = unity_SpecCube0_BoxMin.w;
            
            UNITY_BRANCH
            if (interpolator < 0.99999) 
            {
                float3 reflectionUV2 = getReflectionUV(reflDir, worldPos, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);
                half4 probe1 = UNITY_SAMPLE_TEXCUBE_SAMPLER_LOD(unity_SpecCube1, unity_SpecCube0, reflectionUV2, metallicSmoothness.w * 6);
                half3 probe1sample = DecodeHDR(probe1, unity_SpecCube1_HDR);
                indirectSpecular = lerp(probe1sample, probe0sample, interpolator);
            }
            else 
            {
                indirectSpecular = probe0sample;
            }

            half3 metallicColor = indirectSpecular * lerp(0.05,diffuseColor.rgb, metallicSmoothness.x);
            spec = lerp(indirectSpecular, metallicColor, pow(vdn, 0.05));
            spec = lerp(spec, spec * lighting, metallicSmoothness.w); // should only not see shadows on a perfect mirror.
            
            #if defined(LIGHTMAP_ON)
                float specMultiplier = max(0, lerp(1, pow(length(lighting), _SpecLMOcclusionAdjust), _SpecularLMOcclusion));
                spec *= specMultiplier;
            #endif
        #endif
        return spec;
    }

    half3 getDirectSpecular(half4 lightCol, half3 diffuseColor, half4 metallicSmoothness, float rdv, float atten, float NdotH)
    {	
        #ifdef _LIGHTING_MODEL
            half roughness = max(0.0001, 1-metallicSmoothness.w);
            float roughnessSqr = roughness*roughness;
            float NdotHSqr = NdotH*NdotH;
            float TanNdotHSqr = (1-NdotHSqr)/NdotHSqr;
            return (1.0/3.1415926535) * sqrt(roughness/(NdotHSqr * (roughnessSqr + TanNdotHSqr))) * lightCol;
        #else
            half smoothness = max(0.0001, 1-metallicSmoothness.w);
            smoothness *= 1.7 - 0.7 * smoothness;
            
            half3 specularReflection = saturate(pow(rdv, smoothness * 128)) * lightCol;
            specularReflection = lerp(specularReflection, specularReflection * diffuseColor, metallicSmoothness.x);
            specularReflection *= lerp(0,5, smoothness * 0.05); //Artificially brighten to be as bright as standard
            return specularReflection * atten;
        #endif
    }

    float3 getNormal(float3 normalMap, float3 bitangent, float3 tangent, float3 worldNormal)
    {
        half3 tspace0 = half3(tangent.x, bitangent.x, worldNormal.x);
        half3 tspace1 = half3(tangent.y, bitangent.y, worldNormal.y);
        half3 tspace2 = half3(tangent.z, bitangent.z, worldNormal.z);

        half3 nMap = normalMap;
        nMap.xy *= _BumpScale;

        half3 calcedNormal;
        calcedNormal.x = dot(tspace0, nMap);
        calcedNormal.y = dot(tspace1, nMap);
        calcedNormal.z = dot(tspace2, nMap);

        return normalize(calcedNormal);
    }

    half3 getRealtimeLightmap(float2 uv, float3 worldNormal)
    {
        float2 realtimeUV = uv * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        float4 bakedCol = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, realtimeUV);
        float3 realtimeLightmap = DecodeRealtimeLightmap(bakedCol);

        #ifdef DIRLIGHTMAP_COMBINED
            half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, realtimeUV);
            realtimeLightmap += DecodeDirectionalLightmap (realtimeLightmap, realtimeDirTex, worldNormal);
        #endif
        
        return realtimeLightmap * _RTLMStrength;
    }

    half3 getLightmap(float2 uv, float3 worldNormal, float3 worldPos)
    {
        float2 lightmapUV = uv * unity_LightmapST.xy + unity_LightmapST.zw;
        half4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, lightmapUV);
        half3 lightMap = DecodeLightmap(bakedColorTex);
        
        #ifdef DIRLIGHTMAP_COMBINED
            fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, lightmapUV);
            lightMap = DecodeDirectionalLightmap(lightMap, bakedDirTex, worldNormal);
        #endif
        return lightMap * _LMStrength;
    }

    half3 getLightDir(float3 worldPos)
    {
        half3 lightDir = UnityWorldSpaceLightDir(worldPos);
        
        half3 probeLightDir = unity_SHAr.xyz + unity_SHAg.xyz + unity_SHAb.xyz;
        lightDir = (lightDir + probeLightDir); //Make light dir the average of the probe direction and the light source direction.

            #if !defined(POINT) && !defined(SPOT) // if the average length of the light probes is null, and we don't have a directional light in the scene, fall back to our fallback lightDir
                if(length(unity_SHAr.xyz*unity_SHAr.w + unity_SHAg.xyz*unity_SHAg.w + unity_SHAb.xyz*unity_SHAb.w) == 0 && ((_LightColor0.r+_LightColor0.g+_LightColor0.b) / 3) < 0.1)
                {
                    lightDir = float4(1, 1, 1, 0);
                }
            #endif

        return normalize(lightDir);
    }



    //Triplanar map a texture (Object or World space), or sample it normally.
    float4 texTP( sampler2D tex, float4 tillingOffset, float3 worldPos, float3 objPos, float3 worldNormal, float3 objNormal, float falloff, float2 uv)
    {
        if(_TextureSampleMode != 0){
            
            worldPos = lerp(worldPos, objPos, _TextureSampleMode - 1);
            worldNormal = lerp(worldNormal, objNormal, _TextureSampleMode - 1);
            
            float3 projNormal = pow(abs(worldNormal),falloff);
            projNormal /= projNormal.x + projNormal.y + projNormal.z;
            float3 nsign = sign(worldNormal);
            half4 xNorm; half4 yNorm; half4 zNorm;
            xNorm = tex2D( tex, tillingOffset.xy * worldPos.zy * float2( nsign.x, 1.0 ) + tillingOffset.zw);
            yNorm = tex2D( tex, tillingOffset.xy * worldPos.xz * float2( nsign.y, 1.0 ) + tillingOffset.zw);
            zNorm = tex2D( tex, tillingOffset.xy * worldPos.xy * float2( -nsign.z, 1.0 ) + tillingOffset.zw);

            return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
        }
        else{
            return tex2D(tex, uv);
        } 
    }
    //same as above but for normal maps
    float3 texTPNorm( sampler2D tex, float4 tillingOffset, float3 worldPos, float3 objPos, float3 worldNormal, float3 objNormal, float falloff, float2 uv)
    {
        if(_TextureSampleMode != 0){
            
            worldPos = lerp(worldPos, objPos, _TextureSampleMode - 1);
            worldNormal = lerp(worldNormal, objNormal, _TextureSampleMode - 1);

            float3 projNormal = pow(abs(worldNormal), falloff);
            projNormal /= projNormal.x + projNormal.y + projNormal.z;
            float3 nsign = sign(worldNormal);
            half4 xNorm; half4 yNorm; half4 zNorm;
            xNorm = tex2D( tex, tillingOffset.xy * worldPos.zy * float2( nsign.x, 1.0 ) + tillingOffset.zw);
            yNorm = tex2D( tex, tillingOffset.xy * worldPos.xz * float2( nsign.y, 1.0 ) + tillingOffset.zw);
            zNorm = tex2D( tex, tillingOffset.xy * worldPos.xy * float2( -nsign.z, 1.0 ) + tillingOffset.zw);

            xNorm.xyz = UnpackNormal(xNorm);
            yNorm.xyz = UnpackNormal(yNorm);
            zNorm.xyz = UnpackNormal(zNorm);
            
            return (xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z);
        }
        else{
            return UnpackNormal(tex2D(tex, uv));
        }
    }

    float shEvaluateDiffuseL1Geomerics(float L0, float3 L1, float3 n)
    {
        // average energy
        float R0 = L0;

        // avg direction of incoming light
        float3 R1 = 0.5f * L1;

        // directional brightness
        float lenR1 = length(R1);

        // linear angle between normal and direction 0-1
        //float q = 0.5f * (1.0f + dot(R1 / lenR1, n));
        //float q = dot(R1 / lenR1, n) * 0.5 + 0.5;
        float q = dot(normalize(R1), n) * 0.5 + 0.5;

        // power for q
        // lerps from 1 (linear) to 3 (cubic) based on directionality
        float p = 1.0f + 2.0f * lenR1 / R0;

        // dynamic range constant
        // should vary between 4 (highly directional) and 0 (ambient)
        float a = (1.0f - lenR1 / R0) / (1.0f + lenR1 / R0);

        return R0 * (a + (1.0f - a) * (p + 1.0f) * pow(q, p));
    }
#endif