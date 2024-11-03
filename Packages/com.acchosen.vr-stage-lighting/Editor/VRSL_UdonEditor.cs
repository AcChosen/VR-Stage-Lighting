﻿using UnityEngine;
using UnityEngine.UI;
using System.Threading;

#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDKBase.Midi;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UdonSharpEditor;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
#endif

#endif

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Immutable;
using System;
using System.IO;
using System.Collections.Generic;
#endif

namespace VRSL.EditorScripts
{
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CanEditMultipleObjects]
    public class VRSL_UdonEditor : Editor
    {
        public static Texture logo;
       // public static string ver = "VR Stage Lighting ver:" + " <b><color=#6a15ce> 2.4</color></b>";
        public void OnEnable() 
        {
            logo = Resources.Load("VRStageLighting-Logo") as Texture;
        }
        public static string GetVersion()
        {
            string path = Application.dataPath;
            path = path.Replace("Assets","");
            path += "Packages"  + "\\" + "com.acchosen.vr-stage-lighting" + "\\";
            path += "Runtime" + "\\"  + "VERSION.txt";

            StreamReader reader = new StreamReader(path); 
            string versionNum = reader.ReadToEnd();
            string ver = "VR Stage Lighting ver:" + " <b><color=#b33cff>" + versionNum + "</color></b>";
            return ver;
        }
        public static void DrawLogo()
        {
            Vector2 contentOffset = new Vector2(0f, -2f);
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fixedHeight = 150;
            //style.fixedWidth = 300;
            style.contentOffset = contentOffset;
            style.alignment = TextAnchor.MiddleCenter;
            var rect = GUILayoutUtility.GetRect(300f, 140f, style);
            GUI.Box(rect, logo,style);
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


    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(VRStageLighting_DMX_Static))]
    [CanEditMultipleObjects]
    public class VRStageLighting_DMX_Static_Editor : VRSL_UdonEditor
    {
        GUIStyle l, I;
        GUIContent colorLabel;
        VRSL_LocalUIControlPanel panel;
        string[] fixDefinitionNames = new string[1];
    //  SerializedProperty _globalIntensity;
        public static GUIStyle InfoLabel()
        {
            GUIStyle g = new GUIStyle();
            g.fontSize = 13;
            g.fontStyle = FontStyle.Italic;
            g.normal.textColor = Color.white;
            return g;
        }

        public static GUIStyle SectionLabel()
        {
            GUIStyle g = new GUIStyle();
            g.fontSize = 15;
            g.fontStyle = FontStyle.Bold;
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
        SceneView.duringSceneGui += this.OnSceneGUI;
        GetPanel();
        }
        void OnSceneGUI(SceneView sceneView)
        {

        }
        string[] GetFixtureOptions(string fixtureDefGUID)
        {
          VRSL_FixtureDefinitions fixDefAsset = (VRSL_FixtureDefinitions) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(fixtureDefGUID), typeof(VRSL_FixtureDefinitions));
          return fixDefAsset.GetNames();
        }
        public void GetPanel()
        {
            List<GameObject> sceneObjects = GetAllObjectsOnlyInScene();
            foreach(GameObject go in sceneObjects)
            {
                panel = go.GetComponent<VRSL_LocalUIControlPanel>();
                if(panel != null)
                {
                    fixDefinitionNames = GetFixtureOptions(panel.fixtureDefGUID);
                    break;
                }
            }
        }

        static List<GameObject> GetAllObjectsOnlyInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                    objectsInScene.Add(go);
            }
            return objectsInScene;
        }            
    
        void OnDisable( )
        {
            EditorApplication.hierarchyChanged -= HierarchyChanged;
            SceneView.duringSceneGui -= this.OnSceneGUI;
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
#if UDONSHARP
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
#endif
            DrawLogo();
            ShurikenHeaderCentered(GetVersion());
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
            if(serializedObject.FindProperty("enableDMXChannels").boolValue && panel != null)
            {
                EditorGUI.indentLevel++;
                serializedObject.FindProperty("fixtureDefintion").intValue = EditorGUILayout.Popup("Fixture Type",serializedObject.FindProperty("fixtureDefintion").intValue, fixDefinitionNames);
                EditorGUI.indentLevel--;
            }

            serializedObject.FindProperty("nineUniverseMode").boolValue = EditorGUILayout.Toggle(new GUIContent("Extended Universe Mode", 
            "Enables 9-Universe mode for this fixture. The grid will be split up by RGB channels with each section and color representing a universe." + 
            " Only availble on the Vertical and Horizontal Grid nodes."), fixture.nineUniverseMode);
            
            serializedObject.FindProperty("fixtureID").intValue = EditorGUILayout.IntField(new GUIContent("Fixture ID", 
            "The ID number for this fixture. This is mostly for organizational purposes and is entirely optional. Most DMX software have an ID attached to each fixture to run the fixtures through commands more easily, and it is recommended to have those IDs lined up here as well for the sake simplicity. This ID is public and can also be used for Udon scripting as well."),fixture.fixtureID);

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
                "The industry standard Artnet Universe. Use this to choose which universe to read the DMX Channel from."),fixture.dmxUniverse, 1, 9);
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
            EditorGUILayout.PropertyField( serializedObject.FindProperty("finalIntensityComponentMode"), new GUIContent("Control Component Intensities"));
            EditorGUI.indentLevel++;
            if(serializedObject.FindProperty("finalIntensityComponentMode").boolValue){

                serializedObject.FindProperty("finalIntensityVolumetric").floatValue  = EditorGUILayout.Slider(new GUIContent("Volumetric Intensity",
                "Sets the maximum brightness value of Global Intensity for volumetric meshes only. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensityVolumetric, 0.0f, 1.0f);
                
                serializedObject.FindProperty("finalIntensityProjection").floatValue  = EditorGUILayout.Slider(new GUIContent("Projection Intensity",
                "Sets the maximum brightness value of Global Intensity for projection meshes only. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensityProjection, 0.0f, 1.0f);

                serializedObject.FindProperty("finalIntensityFixture").floatValue  = EditorGUILayout.Slider(new GUIContent("Fixture/Other Intensity",
                "Sets the maximum brightness value of Global Intensity for everything else. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensityFixture, 0.0f, 1.0f);
            }
            else{
                serializedObject.FindProperty("finalIntensity").floatValue  = EditorGUILayout.Slider(new GUIContent("Final Intensity",
                "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensity, 0.0f, 1.0f);
            }
            EditorGUI.indentLevel--;
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

    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(VRStageLighting_AudioLink_Laser))]
    [CanEditMultipleObjects]
    public class VRStageLighting_AudioLink_Laser_Editor : VRSL_UdonEditor
    {

        void OnSceneGUI(SceneView sceneView)
        {

        }

        new void OnEnable( )
        {
            base.OnEnable();
            EditorApplication.hierarchyChanged += HierarchyChanged;
            SceneView.duringSceneGui += this.OnSceneGUI;
        }
    
        void OnDisable( )
        {
            EditorApplication.hierarchyChanged -= HierarchyChanged;
            SceneView.duringSceneGui -= this.OnSceneGUI;
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
        public static GUIStyle SectionLabel()
        {
            GUIStyle g = new GUIStyle();
            g.fontSize = 15;
            g.fontStyle = FontStyle.Bold;
            g.normal.textColor = Color.white;
            return g;
        }

        public override void OnInspectorGUI()
        {
#if UDONSHARP
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
#endif
            DrawLogo();
            ShurikenHeaderCentered(GetVersion());
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

    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [InitializeOnLoad]
    [CustomEditor(typeof(VRStageLighting_AudioLink_Static))]
    [CanEditMultipleObjects]
    public class VRStageLighting_AudioLink_Static_Editor : VRSL_UdonEditor
    {


        void OnSceneGUI(SceneView sceneView)
        {

        }

        new void OnEnable( )
        {
            base.OnEnable();
            EditorApplication.hierarchyChanged += HierarchyChanged;
            SceneView.duringSceneGui += this.OnSceneGUI;
        }
    
        void OnDisable( )
        {
            EditorApplication.hierarchyChanged -= HierarchyChanged;
            SceneView.duringSceneGui -= this.OnSceneGUI;
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
        public static GUIStyle SectionLabel()
        {
            GUIStyle g = new GUIStyle();
            g.fontSize = 14;
            g.fontStyle = FontStyle.Bold;
            g.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            return g;
        }
            void GuiLine( int i_height = 1 )

   {
        try{
       //GUIStyle g = GUIStyle.none;
       //g.fixedHeight = 6;
       Rect rect = EditorGUILayout.GetControlRect(false, i_height);

       rect.height = i_height;

       EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
        }
        catch(Exception e)
        {
            e.GetType();
        }

   }

        public override void OnInspectorGUI()
        {
#if UDONSHARP
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
#endif
            DrawLogo();
            ShurikenHeaderCentered(GetVersion());
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            VRStageLighting_AudioLink_Static fixture = (VRStageLighting_AudioLink_Static)target;
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();




            EditorGUILayout.Space();
            GuiLine();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fine Intensity Controls", SectionLabel());
            serializedObject.Update();
            EditorGUILayout.PropertyField( serializedObject.FindProperty("finalIntensityComponentMode"), new GUIContent("Control Component Intensities"));
            EditorGUI.indentLevel++;
            if(serializedObject.FindProperty("finalIntensityComponentMode").boolValue){

                serializedObject.FindProperty("finalIntensityVolumetric").floatValue  = EditorGUILayout.Slider(new GUIContent("Volumetric Intensity",
                "Sets the maximum brightness value of Global Intensity for volumetric meshes only. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensityVolumetric, 0.0f, 1.0f);
                
                serializedObject.FindProperty("finalIntensityProjection").floatValue  = EditorGUILayout.Slider(new GUIContent("Projection Intensity",
                "Sets the maximum brightness value of Global Intensity for projection meshes only. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensityProjection, 0.0f, 1.0f);

                serializedObject.FindProperty("finalIntensityFixture").floatValue  = EditorGUILayout.Slider(new GUIContent("Fixture/Other Intensity",
                "Sets the maximum brightness value of Global Intensity for everything else. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensityFixture, 0.0f, 1.0f);
            }
            else{
                serializedObject.FindProperty("finalIntensity").floatValue  = EditorGUILayout.Slider(new GUIContent("Final Intensity",
                "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."), fixture.finalIntensity, 0.0f, 1.0f);
            }

            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                UpdateSettings(fixture);
            //EditorGUIUtility.LookLikeControls();
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
#if UDONSHARP
                    #pragma warning disable 0618 //suppressing obsoletion warnings
                    //VRStageLighting_RAW_Static[] staticLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_RAW_Static>();
                    VRStageLighting_AudioLink_Static[] audioLinkLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_AudioLink_Static>();
                // VRStageLighting_Animated_Static[] animatedLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_Animated_Static>();
                    VRStageLighting_DMX_Static[] dmxLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_DMX_Static>();
                    //VRStageLighting_RAW_Laser[] rawLasers = obj.GetUdonSharpComponentsInChildren<VRStageLighting_RAW_Laser>();
                    VRStageLighting_AudioLink_Laser[] audioLinkLasers = obj.GetUdonSharpComponentsInChildren<VRStageLighting_AudioLink_Laser>();
                // VRStageLighting_DMX_Static[] dmxLights = obj.GetUdonSharpComponentsInChildren<VRStageLighting_DMX_Static>();
                    VRSL_LocalUIControlPanel[] controlPanels = obj.GetUdonSharpComponentsInChildren<VRSL_LocalUIControlPanel>();
                    #pragma warning restore 0618 //suppressing obsoletion warnings
#else
                    VRStageLighting_AudioLink_Static[] audioLinkLights = obj.GetComponentsInChildren<VRStageLighting_AudioLink_Static>();
                    VRStageLighting_DMX_Static[] dmxLights = obj.GetComponentsInChildren<VRStageLighting_DMX_Static>();
                    VRStageLighting_AudioLink_Laser[] audioLinkLasers = obj.GetComponentsInChildren<VRStageLighting_AudioLink_Laser>();
                    VRSL_LocalUIControlPanel[] controlPanels = obj.GetComponentsInChildren<VRSL_LocalUIControlPanel>();
#endif
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

                    if(controlPanels != null)
                    {
                        foreach(VRSL_LocalUIControlPanel panel in controlPanels)
                        {
                            panel._CheckDepthLightStatus();
                            //Debug.Log("AutoChecking Status");
                        }
                    }

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
}