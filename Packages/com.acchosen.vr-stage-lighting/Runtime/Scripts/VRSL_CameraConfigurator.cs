
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif



namespace VRSL.EditorScripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class VRSL_CameraConfigurator : UdonSharpBehaviour
    {
        public Camera camObj;
        public float defaultSize = 9.6f;
        public float vertDefaultSize = 5.4f;
        public float percentageReduction = 0.6667f;
        public float percentageFurtherReduction = 0.4444f;



        public const float vertXMin1080p = -4.427f;
        public const float vertXMax1080p  = 4.478f;
        public const float vertXMin720p = -4.6189f;
        public const float vertXMax720p = 4.659f;
        public const float vertXMin480p= -4.7393f;
        public const float vertXMax480p= 4.7789f;
        public const float horizXMin1080p = 0.02f;
        public const float horizXMax1080p = 0.02f;
        public const float horizXMin720p= -1.648f;
        public const float horizXMax720p = 1.684f;
        public const float horizXMin480p = -2.755f;
        public const float horizXMax480p = 2.798f;


        
        public const float vertYMin1080p = 0.25f;
        public const float vertYMax1080p  = 0.25f;
        public const float vertYMin720p = -1.416f;
        public const float vertYMax720p = 1.916f;
        public const float vertYMin480p= -2.528f;
        public const float vertYMax480p= 3.028f;
        public const float horizYMin1080p = -3.79f;
        public const float horizYMax1080p = 4.3f;
        public const float horizYMin720p= -4.108f;
        public const float horizYMax720p = 4.606f;
        public const float horizYMin480p = -4.322f;
        public const float horizYMax480p = 4.822f;








        public const int TEN80p = 0;
        public const int SEVEN20p = 1;
        public const int FOUR80p = 2;

        [SerializeField, FieldChangeCallback(nameof(YPos))]
        private float yPos = -3.79f;
        public float YPos
        {
            get => yPos;
            set
            {
                yPos = value;
                _UpdateCameraPosition();
            }
        }
        [SerializeField, FieldChangeCallback(nameof(XPos))]
        private float xPos = 0.02f;
        public float XPos
        {
            get => xPos;
            set
            {
                xPos = value;
                _UpdateCameraPosition();
            }
        }

        [SerializeField, FieldChangeCallback(nameof(IsHorizontal))]
        private bool isHorizontal = true;

        public bool IsHorizontal{
            get => isHorizontal;
            set
            {
                isHorizontal = value;
                xPos = 0.0f;
                _UpdateCameraPosition();
            }
        }

        [SerializeField, FieldChangeCallback(nameof(Resolution))]
        private int resolution = 0;

        public int Resolution{
            get => resolution;
            set
            {
                resolution = value;
                _UpdateCameraPosition();
            }
        }

        public int scaleDefine;
        void Start()
        {
            _TryGetCam();
        }
        public void _UpdateCameraPosition()
        {

            if(IsHorizontal)
            {
                switch(resolution)
                {
                    case TEN80p:
                        yPos = Mathf.Clamp(yPos, horizYMin1080p, horizYMax1080p);
                        xPos = Mathf.Clamp(xPos, horizXMin1080p,horizXMax1080p);
                        break;
                    case SEVEN20p:
                        yPos = Mathf.Clamp(yPos, horizYMin720p,horizYMax720p);
                        xPos = Mathf.Clamp(xPos, horizXMin720p,horizXMax720p);
                        break;
                    case FOUR80p:
                        yPos = Mathf.Clamp(yPos, horizYMin480p, horizYMax480p);
                        xPos = Mathf.Clamp(xPos, horizXMin480p,horizXMax480p);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch(resolution)
                {
                    case TEN80p:
                        yPos = Mathf.Clamp(yPos, vertYMin1080p, vertYMax1080p);
                        xPos = Mathf.Clamp(xPos,vertXMin1080p ,vertXMax1080p);
                        break;
                    case SEVEN20p:
                        yPos = Mathf.Clamp(yPos, vertYMin720p, vertYMax720p);
                        xPos = Mathf.Clamp(xPos,vertXMin720p ,vertXMax720p);
                        break;
                    case FOUR80p:
                        yPos = Mathf.Clamp(yPos, vertYMin480p, vertYMax480p);
                        xPos = Mathf.Clamp(xPos,vertXMin480p ,vertXMax480p);
                        break;
                    default:
                        break;
                }
            }

            camObj.transform.localPosition = new Vector3(xPos,yPos, camObj.transform.localPosition.z);
        }
        public void _TryGetCam()
        {
            if(camObj == null)
            {
                camObj = GetComponent<Camera>();
            }
        }
    }











    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(VRSL_CameraConfigurator))]
    public class VRSL_CameraConfigurator_Editor : Editor
    {
    public static void DrawLogo(Rect r, int displayHeight, int displayWidth)
    {
        var logo = Resources.Load("VRStageLighting-Logo") as Texture;
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
        Vector2 contentOffset = new Vector2(0f, -2f);
        GUIStyle style = new GUIStyle();
        style.fixedHeight = 170f;
        style.contentOffset = contentOffset;
        r.x += 400f;
        r.y += 20f;
        GUI.Box(r, logo,style);

        r.y +=40f;
        r.x +=40f;
        EditorGUI.LabelField(r, "DMX Camera Position Configurator");
        r.x-=150f;
        r.y+=15f;
        EditorGUI.LabelField(r, "Width: " + displayWidth + "px");
        r.y+=15f;
        EditorGUI.LabelField(r, "Height: " + displayHeight + "px");

    }
        public string[] resolutionSelection = {"1080p", "720p", "480p"};
        
        static GUIContent Label(string label, string tooltip)
        {
            return new GUIContent(label, tooltip);
        }
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            serializedObject.Update();

            
            VRSL_CameraConfigurator camConfig = (VRSL_CameraConfigurator)target;
            EditorGUI.BeginChangeCheck();
            if(camConfig.camObj == null)
            {
                camConfig._TryGetCam();
            }
            serializedObject.FindProperty("camObj").objectReferenceValue = EditorGUILayout.ObjectField(Label("DMX Camera", "The DMX Camera this script is attached to."), camConfig.camObj, typeof(Camera), true);
            serializedObject.FindProperty("isHorizontal").boolValue = EditorGUILayout.ToggleLeft("Is Horizontal?", camConfig.IsHorizontal);
            serializedObject.FindProperty("resolution").intValue = GUILayout.SelectionGrid(serializedObject.FindProperty("resolution").intValue,resolutionSelection,4,"toggle");
            //serializedObject.FindProperty("is720p").boolValue = EditorGUILayout.ToggleLeft("Is 720p? (Smaller Grid)", camConfig.Is720p);
            GUIStyle style = new GUIStyle(EditorStyles.label);
            var rect = GUILayoutUtility.GetRect(50f, 220f, style);
            var oldRect = rect;
            GUI.Box(rect, "");


            Texture graph = Resources.Load("notscientificgraph") as Texture;
            
            GUIStyle imagestyle = new GUIStyle(EditorStyles.label);
            imagestyle.fixedHeight = 161f;
            //style.fixedWidth = 300;
            //style.contentOffset = contentOffset;
            //imagestyle.alignment = TextAnchor.LowerLeft;
            GUI.Box(rect, graph,imagestyle);

            float xMin = VRSL_CameraConfigurator.horizXMin1080p;
            float xMax = VRSL_CameraConfigurator.horizXMax1080p;

            float yMin = VRSL_CameraConfigurator.horizYMin1080p;
            float yMax = VRSL_CameraConfigurator.horizYMax1080p;

            float fh = 31f;
            float yOffset = 68f;
            float xOffset = 0.0f;
            float yMultiplier = 16.0f;
            float xMultiplier = 1.0f;
            float widthOffset = 15f;

            int displayWidth = 1920;
            int displayHeight = 208;
            Texture horizDummy;
            if(serializedObject.FindProperty("isHorizontal").boolValue)
            {
                horizDummy = Resources.Load("horizGridDummy") as Texture;
                if(serializedObject.FindProperty("resolution").intValue == VRSL_CameraConfigurator.SEVEN20p)
                {
                    //HORIZONTAL 720P
                    xMin = VRSL_CameraConfigurator.horizXMin720p;
                    xMax = VRSL_CameraConfigurator.horizXMax720p;
                    yMin = VRSL_CameraConfigurator.horizYMin720p;
                    yMax = VRSL_CameraConfigurator.horizYMax720p;


                    fh = fh * camConfig.percentageReduction;
                    xOffset = 45.0f;
                    yOffset = 74f;
                    yMultiplier = 16.08f;
                    xMultiplier = 28.0f;
                    camConfig.camObj.orthographicSize = camConfig.defaultSize * camConfig.percentageReduction;
                    displayWidth = 1280;
                    displayHeight = 139;
                }
                else if(serializedObject.FindProperty("resolution").intValue == VRSL_CameraConfigurator.FOUR80p)
                {
                    //HORIZONTAL 480P
                    xMin = VRSL_CameraConfigurator.horizXMin480p;
                    xMax = VRSL_CameraConfigurator.horizXMax480p;
                    yMin = VRSL_CameraConfigurator.horizYMin480p;
                    yMax = VRSL_CameraConfigurator.horizYMax480p;

                    xOffset = 78.0f;
                    yOffset = 78f;
                    yMultiplier = 16.08f;
                    xMultiplier = 28.25f;


                    fh = fh * camConfig.percentageFurtherReduction;
                    camConfig.camObj.orthographicSize = camConfig.defaultSize * camConfig.percentageFurtherReduction;
                    displayWidth = 852;
                    displayHeight = 92;
                }
                else
                {
                    camConfig.camObj.orthographicSize = camConfig.defaultSize;
                }
            }
            else
            {
                camConfig.camObj.orthographicSize = camConfig.vertDefaultSize;
                xMin = VRSL_CameraConfigurator.vertXMin1080p;
                xMax = VRSL_CameraConfigurator.vertXMax1080p;
                yMin = VRSL_CameraConfigurator.vertYMin1080p;
                yMax = VRSL_CameraConfigurator.vertYMax1080p;


                horizDummy = Resources.Load("vertGridDummy") as Texture;
                fh = 162f;
                yOffset = 4f;
                xOffset = 127f;
                xMultiplier = 29.0f;
                displayHeight = 1080;
                displayWidth = 208;

                if(serializedObject.FindProperty("resolution").intValue == VRSL_CameraConfigurator.SEVEN20p)
                {
                    camConfig.camObj.orthographicSize = camConfig.vertDefaultSize * camConfig.percentageReduction;
                    //VERTICAL 720P
                    xMin = VRSL_CameraConfigurator.vertXMin720p;
                    xMax = VRSL_CameraConfigurator.vertXMax720p;
                    yMin = VRSL_CameraConfigurator.vertYMin720p;
                    yMax = VRSL_CameraConfigurator.vertYMax720p;
                    fh = fh * camConfig.percentageReduction;

                    xOffset = 131f;
                    yOffset = 30f;
                    //yMultiplier = 16.08f;
                    xMultiplier = 28.5f;
                    displayHeight = 720;
                    displayWidth = 139;
                }
                else if(serializedObject.FindProperty("resolution").intValue == VRSL_CameraConfigurator.FOUR80p)
                {
                    camConfig.camObj.orthographicSize = camConfig.vertDefaultSize * camConfig.percentageFurtherReduction;
                    //VERTICAL 480P
                    xMin = VRSL_CameraConfigurator.vertXMin480p;
                    xMax = VRSL_CameraConfigurator.vertXMax480p;
                    yMin = VRSL_CameraConfigurator.vertYMin480p;
                    yMax = VRSL_CameraConfigurator.vertYMax480p;
                    fh = fh * camConfig.percentageFurtherReduction;

                    
                    xOffset = 135f;
                    yOffset = 48f;
                    xMultiplier = 28.45f;
                    displayHeight = 480;
                    displayWidth = 92;
                }
                else
                {
                    camConfig.camObj.orthographicSize = camConfig.vertDefaultSize;
                }
            }

            imagestyle.fixedHeight = fh;
            rect.x = (rect.x+xOffset) + (serializedObject.FindProperty("xPos").floatValue*xMultiplier);
            rect.y = (rect.y+yOffset) - (serializedObject.FindProperty("yPos").floatValue*yMultiplier);
            rect.width = rect.width + widthOffset;
            GUI.Box(rect, horizDummy,imagestyle);

            var vertSliderRect = oldRect; //new Rect(314, 130, 100, 140)
            var horizSliderRect = oldRect; //new Rect(30, 295, 260, 30)

            vertSliderRect.x = 314f;
            vertSliderRect.width = 100f;
            vertSliderRect.height = 140f;
            vertSliderRect.y += 15f;

            horizSliderRect.x = 30f;
            horizSliderRect.width = 260f;
            horizSliderRect.height = 30f;            
            horizSliderRect.y += 160f;



            serializedObject.FindProperty("yPos").floatValue = GUI.VerticalSlider(vertSliderRect,camConfig.YPos, yMax, yMin);
            serializedObject.FindProperty("yPos").floatValue = EditorGUI.FloatField(new Rect(vertSliderRect.x + 20f, vertSliderRect.y +30f, 100, 20) , serializedObject.FindProperty("yPos").floatValue);
            EditorGUI.LabelField(new Rect(330f, vertSliderRect.y +45f, 100, 20), "Y Position");
            serializedObject.FindProperty("xPos").floatValue = GUI.HorizontalSlider(horizSliderRect,camConfig.XPos, xMin, xMax);
            serializedObject.FindProperty("xPos").floatValue = EditorGUI.FloatField(new Rect(horizSliderRect.x+140f, horizSliderRect.y +20f, 100, 20) , serializedObject.FindProperty("xPos").floatValue);
            EditorGUI.LabelField(new Rect(100, horizSliderRect.y+20f, 100, 20), "X Position");

            // if(serializedObject.FindProperty("isHorizontal").boolValue)
            // {
            //     if(serializedObject.FindProperty("is720p").boolValue == false)
            //     {
            //         serializedObject.FindProperty("yPos").floatValue = Mathf.Clamp(serializedObject.FindProperty("yPos").floatValue, -3.79f, 4.3f);
            //         serializedObject.FindProperty("xPos").floatValue = Mathf.Clamp(serializedObject.FindProperty("xPos").floatValue, 0.02f,0.02f);
            //     }
            // }

            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                camConfig._UpdateCameraPosition();
            }
            DrawLogo(oldRect, displayHeight, displayWidth);
        }


    }

    #endif
}
