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
    public static string ver = "VR Stage Lighting ver:" + "<b><color=#6a15ce> 2.0</color></b>";
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
        style.fixedHeight = 150;
        //style.fixedWidth = 300;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(300f, 140f, style);
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
        style.fontSize = 14;
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
    GUIStyle l, I;
    GUIContent colorLabel;
  //  SerializedProperty _globalIntensity;
    public static GUIStyle SectionLabel()
    {
        GUIStyle g = new GUIStyle();
        g.fontSize = 15;
        g.fontStyle = FontStyle.Bold;
        g.normal.textColor = Color.white;
        return g;
    }
    public static GUIStyle InfoLabel()
    {
        GUIStyle g = new GUIStyle();
        g.fontSize = 13;
        g.fontStyle = FontStyle.Italic;
        g.normal.textColor = Color.white;
        return g;
    }

    public new void OnEnable() 
    {
        base.OnEnable();
        l = SectionLabel();
        I = InfoLabel();
        colorLabel = new GUIContent();
        colorLabel.text = "Emission Color";
      //  _globalIntensity = serializedObject.FindProperty("globalIntensity");
      EditorApplication.hierarchyChanged += HierarchyChanged;

    }
             
 
     void OnDisable( )
     {
         EditorApplication.hierarchyChanged -= HierarchyChanged;
     }
    private void HierarchyChanged()
     {
         VRStageLighting_DMX_Static fixture = (VRStageLighting_DMX_Static)target;
         UpdateSettings(fixture);
     }
     void UpdateSettings(VRStageLighting_DMX_Static fixture)
     {
        try{
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
        catch(NullReferenceException e)
        {
            e.ToString();
        }
     }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
        DrawLogo();
        ShurikenHeaderCentered(ver);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        VRStageLighting_DMX_Static fixture = (VRStageLighting_DMX_Static)target;
        //EditorGUIUtility.LookLikeInspector();
        EditorGUI.BeginChangeCheck();
        //base.OnInspectorGUI();

        //DMX SETTINGS SECTION
        GUILayout.Label("DMX Settings", l);
        serializedObject.FindProperty("enableDMXChannels").boolValue = EditorGUILayout.Toggle(new GUIContent("Enable DMX", 
        "The industry standard DMX Channel this fixture begins on. Most standard VRSL fixtures are 13 channels"), fixture.enableDMXChannels);
        serializedObject.FindProperty("useLegacySectorMode").boolValue  = EditorGUILayout.Toggle(new GUIContent("Enable Legacy Sector Mode", 
        "Enables the legacy 'Sector' based method of assigning DMX Channels. Keep this unchecked to use industry standard DMX Channels."), fixture.useLegacySectorMode);
        if(fixture.useLegacySectorMode)
        {
            serializedObject.FindProperty("sector").intValue = EditorGUILayout.IntField(new GUIContent("Sector", 
            "Chooses the DMX Address to start this fixture at. A Sector in this context is every 13 Channels. I.E Sector 0 is channels 1-13, Sector 1 is channels 14-26, etc."),fixture.sector);
            serializedObject.FindProperty("singleChannelMode").boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Single Channel Mode",
            "Enables single channel DMX mode for this fixture. This is for single channeled fixtures instead of the standard 13-channeled ones. Currently, the 'Flasher' fixture is the only single-channeled fixture at the moment"), fixture.singleChannelMode);
            if(fixture.singleChannelMode)
            {
                serializedObject.FindProperty("Channel").intValue = EditorGUILayout.IntSlider(new GUIContent("Single Channel CH",
                "Chooses the which of the 13 Channels of the current sector to sample from when single channel mode is enabled. Do not worry about this value if you are not using a single-channeled fixture."),fixture.Channel, 0, 12);
                
            }
        }
        else
        {
            serializedObject.FindProperty("dmxChannel").intValue = EditorGUILayout.IntSlider(new GUIContent("DMX Channel", 
            "The industry standard DMX Channel this fixture begins on. Most standard VRSL fixtures are 13 channels"),fixture.dmxChannel, 1, 512);
            serializedObject.FindProperty("dmxUniverse").intValue = EditorGUILayout.IntSlider(new GUIContent("Universe", 
            "The industry standard Artnet Universe. Use this to choose which universe to read the DMX Channel from."),fixture.dmxUniverse, 1, 3);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label(fixture._DMXChannelToString(), I);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        serializedObject.FindProperty("legacyGoboRange").boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Legacy Gobo Range", 
        "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."), fixture.legacyGoboRange);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //GENERAL SETTINGS
        GUILayout.Label("General Settings", l);
        serializedObject.FindProperty("globalIntensity").floatValue = EditorGUILayout.Slider(new GUIContent("Global Intensity",
        "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."), fixture.globalIntensity, 0.0f, 1.0f);
        serializedObject.FindProperty("finalIntensity").floatValue  = EditorGUILayout.Slider(new GUIContent("Final Intensity",
        "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensity, 0.0f, 1.0f);
        serializedObject.FindProperty("lightColorTint").colorValue = EditorGUILayout.ColorField(colorLabel, fixture.lightColorTint, true, true, true);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //MOVEMENT SETTINGS
        GUILayout.Label("Movement Settings", l);
        serializedObject.FindProperty("invertPan").boolValue = EditorGUILayout.Toggle(new GUIContent("Invert Pan", 
        "Invert the tilt values (Up/Down Movement) for movers."), fixture.invertPan);
        serializedObject.FindProperty("invertTilt").boolValue = EditorGUILayout.Toggle(new GUIContent("Invert Tilt", 
        "Enable this if the mover is hanging upside down."), fixture.invertTilt);
        serializedObject.FindProperty("isUpsideDown").boolValue = EditorGUILayout.Toggle(new GUIContent("Is Upside Down?",
        "Enable projection spinning (Udon Override Only)."), fixture.isUpsideDown);
        serializedObject.FindProperty("maxMinPan").floatValue = EditorGUILayout.FloatField(new GUIContent("Max/Min Pan Range",
        "Control the range of rotation for the pan channel of the fixture"), fixture.maxMinPan);
        serializedObject.FindProperty("maxMinTilt").floatValue = EditorGUILayout.FloatField(new GUIContent("Max/Min Tilt Range",
        "Control the range of rotation for the tilt channel of the fixture"), fixture.maxMinTilt);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //FIXTURE SETTINGS
        GUILayout.Label("Fixture Settings", l);
        serializedObject.FindProperty("enableAutoSpin").boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Projection Spin",
        "Enable projection spinning (Udon Override Only)."), fixture.enableAutoSpin);
        serializedObject.FindProperty("enableStrobe").boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Strobe Functionality",
        "Enable strobe effects (via DMX Only)."), fixture.enableStrobe);
        serializedObject.FindProperty("tiltOffsetBlue").floatValue = EditorGUILayout.Slider(new GUIContent("Tilt Offset",
        "Tilt (Up/Down) offset/movement. Directly controls tilt when in Udon Mode; is an offset when in DMX mode."), fixture.tiltOffsetBlue, 0.0f, 360.0f);
        serializedObject.FindProperty("panOffsetBlueGreen").floatValue = EditorGUILayout.Slider(new GUIContent("Pan Offset",
        "Pan (Left/Right) offset/movement. Directly controls pan when in Udon Mode; is an offset when in DMX mode."), fixture.panOffsetBlueGreen, 0.0f, 360.0f);
        serializedObject.FindProperty("selectGOBO").intValue = EditorGUILayout.IntSlider(new GUIContent("Projection GOBO Selection",
        "The meshes used to make up the light. You need atleast 1 mesh in this group for the script to work properly."), fixture.selectGOBO, 1, 8);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //MESH SETTINGS
        GUILayout.Label("Mesh Settings", l);
        serializedObject.FindProperty("coneWidth").floatValue = EditorGUILayout.Slider(new GUIContent("Fixture Cone Width",
        "Controls the radius of a mover/spot light."), fixture.coneWidth, 0.0f, 5.5f);
        serializedObject.FindProperty("coneLength").floatValue = EditorGUILayout.Slider(new GUIContent("Fixture Cone Length",
        "Controls the length of the cone of a mover/spot light."), fixture.coneLength, 0.5f, 10.0f);
        serializedObject.FindProperty("maxConeLength").floatValue = EditorGUILayout.Slider("Max Cone Length", fixture.maxConeLength, 0.275f, 10.0f);
        
        SerializedProperty meshRends = serializedObject.FindProperty("objRenderers");
        EditorGUILayout.PropertyField(meshRends, true);
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            UpdateSettings(fixture);
        //EditorGUIUtility.LookLikeControls();
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

// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_RAW_Static))]
// public class VRStageLighting_RAW_Static_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
        
//         //EditorGUILayout.Space();
//         VRStageLighting_RAW_Static fixture = (VRStageLighting_RAW_Static)target;
//         EditorGUI.BeginChangeCheck();
//         base.OnInspectorGUI();
//         if(EditorGUI.EndChangeCheck())
//         {
//             if(fixture.objRenderers.Length > 0)
//             {
//                 bool isEmpty = false;
//                 foreach(MeshRenderer rend in fixture.objRenderers)
//                 {
//                     if(rend == null)
//                     {
//                         isEmpty = true;
//                         break;
//                     }
//                 }
//                 if(!isEmpty)
//                 {
//                     fixture._SetProps();
//                     fixture._UpdateInstancedProperties();
//                 }
//             }
//         } 
//     }
// }
// #endif

// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_RAW_Laser))]
// public class VRStageLighting_RAW_Laser_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
        
//         //EditorGUILayout.Space();
//         VRStageLighting_RAW_Laser fixture = (VRStageLighting_RAW_Laser)target;
//         EditorGUI.BeginChangeCheck();
//         base.OnInspectorGUI();
//         if(EditorGUI.EndChangeCheck())
//         {
//             if(fixture.objRenderers.Length > 0)
//             {
//                 bool isEmpty = false;
//                 foreach(MeshRenderer rend in fixture.objRenderers)
//                 {
//                     if(rend == null)
//                     {
//                         isEmpty = true;
//                         break;
//                     }
//                 }
//                 if(!isEmpty)
//                 {
//                     fixture._SetProps();
//                     fixture._UpdateInstancedProperties();
//                 }
//             }
//         } 
//     }
// }
// #endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(VRStageLighting_AudioLink_Laser))]
public class VRStageLighting_AudioLink_Laser_Editor : VRSL_UdonEditor
{


