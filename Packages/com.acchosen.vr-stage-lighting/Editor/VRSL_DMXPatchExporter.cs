#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UDONSHARP
using UdonSharpEditor;
#endif
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace VRSL.EditorScripts
{
    public class VRSL_DMXPatchExporter : Editor
    {
        private static VRSL_LocalUIControlPanel panel;
        private static List<GameObject> sceneObjects = new List<GameObject>();
        public static bool hasLocalPanel;
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
        private static void CheckForLocalPanel()
        {
            sceneObjects = GetAllObjectsOnlyInScene();
            hasLocalPanel = false;
            panel = null;
         //   colorLabel = new GUIContent();
         //  colorLabel.text = "Emission Color";
            foreach (GameObject go in sceneObjects)
            {
                #if UDONSHARP
                #pragma warning disable 0618 //suppressing obsoletion warnings
                panel =  go.GetUdonSharpComponent<VRSL_LocalUIControlPanel>();
                #pragma warning restore 0618
                #else
                panel =  go.GetComponent<VRSL_LocalUIControlPanel>();
                #endif
                if(panel != null)
                {
                    hasLocalPanel = true;
                    break;
                }
            }
            return;
        }
 

        [MenuItem("VRSL/Save Patch Data", priority = 500)]
        public static void SavePatchData()
        {
            CheckForLocalPanel();
            if(!hasLocalPanel){Debug.LogWarning("VRSL Patch Exporter: Please make sure there is a VRSL-LocalUIControlPanel in your scene before attempting to save."); return;}
            if(panel.fixtureSaveFile == "NONE")
            {
                try
                {
                    VRSL_DMXPatchSettings asset = ScriptableObject.CreateInstance<VRSL_DMXPatchSettings>();
                    asset.SetDMXFixtureData();
                    string name = "VRSL DMX Patch Data_" + SceneManager.GetActiveScene().name + ".asset";
                    string parentDirectory = "VRSL DMX Patch Folder";
                    if(AssetDatabase.IsValidFolder("Assets/" + parentDirectory) == false)
                    {
                        AssetDatabase.CreateFolder("Assets", parentDirectory);
                    }
                    string path = "Assets/" + parentDirectory + "/" + name;
                    AssetDatabase.CreateAsset(asset, path);
                 //   AssetDatabase.SaveAssets();

                  //  EditorUtility.FocusProjectWindow();

                   // Selection.activeObject = asset;


                    SerializedObject so = new SerializedObject(panel);
                    so.FindProperty("fixtureSaveFile").stringValue = AssetDatabase.GUIDFromAssetPath(path).ToString();
                    so.ApplyModifiedProperties();
#if UDONSHARP
                    #pragma warning disable 0618 //suppressing obsoletion warnings
                    panel.UpdateProxy();
                    #pragma warning restore 0618 //suppressing obsoletion warnings
                    panel.fixtureSaveFile = so.FindProperty("fixtureSaveFile").stringValue;
                    #pragma warning disable 0618 //suppressing obsoletion warnings
                    panel.ApplyProxyModifications();
                    #pragma warning restore 0618 //suppressing obsoletion warnings
#else
                    panel.fixtureSaveFile = so.FindProperty("fixtureSaveFile").stringValue;
#endif

                   /// asset = (VRSL_DMXPatchSettings) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureSaveFile), typeof(VRSL_DMXPatchSettings));
                   // asset.SetScene();
                   // asset.SetDMXFixtureData();
                    asset.ForceSave();
                }
                catch
                {
                    Debug.LogError("VRSL Patch Exporter: Failed to create patch data.");
                }
            }
            else
            {
                try
                {
                    VRSL_DMXPatchSettings asset = (VRSL_DMXPatchSettings) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureSaveFile), typeof(VRSL_DMXPatchSettings));
                    asset.SetScene();
                    asset.SetDMXFixtureData();
                    asset.ForceSave();
                }
                catch (NullReferenceException e)
                {
                   // Debug.LogError("VRSL Patch Exporter: Could not find patch data file. Removing Link patch data link.");
                    e.GetType();
                    SerializedObject so = new SerializedObject(panel);
                    so.FindProperty("fixtureSaveFile").stringValue = "NONE";
                    so.ApplyModifiedProperties();
#if UDONSHARP
                    #pragma warning disable 0618 //suppressing obsoletion warnings
                    panel.UpdateProxy();
                    #pragma warning restore 0618 //suppressing obsoletion warnings
                    panel.fixtureSaveFile = so.FindProperty("fixtureSaveFile").stringValue;
                    #pragma warning disable 0618 //suppressing obsoletion warnings
                    panel.ApplyProxyModifications();
                    #pragma warning restore 0618 //suppressing obsoletion warnings
#else
                    panel.fixtureSaveFile = so.FindProperty("fixtureSaveFile").stringValue;
#endif
                    SavePatchData();
                }
                catch(Exception e)
                {
                    Debug.LogError("VRSL Patch Exporter: Failed to save patch data.");
                    e.GetType();
                }
            }
            

        }
        [MenuItem("VRSL/Load Patch Data", priority = 501)]
        public static void LoadPatchData()
        {
            CheckForLocalPanel();
            if(!hasLocalPanel){Debug.LogWarning("VRSL Patch Exporter: Please make sure there is a VRSL-LocalUIControlPanel in your scene before attempting to Load."); return;}
            if(panel.fixtureSaveFile != "NONE")
            {
                try
                {
                    VRSL_DMXPatchSettings asset = (VRSL_DMXPatchSettings) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureSaveFile), typeof(VRSL_DMXPatchSettings));
                    asset.LoadDMXFixtureData();
                }
                catch
                {
                    Debug.LogError("VRSL Patch Exporter: Failed to load patch data.");
                }
            }
            else
            {
                Debug.LogError("VRSL Patch Exporter: Fixture Save File Not Found. The file may have been lost or has not been created yet.");
            }
        }
        [MenuItem("VRSL/Export/To JSON", priority = 502)]
        public static void ExportToJSON()
        {
            CheckForLocalPanel();
            if(!hasLocalPanel){Debug.LogWarning("VRSL Patch Exporter: Please make sure there is a VRSL-LocalUIControlPanel in your scene before attempting to export."); return;}
            if(panel.fixtureSaveFile != "NONE")
            {
                try
                {
                    VRSL_DMXPatchSettings asset = (VRSL_DMXPatchSettings) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureSaveFile), typeof(VRSL_DMXPatchSettings));
                    asset.ToJsonFile(true);
                }
                catch
                {
                    Debug.LogError("VRSL Patch Exporter: Failed to export patch data.");
                }
            }
            else
            {
                Debug.LogError("VRSL Patch Exporter: Fixture Save File Not Found. The file may have been lost or has not been created yet.");
            }
        }

        [MenuItem("VRSL/Export/To MVR", priority = 503)]
        public static void ExportToMVR()
        {
            CheckForLocalPanel();
            if(!hasLocalPanel){Debug.LogWarning("VRSL Patch Exporter: Please make sure there is a VRSL-LocalUIControlPanel in your scene before attempting to export."); return;}
            if(panel.fixtureSaveFile != "NONE")
            {
                try
                {
                    VRSL_DMXPatchSettings asset = (VRSL_DMXPatchSettings) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureSaveFile), typeof(VRSL_DMXPatchSettings));
                    asset.ToMVRFile();
                }
                catch
                {
                    Debug.LogError("VRSL Patch Exporter: Failed to export patch data.");
                    throw;
                }
            }
            else
            {
                Debug.LogError("VRSL Patch Exporter: Fixture Save File Not Found. The file may have been lost or has not been created yet.");
            }
        }

        [MenuItem("VRSL/Export/To PDF (Windows)", priority = 504)]
        public static void ExportToPDF()
        {
            CheckForLocalPanel();
            if(!hasLocalPanel){Debug.LogWarning("VRSL Patch Exporter: Please make sure there is a VRSL-LocalUIControlPanel in your scene before attempting to export."); return;}
            if(panel.fixtureSaveFile != "NONE")
            {
                try
                {
#if !UNITY_EDITOR_LINUX && !UNITY_ANDROID && !UNITY_IOS
                    VRSL_DMXPatchSettings asset = (VRSL_DMXPatchSettings) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureSaveFile), typeof(VRSL_DMXPatchSettings));
                    asset.ToPDF();
#else
                    EditorUtility.DisplayDialog("PDF export error", "PDF export is currently a Windows only feature", "OK", "Cancel");
#endif
                }
                catch
                {
                    Debug.LogError("VRSL Patch Exporter: Failed to export patch data.");
                    throw;
                }
            }
            else
            {
                Debug.LogError("VRSL Patch Exporter: Fixture Save File Not Found. The file may have been lost or has not been created yet.");
            }
        }
    }
}
#endif
