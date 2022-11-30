//This file contains all of the neccisary functions for lighting to work a'la standard shading.
//Feel free to add to this.

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

half3 getDirectSpecular(half4 lightCol, half3 diffuseColor, half4 metallicSmoothness, float rdv, float atten)
{	
	half smoothness = max(0.0001, 1-metallicSmoothness.w);
	smoothness *= 1.7 - 0.7 * smoothness;
	
    half3 specularReflection = saturate(pow(rdv, smoothness * 128)) * lightCol;
	specularReflection = lerp(specularReflection, specularReflection * diffuseColor, metallicSmoothness.x);
    specularReflection *= lerp(0,5, smoothness * 0.05); //Artificially brighten to be as bright as standard
    return specularReflection * atten;
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