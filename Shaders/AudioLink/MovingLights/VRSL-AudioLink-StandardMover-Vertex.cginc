 //This file contains the vertex and fragment functions for both the ForwardBase and Forward Add pass.
//FOR MOVER LIGHT SHADER
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

float4 CalculateProjectionScaleRange(appdata v, float4 input, float scalar)
{
	float4 oldinput = input;
	float4x4 scaleMatrix = float4x4(scalar, 0, 0, 0,
	0, scalar, 0, 0,
	0, 0, scalar, 0,
	0, 0, 0, 1.0);
	float4 newOrigin = input.w * _ProjectionRangeOrigin;
	input.xyz = input.xyz - newOrigin;
	//Do stretch
	float4 newProjectionScale = mul(scaleMatrix, input);
	input.xyz = newProjectionScale;
	input.xyz = input.xyz + newOrigin;
	input.xyz = IF(v.color.r == 1.0 && ceil(v.color.g) == 1, input.xyz, oldinput);
	return input;


}
float4 CalculateConeWidth(appdata v, float4 input, float scalar)
{
	#if defined(VOLUMETRIC_YES)


			#ifdef WASH
				scalar *= 2.5;
				scalar += 2.50;
				//scalar = clamp(scalar, 0.0, 50.0);
			#endif

			//Set New Origin
			float4 newOrigin = input.w * _FixtureRotationOrigin; 
			input.xyz = input.xyz - newOrigin;
			scalar = -scalar;
			// Do Transformation
			float distanceFromFixture = (v.uv.x) * (scalar);
			distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, 0.05));
			input.y = (input.y) + (-v.normal.y) * (distanceFromFixture);
			input.x = (input.x) + (-v.normal.x) * (distanceFromFixture);
			float3 originStretch = input.xyz;
			float3 stretchedcoords = (v.tangent.z*getMaxConeLength());
			input.xyz = lerp(originStretch, (input.xyz * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
			input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);
			
			// }
			// else
			// {
			// 	float distanceFromFixture = (v.uv.x) * scalar;
			// 	distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));
			// 	input.y = (input.y) + (v.normal.y) * distanceFromFixture;
			// 	input.x = (input.x) + (v.normal.x) * distanceFromFixture;

			// 	float3 originStretch = input.xyz;
			// 	float3 stretchedcoords = (v.tangent.z*getMaxConeLength());
			// 	input.xyz = lerp(originStretch, (input.xyz * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
			// 	input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);


			// }
			input.xyz = input.xyz + newOrigin;

			return input;
	#endif

	#if defined(PROJECTION_YES)
		if(ceil(v.color.g) >= 0.9 && ceil(v.color.r) >= 0.5)
		{
			float4 newOrigin = input.w * _ProjectionRangeOrigin; 
			input.xyz = input.xyz - newOrigin;

			// Do Transformation
			float distanceFromFixture = (v.texcoord.x) * scalar;
			//input.xy = input.xy + v.normal.xy * distanceFromFixture;
			input.x = (input.x) + (v.normal.x) * distanceFromFixture;
			input.z = (input.z) + (v.normal.z) * distanceFromFixture;

			//Rest Origin
			input.xyz = input.xyz + newOrigin;

			return input;

		}
		else
		{
			return input;
		}
	#endif
	return input;

}

