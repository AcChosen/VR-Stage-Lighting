
using UnityEngine;
#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.SDKBase.VRCShader;
#else
using static UnityEngine.Shader;
using UnityEngine.Rendering;
#endif

#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

#if UDONSHARP
using UdonSharpEditor;
#endif
#endif

namespace VRSL
{    
    public enum VolumetricQualityModes
    {
        High,
        Medium,
        Low
    }
    public enum DefaultQualityModes
    {
        High,
        Low
    }

#if UDONSHARP
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRSL_LocalUIControlPanel : UdonSharpBehaviour
#else
    public class VRSL_LocalUIControlPanel : MonoBehaviour
#endif
    {
        [SerializeField, HideInInspector]
        private VRStageLighting_AudioLink_Laser[] audioLinkLasers;
        [SerializeField, HideInInspector]
        private VRStageLighting_AudioLink_Static[] audiolinkLights;
        [SerializeField, HideInInspector]
        private VRStageLighting_DMX_Static[] dmxLights;
        [Header("Quality Modes")]
        public VolumetricQualityModes volumetricQuality;
        public bool lockVolumetricQualityMode;
        [Space(5.0f)]
        public DefaultQualityModes blinderProjectionQuality;
        public bool lockBlinderProjectionQualityMode;
        [Space(5.0f)]

        public DefaultQualityModes parProjectionQuality;
        public bool lockParProjectionQualityMode;
        [Space(5.0f)]
        public DefaultQualityModes otherProjectionQuality;
        public bool lockOtherProjectionQualityMode;
        [Space(5.0f)]
        public DefaultQualityModes discoballQuality;
        public bool lockDiscoballQualityMode;
        [Space(5.0f)]
        public DefaultQualityModes lensFlareQuality;
        public bool lockLensFlareQualityMode;
       // [Space(20.0f)]
        [Header("Video Sampling")]
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
        public UnityEngine.UI.Button volumetricHighButton,volumetricMedButton, volumetricLowButton;
        public UnityEngine.UI.Text volumetricHighText, volumetricMedText, volumetricLowText;
        public UnityEngine.UI.Button blinderProjectionHighButton,  blinderProjectionLowButton;
        public UnityEngine.UI.Text  blinderProjectionHighText,  blinderProjectionLowText;
        
        public UnityEngine.UI.Button parProjectionHighButton,  parProjectionLowButton;
        public UnityEngine.UI.Text  parProjectionHighText,  parProjectionLowText;
        public UnityEngine.UI.Button otherProjectionHighButton,  otherProjectionLowButton;
        public UnityEngine.UI.Text  otherProjectionHighText,  otherProjectionLowText;
        
        public UnityEngine.UI.Button discoballHighButton,  discoballLowButton;
        public UnityEngine.UI.Text  discoballHighText,  discoballLowText;
        public UnityEngine.UI.Button lensFlareHighButton,  lensFlareLowButton;
        public UnityEngine.UI.Text lensFlareHighText, lensFlareLowText;

        public UnityEngine.UI.Button globalStrobeToggleButton;
        public UnityEngine.UI.Text globalStrobeLabel;
        UnityEngine.UI.ColorBlock defaultColorBlock;
        UnityEngine.UI.ColorBlock cbOn;
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
        public bool delayStrobeForGI = true;
        [Space(5)]
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
       // public bool adjustInGameInterpolation;
       public bool sperateInGameInterpolationSpeed = true;
        public float inGameInterpolationModifier = 1.55f;
        public bool outputDebugLogs;

        [HideInInspector]
        public int volumetricMeshQuality;

        [HideInInspector]
        public string fixtureDefGUID = "4d88361aa1276d64d8a60009bfb590ed";

        [HideInInspector]
        public string fixtureSaveFile = "NONE";

        [HideInInspector]
        public bool useDMXGI = false;

        [SerializeField, FieldChangeCallback(nameof(VolumetricNoise))]
        private bool _volumetricNoise = true;
        int _Udon_DMXGridRenderTexture, _Udon_DMXGridRenderTextureMovement, _Udon_DMXGridSpinTimer, _Udon_DMXGridStrobeTimer, _Udon_DMXGridStrobeOutput;

        public bool VolumetricNoise
        {
#if (UNITY_ANDROID || UNITY_IOS) && UDONSHARP
            set {
                _volumetricNoise = false;
                _CheckDepthLightStatus();
            }
            get => false;
#else
            set
            {
                _volumetricNoise = value;
                _CheckDepthLightStatus();
            }
            get => _volumetricNoise;
#endif
        }

        [SerializeField, FieldChangeCallback(nameof(RequireDepthLight))]
        private bool _requireDepthLight = true;

        public bool RequireDepthLight
        {
#if (UNITY_ANDROID || UNITY_IOS) && UDONSHARP
            set {
                _requireDepthLight = false;
                _CheckDepthLightStatus();
                _DepthLightStatusReport();
            }
            get => false;
#else
            set
            {
                _requireDepthLight = value;
                _CheckDepthLightStatus();
                _DepthLightStatusReport();
            }
            get => _requireDepthLight;
#endif
        }

        [SerializeField, FieldChangeCallback(nameof(GlobalDisableStrobe))]
        private bool _globalDisableStrobe = false;

        public bool GlobalDisableStrobe
        {
            set
            {
                _globalDisableStrobe = value;
                SetStrobeTextureStatus();
            }
            get => _globalDisableStrobe;
        }

        public float _targetCRTUpdateRate = 0.0f;

        public void _ToggleGlobalStrobe()
        {
            GlobalDisableStrobe = !GlobalDisableStrobe;
        }

        void SetGlobalStrobeUI()
        {
            
            if(globalStrobeToggleButton){globalStrobeToggleButton.colors = GlobalDisableStrobe ? cbOn : defaultColorBlock;}
            globalStrobeToggleButton.gameObject.SetActive(isUsingDMX);
        }

        void SetStrobeTextureStatus()
        {
            foreach(CustomRenderTexture rt in DMX_CRTS_Legacy)
            {
                if(rt == null){continue;}
                if(rt.name.Contains("Strobe") && rt.material.HasProperty("_DisableStrobe"))
                {
                    rt.material.SetFloat("_DisableStrobe",GlobalDisableStrobe ? 1f : 0f);
                }
            }       
            foreach(CustomRenderTexture rt in DMX_CRTS_Horizontal)
            {
                if(rt == null){continue;}
                if(rt.name.Contains("Strobe") && rt.material.HasProperty("_DisableStrobe"))
                {
                    rt.material.SetFloat("_DisableStrobe",GlobalDisableStrobe ? 1f : 0f);
                }
            }        
            foreach(CustomRenderTexture rt in DMX_CRTS_Vertical)
            {
                if(rt == null){continue;}
                if(rt.name.Contains("Strobe") && rt.material.HasProperty("_DisableStrobe"))
                {
                    rt.material.SetFloat("_DisableStrobe",GlobalDisableStrobe ? 1f : 0f);
                }
            }
            SetGlobalStrobeUI();     
        }

        
        void _SetTextureIDS()
        {
            _Udon_DMXGridRenderTexture = PropertyToID("_Udon_DMXGridRenderTexture");
            _Udon_DMXGridRenderTextureMovement = PropertyToID("_Udon_DMXGridRenderTextureMovement");
            _Udon_DMXGridSpinTimer = PropertyToID("_Udon_DMXGridSpinTimer");
            _Udon_DMXGridStrobeTimer = PropertyToID("_Udon_DMXGridStrobeTimer");
            _Udon_DMXGridStrobeOutput = PropertyToID("_Udon_DMXGridStrobeOutput");
        }

        void ReduceInGameInterpolation()
        {
            if(sperateInGameInterpolationSpeed)
            {
                foreach(CustomRenderTexture rend in DMX_CRTS_Horizontal)
                {
                    
                    if(rend.material != null && rend.material.name.Contains("Interpolated"))
                    {
                        float max = Mathf.Clamp01(rend.material.GetFloat("_MaximumSmoothnessDMX"));
                        float min = Mathf.Clamp01(rend.material.GetFloat("_MinimumSmoothnessDMX"));
                        rend.material.SetFloat("_MaximumSmoothnessDMX", Mathf.Clamp01(max/inGameInterpolationModifier));
                        rend.material.SetFloat("_MinimumSmoothnessDMX", Mathf.Clamp01(min/inGameInterpolationModifier));
                    }
                }
                foreach(CustomRenderTexture rend in DMX_CRTS_Vertical)
                {
                    if(rend.material != null && rend.material.name.Contains("Interpolated"))
                    {
                        float max = Mathf.Clamp01(rend.material.GetFloat("_MaximumSmoothnessDMX"));
                        float min = Mathf.Clamp01(rend.material.GetFloat("_MinimumSmoothnessDMX"));
                        rend.material.SetFloat("_MaximumSmoothnessDMX", Mathf.Clamp01(max/inGameInterpolationModifier));
                        rend.material.SetFloat("_MinimumSmoothnessDMX", Mathf.Clamp01(min/inGameInterpolationModifier));
                    }
                }
                foreach(CustomRenderTexture rend in DMX_CRTS_Legacy)
                {
                    if(rend.material != null && rend.material.name.Contains("Interpolated"))
                    {
                        float max = Mathf.Clamp01(rend.material.GetFloat("_MaximumSmoothnessDMX"));
                        float min = Mathf.Clamp01(rend.material.GetFloat("_MinimumSmoothnessDMX"));
                        rend.material.SetFloat("_MaximumSmoothnessDMX", Mathf.Clamp01(max/inGameInterpolationModifier));
                        rend.material.SetFloat("_MinimumSmoothnessDMX", Mathf.Clamp01(min/inGameInterpolationModifier));
                    }
                }
            }
        }


        public void OnEnable() 
        {
            _CheckDepthLightStatus();
        }
        void Start()
        {
            if(volumetricHighButton){
                defaultColorBlock = volumetricHighButton.colors;
                cbOn = defaultColorBlock;
                cbOn.normalColor = new Color(cbOn.normalColor.r + 0.35f, cbOn.normalColor.r + 0.35f, cbOn.normalColor.g + 0.35f, 1.0f);
            }
            if(bloomAnimator == null)
            {
                GameObject anim = GameObject.Find("PostProcessingExample-Bloom");
                if(anim != null)
                {
                    bloomAnimator = anim.GetComponent<Animator>();
                }
            }
            _SetTextureIDS();
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
            _SetVolumetricQualityMode();
            _SetBlinderProjectionQualityMode();
            _SetParProjectionQualityMode();
            _SetOtherProjectionQualityMode();
            _SetDiscoBallQualityMode();
            _SetLensFlareQualtiyMode();
            _CheckButtonLockStatus();

            #if !UNITY_EDITOR
                ReduceInGameInterpolation();
            #endif
            SetStrobeTextureStatus();
        }

        void _CheckButtonLockStatus()
        {
            Color disableColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);
            Color disableButEnabledColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            Color disabledTextColor = new Color(1.0f, 1.0f, 1.0f, 0.045f);
            if(lockVolumetricQualityMode)
            {
                if(volumetricHighButton){
                    volumetricHighButton.image.color = volumetricQuality == VolumetricQualityModes.High ? disableButEnabledColor : disableColor;
                    volumetricHighButton.interactable = false;}
                if(volumetricMedButton){
                    volumetricMedButton.image.color = volumetricQuality == VolumetricQualityModes.Medium ? disableButEnabledColor : disableColor;
                    volumetricMedButton.interactable = false;}
                if(volumetricLowButton){
                    volumetricLowButton.image.color = volumetricQuality == VolumetricQualityModes.Low ? disableButEnabledColor : disableColor;
                    volumetricLowButton.interactable = false;}
                if(volumetricHighText){volumetricHighText.color = disabledTextColor;}
                if(volumetricMedText){volumetricMedText.color = disabledTextColor;}
                if(volumetricLowText){volumetricLowText.color = disabledTextColor;}
            }
            if(lockBlinderProjectionQualityMode)
            {
                if(blinderProjectionHighButton){
                    blinderProjectionHighButton.image.color = blinderProjectionQuality == DefaultQualityModes.High ? disableButEnabledColor : disableColor;
                    blinderProjectionHighButton.interactable = false;}
                if(blinderProjectionLowButton){
                    blinderProjectionLowButton.image.color = blinderProjectionQuality == DefaultQualityModes.Low ? disableButEnabledColor : disableColor;
                    blinderProjectionLowButton.interactable = false;}
                if(blinderProjectionHighText){blinderProjectionHighText.color = disabledTextColor;}
                if(blinderProjectionLowText){blinderProjectionLowText.color = disabledTextColor;}
            }
            if(lockLensFlareQualityMode)
            {
                if(lensFlareHighButton){
                    lensFlareHighButton.image.color = lensFlareQuality == DefaultQualityModes.High ? disableButEnabledColor : disableColor;
                    lensFlareHighButton.interactable = false;}
                if(lensFlareLowButton){
                    lensFlareLowButton.image.color = lensFlareQuality == DefaultQualityModes.Low ? disableButEnabledColor : disableColor;
                    lensFlareLowButton.interactable = false;}
                if(lensFlareHighText){lensFlareHighText.color = disabledTextColor;}
                if(lensFlareLowText){lensFlareLowText.color = disabledTextColor;}
            }

            if(lockParProjectionQualityMode)
            {
                if(parProjectionHighButton){
                    parProjectionHighButton.image.color = parProjectionQuality == DefaultQualityModes.High ? disableButEnabledColor : disableColor;
                    parProjectionHighButton.interactable = false;}
                if(parProjectionLowButton){
                    parProjectionLowButton.image.color = parProjectionQuality == DefaultQualityModes.Low ? disableButEnabledColor : disableColor;
                    parProjectionLowButton.interactable = false;}
                if(parProjectionHighText){parProjectionHighText.color = disabledTextColor;}
                if(parProjectionLowText){parProjectionLowText.color = disabledTextColor;}
            }
            if(lockOtherProjectionQualityMode)
            {
                if(otherProjectionHighButton){
                    otherProjectionHighButton.image.color = otherProjectionQuality == DefaultQualityModes.High ? disableButEnabledColor : disableColor;
                    otherProjectionHighButton.interactable = false;}
                if(otherProjectionLowButton){
                    otherProjectionLowButton.image.color = otherProjectionQuality == DefaultQualityModes.Low ? disableButEnabledColor : disableColor;
                    otherProjectionLowButton.interactable = false;}
                if(otherProjectionHighText){otherProjectionHighText.color = disabledTextColor;}
                if(otherProjectionLowText){otherProjectionLowText.color = disabledTextColor;}
            }
            if(lockDiscoballQualityMode)
            {
                if(discoballHighButton){
                    discoballHighButton.image.color = discoballQuality == DefaultQualityModes.High ? disableButEnabledColor : disableColor;
                    discoballHighButton.interactable = false;}
                if(discoballLowButton){
                    discoballLowButton.image.color = discoballQuality == DefaultQualityModes.Low ? disableButEnabledColor : disableColor;
                    discoballLowButton.interactable = false;}
                if(discoballHighText){discoballHighText.color = disabledTextColor;}
                if(discoballLowText){discoballLowText.color = disabledTextColor;}
            }
        }

