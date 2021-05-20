#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
inline float CorrectedLinearEyeDepth(float z, float B)
{
	#if UNITY_REVERSED_Z
	if (z == 0)
		return LinearEyeDepth(z);
	#endif
	return 1.0 / (z/UNITY_MATRIX_P._34 + B);
}

float4 VolumetricLightingBRDF(v2f i)
{

    UNITY_SETUP_INSTANCE_ID(i);

	
	// if((ceil(i.color.r) != 0 && (ceil(i.color.g) != 1) && ceil(i.color.b) != 0 && (ceil(i.uv.x != 0.9)) && (ceil(i.uv.y != 0.9)) ))
	// {
		if((all(i.rgbColor <= float4(0.01,0.01,0.01,1)) || i.intensityStrobeGOBOSpinSpeed.x <= 0.01) && isOSC() == 1)
		{
			return half4(0,0,0,0);
		} 
		i.uv.x = saturate(i.uv.x);
		i.uv.y = saturate(i.uv.y);

		float fade = 0.0;
		float altFade = 1.0;
		float depthFade = 0.0;
		float distFade = 0.0;
		float fadeAmt = 0.0;
		half2 uvMap = half2(0.0,0.0);
		float4 intensityMod = float4(0,0,0,0);

		//replacement for _WorldSpaceCameraPos
		float3 wpos;
		wpos.x = unity_CameraToWorld[0][3];
		wpos.y = unity_CameraToWorld[1][3];
		wpos.z = unity_CameraToWorld[2][3];


		_SpinSpeed = IF(checkPanInvertY() == 1, -_SpinSpeed, _SpinSpeed);
        _SpinSpeed = IF(isOSC() == 1, _SpinSpeed * i.intensityStrobeGOBOSpinSpeed.z, _SpinSpeed);
		float spinSpeed = 0.0;
		//Inside Faces
		if(i.color.r < 0.90000)
		{

			float3 tspace0 = float3(i.tan.x, i.bitan.x, i.norm.x);
			float3 tspace1 = float3(i.tan.y, i.bitan.y, i.norm.y);
			float3 tspace2 = float3(i.tan.z, i.bitan.z, i.norm.z);

			float2 normUVs = i.uv;

			float3 nMap = UnpackNormal(tex2D(_InsideConeNormalMap, normUVs));
			float3 calcedNormal = float3(dot(tspace0, nMap), dot(tspace1, nMap), dot(tspace2, nMap));

			fadeAmt = 1-(_FadeAmt);
			//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
			float perspectiveDivide = 1.0f / i.pos.w;
			float4 direction = i.worldDirection * perspectiveDivide;
			//float2 altScreenPos = i.screenPos.xy * perspectiveDivide;
			//altScreenPos = UnityStereoTransformScreenSpaceTex(altScreenPos);
			float2 screenUV = i.screenPos.xy / i.screenPos.w;

			float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);
			sceneZ = CorrectedLinearEyeDepth(sceneZ, direction.w);
			//sceneZ = LinearEyeDepth(sceneZ);
			depthFade = saturate(fadeAmt * (sceneZ - i.screenPos.z));

			distFade = saturate(distance(i.worldPos.rgb, wpos) * _DistFade) ;
			float3 viewDir = normalize(wpos - i.worldPos);

			//viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));
			//viewDir = float3(viewDir.x,viewDir.y,viewDir.z);
			fade = pow(saturate(dot(normalize(calcedNormal), normalize(viewDir))), _FadeStrength);
			altFade = pow(saturate(dot(normalize(i.norm), viewDir)),pow(_FadeStrength,_InnerFadeStrength));
			uvMap = half2((i.uv.x * getConeLength()), i.uv.y);
			spinSpeed = (-_SpinSpeed) * UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
			i.uv2.x = i.uv2.x + _Time * 0.5;
			i.uv2.y = i.uv2.y + _Time * 0.1;
			intensityMod = lerp(float4(0,0,0,0), float4(1,1,1,0), (pow(i.uv.x,_InnerIntensityCurve)));
			fade *=intensityMod;
			//fade = (pow(max(0, dot(i.norm, -viewDir)), pow(_FadeStrength,_InnerFadeStrength)));
			
		}
		//Outside Faces
		else
		{
			fadeAmt = 1-(_FadeAmt);
			//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
			float perspectiveDivide = 1.0f / i.pos.w;
			float4 direction = i.worldDirection * perspectiveDivide;
			float2 altScreenPos = i.screenPos.xy * perspectiveDivide;

			float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, altScreenPos);

			sceneZ = CorrectedLinearEyeDepth(sceneZ, direction.w);
			depthFade = saturate(fadeAmt * (sceneZ - i.screenPos.z));

			distFade = saturate(distance(i.worldPos.rgb, wpos) * _DistFade) ;
    		float3 viewDir = normalize(wpos - i.worldPos);
			//viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));
			fade = pow(saturate(dot(normalize(i.norm), viewDir)), _FadeStrength);
			uvMap = half2(i.uv.x * getConeLength(), i.uv.y);
			//fade = (pow(max(0, dot(i.norm, -viewDir)), _FadeStrength));
			spinSpeed = (-_SpinSpeed) * UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
			i.uv2.x = i.uv2.x + (_Time * -0.5);
			i.uv2.y = i.uv2.y + (_Time * -0.1);



		}
		// float4 targetWrld = mul(_FixtureRotationOrigin, unity_ObjectToWorld);
		// float theta = (acos(dot(targetWrld.xyz, wpos)/(length(targetWrld.xyz) * length(wpos))));

		fixed4 col = tex2D(_LightMainTex, uvMap) * ((sin(_Time.y * _PulseSpeed) * 0.5 + 1));
		col = ((((col*fade) * altFade) * depthFade) * distFade) * _FixtureMaxIntensity;
		// col *= fade;
		// col *= altFade;
		// col *= depthFade;
		// col *= distFade;
		// col *= _FixtureMaxIntensity;


		//UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));


		float strobe = IF(isStrobe() == 1, i.intensityStrobeGOBOSpinSpeed.y, 1);
		col = col * getEmissionColor();
		col = lerp(col, fixed4(0,0,0,0), pow(i.uv.x,.5));
		//Beam Divider and spin
		const float pi = 3.14159265;
		float divide = (sin(i.uv.y * pi * floor(_StripeSplit) * 2 + (_Time.w * spinSpeed)) + 1.0);
		divide = lerp(1.0, divide, _StripeSplitStrength);


		//noise apply
		float2 texUV = i.uv2;
		
		float randVal = frac(sin((_NoiseSeed + 1) * 10000)); // 適当なランダム値
		texUV += float2((i.worldPos.x + (_NoiseSeed + 1) * 2.345 + randVal) * 0.2357 * 0.01, (i.worldPos.y + (_NoiseSeed + 1) * 8.345 + randVal) * 0.324643 *  0.1 + 5);
		
		float4 tex = tex2D(_NoiseTex, texUV);
		tex = lerp(fixed4(1, 1, 1, 1), tex, _NoisePower);
		col *= tex;
		col *= divide;

		float4 result = IF(isOSC() == 1, lerp(fixed4(0,0,0,col.w),(((col) * i.rgbColor) * strobe),i.intensityStrobeGOBOSpinSpeed.x * _FixtureMaxIntensity), (col) * strobe);
		result = IF(isOSC() == 1,lerp(half4(0,0,0,result.w), result, i.intensityStrobeGOBOSpinSpeed.x), result);
		result = IF(i.intensityStrobeGOBOSpinSpeed.x <= _IntensityCutoff && isOSC() == 1, half4(0,0,0,result.w), result);
		// result.xyz = rgb2hsv(result.xyz);
		result = clamp(0,3, result);
		float resultAvg = ((result.x + result.y + result.z)/3)*1.5;
		float4 resultAvgStrength = lerp(result, float4(resultAvg,resultAvg,resultAvg,result.a), _Saturation);
		result = lerp(resultAvgStrength, result, pow(uvMap.x, _SaturationLength));
		// result.xyz = hsv2rgb(result.xyz);

		if(i.color.r >= 0.9)
		{
			result = result * 4;
		}

		result = lerp(half4(0,0,0,result.w), result, getGlobalIntensity());
		result = lerp(half4(0,0,0,result.w), result, getFinalIntensity());
		result = result * _UniversalIntensity;
		return result;
	// }
	// else
	// {
	// 	return fixed4(0,0,0,0);
	// }

    
}