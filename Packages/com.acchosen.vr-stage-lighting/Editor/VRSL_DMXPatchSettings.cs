#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Data;
using UnityEngine.UIElements;
using System.Linq;

#if UDONSHARP
using VRC.Udon;
using UdonSharpEditor;
#endif

#if !UNITY_EDITOR_LINUX && !UNITY_ANDROID && !UNITY_IOS
using System.Drawing.Printing;
#endif

namespace VRSL.EditorScripts
{
    public class StringWriterUtf8 : StringWriter
    {
        public StringWriterUtf8(StringBuilder sb) : base(sb)
        {
        }
 
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    [Serializable]
    public struct JSONDMXFixtureData_Container
    {
        public JSONDMXFixtureData_Container(JSONDMXFixtureData[] d)
        {
            fixtures = d;
        }
        public JSONDMXFixtureData[] fixtures;
    }
    [Serializable]
    public struct JSONDMXFixtureData
    {
        public JSONDMXFixtureData(DMXFixtureData fixture, string[] fixtureTypes, string[] cd)
        {
            name = fixture.name;
            id = fixture.fixtureID;
            channel = fixture.dmxChannel;
            universe = fixture.dmxUniverse;
            fixtureDefintion = fixtureTypes[fixture.fixtureDefintion];
            channelNames = cd;
            position = fixture.position;
            rotation = fixture.rotation.eulerAngles;
            invertPan = fixture.invertPan;
            invertTilt = fixture.invertTilt;
            panRange = Mathf.Abs(fixture.maxMinPan);
            tiltRange = Mathf.Abs(fixture.maxMinTilt);
        }
        public int id;
        public string name;
        public int channel;
        public int universe;
        public string fixtureDefintion;
        public string[] channelNames;  
        public Vector3 position;
        public Vector3 rotation;
        public bool invertPan;
        public bool invertTilt;
        public float panRange;
        public float tiltRange;
    }

    [Serializable]
    public struct DMXFixtureData_ObjRenderers
    {
        public DMXFixtureData_ObjRenderers(MeshRenderer[] objRenderers)
        {
                objRenderers_name = new string[objRenderers.Length];
                objRenderers_GlobalObjectId = new string[objRenderers.Length];
                for(int i = 0; i < objRenderers.Length; i++)
                {
                    objRenderers_name[i] = objRenderers[i].name;
                    GlobalObjectId objRenderers_id = GlobalObjectId.GetGlobalObjectIdSlow(objRenderers[i]);
                    objRenderers_GlobalObjectId[i] = objRenderers_id.ToString();
                }
        }
        public string[] objRenderers_name;
        public string[] objRenderers_GlobalObjectId;
        


        public MeshRenderer[] GetRenderers()
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            for(int i = 0; i < objRenderers_GlobalObjectId.Length; i++)
            {
                GlobalObjectId id;
                if(GlobalObjectId.TryParse(objRenderers_GlobalObjectId[i], out id))
                {
                    MeshRenderer x = (MeshRenderer) GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
                    //UnityEngine.Debug.Log("Found Renderer: " + x.name);
                    renderers.Add(x);
                }
            }
            return renderers.ToArray();
        }
    }



    [Serializable]
    public struct DMXFixtureData
    {
        public DMXFixtureData(VRStageLighting_DMX_Static fixture, GlobalObjectId id)
        {
                name = fixture.gameObject.name;
                position = fixture.gameObject.transform.position;
                rotation = fixture.gameObject.transform.rotation;
                targetObjectId = id.targetObjectId;
                targetPrefabId = id.targetPrefabId;
                assetGUID = id.assetGUID.ToString();
                enableDMXChannels = fixture.enableDMXChannels;
                nineUniverseMode = fixture.nineUniverseMode;
                fixtureID = fixture.fixtureID;
                useLegacySectorMode = fixture.useLegacySectorMode;
                singleChannelMode = fixture.singleChannelMode;
                sector = fixture.sector;
                Channel = fixture.Channel;
                legacyGoboRange = fixture.legacyGoboRange;
                globalIntensity = fixture.globalIntensity;
                finalIntensity = fixture.finalIntensity;
                lightColorTint = fixture.lightColorTint;
                invertPan = fixture.invertPan;
                invertTilt = fixture.invertTilt;
                isUpsideDown = fixture.isUpsideDown;
                enableAutoSpin = fixture.enableAutoSpin;
                enableStrobe = fixture.enableStrobe;
                tiltOffsetBlue = fixture.tiltOffsetBlue;
                panOffsetBlueGreen = fixture.panOffsetBlueGreen;
                selectGOBO = fixture.selectGOBO;
                //objRenderers = fixture.objRenderers;
                objRenderers = new DMXFixtureData_ObjRenderers(fixture.objRenderers);
                coneWidth = fixture.coneWidth;
                coneLength = fixture.coneLength;
                maxConeLength = fixture.maxConeLength;
                maxMinPan = fixture.maxMinPan;
                maxMinTilt = fixture.maxMinTilt;
                fixtureDefintion = fixture.fixtureDefintion;
                if(useLegacySectorMode)
                {
                    Vector2Int chanUni = fixture.GetSectorConversion();
                    dmxChannel = chanUni.x;
                    dmxUniverse = chanUni.y;
                }
                else
                {
                    dmxChannel = fixture.dmxChannel;
                    dmxUniverse = fixture.dmxUniverse;
                }


        }
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public ulong targetPrefabId;
        public ulong targetObjectId;
        public string assetGUID;
        public bool enableDMXChannels;
        public bool nineUniverseMode;
        public int fixtureID;
        public int dmxChannel;
        public int dmxUniverse;
        public int fixtureDefintion;
        public bool useLegacySectorMode;
        public bool singleChannelMode;
        public int sector;
        public int Channel;
        public bool legacyGoboRange;
        public float globalIntensity;
        public float finalIntensity;
        public UnityEngine.Color lightColorTint;
        public bool invertPan;
        public bool invertTilt;
        public bool isUpsideDown;
        public bool enableAutoSpin;
        public bool enableStrobe;
        public float tiltOffsetBlue;
        public float panOffsetBlueGreen;
        public int selectGOBO;
        public DMXFixtureData_ObjRenderers objRenderers;
        public float coneWidth;
        public float coneLength;
        public float maxConeLength;
        public float maxMinPan;
        public float maxMinTilt;

        
    }
    [CreateAssetMenuAttribute(menuName = "VRSL/DMX Fixture Patch File", fileName = "VRSL DMX Fixture Patch File")]
    [System.Serializable]
    public class VRSL_DMXPatchSettings : ScriptableObject
    {

        [HideInInspector]
        public Scene targetScene;
        [HideInInspector] 
        public string scenePath;
        [HideInInspector]
        public string[] idStrings;
        [HideInInspector]
        public DMXFixtureData[] data;

        private VRSL_LocalUIControlPanel panel;
        private string[] dataToPDF;
        private int pdfLineCount = 0;
        private int pdfPageCount = 1;
        private DataTable patchList;


