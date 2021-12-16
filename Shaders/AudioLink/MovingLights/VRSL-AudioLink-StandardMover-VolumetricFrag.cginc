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

float3 RGB2HSV(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsb2rgb( float3 c ){
    float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
    rgb = rgb*rgb*(3.0-2.0*rgb);
    return c.z * lerp( float3(1,1,1), rgb, c.y);
}

float4 VolumetricLightingBRDF(v2f i)
{

    UNITY_SETUP_INSTANCE_ID(i);

	
	// if((ceil(i.color.r) != 0 && (ceil(i.color.g) != 1) && ceil(i.color.b) != 0 && (ceil(i.uv.x != 0.9)) && (ceil(i.uv.y != 0.9)) ))
	// {
		float audioReact = i.audioGlobalFinalIntensity.x;
		float globalintensity = i.audioGlobalFinalIntensity.y;
		float finalintensity = i.audioGlobalFinalIntensity.z;
		float4 emissionTint = i.emissionColor;
		if(globalintensity <= 0.005 || finalintensity <= 0.005 || audioReact <= 0.005 || all(emissionTint<= float4(0.005, 0.005, 0.005, 1.0)))
		{
			return half4(0,0,0,0);
		}
		// if((all(i.rgbColor <= float4(0.01,0.01,0.01,1)) || i.intensityStrobeGOBOSpinSpeed.x <= 0.01) && isOSC() == 1)
		// {
		// 	return(0,0,0,0);
		// } 
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
        //_SpinSpeed = IF(isOSC() == 1, _SpinSpeed * i.intensityStrobeGOBOSpinSpeed.z, _SpinSpeed);
		float spinSpeed = 0.0;
		float fadeStrength = _FadeStrength + lerp(2.0, 0.0, clamp(0,1,getConeWidth()));
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
			float4 fixturepos = mul(unity_ObjectToWorld, (_FixtureRotationOrigin));
			float3 viewDir = normalize((wpos) - (i.worldPos + fixturepos));
			viewDir = viewDir * 1.05;
			//viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));
			//viewDir = float3(viewDir.x,viewDir.y,viewDir.z);
			fade = pow(saturate(dot(normalize(calcedNormal), normalize(viewDir))), fadeStrength);
			altFade = pow(saturate(dot(normalize(i.norm), (viewDir))),pow(fadeStrength,_InnerFadeStrength));
			float fixtureBrightness = pow(saturate(dot(normalize(i.norm), (viewDir))),pow(fadeStrength,0.0));
			altFade = lerp(altFade, fixtureBrightness, i.uv.x);
		//	uvMap = half2((i.uv.x * getConeLength()), i.uv.y);
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
			viewDir = viewDir * 1.05;
			//viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));
			fade = pow(saturate(dot(normalize(i.norm), viewDir)), fadeStrength);
		//	uvMap = half2(i.uv.x * getConeLength(), i.uv.y);
			//fade = (pow(max(0, dot(i.norm, -viewDir)), _FadeStrength));
			spinSpeed = (-_SpinSpeed) * UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
			i.uv2.x = i.uv2.x + (_Time * -0.5);
			i.uv2.y = i.uv2.y + (_Time * -0.1);
		}
		uvMap = half2(saturate(i.uv.x * getConeLength()), i.uv.y);
		fixed4 gradientTexture = tex2D(_LightMainTex, uvMap);
		//float4 gradientTexture = lerp(float4(1,1,1,1), float4(0,0,0,0), pow(clamp(0,1,uvMap.x),0.5));
		fixed4 col = gradientTexture.r * ((sin(_Time.y * _PulseSpeed) * 0.5 + 1));
		col = ((((col*fade) * altFade) * depthFade) * distFade) * _FixtureMaxIntensity;


		//UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));


//		float strobe = IF(isStrobe() == 1, i.intensityStrobeGOBOSpinSpeed.y, 1);
		col = col * emissionTint;
		

		float3 newCol = RGB2HSV(col.rgb);
		newCol = float3(newCol.x, clamp(newCol.y,0.0, 1.0)-.1, newCol.z);
		col.xyz = lerp(col.xyz,hsb2rgb(newCol), gradientTexture.r);
		col*= (i.blindingEffect * i.blindingEffect);

		col = lerp(col, fixed4(0,0,0,0), pow(i.uv.x,.5));
		//Beam Splitter and spin
		const float pi = 3.14159265;
		float divide = (sin(i.uv.y * pi * floor(_StripeSplit) * 2 + (_Time.w * spinSpeed)) + 1.0);
		float splitstr = IF(_GoboBeamSplitEnable == 1 && instancedGOBOSelection() > 1, _StripeSplitStrength, 0);
		divide = lerp(1.0, divide, splitstr);


		//noise apply
		float2 texUV = i.uv2;
		
		float4 tex = tex2D(_NoiseTex, texUV);
		tex = lerp(fixed4(1, 1, 1, 1), tex, _NoisePower);
		col *= tex;
		col *= divide;

		float4 result = col;
		float lm = length(wpos - i.worldPos);
		lm = clamp(lm * 0.3, 0.0, 1.0) - 0.0;
		col *= float4(lm, lm, lm, 1.0);

		if(i.color.r >= 0.9)
		{
			result = result * 4;
		}
		result = lerp(half4(0,0,0,result.w), result, audioReact * audioReact);
		result = lerp(half4(0,0,0,result.w), result, ((globalintensity) * globalintensity));
		result = lerp(half4(0,0,0,result.w), result, ((finalintensity) * finalintensity));
		result = result * _UniversalIntensity;
		return result;
	// }
	// else
	// {
	// 	return fixed4(0,0,0,0);
	// }

    
}