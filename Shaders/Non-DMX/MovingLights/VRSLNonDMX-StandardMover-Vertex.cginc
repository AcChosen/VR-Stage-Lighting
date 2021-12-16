 //This file contains the vertex and fragment functions for both the ForwardBase and Forward Add pass.
//FOR MOVER LIGHT SHADER
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

float4 calculateRotations(appdata v, float4 input, int normalsCheck)
{

	//CALCULATE BASE ROTATION. MORE FUN MATH. THIS IS FOR PAN.
	float angleY = radians(getOffsetY());
	float c = cos(angleY);
	float s = sin(angleY);
	float4x4 rotateYMatrix = float4x4(c, -s, 0, 0,
		s, c, 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 1);
	float4 BaseAndFixturePos = input;

	//INVERSION CHECK
	rotateYMatrix = IF(checkPanInvertY() == 1, transpose(rotateYMatrix), rotateYMatrix);

	float4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);
	//LOCALROTY IS NEW ROTATION


	//CALCULATE FIXTURE ROTATION. WOO FUN MATH. THIS IS FOR TILT.

	//set new origin to do transform
	float4 newOrigin = input.w * _FixtureRotationOrigin;
	//if input.w is 1 (vertex), origin changes
	//if input.w is 0 (normal/tangent), origin doesn't change

	//subtract new origin from original origin for blue vertexes
	input.xyz = IF(v.color.b == 1.0, input.xyz - newOrigin, input.xyz);


	//DO ROTATION
	float angleX = radians(getOffsetX());
	c = cos(angleX);
	s = sin(angleX);
	float4x4 rotateXMatrix = float4x4(1, 0, 0, 0,
		0, c, -s, 0,
		0, s, c, 0,
		0, 0, 0, 1);
		
	//float4 fixtureVertexPos = input;
		
	//INVERSION CHECK
	rotateXMatrix = IF(checkTiltInvertZ() == 1, transpose(rotateXMatrix), rotateXMatrix);

	//float4 localRotX = mul(rotateXMatrix, fixtureVertexPos);
	//LOCALROTX IS NEW ROTATION



	//COMBINED ROTATION FOR FIXTURE

	float4x4 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
	float4 localRotXY = mul(rotateXYMatrix, input);
	//LOCALROTXY IS COMBINED ROTATION

	//Apply fixture rotation ONLY to those with blue vertex colors

	//apply LocalRotXY rotation then add back old origin
	input.xyz = IF(v.color.b == 1.0, localRotXY, input.xyz);
	input.xyz = IF(v.color.b == 1.0, input.xyz + newOrigin, input.xyz);
	
	//appy LocalRotY rotation to lightfixture base;
	input.xyz = IF(v.color.g == 1.0, localRotY, input.xyz);

	return input;
}

float4 InvertVolumetricRotations (float4 input)
{
	float sX, cX, sY, cY;
	float angleY = radians(getOffsetY());
	sY = sin(angleY);
	cY = cos(angleY);
	float4x4 rotateYMatrix = float4x4(cY, sY, 0, 0,
		-sY, cY, 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 1);
	float4 BaseAndFixturePos = input;

		//INVERSION CHECK
	rotateYMatrix = IF(checkPanInvertY() == 1, transpose(rotateYMatrix), rotateYMatrix);

	//LOCALROTY IS NEW ROTATION


	float tiltOffset = 90.0;
	tiltOffset = IF(checkTiltInvertZ() == 1, -tiltOffset, tiltOffset);
	//set new origin to do transform
	float4 newOrigin = input.w * _FixtureRotationOrigin;
	input.xyz -= newOrigin;


	float angleX = radians(getOffsetX() + (tiltOffset));
	sX = sin((angleX));
	cX = cos((angleX));

	float4x4 rotateXMatrix = float4x4(1, 0, 0, 0,
		0, cX, sX, 0,
		0, -sX, cX, 0,
		0, 0, 0, 1);

	//float4 fixtureVertexPos = input;

		//INVERSION CHECK
	rotateXMatrix = IF(checkTiltInvertZ() == 1, transpose(rotateXMatrix), rotateXMatrix);
	//float4 localRotX = mul(rotateXMatrix, fixtureVertexPos);

	float4x4 rotateXYMatrix = mul(rotateXMatrix, rotateYMatrix);
	float4 localRotXY = mul(rotateXYMatrix, input);

	input.xyz = localRotXY;
	input.xyz += newOrigin;
	return input;
}

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

			//Set New Origin
			float4 newOrigin = input.w * _FixtureRotationOrigin; 
			input.xyz = input.xyz - newOrigin;

			// Do Transformation
			#if !defined(RAW)
			if(v.color.r < 0.9)
			{
				float distanceFromFixture = (v.uv.x) * (scalar);
				distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));

				input.z = (input.z) + (-v.normal.z) * (distanceFromFixture);
				input.x = (input.x) + (-v.normal.x) * (distanceFromFixture);
			}
			else
			{
				float distanceFromFixture = (v.uv.x) * scalar;
				distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));
				input.z = (input.z) + (v.normal.z) * distanceFromFixture;
				input.x = (input.x) + (v.normal.x) * distanceFromFixture;

			}
			#endif
			#if defined(RAW)
			{
			if(v.color.r < 0.9)
			{
				float distanceFromFixture = (v.uv.x) * (scalar);
				distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));
				//input.z = (input.z) + (-v.normal.z) * (getConeLength());
				input.y = (input.y) + (-v.normal.y) * (distanceFromFixture);
				input.x = (input.x) + (-v.normal.x) * (distanceFromFixture);
				float3 originStretch = input.xyz;
				float3 stretchedcoords = (v.tangent.z*getMaxConeLength());
				input.xyz = lerp(originStretch, (input.xyz * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
				input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);

			}
			else
			{
				float distanceFromFixture = (v.uv.x) * scalar;
				distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));
				//input.z = (input.z) + (v.normal.z) * (getConeLength());
				input.y = (input.y) + (v.normal.y) * distanceFromFixture;
				input.x = (input.x) + (v.normal.x) * distanceFromFixture;
				///input.xyz = 0;

				float3 originStretch = input.xyz;
				float3 stretchedcoords = (v.tangent.z*getMaxConeLength());
				input.xyz = lerp(originStretch, (input.xyz * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
				input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);
				

			}
			}
			#endif

			//input.y *= _ConeLength;

			//Rest Origin
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

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



