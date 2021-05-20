
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_LocalUIControlPanel : UdonSharpBehaviour
{
    [Header("Materials")]
    public Material[] fixtureMaterials;
    public Material[] volumetricMaterials;
    public Material[] projectionMaterials;
    public Material[] discoBallMaterials;
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
    public UnityEngine.UI.Slider bloomSlider;
    public UnityEngine.UI.Text masterSliderText, fixtureSliderText, volumetricSliderText, projectionSliderText, discoBallSliderText, bloomSliderText;
    public float fixtureIntensityMax = 1.0f, volumetricIntensityMax = 1.0f, projectionIntensityMax = 1.0f, discoballIntensityMax = 1.0f;

    void Start()
    {
        _SetFinalIntensity();
        _SetFixtureIntensity();
        _SetVolumetricIntensity();
        _SetProjectionIntensity();
        _SetDiscoBallIntensity();
        _SetBloomIntensity();
    }
    
    public void _SetFinalIntensity()
    {
        fixtureIntensityMax = masterSlider.value;
        volumetricIntensityMax = masterSlider.value;
        projectionIntensityMax = masterSlider.value;
        discoballIntensityMax = masterSlider.value;
        _SetFixtureIntensity();
        _SetVolumetricIntensity();
        _SetProjectionIntensity();
        _SetDiscoBallIntensity();
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
    public void _SetBloomIntensity()
    {
        if(bloomAnimator != null)
        {
            bloomAnimator.SetFloat("BloomIntensity", bloomSlider.value);
            bloomSliderText.text = Mathf.Round(bloomSlider.value * 100.0f).ToString();
        }
    }
}
