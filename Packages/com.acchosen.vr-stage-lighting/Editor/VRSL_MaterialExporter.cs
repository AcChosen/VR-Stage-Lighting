#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRSL.EditorScripts
{
    public class VRSL_MaterialExporter : Editor
    {
        [MenuItem("VRSL/Utilties/Generate Unique VRSL Materials", priority = 251)]
        public static void GenerateUniqueVRSLMaterials()
        {
            //Get target folder and scene name
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder To Save Materials In","Assets", "");
            try
            {        
                int assetIndex = folderPath.IndexOf("Assets");
                folderPath = folderPath.Substring(assetIndex);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
            string sceneName = SceneManager.GetActiveScene().name;
            //Find all objects in scene and get their materials if they are VRSL materials
            Transform[] transforms = FindObjectsOfType<Transform>();
            Dictionary<string,Material> vrslMats = new Dictionary<string, Material>();
            for(int i = 0; i < transforms.Length; i++)
            {
                Renderer rend = transforms[i].gameObject.GetComponent<Renderer>();
                if(rend == null){continue;}
                Material mat = rend.sharedMaterial;
                if(!vrslMats.ContainsValue(mat))
                {
                    if(mat.name.StartsWith("VRSL"))
                    {
                        try{
                        vrslMats.Add(mat.name, mat);
                        }
                        catch(ArgumentException e)
                        {
                            string trace = e.StackTrace;
                        }
                    }
                }
            }
            //Make copies of materials and save them to folder
            Dictionary<string,Material> vrslMatsCopy = new Dictionary<string,Material>(vrslMats);
            var keys = new List<string>(vrslMatsCopy.Keys);
            string newFolderName = "VRSLMaterials-" + sceneName;
            AssetDatabase.CreateFolder(folderPath, newFolderName);
            foreach (string key in keys)
            {
                vrslMatsCopy[key] = new Material(vrslMats[key]);
                vrslMatsCopy[key].name = sceneName + "-" + vrslMatsCopy[key].name;
                AssetDatabase.CreateAsset(vrslMatsCopy[key], folderPath + "/" + newFolderName + "/" + vrslMatsCopy[key].name + ".mat");
            }
            //Apply new materials to objects.
            for(int i = 0; i < transforms.Length; i++)
            {
                Renderer rend = transforms[i].gameObject.GetComponent<Renderer>();
                if(rend == null){continue;}
                Material mat = rend.sharedMaterial;
                if(vrslMats.ContainsValue(mat))
                {
                    mat = vrslMatsCopy[mat.name];
                }
                rend.material = mat;
                Undo.RecordObject(rend, "Generate Unique VRSL Materials");
                if(PrefabUtility.IsPartOfAnyPrefab(rend))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(rend);
                }
            }

                VRSL_LocalUIControlPanel controlPanel = FindFirstObjectByType<VRSL_LocalUIControlPanel>();
                if(controlPanel != null)
                {
                    for(int i = 0; i < controlPanel.laserMaterials.Length; i++){
                        if(vrslMats.ContainsValue(controlPanel.laserMaterials[i]))
                        {
                            controlPanel.laserMaterials[i] = vrslMatsCopy[controlPanel.laserMaterials[i].name];
                        }
                    }
                    for(int i = 0; i < controlPanel.fixtureMaterials.Length; i++){
                        if(vrslMats.ContainsValue(controlPanel.fixtureMaterials[i]))
                        {
                            controlPanel.fixtureMaterials[i] = vrslMatsCopy[controlPanel.fixtureMaterials[i].name];
                        }
                    }
                    for(int i = 0; i < controlPanel.discoBallMaterials.Length; i++){
                        if(vrslMats.ContainsValue(controlPanel.discoBallMaterials[i]))
                        {
                            controlPanel.discoBallMaterials[i] = vrslMatsCopy[controlPanel.discoBallMaterials[i].name];
                        }
                    }
                    for(int i = 0; i < controlPanel.projectionMaterials.Length; i++){
                        if(vrslMats.ContainsValue(controlPanel.projectionMaterials[i]))
                        {
                            controlPanel.projectionMaterials[i] = vrslMatsCopy[controlPanel.projectionMaterials[i].name];
                        }
                    }
                    for(int i = 0; i < controlPanel.volumetricMaterials.Length; i++){
                        if(vrslMats.ContainsValue(controlPanel.volumetricMaterials[i]))
                        {
                            controlPanel.volumetricMaterials[i] = vrslMatsCopy[controlPanel.volumetricMaterials[i].name];
                        }
                    }
                    Undo.RecordObject(controlPanel, "Generate Unique VRSL Materials");
                    if(PrefabUtility.IsPartOfAnyPrefab(controlPanel))
                    {
                        PrefabUtility.RecordPrefabInstancePropertyModifications(controlPanel);
                    }
                }
        }
    }
}
#endif