 //This file contains the vertex and fragment functions for both the ForwardBase and Forward Add pass.
//FOR MOVER LIGHT SHADER
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

float4 calculateRotations(appdata v, float4 input, int normalsCheck, float pan, float tilt)
{
//	input = IF(worldspacecheck == 1, float4(UnityObjectToWorldNormal(v.normal).x * -1.0, UnityObjectToWorldNormal(v.normal).y * -1.0, UnityObjectToWorldNormal(v.normal).z * -1.0, 1), input)
	
	
	

	//CALCULATE BASE ROTATION. MORE FUN MATH. THIS IS FOR PAN.
	float angleY = radians(getOffsetY() + pan);
	float c, s;
	sincos(angleY, s, c);

	float3x3 rotateYMatrix = float3x3(c, -s, 0,
									s, c, 0,
									0, 0, 1);
	float3 BaseAndFixturePos = input.xyz;

	//INVERSION CHECK
	rotateYMatrix = checkPanInvertY() == 1 ? transpose(rotateYMatrix) : rotateYMatrix;

	float3 localRotY = mul(rotateYMatrix, BaseAndFixturePos);
	//LOCALROTY IS NEW ROTATION


	//CALCULATE FIXTURE ROTATION. WOO FUN MATH. THIS IS FOR TILT.

	//set new origin to do transform
	float3 newOrigin = input.w * _FixtureRotationOrigin.xyz;
	//if input.w is 1 (vertex), origin changes
	//if input.w is 0 (normal/tangent), origin doesn't change

	//subtract new origin from original origin for blue vertexes
	input.xyz = v.color.b == 1.0 ? input.xyz - newOrigin : input.xyz;


	//DO ROTATION


	//#if defined(PROJECTION_YES)
	//buffer[3] = GetTiltValue(sector);
	//#endif



	float angleX = radians(getOffsetX() + tilt);
	sincos(angleX, s, c);
	float3x3 rotateXMatrix = float3x3(1, 0, 0,
									0, c, -s,
									0, s, c);
		
	//float4 fixtureVertexPos = input;
		
	//INVERSION CHECK
	rotateXMatrix = checkTiltInvertZ() == 1 ? transpose(rotateXMatrix) : rotateXMatrix;

	//float4 localRotX = mul(rotateXMatrix, fixtureVertexPos);
	//LOCALROTX IS NEW ROTATION



	//COMBINED ROTATION FOR FIXTURE

	float3x3 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
	float3 localRotXY = mul(rotateXYMatrix, input.xyz);
	//LOCALROTXY IS COMBINED ROTATION

	//Apply fixture rotation ONLY to those with blue vertex colors

	//apply LocalRotXY rotation then add back old origin
	input.xyz = v.color.b == 1.0 ? localRotXY + newOrigin : input.xyz;
	//input.xyz = v.color.b == 1.0 ? input.xyz + newOrigin : input.xyz;
	
	//appy LocalRotY rotation to lightfixture base;
	input.xyz = v.color.g == 1.0 ? localRotY : input.xyz;

	return input;
}

float4 InvertVolumetricRotations (float4 input, float pan, float tilt)
{
	float sX, cX, sY, cY;
	float angleY = radians(getOffsetY() + pan);
	sincos(angleY, sY, cY);
	float3x3 rotateYMatrix = float3x3(cY, sY, 0,
									-sY, cY, 0,
									0, 0, 1);
	float3 BaseAndFixturePos = input.xyz;

		//INVERSION CHECK
	rotateYMatrix = checkPanInvertY() == 1 ? transpose(rotateYMatrix) : rotateYMatrix;

	//float4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);
	//LOCALROTY IS NEW ROTATION


	float tiltOffset = 90.0;
	tiltOffset = checkTiltInvertZ() == 1 ? -tiltOffset : tiltOffset;
	//set new origin to do transform
	float4 newOrigin = input.w * _FixtureRotationOrigin;
	input.xyz -= newOrigin;


	float angleX = radians(getOffsetX() + (tiltOffset) + tilt);
	sincos(angleX, sX, cX);
	float3x3 rotateXMatrix = float3x3(1, 0, 0,
									0, cX, sX,
									0, -sX, cX);

	//float4 fixtureVertexPos = input;

		//INVERSION CHECK
	rotateXMatrix = checkTiltInvertZ() == 1 ? transpose(rotateXMatrix) : rotateXMatrix;
	//float4 localRotX = mul(rotateXMatrix, fixtureVertexPos);

	float3x3 rotateXYMatrix = mul(rotateXMatrix, rotateYMatrix);
	float3 localRotXY = mul(rotateXYMatrix, input);

	input.xyz = localRotXY;
	input.xyz += newOrigin;
	return input;
}

