
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Udon;

public class VRSL_AudioLink_SmoothingPanel : UdonSharpBehaviour
{
    public Material smoothingMaterial;
    public Slider bassSmoothingSlider, lowerMidSmoothingSlider, upperMidSmoothingSlider, trebleSmoothingSlider, colorChordSmoothingSlider;
    public Text bassSmoothingText, lowerMidSmoothingText, upperMidSmoothingText, trebleSmoothingText, colorChordSmoothingText;
    float bassSmoothingInit, lowerMidSmoothingInit, upperMidSmoothingInit, trebleSmoothingInit, colorChordSmoothingInit;

    void Start() 
    {
        bassSmoothingInit = bassSmoothingSlider.value;
        lowerMidSmoothingInit = lowerMidSmoothingSlider.value;
        upperMidSmoothingInit = upperMidSmoothingSlider.value;
        trebleSmoothingInit = trebleSmoothingSlider.value;
        colorChordSmoothingInit = colorChordSmoothingSlider.value;
        UpdateSettings();
    }

    void UpdateText()
    {
        bassSmoothingText.text = (( (Mathf.Abs(bassSmoothingSlider.value - 1.0f))*100.0f).ToString());
        lowerMidSmoothingText.text = (((Mathf.Abs(lowerMidSmoothingSlider.value - 1.0f))*100.0f).ToString());
        upperMidSmoothingText.text = (((Mathf.Abs(upperMidSmoothingSlider.value - 1.0f))*100.0f).ToString());
        trebleSmoothingText.text = (((Mathf.Abs(trebleSmoothingSlider.value - 1.0f))*100.0f).ToString());
        colorChordSmoothingText.text = (((Mathf.Abs(colorChordSmoothingSlider.value - 1.0f))*100.0f).ToString());
    }

    public void UpdateSettings()
    {
        smoothingMaterial.SetFloat("_Band0Smoothness", bassSmoothingSlider.value);
        smoothingMaterial.SetFloat("_Band1Smoothness", lowerMidSmoothingSlider.value);
        smoothingMaterial.SetFloat("_Band2Smoothness", upperMidSmoothingSlider.value);
        smoothingMaterial.SetFloat("_Band3Smoothness", trebleSmoothingSlider.value);
        smoothingMaterial.SetFloat("_LightColorChordSmoothenss", colorChordSmoothingSlider.value);
        UpdateText();
    }

    public void ResetSettings()
    {
        bassSmoothingSlider.value = bassSmoothingInit;
        lowerMidSmoothingSlider.value = lowerMidSmoothingInit;
        upperMidSmoothingSlider.value = upperMidSmoothingInit;
        trebleSmoothingSlider.value = trebleSmoothingInit;
        colorChordSmoothingSlider.value = colorChordSmoothingInit;
    }
}