        public void CheckData()
        {
            if(data == null)
            {
                UnityEngine.Debug.Log("DMX Fixture Data Array is null!");
            }
            else
            {
                if(data.Length == 0)
                {
                    UnityEngine.Debug.Log("DMX Fixture Data Array is length 0!");
                }
            }
        }
        public void ForceSave()
        {
            string assetPath =  AssetDatabase.GetAssetPath(this.GetInstanceID());
            if(targetScene != null)
                AssetDatabase.RenameAsset(assetPath, "VRSL DMX Patch Data_" + targetScene.name);
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<VRSL_DMXPatchSettings>(AssetDatabase.GetAssetPath(this.GetInstanceID())); 
        }
        public void SetScene()
        {
            targetScene = SceneManager.GetActiveScene();
            scenePath = targetScene.path;
        }
        public void SetDMXFixtureData()
        {
            UnityEngine.Debug.Log("Saving Fixture Data...");
            List<GameObject> sceneObjects = GetAllObjectsOnlyInScene();
            List<VRStageLighting_DMX_Static> sceneFixtures = new List<VRStageLighting_DMX_Static>();
            foreach(GameObject go in sceneObjects)
            {
#if UDONSHARP
                #pragma warning disable 0618 //suppressing obsoletion warnings
                VRStageLighting_DMX_Static lightScript = go.GetUdonSharpComponent<VRStageLighting_DMX_Static>();
                #pragma warning restore 0618 //suppressing obsoletion warnings
#else
                VRStageLighting_DMX_Static lightScript = go.GetComponent<VRStageLighting_DMX_Static>();
#endif
                if(lightScript != null)
                {
                    sceneFixtures.Add(lightScript);
                }
            }
            VRStageLighting_DMX_Static[] fixtures = sceneFixtures.ToArray();
            if(fixtures.Length > 0)
            {
                targetScene = SceneManager.GetActiveScene();
                scenePath = targetScene.path;
                //this.name += "_" + targetScene.name;

                data = new DMXFixtureData[fixtures.Length];
                GlobalObjectId[] ids = new GlobalObjectId[fixtures.Length];
                idStrings = new string[fixtures.Length];
                GlobalObjectId.GetGlobalObjectIdsSlow(fixtures, ids);
                for(int i = 0; i < data.Length; i++)
                {
                    idStrings[i] = ids[i].ToString();
                    data[i] = new DMXFixtureData(fixtures[i], ids[i]);

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

        // GlobalObjectId[] GetAllFixtureIDs(VRStageLighting_DMX_Static[] fixtures)
        // {
        //     GlobalObjectId[] ids = new GlobalObjectId[fixtures.Length];
        //     GlobalObjectId.GetGlobalObjectIdsSlow(fixtures, ids);
        //     return ids;
        // }
        public void LoadDMXFixtureData()
        {
            if(data == null){return;}

            if(targetScene == null){return;}
            if(data.Length > 0)
            {
                List<GameObject> sceneObjects = GetAllObjectsOnlyInScene();
                List<VRStageLighting_DMX_Static> sceneFixtures = new List<VRStageLighting_DMX_Static>();
                foreach(GameObject go in sceneObjects)
                {
#if UDONSHARP
                    #pragma warning disable 0618 //suppressing obsoletion warnings
                    VRStageLighting_DMX_Static lightScript = go.GetUdonSharpComponent<VRStageLighting_DMX_Static>();
                    #pragma warning restore 0618 //suppressing obsoletion warnings
#else
                    VRStageLighting_DMX_Static lightScript = go.GetComponent<VRStageLighting_DMX_Static>();
#endif
                    if(lightScript != null)
                    {
                        sceneFixtures.Add(lightScript);
                    }
                }

                //GlobalObjectId[] ids  = GetAllFixtureIDs(sceneFixtures.ToArray());
                foreach(VRStageLighting_DMX_Static fixture in sceneFixtures)
                {
                    try
                    {
                        bool sucess = false;

                        int dmxID = 0;
                        GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(fixture);
                        string guid = id.assetGUID.ToString();
                        ulong localID = 0;
                        for(int i = 0; i < data.Length; i++)
                        {   
                            if(PrefabUtility.IsPartOfAnyPrefab(fixture))
                            {
                                localID = id.targetPrefabId;
                                    if(guid == data[i].assetGUID)
                                    {
                                      //  Debug.Log("Found " + guid + " with " + data[i].assetGUID);
                                        if((ulong) localID == data[i].targetPrefabId)
                                        {
                                            UnityEngine.Debug.Log("Found " + localID + " successfully at " + fixture.name);
                                            sucess = true;
                                            dmxID = i;
                                            break;
                                        }
                                    }
                            }
                            else
                            {
                                localID = id.targetObjectId;    
                                if(guid == data[i].assetGUID)
                                {
                                 //   Debug.Log("Found " + guid + " with " + data[i].assetGUID);
                                    if((ulong) localID == data[i].targetObjectId)
                                    {
                                        UnityEngine.Debug.Log("Found " + localID + " successfully at " + fixture.name);
                                        sucess = true;
                                        dmxID = i;
                                        break;
                                    }
                                }
                            }
                        }
                        if(sucess)
                        {
                            var so = new SerializedObject(fixture);
                            so.FindProperty("enableDMXChannels").boolValue = data[dmxID].enableDMXChannels;
                            so.FindProperty("fixtureID").intValue = data[dmxID].fixtureID;
                            so.FindProperty("dmxChannel").intValue = data[dmxID].dmxChannel;
                            so.FindProperty("dmxUniverse").intValue = data[dmxID].dmxUniverse;
                            so.FindProperty("useLegacySectorMode").boolValue = data[dmxID].useLegacySectorMode;
                            so.FindProperty("singleChannelMode").boolValue = data[dmxID].singleChannelMode;
                            so.FindProperty("sector").intValue = data[dmxID].sector; 
                            so.FindProperty("Channel").intValue = data[dmxID].Channel;
                            so.FindProperty("legacyGoboRange").boolValue = data[dmxID].legacyGoboRange;
                            so.FindProperty("globalIntensity").floatValue = data[dmxID].globalIntensity;
                            so.FindProperty("finalIntensity").floatValue = data[dmxID].finalIntensity;
                            so.FindProperty("lightColorTint").colorValue = data[dmxID].lightColorTint;
                            so.FindProperty("nineUniverseMode").boolValue = data[dmxID].nineUniverseMode;
                            so.FindProperty("invertPan").boolValue = data[dmxID].invertPan;
                            so.FindProperty("invertTilt").boolValue = data[dmxID].invertTilt;
                            so.FindProperty("isUpsideDown").boolValue = data[dmxID].isUpsideDown;
                            so.FindProperty("enableAutoSpin").boolValue = data[dmxID].enableAutoSpin;
                            so.FindProperty("enableStrobe").boolValue = data[dmxID].enableStrobe;
                            so.FindProperty("tiltOffsetBlue").floatValue = data[dmxID].tiltOffsetBlue;
                            so.FindProperty("panOffsetBlueGreen").floatValue = data[dmxID].panOffsetBlueGreen;
                            so.FindProperty("selectGOBO").intValue = data[dmxID].selectGOBO;
                            so.FindProperty("coneWidth").floatValue = data[dmxID].coneWidth;
                            so.FindProperty("coneLength").floatValue = data[dmxID].coneLength;
                            so.FindProperty("maxConeLength").floatValue = data[dmxID].maxConeLength;
                            so.FindProperty("maxMinPan").floatValue = data[dmxID].maxMinPan;
                            so.FindProperty("maxMinTilt").floatValue = data[dmxID].maxMinTilt;
                            so.FindProperty("fixtureDefintion").intValue = data[dmxID].fixtureDefintion;
                            SerializedProperty rendsProp = so.FindProperty("objRenderers");
                            rendsProp.ClearArray();
                            so.ApplyModifiedProperties();

                            var sof = new SerializedObject(fixture);
                            SerializedProperty rendsProperty = sof.FindProperty("objRenderers");
                            MeshRenderer[] rends = data[dmxID].objRenderers.GetRenderers();
                            rendsProperty.arraySize = rends.Length;
                            for(int i = 0; i < rends.Length; i++)
                            {
                                UnityEngine.Debug.Log("Array Element: " + i);
                                rendsProperty.InsertArrayElementAtIndex(i);
                                rendsProperty.GetArrayElementAtIndex(i).objectReferenceValue = rends[i];
                            }
                            sof.ApplyModifiedProperties();

#if UDONSHARP
                            #pragma warning disable 0618 //suppressing obsoletion warnings
                            fixture.UpdateProxy();
                            #pragma warning restore 0618 //suppressing obsoletion warnings.
#endif

                            fixture.enableDMXChannels = data[dmxID].enableDMXChannels;
                            fixture.fixtureID = data[dmxID].fixtureID;
                            fixture.dmxChannel = data[dmxID].dmxChannel;
                            fixture.dmxUniverse = data[dmxID].dmxUniverse;
                            fixture.useLegacySectorMode = data[dmxID].useLegacySectorMode;
                            fixture.singleChannelMode = data[dmxID].singleChannelMode;
                            fixture.sector = data[dmxID].sector; 
                            fixture.Channel = data[dmxID].Channel;
                            fixture.legacyGoboRange = data[dmxID].legacyGoboRange;
                            fixture.globalIntensity = data[dmxID].globalIntensity;
                            fixture.finalIntensity = data[dmxID].finalIntensity;
                            fixture.lightColorTint = data[dmxID].lightColorTint;
                            fixture.nineUniverseMode = data[dmxID].nineUniverseMode;
                            fixture.invertPan = data[dmxID].invertPan;
                            fixture.invertTilt = data[dmxID].invertTilt;
                            fixture.isUpsideDown = data[dmxID].isUpsideDown;
                            fixture.enableAutoSpin = data[dmxID].enableAutoSpin;
                            fixture.enableStrobe = data[dmxID].enableStrobe;
                            fixture.tiltOffsetBlue = data[dmxID].tiltOffsetBlue;
                            fixture.panOffsetBlueGreen = data[dmxID].panOffsetBlueGreen;
                            fixture.selectGOBO = data[dmxID].selectGOBO;
                            fixture.coneWidth = data[dmxID].coneWidth;
                            fixture.coneLength = data[dmxID].coneLength;
                            fixture.maxConeLength =  data[dmxID].maxConeLength;
                            fixture.maxMinPan = data[dmxID].maxMinPan;
                            fixture.maxMinTilt = data[dmxID].maxMinTilt;
                            fixture.fixtureDefintion = data[dmxID].fixtureDefintion;
                            fixture.objRenderers = rends;

#if UDONSHARP
                            #pragma warning disable 0618 //suppressing obsoletion warnings
                            fixture.ApplyProxyModifications();
                            #pragma warning restore 0618 //suppressing obsoletion warnings
#endif
                            if(PrefabUtility.IsPartOfAnyPrefab(fixture))
                            {
                                PrefabUtility.RecordPrefabInstancePropertyModifications(fixture);
                            }
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
                    catch(Exception ex)
                    {
                            // Get stack trace for the exception with source file information
                        var st = new StackTrace(ex, true);
                        // Get the top stack frame
                        var frame = st.GetFrame(0);
                        // Get the line number from the stack frame
                        var line = frame.GetFileLineNumber();
                        UnityEngine.Debug.LogError("Error At Line: " + line.ToString());
                        throw;
                    }
                }
            }
        }
        private bool CheckForLocalPanel()
        {
            List<GameObject> sceneObjects = sceneObjects = GetAllObjectsOnlyInScene();
            bool hasLocalPanel = false;
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
            return hasLocalPanel;
        }
#if !UNITY_EDITOR_LINUX && !UNITY_ANDROID && !UNITY_IOS
        private void OnPrintPage(object sender, PrintPageEventArgs ev)
        {   
                pdfPageCount++;
                ev.HasMorePages = DrawTable(ev.Graphics);          
        }


        bool DrawTable(System.Drawing.Graphics graph)
        {

            int rowHeight = 30;
            int rowCount = 33;
            int tableHeight = rowHeight * rowCount;
            int numOfItems = Mathf.Abs(data.Length - pdfLineCount);
            if(numOfItems < rowCount)
            {
                rowCount = numOfItems;
            }
            var image = new Bitmap(800, (rowHeight * rowCount)-10);
            int xOffset = 25;
            int yOFfset = 50;
            try 
            {
                graph.DrawString(this.name, new System.Drawing.Font("Arial", 16), Brushes.Black,new Point(xOffset,15));

                graph.FillRectangle(Brushes.White, new Rectangle(new Point(0 + xOffset, 0+ yOFfset), image.Size));
                int col = 0 + xOffset;
                int finalNumberofRows = 0;
                bool drewRectangle = false;
                for (int i = 0; i < 5; i++) 
                {
                    int nextColumn = 0;
                    string columnName = "ID";
                    graph.DrawLine(Pens.Black, new Point(col, 0 + yOFfset), new Point(col, image.Height + yOFfset));
                    switch(i)
                    {
                        default:
                            nextColumn +=50;
                            columnName = "ID";
                            break;
                        case 1:
                            nextColumn+=400;
                            columnName = "Fixture Name";
                            break;
                        case 2:
                            nextColumn+=200;
                            columnName = "Fixture Type";
                            break;
                        case 3:
                            nextColumn+=75;
                            columnName = "Universe";
                            break;
                        case 4:
                            nextColumn+=75;
                            columnName = "Address";
                            break;
                    }
                    finalNumberofRows = 0 + pdfLineCount;
                    
                    for (int row = 0 + yOFfset; row <= image.Height; row += rowHeight) 
                    {
                        
                        if(row == 0 + yOFfset)
                        {
                            if(drewRectangle == false)
                            {
                                graph.FillRectangle(Brushes.Gray, new Rectangle(new Point(col, row), new System.Drawing.Size(image.Width,rowHeight)));
                                drewRectangle = true;
                            }

                            graph.DrawLine(Pens.Black, new Point(col, row), new Point(col + nextColumn, row));
                            graph.DrawString(columnName, new System.Drawing.Font("Arial", 12), Brushes.Black,new Point(col+1,row+8));
                        }
                        else
                        {
                            if(finalNumberofRows < patchList.Rows.Count)
                            {
                                graph.DrawLine(Pens.Black, new Point(col, row), new Point(col + nextColumn, row));
                                graph.DrawString(patchList.Rows[finalNumberofRows][i].ToString(), new System.Drawing.Font("Arial", 12), Brushes.Black,new Point(col+1,row+8));
                                finalNumberofRows++;
                            }
                        }

                    }
                    col+= nextColumn;


                   // graph.DrawString(i.ToString(), new System.Drawing.Font("Arial", 16), Brushes.Black,new Point(col-25,5+yOFfset));
                }
                pdfLineCount+=finalNumberofRows;

                // for (int row = 0 + yOFfset; row < image.Height; row += rowHeight) {
                //     graph.DrawLine(Pens.Black, new Point(0 + xOffset, row), new Point(image.Width + xOffset, row));
                // }
                graph.DrawRectangle(Pens.Black, new Rectangle(0+ xOffset, 0 + yOFfset, image.Width - 1, image.Height - 1));
                graph.DrawString(pdfPageCount.ToString(), new System.Drawing.Font("Arial", 12), Brushes.Black,new Point((image.Width / 2) + xOffset ,tableHeight + yOFfset + 20));
            
            } 
            finally 
            {
                image.Dispose();
            }
            return pdfLineCount < data.ToArray().Length;
        }




        public void ToPDF()
        {
            if(CheckForLocalPanel() == false){return;}
            pdfLineCount = 0;
            pdfPageCount = 0;

            patchList = new DataTable("Patch List");
            DataColumn column;
            DataRow row;
            //ID Column
            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "ID";
            column.Caption = "Fixture ID";
            column.ReadOnly = false;
            column.Unique = false;
            patchList.Columns.Add(column);


            //Name Column
            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "Fixture Name";
            column.Caption = "Fixture Name";
            column.ReadOnly = false;
            column.Unique = false;
            patchList.Columns.Add(column);


            //Type Column
            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "Fixture Type";
            column.Caption = "Fixture Type";
            column.ReadOnly = false;
            column.Unique = false;
            patchList.Columns.Add(column);


            //Universe Column
            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "Universe";
            column.Caption = "Universe";
            column.ReadOnly = false;
            column.Unique = false;
            patchList.Columns.Add(column);

            //Address Column
            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "Address";
            column.Caption = "Address";
            column.ReadOnly = false;
            column.Unique = false;
            patchList.Columns.Add(column);


            // DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            // PrimaryKeyColumns[0] = patchList.Columns["id"];
            // patchList.PrimaryKey = PrimaryKeyColumns;

            DataSet dtSet = new DataSet();

            dtSet.Tables.Add(patchList);
            
            VRSL_FixtureDefinitions fixDefAsset = (VRSL_FixtureDefinitions) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureDefGUID), typeof(VRSL_FixtureDefinitions));
            string[] definitions = fixDefAsset.GetNames();

            DMXFixtureData[] sortedData = data.OrderBy(c => c.fixtureID).ToArray();
            
            foreach(DMXFixtureData f in sortedData)
            {
                row = patchList.NewRow();
                row["ID"] = f.fixtureID;
                row["Fixture Name"] = f.name;
                row["Fixture Type"] = definitions[f.fixtureDefintion];
                row["Universe"] = f.dmxUniverse;
                row["Address"] = f.dmxChannel;
                patchList.Rows.Add(row);
            }

            pdfLineCount = 0;
            PrintDocument document = new PrintDocument();
            document.PrintPage += new PrintPageEventHandler(OnPrintPage);
            document.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            document.Print();
            UnityEngine.Debug.Log("Sucessfully Exported PDF File"); 
        }
#endif
        public string ToJsonFile(bool refreshEditor)
        {
            if(CheckForLocalPanel())
            {
                try
                {
                    VRSL_FixtureDefinitions fixDefAsset = (VRSL_FixtureDefinitions) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureDefGUID), typeof(VRSL_FixtureDefinitions));
                    JSONDMXFixtureData[] jsonFixtureData = new JSONDMXFixtureData[data.Length];
                    string[] definitions = fixDefAsset.GetNames();
                    for(int i = 0; i < jsonFixtureData.Length; i++)
                    {
                        jsonFixtureData[i] = new JSONDMXFixtureData(data[i], definitions, fixDefAsset.GetChannelDefinition(data[i].fixtureDefintion));
                    }
                    JSONDMXFixtureData_Container jsonContainer = new JSONDMXFixtureData_Container(jsonFixtureData);
                    string json = JsonUtility.ToJson(jsonContainer, true);
                    string assetPath = AssetDatabase.GetAssetPath(this);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
                    string targetString = this.name + ".asset";
                    string newString = this.name + ".json";
                    filePath = filePath.Replace("/", "\\");
                    filePath = filePath.Replace(targetString, newString);
                    File.WriteAllText(filePath, json);
                    //UnityEngine.Debug.Log(json);
                    if(refreshEditor)
                    {
                        AssetDatabase.Refresh();
                    }
                    UnityEngine.Debug.Log("Sucessfully Exported JSON File");
                    return filePath;
                }
                catch
                {
                    UnityEngine.Debug.LogError("Failed to Export to JSON!");
                    return "FAILED";
                }
            }
            return "FAILED";
            
        }


        string ToXMLTransformMatrix(Vector3 Position, Vector3 Rotation)
        {
            UnityEngine.Matrix4x4 rotationMatrix = UnityEngine.Matrix4x4.Rotate(Quaternion.Euler(Rotation));
            Vector4 u = rotationMatrix.GetRow(0);
            Vector4 v = rotationMatrix.GetRow(1);
            Vector4 w = rotationMatrix.GetRow(2);
            //Vector4 o = matrix.GetRow(3);
            Vector3 u1 = new Vector3(u.x, u.y, u.z);
            Vector3 v1 = new Vector3(v.x, v.y, v.z);
            Vector3 w1 = new Vector3(w.x, w.y, w.z);
            Position = new Vector3(Position.x * 1000f, Position.y * 1000f, Position.z * 1000f);
            //Vector3 o1 = new Vector3(o.x, o.y, o.z);
            string output = u1.ToString("F6") + " " + v1.ToString("F6") + " " + w1.ToString("F6") + " " + Position.ToString("F6");
            output = output.Replace("(", "{");
            output = output.Replace(")", "}");
            output = output.Replace(" ", "");
            return output;
        }

        int GetAbsoluteDMXAddress(int universe, int channel)
        {
            return ((universe-1) * 512) + channel;
        }
        XmlElement GenerateFixtureElement(XmlDocument doc, JSONDMXFixtureData data)
        {
            XmlElement fixture = doc.CreateElement("Fixture");
            fixture.SetAttribute("uuid", System.Guid.NewGuid().ToString());
            fixture.SetAttribute("name", data.name);
            XmlElement matrix = doc.CreateElement("Matrix");
            matrix.InnerText = ToXMLTransformMatrix(data.position, data.rotation);
            XmlElement FixtureID = doc.CreateElement("FixtureID");
            FixtureID.InnerText = data.id.ToString();

            XmlElement Addresses = doc.CreateElement("Addresses");
            XmlElement Address = doc.CreateElement("Address");
            Address.InnerText = GetAbsoluteDMXAddress(data.universe, data.channel).ToString();
            Addresses.AppendChild(Address);

            XmlElement DMXInvertPan = doc.CreateElement("DMXInvertPan");
            DMXInvertPan.InnerText = data.invertPan.ToString();
            XmlElement DMXInvertTilt = doc.CreateElement("DMXInvertTilt");
            DMXInvertTilt.InnerText = data.invertTilt.ToString();


            XmlElement UnitNumber = doc.CreateElement("UnitNumber");
            UnitNumber.InnerText = "0";

            XmlElement GDTFSpec = doc.CreateElement("GDTFSpec");
            GDTFSpec.InnerText = data.fixtureDefintion + ".gdtf";

            XmlElement GDTFMode = doc.CreateElement("GDTFMode");
            GDTFMode.InnerText = "Default";

            fixture.AppendChild(matrix);
            fixture.AppendChild(FixtureID);
            fixture.AppendChild(GDTFSpec);
            fixture.AppendChild(GDTFMode);
            fixture.AppendChild(UnitNumber);
            fixture.AppendChild(Addresses);
            fixture.AppendChild(DMXInvertPan);
            fixture.AppendChild(DMXInvertTilt);
            


            return fixture;
        }


        string GeneralSceneDescriptionXML(JSONDMXFixtureData[] data)
        {
            string output = "";
            try
            {
            // output += "\n" + @"<GeneralSceneDescription verMajor=""1"" verMinor=""6"" provider=""VR Stage Lighting"" providerVersion""1"">";
            // output += "\n";
            // output += "\n" + " <UserData/>";
            XmlDocument xdoc = new XmlDocument();
            XmlNode docNode = xdoc.CreateXmlDeclaration("1.0", null, "no");
            xdoc.AppendChild(docNode);


           // UnityEngine.Debug.Log("Creating XML Document");
            //xdoc.LoadXml(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
           // UnityEngine.Debug.Log("Loading XML Document");
            XmlElement GeneralSceneDescription = xdoc.CreateElement("GeneralSceneDescription");
          //  UnityEngine.Debug.Log("Generating Scene Description Node");
            GeneralSceneDescription.SetAttribute("verMajor", "1");
            GeneralSceneDescription.SetAttribute("verMinor", "6");
            GeneralSceneDescription.SetAttribute("provider", "VR Stage Lighting");
            GeneralSceneDescription.SetAttribute("providerVersion", "1");
           // UnityEngine.Debug.Log("Finished setting attributes for General Scene Description");

           
           XmlElement UserData = xdoc.CreateElement("UserData");
           XmlElement Scene = xdoc.CreateElement("Scene");
           XmlElement Layers = xdoc.CreateElement("Layers");
           XmlElement Layer = xdoc.CreateElement("Layer");

           Layer.SetAttribute("name", "VRSL Main");
           Layer.SetAttribute("uuid", System.Guid.NewGuid().ToString());
           XmlElement ChildList = xdoc.CreateElement("ChildList");
        
            foreach(JSONDMXFixtureData fixture in data)
            {
                XmlElement f = GenerateFixtureElement(xdoc, fixture);
                ChildList.AppendChild(f);
            }
           Layer.AppendChild(ChildList);
           Layers.AppendChild(Layer);
           Scene.AppendChild(Layers);

        
           GeneralSceneDescription.AppendChild(UserData);
           GeneralSceneDescription.AppendChild(Scene);
           
           
           
           
           
           
           
           
           
            xdoc.AppendChild(GeneralSceneDescription);
            UnityEngine.Debug.Log("Appending Scene Description");





            // var buffer = new StringBuilder();
            // var writer = XmlWriter.Create(buffer, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8});
            // xdoc.Save(writer);
            // writer.Close();

            // output = buffer.ToString();
            //output = ToEncoding(xdoc,Encoding.UTF8);

                var sb = new StringBuilder();
                var sw = new StringWriterUtf8(sb);
                xdoc.Save(sw);
                output = sb.ToString();
            }
            catch(Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                UnityEngine.Debug.LogError("Error At Line: " + line.ToString());
                throw;
            }

            //string output = "";
            //doc.Save
            return output;
        }

        bool CapContains(string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        string FixtureDefinitionXML(string name, string[] channels)
        {
            string output = "";
            try
            {
                XmlDocument xdoc = new XmlDocument();
                XmlNode docNode = xdoc.CreateXmlDeclaration("1.0", null, "no");
                xdoc.AppendChild(docNode);

                XmlElement GDTF = xdoc.CreateElement("GDTF");
                GDTF.SetAttribute("DataVersion", "1.1");
                XmlElement FixtureType = xdoc.CreateElement("FixtureType");
                FixtureType.SetAttribute("Name", name);
                FixtureType.SetAttribute("ShortName", name);
                FixtureType.SetAttribute("LongName", name);
                FixtureType.SetAttribute("Manufacturer", "VR Stage Lighting");
                FixtureType.SetAttribute("FixtureTypeID", System.Guid.NewGuid().ToString());
                FixtureType.SetAttribute("RefFT", "");
                
                XmlElement AttributeDefinitions = xdoc.CreateElement("AttributeDefinitions");
                XmlElement FeatureGroups = xdoc.CreateElement("FeatureGroups");
                bool hasMovement = false;
                bool hasRGB = false;
                bool hasDimmer = false;
                bool hasStrobe = false;
                bool hasZoom = false;
                bool hasGobo = false;
                bool hasLaserBeam = false;
                bool hasFog = false;
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                for(int i = 0; i < channels.Length; i++)
                {
                    string c = channels[i];
                    if(!hasMovement)
                    {
                        if(CapContains(c,"pan",comp) || CapContains(c,"tilt",comp) || CapContains(c,"movement",comp) || CapContains(c,"rotate",comp))
                        {
                            hasMovement = true;
                            continue;
                        }
                    }
                    if(!hasRGB)
                    {
                        if(CapContains(c,"red",comp) || CapContains(c,"green",comp) || CapContains(c,"blue",comp) || CapContains(c,"gradient",comp) || CapContains(c,"colour",comp) || CapContains(c,"color",comp))
                        {
                            hasRGB = true;
                            continue;
                        }
                    }
                    if(!hasDimmer)
                    {
                        if(CapContains(c,"dimmer",comp) || CapContains(c,"intensity",comp) || CapContains(c,"power",comp))
                        {
                            hasDimmer = true;
                            continue;
                        }
                    }
                    if(!hasStrobe)
                    {
                        if(CapContains(c,"strobe",comp) || CapContains(c,"shutter",comp) || CapContains(c,"flash",comp))
                        {
                            hasStrobe = true;
                            continue;
                        }
                    }
                    if(!hasZoom)
                    {
                        if(CapContains(c,"zoom",comp) || CapContains(c,"focus",comp) || CapContains(c,"cone",comp))
                        {
                            hasZoom = true;
                            continue;
                        }
                    }
                    if(!hasGobo)
                    {
                        if(CapContains(c,"gobo",comp) || CapContains(c,"cookie",comp))
                        {
                            hasGobo = true;
                            continue;
                        }
                    }
                    if(!hasLaserBeam)
                    {
                        if(CapContains(c,"laser",comp) || CapContains(c,"beam",comp))
                        {
                            hasLaserBeam = true;
                            continue;
                        }
                    }
                    if(!hasFog)
                    {
                        if(CapContains(c,"fog",comp) || CapContains(c,"c02",comp))
                        {
                            hasFog = true;
                            continue;
                        }
                    }
                }

                if(hasMovement)
                {
                    XmlElement FeatureGroup_Movement = xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Movement.SetAttribute("Name", "Position");
                    FeatureGroup_Movement.SetAttribute("Pretty", "Position");
                    XmlElement Feature_PT = xdoc.CreateElement("Feature");    
                    Feature_PT.SetAttribute("Name", "PanTilt");

                    XmlElement Feature_Rotate = xdoc.CreateElement("Feature");    
                    Feature_Rotate.SetAttribute("Name", "Rotate");

                    FeatureGroup_Movement.AppendChild(Feature_PT);
                    FeatureGroup_Movement.AppendChild(Feature_Rotate);

                    FeatureGroups.AppendChild(FeatureGroup_Movement);
                }
                if(hasRGB)
                {
                    XmlElement FeatureGroup_RGB = xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_RGB.SetAttribute("Name", "Color");
                    FeatureGroup_RGB.SetAttribute("Pretty", "Color");        

                    XmlElement Feature_RGB = xdoc.CreateElement("Feature");    
                    Feature_RGB.SetAttribute("Name", "RGB");
                    XmlElement Feature_Color = xdoc.CreateElement("Feature");    
                    Feature_Color.SetAttribute("Name", "Color");
                    FeatureGroup_RGB.AppendChild(Feature_RGB);
                    FeatureGroup_RGB.AppendChild(Feature_Color);   

                    FeatureGroups.AppendChild(FeatureGroup_RGB);      
                }
                if(hasDimmer)
                {
                    XmlElement FeatureGroup_Dimmer = xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Dimmer.SetAttribute("Name", "Dimmer");
                    FeatureGroup_Dimmer.SetAttribute("Pretty", "Dimmer");  

                    XmlElement Feature_Dimmer = xdoc.CreateElement("Feature");    
                    Feature_Dimmer.SetAttribute("Name", "Dimmer");
                    FeatureGroup_Dimmer.AppendChild(Feature_Dimmer); 

                    FeatureGroups.AppendChild(FeatureGroup_Dimmer);                      
                }
                if(hasStrobe)
                {
                    XmlElement FeatureGroup_Strobe = xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Strobe.SetAttribute("Name", "Shutter");
                    FeatureGroup_Strobe.SetAttribute("Pretty", "Shutter");     

                    XmlElement Feature_Strobe = xdoc.CreateElement("Feature");    
                    Feature_Strobe.SetAttribute("Name", "Shutter");
                    FeatureGroup_Strobe.AppendChild(Feature_Strobe); 

                    FeatureGroups.AppendChild(FeatureGroup_Strobe);                          
                }
                if(hasZoom)
                {
                    XmlElement FeatureGroup_Zoom = xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Zoom.SetAttribute("Name", "Focus");
                    FeatureGroup_Zoom.SetAttribute("Pretty", "Focus");

                    XmlElement Feature_Zoom = xdoc.CreateElement("Feature");    
                    Feature_Zoom.SetAttribute("Name", "Focus");
                    FeatureGroup_Zoom.AppendChild(Feature_Zoom);  

                    FeatureGroups.AppendChild(FeatureGroup_Zoom);                            
                }
                if(hasGobo)
                {
                    XmlElement FeatureGroup_Gobo= xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Gobo.SetAttribute("Name", "Gobo");
                    FeatureGroup_Gobo.SetAttribute("Pretty", "Gobo");     

                    XmlElement Feature_Gobo = xdoc.CreateElement("Feature");    
                    Feature_Gobo.SetAttribute("Name", "Gobo");
                    FeatureGroup_Gobo.AppendChild(Feature_Gobo);   

                    FeatureGroups.AppendChild(FeatureGroup_Gobo);                       
                }
                if(hasLaserBeam)
                {
                    XmlElement FeatureGroup_Laser= xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Laser.SetAttribute("Name", "Laser");
                    FeatureGroup_Laser.SetAttribute("Pretty", "Laser"); 

                    XmlElement Feature_Laser = xdoc.CreateElement("Feature");    
                    Feature_Laser.SetAttribute("Name", "Laser");
                    FeatureGroup_Laser.AppendChild(Feature_Laser);

                    FeatureGroups.AppendChild(FeatureGroup_Laser);  
                
                }
                if(hasFog)
                {
                    XmlElement FeatureGroup_Fog = xdoc.CreateElement("FeatureGroup");
                    FeatureGroup_Fog.SetAttribute("Name", "Fog");
                    FeatureGroup_Fog.SetAttribute("Pretty", "Fog");     

                    XmlElement Feature_Fog = xdoc.CreateElement("Feature");    
                    Feature_Fog.SetAttribute("Name", "Fog");
                    FeatureGroup_Fog.AppendChild(Feature_Fog);  

                    FeatureGroups.AppendChild(FeatureGroup_Fog);               
                }

                XmlElement FeatureGroup_Control = xdoc.CreateElement("FeatureGroup");
                FeatureGroup_Control.SetAttribute("Name", "Control");
                FeatureGroup_Control.SetAttribute("Pretty", "Control");
                XmlElement Feature_Control = xdoc.CreateElement("Feature");    
                Feature_Control.SetAttribute("Name", "Control");
                FeatureGroup_Control.AppendChild(Feature_Control);

                FeatureGroups.AppendChild(FeatureGroup_Control);   

                XmlElement FeatureGroup_Other = xdoc.CreateElement("FeatureGroup");
                FeatureGroup_Other.SetAttribute("Name", "FeatureGroup");
                FeatureGroup_Other.SetAttribute("Pretty", "FeatureGroup"); 
                XmlElement Feature_Other = xdoc.CreateElement("Feature");    
                Feature_Other.SetAttribute("Name", "Feature");
                FeatureGroup_Other.AppendChild(Feature_Other);

                FeatureGroups.AppendChild(FeatureGroup_Other);   

                int numOfGobos = 8;






                AttributeDefinitions.AppendChild(FeatureGroups);

//////////////////////////////////////////////////////////////////////
                XmlElement Attributes = xdoc.CreateElement("Attributes");
                bool hasFinePan = false;
                int panChan = 0;
                int finePanChan = 0;
                bool hasFineTilt = false;
                int tiltChan = 0;
                int fineTiltChan = 0;
                bool hasFineRotate = false;
                int rotateChan = 0;
                int fineRotateChan = 0;
                for(int i = 0; i < channels.Length; i++)
                {
                    string c = channels[i];
                    if(CapContains(c,"pan",comp) || CapContains(c,"tilt",comp) || CapContains(c,"rotate",comp))
                    {
                        if(CapContains(c,"pan",comp))
                        {
                            if(CapContains(c,"fine",comp))
                            {
                                hasFinePan = true; 
                                finePanChan = i; 
                                continue;
                            }
                            else
                            {
                                panChan = i;
                            }
                        }
                        if(CapContains(c,"tilt",comp))
                        {
                            if(CapContains(c,"fine",comp))
                            {
                                hasFineTilt = true; 
                                fineTiltChan = i; 
                                continue;
                            }
                            else
                            {
                                tiltChan = i;
                            }
                        }
                        if(CapContains(c,"rotate",comp))
                        {
                            if(CapContains(c,"fine",comp))
                            {
                                hasFineRotate = true; 
                                fineRotateChan = i; 
                                continue;
                            }
                            else
                            {
                                rotateChan = i; 
                            }
                        }
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "Angle");
                        if(CapContains(c,"pan",comp) || CapContains(c,"tilt",comp))
                        {
                            Attribute.SetAttribute("Feature", "Position.PanTilt");
                        }
                        else
                        {
                            Attribute.SetAttribute("Feature", "Position.Rotate");
                        }
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"red",comp) || CapContains(c,"green",comp) || CapContains(c,"blue",comp) || CapContains(c,"gradient",comp) || CapContains(c,"colour",comp) || CapContains(c,"color",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        if(CapContains(c,"red",comp) || CapContains(c,"green",comp) || CapContains(c,"blue",comp))
                        {
                            Attribute.SetAttribute("Feature", "Color.RGB");
                        }
                        else
                        {
                            Attribute.SetAttribute("Feature", "Color.Color");
                        }
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"dimmer",comp) || CapContains(c,"intensity",comp) || CapContains(c,"power",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Dimmer.Dimmer");
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"strobe",comp) || CapContains(c,"shutter",comp) || CapContains(c,"flash",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Shutter.Shutter");
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"zoom",comp) || CapContains(c,"focus",comp) || CapContains(c,"cone",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Focus.Focus");
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"gobo",comp) || CapContains(c,"cookie",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Gobo.Gobo");
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"laser",comp) || CapContains(c,"beam",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Laser.Laser");
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else if(CapContains(c,"fog",comp) || CapContains(c,"c02",comp))
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Fog.Fog");
                        Attributes.AppendChild(Attribute);
                        continue;
                    }
                    else
                    {
                        XmlElement Attribute = xdoc.CreateElement("Attribute");
                        Attribute.SetAttribute("Name", c);
                        Attribute.SetAttribute("Pretty", c);
                        Attribute.SetAttribute("PhysicalUnit", "None");
                        Attribute.SetAttribute("Feature", "Control.Control");
                        Attributes.AppendChild(Attribute);
                        continue;                        
                    }
                }
                AttributeDefinitions.AppendChild(Attributes);
//////////////////////////////////////////////////////////////////////



                XmlElement DMXModes = xdoc.CreateElement("DMXModes");

                XmlElement Models = xdoc.CreateElement("Models");
                XmlElement Model = xdoc.CreateElement("Model");
                Model.SetAttribute("Name", "Base");
                Models.AppendChild(Model);

                XmlElement Geometries = xdoc.CreateElement("Geometries");
                XmlElement Geometry = xdoc.CreateElement("Geometry");
                Geometry.SetAttribute("Model", "Base");
                Geometry.SetAttribute("Name", "Base");
                Geometry.SetAttribute("Position", "{1,0,0,0}{0,1,0,0}{0,0,1,0}{0,0,0,1}");

                Geometries.AppendChild(Geometry);

                FixtureType.AppendChild(AttributeDefinitions);

                if(hasGobo)
                {
                    XmlElement Wheels = xdoc.CreateElement("Wheels");
                    XmlElement GoboWheel = xdoc.CreateElement("Wheel");
                    GoboWheel.SetAttribute("Name", "GoboWheel");
                    string slotName = "Open";
                    for(int i = 0; i < numOfGobos; i++)
                    {
                        XmlElement Slot = xdoc.CreateElement("Slot");
                        if(i > 0)
                        {
                            slotName = "Gobo " + i;
                        }
                        Slot.SetAttribute("Name", slotName);
                        GoboWheel.AppendChild(Slot);  
                    }
                    Wheels.AppendChild(GoboWheel);  
                    FixtureType.AppendChild(Wheels);
                }


                FixtureType.AppendChild(Models);
                FixtureType.AppendChild(Geometries);






                XmlElement DMXMode = xdoc.CreateElement("DMXMode");
                DMXMode.SetAttribute("Description", "Default Channel Mode");
                DMXMode.SetAttribute("Geometry", "Base");
                DMXMode.SetAttribute("Name", "Default");

                XmlElement DMXChannels = xdoc.CreateElement("DMXChannels");






                //int fineOffset = 0;

                for(int i = 0; i < channels.Length; i++)
                {
                    string highlight = "None";
                    string c = channels[i];
                    int chan = (i + 1);
                    bool fineCheck = false;
                    XmlElement DMXChannel = xdoc.CreateElement("DMXChannel");
                    if(CapContains(c,"pan",comp))
                    {
                        if(CapContains(c,"fine",comp)){continue;}
                        DMXChannel.SetAttribute("DMXBreak", "1");
                        DMXChannel.SetAttribute("Geometry", "Base");
                        if(hasFinePan)
                        {
                            int offset = Mathf.Abs(finePanChan - panChan);
                            offset = offset * ( finePanChan < panChan ? -1 : 1);
                            DMXChannel.SetAttribute("Offset", (chan).ToString() + "," + (chan + offset).ToString());
                            //fineOffset++;
                            fineCheck = true;
                        }
                        else
                        {
                            DMXChannel.SetAttribute("Offset", (chan).ToString());
                        }
                    }
                    else if(CapContains(c,"tilt",comp))
                    {
                        if(CapContains(c,"fine",comp)){continue;}
                        DMXChannel.SetAttribute("DMXBreak", "1");
                        DMXChannel.SetAttribute("Geometry", "Base");
                        if(hasFineTilt)
                        {
                            int offset = Mathf.Abs(fineTiltChan - tiltChan);
                            offset = offset * ( fineTiltChan < tiltChan ? -1 : 1);
                            DMXChannel.SetAttribute("Offset", (chan).ToString() + "," + (chan + offset).ToString());
                           // fineOffset++;
                            fineCheck = true;
                        }
                        else
                        {
                            DMXChannel.SetAttribute("Offset", (chan).ToString());
                        }
                    }
                    else if(CapContains(c,"rotate",comp))
                    {
                        if(CapContains(c,"fine",comp)){continue;}
                        DMXChannel.SetAttribute("DMXBreak", "1");
                        DMXChannel.SetAttribute("Geometry", "Base");
                        if(hasFineRotate)
                        {
                            int offset = Mathf.Abs(fineRotateChan - rotateChan);
                            offset = offset * ( fineRotateChan < rotateChan ? -1 : 1);
                            DMXChannel.SetAttribute("Offset", (chan).ToString() + "," + (chan + offset).ToString());
                            //fineOffset++;
                            fineCheck = true;
                        }
                        else
                        {
                            DMXChannel.SetAttribute("Offset", (chan).ToString());
                        }
                    }
                    else
                    {
                        DMXChannel.SetAttribute("DMXBreak", "1");
                        DMXChannel.SetAttribute("Geometry", "Base");
                        DMXChannel.SetAttribute("Offset", (chan).ToString());
                    }
                    if(CapContains(c,"red",comp) || CapContains(c,"green",comp) || CapContains(c,"blue",comp) || CapContains(c,"dim",comp) || CapContains(c,"dimmer",comp) || CapContains(c,"intensity",comp))
                    {
                        highlight = "255/1";
                    }
                    string master = "None";
                    if(CapContains(c,"dim",comp) || CapContains(c,"dimmer",comp))
                    {
                        master = "Grand";
                    }
                    string dmxFrom =  fineCheck ? "0/2" : "0/1";
                    DMXChannel.SetAttribute("Highlight", highlight);
                    
                    XmlElement LogicalChannel = xdoc.CreateElement("LogicalChannel");
                    LogicalChannel.SetAttribute("Attribute", c);
                    LogicalChannel.SetAttribute("Master", master);

                    XmlElement ChannelFunction = xdoc.CreateElement("ChannelFunction");
                    ChannelFunction.SetAttribute("Attribute", c);
                    ChannelFunction.SetAttribute("DMXFrom", dmxFrom);
                    ChannelFunction.SetAttribute("Name", c);
                    if((CapContains(c,"gobo",comp) || CapContains(c,"cookie",comp)) && CapContains(c,"select",comp))
                    {   
                       ChannelFunction.SetAttribute("Wheel", "GoboWheel");
                       string goboName = "Open";
                       int fromVal = 0;
                       for(int j = 0; j < numOfGobos; j++)
                       {
                            XmlElement ChannelSet = xdoc.CreateElement("ChannelSet"); 
                            if(j > 0)
                            {
                                goboName = "Gobo " + j.ToString();
                                fromVal = fromVal + Mathf.FloorToInt(255 / numOfGobos);
                            }
                            ChannelSet.SetAttribute("Name", goboName);
                            ChannelSet.SetAttribute("DMXFrom", fromVal.ToString() + "/1");
                            ChannelSet.SetAttribute("PhysicalFrom", "0");
                            ChannelSet.SetAttribute("PhysicalTo", "0");
                            ChannelSet.SetAttribute("WheelSlotIndex", (j+1).ToString());
                            ChannelFunction.AppendChild(ChannelSet);
                       }
                    }

                    LogicalChannel.AppendChild(ChannelFunction);
                    DMXChannel.AppendChild(LogicalChannel);
                    DMXChannels.AppendChild(DMXChannel);
                }






                DMXMode.AppendChild(DMXChannels);
                DMXModes.AppendChild(DMXMode);
                FixtureType.AppendChild(DMXModes);
                GDTF.AppendChild(FixtureType);
                xdoc.AppendChild(GDTF);

                // var buffer = new StringBuilder();
                // var writer = XmlWriter.Create(buffer, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8});
                // xdoc.Save(writer);
                // writer.Close();

                

               // output = ToEncoding(xdoc,Encoding.UTF8);
                var sb = new StringBuilder();
                var sw = new StringWriterUtf8(sb);
                xdoc.Save(sw);
                output = sb.ToString();
            }
            catch(Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                UnityEngine.Debug.LogError("Error At Line: " + line.ToString());
                throw;
            }
            return output;
        }
        
        public void ToMVRFile()
        {
            if(CheckForLocalPanel())
            {

                VRSL_FixtureDefinitions fixDefAsset = (VRSL_FixtureDefinitions) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(panel.fixtureDefGUID), typeof(VRSL_FixtureDefinitions));
                JSONDMXFixtureData[] jsonFixtureData = new JSONDMXFixtureData[data.Length];
                string[] definitions = fixDefAsset.GetNames();
                for(int i = 0; i < jsonFixtureData.Length; i++)
                {
                    jsonFixtureData[i] = new JSONDMXFixtureData(data[i], definitions, fixDefAsset.GetChannelDefinition(data[i].fixtureDefintion));
                }


                string assetPath = AssetDatabase.GetAssetPath(this);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
                string targetString = this.name + ".asset";
                string newString = this.name + ".mvr";   
                filePath = filePath.Replace("/", "\\");
                filePath = filePath.Replace(targetString, newString);    
                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                    {
                            //CREATE SCENE DESCRIPTION ////////////////////////////////////////
                            var sceneDescriptionBytes = Encoding.ASCII.GetBytes(GeneralSceneDescriptionXML(jsonFixtureData));
                            var sceneDescriptionName = "GeneralSceneDescription.xml";
                            var sceneDescriptionEntry = archive.CreateEntry(sceneDescriptionName, System.IO.Compression.CompressionLevel.NoCompression);
                            using (var zipStream = sceneDescriptionEntry.Open())
                            {
                                zipStream.Write(sceneDescriptionBytes, 0, sceneDescriptionBytes.Length);
                            }
                            /////////////////////////////////////////////////////////////////
                            for(int i = 0; i < fixDefAsset.definitions.Length; i++)
                            {
                                var defName = fixDefAsset.definitions[i].name + ".gdtf";
                                var fixDefEntry = archive.CreateEntry(defName, System.IO.Compression.CompressionLevel.NoCompression);




                                
                                using (var memoryStream = new MemoryStream())
                                {
                                    using (var defArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                                    {
                                        var descriptionBytes = Encoding.ASCII.GetBytes(FixtureDefinitionXML(fixDefAsset.definitions[i].name, fixDefAsset.definitions[i].channelNames));
                                        var descriptionEntry = defArchive.CreateEntry("description.xml", System.IO.Compression.CompressionLevel.NoCompression);
                                        using (var fixDefZipStream = descriptionEntry.Open())
                                        {
                                            fixDefZipStream.Write(descriptionBytes, 0, descriptionBytes.Length);
                                        } 
                                        using (var zipStream = fixDefEntry.Open())
                                        {
                                            defArchive.Dispose();
                                            zipStream.Write(memoryStream.ToArray(), 0, memoryStream.ToArray().Length);
                                        }
                                    }

                                    memoryStream.Dispose();
                                }
                            }
                            

                    }
                }
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Sucessfully Exported MVR File");         
            }
        }
    }

