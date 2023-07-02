//Since this is shared, and the output structs/input structs are all slightly differently named in each shader template, just handle them all here.
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));
float4 CustomStandardLightingBRDF(
    #if defined(GEOMETRY)
        v2f i
    #elif defined(TESSELLATION)
        vertexOutput i
    #else
        v2f i
    #endif
    )
{



    //UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    UNITY_SETUP_INSTANCE_ID(i);
    // UNITY_SETUP_INSTANCE_ID(i);
    // if(((i.uv.x) == 0.0 && (i.uv.y) == 0.0 || _PureEmissiveToggle == 1))
    // {
    //     //Color Light Bulb Itself
    //     float strobe = IF(isStrobe() == 1, GetStrobeOutput(getChannelSectorX()), 1);
    //     return IF(isDMX() == 1, (getEmissionColor() * GetDMXColor(getChannelSectorX())) * strobe, getEmissionColor() * strobe);
    // }

    if ((((i.uv.x) == 0.9 && (i.uv.y) == 0.9) || (5.0 <= ceil(i.color.g * 10)) <= 7.0 && ceil(i.color.r) != 0 && ceil(i.color.b) != 0))
    {
        discard;
        return float4(0,0,0,0);      
    }

    else if((!(ceil(i.color.r) != 0 && ceil(i.color.g) != 1 && ceil(i.color.b) != 0)) || _PureEmissiveToggle != 0)
    {
        #ifdef _LIGHTING_MODEL
        UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

        FRAGMENT_SETUP(s)
        

        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        UnityLight mainLight = MainLight ();
        UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

        half occlusion = Occlusion(i.uv);
        UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

        half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
        // c.rgb += DynamicLM(i.tex.xy);
        // c.rgb += Emission(i.tex.xy);
        UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
        UNITY_APPLY_FOG(_unity_fogCoord, c.rgb);








        float3 lighting = c.rgb;
        float al = s.alpha;
        #else
            //LIGHTING PARAMS
            UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
            float3 lightDir = getLightDir(i.worldPos);
            float4 lightCol = _LightColor0;

            //NORMAL
            float3 normalMap = texTPNorm(_BumpMap, _MainTex_ST, i.worldPos, i.objPos, i.btn[2], i.objNormal, _TriplanarFalloff, i.uv);
            float3 worldNormal = getNormal(normalMap, i.btn[0], i.btn[1], i.btn[2]);

            //METALLIC SMOOTHNESS
            float4 metallicGlossMap = texTP(_MetallicGlossMap, _MainTex_ST, i.worldPos, i.objPos, i.btn[2], i.objNormal, _TriplanarFalloff, i.uv);
            float4 metallicSmoothness = getMetallicSmoothness(metallicGlossMap);

            //DIFFUSE
        //   fixed4 diffuse = texTP(_MainTex, _MainTex_ST, i.worldPos, i.objPos, i.btn[2], i.objNormal, _TriplanarFalloff, i.uv) * _Color;
            fixed4 diffuse = lerp(texTP(_MainTex, _MainTex_ST, i.worldPos, i.objPos, i.btn[2], i.objNormal, _TriplanarFalloff, i.uv) * _Color, _Color, lerp(0, i.color.r, i.color.b));
            fixed4 diffuseColor = diffuse; //Store for later use, we alter it after.
            diffuse.rgb *= (1-metallicSmoothness.x);

            //LIGHTING VECTORS
            float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            float3 halfVector = normalize(lightDir + viewDir);
            float3 reflViewDir = reflect(-viewDir, worldNormal);
            float3 reflLightDir = reflect(lightDir, worldNormal);

            //DOT PRODUCTS FOR LIGHTING
            float ndl = saturate(dot(lightDir, worldNormal));
            float NdotH = max(0.0,dot(worldNormal, halfVector));
            float vdn = abs(dot(viewDir, worldNormal));
            float rdv = saturate(dot(reflLightDir, float4(-viewDir, 0)));

            //LIGHTING
            float3 lighting = float3(0,0,0);

            #if defined(LIGHTMAP_ON)
                float3 indirectDiffuse = 0;
                float3 directDiffuse = getLightmap(i.uv1, worldNormal, i.worldPos);
                #if defined(DYNAMICLIGHTMAP_ON)
                    float3 realtimeLM = getRealtimeLightmap(i.uv2, worldNormal);
                    directDiffuse += realtimeLM;
                #endif
            #else
                float3 indirectDiffuse;
                if(_LightProbeMethod == 0)
                {
                    indirectDiffuse = ShadeSH9(float4(worldNormal, 1));
                }
                else
                {
                    float3 L0 = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                    indirectDiffuse.r = shEvaluateDiffuseL1Geomerics(L0.r, unity_SHAr.xyz, worldNormal);
                    indirectDiffuse.g = shEvaluateDiffuseL1Geomerics(L0.g, unity_SHAg.xyz, worldNormal);
                    indirectDiffuse.b = shEvaluateDiffuseL1Geomerics(L0.b, unity_SHAb.xyz, worldNormal);
                }

                float3 directDiffuse = ndl * attenuation * _LightColor0;
            #endif

            float3 indirectSpecular = getIndirectSpecular(i.worldPos, diffuseColor, vdn, metallicSmoothness, reflViewDir, indirectDiffuse, viewDir, directDiffuse);
            float3 directSpecular = getDirectSpecular(lightCol, diffuseColor, metallicSmoothness, rdv, attenuation, NdotH);

            lighting = diffuse * (directDiffuse + indirectDiffuse); 
            lighting += directSpecular; 
            lighting += indirectSpecular;

            float al = 1;
            #if defined(alphablend)
                al = diffuseColor.a;
            #endif
            
        #endif
        
        
        //LIGHT BULB COLOR/////////////////////////
        #ifdef VRSL_DMX
            if(((i.uv.x) == 0.0 && (i.uv.y) == 0.0 || _PureEmissiveToggle == 1))
            {
                float strobe = IF(isStrobe() == 1, i.intensityStrobe.y, 1);
                float4 emission = IF(isDMX() == 1, (getEmissionColor() * i.rgbColor) * strobe, getEmissionColor() * strobe);

                emission *=(_FixtureMaxIntensity)*1500;
                emission = clamp(emission, 0, _LensMaxBrightness*100);
                half limit = 0.025;
                if((all(i.rgbColor >=half4(limit,limit,limit,1)) || i.intensityStrobe.x >= limit) && isDMX() == 1)
                {
                    float4 potentialBrightness = emission * _FixutreIntensityMultiplier; 
                    emission = lerp(emission, potentialBrightness, pow(i.intensityStrobe.x, 1.9));          
                }
                else
                {
                    if(isDMX() == 1)
                    {
                        emission = half4(0,0,0,1.0f);                   
                    }
                }
                emission = lerp((half4(0,0,0,emission.w)), emission, getGlobalIntensity());
                emission = lerp((half4(0,0,0,emission.w)), emission, getFinalIntensity());
                emission = emission * _UniversalIntensity;   
                lighting += emission;

                return float4(lighting, al);
            }
            else
            {
                lighting += (tex2D(_DecorativeEmissiveMap, i.uv) * _DecorativeEmissiveMapStrength);
                return float4(lighting, al);
            }
        #endif
        #ifdef VRSL_AUDIOLINK
            if(((i.uv.x) == 0.0 && (i.uv.y) == 0.0 || _PureEmissiveToggle == 1))
            {
                float4 emission = getEmissionColor();
                float intensities = getGlobalIntensity() * getFinalIntensity() * _UniversalIntensity;
                emission *=(_FixtureMaxIntensity)*1500;
                emission = clamp(emission, 0, (_LensMaxBrightness*100 * intensities));
                half limit = 0.025;

                #ifndef RAW
                    emission = lerp((half4(0,0,0,emission.w)), emission, GetAudioReactAmplitude());
                #endif
                emission = lerp((half4(0,0,0,emission.w)), emission, intensities);

                #ifdef WASH
                emission = i.uv1.y > 0.0 ? saturate(emission) - 0.25 : emission;
                #endif
                lighting = emission;
                
                float lightingAVG = (lighting.x + lighting.y + lighting.z)/3;
                #ifdef RAW
                    lighting = lerp(lighting,float3(lightingAVG, lightingAVG, lightingAVG), pow(_Saturation,2));
                #else
                    lighting = lerp(lighting,float3(lightingAVG, lightingAVG, lightingAVG), pow(_Saturation,1));
                #endif

                return float4(lighting, al);
            }
            else
            {
                lighting += (tex2D(_DecorativeEmissiveMap, i.uv) * _DecorativeEmissiveMapStrength);
                return float4(lighting, al);
            }
        #endif
        //////////////////////
        

    }

    else
    {
    discard;
    return 0;

    
    }



}