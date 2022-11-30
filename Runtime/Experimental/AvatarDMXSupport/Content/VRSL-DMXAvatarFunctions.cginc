SamplerState sampler_point_clamp;
Texture2D _DMXTexture;
//sampler2D _DMXTexture;

//CHANNELS - Groups: Intesnity, Red, Green, Blue
//Group 1: 0 - 3
//Group 2: 4 - 7
//Group 3: 8 - 11
//Group 4: 12 - 15
//Aux 1: 16 
//Aux 2: 17

float2 GetGroupUVs(uint group, uint channel)
{
    group = clamp(group, 1, 4);
    float2 uv = float2(0.0,0.5);
    uint g = (group - 1) * 3;
    uint ch = group + channel + g;
    uv.x = (ch/18.0) - 0.0260;
    
    return uv;
}
//function for getting the value on the OSC Grid in the bottom right corner configuration
float GetValueAtCoords(uint group, uint channel)
{
    float2 xyUV = GetGroupUVs(group, channel);
    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
   // float4 c = tex2Dlod(_DMXTexture, uvcoords);
    float4 c = _DMXTexture.SampleLevel(sampler_point_clamp, xyUV, 0);
    float3 cRGB = float3(c.r, c.g, c.b);
    float value = LinearRgbToLuminance(cRGB);
    value = LinearToGammaSpaceExact(value);
    return value;
}
float GetDMXIntensity(uint group)
{
    return GetValueAtCoords(group, 0);
}

float4 GetDMXColor(uint group)
{
    float red = GetValueAtCoords(group, 1);
    float green = GetValueAtCoords(group, 2);
    float blue = GetValueAtCoords(group, 3);
    return float4(red, green, blue, 1.0);
}

float4 GetDMXColorAndIntensity(uint group)
{
   //return lerp(fixed4(0,0,0,1), GetDMXColor(group), GetDMXIntensity(group));
   return GetDMXColor(group) * GetDMXIntensity(group);
}