    [CustomEditor(typeof(VRSL_DMXPatchSettings))]
    public class VRSL_DMXPatchSettings_Editor: Editor 
    {
        private SerializedProperty data, idStrings, targetScene, scenePath;
        SceneAsset sceneAsset;
        VRSL_DMXPatchSettings settings = null;
        private void OnEnable()
        {
            // // Link the properties
            // data = serializedObject.FindProperty("data");
            // idStrings = serializedObject.FindProperty("idStrings");
            // targetScene = serializedObject.FindProperty("targetScene");
            // scenePath = serializedObject.FindProperty("scenePath");
            
            settings = (VRSL_DMXPatchSettings) target;
            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(settings.scenePath);
        }

        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();
            SerializedObject so = new SerializedObject(settings);
            // Load the real class values into the serialized copy
            so.Update();
            if(settings != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Target Scene", sceneAsset, typeof(SceneAsset), false);
                EditorGUI.EndDisabledGroup();
                // if(GUILayout.Button("Check Data Status"))
                // {
                //     settings.CheckData();
                // }
                if(GUILayout.Button("Save Scene DMX Patch Data"))
                {
                    settings.SetDMXFixtureData();
                    settings.ForceSave();
                 //   EditorUtility.SetDirty(settings);
                 //   Undo.RecordObject(settings, "Undo Save Scene DXM Patch Data");

                }
                if(GUILayout.Button("Load Scene DMX Patch Data"))
                {
                    settings.LoadDMXFixtureData();
                }
                if(GUILayout.Button("Export To JSON File"))
                {
                    settings.ToJsonFile(true);
                }
                if(GUILayout.Button("Export To MVR File"))
                {
                    settings.ToMVRFile();
                }
                if(GUILayout.Button("Export To PDF File (Windows)"))
                {
#if !UNITY_EDITOR_LINUX && !UNITY_ANDROID  && !UNITY_IOS
                    settings.ToPDF();
#else
                    EditorUtility.DisplayDialog("PDF export error", "PDF export is currently a Windows only feature", "OK", "Cancel");
#endif
                }
                if(settings.data != null)
                {
                    for(int i = 0; i < settings.data.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        EditorGUILayout.LabelField(settings.data[i].name);
                        EditorGUILayout.LabelField("DMX Universe: " + settings.data[i].dmxUniverse, GUILayout.Width(100f));
                        EditorGUILayout.LabelField("DMX Channel: " + settings.data[i].dmxChannel);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            // Write back changed values and evtl mark as dirty and handle undo/redo
            so.ApplyModifiedProperties();
        }
    }
}
#endif
