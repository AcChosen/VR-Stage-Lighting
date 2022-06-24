
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]

public class VRSL_LocalUIControlPanel : UdonSharpBehaviour
{
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


    void Start()
    {
        _SetFinalIntensity();
        _SetFixtureIntensity();
        _SetVolumetricIntensity();
        _SetProjectionIntensity();
        _SetDiscoBallIntensity();
        _SetBloomIntensity();
        _CheckDMX();
        _CheckAudioLink();
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
}
