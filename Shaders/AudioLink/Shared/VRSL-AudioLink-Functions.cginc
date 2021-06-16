
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

// Map of where features in AudioLink are.
#define ALPASS_DFT              int2(0,4)
#define ALPASS_WAVEFORM         int2(0,6)
#define ALPASS_AUDIOLINK        int2(0,0)
#define ALPASS_AUDIOLINKHISTORY int2(1,0) 
#define ALPASS_GENERALVU        int2(0,22)

#define ALPASS_GENERALVU_INSTANCE_TIME   int2(2,22)
#define ALPASS_GENERALVU_LOCAL_TIME      int2(3,22)

#define ALPASS_CCINTERNAL       int2(12,22)
#define ALPASS_CCSTRIP          int2(0,24)
#define ALPASS_CCLIGHTS         int2(0,25)
#define ALPASS_AUTOCORRELATOR   int2(0,27)

// Some basic constants to use (Note, these should be compatible with
// future version of AudioLink, but may change.
#define CCMAXNOTES 10
#define SAMPHIST 3069 //Internal use for algos, do not change.
#define SAMPLEDATA24 2046
#define EXPBINS 24
#define EXPOCT 10
#define ETOTALBINS ((EXPBINS)*(EXPOCT))
#define AUDIOLINK_WIDTH  128
#define _SamplesPerSecond 48000
#define _RootNote 0

#define AudioLinkData(xycoord) tex2Dlod( _AudioSpectrum, float4( uint2(xycoord) * _AudioSpectrum_TexelSize.xy, 0, 0 ) )

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

uint checkIfColorChord()
{
    return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableColorChord);
}
////////////////////////////////////////////////////////////////////////////////////////////


float GetAudioReactAmplitude()
{
    if(checkIfAudioLink() > 0)
    {

            float2 break6_g1 = float2(0.5,0.5);
			float temp_output_5_0_g1 = ( break6_g1.x - 0.5 );
			float temp_output_3_0_g1 = 1;
			float temp_output_8_0_g1 = 0;
			float temp_output_20_0_g1 = 1;
			float temp_output_7_0_g1 = ( break6_g1.y - 0.5 );
			float2 appendResult16_g1 = (float2(( ( ( temp_output_5_0_g1 * temp_output_3_0_g1 * temp_output_20_0_g1 ) + ( temp_output_7_0_g1 * temp_output_8_0_g1 * temp_output_20_0_g1 ) ) + 0.5 ) , ( ( ( temp_output_7_0_g1 * temp_output_3_0_g1 * temp_output_20_0_g1 ) - ( temp_output_5_0_g1 * temp_output_8_0_g1 * temp_output_20_0_g1 ) ) + 0.5 )));
			float _Pulse_Instance = 0;
			float _Delay_Instance = getDelay();
			float _Band_Instance = getBand();
			float2 appendResult9_g2 = (float2(( (_Delay_Instance + (( appendResult16_g1.x * _Pulse_Instance ) - 0.0) * (1.0 - _Delay_Instance) / (1.0 - 0.0)) % 1.0 ) , ( ( ( _Band_Instance * 0.25 ) + 0.125 ) * 0.0625 )));
			float4 tex2DNode19 = tex2Dlod( _AudioSpectrum, float4(appendResult9_g2,0,0));
			float amplitude36 = tex2DNode19.r;
        return amplitude36 * getBandMultiplier();
    }
    else
    {
        return 1;
    }

}

float4 GetColorChordLight()
{
    return AudioLinkData(ALPASS_CCLIGHTS).rgba;
}

float4 getEmissionColor()
{
    float4 emissiveColor = UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
    float4 col =  IF(UNITY_ACCESS_INSTANCED_PROP(Props,_EnableColorTextureSample) > 0,((emissiveColor.r + emissiveColor.g + emissiveColor.b)/3.0) * GetTextureSampleColor(),emissiveColor);
    return IF(checkIfColorChord() == 1, GetColorChordLight(), col);
}