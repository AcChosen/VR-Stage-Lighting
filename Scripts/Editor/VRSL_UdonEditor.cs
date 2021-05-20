using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using System.Collections.Immutable;
#endif
#if !COMPILER_UDONSHARP && UNITY_EDITOR
public class VRSL_UdonEditor : Editor
{
    public static Texture logo;
    public static string ver = "VR Stage Lighting ver:" + "<color=#9b34ebff> 1.0</color>";
    public void OnEnable() 
    {
        logo = Resources.Load("VRStageLighting-Logo") as Texture;
    }
    public static void DrawLogo()
    {
        ///GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
        // GUILayout.FlexibleSpace();
        //GUI.DrawTexture(pos,logo,ScaleMode.ScaleToFit);
        //EditorGUI.DrawPreviewTexture(new Rect(0,0,400,150), logo);
        Vector2 contentOffset = new Vector2(0f, -2f);
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fixedHeight = 200;
        //style.fixedWidth = 300;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(300f, 190f, style);
        //GUILayout.Label(logo,style, GUILayout.MaxWidth(500), GUILayout.MaxHeight(200));
        GUI.Box(rect, logo,style);
        //GUILayout.Label(logo);
        // GUILayout.FlexibleSpace();
        //GUILayout.EndArea();
    }
    private static Rect DrawShurikenCenteredTitle(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);

        GUI.Box(rect, title, style);
        return rect;
    }
    public static void ShurikenHeaderCentered(string title)
    {
        DrawShurikenCenteredTitle(title, new Vector2(0f, -2f), 22);
    }
}
#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_DMX))]
public class VRStageLighting_DMX_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        VRStageLighting_DMX dmxTarget = (VRStageLighting_DMX) target;
        if (GUILayout.Button(new GUIContent("Update Instanced Properties", "Updates all the settings of this fixture to their respective shaders during Runtime"))) { dmxTarget._UpdateInstancedProperties(); }
        //EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}
#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_RAW))]
public class VRStageLighting_RAW_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}
#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_AudioLink))]
public class VRStageLighting_AudioLink_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}
#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_Animated))]
public class VRStageLighting_Animated_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}
#endif