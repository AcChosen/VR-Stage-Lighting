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

//3D noise based on iq's https://www.shadertoy.com/view/4sfGzS
//HLSL conversion by Orels1~
half Noise(half3 p)
{
  half3 i = floor(p); p -= i; 
  p *= p * (3. - 2. * p);
  half2 uv = (p.xy + i.xy + half2(37, 17) * i.z + .5)*0.00390625;
  //uv.y *= -1;
  float4 uv4 = float4(uv.x, uv.y * -1, 0.0, 0.0);
  p.xy = tex2Dlod(_LightMainTex, uv4).yx;
  return lerp(p.x, p.y, p.z);
}

const float3x3 m = float3x3( 0.00,  0.80,  0.60,
                    -0.80,  0.36, -0.48,
                    -0.60, -0.48,  0.64 );


float4 VolumetricLightingBRDF(v2f i, fixed facePos)
{
	//UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    UNITY_SETUP_INSTANCE_ID(i);

	 float gi = getGlobalIntensity();
	 float fi = getFinalIntensity();
		if(((all(i.rgbColor <= float4(0.005,0.005,0.005,1)) || i.intensityStrobeGOBOSpinSpeed.x <= 0.005) && isOSC() == 1) || gi <= 0.005 || fi <= 0.005)
		{
			return half4(0,0,0,0);
		} 
		//uint gobo = instancedGOBOSelection();
		//Select Gobo!
		//uint gobo = i.intensityStrobeGOBOSpinSpeed.w;

		//Initialize UVMap
		half2 uvMap = half2(0.0,0.0);

		//replacement for _WorldSpaceCameraPos
		float3 wpos;
		wpos.x = unity_CameraToWorld[0][3];
		wpos.y = unity_CameraToWorld[1][3];
		wpos.z = unity_CameraToWorld[2][3];

		//Get Gobo Spin speed information
		float spinSpeed = (-i.intensityStrobeGOBOSpinSpeed.z) * UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
		//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
		//Get Screen Pos UVs
		//float perspectiveDivide = 1.0f / i.pos.w;
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
		//Attempt to fade cone away when intersecting with the camera.
		//float cameraFade = i.camAngleCamfade.y;

		//Fade the camera less when you are closer to the source of the light.
		i.camAngleCamfade.y = lerp(1.0, i.camAngleCamfade.y, saturate(pow(i.uv.x, 0.1)));

		//Set uv map properly and adjust for the length of the cone.
		uvMap = half2(saturate(i.uv.x * getConeLength()), i.uv.y);

		i.uv.x = saturate(i.uv.x * 1.5);

		//Generate gradient for cone.
		fixed gradientTexture = saturate((pow(-uvMap.x + 1, 2.25)));
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
			edgeFade = Fresnel(-i.objNormal, viewDir, _InnerFadeStrength);
			threeDNoiseScale = _Noise2Stretch;
		}
		else
		{
			//Reduce fade strength if closer to source of light
			float fadeSTR = lerp(1.0, _FadeStrength, saturate(pow(i.uv.x, 0.1) + 0.05));
			edgeFade = Fresnel(i.objNormal, viewDir, fadeSTR);
			threeDNoiseScale = _Noise2StretchInside;
		}

		//Combine Gradient with emission color, intersection fade and camera fade.
		col = col * getEmissionColor() * intersectionFade * i.camAngleCamfade.y;

		//Get Strobe information
		float strobe = isStrobe() == 1 ? i.intensityStrobeGOBOSpinSpeed.y : 1;

		//Generate 2D noise texture, add scroll effect, and map to cone.
		float2 texUV = i.uv2;
		texUV.x = _Time * 0.75f + texUV.x;
		texUV.y = _Time * 0.10f + texUV.y;
		#if !defined(WASH)
		float4 tex = tex2D(_NoiseTex, texUV);
		#else
		float4 tex = float4(1,1,1,1);
		#endif

		//initialize 3D noise value and 2D noise strength value
		float threeDNoise = 1.0f;
		half np = 0.0f;

		//If we are using 3D noise...
		if(_ToggleMagicNoise > 0)
		{
			//Get vertex/frag position in worldspace
			float3 worldposNoise = i.worldPos.xyz;
			//Add Scrolling effect
			worldposNoise.x += (_Time * _Noise2X);
			worldposNoise.y += (_Time * _Noise2Y);
			worldposNoise.z += (_Time * _Noise2Z);
			//Add Tiling effect
			//float3 q = threeDNoiseScale * worldposNoise.xyz;
			float3 q = float3(0,0,0);
			q.x = threeDNoiseScale * worldposNoise.x;
			q.y = threeDNoiseScale * worldposNoise.y;
			q.z = threeDNoiseScale * worldposNoise.z;

			//Use IQ's noise calculation equation to calculate 3D noise in world space.
			//Currently only sampling the noise twice as sampling anymore isn't creating any visible improvements.
			threeDNoise = 0.5000f*Noise( q ); 
			q = mul(m,q)*2.01f;
			threeDNoise += 0.2500f*Noise( q ); 
			// q = mul(m,q)*2.02;
			// threeDNoise += 0.1250*Noise( q ); 
			// q = mul(m,q)*2.03;
			// threeDNoise += 0.0625*Noise( q ); 
			// q = mul(m,q)*2.01;
			
			//If we aren't using gobos, remove the 2D noise effect
			np = i.intensityStrobeGOBOSpinSpeed.w > 1 ? _NoisePower : 0.0;
			//Set 3D Noise Power
			#ifndef WASH
				float newNP = lerp(_Noise2Power - 0.2, _Noise2Power, gradientTexture.r);
				threeDNoise = lerp(1, threeDNoise, newNP);
			#else
				threeDNoise = lerp(1, threeDNoise, _Noise2Power);
			#endif
		}
		//If we aren't using 3D noise..
		else
		{
			//If we are using gobos, add another 0.2 to the 2D noise power strength.
			//np = i.intensityStrobeGOBOSpinSpeed.w > 1 ? clamp(0,1,_NoisePower + 0.2) : _NoisePower; 
			#ifndef WASH
			np = i.intensityStrobeGOBOSpinSpeed.w > 1 ? clamp(0,1,_NoisePower + 0.2) : _NoisePower; 
			#else
				//tex = tex2D(_NoiseTex, texUV);
				np = _NoisePower;
			#endif
		}

		//Mix 2D noise Power
		tex = lerp(fixed4(1, 1, 1, 1), tex, np);



		//Mix in blinding effect.
		col*= ((i.blindingEffect));
		//col*=2.0;
		col = lerp(col, fixed4(0,0,0,0), pow(i.uv.x,.5));


		//Beam Splitter and gobo spin
		//const float pi = 3.14159265;
		//Choose split strength and pattern based on information in i.stripeinfo.
		#if !defined(WASH)
		float splitter = (sin(i.uv.y * 3.14159265 * floor(i.stripeInfo.x) * 2.0f + ( spinSpeed * 5.0)) + 1.0);

		//Do not use beam splitting if we aren't using gobos.
		float splitstr = IF(_GoboBeamSplitEnable == 1 && i.intensityStrobeGOBOSpinSpeed.w > 1, i.stripeInfo.y, 0);
		splitter = lerp(1.0, splitter, splitstr);
		#else
		float splitter = 1.0;
		#endif

		//Mix in 2D noise, beam splitting, and 3D noise.
		// col *= tex;
		// col *= splitter;
		// col *= threeDNoise;
		col *= tex * splitter * threeDNoise;

		//Add more power to Inner side of cone
		float4 result = col;
		#if !defined(WASH)
		result = lerp(col,col * 25, saturate(pow(gradientTexture, _InnerIntensityCurve)));
		#endif
		//Mix in Frensel Edge Fade
		result *= edgeFade;


		if(isOSC() == 1)
		{
			result = lerp(fixed4(0,0,0,result.w), (result * i.rgbColor * strobe), i.intensityStrobeGOBOSpinSpeed.x * _FixtureMaxIntensity);
			result = lerp(half4(0,0,0,result.w), result, i.intensityStrobeGOBOSpinSpeed.x * i.intensityStrobeGOBOSpinSpeed.x * 2);
		}
		else
		{
			result *= strobe;
		}
		result *= _FixtureMaxIntensity;
		// result = isOSC() == 1 ?
		// lerp(fixed4(0,0,0,result.w), (result * i.rgbColor * strobe), i.intensityStrobeGOBOSpinSpeed.x * _FixtureMaxIntensity) :
		// result * strobe;

		// result = isOSC() == 1 ? 
		// 	lerp(half4(0,0,0,result.w), result, i.intensityStrobeGOBOSpinSpeed.x * i.intensityStrobeGOBOSpinSpeed.x * 2) : result;

		result = (i.intensityStrobeGOBOSpinSpeed.x <= _IntensityCutoff && isOSC() == 1) ? half4(0,0,0,result.w) : result;

		//Fixture lens is now apart of Volumetrics, calculation for lens strenght is here
		//float maxBrightness = lerp(1.0, _LensMaxBrightness)
		#if !defined(WASH)
		float4 r2 = i.uv.x < 0.001 ? 10 * result * (_LensMaxBrightness * i.intensityStrobeGOBOSpinSpeed.x): result;
		#else
		float4 r2 = i.uv.x < 0.001 ? result * (_LensMaxBrightness * i.intensityStrobeGOBOSpinSpeed.x): result;
		#endif
		float gifi = (gi * gi) * (fi * fi) * _UniversalIntensity;
		result = lerp(result, r2, gifi);

		//Find Greyscale value of cone.
		float3 newCol = (result.r + result.g + result.b)/3;
		float satMod = 5.0;
		if(i.intensityStrobeGOBOSpinSpeed.w <= 1)
		{
			satMod = 0.0;
			result *= 0.8;

		}
		// float satMod = i.intensityStrobeGOBOSpinSpeed.w > 1 ? 5.0 : 0.0;
		//Create fake white power effect at source of cone and use Saturation and Saturation Length to blend that effect
		newCol.xyz = lerp(result.xyz,newCol * 10, saturate((pow(saturate(gradientTexture - 0.25), _SaturationLength - satMod)) * tex.r) +0.005);
		result.xyz = (lerp(result.xyz, newCol.xyz, _Saturation)) * gifi;
		//Mix in Camera angle into strength for outside faces, increase strength for inside faces.
		result = facePos > 0 ? lerp(result * i.camAngleCamfade.x, result, i.camAngleCamfade.y) : result * 3;

		return result;

    
}