inline float4 CalculateFrustumCorrection()
{
	float x1 = -UNITY_MATRIX_P._31/(UNITY_MATRIX_P._11*UNITY_MATRIX_P._34);
	float x2 = -UNITY_MATRIX_P._32/(UNITY_MATRIX_P._22*UNITY_MATRIX_P._34);
	return float4(x1, x2, 0, UNITY_MATRIX_P._33/UNITY_MATRIX_P._34 + x1*UNITY_MATRIX_P._13 + x2*UNITY_MATRIX_P._23);
}
float2 GetStripeInfo(uint goboSelection)
{
	float2 result = float2(0.0,0.0);
	switch(goboSelection)
	{
		case 1:
			return result;
		case 2:
			result.x = _StripeSplit;
			result.y = _StripeSplitStrength;
			return result;
		case 3:
			result.x = _StripeSplit2;
			result.y = _StripeSplitStrength2;
			return result;
		case 4:
			result.x = _StripeSplit3;
			result.y = _StripeSplitStrength3;
			return result;
		case 5:
			result.x = _StripeSplit4;
			result.y = _StripeSplitStrength4;
			return result;
		case 6:
			result.x = _StripeSplit5;
			result.y = _StripeSplitStrength5;
			return result;
		case 7:
			result.x = _StripeSplit6;
			result.y = _StripeSplitStrength6;
			return result;
		case 8:
			result.x = _StripeSplit7;
			result.y = _StripeSplitStrength7;
			return result;
		default:
			return result;
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//VERTEX SHADER
v2f vert (appdata v)
{
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	UNITY_TRANSFER_INSTANCE_ID(v, o);

	v.vertex = CalculateConeWidth(v, v.vertex, getConeWidth());
	v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);
	//calculate rotations for verts
	//v.vertex = calculateRotations(v, v.vertex, 0);
	#if defined(PROJECTION_YES)
		#ifdef RAW
			o.globalFinalIntensity.x = getGlobalIntensity();
			o.globalFinalIntensity.y = getFinalIntensity();
		#else
			o.audioGlobalFinalConeIntensity.x = GetAudioReactAmplitude();
			o.audioGlobalFinalConeIntensity.y = getGlobalIntensity();
			o.audioGlobalFinalConeIntensity.z = getFinalIntensity();
			o.audioGlobalFinalConeIntensity.w = getConeWidth();
		#endif

		o.emissionColor = getEmissionColor();
	#endif
	#if defined(VOLUMETRIC_YES)
		#ifdef RAW
			o.globalFinalIntensity.x = getGlobalIntensity();
			o.globalFinalIntensity.y = getFinalIntensity();
		#else
			o.audioGlobalFinalIntensity.x = GetAudioReactAmplitude();
			o.audioGlobalFinalIntensity.y = getGlobalIntensity();
			o.audioGlobalFinalIntensity.z = getFinalIntensity();
		#endif
		o.emissionColor = getEmissionColor();
		float3 worldCam;
		worldCam.x = unity_CameraToWorld[0][3];
		worldCam.y = unity_CameraToWorld[1][3];
		worldCam.z = unity_CameraToWorld[2][3];
		float3 objCamPos = mul(unity_WorldToObject, float4(worldCam, 1)).xyz;
		//objCamPos = InvertVolumetricRotations(float4(objCamPos,1)).xyz;
		float len = length(objCamPos.xy);
		len *= len;
		o.camAngleLen.y = len;
		float4 originScreenPos = ComputeScreenPos(UnityObjectToClipPos(float4(0,0,0,0)));
		float2 originScreenUV = originScreenPos.xy / originScreenPos.w;
		o.camAngleLen.x = saturate((1-distance(float2(0.5, 0.5), originScreenUV))-0.5);
		o.camAngleLen.x = pow(o.camAngleLen.x, 0.5);
		o.blindingEffect = clamp(0.6/len,1.0,8.0);
		//camAngle = lerp(1, camAngle, len);

		
		 #ifndef WASH
		 	float endBlind = lerp(1.0, o.blindingEffect, 0.35);
			o.blindingEffect = lerp(endBlind, o.blindingEffect * 2.2, o.camAngleLen.x);
		 #else
		 	o.blindingEffect = lerp(1, o.blindingEffect * 2.0, o.camAngleLen.x);
		 #endif
		//o.camAngle = camAngle;
		//o.viewDir.yzw = objCamPos.xyz;
	#endif

	//calculate rotations for normals, cast to float4 first with 0 as w
	float4 newNormals = float4(v.normal.x, v.normal.y, v.normal.z, 0);
	//newNormals = calculateRotations(v, newNormals, 1);
	v.normal = newNormals.xyz;

	//calculate rotations for tangents, cast to float4 first with 0 as w
	float4 newTangent = float4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
	//newTangent = calculateRotations(v, newTangent, 1);
	v.tangent = newTangent.xyz;

	//original surface shader related code
    float3 worldNormal = UnityObjectToWorldNormal(v.normal);
    float3 tangent = UnityObjectToWorldDir(v.tangent);
    float3 bitangent = cross(tangent, worldNormal);


	#if !defined(PROJECTION_YES) && !defined(VOLUMETRIC_YES)
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	#endif

	#if defined(PROJECTION_YES) 
		
		//UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		//move verts to clip space
		o.pos = UnityObjectToClipPos(v.vertex);
		//get screen space position of verts
		o.screenPos = ComputeScreenPos(o.pos);
		//Putting in the vertex position before the transformation seems to somewhat move the projection correctly, but is still incorrect...?
		o.ray = UnityObjectToViewPos(v.vertex).xyz;
		//invert z axis so that it projects from camera properly
		o.ray *= float3(1,1,-1);
		//saving vertex color incase needing to perform rotation calculation in fragment shader
		o.color = v.color;
		//o.sector.x = (float)sector;

		o.worldPos = mul(unity_ObjectToWorld, v.vertex);

		//For Mirror Depth Correction
		o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
		// pack correction factor into direction w component to save space
		o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
		//o.viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);
		o.viewDir = normalize(UnityObjectToViewPos(v.vertex.xyz)); // get normalized view dir
		o.viewDir /= o.viewDir.z; // rescale vector so z is 1.0
		#ifdef RAW
			if(o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
			{
				v.vertex = float4(0,0,0,0);
				o.pos = UnityObjectToClipPos(v.vertex);
			}
		#else
			if(o.audioGlobalFinalConeIntensity.x <= 0.005 || o.audioGlobalFinalConeIntensity.y <= 0.005 || o.audioGlobalFinalConeIntensity.z <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
			{
				v.vertex = float4(0,0,0,0);
				o.pos = UnityObjectToClipPos(v.vertex);
			}
		#endif		

		//o.normal = v.normal;
	#endif
    
	
	#if defined(UNITY_PASS_FORWARDBASE)
    o.uv1 = v.uv1;
    o.uv2 = v.uv2;
    #endif


	//Volumetric Part - Vertex Shader
	#if defined(VOLUMETRIC_YES)
		o.pos = UnityObjectToClipPos(v.vertex);

		o.screenPos = ComputeScreenPos (o.pos);
		o.uvClone = v.uv2;
		o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
		o.uv = TRANSFORM_TEX(v.uv, _LightMainTex);
		//o.uv = UnityStereoScreenSpaceUVAdjust(uv, sb)
		COMPUTE_EYEDEPTH(o.screenPos.z);
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);

		//For Mirror Depth Correction
		o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
		// pack correction factor into direction w component to save space
		o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
		o.color = v.color;
		//o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.objPos = v.vertex;
		o.objNormal = v.normal;
		o.stripeInfo = GetStripeInfo(instancedGOBOSelection());
		 o.norm = worldNormal;
		#ifdef RAW
			if(o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
			{
				v.vertex = float4(0,0,0,0);
				o.pos = UnityObjectToClipPos(v.vertex);
			}
		#else
			if(o.audioGlobalFinalIntensity.x <= 0.005 || o.audioGlobalFinalIntensity.y <= 0.005 || o.audioGlobalFinalIntensity.z <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
			{
				v.vertex = float4(0,0,0,0);
				o.pos = UnityObjectToClipPos(v.vertex);
			}
		#endif
	#endif
    
    #if !defined(UNITY_PASS_SHADOWCASTER) && !defined(PROJECTION_YES) && !defined(VOLUMETRIC_YES)
	
	o.color = v.color;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.objPos = v.vertex;
    o.objNormal = v.normal;
	o.btn[0] = bitangent;
    o.btn[1] = tangent;
    o.btn[2] = worldNormal;
		#if !defined(VOLUMETRIC_YES) && !defined (PROJECTION_YES)
    		UNITY_TRANSFER_SHADOW(o, o.uv);

		#endif
    #else
	o.color = v.color;
    #endif

  return o;
}

//START OF FRAG SHADER

#if defined(VOLUMETRIC_YES)
fixed4 frag (v2f i, fixed facePos : VFACE) : SV_Target
{
	return VolumetricLightingBRDF(i, facePos);
}
#endif	
#if !defined(VOLUMETRIC_YES)			
fixed4 frag (v2f i) : SV_Target
{
	
    //Return only this if in the shadowcaster
    #if defined(UNITY_PASS_SHADOWCASTER)
	if(i.color.r > 0 && i.color.b > 0)
	{
		discard;
		return half4(0,0,0,0);		
	}
	else
	{
		SHADOW_CASTER_FRAGMENT(i);
	}

	// #elif defined (VOLUMETRIC_YES)
	// 	return VolumetricLightingBRDF(i);
	#elif defined (PROJECTION_YES)
		return ProjectionFrag(i);
    
	#else
        return CustomStandardLightingBRDF(i);
    #endif
}
#endif