        public void _Test()
        {
            if(outputDebugLogs)
                Debug.Log("This is a test");
        }


    public void _SetVolumetricHigh()
    {
        if(lockVolumetricQualityMode){return;}
        volumetricQuality = VolumetricQualityModes.High;
        _SetVolumetricQualityMode();
    }
    public void _SetVolumetricMed()
    {
        if(lockVolumetricQualityMode){return;}
        volumetricQuality = VolumetricQualityModes.Medium;
        _SetVolumetricQualityMode();
    }
    public void _SetVolumetricLow()
    {
        if(lockVolumetricQualityMode){return;}
        volumetricQuality = VolumetricQualityModes.Low;
        _SetVolumetricQualityMode();
    }
    public void _SetProjectionBlindersHigh()
    {
        if(lockBlinderProjectionQualityMode){return;}
        blinderProjectionQuality = DefaultQualityModes.High;
        _SetBlinderProjectionQualityMode();
    }
    public void _SetProjectionBlindersLow()
    {
        if(lockBlinderProjectionQualityMode){return;}
        blinderProjectionQuality = DefaultQualityModes.Low;
        _SetBlinderProjectionQualityMode();
    }
    public void _SetProjectionParsHigh()
    {
        if(lockParProjectionQualityMode){return;}
        parProjectionQuality = DefaultQualityModes.High;
        _SetParProjectionQualityMode();
    }
    public void _SetProjectionParsLow()
    {
        if(lockParProjectionQualityMode){return;}
        parProjectionQuality = DefaultQualityModes.Low;
        _SetParProjectionQualityMode();
    }
    public void _SetProjectionOtherHigh()
    {
        if(lockOtherProjectionQualityMode){return;}
        otherProjectionQuality = DefaultQualityModes.High;
        _SetOtherProjectionQualityMode();
    }
    public void _SetProjectionOtherLow()
    {
        if(lockOtherProjectionQualityMode){return;}
        otherProjectionQuality = DefaultQualityModes.Low;
        _SetOtherProjectionQualityMode();
    }
    public void _SetDiscoballHigh()
    {
        if(lockDiscoballQualityMode){return;}
        discoballQuality = DefaultQualityModes.High;
        _SetDiscoBallQualityMode();
    }
    public void _SetDiscoballLow()
    {
        if(lockDiscoballQualityMode){return;}
        discoballQuality = DefaultQualityModes.Low;
        _SetDiscoBallQualityMode();
    }
    public void _SetLensFlareHigh()
    {
        if(lockLensFlareQualityMode){return;}
        lensFlareQuality = DefaultQualityModes.High;
        _SetLensFlareQualtiyMode();
    }
    public void _SetLensFlareLow()
    {
        if(lockLensFlareQualityMode){return;}
        lensFlareQuality = DefaultQualityModes.Low;
        _SetLensFlareQualtiyMode();
    }
    public void _UpdateAllQualityModes()
    {
        _SetDiscoBallQualityMode();
        _SetVolumetricQualityMode();
        _SetParProjectionQualityMode();
        _SetOtherProjectionQualityMode();
        _SetBlinderProjectionQualityMode();
        _SetLensFlareQualtiyMode();
    }
    public void _SetVolumetricQualityMode()
    {
        switch(volumetricQuality)
        {
            case VolumetricQualityModes.High:
                if(volumetricHighButton){volumetricHighButton.colors = cbOn;}
                if(volumetricMedButton){volumetricMedButton.colors = defaultColorBlock;}
                if(volumetricLowButton){volumetricLowButton.colors = defaultColorBlock;}
                break;
            case VolumetricQualityModes.Medium:
                if(volumetricHighButton){volumetricHighButton.colors = defaultColorBlock;}
                if(volumetricMedButton){volumetricMedButton.colors = cbOn;}
                if(volumetricLowButton){volumetricLowButton.colors = defaultColorBlock;}
                break;
            case VolumetricQualityModes.Low:
                if(volumetricHighButton){volumetricHighButton.colors = defaultColorBlock;}
                if(volumetricMedButton){volumetricMedButton.colors = defaultColorBlock;}
                if(volumetricLowButton){volumetricLowButton.colors = cbOn;}
                break;
            default:
                break;
        }
        SetVolumetricQuality();
    }
    public void _SetBlinderProjectionQualityMode()
    {
        
        switch(blinderProjectionQuality)
        {
            case DefaultQualityModes.High:
                if(blinderProjectionHighButton){blinderProjectionHighButton.colors = cbOn;}
                if(blinderProjectionLowButton){blinderProjectionLowButton.colors = defaultColorBlock;}
                break;
            case DefaultQualityModes.Low:
                if(blinderProjectionHighButton){blinderProjectionHighButton.colors = defaultColorBlock;}
                if(blinderProjectionLowButton){blinderProjectionLowButton.colors = cbOn;}
                break;
            default:
                break;
        }
        SetBlinderProjectionQuality();
    }

