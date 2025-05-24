using UnityEngine;
#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;
#endif
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
#if UDONSHARP
using UdonSharpEditor;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
#endif
#endif

namespace VRSL
{

    public enum AudioLinkBandState
    {
        Bass,
        Low_Mids,
        High_Mids,
        Treble
    }

#if UDONSHARP
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRStageLighting_AudioLink_Static : UdonSharpBehaviour
#else
    public class VRStageLighting_AudioLink_Static : MonoBehaviour
#endif
    {
        //////////////////Public Variables////////////////////

        [Header("Audio Link Settings")]
        [SerializeField, FieldChangeCallback(nameof(EnableAudioLink)),
         Tooltip("Enable or disable Audio Link Reaction for this fixture.")
        ]
        private bool enableAudioLink;


        //[Tooltip("The Audio Link Script to react to.")]
        //public AudioLink audioLink;

        [SerializeField, FieldChangeCallback(nameof(Band)), //Range(0, 3)
         Tooltip("The frequency band of the spectrum to react to.")
        ]
        private AudioLinkBandState band;


        [SerializeField, FieldChangeCallback(nameof(Delay)), Range(0, 127),
         Tooltip("The level of delay to add to the reaction.")
        ]
        private int delay;


        [SerializeField, FieldChangeCallback(nameof(BandMultiplier)), Range(1.0f, 15.0f),
         Tooltip("Multiplier for the sensativity of the reaction.")
        ]
        private float bandMultiplier = 1.0f;


        [SerializeField, FieldChangeCallback(nameof(ColorChord)),
         Tooltip("Enable Color Chord tinting of the light emission.")
        ]
        private bool enableColorChord;
        

        [Header("General Settings")]

        [SerializeField, FieldChangeCallback(nameof(GlobalIntensity)), Range(0, 1),
         Tooltip("Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity.")
        ]
        private float globalIntensity = 1; 


        [SerializeField, HideInInspector, FieldChangeCallback(nameof(FinalIntensity)), Range(0, 1),
         Tooltip("Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI.")
        ]
        public float finalIntensity = 1;

        [HideInInspector, FieldChangeCallback(nameof(FinalIntensityComponentMode)),
         Tooltip("Choose between setting the Final Intensity for all meshes, or individual meshes")
        ]
        public bool finalIntensityComponentMode = false;

        [HideInInspector, FieldChangeCallback(nameof(FinalIntensityVolumetric)), Range(0, 1),
         Tooltip("Sets the maximum brightness value of Global Intensity For Volumetric Meshes Only. Good for personalized settings of the max brightness of the shader by other users via UI.")
        ]
        public float finalIntensityVolumetric = 1;

        [HideInInspector, FieldChangeCallback(nameof(FinalIntensityProjection)), Range(0, 1),
         Tooltip("Sets the maximum brightness value of Global Intensity For Projection Meshes Only. Good for personalized settings of the max brightness of the shader by other users via UI.")
        ]
        public float finalIntensityProjection = 1;

        [HideInInspector, FieldChangeCallback(nameof(FinalIntensityFixture)), Range(0, 1),
         Tooltip("Sets the maximum brightness value of Global Intensity For Fixture Meshes Only. Good for personalized settings of the max brightness of the shader by other users via UI.")
        ]
        public float finalIntensityFixture = 1;

        [SerializeField, FieldChangeCallback(nameof(LightColorTint)), ColorUsage(false, true),
         Tooltip("The main color of the light.")
        ]
        private Color lightColorTint = Color.white * 2.0f;


        [SerializeField, FieldChangeCallback(nameof(ColorTextureSampling)),
         Tooltip("Check this box if you wish to sample seperate texture for the color. The color will be influenced by the intensity of the original emission color! The texture is set in the shader itself.")
        ]
        private bool enableColorTextureSampling;

        [SerializeField, FieldChangeCallback(nameof(TraditionalColorTextureSampling)),
         Tooltip("Check this box if you wish to use traditional color sampling instead of white to black conversion")
        ]
        private bool traditionalColorTextureSampling;


        [SerializeField, FieldChangeCallback(nameof(TextureSamplingCoordinates)),
         Tooltip("The UV coordinates to sample the color from on the texture.")
        ]
        private Vector2 textureSamplingCoordinates = new Vector2(0.5f, 0.5f);

        [SerializeField, FieldChangeCallback(nameof(ThemeColorSampling)),
         Tooltip("Check this box if you wish to enable AudioLink Theme colors.")
        ]
        private bool enableThemeColorSampling;

        [SerializeField, FieldChangeCallback(nameof(ThemeColorTarget)), Range(1, 4),
         Tooltip("Theme Color to Sample from.")
        ]
        private int themeColorTarget = 1;


        [Space(5)]
        [Header("Movement Settings")]


        // [Tooltip ("Invert the pan values (Left/Right Movement) for movers.")]
        // [FieldChangeCallback(nameof(InvertPan))]
        // [SerializeField]
        // private bool invertPan;


        // [Tooltip ("Invert the tilt values (Up/Down Movement) for movers.")]
        // [FieldChangeCallback(nameof(InvertTilt))]
        // [SerializeField]
        // private bool invertTilt;


        // [Tooltip ("Enable this if the mover is hanging upside down.")]
        // public bool isUpsideDown;

        
        // [Tooltip ("Enable target following for a mover while in Udon mode.")]
        // public bool followTarget;


        [Tooltip ("The target for this mover to follow.")]
        public Transform targetToFollow;


        // [Tooltip ("The speed at which the target is followed, higher = slower.")]
        // public float targetFollowLerpSpeed = 20.0f;


        // [Tooltip ("An offset of where the mover will point at relative to the target.")]
        // public Vector3 targetOffset;


        [Space(5)]
        [Header("Fixture Settings")]

        [SerializeField, FieldChangeCallback(nameof(SpinSpeed)), Range(-10, 10),
         Tooltip("Projection Spin Speed (Udon Override Only).")
        ]
        private float spinSpeed = 4.0f;



        /////////////////////////////////////////////////////////////////////
        [SerializeField, FieldChangeCallback(nameof(ProjectionSpin)),
         Tooltip("Enable projection spinning (Udon Override Only).")
        ]
        private bool enableAutoSpin;





        /////////////////////////////////
        // [Range(0,360.0f)]
        // [Tooltip ("Tilt (Up/Down) offset/movement. Directly controls tilt when in Udon Mode.")]
        // [FieldChangeCallback(nameof(Tilt))]
        // [SerializeField]
        // private float tiltOffsetBlue = 90.0f;
        // float startTiltOffset;

        // [Range(0,360.0f)]
        // [Tooltip ("Pan (Left/Right) offset/movement. Directly controls pan when in Udon Mode; is an offset when in DMX mode.")]
        // [FieldChangeCallback(nameof(Pan))]
        // [SerializeField]
        // public float panOffsetBlueGreen = 0.0f;
        // float startPanOffset;
        ////////////////////////////////////



        [SerializeField, FieldChangeCallback(nameof(SelectGOBO)), Range(1, 8),
         Tooltip("Use this to change what projection is selected")
        ]
        private int selectGOBO = 1;


        [Header("Mesh Settings")]
        ////
        [Tooltip ("The meshes used to make up the light. You need atleast 1 mesh in this group for the script to work properly.")]
        public MeshRenderer[] objRenderers;


        [SerializeField, FieldChangeCallback(nameof(ConeWidth)), Range(0, 5.5f),
         Tooltip("Controls the radius of a mover/spot light.")
        ]
        private float coneWidth = 2.5f;


        [SerializeField, FieldChangeCallback(nameof(ConeLength)), Range(0.001f, 10.0f),
         Tooltip("Controls the length of the cone of a mover/spot light.")
        ]
        private float coneLength = 1.0f; 
        

        [SerializeField, FieldChangeCallback(nameof(MaxConeLength)), Range(0.275f, 10.0f),
         Tooltip("Controls the mesh length of the cone of a mover/spot light")
        ]
        private float maxConeLength = 8.5f; 
        

        /////////////////Private Variables//////////////////
        
        private MaterialPropertyBlock props;
        //bool enableInstancing;
        float targetPanAngle, targetTiltAngle;
        private Vector3 targetToFollowLast;
        float[] spectrumBands;
        private bool wasChanged;
        private Color previousColorTint;
        private Transform previousTargetToFollowTransform;
        
        private float previousConeWidth, previousConeLength, previousGlobalIntensity, previousFinalIntensity, previousMaxConeLength;
        private float previousFinalIntensityVolumetric, previousFinalIntensityProjection, previousFinalIntensityFixture;
        private int previousGOBOSelection;
        [HideInInspector]
        public UnityEngine.Animations.AimConstraint targetConstraint;
        [HideInInspector]
        public bool foldout;

        void OnEnable() 
        {
            Init(true);
        }

        void Start()
        {
            Init(true);
        }

        public void _SetProps()
        {
            props = new MaterialPropertyBlock();
        }
    /////////////////////////////////////////////////////////////////////////PROPERTIES///////////////////////////////////////////////////////////////////////////////////////////////
        public Color LightColorTint
        {
            get
            {
                return lightColorTint;
            }
            set
            {
                previousColorTint = lightColorTint;
                lightColorTint = value;
                _UpdateInstancedProperties();
            }
        }
        public float ConeWidth
        {
            get
            {
                return coneWidth;
            }
            set
            {
                previousConeWidth = coneWidth;
                coneWidth = value;
                _UpdateInstancedProperties();
            }
        }
        public float ConeLength
        {
            get
            {
                return coneLength;
            }
            set
            {
                previousConeLength = coneLength;
                coneLength = value;
                _UpdateInstancedProperties();
            }
        }

            public float MaxConeLength
        {
            get
            {
                return maxConeLength;
            }
            set
            {
                previousMaxConeLength = maxConeLength;
                maxConeLength = value;
                _UpdateInstancedProperties();
            }
        }
        public float GlobalIntensity
        {
            get
            {
                return globalIntensity;
            }
            set
            {
                previousGlobalIntensity = globalIntensity;
                globalIntensity = value;
                _UpdateInstancedProperties();
            }
        }
        public float FinalIntensity
        {
            get
            {
                return finalIntensity;
            }
            set
            {
                previousFinalIntensity = finalIntensity;
                finalIntensity = value;
                _UpdateInstancedProperties();
            }
        }

        public bool FinalIntensityComponentMode
        {
            get
            {
                return finalIntensityComponentMode;
            }
            set
            {
                finalIntensityComponentMode = value;
                _UpdateInstancedProperties();
            }            
        }
        public float FinalIntensityVolumetric
        {
            get
            {
                return finalIntensityVolumetric;
            }
            set
            {
                previousFinalIntensityVolumetric = finalIntensityVolumetric;
                finalIntensityVolumetric  = value;
                _UpdateInstancedProperties();
            }
        }

        public float FinalIntensityProjection
        {
            get
            {
                return finalIntensityProjection;
            }
            set
            {
                previousFinalIntensityProjection = finalIntensityProjection;
                finalIntensityProjection  = value;
                _UpdateInstancedProperties();
            }
        }
        public float FinalIntensityFixture
        {
            get
            {
                return finalIntensityFixture;
            }
            set
            {
                previousFinalIntensityFixture = finalIntensityFixture;
                finalIntensityFixture  = value;
                _UpdateInstancedProperties();
            }
        }
        public int SelectGOBO
        {
            get
            {
                return selectGOBO;
            }
            set
            {
                previousGOBOSelection = selectGOBO;
                selectGOBO = value;
                _UpdateInstancedProperties();
            }
        }
        public bool EnableAudioLink
        {
            get
            {
                return enableAudioLink;
            }
            set
            {
                enableAudioLink = value;
                _UpdateInstancedProperties();
            }
        }
        public AudioLinkBandState Band
        {
            get
            {
                return band;
            }
            set
            {
                band = value;
                _UpdateInstancedProperties();
            }
        }
        public int Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
                _UpdateInstancedProperties();
            }
        }
        public float BandMultiplier
        {
            get
            {
                return bandMultiplier;
            }
            set
            {
                bandMultiplier = value;
                _UpdateInstancedProperties();
            }
        }
        public Vector2 TextureSamplingCoordinates
        {
            get
            {
                return textureSamplingCoordinates;
            }
            set
            {
                textureSamplingCoordinates = value;
                _UpdateInstancedProperties();
            }
        }
        // public bool InvertPan
        // {
        //     get
        //     {
        //         return invertPan;
        //     }
        //     set
        //     {
        //         invertPan = value;
        //         _UpdateInstancedProperties();
        //     }
        // }
        // public bool InvertTilt
        // {
        //     get
        //     {
        //         return invertTilt;
        //     }
        //     set
        //     {
        //         invertTilt = value;
        //         _UpdateInstancedProperties();
        //     }
        // }
        public float SpinSpeed
        {
            get
            {
                return spinSpeed;
            }
            set
            {
                spinSpeed = value;
                _UpdateInstancedProperties();
            }
        }
        public bool ProjectionSpin
        {
            get
            {
                return enableAutoSpin;
            }
            set
            {
                enableAutoSpin = value;
                _UpdateInstancedProperties();
            }
        }
            public bool ColorChord
        {
            get
            {
                return enableColorChord;
            }
            set
            {
                enableColorChord = value;
                _UpdateInstancedProperties();
            }
        }
            public bool ColorTextureSampling
            {
                get
                {
                    return enableColorTextureSampling;
                }
                set
                {
                    enableColorTextureSampling = value;
                    _UpdateInstancedProperties();
                }
            }

            public bool TraditionalColorTextureSampling
            {
                get
                {
                    return traditionalColorTextureSampling;
                }
                set
                {
                    traditionalColorTextureSampling = value;
                    _UpdateInstancedProperties();
                }
            }
            public bool ThemeColorSampling
            {
                get
                {
                    return enableThemeColorSampling;
                }
                set
                {
                    enableThemeColorSampling = value;
                    _UpdateInstancedProperties();
                }
            }
            public int ThemeColorTarget
            {
                get
                {
                    return themeColorTarget;
                }
                set
                {
                    themeColorTarget = value;
                    _UpdateInstancedProperties();
                }

            }
            // public float Pan
            // {
            //     get
            //     {
            //         return panOffsetBlueGreen;
            //     }
            //     set
            //     {
            //         panOffsetBlueGreen = value;
            //         _UpdateInstancedProperties();
            //     }
            // }
            // public float Tilt
            // {
            //     get
            //     {
            //         return tiltOffsetBlue;
            //     }
            //     set
            //     {
            //         tiltOffsetBlue = value;
            //         _UpdateInstancedProperties();
            //     }
            // }

    /////////////////////////////////////////////////////////////////////////END PROPERTIES///////////////////////////////////////////////////////////////////////////////////////////////
        MaterialPropertyBlock _SetFinalIntensityComponents(MaterialPropertyBlock props, MeshRenderer renderer){
            if(!finalIntensityComponentMode){return props;}
                if(renderer.gameObject.name.Contains("Volume") || renderer.gameObject.name.Contains("volume") || renderer.gameObject.name.Contains("Flare") || renderer.gameObject.name.Contains("flare")){
                    props.SetFloat("_FinalIntensity", finalIntensityVolumetric);
                }
                else if(renderer.gameObject.name.Contains("Project") || renderer.gameObject.name.Contains("project")){
                    props.SetFloat("_FinalIntensity", finalIntensityProjection);
                }
                else{
                    props.SetFloat("_FinalIntensity", finalIntensityFixture);
                }
            return props;
        }
        public void _UpdateInstancedProperties()
        {   
            if(props == null)
            {
                if(objRenderers.Length > 0 && objRenderers[0] != null)
                {
                    _SetProps();
                }
                else
                {
                    Debug.Log("Please add atleast one fixture renderer.");
                    return;
                }
            }
            //Color Texture Sampling
            props.SetFloat("_TextureColorSampleX", textureSamplingCoordinates.x);
            props.SetFloat("_TextureColorSampleY", textureSamplingCoordinates.y);
            props.SetInt("_EnableColorTextureSample", enableColorTextureSampling == true ? 1 : 0);
            props.SetInt("_UseTraditionalSampling", traditionalColorTextureSampling == true ? 1 : 0);
            props.SetInt("_EnableThemeColorSampling", enableThemeColorSampling == true ? 1 : 0);
            props.SetInt("_ThemeColorTarget", themeColorTarget);

            //AudioLink Stuff
            props.SetFloat("_EnableAudioLink", enableAudioLink == true ? 1.0f : 0.0f);
            props.SetInt("_EnableColorChord", enableColorChord == true ? 1 : 0);
            //props.SetFloat("_NumBands", spectrumBands.Length);
            props.SetFloat("_Delay", delay);
            props.SetFloat("_BandMultiplier", bandMultiplier);
            int b = (int) band;
            float ba = 1.0f * b;
            props.SetFloat("_Band", ba);
            //Movement Stuff
            // props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
            // props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
            props.SetInt("_EnableSpin", enableAutoSpin == true ? 1 : 0);
            props.SetFloat("_SpinSpeed", spinSpeed);
            // props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
            // props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
            //Other Stuff
            props.SetInt("_ProjectionSelection", selectGOBO);
            props.SetColor("_Emission", lightColorTint);
            props.SetFloat("_ConeWidth", coneWidth);
            props.SetFloat("_GlobalIntensity", globalIntensity);
            props.SetFloat("_FinalIntensity", finalIntensity);
            props.SetFloat("_ConeLength", Mathf.Abs(coneLength - 10.0f));
            props.SetFloat("_MaxConeLength", maxConeLength);
            // for(int i = 0; i < objRenderers.Length; i++)
            // {
            //     objRenderers[i].SetPropertyBlock(props);
            // }
            switch(objRenderers.Length)
            {
                case 1:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    break;
                case 2:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[0])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    break;
                case 3:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    if(objRenderers[2])
                        objRenderers[2].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[2]));
                    break;
                case 4:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    if(objRenderers[2])
                        objRenderers[2].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[2]));
                    if(objRenderers[3])
                        objRenderers[3].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[3]));
                    break;
                case 5:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    if(objRenderers[2])
                        objRenderers[2].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[2]));
                    if(objRenderers[3])
                        objRenderers[3].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[3]));
                    if(objRenderers[4])
                        objRenderers[4].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[4]));
                    break;
                default:
                    Debug.Log("Too many mesh renderers for this fixture!");
                    break;        
            }
        }
        public void _UpdateInstancedPropertiesSansAudioLink()
        {   
            if(props == null)
            {
                if(objRenderers.Length > 0 && objRenderers[0] != null)
                {
                    _SetProps();
                }
                else
                {
                    Debug.Log("Please add atleast one fixture renderer.");
                    return;
                }
            }
            //Color Texture Sampling
            props.SetFloat("_TextureColorSampleX", textureSamplingCoordinates.x);
            props.SetFloat("_TextureColorSampleY", textureSamplingCoordinates.y);
            props.SetInt("_EnableColorTextureSample", enableColorTextureSampling == true ? 1 : 0);
            props.SetInt("_UseTraditionalSampling", traditionalColorTextureSampling == true ? 1 : 0);
            props.SetInt("_EnableThemeColorSampling", enableThemeColorSampling == true ? 1 : 0);
            props.SetInt("_ThemeColorTarget", themeColorTarget);

            //AudioLink Stuff
            props.SetFloat("_EnableAudioLink", 0.0f);
            props.SetInt("_EnableColorChord", 0);
            //props.SetFloat("_NumBands", spectrumBands.Length);
            props.SetFloat("_Delay", delay);
            props.SetFloat("_BandMultiplier", bandMultiplier);
            int b = (int) band;
            float ba = 1.0f * b;
            props.SetFloat("_Band", ba);
            //Movement Stuff
            // props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
            // props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
            props.SetInt("_EnableSpin", enableAutoSpin == true ? 1 : 0);
            props.SetFloat("_SpinSpeed", spinSpeed);
            // props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
            // props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
            //Other Stuff
            props.SetInt("_ProjectionSelection", selectGOBO);
            props.SetColor("_Emission", lightColorTint);
            props.SetFloat("_ConeWidth", coneWidth);
            props.SetFloat("_GlobalIntensity", globalIntensity);
            props.SetFloat("_FinalIntensity", finalIntensity);
            props.SetFloat("_ConeLength", Mathf.Abs(coneLength - 10.0f));
            props.SetFloat("_MaxConeLength", maxConeLength);
            // for(int i = 0; i < objRenderers.Length; i++)
            // {
            //     objRenderers[i].SetPropertyBlock(props);
            // }
            switch(objRenderers.Length)
            {
                case 1:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    break;
                case 2:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    break;
                case 3:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    if(objRenderers[2])
                        objRenderers[2].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[2]));
                    break;
                case 4:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    if(objRenderers[2])
                        objRenderers[2].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[2]));
                    if(objRenderers[3])
                        objRenderers[3].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[3]));
                    break;
                case 5:
                    if(objRenderers[0])
                        objRenderers[0].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[0]));
                    if(objRenderers[1])
                        objRenderers[1].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[1]));
                    if(objRenderers[2])
                        objRenderers[2].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[2]));
                    if(objRenderers[3])
                        objRenderers[3].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[3]));
                    if(objRenderers[4])
                        objRenderers[4].SetPropertyBlock(_SetFinalIntensityComponents(props, objRenderers[4]));
                    break;
                default:
                    Debug.Log("Too many mesh renderers for this fixture!");
                    break;           
            }
        }
        void Init(bool withAL)
        {
            if(objRenderers[0] == null)
            {
                return;
            }
            if(objRenderers.Length > 0)
            {
                _SetProps();
                previousColorTint = lightColorTint;
                previousConeWidth = coneWidth;
                previousConeLength = coneLength;
                previousMaxConeLength = maxConeLength;
                previousGOBOSelection = selectGOBO;
                previousGlobalIntensity = globalIntensity;
                previousFinalIntensity = finalIntensity;
                previousFinalIntensityFixture = finalIntensityFixture;
                previousFinalIntensityProjection = finalIntensityProjection;
                previousFinalIntensityVolumetric = finalIntensityVolumetric;
                if(withAL)
                {
                    _UpdateInstancedProperties();
                }
                else
                {
                    _UpdateInstancedPropertiesSansAudioLink();
                }
            }
            else
            {
                Debug.Log("Please add atleast one fixture renderer.");
                //enableInstancing = false;
            }
        }
        //EDITOR STUFF
        #if !COMPILER_UDONSHARP && UNITY_EDITOR

        void SetConstraints(VRStageLighting_AudioLink_Static fixture)
        {

            UnityEngine.Animations.ConstraintSource s = new UnityEngine.Animations.ConstraintSource();
            s.sourceTransform = fixture.targetToFollow;
            s.weight = 1.0f;
            if(fixture.targetConstraint.sourceCount <= 0)
            {
                fixture.targetConstraint.AddSource(s);
            }
            else
            {
                fixture.targetConstraint.SetSource(0,s);
            }
            //Debug.Log(fixture.name + " is set to follow " + fixture.targetConstraint.GetSource(0).sourceTransform.name);
        }
        public void _CheckConstraints(VRStageLighting_AudioLink_Static fixture)
        {
            if(fixture.targetToFollow != null)
            {
                if(fixture.targetConstraint == null)
                {
                    //Debug.Log("Couldn't Find Target Constraint... Attempting To grab it");
                    bool hasConstraint = false;
                    UnityEngine.Animations.AimConstraint[] childs = fixture.GetComponentsInChildren<UnityEngine.Animations.AimConstraint>();
                    foreach(UnityEngine.Animations.AimConstraint x in childs)
                    {
                        if(x.gameObject.name.Contains("Head"))
                        {
                            fixture.targetConstraint = x;
                            hasConstraint = true;
                            break;
                        }
                    }
                    if(!hasConstraint)
                    {
                        Debug.Log("This fixture does not support target following/tracking");
                        fixture.targetToFollow = null;
                    }
                    else
                    {
                        SetConstraints(fixture);
                    }
                }
                else
                {
                    //Debug.Log("Found Target Constraint " + fixture.targetConstraint.name + ". Attempting to update source...");
                    if(fixture.targetConstraint.sourceCount > 0)
                    {
                        //Debug.Log("Constraint already has source... Updating it...");
                        if(fixture.targetConstraint.GetSource(0).sourceTransform != fixture.targetToFollow)
                        {
                            SetConstraints(fixture);
                        }
                    }
                    else
                    {
                        //Debug.Log("Constraint does not have a source... Updating it...");
                        SetConstraints(fixture);
                    }
                }
            }
        }


            private void OnDrawGizmos()
            {
#if UDONSHARP
                #pragma warning disable 0618 //suppressing obsoletion warnings
                this.UpdateProxy(ProxySerializationPolicy.RootOnly);
                #pragma warning restore 0618 //suppressing obsoletion warnings
#endif
                Gizmos.color = lightColorTint;
                if(targetToFollow != null)
                {
                    Gizmos.DrawWireSphere(targetToFollow.position, 0.25f);
                }
                Gizmos.DrawWireSphere(transform.position, 0.05f);
            }
            //Handle Duplication Events
            void OnValidate()
            {
                Event e = Event.current;
        
                if (e != null)
                {
                    if (e.type == EventType.ExecuteCommand && e.commandName == "Duplicate")
                    {
                        Init(false);
                        return;
                    }
                }
            }
        #endif
    }
}
