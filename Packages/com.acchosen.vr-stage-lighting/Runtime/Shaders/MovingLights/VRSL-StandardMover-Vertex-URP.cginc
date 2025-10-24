//This file contains the vertex and fragment functions for both the ForwardBase and Forward Add pass.
//FOR MOVER LIGHT SHADER
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

#ifdef VRSL_DMX
half4 calculateRotations(appdata v, half4 input, int normalsCheck, half pan, half tilt)
{
    //	input = IF(worldspacecheck == 1, half4(UnityObjectToWorldNormal(v.normal).x * -1.0, UnityObjectToWorldNormal(v.normal).y * -1.0, UnityObjectToWorldNormal(v.normal).z * -1.0, 1), input)
    //CALCULATE BASE ROTATION. MORE FUN MATH. THIS IS FOR PAN.
    half angleY = radians(getOffsetY() + pan);
    half c, s;
    sincos(angleY, s, c);

    half3x3 rotateYMatrix = half3x3(c, -s, 0,
                                    s, c, 0,
                                    0, 0, 1);
    half3 BaseAndFixturePos = input.xyz;

    //INVERSION CHECK
    rotateYMatrix = checkPanInvertY() == 1 ? transpose(rotateYMatrix) : rotateYMatrix;

    half3 localRotY = mul(rotateYMatrix, BaseAndFixturePos);
    //LOCALROTY IS NEW ROTATION


    //CALCULATE FIXTURE ROTATION. WOO FUN MATH. THIS IS FOR TILT.

    //set new origin to do transform
    half3 newOrigin = input.w * _FixtureRotationOrigin.xyz;
    //if input.w is 1 (vertex), origin changes
    //if input.w is 0 (normal/tangent), origin doesn't change

    //subtract new origin from original origin for blue vertexes
    input.xyz = v.color.b == 1.0 ? input.xyz - newOrigin : input.xyz;


    //DO ROTATION


    //#if defined(PROJECTION_YES)
    //buffer[3] = GetTiltValue(sector);
    //#endif
    half angleX = radians(getOffsetX() + tilt);
    sincos(angleX, s, c);
    half3x3 rotateXMatrix = half3x3(1, 0, 0,
                                    0, c, -s,
                                    0, s, c);

    //half4 fixtureVertexPos = input;

    //INVERSION CHECK
    rotateXMatrix = checkTiltInvertZ() == 1 ? transpose(rotateXMatrix) : rotateXMatrix;

    //half4 localRotX = mul(rotateXMatrix, fixtureVertexPos);
    //LOCALROTX IS NEW ROTATION


    //COMBINED ROTATION FOR FIXTURE

    half3x3 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
    half3 localRotXY = mul(rotateXYMatrix, input.xyz);
    //LOCALROTXY IS COMBINED ROTATION

    //Apply fixture rotation ONLY to those with blue vertex colors

    //apply LocalRotXY rotation then add back old origin
    input.xyz = v.color.b == 1.0 ? localRotXY + newOrigin : input.xyz;
    //input.xyz = v.color.b == 1.0 ? input.xyz + newOrigin : input.xyz;

    //appy LocalRotY rotation to lightfixture base;
    input.xyz = v.color.g == 1.0 ? localRotY : input.xyz;

    return input;
}

