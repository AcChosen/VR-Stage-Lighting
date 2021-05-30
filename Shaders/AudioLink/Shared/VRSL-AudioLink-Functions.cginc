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

///////////////////////////////////////////////////////////////////////////////////////////
float getNumBands()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _NumBands);
}

float getBand()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _Band);
}

float getDelay()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _Delay);
}

float getBandMultiplier()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _BandMultiplier);
}

float checkIfAudioLink()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableAudioLink);
}
////////////////////////////////////////////////////////////////////////////////////////////


float GetAudioReactAmplitude()
{
    if(checkIfAudioLink() > 0)
    {
        // float temp_output_22_0 = ( 1.0 / getNumBands() );
        // float2 appendResult30 = (float2(( ( getDelay() * 0.03125 ) + ( 0.03125 * 0.5 ) ) , ( ( getBand() * temp_output_22_0 ) + ( temp_output_22_0 * 0.5 ) )));
        // float4 tex2DNode19 = tex2D( _AudioSpectrum, appendResult30 );
        // float amplitude36 = tex2DNode19.r;
            float2 break6_g1 = float2(0.5,0.5);
			float temp_output_5_0_g1 = ( break6_g1.x - 0.5 );
			//float _PulseRotation_Instance = UNITY_ACCESS_INSTANCED_PROP(_PulseRotation_arr, _PulseRotation);
			//float temp_output_2_0_g1 = radians( _PulseRotation_Instance );
			float temp_output_3_0_g1 = 1;
			float temp_output_8_0_g1 = 0;
			float temp_output_20_0_g1 = 1;
			float temp_output_7_0_g1 = ( break6_g1.y - 0.5 );
			float2 appendResult16_g1 = (float2(( ( ( temp_output_5_0_g1 * temp_output_3_0_g1 * temp_output_20_0_g1 ) + ( temp_output_7_0_g1 * temp_output_8_0_g1 * temp_output_20_0_g1 ) ) + 0.5 ) , ( ( ( temp_output_7_0_g1 * temp_output_3_0_g1 * temp_output_20_0_g1 ) - ( temp_output_5_0_g1 * temp_output_8_0_g1 * temp_output_20_0_g1 ) ) + 0.5 )));
			float _Pulse_Instance = 0;
			float _Delay_Instance = getDelay();
			float _Band_Instance = getBand();
			float2 appendResult9_g2 = (float2(( (_Delay_Instance + (( appendResult16_g1.x * _Pulse_Instance ) - 0.0) * (1.0 - _Delay_Instance) / (1.0 - 0.0)) % 1.0 ) , ( ( ( _Band_Instance * 0.25 ) + 0.125 ) * 0.0625 )));
			float4 tex2DNode19 = tex2D( _AudioSpectrum, appendResult9_g2 );
			float amplitude36 = tex2DNode19.r;
        //float amplitude36 = 1.0;
        return amplitude36 * getBandMultiplier();
    }
    else
    {
        return 1;
    }

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