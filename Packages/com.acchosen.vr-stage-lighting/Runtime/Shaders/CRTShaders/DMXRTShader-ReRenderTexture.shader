Shader "VRSL/DMX CRTs/Re-Render RT"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE STROBE
    Properties
    {
        [NoScaleOffset]_Input("", 2D) = "white" {}
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 4.5

            #define VRSL_DMX

            sampler2D _Input;
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                return tex2D(_Input, IN.localTexcoord.xy);
            }
            ENDCG
         }
    }
}
