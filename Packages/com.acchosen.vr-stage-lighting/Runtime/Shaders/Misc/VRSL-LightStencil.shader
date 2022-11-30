Shader "VRSL/Stencils/VRSL-Light Stencil"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _Comp ("Comp", Float) = 2
		[Enum(UnityEngine.Rendering.StencilOp)] _Pass ("Pass Back", Float) = 1
        [Enum(UnityEngine.Rendering.StencilOp)] _PassF ("Pass Front", Float) = 1
		[Enum(UnityEngine.Rendering.StencilOp)] _Fail ("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _ZFail ("Zfail", Float) = 0
    }
    SubShader
    {
        Zwrite Off
        Tags { "RenderType"="Transparent"  "Queue" = "Geometry-1000"}
        LOD 100
        
        ColorMask 0


        // Pass
        // {
        //     Cull Back
        //     Stencil
        //     {
        //          Ref 142
        //          Comp [_Comp]
        //          Pass [_Pass]
        //          Zfail[_ZFail]
        //     }  
        //     ColorMask 0
        // }
        Pass
        {
            Cull Front
            Stencil
            {
                 Ref 142
                 Comp [_Comp]
                 Pass [_PassF]
                 Zfail[_ZFail]
            }  
            ColorMask 0
        }
    }
    
}
