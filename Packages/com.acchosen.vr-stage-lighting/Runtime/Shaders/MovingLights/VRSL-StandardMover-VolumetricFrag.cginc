// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

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
float Fresnel(float3 Normal, float3 ViewDir, float Power)
{
    return pow(max(0, dot(Normal, -ViewDir)), Power);
}
#ifndef _POTATO_MODE_ON
	//3D noise based on iq's https://www.shadertoy.com/view/4sfGzS
	//HLSL conversion by Orels1~
	half Noise(float3 p)
	{
	float3 i = floor(p); p -= i; 
	p *= p * (3. - 2. * p);
	float2 uv = (p.xy + i.xy + float2(37, 17) * i.z + .5)*0.00390625;
	//uv.y *= -1;
	float4 uv4 = float4(uv.x, uv.y * -1, 0.0, 0.0);
	p.xy = tex2Dlod(_LightMainTex, uv4).yx;
	return lerp(p.x, p.y, p.z);
	}
#endif


float4 VolumetricLightingBRDF(v2f i, fixed facePos)
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

	#if defined(_POTATO_MODE_ON)
		float noise2Stretch = _Noise2StretchPotato;
		float noise2StretchInside = _Noise2StretchInsidePotato;
		float noise2X = _Noise2XPotato;
		float noise2Y = _Noise2YPotato;
		float noise2Z = _Noise2ZPotato;
		float noise2Power = _Noise2PowerPotato;
	#elif defined(_HQ_MODE)
		float noise2Stretch = _Noise2Stretch;
		float noise2StretchInside = _Noise2StretchInside;
		float noise2X = _Noise2X;
		float noise2Y = _Noise2Y;
		float noise2Z = _Noise2Z;
		float noise2Power = _Noise2Power;
	#else
		float noise2Stretch = _Noise2StretchDefault;
		float noise2StretchInside = _Noise2StretchInsideDefault;
		float noise2X = _Noise2XDefault;
		float noise2Y = _Noise2YDefault;
		float noise2Z = _Noise2ZDefault;
		float noise2Power = _Noise2PowerDefault;
	#endif


	//UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		UNITY_SETUP_INSTANCE_ID(i);


		#ifdef VRSL_DMX
			float gi = getGlobalIntensity();
			float fi = getFinalIntensity();
		#endif
		#ifdef VRSL_AUDIOLINK
			float audioReact = i.audioGlobalFinalIntensity.x;
			float gi = i.audioGlobalFinalIntensity.y;
			float fi = i.audioGlobalFinalIntensity.z;
			//Get Emission Color
			float4 emissionTint = i.emissionColor;
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
			if(((all(i.rgbColor <= float4(0.005,0.005,0.005,1)) || i.intensityStrobeGOBOSpinSpeed.x <= 0.005) && isDMX() == 1) || gi <= 0.005 || fi <= 0.005)
			{
				//If the light is basically off, don't render anything.
				return half4(0,0,0,0);
			}
		#endif 
		#ifdef VRSL_AUDIOLINK
			if(gi <= 0.005 || fi <= 0.005 || audioReact <= 0.005 || all(emissionTint<= float4(0.005, 0.005, 0.005, 1.0)))
			{
				//If the light is basically off, don't render anything.
				return half4(0,0,0,0);
			}
		#endif




		half widthNormalized = i.coneWidth/5.5;
		//uint gobo = instancedGOBOSelection();
		//Select Gobo!
		//uint gobo = i.intensityStrobeGOBOSpinSpeed.w;
		#ifdef VRSL_DMX
			float spinSpeed = i.intensityStrobeGOBOSpinSpeed.z;
			spinSpeed = (-spinSpeed) * UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
		#endif
		#ifdef VRSL_AUDIOLINK
			float goboSpinSpeed = checkPanInvertY() == 1 ? -getGoboSpinSpeed() : getGoboSpinSpeed();
			float spinSpeed = UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
			spinSpeed *= -goboSpinSpeed;
		#endif

		//Initialize UVMap
		half2 uvMap = half2(0.0,0.0);

		//replacement for _WorldSpaceCameraPos
		float3 wpos;
		wpos.x = unity_CameraToWorld[0][3];
		wpos.y = unity_CameraToWorld[1][3];
		wpos.z = unity_CameraToWorld[2][3];

		//Get Gobo Spin speed information
		
		//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
		//Get Screen Pos UVs
		//float perspectiveDivide = 1.0f / i.pos.w;
		#ifdef _USE_DEPTH_LIGHT
			float4 direction = i.worldDirection * (1.0f / i.pos.w);
			float2 screenUV = i.screenPos.xy / i.screenPos.w;
			//Sampling Depth
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);
			//Correct for mirrors as Eye Depth
			depth = CorrectedLinearEyeDepth(depth, direction.w);
			//Convert to Raw Depth
			depth = (1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z);
			//Convert to Linear01 Deppth
			depth = Linear01Depth(depth);


			//Attempt to fade cone away when intersecting with another object
			float intersectionFade = saturate(((depth * _ProjectionParams.z) - i.screenPos.w));
			intersectionFade = lerp(1, intersectionFade, saturate(i.uv.x * _FadeAmt));
		#else
			float intersectionFade = 1.0;
		#endif
		//Attempt to fade cone away when intersecting with the camera.
		//float cameraFade = i.camAngleCamfade.y;

		//Fade the camera less when you are closer to the source of the light.
		#ifdef VRSL_DMX
			float cameraFade = i.camAngleCamfade.y;
		#endif
		#ifdef VRSL_AUDIOLINK
			float cameraFade = saturate(((distance(wpos, i.worldPos) - 0.5)));
		#endif

		cameraFade = lerp(1.0, cameraFade, saturate(pow(i.uv.x, 0.1)));

		//Set uv map properly and adjust for the length of the cone.
		#if _ALPHATEST_ON
			#ifdef WASH
				uvMap = half2(saturate(i.uv.x * (getConeLength() -3.5)), i.uv.y);
			#else
				uvMap = half2(saturate(i.uv.x * (getConeLength() - 1.85)), i.uv.y);
			#endif
		#else
			uvMap = half2(saturate(i.uv.x * getConeLength()), i.uv.y);
		#endif

		i.uv.x = saturate(i.uv.x * 1.5);

		#ifdef VRSL_DMX
			float gobo = i.intensityStrobeGOBOSpinSpeed.w;
		#endif
		#ifdef VRSL_AUDIOLINK
			uint gobo = instancedGOBOSelection();
		#endif

		//Generate gradient for cone.
		#ifndef _ALPHATEST_ON
			#ifndef _POTATO_MODE_ON
				half grad = gobo > 1 ? _GradientModGOBO * 0.6f : _GradientMod * 0.8f;
			#else
				half grad = gobo > 1 ? _GradientModGOBO * 0.95f : _GradientMod*1.75f;
			#endif
			fixed gradientTexture = saturate((pow(saturate(-uvMap.x + 0.95), grad)));
		#else
			half grad = 1.75;
			fixed gradientTexture = saturate((pow(saturate(-uvMap.x + 0.95), grad)));
		#endif
		
		fixed4 col = gradientTexture.r;

		//Calculate View Direction for fading edges.
		//float3 viewDir = normalize(wpos - i.worldPos.xyz);
		float3 viewDir = (wpos - i.worldPos.xyz);
		viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));
		//Initialize Edge Fade
		float edgeFade = 0.0;
		//Fade away edges of cone using a Fresnel calculation.
		//Set the strength of the fade based on which side of t he polygon.
		float threeDNoiseScale = 1.0;
		if(facePos > 0)
		{
			
			#if _ALPHATEST_ON
				#ifdef WASH
					float fadeSTR = 0.1 * (_InnerFadeStrength+2);
				#else
					float fadeSTR = 0.1 * (_InnerFadeStrength);
				#endif
			#else
				float fadeSTR = _InnerFadeStrength;
			#endif
			edgeFade = Fresnel(-i.objNormal, viewDir, fadeSTR);
			threeDNoiseScale = noise2Stretch;
		}
		else
		{

			//Reduce fade strength if closer to source of light
			
			#if _ALPHATEST_ON
				float fadeSTR = (lerp(1.0, _FadeStrength + (lerp(5.0, 1.0, widthNormalized)), saturate(pow(i.uv.x, 0.1) + 0.05))) * 0.5;
			#else
				float fadeSTR = lerp(1.0, _FadeStrength + (lerp(5.0, 1.0, widthNormalized)), saturate(pow(i.uv.x, 0.1) + 0.05));
			#endif
			edgeFade = Fresnel(i.objNormal, viewDir, fadeSTR);
			threeDNoiseScale = noise2StretchInside;
		}

		//Combine Gradient with emission color, intersection fade and camera fade.
		col = col * getEmissionColor() * intersectionFade * cameraFade;

		//Get Strobe information
		#ifdef VRSL_DMX
			float strobe = isStrobe() == 1 ? i.intensityStrobeGOBOSpinSpeed.y : 1;
		#endif
		#ifdef VRSL_AUDIOLINK
			float strobe = 1.0;
		#endif
		#ifndef _ALPHATEST_ON
			#ifndef _POTATO_MODE_ON
				//Generate 2D noise texture, add scroll effect, and map to cone.
				#ifdef _2D_NOISE_ON
					float2 texUV = i.uv2;
					float3 baseWorldPos = unity_ObjectToWorld._m03_m13_m23;
					texUV.x += baseWorldPos.z;
					texUV.y += baseWorldPos.x;
					texUV.x = (_Time.y*0.1) * 0.75f + texUV.x;
					texUV.y = (_Time.y*0.1) * 0.10f + texUV.y;
					
					#ifdef _HQ_MODE
						float4 tex = tex2D(_NoiseTexHigh, texUV);
					#else
						float4 tex = tex2D(_NoiseTex, texUV);
					#endif
				#else
					float4 tex = float4(1,1,1,1);
				#endif

				//initialize 3D noise value and 2D noise strength value
				float threeDNoise = 1.0f;
				half np = 0.0f;

				//If we are using 3D noise...
				#if (defined(_MAGIC_NOISE_ON_HIGH) && defined(_HQ_MODE)) || (defined(_MAGIC_NOISE_ON_MED) && !defined(_HQ_MODE))
				//if(_ToggleMagicNoise > 0)
				//{
					//Get vertex/frag position in worldspace
					float3 worldposNoise = i.worldPos.xyz;
					//Add Scrolling effect
					worldposNoise.x += ((_Time.y*0.1) * noise2X);
					worldposNoise.y += ((_Time.y*0.1) * noise2Y);
					worldposNoise.z += ((_Time.y*0.1) * noise2Z);
					//Add Tiling effect
					//float3 q = threeDNoiseScale * worldposNoise.xyz;
					float3 q = float3(0,0,0);
					q.x = threeDNoiseScale * worldposNoise.x;
					q.y = threeDNoiseScale * worldposNoise.y;
					q.z = threeDNoiseScale * worldposNoise.z;

					//Use IQ's noise calculation equation to calculate 3D noise in world space.
					//Currently only sampling the noise twice as sampling anymore isn't creating any visible improvements.
					threeDNoise = 0.5000f*Noise( q ); 
					#ifdef _HQ_MODE
						float3x3 m = float3x3( 0.00000,  0.80000,  0.60000,
											-0.80000,  0.36000, -0.48000,
											-0.60000, -0.48000,  0.64000);
						q = mul(m,q)*2.01;
						threeDNoise += 0.2500*Noise( q );
						q = mul(m,q)*2.02;
						threeDNoise += 0.1250*Noise( q ); 
						q = mul(m,q)*2.03;
						threeDNoise += 0.0625*Noise( q ); 
						q = mul(m,q)*2.01;
					#endif
					
					//If we aren't using gobos, remove the 2D noise effect
					np = gobo > 1 ? _NoisePower : 0.0;
					//Set 3D Noise Power
					#ifndef WASH
						float newNP = lerp(noise2Power - 0.2, noise2Power, gradientTexture.r);
						threeDNoise = lerp(1, threeDNoise, newNP);
					#else
						// float nnp = lerp(noise2Power * 5, noise2Power, saturate(-uvMap.x));
						threeDNoise = lerp(1, threeDNoise, noise2Power);
					#endif
				//}
				//If we aren't using 3D noise..
				#else
				//{
					//If we are using gobos, add another 0.2 to the 2D noise power strength.
					//np = gobo > 1 ? clamp(0,1,_NoisePower + 0.2) : _NoisePower; 
					#ifndef WASH
					np = gobo > 1 ? clamp(0,1,_NoisePower + 0.2) : _NoisePower; 
					#else
						//tex = tex2D(_NoiseTex, texUV);
						np = _NoisePower;
					#endif
				//}
				#endif
				//Mix 2D noise Power
				tex = lerp(fixed4(1, 1, 1, 1), tex, np);
			#endif
		#else
			float4 tex = float4(1,1,1,1);
		#endif


		//Mix in blinding effect.
		//col*= ((i.blindingEffect));
		// col = lerp(col, col*i.blindingEffect * i.blindingEffect * 10, gradientTexture);
		//col*=2.0;
		col = lerp(col, fixed4(0,0,0,0), pow(i.uv.x,.5));


		//Beam Splitter and gobo spin
		//const float pi = 3.14159265;
		//Choose split strength and pattern based on information in i.stripeinfo.
		#ifdef VRSL_DMX
			#if !defined(WASH)
			float splitter = (sin(i.uv.y * 3.14159265 * floor(i.stripeInfo.x) * 2.0f + ( spinSpeed * 5.0)) + 1.0);

			//Do not use beam splitting if we aren't using gobos.
			float splitstr = IF(_GoboBeamSplitEnable == 1 && gobo > 1, i.stripeInfo.y, 0);
			splitter = lerp(1.0, splitter, splitstr);
			#else
			float splitter = 1.0;
			#endif
		#endif
		#ifdef VRSL_AUDIOLINK
			#if !defined(WASH)
			//Beam Splitter and gobo spin
				const float pi = 3.14159265;
				//Choose split strength and pattern based on information in i.stripeinfo.
				float splitter = (sin(i.uv.y * pi * floor(i.stripeInfo.x) * 2 + (_Time.w * spinSpeed)) + 1.0);
				//Do not use beam splitting if we aren't using gobos.
				//float splitstr = IF(_GoboBeamSplitEnable == 1 && gobo > 1, i.stripeInfo.y, 0);
				float splitstr = _GoboBeamSplitEnable == 1 && gobo > 1 ? i.stripeInfo.y : 0;
				splitter = lerp(1.0, splitter, splitstr);
			#else
				float splitter = 1.0;
			#endif
		#endif

		//Mix in 2D noise, beam splitting, and 3D noise.
		// col *= tex;
		// col *= splitter;
		// col *= threeDNoise;
		#ifndef _ALPHATEST_ON
			#ifndef _POTATO_MODE_ON
				col *= tex * splitter * threeDNoise;
			#else
				col *= splitter;
			#endif	
		#else
			col *= splitter;
		#endif

		//Add more power to Inner side of cone
		float4 result = col;
		#if !defined(WASH)
		#if _ALPHATEST_ON
			result = lerp(col,col * 25, saturate(pow(gradientTexture, (_InnerIntensityCurve+20))));
		#else
			result = lerp(col,col * 25, saturate(pow(gradientTexture, _InnerIntensityCurve)));
		#endif
		#endif
		//#ifndef _ALPHATEST_ON
			//Mix in Frensel Edge Fade
			#if _ALPHATEST_ON
				
				result *= lerp(1.0, edgeFade, pow(gradientTexture.r, 0.25));
				
			#else
				result *= edgeFade;
			#endif
		//#endif
		float maxIntensity = _FixtureMaxIntensity;
		#ifdef _HQ_MODE
		maxIntensity *= 0.75;
		#endif
		#ifdef VRSL_DMX
			if(isDMX() == 1)
			{
				result = lerp(fixed4(0,0,0,result.w), (result * i.rgbColor * strobe), i.intensityStrobeGOBOSpinSpeed.x * maxIntensity);
				result = lerp(half4(0,0,0,result.w), result, i.intensityStrobeGOBOSpinSpeed.x * i.intensityStrobeGOBOSpinSpeed.x * 2);
			}
			else
			{
				result *= strobe;
			}
		#endif

		if( i.uv.x < 0.001 == false)
		{
			float blinding = i.blindingEffect;
			//#ifdef VRSL_AUDIOLINK
				blinding = lerp(1.0, blinding, _BlindingStrength);
			//#endif
			#ifdef _ALPHATEST_ON
				result = lerp(result, result*blinding * 20, gradientTexture);
				#ifdef WASH
					maxIntensity +=0.25;
				#endif
			#else
				result = lerp(result, result*blinding * blinding * 10, gradientTexture);
			#endif
			result *= saturate(maxIntensity - (lerp(0.15, maxIntensity * 0.95, pow(widthNormalized,0.4))));
			
		}
		// result = isDMX() == 1 ?
		// lerp(fixed4(0,0,0,result.w), (result * i.rgbColor * strobe), i.intensityStrobeGOBOSpinSpeed.x * maxIntensity) :
		// result * strobe;

		// result = isDMX() == 1 ? 
		// 	lerp(half4(0,0,0,result.w), result, i.intensityStrobeGOBOSpinSpeed.x * i.intensityStrobeGOBOSpinSpeed.x * 2) : result;
		#ifdef VRSL_DMX
			result = (i.intensityStrobeGOBOSpinSpeed.x <= _IntensityCutoff && isDMX() == 1) ? half4(0,0,0,result.w) : result;
			//Fixture lens is now apart of Volumetrics, calculation for lens strenght is here
			//float maxBrightness = lerp(1.0, _LensMaxBrightness)
			#if !defined(WASH)
			float4 r2 = i.uv.x < 0.001 ? 10 * result * (_LensMaxBrightness * i.intensityStrobeGOBOSpinSpeed.x): result;
			#else
			float4 r2 = i.uv.x < 0.001 ? result * (_LensMaxBrightness * i.intensityStrobeGOBOSpinSpeed.x): result;
			#endif
			float camAngle = i.camAngleCamfade.x;
		#endif
		#ifdef VRSL_AUDIOLINK
			#if !defined(WASH)
			float4 r2 = i.uv.x < 0.001 ? 60 * result * _LensMaxBrightness : result;
			#else
			float4 r2 = i.uv.x < 0.001 ? result * _LensMaxBrightness : result;
			#endif
			float camAngle = i.camAngleLen.x;
		#endif

		float gifi = (gi * gi) * (fi * fi) * _UniversalIntensity;
		result = lerp(result, r2, gifi);

		//Find Greyscale value of cone.
		float3 newCol = (result.r + result.g + result.b)/3;
		float satMod = 5.0;

		//Create fake white power effect at source of cone and use Saturation and Saturation Length to blend that effect
		#ifndef _ALPHATEST_ON
			#ifndef _POTATO_MODE_ON
				newCol.xyz = lerp(result.xyz,newCol * 10, saturate((pow(saturate(gradientTexture - 0.25), _SaturationLength - satMod)) * tex.r) +0.005);
			#else
				newCol.xyz = lerp(result.xyz,newCol * 10, saturate((pow(saturate(gradientTexture - 0.25), _SaturationLength - satMod))) +0.005);
			#endif
			result.xyz = (lerp(result.xyz, newCol.xyz, _Saturation)) * gifi;
		#endif


		//Mix in Camera angle into strength for outside faces, increase strength for inside faces.
		result = facePos > 0 ? lerp(result * camAngle, result, cameraFade) : result * 3;
		float lastGrad = saturate((1-i.uv.x)*+1);
		float distfade = _DistFade;
		#ifdef _ALPHATEST_ON
		distfade -= 0.4;
		#endif
		lastGrad = lerp(1, lastGrad, distfade);
		#ifdef VRSL_DMX
			result = lerp(half4(0,0,0,result.w), result, i.intensityStrobeGOBOSpinSpeed.x * i.intensityStrobeGOBOSpinSpeed.x * 2);
		#endif
		#ifdef VRSL_AUDIOLINK
			result = lerp(half4(0,0,0,result.w), result, audioReact * audioReact);
		#endif

		#ifdef _ALPHATEST_ON
			result = result * lastGrad * gifi * 0.25;
			clip(result.a - DITHER_THRESHOLDS[index]);
			clip((((result.r + result.g + result.b)/3)) - DITHER_THRESHOLDS[index]);
			return result;
		#else
			return result * lastGrad;
		#endif

    
}