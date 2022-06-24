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
    return pow(max(0, dot(normalize(Normal), -ViewDir)), Power);
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
	//Setup instancing
    UNITY_SETUP_INSTANCE_ID(i);

	//Get Global/FInal Intensity
	#ifdef RAW
		float globalintensity = i.globalFinalIntensity.x;
		float finalintensity = i.globalFinalIntensity.y;
		//Get Emission Color
		float4 emissionTint = i.emissionColor;

		//Check if we should even render the cone at all?
		if(globalintensity <= 0.005 || finalintensity <= 0.005 || all(emissionTint <= float4(0.005, 0.005, 0.005, 1.0)))
		{
			//If the light is basically off, don't render anything.
			return half4(0,0,0,0);
		}
	#else
		float audioReact = i.audioGlobalFinalIntensity.x;
		float globalintensity = i.audioGlobalFinalIntensity.y;
		float finalintensity = i.audioGlobalFinalIntensity.z;
		//Get Emission Color
		float4 emissionTint = i.emissionColor;


		if(globalintensity <= 0.005 || finalintensity <= 0.005 || audioReact <= 0.005 || all(emissionTint<= float4(0.005, 0.005, 0.005, 1.0)))
		{
			//If the light is basically off, don't render anything.
			return half4(0,0,0,0);
		}
	#endif
	//not sure why this was here... Keeping it just in case... it shouldn't do anything technically.
	// i.uv.x = saturate(i.uv.x);
	// i.uv.y = saturate(i.uv.y);


	//Get gobo selection! Getting this in vertex shader seems to cause weird artifacting for some reason!
	uint gobo = instancedGOBOSelection();

	//Initialize UVMap
	half2 uvMap = half2(0.0,0.0);

	//replacement for _WorldSpaceCameraPos
	float3 wpos;
	wpos.x = unity_CameraToWorld[0][3];
	wpos.y = unity_CameraToWorld[1][3];
	wpos.z = unity_CameraToWorld[2][3];

	//Get Gobo Spin speed information
	float goboSpinSpeed = IF(checkPanInvertY() == 1, -getGoboSpinSpeed(), getGoboSpinSpeed());
	float spinSpeed = UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);

	//CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
	//Get Screen Pos UVs
	float perspectiveDivide = 1.0f / i.pos.w;
	float4 direction = i.worldDirection * perspectiveDivide;
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
	
	//Starts to fade about 1 meter away(x/1). increase divisor to increase distance
	
	float cameraFade = saturate(((distance(wpos, i.worldPos) - 0.5)));
	//float cameraFade = 0;


	//Fade the camera less when you are closer to the source of the light.
	cameraFade = (lerp(1.0, cameraFade, saturate(pow(i.uv.x, 0.1))));

	//Set uv map properly and adjust for the length of the cone.
	uvMap = half2(saturate(i.uv.x * getConeLength()), i.uv.y);

	//Generate gradient for cone.
	// #ifndef WASH
		fixed gradientTexture = saturate((pow(-uvMap.x + 1, 2.25)));
	// #else
	// 	float camFadeMod = saturate(1/(i.camAngleLen.y));
	// 	fixed gradientTexture = saturate((pow(-uvMap.x + 1, lerp(2.25, 2.25+(getConeWidth()),camFadeMod))));
	// #endif
	fixed4 col = gradientTexture.r;

	//Calculate View Direction for fading edges.
	float3 viewDir = normalize(wpos - i.worldPos.xyz);
	viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));
	//Initialize Edge Fade
	float edgeFade = 0.0;
	//float threeDNoiseScale = 1.0;
	//Fade away edges of cone using a Fresnel calculation.
	//Set the strength of the fade based on which side of the polygon.
	//float threeDNoiseScale = 1.0;
	half noiseScaleThreeD;
	if(facePos > 0)
	{
		edgeFade = Fresnel(-i.objNormal, viewDir, _InnerFadeStrength);
		noiseScaleThreeD = _Noise2Stretch;
	}
	else
	{
		//Reduce fade strength if closer to source of light
		float fadeSTR = lerp(1.0, _FadeStrength, saturate(pow(i.uv.x, 0.1) + 0.05));
		edgeFade = Fresnel(i.objNormal, viewDir, fadeSTR);
		noiseScaleThreeD = _Noise2StretchInside;
	}

	//Set correct gobo spin speed direction;
	spinSpeed *= -goboSpinSpeed;

	//Combine Gradient with emission color, intersection fade and camera fade.
	col = col * emissionTint * cameraFade;
	//return cameraFade;

	//Generate 2D noise texture, add scroll effect, and map to cone.
	float2 texUV = i.uv2;
	texUV.x = texUV.x + _Time * 0.75;
	texUV.y = texUV.y + _Time * 0.1;
		#if !defined(WASH)
		float4 tex = tex2D(_NoiseTex, texUV);
		#else
		float4 tex = float4(1,1,1,1);
		#endif

	//initialize 3D noise value and 2D noise strength value
	float threeDNoise = 1.0;
	half np = 0.0;
	

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
		float3 q = noiseScaleThreeD * worldposNoise.xyz;

		//Use IQ's noise calculation equation to calculate 3D noise in world space.
		//Currently only sampling the noise twice as sampling anymore isn't creating any visible improvements.
		threeDNoise  = 0.5000*Noise( q ); 
		q = mul(m,q)*2.01;
		threeDNoise += 0.2500*Noise( q ); 
		// q = mul(m,q)*2.02;
		// threeDNoise += 0.1250*Noise( q ); 
		// q = mul(m,q)*2.03;
		// threeDNoise += 0.0625*Noise( q ); 
		// q = mul(m,q)*2.01;
		
		//If we aren't using gobos, remove the 2D noise effect
		np = gobo > 1 ? _NoisePower : 0.0;
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
		#ifndef WASH
			np = gobo > 1 ? clamp(0,1,_NoisePower + 0.2) : _NoisePower; 
		#else
			//tex = tex2D(_NoiseTex, texUV);
			np = _NoisePower;
		#endif
	}

	//Mix 2D noise Power
	tex = lerp(fixed4(1, 1, 1, 1), tex, np);

	//Find Greyscale value of cone.
	float3 newCol = (col.r + col.g + col.b)/3;
	//Create fake white power effect at source of cone and use Saturation and Saturation Length to blend that effect
	float satMod = 5.0;
	if(gobo <= 1)
	{
		satMod = 0.0;
		col *= 0.8;

	}
	//float satMod = gobo > 1 ? 5.0 : 0.0;
	newCol.xyz = lerp(col.xyz,newCol * 5, saturate(pow(saturate(gradientTexture - 0.25), _SaturationLength - satMod)) * tex.r);
	col.xyz = lerp(col.xyz, newCol.xyz, _Saturation);

	//Mix in blinding effect.
	col*= ((i.blindingEffect * (i.blindingEffect * 0.25)));
	col*=6.0;
	col = lerp(col, fixed4(0,0,0,0), saturate(pow(i.uv.x,.5)));

	#if !defined(WASH)
	//Beam Splitter and gobo spin
		const float pi = 3.14159265;
		//Choose split strength and pattern based on information in i.stripeinfo.
		float splitter = (sin(i.uv.y * pi * floor(i.stripeInfo.x) * 2 + (_Time.w * spinSpeed)) + 1.0);
		//Do not use beam splitting if we aren't using gobos.
		float splitstr = IF(_GoboBeamSplitEnable == 1 && gobo > 1, i.stripeInfo.y, 0);
		splitter = lerp(1.0, splitter, splitstr);
	#else
		float splitter = 1.0;
	#endif


	//Mix in 2D noise, beam splitting, and 3D noise.
	col *= tex;
	col *= splitter;
	col *= threeDNoise;

	//Add more power to Inner side of cone
	float4 result = col;
	#if !defined(WASH)
	result = lerp(col,col * 25, saturate(pow(gradientTexture, _InnerIntensityCurve)));
	#endif

	//Mix in Frensel Edge Fade
	result *= edgeFade;
	
	//Combine Global Intensity and Final Intensity
	float gifi = (globalintensity * globalintensity) * (finalintensity * finalintensity);

	//Fixture lens is now apart of Volumetrics, calculation for lens strenght is here
	#if !defined(WASH)
	float4 r2 = i.uv.x < 0.001 ? 60 * result * _LensMaxBrightness : result;
	#else
	float4 r2 = i.uv.x < 0.001 ? result * _LensMaxBrightness : result;
	#endif
	//Mix in the rest of the external intensity properties.
	result = lerp(result, r2, gifi);
	result = lerp(half4(0,0,0,result.w), result, gifi);
	result *= _FixtureMaxIntensity * _UniversalIntensity;
	#ifndef RAW
		result = lerp(half4(0,0,0,result.w), result, audioReact * audioReact);
	#endif

	//Mix in Camera angle into strength for outside faces, increase strength for inside faces.
	result = facePos > 0 ? lerp(result * i.camAngleLen.x, result, cameraFade) : result * 3;
	return result * intersectionFade;
    
}