//VERTEX SHADER
v2f vert (appdata v)
{
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
	//UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
	v.vertex = CalculateConeWidth(v, v.vertex, getConeWidth());
	v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);
	//calculate rotations for verts
	v.vertex = calculateRotations(v, v.vertex, 0);
	#if defined(PROJECTION_YES)
		o.globalFinalIntensity.x = getGlobalIntensity();
		o.globalFinalIntensity.y = getFinalIntensity();
		o.emissionColor = getEmissionColor();
		o.projectionorigin = calculateRotations(v, _ProjectionRangeOrigin, 0);
	#endif
	#if defined(VOLUMETRIC_YES)
		o.globalFinalIntensity.x = getGlobalIntensity();
		o.globalFinalIntensity.y = getFinalIntensity();
		o.emissionColor = getEmissionColor();
		float3 worldCam;
		worldCam.x = unity_CameraToWorld[0][3];
		worldCam.y = unity_CameraToWorld[1][3];
		worldCam.z = unity_CameraToWorld[2][3];
		float3 objCamPos = mul(unity_WorldToObject, float4(worldCam, 1)).xyz;
		//objCamPos = InvertVolumetricRotations(float4(objCamPos,1)).xyz;
		float len = length(objCamPos.xy);
		len *= len;
		float4 originScreenPos = ComputeScreenPos(UnityObjectToClipPos(float4(0,0,0,0)));
		float2 originScreenUV = originScreenPos.xy / originScreenPos.w;
		float camAngle = saturate((1-distance(float2(0.5, 0.5), originScreenUV))-0.5);
		o.blindingEffect = clamp(0.6/len,1.0,16.0);
		o.blindingEffect = lerp(1, o.blindingEffect, camAngle);
	#endif

	//calculate rotations for normals, cast to float4 first with 0 as w
	float4 newNormals = float4(v.normal.x, v.normal.y, v.normal.z, 0);
	newNormals = calculateRotations(v, newNormals, 1);
	v.normal = newNormals.xyz;

	//calculate rotations for tangents, cast to float4 first with 0 as w
	float4 newTangent = float4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
	newTangent = calculateRotations(v, newTangent, 1);
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
		if(o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
		{
			v.vertex = float4(0,0,0,0);
			o.pos = UnityObjectToClipPos(v.vertex);
		}

		//o.normal = v.normal;
	#endif
    
	
	#if defined(UNITY_PASS_FORWARDBASE)
    o.uv1 = v.uv1;
    o.uv2 = v.uv2;
    #endif


	//Volumetric Part - Vertex Shader
	#if defined(VOLUMETRIC_YES)
		o.pos = UnityObjectToClipPos(v.vertex);
		o.viewDir = ObjSpaceViewDir(v.vertex);
		o.screenPos = ComputeScreenPos (o.pos);
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
		o.bitan = bitangent;
		o.tan = tangent;
		o.norm = worldNormal;
		if(o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
		{
			v.vertex = float4(0,0,0,0);
			o.pos = UnityObjectToClipPos(v.vertex);
		}

	#endif

	//Projection Part - Vertex Shader
	// #if defined(PROJECTION_YES)


	// #endif
    
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

	#elif defined (VOLUMETRIC_YES)
		return VolumetricLightingBRDF(i);
	#elif defined (PROJECTION_YES)
		return ProjectionFrag(i);
    
	#else
        return CustomStandardLightingBRDF(i);
    #endif
}