     new void OnEnable( )
     {
         base.OnEnable();
         EditorApplication.hierarchyChanged += HierarchyChanged;
     }
 
     void OnDisable( )
     {
         EditorApplication.hierarchyChanged -= HierarchyChanged;
     }

     private void HierarchyChanged( )
     {
         VRStageLighting_AudioLink_Laser fixture = (VRStageLighting_AudioLink_Laser)target;
         UpdateSettings(fixture);
     }



     void UpdateSettings(VRStageLighting_AudioLink_Laser fixture)
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
            UpdateSettings(fixture);
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

    new void OnEnable( )
     {
         base.OnEnable();
         EditorApplication.hierarchyChanged += HierarchyChanged;
     }
 
     void OnDisable( )
     {
         EditorApplication.hierarchyChanged -= HierarchyChanged;
     }

     void UpdateSettings(VRStageLighting_AudioLink_Static fixture)
     {
        try{
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
                        fixture._CheckConstraints(fixture);
                    }
                }
            }
        }
        catch(NullReferenceException e)
        {
            e.ToString();
        }
     }
 
     private void HierarchyChanged( )
     {
         VRStageLighting_AudioLink_Static fixture = (VRStageLighting_AudioLink_Static)target;
         UpdateSettings(fixture);
     }
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
            UpdateSettings(fixture);
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

// #if !COMPILER_UDONSHARP && UNITY_EDITOR
// [CustomEditor(typeof(VRStageLighting_Animated_Static))]
// public class VRStageLighting_Animated_Static_Editor : VRSL_UdonEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
//         DrawLogo();
//         ShurikenHeaderCentered(ver);
//         EditorGUILayout.Space();
//         EditorGUILayout.Space();
//         //EditorGUILayout.Space();
//         VRStageLighting_Animated_Static fixture = (VRStageLighting_Animated_Static)target;
//         EditorGUI.BeginChangeCheck();
//         base.OnInspectorGUI();
//         if(EditorGUI.EndChangeCheck())
//         {
//             if(fixture.objRenderers.Length > 0)
//             {
//                 bool isEmpty = false;
//                 foreach(MeshRenderer rend in fixture.objRenderers)
//                 {
//                     if(rend == null)
//                     {
//                         isEmpty = true;
//                         break;
//                     }
//                 }
//                 if(!isEmpty)
//                 {
//                     fixture._SetProps();
//                     if(Application.isPlaying)
//                     {
//                         fixture._UpdateInstancedProperties();
//                     }
//                     else
//                     {
//                         fixture._UpdateInstancedPropertiesSansAudioLink();
//                     }
//                 }
//             }
//         }
        
//     }
// }
// #endif

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
//        Debug.Log(state);
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
            e.GetType();
            return;
        }
        try
        {  
            foreach(GameObject obj in objs)
            {
                //VRStageLighting_RAW_Static[] staticLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_RAW_Static>();
                VRStageLighting_AudioLink_Static[] audioLinkLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_AudioLink_Static>();
               // VRStageLighting_Animated_Static[] animatedLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_Animated_Static>();
                VRStageLighting_DMX_Static[] dmxLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_DMX_Static>();
                //VRStageLighting_RAW_Laser[] rawLasers = obj.GetUdonSharpComponentsInChildren<VRStageLighting_RAW_Laser>();
                VRStageLighting_AudioLink_Laser[] audioLinkLasers = obj.GetUdonSharpComponentsInChildren<VRStageLighting_AudioLink_Laser>();
            // VRStageLighting_DMX_Static[] dmxLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_DMX_Static>();
                // if(staticLights != null)
                // {
                //         foreach(VRStageLighting_RAW_Static fixture in staticLights)
                //         {
                //             if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                //             {
                //                 fixture._SetProps();
                //                 fixture._UpdateInstancedProperties();
                //             }
                //         }
                // }
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
                // if(rawLasers != null)
                // {
                //     foreach(VRStageLighting_RAW_Laser fixture in rawLasers)
                //     {
                //         if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                //         {
                //             fixture._SetProps();
                //             fixture._UpdateInstancedProperties();
                //         }
                //     }
                // }
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
                // if(animatedLights != null)
                // {
                //     foreach(VRStageLighting_Animated_Static fixture in animatedLights)
                //     {
                //         if(fixture.objRenderers.Length > 0 && fixture.objRenderers[0] != null)
                //         {
                //             fixture._SetProps();
                //             if(Application.isPlaying)
                //             {
                //                 fixture._UpdateInstancedProperties();
                //             }
                //             else
                //             {
                //                 fixture._UpdateInstancedPropertiesSansAudioLink();
                //             }
                //         }
                //     }
                // }
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
            e.GetType();
            return;
        }
    }
}
#endif