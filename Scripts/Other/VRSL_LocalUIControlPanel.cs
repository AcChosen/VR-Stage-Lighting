
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif

namespace VRSL
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class VRSL_LocalUIControlPanel : UdonSharpBehaviour
    {
        public Texture videoSampleTargetTexture;

        [Header("Materials")]
        public Material[] fixtureMaterials;
        public Material[] volumetricMaterials;
        public Material[] projectionMaterials;
        public Material[] discoBallMaterials;
        public Material[] laserMaterials;
        [Space(5)]

        [Header("Post Processing Animators")]
        public Animator bloomAnimator;

        [Space(5)]
        [Header("UI Sliders")]
        public UnityEngine.UI.Slider masterSlider;
        public UnityEngine.UI.Slider fixtureSlider;
        public UnityEngine.UI.Slider volumetricSlider;
        public UnityEngine.UI.Slider projectionSlider;
        public UnityEngine.UI.Slider discoBallSlider;
        public UnityEngine.UI.Slider laserSlider;
        public UnityEngine.UI.Slider bloomSlider;
        public UnityEngine.UI.Text masterSliderText, fixtureSliderText, volumetricSliderText, projectionSliderText, discoBallSliderText, laserSliderText, bloomSliderText;
        public float fixtureIntensityMax = 1.0f, volumetricIntensityMax = 1.0f, projectionIntensityMax = 1.0f, discoballIntensityMax = 1.0f, laserIntensityMax = 1.0f;

        public UnityEngine.UI.Toggle volumetricNoiseToggle;
        public bool isUsingDMX = true;
        public bool isUsingAudioLink = true;
        [Space(10)]
        [Header("0 = Horizontal Mode  1 = Vertical Mode  2 = Legacy Mode")]

        [Range(0 ,2)]
        public int DMXMode;
        
        const int HORIZONTAL_MODE = 0;
        const int VERTICAL_MODE = 1;
        const int LEGACY_MODE = 2;
        [Space(20)]
        public CustomRenderTexture[] DMX_CRTS_Horizontal;
        public CustomRenderTexture[] DMX_CRTS_Vertical;
        public CustomRenderTexture[] DMX_CRTS_Legacy;
        public CustomRenderTexture[] AudioLink_CRTs;

        [HideInInspector]
        public int fixtureGizmos;

        [HideInInspector]
        public float panRangeTarget = 180f; 
        [HideInInspector]
        public float tiltRangeTarget = -180f;

        [HideInInspector]
        public bool useLegacyStaticLights = false;
        public bool useExtendedUniverses = false;

        [FieldChangeCallback(nameof(VolumetricNoise)), SerializeField]
        private bool _volumetricNoise = true;

        public bool VolumetricNoise
        {
            set
            {
                _volumetricNoise = value;
                _CheckDepthLightStatus();
            }
            get => _volumetricNoise;
        }

        [FieldChangeCallback(nameof(RequireDepthLight)), SerializeField]
        private bool _requireDepthLight = true;

        public bool RequireDepthLight
        {
            set
            {
                _requireDepthLight = value;
                _CheckDepthLightStatus();
                _DepthLightStatusReport();
            }
            get => _requireDepthLight;
        }

        public void OnEnable() 
        {
            _CheckDepthLightStatus();
        }
        void Start()
        {
            _CheckDepthLightStatus();
            _SetFinalIntensity();
            _SetFixtureIntensity();
            _SetVolumetricIntensity();
            _SetProjectionIntensity();
            _SetDiscoBallIntensity();
            _SetBloomIntensity();
            _CheckDMX();
            _CheckAudioLink();
            _CheckkExtendedUniverses();
            _ForceUpdateVideoSampleTexture();
        }

        public void _Test()
        {
            Debug.Log("This is a test");
        }

        public void _CheckDepthLightStatus()
        {

            foreach(Material mat in volumetricMaterials)
            {
                mat.SetInt("_PotatoMode", _volumetricNoise ? 0 : 1);
                mat.SetInt("_UseDepthLight", _requireDepthLight ? 1 : 0);
                SetKeyword(mat, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(mat.GetInt("_UseDepthLight"))) == 1 ? true : false);
                SetKeyword(mat, "_MAGIC_NOISE_ON", (Mathf.FloorToInt(mat.GetInt("_MAGIC_NOISE_ON"))) == 1 ? true : false);
                SetKeyword(mat, "_POTATO_MODE_ON", (Mathf.FloorToInt(mat.GetInt("_PotatoMode"))) == 1 ? true : false);
            }
            foreach(Material mat in projectionMaterials)
            {
                mat.SetInt("_UseDepthLight", _requireDepthLight ? 1 : 0);
            }
            foreach(Material mat in fixtureMaterials)
            {
                mat.SetInt("_UseDepthLight", _requireDepthLight ? 1 : 0);
            }
        }
        void _DepthLightStatusReport()
        {
            // if(_requireDepthLight)
            // {
            //     Debug.Log("VRSL Control Panel: Enabling Depth Light Requirement");
            // }
            // else
            // {
            //     Debug.Log("VRSL Control Panel: Disabling Depth Light Requirement");
            // }
        }
        
        void EnableCRTS(CustomRenderTexture[] rtArray)
        {
            foreach(CustomRenderTexture rt in rtArray)
            {
                rt.updateMode = CustomRenderTextureUpdateMode.Realtime;
            }
        }
        void DisableCRTS(CustomRenderTexture[] rtArray)
        {
            foreach(CustomRenderTexture rt in rtArray)
            {
                rt.updateMode = CustomRenderTextureUpdateMode.OnDemand;
            }
        }
        public void _CheckDMX()
        {
            if(isUsingDMX)
            {
                switch(DMXMode)
                {
                    case HORIZONTAL_MODE:
                        EnableCRTS(DMX_CRTS_Horizontal);
                        DisableCRTS(DMX_CRTS_Vertical);
                        DisableCRTS(DMX_CRTS_Legacy);
                        break;
                    case VERTICAL_MODE:
                        DisableCRTS(DMX_CRTS_Horizontal);
                        EnableCRTS(DMX_CRTS_Vertical);
                        DisableCRTS(DMX_CRTS_Legacy);
                        break;
                    case LEGACY_MODE:
                        DisableCRTS(DMX_CRTS_Horizontal);
                        DisableCRTS(DMX_CRTS_Vertical);
                        EnableCRTS(DMX_CRTS_Legacy);
                        break;
                    default:
                        DisableCRTS(DMX_CRTS_Horizontal);
                        DisableCRTS(DMX_CRTS_Vertical);
                        DisableCRTS(DMX_CRTS_Legacy);
                        break;
                }
            }
            else
            {
                DisableCRTS(DMX_CRTS_Horizontal);
                DisableCRTS(DMX_CRTS_Vertical);
                DisableCRTS(DMX_CRTS_Legacy);
            }
        }


        public void _CheckAudioLink()
        {
            if(isUsingAudioLink)
            {
                EnableCRTS(AudioLink_CRTs);
            }
            else
            {
                DisableCRTS(AudioLink_CRTs);
            }
        }

        public void _CheckkExtendedUniverses()
        {
            foreach(CustomRenderTexture crt in DMX_CRTS_Horizontal)
            {
                crt.material.SetInt("_NineUniverseMode", useExtendedUniverses ? 1 : 0);
            }
            foreach(CustomRenderTexture crt in DMX_CRTS_Vertical)
            {
                crt.material.SetInt("_NineUniverseMode", useExtendedUniverses ? 1 : 0);
            }
        }

        public void _ForceUpdateVideoSampleTexture()
        {
            if(videoSampleTargetTexture == null)
            {
                return;
            }
            foreach(Material m in laserMaterials)
            {
                if(m.HasProperty("_SamplingTexture"))
                {
                    m.SetTexture("_SamplingTexture",videoSampleTargetTexture);
                }
            }
            foreach(Material m in fixtureMaterials)
            {
                if(m.HasProperty("_SamplingTexture"))
                {
                    m.SetTexture("_SamplingTexture",videoSampleTargetTexture);
                }
            }
            foreach(Material m in discoBallMaterials)
            {
                if(m.HasProperty("_SamplingTexture"))
                {
                    m.SetTexture("_SamplingTexture",videoSampleTargetTexture);
                }
            }
            foreach(Material m in projectionMaterials)
            {
                if(m.HasProperty("_SamplingTexture"))
                {
                    m.SetTexture("_SamplingTexture",videoSampleTargetTexture);
                }
            }
            foreach(Material m in volumetricMaterials)
            {
                if(m.HasProperty("_SamplingTexture"))
                {
                    m.SetTexture("_SamplingTexture",videoSampleTargetTexture);
                }
            }
        }
        
        public void _SetFinalIntensity()
        {
            fixtureIntensityMax = masterSlider.value;
            volumetricIntensityMax = masterSlider.value;
            projectionIntensityMax = masterSlider.value;
            discoballIntensityMax = masterSlider.value;
            laserIntensityMax = masterSlider.value;
            _SetFixtureIntensity();
            _SetVolumetricIntensity();
            _SetProjectionIntensity();
            _SetDiscoBallIntensity();
            _SetLaserIntensity();
            masterSliderText.text = Mathf.Round(masterSlider.value * 100.0f).ToString();
        }

        public void _SetFixtureIntensity()
        {
            foreach(Material mat in fixtureMaterials)
            {
                mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, fixtureIntensityMax, fixtureSlider.value));
            }
            fixtureSliderText.text = Mathf.Round(fixtureSlider.value * 100.0f).ToString();
        }

        public void _SetVolumetricIntensity()
        {
            foreach(Material mat in volumetricMaterials)
            {
                mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, volumetricIntensityMax, volumetricSlider.value));
            }
            volumetricSliderText.text = Mathf.Round(volumetricSlider.value * 100.0f).ToString();
        }

        public void _SetProjectionIntensity()
        {
            foreach(Material mat in projectionMaterials)
            {
                mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, projectionIntensityMax, projectionSlider.value));
            }
            projectionSliderText.text = Mathf.Round(projectionSlider.value * 100.0f).ToString();
        }

        public void _SetDiscoBallIntensity()
        {
            foreach(Material mat in discoBallMaterials)
            {
                mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, discoballIntensityMax, discoBallSlider.value));
            }
            discoBallSliderText.text = Mathf.Round(discoBallSlider.value * 100.0f).ToString();
        }

        public void _SetLaserIntensity()
        {
            foreach(Material mat in laserMaterials)
            {
                mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, laserIntensityMax, laserSlider.value));
            }
            laserSliderText.text = Mathf.Round(laserSlider.value * 100.0f).ToString();
        }
        public void _SetBloomIntensity()
        {
            if(bloomAnimator != null)
            {
                bloomAnimator.SetFloat("BloomIntensity", bloomSlider.value);
                bloomSliderText.text = Mathf.Round(bloomSlider.value * 100.0f).ToString();
            }
        }

        void SetKeyword(Material mat, string keyword, bool status)
        {
            if (status)
            {
                mat.EnableKeyword(keyword);
            } 
            else 
            {
                mat.DisableKeyword(keyword);
            }
        }
    }

        #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(VRSL_LocalUIControlPanel))]
    public class VRSL_LocalUIControlPanel_Editor : Editor
    {
        public static Texture logo;
        public static string ver = "VR Stage Lighting ver:" + " <b><color=#6a15ce> 2.1</color></b>";

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
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawLogo();
            ShurikenHeaderCentered(ver);
            EditorGUILayout.Space();
            VRSL_LocalUIControlPanel controlPanel = (VRSL_LocalUIControlPanel)target;
            if (GUILayout.Button(new GUIContent("Force Update Target AudioLink Sample Texture", "Updates all AudioLink VRSL Fixtures to sample from the selected target texture when texture sampling is enabled on the fixture."))) { controlPanel._ForceUpdateVideoSampleTexture(); }
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
    #endif
}