half4 InvertVolumetricRotations(half4 input, half pan, half tilt)
{
    half sX, cX, sY, cY;
    half angleY = radians(getOffsetY() + pan);
    sincos(angleY, sY, cY);
    half3x3 rotateYMatrix = half3x3(cY, sY, 0,
                                    -sY, cY, 0,
                                    0, 0, 1);
    half3 BaseAndFixturePos = input.xyz;

    //INVERSION CHECK
    rotateYMatrix = checkPanInvertY() == 1 ? transpose(rotateYMatrix) : rotateYMatrix;

    //half4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);
    //LOCALROTY IS NEW ROTATION


    half tiltOffset = 90.0;
    tiltOffset = checkTiltInvertZ() == 1 ? -tiltOffset : tiltOffset;
    //set new origin to do transform
    half4 newOrigin = input.w * _FixtureRotationOrigin;
    input.xyz -= newOrigin;


    half angleX = radians(getOffsetX() + (tiltOffset) + tilt);
    sincos(angleX, sX, cX);
    half3x3 rotateXMatrix = half3x3(1, 0, 0,
                                    0, cX, sX,
                                    0, -sX, cX);

    //half4 fixtureVertexPos = input;

    //INVERSION CHECK
    rotateXMatrix = checkTiltInvertZ() == 1 ? transpose(rotateXMatrix) : rotateXMatrix;
    //half4 localRotX = mul(rotateXMatrix, fixtureVertexPos);

    half3x3 rotateXYMatrix = mul(rotateXMatrix, rotateYMatrix);
    half3 localRotXY = mul(rotateXYMatrix, input);

    input.xyz = localRotXY;
    input.xyz += newOrigin;
    return input;
}
#endif


half4 CalculateProjectionScaleRange(appdata v, half4 input, half scalar)
{
    half4 oldinput = input;
    half4 newOrigin = input.w * _ProjectionRangeOrigin;
    input.xyz = input.xyz - newOrigin;
    //Do stretch
    input.xyz = input.xyz * scalar;
    input.xyz = input.xyz + newOrigin;
    input.xyz = (v.color.r == 1.0 && ceil(v.color.g) == 1) ? input.xyz : oldinput;
    return input;
}