    public void _SetParProjectionQualityMode()
    {
        
        switch(parProjectionQuality)
        {
            case DefaultQualityModes.High:
                if(parProjectionHighButton){parProjectionHighButton.colors = cbOn;}
                if(parProjectionLowButton){parProjectionLowButton.colors = defaultColorBlock;}
                break;
            case DefaultQualityModes.Low:
                if(parProjectionHighButton){parProjectionHighButton.colors = defaultColorBlock;}
                if(parProjectionLowButton){parProjectionLowButton.colors = cbOn;}
                break;
            default:
                break;
        }
        SetParProjectionQuality();
    }
    public void _SetOtherProjectionQualityMode()
    {
        
        switch(otherProjectionQuality)
        {
            case DefaultQualityModes.High:
                if(otherProjectionHighButton){otherProjectionHighButton.colors = cbOn;}
                if(otherProjectionLowButton){otherProjectionLowButton.colors = defaultColorBlock;}
                break;
            case DefaultQualityModes.Low:
                if(otherProjectionHighButton){otherProjectionHighButton.colors = defaultColorBlock;}
                if(otherProjectionLowButton){otherProjectionLowButton.colors = cbOn;}
                break;
            default:
                break;
        }
        SetOtherProjectionQuality();
    }
    public void _SetDiscoBallQualityMode()
    {
        
        switch(discoballQuality)
        {
            case DefaultQualityModes.High:
                if(discoballHighButton){discoballHighButton.colors = cbOn;}
                if(discoballLowButton){discoballLowButton.colors = defaultColorBlock;}
                break;
            case DefaultQualityModes.Low:
                if(discoballHighButton){discoballHighButton.colors = defaultColorBlock;}
                if(discoballLowButton){discoballLowButton.colors = cbOn;}
                break;
            default:
                break;
        }
        SetDiscoballQuality();
    }
    public void _SetLensFlareQualtiyMode()
    {
        switch(lensFlareQuality)
        {
            case DefaultQualityModes.High:
                if(lensFlareHighButton){lensFlareHighButton.colors = cbOn;}
                if(lensFlareLowButton){lensFlareLowButton.colors = defaultColorBlock;}
                break;
            case DefaultQualityModes.Low:
                if(lensFlareHighButton){lensFlareHighButton.colors = defaultColorBlock;}
                if(lensFlareLowButton){lensFlareLowButton.colors = cbOn;}
                break;
            default:
                break;
        }
        SetLensFlareQuality();
    }
        public void _CheckDepthLightStatus()
        {

            foreach(Material mat in volumetricMaterials)
            {
                mat.SetInt("_PotatoMode", VolumetricNoise ? 0 : 1);
                mat.SetInt("_UseDepthLight", RequireDepthLight ? 1 : 0);
                if(mat.HasProperty("_UseDepthLight")){
                SetKeyword(mat, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(mat.GetInt("_UseDepthLight"))) == 1 ? true : false);}
                if(mat.HasProperty("_MAGIC_NOISE_ON_MED")){
                SetKeyword(mat, "_MAGIC_NOISE_ON_MED", (Mathf.FloorToInt(mat.GetInt("_MAGIC_NOISE_ON_MED"))) == 1 ? true : false);}
                if(mat.HasProperty("_MAGIC_NOISE_ON_HIGH")){
                SetKeyword(mat, "_MAGIC_NOISE_ON_HIGH", (Mathf.FloorToInt(mat.GetInt("_MAGIC_NOISE_ON_HIGH"))) == 1 ? true : false);}
                if(mat.HasProperty("_PotatoMode")){
                SetKeyword(mat, "_POTATO_MODE_ON", (Mathf.FloorToInt(mat.GetInt("_PotatoMode"))) == 1 ? true : false);}
            }
            foreach(Material mat in projectionMaterials)
            {
                mat.SetInt("_UseDepthLight", RequireDepthLight ? 1 : 0);
            }
            if(fixtureMaterials != null)
            {
                foreach(Material mat in fixtureMaterials)
                {
                    if(mat != null)
                    {
                        mat.SetInt("_UseDepthLight", RequireDepthLight ? 1 : 0);
                        if(mat.HasProperty("_UseDepthLight")){
                        SetKeyword(mat, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(mat.GetInt("_UseDepthLight"))) == 1 ? true : false);}
                    }
                }
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
                #if UNITY_2022
                    rt.updatePeriod = _targetCRTUpdateRate;
                #endif
                if(rt.name.Contains("Color"))
                {
                    if(outputDebugLogs){
                        Debug.Log("DMX Color: " + rt.name);}
                    #if UDONSHARP
                    VRCShader.SetGlobalTexture(_Udon_DMXGridRenderTexture, rt);
                    #else
                    Shader.SetGlobalTexture(_Udon_DMXGridRenderTexture, rt, RenderTextureSubElement.Default);
                    #endif
                }
                else if(rt.name.Contains("Movement"))
                {
                    if(outputDebugLogs){
                        Debug.Log("DMX Movement: " + rt.name);}
                    #if UDONSHARP
                    VRCShader.SetGlobalTexture(_Udon_DMXGridRenderTextureMovement, rt);
                    #else
                    Shader.SetGlobalTexture(_Udon_DMXGridRenderTextureMovement, rt, RenderTextureSubElement.Default);
                    #endif
                }
                else if(rt.name.Contains("Spin"))
                {
                    if(outputDebugLogs){
                        Debug.Log("DMX Spin Timings: " + rt.name);}
                    #if UDONSHARP
                    VRCShader.SetGlobalTexture(_Udon_DMXGridSpinTimer, rt);
                    #else
                    Shader.SetGlobalTexture(_Udon_DMXGridSpinTimer, rt, RenderTextureSubElement.Default);
                    #endif
                }
                else if(rt.name.Contains("Strobe"))
                {
                    if(rt.name.Contains("Timings"))
                    {
                        if(outputDebugLogs){
                            Debug.Log("DMX Strobe Timings: " + rt.name);}
                        #if UDONSHARP
                        VRCShader.SetGlobalTexture(_Udon_DMXGridStrobeTimer, rt);
                        #else
                        Shader.SetGlobalTexture(_Udon_DMXGridStrobeTimer, rt, RenderTextureSubElement.Default);
                        #endif
                    }
                    else
                    {
                        //Debug.Log("Setting Strobe Output");
                        if(delayStrobeForGI)
                        {
                            if(rt.name.Contains("Delay-Final") && DMXMode != LEGACY_MODE)
                            {
                                #if UDONSHARP
                                if(outputDebugLogs){
                                    Debug.Log("DMX Strobe Output: " + rt.name);}
                                VRCShader.SetGlobalTexture(_Udon_DMXGridStrobeOutput, rt);
                                #else
                                Shader.SetGlobalTexture(_Udon_DMXGridStrobeOutput, rt, RenderTextureSubElement.Default);
                                #endif
                            }
                            else if(DMXMode == LEGACY_MODE)
                            {
                                #if UDONSHARP
                                if(outputDebugLogs){
                                    Debug.Log("DMX Strobe Output: " + rt.name);}
                                VRCShader.SetGlobalTexture(_Udon_DMXGridStrobeOutput, rt);
                                #else
                                Shader.SetGlobalTexture(_Udon_DMXGridStrobeOutput, rt, RenderTextureSubElement.Default);
                                #endif  
                            }
                        }
                        else
                        {
                            if(rt.name.Contains("Delay") == false)
                            {           
                                #if UDONSHARP
                                if(outputDebugLogs){
                                    Debug.Log("Strobe Output: " + rt.name);}
                                VRCShader.SetGlobalTexture(_Udon_DMXGridStrobeOutput, rt);
                                #else
                                Shader.SetGlobalTexture(_Udon_DMXGridStrobeOutput, rt, RenderTextureSubElement.Default);
                                #endif
                            }
                        }
                    }
                }
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

