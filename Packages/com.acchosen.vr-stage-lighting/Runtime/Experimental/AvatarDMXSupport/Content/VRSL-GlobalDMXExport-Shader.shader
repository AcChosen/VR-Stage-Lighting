Shader "VRSL/DMX CRTs/Experimental/Global Avatar Export"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" "DMXExport"="DMXExport" }
        Pass
        {
            Tags { "LightMode"="Vertex" }
            ColorMask 0
            ZTest Off
        }
        GrabPass
        {
            Tags { "LightMode"="Vertex" }
            "_DMXTexture"
        }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            ColorMask 0
            ZTest Off
        }
    }
}