half4 ConeScale(appdata v, half4 input, half scalar)
{
    //Set New Origin
    #ifdef VRSL_DMX
    half4 newOrigin = input.w * _FixtureRotationOrigin;
    input.xyz = input.xyz - newOrigin;
    //scalar = -scalar;

    half4x4 scaleMatrix = half4x4(scalar, 0, 0, 0,
                                  0, 1, 0, 0,
                                  0, 0, scalar, 0,
                                  0, 0, 0, 1);

    input = mul(input, scaleMatrix);
    input.xyz = input.xyz + newOrigin;
    return input;
    #endif
    #ifdef VRSL_AUDIOLINK
		half4 newOrigin = input.w * _FixtureRotationOrigin; 
		input.xyz = input.xyz - newOrigin;
		//scalar = -scalar;

		half4x4 scaleMatrix 	= half4x4(scalar,	0,	0,	0,
												0,	scalar,  0,	0,
												0,	0,	1,0,
												0,	0,	0,	1);

		input = mul(input,scaleMatrix);
		input.xyz = input.xyz + newOrigin;
		return input;
    #endif
}
#ifdef VRSL_DMX
half4 CalculateConeWidth(appdata v, half4 input, half scalar, uint dmx)
{
    #if defined(VOLUMETRIC_YES)

			// if((ceil(v.color.r) > 0 && ceil(v.color.g) < 1 && ceil(v.color.b) > 0 ))
			// {

				//Set New Origin
				half4 newOrigin = input.w * _FixtureRotationOrigin; 
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
					half distanceFromFixture = (v.uv.x) * (scalar);
					distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, _ConeSync));

					input.z = (input.z) + (-v.normal.z) * (distanceFromFixture);
					input.x = (input.x) + (-v.normal.x) * (distanceFromFixture);
					half3 originStretch = input.xyz;
					half3 stretchedcoords = ((-v.tangent.y)*getMaxConeLength(dmx));
					input.xyz = lerp(originStretch, (originStretch * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
					input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);

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
				half4 newOrigin = input.w * _ProjectionRangeOrigin; 
				input.xyz = input.xyz - newOrigin;

				// Do Transformation
				half distanceFromFixture = (v.texcoord.x) * scalar;
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
#endif
#ifdef VRSL_AUDIOLINK
	half4 CalculateConeWidth(appdata v, half4 input, half scalar)
	{
#if defined(VOLUMETRIC_YES)
#ifdef WASH
					scalar *= 2.5;
					scalar += 2.50;
					//scalar = clamp(scalar, 0.0, 50.0);
#endif

				//Set New Origin
				half4 newOrigin = input.w * _FixtureRotationOrigin; 
				input.xyz = input.xyz - newOrigin;
				scalar = -scalar;
				// Do Transformation
				half distanceFromFixture = (v.uv.x) * (scalar);
				distanceFromFixture = lerp(0, distanceFromFixture, pow(v.uv.x, 0.05));
				input.y = (input.y) + (-v.normal.y) * (distanceFromFixture);
				input.x = (input.x) + (-v.normal.x) * (distanceFromFixture);
				half3 originStretch = input.xyz;
				half3 stretchedcoords = (v.tangent.z*getMaxConeLength());
				input.xyz = lerp(originStretch, (input.xyz * stretchedcoords), pow(v.uv.x,lerp(1, 0.1, v.uv.x)-0.5));
				input.xyz = IF(v.uv.x < 0.001, originStretch, input.xyz);
				
				input.xyz = input.xyz + newOrigin;

				return input;
#endif

#if defined(PROJECTION_YES)
			if(ceil(v.color.g) >= 0.9 && ceil(v.color.r) >= 0.5)
			{
				half4 newOrigin = input.w * _ProjectionRangeOrigin; 
				input.xyz = input.xyz - newOrigin;

				// Do Transformation
				half distanceFromFixture = (v.texcoord.x) * scalar;
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
#endif


inline float4 CalculateFrustumCorrection()
{
    float x1 = -UNITY_MATRIX_P._31 / (UNITY_MATRIX_P._11 * UNITY_MATRIX_P._34);
    float x2 = -UNITY_MATRIX_P._32 / (UNITY_MATRIX_P._22 * UNITY_MATRIX_P._34);
    return float4(
        x1, x2, 0, UNITY_MATRIX_P._33 / UNITY_MATRIX_P._34 + x1 * UNITY_MATRIX_P._13 + x2 * UNITY_MATRIX_P._23);
}

half2 GetStripeInfo(uint goboSelection)
{
    switch (goboSelection)
    {
    case 2:
        return half2(_StripeSplit, _StripeSplitStrength);
    case 3:
        return half2(_StripeSplit2, _StripeSplitStrength2);
    case 4:
        return half2(_StripeSplit3, _StripeSplitStrength3);
    case 5:
        return half2(_StripeSplit4, _StripeSplitStrength4);
    case 6:
        return half2(_StripeSplit5, _StripeSplitStrength5);
    case 7:
        return half2(_StripeSplit6, _StripeSplitStrength6);
    case 8:
        return half2(_StripeSplit7, _StripeSplitStrength7);
    default:
        return half2(0.0f, 0.0f);
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////START VERTEX SHADERS///////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//VERTEX SHADER
v2f vert(appdata v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    ////////////////////////////////////////START DMX VERTEX//////////////////////////////////////////////////////////////////////
    #ifdef VRSL_DMX
    uint dmx = getDMXChannel();
    half oscConeWidth = getDMXConeWidth(dmx);
    half oscPanValue = GetPanValue(dmx);
    half oscTiltValue = GetTiltValue(dmx);


    v.vertex = CalculateConeWidth(v, v.vertex, oscConeWidth, dmx);
    v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);

    #if defined(VOLUMETRIC_YES)
    #ifdef _ALPHATEST_ON
				v.vertex = ConeScale(v, v.vertex, _MinimumBeamRadius-0.25);
    #else
				v.vertex = ConeScale(v, v.vertex, _MinimumBeamRadius);
    #endif
    #endif
    //calculate rotations for verts
    v.vertex = calculateRotations(v, v.vertex, 0, oscPanValue, oscTiltValue);
    #if defined(PROJECTION_YES)
			o.projectionorigin = calculateRotations(v, _ProjectionRangeOrigin, 0, oscPanValue, oscTiltValue);
    #endif
    #if defined(VOLUMETRIC_YES)
			o.coneWidth = oscConeWidth + 1.5;
			float3 worldCam;
			worldCam.x = unity_CameraToWorld[0][3];
			worldCam.y = unity_CameraToWorld[1][3];
			worldCam.z = unity_CameraToWorld[2][3];
			float3 objCamPos = mul(unity_WorldToObject, float4(worldCam, 1)).xyz;
			objCamPos = InvertVolumetricRotations(float4(objCamPos,1), oscPanValue, oscTiltValue).xyz;
			
    #ifdef _ALPHATEST_ON
				half len = length(objCamPos.xy);
				len *= (_BlindingAngleMod);
    #else
				half len = length(objCamPos.xy);
				len *= (len * _BlindingAngleMod);
    #endif
			float4 originScreenPos = ComputeScreenPos(UnityObjectToClipPos(_FixtureRotationOrigin));
			float2 originScreenUV = originScreenPos.xy / originScreenPos.w;
			o.camAngleCamfade.x = saturate((1-distance(half2(0.5, 0.5), originScreenUV))-0.5);
			
			//camAngle = lerp(1, camAngle, len);
			//o.blindingEffect = lerp(1, o.blindingEffect * 2.5, o.camAngleCamfade.x);
		//	 #ifndef WASH

    #ifdef _ALPHATEST_ON
					o.blindingEffect = clamp(0.6/len,1.0,20.0);
					half endBlind = 1.0;
					o.blindingEffect = lerp(endBlind, o.blindingEffect, o.camAngleCamfade.x);
    #else
					o.blindingEffect = clamp(0.6/len,1.0,20.0);
					half endBlind = lerp(1.0, o.blindingEffect, 0.15);
					o.blindingEffect = lerp(endBlind, o.blindingEffect * 2.2, o.camAngleCamfade.x);	
    #endif
    // #else
    // 	o.blindingEffect = lerp(1, o.blindingEffect * 2.0, o.camAngleCamfade.x);
    //#endif
    //o.camAngle = camAngle;
    //o.viewDir.yzw = objCamPos.xyz;
    #endif

    //calculate rotations for normals, cast to half4 first with 0 as w
    half4 newNormals = half4(v.normal.x, v.normal.y, v.normal.z, 0);
    newNormals = calculateRotations(v, newNormals, 1, oscPanValue, oscTiltValue);
    v.normal = newNormals.xyz;

    //calculate rotations for tangents, cast to half4 first with 0 as w
    // half4 newTangent = half4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
    // newTangent = calculateRotations(v, newTangent, 1, oscPanValue, oscTiltValue);
    // v.tangent = newTangent.xyz;

    #if defined(FIXTURE_SHADOWCAST)
			//o.pos = UnityObjectToClipPos(v.vertex);
			o.pos = UnityClipSpaceShadowCasterPos(v.vertex, v.normal);
			o.pos = UnityApplyLinearShadowBias(o.pos);
			//o.normal = v.normal;
			//TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
			return o;
    #endif

    //original surface shader related code
    #if !defined(VOLUMETRIC_YES) && !defined(FIXTURE_SHADOWCAST)
    half3 worldNormal = UnityObjectToWorldNormal(v.normal);
    half3 tangent = UnityObjectToWorldDir(v.tangent);
    half3 bitangent = cross(tangent, worldNormal);
    #endif


    #if !defined(PROJECTION_YES) && !defined(VOLUMETRIC_YES) && !defined(FIXTURE_SHADOWCAST)
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
			o.ray *= half3(1,1,-1);
			//saving vertex color incase needing to perform rotation calculation in fragment shader
			o.color = v.color;
			o.dmx.x = (half)dmx;

			o.worldPos = mul(unity_ObjectToWorld, v.vertex);

			//For Mirror Depth Correction
			o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
			// pack correction factor into direction w component to save space
			o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
			//o.viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz); // get normalized view dir
			o.viewDir = normalize(UnityObjectToViewPos(v.vertex.xyz));
			o.viewDir /= o.viewDir.z; // rescale vector so z is 1.0
			//GET DMX/DMX VALUES
			o.intensityStrobeWidth = half3(GetDMXIntensity(dmx, 1.0), GetStrobeOutput(dmx), oscConeWidth);
    #ifdef WASH
				half spinSpeed = 0.0;
    #else
				half spinSpeed = getGoboSpinSpeed(dmx);
    #endif
			o.goboPlusSpinPanTilt = half4(getDMXGoboSelection(dmx), spinSpeed, oscPanValue, oscTiltValue);
			o.rgbColor = GetDMXColor(dmx);
			if(((all(o.rgbColor <= half4(0.01,0.01,0.01,1)) || o.intensityStrobeWidth.x <= 0.01) && isDMX() == 1) || getGlobalIntensity() <= 0.005 || getFinalIntensity() <= 0.005 || all(o.emissionColor <= half4(0.005, 0.005, 0.005, 1.0)))
			{
				v.vertex = half4(0,0,0,0);
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
    #ifdef _HQ_MODE
				o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTexHigh);
    #else
				o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
    #endif
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
			//GETTING DATA FROM DMX TEXTURE
			o.intensityStrobeGOBOSpinSpeed = half4(GetDMXIntensity(dmx, 1.0),GetStrobeOutput(dmx), getGoboSpinSpeed(dmx), getDMXGoboSelection(dmx));
			o.intensityStrobeGOBOSpinSpeed.x = isDMX() == 1 ? o.intensityStrobeGOBOSpinSpeed.x : 1.0;
    #if !defined(WASH)
			uint gobo = isDMX() > 0 ? ceil(o.intensityStrobeGOBOSpinSpeed.w) : instancedGOBOSelection();
			o.stripeInfo = GetStripeInfo(gobo);
    #endif
			o.rgbColor = GetDMXColor(dmx);
			if(((all(o.rgbColor <= half4(0.005,0.005,0.005,1)) || o.intensityStrobeGOBOSpinSpeed.x <= 0.01) && isDMX() == 1) || getGlobalIntensity() <= 0.005 || getFinalIntensity() <= 0.005)
			{
				v.vertex = half4(0,0,0,0);
				o.pos = UnityObjectToClipPos(v.vertex);
			} 
    #endif

    //Projection Part - Vertex Shader
    // #if defined(PROJECTION_YES)


    // #endif

    #if !defined(UNITY_PASS_SHADOWCASTER) && !defined(PROJECTION_YES) && !defined(VOLUMETRIC_YES)

    o.color = v.color;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);


    o.intensityStrobe = half2(GetDMXIntensity(dmx, 1.0), GetStrobeOutput(dmx));
    o.rgbColor = GetDMXColor(dmx);
    o.btn[0] = bitangent;
    o.btn[1] = tangent;
    o.btn[2] = worldNormal;
    #ifdef _LIGHTING_MODEL
			o.eyeVec.xyz = NormalizePerVertexNormal(o.worldPos.xyz - _WorldSpaceCameraPos);
			o.ambientOrLightmapUV = VertexGIForward(v, o.worldPos, o.btn[2]);
    #else
    o.objPos = v.vertex;
    o.objNormal = v.normal;
    #endif
    #if !defined(VOLUMETRIC_YES) && !defined (PROJECTION_YES)
    UNITY_TRANSFER_SHADOW(o, o.uv);

    #endif
    #else
    #if !defined(VOLUMETRIC_YES) && !defined(FIXTURE_SHADOWCAST)
				o.color = v.color;
    #endif
    #endif
    #endif
    ////////////////////////////////////////END DMX VERTEX//////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////START AUDIOLINK VERTEX//////////////////////////////////////////////////////////////////////

    #ifdef VRSL_AUDIOLINK
		v.vertex = CalculateConeWidth(v, v.vertex, getConeWidth());
		v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);

    #if defined(VOLUMETRIC_YES)
    #ifdef _ALPHATEST_ON
				v.vertex = ConeScale(v, v.vertex, _MinimumBeamRadius-0.25);
    #else
				v.vertex = ConeScale(v, v.vertex, _MinimumBeamRadius);
    #endif
    #endif
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
			o.coneWidth = getConeWidth() + 1.25;
			worldCam.x = unity_CameraToWorld[0][3];
			worldCam.y = unity_CameraToWorld[1][3];
			worldCam.z = unity_CameraToWorld[2][3];
			half3 objCamPos = mul(unity_WorldToObject, float4(worldCam, 1)).xyz;
			//objCamPos = InvertVolumetricRotations(float4(objCamPos,1)).xyz;
			half len = length(objCamPos.xy);
			len *= len;
			o.camAngleLen.y = len;
			float4 originScreenPos = ComputeScreenPos(UnityObjectToClipPos(half4(0,0,0,0)));
			float2 originScreenUV = originScreenPos.xy / originScreenPos.w;
			o.camAngleLen.x = saturate((1-distance(half2(0.5, 0.5), originScreenUV))-0.5);
			o.camAngleLen.x = pow(o.camAngleLen.x, 0.5);
			o.blindingEffect = clamp(0.6/len,1.0,8.0);
			//camAngle = lerp(1, camAngle, len);


    #ifndef WASH
				half endBlind = lerp(1.0, o.blindingEffect, 0.35);
				o.blindingEffect = lerp(endBlind, o.blindingEffect * 2.2, o.camAngleLen.x);
    #else
				o.blindingEffect = lerp(1, o.blindingEffect * 2.0, o.camAngleLen.x);
    #endif
    //o.camAngle = camAngle;
    //o.viewDir.yzw = objCamPos.xyz;
    #endif

		//calculate rotations for normals, cast to half4 first with 0 as w
		half4 newNormals = half4(v.normal.x, v.normal.y, v.normal.z, 0);
		//newNormals = calculateRotations(v, newNormals, 1);
		v.normal = newNormals.xyz;

		//calculate rotations for tangents, cast to half4 first with 0 as w
		half4 newTangent = half4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
		//newTangent = calculateRotations(v, newTangent, 1);
		v.tangent = newTangent.xyz;

		//original surface shader related code
		half3 worldNormal = UnityObjectToWorldNormal(v.normal);
		half3 tangent = UnityObjectToWorldDir(v.tangent);
		half3 bitangent = cross(tangent, worldNormal);


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
			o.ray *= half3(1,1,-1);
			//saving vertex color incase needing to perform rotation calculation in fragment shader
			o.color = v.color;
			//o.sector.x = (half)sector;

			o.worldPos = mul(unity_ObjectToWorld, v.vertex);

			//For Mirror Depth Correction
			o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
			// pack correction factor into direction w component to save space
			o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
			//o.viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);
			o.viewDir = normalize(UnityObjectToViewPos(v.vertex.xyz)); // get normalized view dir
			o.viewDir /= o.viewDir.z; // rescale vector so z is 1.0
    #ifdef RAW
				if(o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor.xyz <= half4(0.005, 0.005, 0.005, 1.0)))
				{
					v.vertex = half4(0,0,0,0);
					o.pos = UnityObjectToClipPos(v.vertex);
				}
    #else
				if(o.audioGlobalFinalConeIntensity.x <= 0.005 || o.audioGlobalFinalConeIntensity.y <= 0.005 || o.audioGlobalFinalConeIntensity.z <= 0.005 || all(o.emissionColor.xyz <= half4(0.005, 0.005, 0.005, 1.0)))
				{
					v.vertex = half4(0,0,0,0);
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
    #if _USE_DEPTH_LIGHT
				o.screenPos = ComputeScreenPos (o.pos);
    #else
				o.screenPos = half4(0,0,0,0);
    #endif
			o.uvClone = v.uv2;
    #ifdef _HQ_MODE
				o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTexHigh);
    #else
				o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
    #endif
			o.uv = TRANSFORM_TEX(v.uv, _LightMainTex);
			//o.uv = UnityStereoScreenSpaceUVAdjust(uv, sb)
    #if _USE_DEPTH_LIGHT
				COMPUTE_EYEDEPTH(o.screenPos.z);
    #endif
			UNITY_TRANSFER_FOG(o,o.vertex);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);

			//For Mirror Depth Correction
    #if _USE_DEPTH_LIGHT
				o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
				// pack correction factor into direction w component to save space
				o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
    #else
				o.worldDirection = half4(0,0,0,0);
    #endif
			o.color = v.color;
			//o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.objPos = v.vertex;
			o.objNormal = v.normal;
			o.stripeInfo = GetStripeInfo(instancedGOBOSelection());
			o.norm = worldNormal;
    #ifdef RAW
				if(o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor.xyz <= half4(0.005, 0.005, 0.005, 1.0)))
				{
					v.vertex = half4(0,0,0,0);
					o.pos = UnityObjectToClipPos(v.vertex);
				}
    #else
				if(o.audioGlobalFinalIntensity.x <= 0.005 || o.audioGlobalFinalIntensity.y <= 0.005 || o.audioGlobalFinalIntensity.z <= 0.005 || all(o.emissionColor.xyz <= half4(0.005, 0.005, 0.005, 1.0)))
				{
					v.vertex = half4(0,0,0,0);
					o.pos = UnityObjectToClipPos(v.vertex);
				}
    #endif
    #endif

    #if !defined(UNITY_PASS_SHADOWCASTER) && !defined(PROJECTION_YES) && !defined(VOLUMETRIC_YES)
		
		o.color = v.color;
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.btn[0] = bitangent;
		o.btn[1] = tangent;
		o.btn[2] = worldNormal;
    #ifdef _LIGHTING_MODEL
			o.eyeVec.xyz = NormalizePerVertexNormal(o.worldPos.xyz - _WorldSpaceCameraPos);
			o.ambientOrLightmapUV = VertexGIForward(v, o.worldPos, o.btn[2]);
    #else
			o.objPos = v.vertex;
			o.objNormal = v.normal;
    #endif
    #if !defined(VOLUMETRIC_YES) && !defined (PROJECTION_YES)
				UNITY_TRANSFER_SHADOW(o, o.uv);

    #endif
    #else
		o.color = v.color;
    #endif
    #endif

    ////////////////////////////////////////END AUDIOLINK VERTEX//////////////////////////////////////////////////////////////////////

    return o;
}