        public void _SetDMXHorizontal()
        {
            if(isUsingDMX)
            {
                DMXMode = HORIZONTAL_MODE;
                _CheckDMX();
            }
        }
        public void _SetDMXVertical()
        {
            if(isUsingDMX)
            {
                DMXMode = VERTICAL_MODE;
                _CheckDMX();
            }
        }
        public void _SetDMXLegacy()
        {
            if(isUsingDMX)
            {
                DMXMode = LEGACY_MODE;
                _CheckDMX();
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
                if(mat != null)
                {
                    mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, fixtureIntensityMax, fixtureSlider.value));
                }
            }
            fixtureSliderText.text = Mathf.Round(fixtureSlider.value * 100.0f).ToString();
        }

        public void _SetVolumetricIntensity()
        {
            foreach(Material mat in volumetricMaterials)
            {
                if(mat != null)
                {
                    mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, volumetricIntensityMax, volumetricSlider.value));
                }
            }
            volumetricSliderText.text = Mathf.Round(volumetricSlider.value * 100.0f).ToString();
        }

        public void _SetProjectionIntensity()
        {
            foreach(Material mat in projectionMaterials)
            {
                if(mat != null)
                {
                    mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, projectionIntensityMax, projectionSlider.value));
                }
            }
            projectionSliderText.text = Mathf.Round(projectionSlider.value * 100.0f).ToString();
        }

        public void _SetDiscoBallIntensity()
        {
            foreach(Material mat in discoBallMaterials)
            {
                if(mat != null)
                {
                    mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, discoballIntensityMax, discoBallSlider.value));
                }
            }
            discoBallSliderText.text = Mathf.Round(discoBallSlider.value * 100.0f).ToString();
        }

        public void _SetLaserIntensity()
        {
            foreach(Material mat in laserMaterials)
            {
                if(mat != null)
                {
                    mat.SetFloat("_UniversalIntensity", Mathf.Lerp(0.0f, laserIntensityMax, laserSlider.value));
                }
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
            else
            {
                bloomSlider.gameObject.SetActive(false);
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

        void SetVolumetricQuality()
        {
            foreach(Material target in volumetricMaterials)
            {
                if(target == null){continue;}
                if(volumetricQuality == VolumetricQualityModes.High)
                {
                    target.SetOverrideTag("RenderType", "Transparent");
                    target.DisableKeyword("_ALPHATEST_ON");  
                    //target.SetInt("_BlendSrc", 1);
                    target.SetInt("_BlendDst", 1);
                    target.SetInt("_ZWrite", 0);
                    target.SetInt("_AlphaToCoverage", 0);
                    target.SetInt("_HQMode", 1);
                    target.SetInt("_RenderMode", 0);
                    if(target.HasProperty("_MAGIC_NOISE_ON_MED")){
                    SetKeyword(target, "_MAGIC_NOISE_ON_MED", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_MED"))) == 1 ? true : false);}
                    if(target.HasProperty("_MAGIC_NOISE_ON_HIGH")){
                    SetKeyword(target, "_MAGIC_NOISE_ON_HIGH", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_HIGH"))) == 1 ? true : false);}
                    if(target.HasProperty("_UseDepthLight")){
                    SetKeyword(target, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(target.GetInt("_UseDepthLight"))) == 1 ? true : false);}
                    if(target.HasProperty("_PotatoMode")){
                    SetKeyword(target, "_POTATO_MODE_ON", (Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1 ? true : false);}
                    if(target.HasProperty("_HQMode")){
                    SetKeyword(target, "_HQ_MODE", (Mathf.FloorToInt(target.GetInt("_HQMode"))) == 1 ? true : false);}
                    //SetKeyword(target, "_2D_NOISE_ON", (Mathf.FloorToInt(target.GetInt("_2D_NOISE_ON"))) == 1 ? true : false);
                    target.renderQueue = 3002;
                }
                else if(volumetricQuality == VolumetricQualityModes.Medium) 
                {
                    target.SetOverrideTag("RenderType", "Transparent");
                    target.DisableKeyword("_ALPHATEST_ON");  
                    //target.SetInt("_BlendSrc", 1);
                    target.SetInt("_BlendDst", 1);
                    target.SetInt("_ZWrite", 0);
                    target.SetInt("_AlphaToCoverage", 0);
                    target.SetInt("_HQMode", 0);
                    target.SetInt("_RenderMode", 1);
                    if(target.HasProperty("_MAGIC_NOISE_ON_MED")){
                    SetKeyword(target, "_MAGIC_NOISE_ON_MED", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_MED"))) == 1 ? true : false);}
                    if(target.HasProperty("_MAGIC_NOISE_ON_HIGH")){
                    SetKeyword(target, "_MAGIC_NOISE_ON_HIGH", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_HIGH"))) == 1 ? true : false);}
                    if(target.HasProperty("_UseDepthLight")){
                    SetKeyword(target, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(target.GetInt("_UseDepthLight"))) == 1 ? true : false);}
                    if(target.HasProperty("_PotatoMode")){
                    SetKeyword(target, "_POTATO_MODE_ON", (Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1 ? true : false);}
                    if(target.HasProperty("_HQMode")){
                    SetKeyword(target, "_HQ_MODE", (Mathf.FloorToInt(target.GetInt("_HQMode"))) == 1 ? true : false);}
                    //SetKeyword(target, "_2D_NOISE_ON", (Mathf.FloorToInt(target.GetInt("_2D_NOISE_ON"))) == 1 ? true : false);
                    target.renderQueue = 3002;
                }
                else
                {
                    target.SetOverrideTag("RenderType", "Opaque");
                    target.EnableKeyword("_ALPHATEST_ON");
                    //target.SetInt("_BlendSrc", 0);
                    target.SetInt("_BlendDst", 0);
                    target.SetInt("_ZWrite", 1);
                    target.SetInt("_AlphaToCoverage", 1);
                    target.SetInt("_HQMode", 0);
                    target.SetInt("_RenderMode", 2);
                    if(target.HasProperty("_MAGIC_NOISE_ON_MED")){
                    SetKeyword(target, "_MAGIC_NOISE_ON_MED", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_MED"))) == 1 ? true : false);}
                    if(target.HasProperty("_MAGIC_NOISE_ON_HIGH")){
                    SetKeyword(target, "_MAGIC_NOISE_ON_HIGH", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_HIGH"))) == 1 ? true : false);}
                    if(target.HasProperty("_UseDepthLight")){
                    SetKeyword(target, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(target.GetInt("_UseDepthLight"))) == 1 ? true : false);}
                    if(target.HasProperty("_PotatoMode")){
                    SetKeyword(target, "_POTATO_MODE_ON", (Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1 ? true : false);}
                    if(target.HasProperty("_HQMode")){
                    SetKeyword(target, "_HQ_MODE", (Mathf.FloorToInt(target.GetInt("_HQMode"))) == 1 ? true : false);}
                    //SetKeyword(target, "_2D_NOISE_ON", (Mathf.FloorToInt(target.GetInt("_2D_NOISE_ON"))) == 1 ? true : false);
                    target.renderQueue = 2452;
                }
            }
        }
        void SetBlinderProjectionQuality()
        {
            foreach(Material target in projectionMaterials)
            {
                if(target == null){continue;}
                if(target.name.Contains("Blinder"))
                {
                    if(blinderProjectionQuality == DefaultQualityModes.High) 
                    {
                        target.SetOverrideTag("RenderType", "Transparent");
                        target.DisableKeyword("_ALPHATEST_ON");  
                        //target.SetInt("_BlendSrc", 1);
                        target.SetInt("_BlendDst", 1);
                        target.SetInt("_ZWrite", 0);
                        target.SetInt("_AlphaToCoverage", 0);
                        target.SetInt("_RenderMode", 1);
                        target.renderQueue = 3001;
                    }
                    else
                    {
                        target.SetOverrideTag("RenderType", "Opaque");
                        target.EnableKeyword("_ALPHATEST_ON");
                        //target.SetInt("_BlendSrc", 0);
                        target.SetInt("_BlendDst", 0);
                        target.SetInt("_ZWrite", 1);
                        target.SetInt("_AlphaToCoverage", 1);
                        target.SetInt("_RenderMode", 2);
                        target.renderQueue = 2451;
                    }
                }
            }
        }
        void SetParProjectionQuality()
        {
            foreach(Material target in projectionMaterials)
            {
                if(target == null){continue;}
                if(target.name.Contains("Par"))
                {
                    if(parProjectionQuality == DefaultQualityModes.High) 
                    {
                        target.SetOverrideTag("RenderType", "Transparent");
                        target.DisableKeyword("_ALPHATEST_ON");  
                        //target.SetInt("_BlendSrc", 1);
                        target.SetInt("_BlendDst", 1);
                        target.SetInt("_ZWrite", 0);
                        target.SetInt("_AlphaToCoverage", 0);
                        target.SetInt("_RenderMode", 1);
                        target.renderQueue = 3001;
                    }
                    else
                    {
                        target.SetOverrideTag("RenderType", "Opaque");
                        target.EnableKeyword("_ALPHATEST_ON");
                        //target.SetInt("_BlendSrc", 0);
                        target.SetInt("_BlendDst", 0);
                        target.SetInt("_ZWrite", 1);
                        target.SetInt("_AlphaToCoverage", 1);
                        target.SetInt("_RenderMode", 2);
                        target.renderQueue = 2451;
                    }
                }
            }
        }
        void SetOtherProjectionQuality()
        {
            foreach(Material target in projectionMaterials)
            {
                if(target == null){continue;}
                if(target.name.Contains("Par") == false && target.name.Contains("Blinder")==false)
                {
                    if(otherProjectionQuality == DefaultQualityModes.High) 
                    {
                        target.SetOverrideTag("RenderType", "Transparent");
                        target.DisableKeyword("_ALPHATEST_ON");  
                        //target.SetInt("_BlendSrc", 1);
                        target.SetInt("_BlendDst", 1);
                        target.SetInt("_ZWrite", 0);
                        target.SetInt("_AlphaToCoverage", 0);
                        target.SetInt("_RenderMode", 1);
                        target.renderQueue = 3001;
                    }
                    else
                    {
                        target.SetOverrideTag("RenderType", "Opaque");
                        target.EnableKeyword("_ALPHATEST_ON");
                        //target.SetInt("_BlendSrc", 0);
                        target.SetInt("_BlendDst", 0);
                        target.SetInt("_ZWrite", 1);
                        target.SetInt("_AlphaToCoverage", 1);
                        target.SetInt("_RenderMode", 2);
                        target.renderQueue = 2451;
                    }
                }
            }
        }

        void SetDiscoballQuality()
        {
            foreach(Material target in discoBallMaterials)
            {
                if(target == null){continue;}
                if(discoballQuality == DefaultQualityModes.High) 
                {
                    target.SetOverrideTag("RenderType", "Transparent");
                    target.DisableKeyword("_ALPHATEST_ON");  
                    //target.SetInt("_BlendSrc", 1);
                    target.SetInt("_BlendDst", 1);
                    target.SetInt("_ZWrite", 0);
                    target.SetInt("_AlphaToCoverage", 0);
                    target.SetInt("_RenderMode", 1);
                    target.renderQueue = 3001;
                }
                else
                {
                    target.SetOverrideTag("RenderType", "Opaque");
                    target.EnableKeyword("_ALPHATEST_ON");
                    //target.SetInt("_BlendSrc", 0);
                    target.SetInt("_BlendDst", 0);
                    target.SetInt("_ZWrite", 1);
                    target.SetInt("_AlphaToCoverage", 1);
                    target.SetInt("_RenderMode", 2);
                    target.renderQueue = 2451;
                }
            }
        }
        void SetLensFlareQuality()
        {
            foreach(Material target in fixtureMaterials)
            {
                if(target == null){continue;}
                if(target.name.Contains("Flare"))
                {
                    if(lensFlareQuality == DefaultQualityModes.High) 
                    {
                        target.SetOverrideTag("RenderType", "Transparent");
                        target.DisableKeyword("_ALPHATEST_ON");  
                        //target.SetInt("_BlendSrc", 1);
                        target.SetInt("_BlendDst", 1);
                        target.SetInt("_ZWrite", 0);
                        target.SetInt("_AlphaToCoverage", 0);
                        target.SetInt("_RenderMode", 1);
                        target.renderQueue = 3001;
                    }
                    else
                    {
                        target.SetOverrideTag("RenderType", "Opaque");
                        target.EnableKeyword("_ALPHATEST_ON");
                        //target.SetInt("_BlendSrc", 0);
                        target.SetInt("_BlendDst", 0);
                        target.SetInt("_ZWrite", 1);
                        target.SetInt("_AlphaToCoverage", 1);
                        target.SetInt("_RenderMode", 2);
                        target.renderQueue = 2451;
                    }
                }
            }
        }


        #if !COMPILER_UDONSHARP && UNITY_EDITOR

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

        public void _GetNewMaterials()
        {
            List<GameObject> sceneObjects = GetAllObjectsOnlyInScene();
            List<Material> freshFixtureMats = new List<Material>();
            freshFixtureMats.AddRange(fixtureMaterials);
            List<Material> freshVolumetricMats = new List<Material>();
            freshVolumetricMats.AddRange(volumetricMaterials);
            List<Material> freshProjectionMats = new List<Material>();
            freshProjectionMats.AddRange(projectionMaterials);
            foreach(GameObject go in sceneObjects)
            {
                MeshRenderer rend = go.GetComponent<MeshRenderer>();
                if(rend != null)
                {
                    if(go.name.Contains("Fixture") && (go.name.Contains("Lamp") || go.name.Contains("Mesh")))
                    {
                        if(!freshFixtureMats.Contains(rend.sharedMaterial) && ((rend.sharedMaterial.shader.FindPropertyIndex("_Band") != -1) || rend.sharedMaterial.shader.FindPropertyIndex("_DMXChannel") != -1))
                        {
                            freshFixtureMats.Add(rend.sharedMaterial);
                        }
                    }
                    else if(go.name.Contains("Volumetric") && ((rend.sharedMaterial.shader.FindPropertyIndex("_Band") != -1) || rend.sharedMaterial.shader.FindPropertyIndex("_DMXChannel") != -1))
                    {
                        if(!freshVolumetricMats.Contains(rend.sharedMaterial))
                        {
                            freshVolumetricMats.Add(rend.sharedMaterial);
                        }
                    }
                    else if (go.name.Contains("Projection") && ((rend.sharedMaterial.shader.FindPropertyIndex("_Band") != -1) || rend.sharedMaterial.shader.FindPropertyIndex("_DMXChannel") != -1))
                    {
                        if(!freshProjectionMats.Contains(rend.sharedMaterial))
                        {
                            freshProjectionMats.Add(rend.sharedMaterial);
                        }
                    }
                    else{continue;}
                }
            }

            fixtureMaterials = freshFixtureMats.ToArray();
            volumetricMaterials = freshVolumetricMats.ToArray();
            projectionMaterials = freshProjectionMats.ToArray();
            if(PrefabUtility.IsPartOfAnyPrefab(this))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        }
        #endif
    }

        #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(VRSL_LocalUIControlPanel))]
    public class VRSL_LocalUIControlPanel_Editor : Editor
    {
        public static Texture logo;
        //public static string ver = "VR Stage Lighting ver:" + " <b><color=#6a15ce> 2.1</color></b>";
        SerializedProperty audioLinkLasers, audiolinkLights, dmxLights, isUsingDMX,isUsingAudioLink, fixtureDefGUID, volumetricMeshQuality;

        static string GetVersion()
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

        public void OnEnable() 
        {
            logo = Resources.Load("VRStageLighting-Logo") as Texture;
            audioLinkLasers = serializedObject.FindProperty("audioLinkLasers");
            audiolinkLights = serializedObject.FindProperty("audiolinkLights");
            dmxLights = serializedObject.FindProperty("dmxLights");
            isUsingDMX = serializedObject.FindProperty("isUsingDMX");
            isUsingAudioLink = serializedObject.FindProperty("isUsingAudioLink");
            fixtureDefGUID = serializedObject.FindProperty("fixtureDefGUID");
            volumetricMeshQuality = serializedObject.FindProperty("volumetricMeshQuality");
        }
        public void _RemoveEmptyMaterials()
        {
            VRSL_LocalUIControlPanel controlPanel = (VRSL_LocalUIControlPanel)target;
            int count = 0;
            for(int i = 0; i < controlPanel.fixtureMaterials.Length; i++)
            {
                if(controlPanel.fixtureMaterials[i] == null)
                {
                    count++;
                }
            }
            Material[] newArray = new Material[controlPanel.fixtureMaterials.Length - count];
            int otherCount = 0;
            for(int i = 0; i < controlPanel.fixtureMaterials.Length; i++)
            {
                if(controlPanel.fixtureMaterials[i] != null)
                {
                    newArray[otherCount] = controlPanel.fixtureMaterials[i];
                    otherCount++;
                }
            }
            controlPanel.fixtureMaterials = newArray;

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
        GUIContent Label(string label)
        {
            GUIContent content = new GUIContent();
            content.text = label;
            return content;
        }
        public override void OnInspectorGUI()
        {
#if UDONSHARP
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
#endif
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            DrawLogo();
            ShurikenHeaderCentered(GetVersion());
            EditorGUILayout.Space();
            VRSL_LocalUIControlPanel controlPanel = (VRSL_LocalUIControlPanel)target;
            if (GUILayout.Button(new GUIContent("Force Update Target AudioLink Sample Texture", "Updates all AudioLink VRSL Fixtures to sample from the selected target texture when texture sampling is enabled on the fixture."))) { controlPanel._ForceUpdateVideoSampleTexture(); }
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Apply Quality Modes to All Materials", "Applies currently set quality modes to all materials."))) { controlPanel._UpdateAllQualityModes(); }
            EditorGUILayout.Space();
                if (GUILayout.Button(new GUIContent("Search For VRSL Materials", "Adds VRSL Compatible Materials in scene to materials lists"))) {controlPanel._GetNewMaterials(); }
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Remove Empty Materials", "Removes all Empty Material slots from material lists."))) {_RemoveEmptyMaterials(); }
            EditorGUILayout.Space();
            if(isUsingDMX.boolValue)
            {
                // EditorGUILayout.PropertyField(dmxLights,true);
                for(int i = 0; i < dmxLights.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(dmxLights.GetArrayElementAtIndex(i));
                }
            }
            if(isUsingAudioLink.boolValue)
            {
                for(int i = 0; i < dmxLights.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(audiolinkLights.GetArrayElementAtIndex(i));
                    EditorGUILayout.PropertyField(audioLinkLasers.GetArrayElementAtIndex(i));
                // EditorGUILayout.PropertyField(audiolinkLights, true);
                // EditorGUILayout.PropertyField(audioLinkLasers,true);
                }
            }
            EditorGUILayout.LabelField("Fixture Definition GUID: " + fixtureDefGUID.stringValue);
            EditorGUILayout.LabelField("Volumetric Mesh Quality State: " + volumetricMeshQuality.intValue);
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                //Debug.Log("Found changes");
                serializedObject.ApplyModifiedProperties();
                Repaint();
            }
        }
    }
    #endif
}
