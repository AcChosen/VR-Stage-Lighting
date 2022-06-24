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
    UNITY_SETUP_INSTANCE_ID(i);
    // UNITY_SETUP_INSTANCE_ID(i);
    // if(((i.uv.x) == 0.0 && (i.uv.y) == 0.0 || _PureEmissiveToggle == 1))
    // {
    //     //Color Light Bulb Itself
    //     float strobe = IF(isStrobe() == 1, GetStrobeOutput(getChannelSectorX()), 1);
    //     return IF(isOSC() == 1, (getEmissionColor() * GetOSCColor(getChannelSectorX())) * strobe, getEmissionColor() * strobe);
    // }

    if ((((i.uv.x) == 0.9 && (i.uv.y) == 0.9) || (5.0 <= ceil(i.color.g * 10)) <= 7.0 && ceil(i.color.r) != 0 && ceil(i.color.b) != 0))
    {
        discard;
        return float4(0,0,0,0);      
    }

    else if((!(ceil(i.color.r) != 0 && ceil(i.color.g) != 1 && ceil(i.color.b) != 0)) || _PureEmissiveToggle != 0)
    {
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
        float3 directSpecular = getDirectSpecular(lightCol, diffuseColor, metallicSmoothness, rdv, attenuation);

        lighting = diffuse * (directDiffuse + indirectDiffuse); 
        lighting += directSpecular; 
        lighting += indirectSpecular;

        float al = 1;
        #if defined(alphablend)
            al = diffuseColor.a;
        #endif

        //LIGHT BULB COLOR/////////////////////////
        if(((i.uv.x) == 0.0 && (i.uv.y) == 0.0 || _PureEmissiveToggle == 1))
        {
            //uint sector = getChannelSectorX();
            //Color Light Bulb Itself

//            float strobe = IF(isStrobe() == 1, i.intensityStrobe.y, 1);
//            float4 emission = IF(isOSC() == 1, (getEmissionColor() * i.rgbColor) * strobe, getEmissionColor() * strobe);
            float4 emission = getEmissionColor();
            // if((all(i.rgbColor <= float4(0.01,0.01,0.01,1)) || i.intensityStrobe.x <= 0.01) && isOSC() == 1)
            // {
            //    return float4(lighting, al);
            // } 
            float intensities = getGlobalIntensity() * getFinalIntensity() * _UniversalIntensity;
            emission *=(_FixtureMaxIntensity)*1500;
            emission = clamp(emission, 0, (_LensMaxBrightness*100 * intensities));
            //lighting += emission;
            //lighting = lerp(lighting, emission, GetOSCIntensity(getChannelSectorX(), _FixtureMaxIntensity));
            half limit = 0.025;
            // if((all(i.rgbColor >=half4(limit,limit,limit,1)) || i.intensityStrobe.x >= limit) && isOSC() == 1)
            // {
            //     float4 potentialBrightness = emission * _FixutreIntensityMultiplier; 
            //     emission = lerp(emission, potentialBrightness, pow(i.intensityStrobe.x, 1.9));          
            // }
            // else
            // {
            //     if(isOSC() == 1)
            //     {
            //         emission = half4(0,0,0,1.0f);                   
            //     }
            // }
            //emission *= _FixutreIntensityMultiplier;
            #ifndef RAW
                emission = lerp((half4(0,0,0,emission.w)), emission, GetAudioReactAmplitude());
            #endif
            emission = lerp((half4(0,0,0,emission.w)), emission, intensities);
            //emission = lerp((half4(0,0,0,emission.w)), emission, );
           // emission = emission * _UniversalIntensity;
            //emission = clamp(emission, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
            //lighting = IF(isOSC() == 1, lerp(lighting, emission, pow(0.1, (GetOSCIntensity(sector, _FixtureMaxIntensity)))) ,lighting + emission);

            #ifdef WASH
            emission = i.uv1.y > 0.0 ? saturate(emission) - 0.25 : emission;
            #endif
            //lighting = IF(isOSC() == 1,lerp(lighting, emission, pow(i.intensityStrobe.x, 1.0)), emission);
            lighting = emission;
            
            float lightingAVG = (lighting.x + lighting.y + lighting.z)/3;
            #ifdef RAW
                lighting = lerp(lighting,float3(lightingAVG, lightingAVG, lightingAVG), pow(_Saturation,2));
            #else
                lighting = lerp(lighting,float3(lightingAVG, lightingAVG, lightingAVG), pow(_Saturation,1));
            #endif
            

            //lighting = clamp(lighting,0, _LensMaxBrightness*100);
            return float4(lighting, al);
        }
        else
        {
            return float4(lighting, al);
        }
        //////////////////////
        

    }

    else
    {
    discard;
    return 0;

    
    }



}