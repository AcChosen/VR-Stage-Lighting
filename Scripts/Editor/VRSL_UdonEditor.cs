using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System.Threading;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
using UnityEngine.SceneManagement;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using System.Collections.Immutable;
using System;
#endif
#if !COMPILER_UDONSHARP && UNITY_EDITOR
public class VRSL_UdonEditor : Editor
{
    public static Texture logo;
    public static string ver = "VR Stage Lighting ver:" + "<color=#9b34ebff> 1.20</color>";
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

// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_DMX))]
// public class VRStageLighting_DMX_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
//         VRStageLighting_DMX fixture = (VRStageLighting_DMX) target;
//         //EditorGUILayout.Space();
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
//         base.OnInspectorGUI();
//         if(GUI.changed && Application.isPlaying)
//         {
//             fixture._UpdateInstancedProperties();
//         }
//     }
// }
// #endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_DMX_Static))]
public class VRStageLighting_DMX_Static_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        VRStageLighting_DMX_Static fixture = (VRStageLighting_DMX_Static)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(fixture.objRenderers.Length > 0)
        {
            bool isEmpty = false;
            foreach(MeshRenderer rend in fixture.objRenderers)
            {
                if(rend == null)
                {
                    isEmpty = true;
                    break;
                }
            }
            if(!isEmpty)
            {
                fixture._SetProps();
                if(Application.isPlaying)
                {
                    fixture._UpdateInstancedProperties();
                }
                else
                {
                    fixture._UpdateInstancedPropertiesSansDMX();
                }
            }
        } 
    }
}
#endif

// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_RAW))]
// public class VRStageLighting_RAW_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
        
//         //EditorGUILayout.Space();
//         base.OnInspectorGUI();
//         VRStageLighting_RAW fixture = (VRStageLighting_RAW)target;
//         if(GUI.changed && Application.isPlaying)
//         {
//             fixture._UpdateInstancedProperties();
//         }
//     }
// }
// #endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_RAW_Static))]
public class VRStageLighting_RAW_Static_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //EditorGUILayout.Space();
        VRStageLighting_RAW_Static fixture = (VRStageLighting_RAW_Static)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(EditorGUI.EndChangeCheck())
        {
            if(fixture.objRenderers.Length > 0)
            {
                bool isEmpty = false;
                foreach(MeshRenderer rend in fixture.objRenderers)
                {
                    if(rend == null)
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if(!isEmpty)
                {
                    fixture._SetProps();
                    fixture._UpdateInstancedProperties();
                }
            }
        } 
    }
}
#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_RAW_Laser))]
public class VRStageLighting_RAW_Laser_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //EditorGUILayout.Space();
        VRStageLighting_RAW_Laser fixture = (VRStageLighting_RAW_Laser)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(EditorGUI.EndChangeCheck())
        {
            if(fixture.objRenderers.Length > 0)
            {
                bool isEmpty = false;
                foreach(MeshRenderer rend in fixture.objRenderers)
                {
                    if(rend == null)
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if(!isEmpty)
                {
                    fixture._SetProps();
                    fixture._UpdateInstancedProperties();
                }
            }
        } 
    }
}
#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_AudioLink_Laser))]
public class VRStageLighting_AudioLink_Laser_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //EditorGUILayout.Space();
        VRStageLighting_AudioLink_Laser fixture = (VRStageLighting_AudioLink_Laser)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(EditorGUI.EndChangeCheck())
        {
            if(fixture.objRenderers.Length > 0)
            {
                bool isEmpty = false;
                foreach(MeshRenderer rend in fixture.objRenderers)
                {
                    if(rend == null)
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if(!isEmpty)
                {
                    fixture._SetProps();
                    if(Application.isPlaying)
                    {
                        fixture._UpdateInstancedProperties();
                    }
                    else
                    {
                        fixture._UpdateInstancedPropertiesSansAudioLink();
                    }
                }
            }
        }
    }
}
#endif

// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_AudioLink))]
// public class VRStageLighting_AudioLink_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
//         base.OnInspectorGUI();
//         VRStageLighting_AudioLink fixture = (VRStageLighting_AudioLink)target;
//         if(GUI.changed && Application.isPlaying)
//         {
//             fixture._UpdateInstancedProperties();
//         }

//         // serializedObject.Update();
//         // if(previousColor != lightColor.colorValue)
//         // {
//         //     fixture.LightColorTint = fixture.LightColorTint;
//         // }
//         // serializedObject.ApplyModifiedProperties();
//     }
// }
// #endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[InitializeOnLoad]
[CustomEditor(typeof(VRStageLighting_AudioLink_Static))]

public class VRStageLighting_AudioLink_Static_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        VRStageLighting_AudioLink_Static fixture = (VRStageLighting_AudioLink_Static)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(EditorGUI.EndChangeCheck())
        {
            if(fixture.objRenderers.Length > 0)
            {
                bool isEmpty = false;
                foreach(MeshRenderer rend in fixture.objRenderers)
                {
                    if(rend == null)
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if(!isEmpty)
                {
                    fixture._SetProps();
                    if(Application.isPlaying)
                    {
                        fixture._UpdateInstancedProperties();
                    }
                    else
                    {
                        fixture._UpdateInstancedPropertiesSansAudioLink();
                    }
                }
            }
        }

        //serializedObject.Update();
        // if(previousColor != lightColor.colorValue)
        // {
        //     fixture.LightColorTint = fixture.LightColorTint;
        // }
        // serializedObject.ApplyModifiedProperties();
    }
}
#endif


// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_Animated))]
// public class VRStageLighting_Animated_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
//         //EditorGUILayout.Space();
//         base.OnInspectorGUI();
//         VRStageLighting_Animated fixture = (VRStageLighting_Animated)target;
//         if(GUI.changed && Application.isPlaying)
//         {
//             fixture._UpdateInstancedProperties();
//         }
//     }
// }
// #endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_Animated_Static))]
public class VRStageLighting_Animated_Static_Editor : VRSL_UdonEditor
{
    public override void OnInspectorGUI()
    {
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        VRStageLighting_Animated_Static fixture = (VRStageLighting_Animated_Static)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(EditorGUI.EndChangeCheck())
        {
            if(fixture.objRenderers.Length > 0)
            {
                bool isEmpty = false;
                foreach(MeshRenderer rend in fixture.objRenderers)
                {
                    if(rend == null)
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if(!isEmpty)
                {
                    fixture._SetProps();
                    if(Application.isPlaying)
                    {
                        fixture._UpdateInstancedProperties();
                    }
                    else
                    {
                        fixture._UpdateInstancedPropertiesSansAudioLink();
                    }
                }
            }
        }
        
    }
}
#endif

#if UNITY_EDITOR && !COMPILER_UDONSHARP
// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class PlayModeStateChanged
{
    // register an event handler when the class is initialized
    static PlayModeStateChanged()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnEditorSceneManagerSceneOpened;
        EditorApplication.update += RunOnce;
        //LoadFixtureSettings();
    }

    static void RunOnce()
     {
        LoadFixtureSettings();
      //  Debug.Log("Running Once... " + EditorApplication.update);
        EditorApplication.update -= RunOnce;
     }

    static void OnEditorSceneManagerSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        //Debug.LogFormat("SceneOpened: {0}", scene.name);
        LoadFixtureSettings();
    }

    // public static void DelayedFixtureLoad()
    // {    
    //     //await Task.Delay(1000);
    //     Thread.Sleep(10000);
    //     LoadFixtureSettings();
    //     Debug.Log("Finsihed Loading VRSL!");
    // }
    private static void LogPlayModeState(PlayModeStateChange state)
    {
        Debug.Log(state);
        if(state == PlayModeStateChange.EnteredEditMode)
        {
            LoadFixtureSettings();
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void LoadFixtureSettings()
    {
        GameObject[] objs;
        try
        {
            Scene scene = SceneManager.GetActiveScene();
            objs = scene.GetRootGameObjects();
        }
        catch(NullReferenceException e)
        {
            return;
        }
        try
        {  
            foreach(GameObject obj in objs)
            {
                VRStageLighting_RAW_Static[] staticLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_RAW_Static>();
                VRStageLighting_AudioLink_Static[] audioLinkLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_AudioLink_Static>();
                VRStageLighting_Animated_Static[] animatedLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_Animated_Static>();
                VRStageLighting_DMX_Static[] dmxLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_DMX_Static>();
                VRStageLighting_RAW_Laser[] rawLasers = obj.GetUdonSharpComponentsInChildren<VRStageLighting_RAW_Laser>();
                VRStageLighting_AudioLink_Laser[] audioLinkLasers = obj.GetUdonSharpComponentsInChildren<VRStageLighting_AudioLink_Laser>();
            // VRStageLighting_DMX_Static[] dmxLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_DMX_Static>();
                if(staticLights != null)
                {
                        foreach(VRStageLighting_RAW_Static fixture in staticLights)
                        {
                            if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                            {
                                fixture._SetProps();
                                fixture._UpdateInstancedProperties();
                            }
                        }
                }
                if(dmxLights != null)
                {
                    foreach(VRStageLighting_DMX_Static fixture in dmxLights)
                    {
                        fixture._SetProps();
                        if(Application.isPlaying)
                        {
                            fixture._UpdateInstancedProperties();
                        }
                        else
                        {
                            fixture._UpdateInstancedPropertiesSansDMX();
                        }
                    }
                }
                if(rawLasers != null)
                {
                    foreach(VRStageLighting_RAW_Laser fixture in rawLasers)
                    {
                        if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                        {
                            fixture._SetProps();
                            fixture._UpdateInstancedProperties();
                        }
                    }
                }
                if(audioLinkLasers != null)
                {
                    foreach(VRStageLighting_AudioLink_Laser fixture in audioLinkLasers)
                    {
                        if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                        {
                            fixture._SetProps();
                            if(Application.isPlaying)
                            {
                                fixture._UpdateInstancedProperties();
                            }
                            else
                            {
                                fixture._UpdateInstancedPropertiesSansAudioLink();
                            }
                        }
                    }
                }
                if(audioLinkLights != null)
                {
                    foreach(VRStageLighting_AudioLink_Static fixture in audioLinkLights)
                    {
                        if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                        {
                            fixture._SetProps();
                            if(Application.isPlaying)
                            {
                                fixture._UpdateInstancedProperties();
                            }
                            else
                            {
                                fixture._UpdateInstancedPropertiesSansAudioLink();
                            }

                        }
                    }
                }
                if(animatedLights != null)
                {
                    foreach(VRStageLighting_Animated_Static fixture in animatedLights)
                    {
                        if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                        {
                            fixture._SetProps();
                            if(Application.isPlaying)
                            {
                                fixture._UpdateInstancedProperties();
                            }
                            else
                            {
                                fixture._UpdateInstancedPropertiesSansAudioLink();
                            }
                        }
                    }
                }
                // foreach(VRStageLighting_DMX_Static fixture in dmxLights)
                // {
                //     if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                //     {
                //         fixture._SetProps();
                //         fixture._UpdateInstancedProperties();
                //     }
                // }
            }
        }
        catch(NullReferenceException e)
        {
            return;
        }
    }
}
#endif