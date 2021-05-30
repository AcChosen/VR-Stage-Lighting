#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

// float blockCenter = 0.019231;
// float blockTraversal = 0.03846;

uint checkPanInvertY()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _PanInvert);
}
uint checkTiltInvertZ()
{
    return (uint) UNITY_ACCESS_INSTANCED_PROP(Props, _TiltInvert);
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float4 GetTextureSampleColor()
{
    return tex2Dlod(_SamplingTexture, float4(UNITY_ACCESS_INSTANCED_PROP(Props,_TextureColorSampleX), UNITY_ACCESS_INSTANCED_PROP(Props,_TextureColorSampleY), 0, 0));
}

uint isStrobe()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableStrobe);
}

uint instancedGOBOSelection()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_ProjectionSelection);
}

float getOffsetX()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_FixtureRotationX);
}

float getOffsetY()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_FixtureBaseRotationY);
}

float getStrobeFreq()

{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_StrobeFreq);
}
float4 getEmissionColor()
{
    float4 emissiveColor = UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
    return IF(UNITY_ACCESS_INSTANCED_PROP(Props,_EnableColorTextureSample) > 0,((emissiveColor.r + emissiveColor.g + emissiveColor.b)/3.0) * GetTextureSampleColor(),emissiveColor);
}

float getConeWidth()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_ConeWidth) - 1.5;
}

uint isGOBOSpin()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableSpin);
}

float getConeLength()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _ConeLength);
}

float getGlobalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity);
}

float getFinalIntensity()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
}



// //function for getting the Strobe Value (Channel 7)
// float GetStrobeValue(uint sector)
// {
//     float rawValue = getValueAtCoords(0.250075, 0.020, sector, _OSCGridRenderTextureRAW);
//     float finalValue = IF(rawValue <= 0.35, 0.0, rawValue);
//     uint remappedvalue = clamp(floor(finalValue * 255),10,255);
//     float frequency = clamp(remappedvalue * 0.0980392156862745, 1.0, 25.0);//hz
//     frequency = IF(frequency <= 2.0, 0.0, frequency);
//     return frequency;

// }

// float GetStrobeOutput(uint sector)
// {
//     float2 recoords = getSectorCoordinates(0.019231 + (6.0 * 0.03846), 0.019231, sector);
//     float4 uvcoords = float4(recoords.x, recoords.y, 0,0);
//     float4 c = tex2Dlod(_OSCGridStrobeTimer, uvcoords);
//     half freq = c.r;
//     half multiplier = 4.0;
//     half strobe = IF(sin(freq*multiplier) > 0.0, 1, 0);
//     strobe = IF(freq <= 000.1, 1, strobe);
//     //half output = IF(freq < 370000.0, 1.0, strobe);
//     strobe = IF(isOSC() == 1, strobe, 1);
    
//     return strobe;

// }

// float CalculateStrobe(uint sector)
// {
//     float value = GetStrobeValue(sector);
//     uint remappedvalue = clamp(floor(value * 255),10,255);
//     float frequency = clamp(remappedvalue * 0.0980392156862745, 1.0, 25.0); //hz
//     //float finalValue = sign(sin(frequency * _Time.y));
//     float finalValue = sign(sin(frequency));
//     finalValue = IF(value == 0.0 , 1.0, finalValue);
//     return finalValue;
// }