float4 CalculateProjectionScaleRange(appdata v, float4 input, float scalar)
{
	float4 oldinput = input;
	float4 newOrigin = input.w * _ProjectionRangeOrigin;
	input.xyz = input.xyz - newOrigin;
	//Do stretch
	input.xyz = input.xyz * scalar;
	input.xyz = input.xyz + newOrigin;
	input.xyz = (v.color.r == 1.0 && ceil(v.color.g) == 1) ? input.xyz : oldinput;
	return input;


}

float4 CalculateConeWidth(appdata v, float4 input, float scalar, uint dmx)
{
	#if defined(VOLUMETRIC_YES)

		// if((ceil(v.color.r) > 0 && ceil(v.color.g) < 1 && ceil(v.color.b) > 0 ))
		// {

			//Set New Origin
			float4 newOrigin = input.w * _FixtureRotationOrigin; 
			input.xyz = input.xyz - newOrigin;
			scalar = -scalar;

			// Do Transformation
			
			//input.xy = input.xy + v.normal.xy * distanceFromFixture;
		//	v.tangent.y *= 0.1235;
			#ifdef WASH
				scalar *= 2.0;
				scalar -= 2.50;
				//scalar = clamp(scalar, 0.0, 50.0);
			#endif
			// if(v.color.r < 0.9)
			// {
				float distanceFromFixture = (v.uv.x) * (scalar);
				distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));

				input.z = (input.z) + (-v.normal.z) * (distanceFromFixture);
				input.x = (input.x) + (-v.normal.x) * (distanceFromFixture);
				float3 originStretch = input.xyz;
				float3 stretchedcoords = ((-v.tangent.y)*getMaxConeLength(dmx));
				input.xyz = lerp(originStretch, (originStretch * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
				input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);
				//input.xyz = (originStretch * stretchedcoords);
			// }
			// else
			// {
			// 	float distanceFromFixture = (v.uv.x) * scalar;
			// 	distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));
			// 	input.z = (input.z) + (v.normal.z) * distanceFromFixture;
			// 	input.x = (input.x) + (v.normal.x) * distanceFromFixture;
			// 	float3 originStretch = input.xyz;
			// 	float3 stretchedcoords = (-v.tangent.y*getMaxConeLength());
			// 	input.xyz = lerp(originStretch, (originStretch * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
			// 	input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);
			// //	input.xyz = (originStretch * stretchedcoords);

			// }

			//input.y *= _ConeLength;

			//Rest Origin
		//	input.y += _FixtureRotationOrigin.w;
			input.xyz = input.xyz + newOrigin;

			return input;
		// }
		// else
		// {
		// 	return input;
		// }
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
	//float2 result = float2(0.0,0.0);
	switch(goboSelection)
	{
		// case 1:
		// 	return float2(0.0f,0.0f);
		case 2:
			//result.x = _StripeSplit;
			//result.y = _StripeSplitStrength;
			return float2(_StripeSplit, _StripeSplitStrength);
		case 3:
			//result.x = _StripeSplit2;
			//result.y = _StripeSplitStrength2;
			return float2(_StripeSplit2, _StripeSplitStrength2);
		case 4:
			//result.x = _StripeSplit3;
			//result.y = _StripeSplitStrength3;
			return float2(_StripeSplit3, _StripeSplitStrength3);
		case 5:
			//result.x = _StripeSplit4;
			//result.y = _StripeSplitStrength4;
			return float2(_StripeSplit4, _StripeSplitStrength4);
		case 6:
			//result.x = _StripeSplit5;
			//result.y = _StripeSplitStrength5;
			return float2(_StripeSplit5, _StripeSplitStrength5);
		case 7:
		//	result.x = _StripeSplit6;
		//	result.y = _StripeSplitStrength6;
			return float2(_StripeSplit6, _StripeSplitStrength6);
		case 8:
		//	result.x = _StripeSplit7;
		//	result.y = _StripeSplitStrength7;
			return float2(_StripeSplit7, _StripeSplitStrength7);
		default:
			return float2(0.0f,0.0f);
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
	



	uint dmx = getDMXChannel();
	float oscConeWidth = getOSCConeWidth(dmx);
	float oscPanValue = GetPanValue(dmx);
	float oscTiltValue = GetTiltValue(dmx);
	v.vertex = CalculateConeWidth(v, v.vertex, oscConeWidth, dmx);
	v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);
	//calculate rotations for verts
	v.vertex = calculateRotations(v, v.vertex, 0, oscPanValue, oscTiltValue);
	#if defined(PROJECTION_YES)
		o.projectionorigin = calculateRotations(v, _ProjectionRangeOrigin, 0, oscPanValue, oscTiltValue);
	#endif
	#if defined(VOLUMETRIC_YES)
		float3 worldCam;
		worldCam.x = unity_CameraToWorld[0][3];
		worldCam.y = unity_CameraToWorld[1][3];
		worldCam.z = unity_CameraToWorld[2][3];
		float3 objCamPos = mul(unity_WorldToObject, float4(worldCam, 1)).xyz;
		objCamPos = InvertVolumetricRotations(float4(objCamPos,1), oscPanValue, oscTiltValue).xyz;
		float len = length(objCamPos.xy);
		len *= len;
		float4 originScreenPos = ComputeScreenPos(UnityObjectToClipPos(_FixtureRotationOrigin));
		float2 originScreenUV = originScreenPos.xy / originScreenPos.w;
		o.camAngleCamfade.x = saturate((1-distance(float2(0.5, 0.5), originScreenUV))-0.5);
		o.blindingEffect = clamp(0.6/len,1.0,20.0);
		//camAngle = lerp(1, camAngle, len);
		//o.blindingEffect = lerp(1, o.blindingEffect * 2.5, o.camAngleCamfade.x);
	//	 #ifndef WASH
		 	float endBlind = lerp(1.0, o.blindingEffect, 0.15);
			o.blindingEffect = lerp(endBlind, o.blindingEffect * 2.2, o.camAngleCamfade.x);
		// #else
		// 	o.blindingEffect = lerp(1, o.blindingEffect * 2.0, o.camAngleCamfade.x);
		 //#endif
		//o.camAngle = camAngle;
		//o.viewDir.yzw = objCamPos.xyz;
	#endif

	//calculate rotations for normals, cast to float4 first with 0 as w
	float4 newNormals = float4(v.normal.x, v.normal.y, v.normal.z, 0);
	newNormals = calculateRotations(v, newNormals, 1, oscPanValue, oscTiltValue);
	v.normal = newNormals.xyz;

	//calculate rotations for tangents, cast to float4 first with 0 as w
	float4 newTangent = float4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
	newTangent = calculateRotations(v, newTangent, 1, oscPanValue, oscTiltValue);
	v.tangent = newTangent.xyz;

	//original surface shader related code
	#if !defined(VOLUMETRIC_YES)
		float3 worldNormal = UnityObjectToWorldNormal(v.normal);
		float3 tangent = UnityObjectToWorldDir(v.tangent);
		float3 bitangent = cross(tangent, worldNormal);
	#endif


	#if !defined(PROJECTION_YES) && !defined(VOLUMETRIC_YES)
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	#endif

	#if defined(PROJECTION_YES) 
		
		//UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		//move verts to clip space
		o.pos = UnityObjectToClipPos(v.vertex);
		o.emissionColor = getEmissionColor();
		//o.uv = TRANSFORM_TEX(v.uv, _ProjectionMainTex);
		//get screen space position of verts
		o.screenPos = ComputeScreenPos(o.pos);
		//Putting in the vertex position before the transformation seems to somewhat move the projection correctly, but is still incorrect...?
		o.ray = UnityObjectToViewPos(v.vertex).xyz;
		//invert z axis so that it projects from camera properly
		o.ray *= float3(1,1,-1);
		//saving vertex color incase needing to perform rotation calculation in fragment shader
		o.color = v.color;
		o.dmx.x = (float)dmx;

		o.worldPos = mul(unity_ObjectToWorld, v.vertex);

		//For Mirror Depth Correction
		o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
		// pack correction factor into direction w component to save space
		o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
		//o.viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz); // get normalized view dir
		o.viewDir = normalize(UnityObjectToViewPos(v.vertex.xyz));
		o.viewDir /= o.viewDir.z; // rescale vector so z is 1.0
		//GET OSC/DMX VALUES
		o.intensityStrobeWidth = float3(GetOSCIntensity(dmx, 1.0), GetStrobeOutput(dmx), oscConeWidth);
		#ifdef WASH
			float spinSpeed = 0.0;
		#else
			float spinSpeed = getGoboSpinSpeed(dmx);
		#endif
		o.goboPlusSpinPanTilt = float4(getOSCGoboSelection(dmx), spinSpeed, oscPanValue, oscTiltValue);
		o.rgbColor = GetOSCColor(dmx);
		if(((all(o.rgbColor <= float4(0.01,0.01,0.01,1)) || o.intensityStrobeWidth.x <= 0.01) && isOSC() == 1) || getGlobalIntensity() <= 0.005 || getFinalIntensity() <= 0.005 || all(o.emissionColor <= float4(0.005, 0.005, 0.005, 1.0)))
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
		//UNITY_INITIALIZE_OUTPUT(v2f, o);
		//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	//	o.viewDir = ObjSpaceViewDir(v.vertex);
		o.screenPos = ComputeScreenPos (o.pos);
		//o.uvClone = v.uv2;
		o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
		o.uv = TRANSFORM_TEX(v.uv, _LightMainTex);
		//o.uv = UnityStereoScreenSpaceUVAdjust(uv, sb)
		COMPUTE_EYEDEPTH(o.screenPos.z);
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.camAngleCamfade.y = saturate(distance(worldCam, o.worldPos) - 0.5);
		//For Mirror Depth Correction
		o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
		// pack correction factor into direction w component to save space
		o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
		//o.color = v.color;
		//o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		//o.objPos = v.vertex;
		o.objNormal = normalize(v.normal);
		//o.bitan = bitangent;

		//o.tan = tangent;
		//o.norm = worldNormal;
		//GETTING DATA FROM OSC TEXTURE
		o.intensityStrobeGOBOSpinSpeed = float4(GetOSCIntensity(dmx, 1.0),GetStrobeOutput(dmx), getGoboSpinSpeed(dmx), getOSCGoboSelection(dmx));
		o.intensityStrobeGOBOSpinSpeed.x = isOSC() == 1 ? o.intensityStrobeGOBOSpinSpeed.x : 1.0;
		#if !defined(WASH)
		uint gobo = isOSC() > 0 ? ceil(o.intensityStrobeGOBOSpinSpeed.w) : instancedGOBOSelection();
		o.stripeInfo = GetStripeInfo(gobo);
		#endif
		o.rgbColor = GetOSCColor(dmx);
		if(((all(o.rgbColor <= float4(0.005,0.005,0.005,1)) || o.intensityStrobeGOBOSpinSpeed.x <= 0.01) && isOSC() == 1) || getGlobalIntensity() <= 0.005 || getFinalIntensity() <= 0.005)
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
	o.intensityStrobe = float2(GetOSCIntensity(dmx, 1.0),GetStrobeOutput(dmx));
	o.rgbColor = GetOSCColor(dmx);
	o.btn[0] = bitangent;
    o.btn[1] = tangent;
    o.btn[2] = worldNormal;
		#if !defined(VOLUMETRIC_YES) && !defined (PROJECTION_YES)
    		UNITY_TRANSFER_SHADOW(o, o.uv);

		#endif
    #else
	#if !defined(VOLUMETRIC_YES)
	o.color = v.color;
	#endif
    #endif



    return o;
}
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



