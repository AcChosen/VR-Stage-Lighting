
float2 LegacyRead(int channel, int sector)
{
    // say we were on sector 6
    // we need to move over 2 sectors
    // and we need to move up 3 sectors

    //1 sector is every 13 channels
        float x = 0.02000;
        float y = 0.02000;
        //TRAVERSING THE Y AXIS OF THE OSC GRID
        float ymod = floor(sector / 2.0);       

        //TRAVERSING THE X AXIS OF THE OSC GRID
        float xmod = sector % 2.0;

        x+= (xmod * 0.50);
        y+= (ymod * 0.04);

        x+= (channel * 0.04);
        //we are now on the correct
        return float2(x,y);

}

float2 IndustryRead(int x, int y, uint DMXChannel)
{
    
    float resMultiplierX = (_OSCGridRenderTextureRAW_TexelSize.z / 13);
    float2 xyUV = float2(0.0,0.0);
    
    xyUV.x = ((x * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.x);
    xyUV.y = (y * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.y;
    xyUV.y -= 0.001;
    xyUV.x -= 0.015;
   // xyUV.x = DMXChannel == 15 ? xyUV.x + 0.0769 : xyUV.x;
    return xyUV;
}

//function for getting the value on the OSC Grid
//Returns a value from 0 to 1
float ReadDMX(uint DMXChannel, sampler2D _Tex)
{
    //DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
    uint x = DMXChannel % 13; // starts at 1 ends at 13
    x = x == 0.0 ? 13.0 : x;
    float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
    y = frac(y)== 0.00000 ? y - 1 : y;

    float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0), DMXChannel);
        
    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    float3 cRGB = float3(c.r, c.g, c.b);
    float value = LinearRgbToLuminance(cRGB);
    value = LinearToGammaSpaceExact(value);
    return value;
}
//function for getting the value on the OSC Grid
//Returns a value from 0 to 1
float ReadDMXRaw(uint DMXChannel, sampler2D _Tex)
{
   // DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
    uint x = DMXChannel % 13; // starts at 1 ends at 13
    x = x == 0.0 ? 13.0 : x;
    float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
    y = frac(y)== 0.00000 ? y - 1 : y;
    if(x == 13.0) //for the 13th channel of each sector... Go down a sector for these DMX Channel Ranges...
    {
    
        //I don't know why, but we need this for some reason otherwise the 13th channel gets shifted around improperly.
        //I"m not sure how to express these exception ranges mathematically. Doing so would be much more cleaner though.
        y = DMXChannel >= 90 && DMXChannel <= 404 ? y - 1 : y;
        y = DMXChannel >= 676 && DMXChannel <= 819 ? y - 1 : y;
        y = DMXChannel >= 1339 ? y - 1 : y;
    }

    float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0), DMXChannel);

    float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
    float4 c = tex2Dlod(_Tex, uvcoords);
    
	return c.r;
}

float GetStrobeOutput(uint DMXChannel)
{
    
    float phase = ReadDMXRaw(DMXChannel + 6, _OSCGridStrobeTimer);
    float status = ReadDMX(DMXChannel + 6, _OSCGridRenderTextureRAW);

    half strobe = (sin(phase));//Get sin wave
    strobe = IF(strobe > 0.0, 1.0, 0.0);//turn to square wave
    //strobe = saturate(strobe);

    strobe = IF(status > 0.2, strobe, 1); //minimum channel threshold set
    
    //check if we should even be strobing at all.
    strobe = IF(isOSC() == 1, strobe, 1);
    strobe = IF(isStrobe() == 1, strobe, 1);
    
    return strobe;

}