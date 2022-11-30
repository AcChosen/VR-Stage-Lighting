Shader "VRSL/DMX CRTs/Experimental/Global Avatar Texture"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE STROBE
    Properties
    {
        [NoScaleOffset]_DMXGridRenderTexture("DMX Grid Render Texture", 2D) = "white" {}
        _Test("Test", Range(0, 40)) = 0
        _TestY("TestY", Range(-0.01, 0.01)) = 0.001
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Pass
        {
            Name "Pass1DMXGrab"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 4.5 

            Texture2D _DMXGridRenderTexture;
            uniform float4 _DMXGridRenderTexture_TexelSize;
            SamplerState sampler_point_repeat;
            half _Test, _TestY;

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float2 IndustryRead(int x, int y)
            {
                
                float resMultiplierX = (_DMXGridRenderTexture_TexelSize.z / 13);
                float2 xyUV = float2(0.0,0.0);
                
                xyUV.x = ((x * resMultiplierX) * _DMXGridRenderTexture_TexelSize.x);
                xyUV.y = (y * resMultiplierX) * _DMXGridRenderTexture_TexelSize.y;
                xyUV.y -= 0.00105;
                xyUV.x -= 0.015;
            // xyUV.x = DMXChannel == 15 ? xyUV.x + 0.0769 : xyUV.x;
                return xyUV;
            }




            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                //CHILL FOR 1 SECOND TO ALLOW DATA TO COME IN
                uint currentPixel = floor(IN.localTexcoord.x * 18);
                //uint DMXChannel = (494+1024+17) + currentPixel;
                uint DMXChannel = 1535 + currentPixel;
                uint x = DMXChannel % 13; // starts at 1 ends at 13
                //x = x == 0.0 ? 13.0 : x;
                //x = currentPixel == 12 ? _Test : x;
                float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
                //y = frac(y)== 0.00000 ? y - 1 : y;
                y = currentPixel == 12 ? y - 1 : y;


                float4 currentFrame = _DMXGridRenderTexture.SampleLevel(sampler_point_repeat, IndustryRead(x,y+1), 0);
                return currentFrame;
            }
            ENDCG
         }
    }
}
