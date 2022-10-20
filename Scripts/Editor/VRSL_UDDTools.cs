using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace VRSL.EditorScripts
{
    public class VRSL_UDDTools : EditorWindow
    {
    static uDesktopDuplication.Texture runTimeScreen;
    private static List<GameObject> sceneObjects = new List<GameObject>();
    static bool hasDesktopDuplication;

        [MenuItem("VRSL/uDD Tools")]

        static void ShowWindow() 
        {
            EditorWindow window = GetWindow<VRSL_UDDTools>();
            window.titleContent = new GUIContent("VRSL uDD Tools");
            CheckForDesktopScreen();
            window.minSize = new Vector2(300f, 50f);
            Extensions.CenterOnMainWin(window);
            window.Show();
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

    static void CheckForDesktopScreen()
    {
        // if(sceneObjects != null)
        // {
        //     return;
        // }
        sceneObjects = GetAllObjectsOnlyInScene();
        foreach(GameObject go in sceneObjects)
        {   
            runTimeScreen = go.GetComponent<uDesktopDuplication.Texture>();
            if(runTimeScreen != null)
            {
                break;
            }
        }
        if(runTimeScreen != null)
        {
            Debug.Log("Found uDesktopDuplication Screen!");
            hasDesktopDuplication = true;
        }
        else
        {
            Debug.Log("Could not find uDesktopDuplication Screen!");
            hasDesktopDuplication = false;
        }
    }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
        // Debug.Log(state);
            if(state == PlayModeStateChange.EnteredPlayMode)
            {
                CheckForDesktopScreen();
            }

        }

        public static GUIStyle SectionLabel()
        {
            GUIStyle g = new GUIStyle();
            g.fontSize = 15;
            g.fontStyle = FontStyle.Bold;
            g.normal.textColor = Color.white;
            g.alignment = TextAnchor.MiddleCenter;
            return g;
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= LogPlayModeState;
        }

        void OnGUI()
        {
            VRSL_ManagerWindow.DrawLogo();
            VRSL_ManagerWindow.ShurikenHeaderCentered(VRSL_ManagerWindow.ver);
            GUILayout.Label("uDesktopDuplication Tools",VRSL_ManagerWindow.Title2());
            if(Application.isPlaying)
            {
                if(hasDesktopDuplication && runTimeScreen !=null)
                {
                    GUILayout.Label("Scene Desktop Duplication Screen.", SectionLabel());
                    //CreateEditor(runTimeScreen, typeof(Editor).Assembly.GetType("UnityEditor.RectTransformEditor")).OnInspectorGUI();
                    var monitor = runTimeScreen.monitor;
                    //SerializedObject obj = new SerializedObject(monitor);
                    try{
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField("Monitor",runTimeScreen, runTimeScreen.GetType(), true);
                        EditorGUI.EndDisabledGroup();
                        var id = EditorGUILayout.Popup("Monitor", monitor.id, uDesktopDuplication.Manager.monitors.Select(x => x.name).ToArray());
                        if (id != monitor.id) { runTimeScreen.monitorId = id; }
                    }
                    catch(NullReferenceException e)
                    {
                        e.GetBaseException();
                        GUILayout.Label("The uDD Object is currently disabled, please re-enable it in your hiearachy!");
                    }
                    //obj.Update();
                    //SerializedProperty prop = obj.FindProperty("monitor");
                    //EditorGUILayout.PropertyField(prop, true);
                    //obj.ApplyModifiedProperties();

                    // DrawProperties(prop, true);
                }
            }
            else
            {
                GUILayout.Label("Please enter play mode to get access to the uDesktopDuplication Object in your scene!");
            }
        }
    }
}
