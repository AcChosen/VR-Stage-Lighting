#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
namespace VRSL.EditorScripts
{
    [Serializable]
    public struct FixtureDefintion
    {
        public FixtureDefintion(string n)
        {
            name = n;
            channelNames = new string[1];
            foldOut = false;
        }
        public string name;
        public string[] channelNames;
        public bool foldOut;

        public void SetNewChannelSize(int size)
        {
            string[] newChannelNames = new string[size];
            int loopVal = 0;
            if(channelNames != null)
            {
                if(newChannelNames.Length > channelNames.Length)
                {
                    loopVal = channelNames.Length;
                }
                else
                {
                    loopVal = newChannelNames.Length;
                }

                for(int i = 0; i < loopVal; i++)
                {
                    newChannelNames[i] = channelNames[i];
                }
                channelNames = newChannelNames;
            }
            else
            {
                channelNames = new string[1];
                channelNames[0] = "";
            }
        }
    }
    [CreateAssetMenuAttribute(menuName = "VRSL/DMX Fixture Definition File", fileName = "VRSL DMX Fixture Definitions")]
    [System.Serializable]
    public class VRSL_FixtureDefinitions : ScriptableObject
    {
        [HideInInspector]
        public FixtureDefintion[] definitions = new FixtureDefintion[1];

        public VRSL_FixtureDefinitions()
        {
            if(definitions != null)
            {
                if(definitions.Length > 0)
                {
                    definitions[0].channelNames = new string[1];
                }
            }
        }
        public void ForceSave()
        {
            //string assetPath =  AssetDatabase.GetAssetPath(this.GetInstanceID());
            //if(targetScene != null)
                //AssetDatabase.RenameAsset(assetPath, "VRSL DMX Fixture Definitions_" + targetScene.name);
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<VRSL_FixtureDefinitions>(AssetDatabase.GetAssetPath(this.GetInstanceID())); 
        }

        public string[] GetNames()
        {
            string[] names = new string[definitions.Length];
            for(int i = 0; i < definitions.Length; i++)
            {
                names[i] = definitions[i].name;
            }
            return names;
        }
        public string[] GetChannelDefinition(int defID)
        {
            return definitions[defID].channelNames;
        }
        public int DefinitionsArraySize
        {
            get
            {
                return definitions.Length;
            }
            set
            {
                FixtureDefintion[] newDefinitions = new FixtureDefintion[value];
                int loopVal = 0;
                if(newDefinitions.Length > definitions.Length)
                {
                    loopVal = definitions.Length;
                }
                else
                {
                    loopVal = newDefinitions.Length;
                }
                for(int i = 0; i < loopVal; i++)
                {
                    newDefinitions[i] = definitions[i];
                }
                definitions = newDefinitions;
                //definitions = new FixtureDefintion[value];
            }
        } 
    }


    [CustomEditor(typeof(VRSL_FixtureDefinitions))]
    public class VRSL_FixtureDefinitionss_Editor: Editor 
    {

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
        private SerializedProperty definitions;
        VRSL_FixtureDefinitions fd = null;
      //  SerializedObject so;
        private void OnEnable()
        {
            // // Link the properties

            
            fd = (VRSL_FixtureDefinitions) target;
           // so = new SerializedObject(fd);
            definitions = serializedObject.FindProperty("definitions");
        }

        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            serializedObject.Update();
           // EditorGUI.BeginChangeCheck();

            
            if(fd != null)
            {
                if(definitions.isArray)
                {

                    int size = definitions.arraySize;

                    
                    EditorGUILayout.BeginHorizontal();
                    int newSize = EditorGUILayout.IntField("Size", size);
                    newSize = Mathf.Abs(newSize);
                    if(GUILayout.Button("-", GUILayout.Width(25f)))
                    {
                        newSize--;
                        if(newSize < 1){newSize = 1;}
                    }
                    if(GUILayout.Button("+", GUILayout.Width(25f)))
                    {
                        newSize++;
                    }
                    EditorGUILayout.EndHorizontal();
                    if(GUILayout.Button("Save Changes"))
                    {
                        serializedObject.ApplyModifiedProperties();
                        fd.ForceSave();
                    }
                    GuiLine();
                    GUILayout.Space(25);
                    bool mainIncreased = false;
                    if (newSize != size)
                    {
                        definitions.arraySize = newSize;
                        fd.DefinitionsArraySize = newSize;
                        mainIncreased = newSize > size;
                    }
                    EditorGUI.indentLevel++;
                    //EditorGUI.indentLevel++;
                    //definitions.arraySize = EditorGUILayout.IntField("Size",definitions.arraySize);
                    for(int i = 0; i < newSize; i++)
                    {
                        EditorGUILayout.BeginVertical("box");
                        SerializedProperty defProp = definitions.GetArrayElementAtIndex(i); 
                        SerializedProperty nameProp = defProp.FindPropertyRelative("name");
                        EditorGUILayout.BeginHorizontal("box");
                        nameProp.stringValue = EditorGUILayout.TextField("Definition " + (i+1).ToString(), nameProp.stringValue);

                        
                        SerializedProperty channelNamesProp = defProp.FindPropertyRelative("channelNames");
                        if(i >= size && mainIncreased)
                        {
                            nameProp.stringValue = "";
                            if(channelNamesProp.isArray)
                            {
                                channelNamesProp.arraySize = 1;
                                fd.definitions[i].SetNewChannelSize(1);
                                SerializedProperty channel = channelNamesProp.GetArrayElementAtIndex(0); 
                                channel.stringValue = "";
                            }
                        }
                        else
                        {
                            int chanSize = channelNamesProp.arraySize;
                            int newChanSize = chanSize;
                            if(GUILayout.Button("-", GUILayout.Width(25f)))
                            {
                                newChanSize--;
                                if(newChanSize < 1){newChanSize = 1;}
                            }
                            if(GUILayout.Button("+", GUILayout.Width(25f)))
                            {
                                newChanSize++;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel++;
                            EditorGUI.indentLevel++;
                            defProp.FindPropertyRelative("foldOut").boolValue = EditorGUILayout.Foldout(defProp.FindPropertyRelative("foldOut").boolValue, "Channels");
                            if(defProp.FindPropertyRelative("foldOut").boolValue)
                            {
                                if(channelNamesProp.isArray)
                                {
                                    bool increased = false;
                                    if (newChanSize != chanSize)
                                    {
                                        channelNamesProp.arraySize = newChanSize;
                                        fd.definitions[i].SetNewChannelSize(newChanSize);
                                        increased = newChanSize > chanSize;
                                    }

                                    EditorGUI.indentLevel++;
                                   // EditorGUI.indentLevel++;
                                    for (int j = 0; j < newChanSize; j++)
                                    {
                                        SerializedProperty channel = channelNamesProp.GetArrayElementAtIndex(j); 
                                        channel.stringValue = EditorGUILayout.TextField("Channel " + (j + 1).ToString(), channel.stringValue);
                                        if(j == newChanSize-1 && increased)
                                        {
                                            channel.stringValue = "";
                                        }
                                    }
                                 //   EditorGUI.indentLevel--;
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUI.indentLevel--;
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(10);
                    }
                    EditorGUI.indentLevel--;
                    //EditorGUI.indentLevel--;
                }
            }
          //  if(EditorGUI.EndChangeCheck())
        //    {
                serializedObject.ApplyModifiedProperties();
                // if(fd != null)
                // {
                //     fd.ForceSave()
                // }
          //  }
        }
    }

}
#endif