v2f vertFixture(appdata v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    ////////////////////////////////////////START DMX VERTEX//////////////////////////////////////////////////////////////////////
    #ifdef VRSL_DMX
        uint dmx = getDMXChannel();
        half oscConeWidth = getDMXConeWidth(dmx);
        half oscPanValue = GetPanValue(dmx);
        half oscTiltValue = GetTiltValue(dmx);


        v.vertex = CalculateConeWidth(v, v.vertex, oscConeWidth, dmx);
        v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);

        //calculate rotations for verts
        v.vertex = calculateRotations(v, v.vertex, 0, oscPanValue, oscTiltValue);

        //calculate rotations for normals, cast to half4 first with 0 as w
        half4 newNormals = half4(v.normal.x, v.normal.y, v.normal.z, 0);
        newNormals = calculateRotations(v, newNormals, 1, oscPanValue, oscTiltValue);
        v.normal = newNormals.xyz;

        //calculate rotations for tangents, cast to half4 first with 0 as w
        // half4 newTangent = half4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
        // newTangent = calculateRotations(v, newTangent, 1, oscPanValue, oscTiltValue);
        // v.tangent = newTangent.xyz;

        #if defined(FIXTURE_SHADOWCAST)
		    //o.pos = UnityObjectToClipPos(v.vertex);
		    o.pos = UnityClipSpaceShadowCasterPos(v.vertex, v.normal);
		    o.pos = UnityApplyLinearShadowBias(o.pos);
		    //o.normal = v.normal;
		    //TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
		    return o;
        #endif

        //original surface shader related code
        half3 worldNormal = UnityObjectToWorldNormal(v.normal);
        half3 tangent = UnityObjectToWorldDir(v.tangent);
        half3 bitangent = cross(tangent, worldNormal);


        #if !defined(FIXTURE_SHADOWCAST)
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        #endif

        #if defined(UNITY_PASS_FORWARDBASE)
            o.uv1 = v.uv1;
            o.uv2 = v.uv2;
        #endif


        #if !defined(UNITY_PASS_SHADOWCASTER)
            o.color = v.color;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);

            o.intensityStrobe = half2(GetDMXIntensity(dmx, 1.0), GetStrobeOutput(dmx));
            o.rgbColor = GetDMXColor(dmx);
            o.btn[0] = bitangent;
            o.btn[1] = tangent;
            o.btn[2] = worldNormal;
            #ifdef _LIGHTING_MODEL
			    o.eyeVec.xyz = NormalizePerVertexNormal(o.worldPos.xyz - _WorldSpaceCameraPos);
			    o.ambientOrLightmapUV = VertexGIForward(v, o.worldPos, o.btn[2]);
            #else
                o.objPos = v.vertex;
                o.objNormal = v.normal;
            #endif
            UNITY_TRANSFER_SHADOW(o, o.uv);
        #else
			o.color = v.color;
        #endif
    #endif
    ////////////////////////////////////////END DMX VERTEX//////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////START AUDIOLINK VERTEX//////////////////////////////////////////////////////////////////////

    #ifdef VRSL_AUDIOLINK
		v.vertex = CalculateConeWidth(v, v.vertex, getConeWidth());
		v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);


		//calculate rotations for normals, cast to half4 first with 0 as w
		half4 newNormals = half4(v.normal.x, v.normal.y, v.normal.z, 0);
		//newNormals = calculateRotations(v, newNormals, 1);
		v.normal = newNormals.xyz;

		//calculate rotations for tangents, cast to half4 first with 0 as w
		half4 newTangent = half4(v.tangent.x, v.tangent.y, v.tangent.z, 0);
		//newTangent = calculateRotations(v, newTangent, 1);
		v.tangent = newTangent.xyz;

		//original surface shader related code
		half3 worldNormal = UnityObjectToWorldNormal(v.normal);
		half3 tangent = UnityObjectToWorldDir(v.tangent);
		half3 bitangent = cross(tangent, worldNormal);


			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		
    #if defined(UNITY_PASS_FORWARDBASE)
		o.uv1 = v.uv1;
		o.uv2 = v.uv2;
    #endif


		o.color = v.color;
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.btn[0] = bitangent;
		o.btn[1] = tangent;
		o.btn[2] = worldNormal;
    #ifdef _LIGHTING_MODEL
		o.eyeVec.xyz = NormalizePerVertexNormal(o.worldPos.xyz - _WorldSpaceCameraPos);
		o.ambientOrLightmapUV = VertexGIForward(v, o.worldPos, o.btn[2]);
    #else
		o.objPos = v.vertex;
		o.objNormal = v.normal;
    #endif
		UNITY_TRANSFER_SHADOW(o, o.uv);

    #else
        o.color = v.color;
    #endif

    ////////////////////////////////////////END AUDIOLINK VERTEX//////////////////////////////////////////////////////////////////////

    return o;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////START FRAG SHADERS//////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#if defined(VOLUMETRIC_YES)
fixed4 frag (v2f i, fixed facePos : VFACE) : SV_Target
{
	return VolumetricLightingBRDF(i, facePos);
}
#endif
#if !defined(VOLUMETRIC_YES)
fixed4 frag(v2f i) : SV_Target
{
    //Return only this if in the shadowcaster
    #if defined(UNITY_PASS_SHADOWCASTER) && !defined(FIXTURE_SHADOWCAST)
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

    #elif defined (FIXTURE_SHADOWCAST)
		SHADOW_CASTER_FRAGMENT(i);
    
    #else
    return CustomStandardLightingBRDF(i);
    #endif
}
#endif
