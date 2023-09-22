#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.IO;

// Based on Morioh's toon shader GUI.
// This code is based off synqark's arktoon-shaders and Xiexe. 
// Citation to "https://github.com/synqark", "https://github.com/synqark/arktoon-shaders", https://gitlab.com/xMorioh/moriohs-toon-shader.
// public enum VolumetricQuality
// {
//     Potato,
//     Default,
//     HQ
// }

public class VRSLInspector : ShaderGUI
{
    
    BindingFlags bindingFlags = BindingFlags.Public |
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Static;

    MaterialProperty _LightingModel = null;


    // MaterialProperty _AreaLitToggle = null;
	// MaterialProperty _AreaLitStrength = null;
	// MaterialProperty _AreaLitRoughnessMult = null;
    // MaterialProperty _OcclusionUVSet = null;
	// MaterialProperty _AreaLitOcclusion = null;
    // MaterialProperty _AreaLitMask = null;
    // MaterialProperty _LightMesh = null;
    // MaterialProperty _LightTex0 = null;
    // MaterialProperty _LightTex1 = null;
    // MaterialProperty _LightTex2 = null;
    // MaterialProperty _LightTex3 = null;
    // MaterialProperty _OpaqueLights = null;



    MaterialProperty _DMXChannel = null;
    MaterialProperty _NineUniverseMode = null;
    MaterialProperty _EnableDMX = null;
    MaterialProperty _EnableExtraChannels = null;
    // MaterialProperty _Udon_DMXGridRenderTextureMovement = null;
    // MaterialProperty _Udon_DMXGridRenderTexture = null;
    // MaterialProperty _Udon_DMXGridStrobeTimer = null;
    // MaterialProperty _Udon_DMXGridSpinTimer = null;
    MaterialProperty  _DMXTexture = null;
    MaterialProperty _UseRawGrid = null;
    MaterialProperty _EnableCompatibilityMode = null;
    MaterialProperty _EnableVerticalMode = null;
    MaterialProperty _EnableLegacyGlobalMovementSpeedChannel = null;

    MaterialProperty _GlobalIntensity = null;
    MaterialProperty _GlobalIntensityBlend = null;
    MaterialProperty _FinalIntensity = null;
    MaterialProperty _UniversalIntensity = null;
    MaterialProperty _Emission = null;
    MaterialProperty _CurveMod = null;
    MaterialProperty _ChannelMode = null;

    MaterialProperty _Saturation = null;
    MaterialProperty _SaturationLength = null;
    MaterialProperty _LensMaxBrightness = null;
    MaterialProperty _GoboBeamSplitEnable = null;
    MaterialProperty _ConeWidth = null;
    MaterialProperty _ConeLength = null;
    MaterialProperty _MaxConeLength = null;
    MaterialProperty _ConeSync = null;
    MaterialProperty _FixutreIntensityMultiplier = null;

    MaterialProperty _FixtureBaseRotationY = null;

    MaterialProperty _FixtureRotationX = null;

    //Mover Housing Specific
    MaterialProperty _FixtureRotationOrigin = null;
    //MaterialProperty _FixtureLensOrigin = null;
    MaterialProperty _MaxMinPanAngle = null;
    MaterialProperty _MaxMinTiltAngle = null;
    MaterialProperty _LightProbeMethod = null;
    MaterialProperty  _DecorativeEmissiveMap = null;
     MaterialProperty  _DecorativeEmissiveMapStrength = null;

    MaterialProperty _MainTex = null;
    MaterialProperty _Color = null;
    MaterialProperty _BumpMap = null;
    MaterialProperty _BumpScale = null;
    MaterialProperty _NormalMap = null;
    MaterialProperty _MetallicGlossMap = null;
    MaterialProperty _MetallicSmoothness = null;
    MaterialProperty _EmissionMask = null;
    MaterialProperty _Metallic = null;
    MaterialProperty _Glossiness = null;
    MaterialProperty _OcclusionMap = null;
    MaterialProperty _OcclusionStrength = null;
    
    //Volumetric Texture specific

    MaterialProperty _LightMainTex = null;




    MaterialProperty _NoiseTex = null;
    MaterialProperty _NoiseTexHigh = null;
    MaterialProperty _NoisePower = null;
    MaterialProperty _Noise2X = null;
    MaterialProperty _Noise2Y = null;
    MaterialProperty _Noise2Z = null;
    MaterialProperty _Noise2Stretch = null;
    MaterialProperty _Noise2StretchInside = null;
    MaterialProperty _Noise2Power = null;

    

    MaterialProperty _Noise2XDefault = null;
    MaterialProperty _Noise2YDefault = null;
    MaterialProperty _Noise2ZDefault = null;
    MaterialProperty _Noise2StretchDefault = null;
    MaterialProperty _Noise2StretchInsideDefault = null;
    MaterialProperty _Noise2PowerDefault = null;



    // MaterialProperty _Noise2XPotato = null;
    // MaterialProperty _Noise2YPotato = null;
    // MaterialProperty _Noise2ZPotato = null;
    // MaterialProperty _Noise2StretchPotato = null;
    // MaterialProperty _Noise2StretchInsidePotato = null;
    // MaterialProperty _Noise2PowerPotato = null;






    MaterialProperty _MAGIC_NOISE_ON_HIGH = null;
    MaterialProperty _MAGIC_NOISE_ON_MED = null;
    MaterialProperty _2D_NOISE_ON = null;
    MaterialProperty _UseDepthLight = null;
    MaterialProperty _PotatoMode = null;
    MaterialProperty _HQMode = null;
    MaterialProperty _GradientMod = null;
    MaterialProperty _GradientModGOBO = null;
    MaterialProperty _RenderMode = null;
   // MaterialProperty _ZWrite = null;
   // MaterialProperty _AlphaToCoverage = null;
   // MaterialProperty _InsideConeNormalMap = null;

    //Volumetric Control Specific
    MaterialProperty _FixtureMaxIntensity = null;
    MaterialProperty _RenderTextureMultiplier = null;
    MaterialProperty _FadeStrength = null;
    MaterialProperty _InnerFadeStrength = null;
    MaterialProperty _InnerIntensityCurve = null;
    MaterialProperty _DistFade = null;
    MaterialProperty _FadeAmt = null;
    MaterialProperty _BlindingAngleMod = null;
    MaterialProperty _BlindingStrength = null; 
    MaterialProperty _StripeSplit = null;
    MaterialProperty _StripeSplitStrength = null;
    MaterialProperty _StripeSplit2 = null;
    MaterialProperty _StripeSplitStrength2 = null;
    MaterialProperty _StripeSplit3 = null;
    MaterialProperty _StripeSplitStrength3 = null;
    MaterialProperty _StripeSplit4 = null;
    MaterialProperty _StripeSplitStrength4 = null;
    MaterialProperty _StripeSplit5 = null;
    MaterialProperty _StripeSplitStrength5 = null;
    MaterialProperty _StripeSplit6 = null;
    MaterialProperty _StripeSplitStrength6 = null;
    MaterialProperty _StripeSplit7 = null;
    MaterialProperty _StripeSplitStrength7 = null;
    MaterialProperty _MinimumBeamRadius = null;
   // MaterialProperty _IntersectionMod = null;

    //Projection Control Spectific
    MaterialProperty _ProjectionRotation = null;
    MaterialProperty _SpinSpeed = null;
    MaterialProperty _ProjectionIntensity = null;
    MaterialProperty _ProjectionFade = null;
    MaterialProperty _ProjectionFadeCurve = null;
    MaterialProperty _ProjectionDistanceFallOff = null;
    MaterialProperty _ProjectionRange = null;
    MaterialProperty _ProjectionRangeOrigin = null;
    MaterialProperty _EnableSpin = null;
    MaterialProperty _LegacyGoboRange = null;
    //MaterialProperty _BlendSrc = null;
    MaterialProperty _BlendDst = null;
   // MaterialProperty _BlendOp = null;
    MaterialProperty _ProjectionCutoff = null;
    MaterialProperty _ProjectionOriginCutoff = null;
    MaterialProperty _ClippingThreshold = null;
    MaterialProperty _AlphaProjectionIntensity = null;

    //Projection Texture Specific
    MaterialProperty _ProjectionSelection = null;
    MaterialProperty _ProjectionMainTex = null;
    MaterialProperty _ProjectionUVMod = null;
    MaterialProperty _ProjectionUVMod2 = null;
    MaterialProperty _ProjectionUVMod3 = null;
    MaterialProperty _ProjectionUVMod4 = null;
    MaterialProperty _ProjectionUVMod5 = null;
    MaterialProperty _ProjectionUVMod6 = null;
     MaterialProperty _ProjectionUVMod7 = null;
      MaterialProperty _ProjectionUVMod8 = null;
    MaterialProperty _RedMultiplier = null;
    MaterialProperty _GreenMultiplier = null;
    MaterialProperty _BlueMultiplier = null;
    
    //Static Projection
    MaterialProperty _ProjectionMaxIntensity = null;
    MaterialProperty _XOffset = null;
    MaterialProperty _YOffset = null;
    MaterialProperty _FeatherOffset = null;
    MaterialProperty _ModX = null;
    MaterialProperty _ModY = null;
    MaterialProperty _Fade = null;

    //Audio Link Stuff
    MaterialProperty _EnableAudioLink = null;
    MaterialProperty _EnableColorChord = null;

    MaterialProperty _NumBands = null;

    MaterialProperty _Band = null;

    MaterialProperty _Delay = null;

    MaterialProperty _BandMultiplier = null;

    //MaterialProperty _AudioSpectrum = null;



    //Interpolation Render Texture
    MaterialProperty _SmoothValue = null;
    MaterialProperty _UseOldSchoolSmoothing = null;
    MaterialProperty _MinimumSmoothnessDMX = null;
    MaterialProperty _MaximumSmoothnessDMX = null;

    //Strobe RenderTexture
    MaterialProperty _MaxStrobeFreq = null;
    MaterialProperty _LowFrequency = null;
    MaterialProperty _MedFrequency = null;
    MaterialProperty _HighFrequency = null;
    MaterialProperty _StrobeType = null;

    //Texture Color Sampling Stuff
    MaterialProperty _TextureColorSampleX = null;
    MaterialProperty _TextureColorSampleY = null;
    MaterialProperty _SamplingTexture = null;
    MaterialProperty _EnableColorTextureSample = null;
    MaterialProperty _EnableThemeColorSampling = null;
    MaterialProperty _ThemeColorTarget = null;



    
    //DiscoBall Exclusives
    MaterialProperty _Cube = null;
    MaterialProperty _RotationSpeed = null;
    MaterialProperty _Multiplier = null;

    //Lens Flare Exlcusives
    MaterialProperty _ColorSat = null;
    MaterialProperty _ScaleFactor = null;
    MaterialProperty _ReferenceDistance = null;
   // MaterialProperty _UVScale = null;
    MaterialProperty _RemoveTextureArtifact = null;
    MaterialProperty _UsePreMultiplyAlpha = null;
    MaterialProperty _LightSourceViewSpaceRadius = null;

    MaterialProperty _DepthOcclusionTestZBias = null;
    MaterialProperty _StartFadeinDistanceWorldUnit = null;
    MaterialProperty _EndFadeinDistanceWorldUnit = null;
    MaterialProperty _ShouldDoFlicker = null;
    MaterialProperty _FlickerAnimSpeed = null;
    MaterialProperty _FlickResultIntensityLowestPoint = null;
    MaterialProperty _AlphaIntensity = null;
    MaterialProperty _EnableAlphaDMX = null;
    MaterialProperty _Cutoff = null;


    MaterialProperty _MultiSampleDepth = null;

    
    //END Discoball Exclusives

    //Shader Type Identifiers
    bool isDiscoBall = false;
    bool isMoverLight = false;
    bool isStaticLight = false;
    bool isProjection = false;
    bool isFixture = false;
    bool isFlasher = false;
    bool isVolumetric = false;
    bool isDMXCompatible = false;
    bool isLensFlare = false;
    bool isSurfaceStatic = false;
    bool isMultiChannelBar = false;

    bool isRTShader = false;
    bool isRTStrobe = false;
    bool isRTSpin = false;
    bool isAudioLink = false;

    //VolumetricQuality volumetricQuality = VolumetricQuality.Default;

    
    //END Shader Type Identifiers

    //Foldout Bools
    static bool showDMXSettings = true;             
    static bool showGeneralControls = true;
    static bool showMoverControls = true;
    static bool showFixtureHousingControls = true;
    static bool showVolumetricTextureSettings = true;
    static bool showVolumetricControls = true;
    static bool showLensFlareControls = true;

    static bool showProjectionControls = true;
    static bool showProjectionTextureSettings = true;
    static bool showAudioLinkControls = true;

    
    //END Foldout Bools


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        Material material = materialEditor.target as Material;
        Shader shader = material.shader;

        //Establich Type Identifiers
        isDiscoBall = shader.name.Contains("Discoball");
        isMoverLight = shader.name.Contains("Mover");
        isStaticLight = shader.name.Contains("Static");
        isLensFlare = shader.name.Contains("Flare");
        isFlasher = shader.name.Contains("Flasher"); 
        isSurfaceStatic = shader.name.Contains("Surface Shader");
        isMultiChannelBar = shader.name.Contains("Multi-Channel Bar");
        isProjection = shader.name.Contains("Projection");
        isFixture = shader.name.Contains("Fixture");
        isVolumetric = shader.name.Contains("Volumetric");
        isAudioLink = shader.name.Contains("AudioLink");
        isDMXCompatible = !shader.name.Contains("Non-DMX") && !shader.name.Contains("NonDMX") && !isAudioLink;
        isRTShader = shader.name.Contains("RT");
        isRTStrobe = shader.name.Contains("Strobe");
        isRTSpin = shader.name.Contains("Spinner");
        
        //END Type Identifiers

        foreach(var property in GetType().GetFields(bindingFlags))  
        {
            if (property.FieldType == typeof(MaterialProperty))
            {
                try{ property.SetValue(this, FindProperty(property.Name, props)); } catch {  }
            } 
        }
        //BEGIN GUI Stuff
        EditorGUI.BeginChangeCheck();
        {
            VRSLStyles.DrawLogo();
            VRSLStyles.ShurikenHeaderCentered(VRSLStyles.GetVersion());
            VRSLStyles.ShurikenHeaderCentered(GetShaderType());
            VRSLStyles.PartingLine();
            VRSLStyles.DepthPassWarning();
            GUILayout.Space(5);
            if(isDiscoBall)
            {
                    DiscoballGUI(materialEditor, props, material);
                    return;

            }
            if(isMoverLight)
            {
                if(isMoverLight && isFixture)
                {
                    MoverLightFixtureGUI(materialEditor, props, material);
                    return;
                }
                if(isMoverLight && isVolumetric)
                {
                    MoverLightVolumetricGUI(materialEditor, props, material);
                    return;
                }
                if(isMoverLight && isProjection)
                {
                    MoverLightProjectionGUI(materialEditor,props, material);
                    return;
                }
            }
            if(isLensFlare)
            {

                LensFlareGUI(materialEditor,props, material);
                return;
            }
            if(isStaticLight)
            {
                if(isStaticLight && isFlasher)
                {
                    FlasherLightGUI(materialEditor,props, material);
                    return;
                }
                if(isStaticLight && isFixture)
                {
                    StaticLightFixtureGUI(materialEditor,props, material);
                    return;
                }
                if(isStaticLight && isProjection)
                {
                    StaticLightProjectionGUI(materialEditor,props, material);
                    return;
                }
                
            }

            if(isSurfaceStatic)
            {
                SurfaceShaderStaticGUI(materialEditor,props, material);
                return;
            }
            if(isRTShader)
            {
                if(isRTStrobe && !isRTSpin)
                {
                    DMXStrobeGUI(materialEditor,props, material);
                    return;
                }
                else if(isRTSpin && !isRTStrobe)
                {
                    DMXSpinnerGUI(materialEditor, props, material);
                    return;
                }
                else
                {
                    DMXInterpolationGUI(materialEditor,props, material);
                    return;
                }
            }
        }
        //END GUI STuff

    }

    public void AudioLinkGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        if(isAudioLink)
        {
            showAudioLinkControls = VRSLStyles.ShurikenFoldout("AudioLink Settings", showAudioLinkControls);
            if(showAudioLinkControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Here you can find your AudioLink settings. \nThe band muliplier is instanced and controled by Udon", MessageType.Info,true);
                matEditor.ShaderProperty(_EnableAudioLink, new GUIContent("Enable Audio Link", "Enables or disables the audio link feature. \nTurn this on to go back to normal Udon mode."));
                //matEditor.TexturePropertySingleLine(new GUIContent("Audio Spectrum", "The AudioLink Audio Spectrum to sample from."),_AudioSpectrum);
                matEditor.ShaderProperty(_Band, new GUIContent("Band", "The frequency band to sample from"));
                matEditor.ShaderProperty(_NumBands, new GUIContent("Number of Bands", "The number of frequency bands the texture supports."));
                matEditor.ShaderProperty(_Delay, new GUIContent("Delay", "How much delay between each sample"));
                matEditor.ShaderProperty(_BandMultiplier, new GUIContent("Band Multiplier", "The frequency band to sample from"));
                //if(!isDiscoBall)
                matEditor.ShaderProperty(_EnableColorChord, new GUIContent("Enable Color Chord", "Enables or disables Color Chord tinting. \nThis will tint the final color of the light after texture tinting."));
                matEditor.ShaderProperty(_EnableThemeColorSampling, new GUIContent("Enable Theme Color Sampling", "Enables or disables AudioLink Theme Color Sampling. \nThis will tint the final color of the light after texture tinting."));
                matEditor.ShaderProperty(_ThemeColorTarget, new GUIContent("Theme Color Target", "Choose which theme color to sample from."));
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
        else
        {
            return;
        }
    }

    public void LensFlareGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 15 and 49 channel mode."));
                //matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 1, 4, 5, and 13 channel mode."));
                switch(target.GetInt("_ChannelMode"))
                {
                    case 0:
                        target.EnableKeyword("_1CH_MODE");
                        target.DisableKeyword("_4CH_MODE");
                        target.DisableKeyword("_5CH_MODE");
                        target.DisableKeyword("_13CH_MODE");
                        break;
                    case 1:
                        target.DisableKeyword("_1CH_MODE");
                        target.EnableKeyword("_4CH_MODE");
                        target.DisableKeyword("_5CH_MODE");
                        target.DisableKeyword("_13CH_MODE");
                        break;
                    case 2: 
                        target.DisableKeyword("_1CH_MODE");
                        target.DisableKeyword("_4CH_MODE");
                        target.EnableKeyword("_5CH_MODE");
                        target.DisableKeyword("_13CH_MODE");
                        break;
                    case 3:
                        target.DisableKeyword("_1CH_MODE");
                        target.DisableKeyword("_4CH_MODE");
                        target.DisableKeyword("_5CH_MODE");
                        target.EnableKeyword("_13CH_MODE");
                        break;
                    default:
                        break;
                }
                //matEditor.ShaderProperty(_LegacyGoboRange, new GUIContent("Enable Legacy Gobo Range", "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
            showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
            if(showGeneralControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
                EditorGUI.indentLevel++;  
                matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
                EditorGUI.indentLevel--;    
                matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
                matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
                GUILayout.Space(10);
                matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
                ColorTextureSamplingGUI(matEditor, props, target);

                //matEditor.ShaderProperty(_CurveMod, new GUIContent("Light Intensity Curve Modifier", "Curve modifier for light intensity."));
                matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
                matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent("Intensity Multipler (For Bloom Scaling)", ""));                
                matEditor.ShaderProperty(_CurveMod, new GUIContent("Light Intensity Curve Modifier", ""));
            // matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the lens brightness. Good for adjusting to increase bloom"));
                matEditor.EnableInstancingField();
                matEditor.RenderQueueField();
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
            showLensFlareControls = VRSLStyles.ShurikenFoldout("Lens Flare Settings", showLensFlareControls);
            if(showLensFlareControls)
            {
                // if(isDMXCompatible)
                // {
                    matEditor.ShaderProperty(_RenderMode, new GUIContent("Render Mode", "Choose between a fully transparent shader, or one that is opaque with a dithering technique."));
                    if(target.GetInt("_RenderMode") == 1) 
                    {
                        target.SetOverrideTag("RenderType", "Transparent");
                        target.DisableKeyword("_ALPHATEST_ON");  
                        //target.SetInt("_BlendSrc", 1);
                        target.SetInt("_BlendDst", 1);
                        target.SetInt("_ZWrite", 0);
                        target.SetInt("_AlphaToCoverage", 0);
                        target.SetInt("_HQMode", 0);
                        target.renderQueue = 3200;
                    }
                    else
                    {
                        matEditor.ShaderProperty(_ClippingThreshold, new GUIContent("Alpha Clip Threshold", "Adjust the clipping threshold of the dither effect."));
                       //matEditor.ShaderProperty(_AlphaProjectionIntensity, new GUIContent("Alpha Projection Intesnity", "Adjust the projection intensity for alphat2coverage mode."));
                        target.SetOverrideTag("RenderType", "Opaque");
                        target.EnableKeyword("_ALPHATEST_ON");
                        //target.SetInt("_BlendSrc", 0);
                        target.SetInt("_BlendDst", 0);
                        target.SetInt("_ZWrite", 1);
                        target.SetInt("_AlphaToCoverage", 1);
                        target.SetInt("_HQMode", 0);
                        target.renderQueue = 2450+200;
                    }
                // }
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.TexturePropertySingleLine(new GUIContent("Flare Texture", ""), _MainTex);
                matEditor.TextureScaleOffsetProperty(_MainTex);
                matEditor.ShaderProperty(_UseDepthLight, new GUIContent("Use Depth Light", "Enable/Disable the reliance of the depth light for this volumetric shader."));
                SetKeyword(target, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(target.GetInt("_UseDepthLight"))) == 1 ? true : false);
                matEditor.ShaderProperty(_FadeAmt, new GUIContent("Fade Strength", ""));
                matEditor.ShaderProperty(_ColorSat, new GUIContent("Color Saturtation Strength", ""));
                matEditor.ShaderProperty(_ScaleFactor, new GUIContent("Scale Factor", ""));
                matEditor.ShaderProperty(_ReferenceDistance, new GUIContent("Reference Distance", ""));

                matEditor.ShaderProperty(_RemoveTextureArtifact, new GUIContent("Remove Texture Artifacting", ""));
                matEditor.ShaderProperty(_UsePreMultiplyAlpha, new GUIContent("UsePreMultiplyAlpha (recommend _BaseMap's alpha = 'From Gray Scale')", ""));
                matEditor.ShaderProperty(_LightSourceViewSpaceRadius, new GUIContent("Light Source View Space Radius", ""));
                matEditor.ShaderProperty(_DepthOcclusionTestZBias, new GUIContent("Depth OcclusionTest Z Bias", ""));
                matEditor.ShaderProperty(_StartFadeinDistanceWorldUnit, new GUIContent("Start Fade in Distance World Unit", ""));
                matEditor.ShaderProperty(_EndFadeinDistanceWorldUnit, new GUIContent("End Fade in Distance World Unit", ""));
                matEditor.ShaderProperty(_ShouldDoFlicker, new GUIContent("Should Do Flicker", ""));
                matEditor.ShaderProperty(_FlickerAnimSpeed, new GUIContent("Flicker Anim Speed", ""));
                matEditor.ShaderProperty(_FlickResultIntensityLowestPoint, new GUIContent("Flick Result Intensity Lowest Point", ""));
            }
    }
    public void SurfaceShaderStaticGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
            matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
            matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
            matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
            matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
            if(isMultiChannelBar)
            {
                matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 15 and 49 channel mode."));
                if(target.GetInt("_ChannelMode") == 1)
                {
                    target.EnableKeyword("_CHANNEL_MODE");
                }
                else
                {
                    target.DisableKeyword("_CHANNEL_MODE");
                }
            }
            else
            {
                matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 1, 4, 5, and 13 channel mode."));
                switch(target.GetInt("_ChannelMode"))
                {
                    case 0:
                        target.EnableKeyword("_1CH_MODE");
                        target.DisableKeyword("_4CH_MODE");
                        target.DisableKeyword("_5CH_MODE");
                        target.DisableKeyword("_13CH_MODE");
                        break;
                    case 1:
                        target.DisableKeyword("_1CH_MODE");
                        target.EnableKeyword("_4CH_MODE");
                        target.DisableKeyword("_5CH_MODE");
                        target.DisableKeyword("_13CH_MODE");
                        break;
                    case 2: 
                        target.DisableKeyword("_1CH_MODE");
                        target.DisableKeyword("_4CH_MODE");
                        target.EnableKeyword("_5CH_MODE");
                        target.DisableKeyword("_13CH_MODE");
                        break;
                    case 3:
                        target.DisableKeyword("_1CH_MODE");
                        target.DisableKeyword("_4CH_MODE");
                        target.DisableKeyword("_5CH_MODE");
                        target.EnableKeyword("_13CH_MODE");
                        break;
                    default:
                        break;
                }
            }
            //matEditor.ShaderProperty(_LegacyGoboRange, new GUIContent("Enable Legacy Gobo Range", "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."));
            EditorGUI.indentLevel--;   
            VRSLStyles.PartingLine();
            EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));

            // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
            // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_Udon_DMXGridRenderTextureMovement);
            // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            GUILayout.Space(10);
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            //ColorTextureSamplingGUI(matEditor, props, target);

            //matEditor.ShaderProperty(_CurveMod, new GUIContent("Light Intensity Curve Modifier", "Curve modifier for light intensity."));
            matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
            matEditor.ShaderProperty(_CurveMod, new GUIContent("Lens Multiplier", "Fixture Intensity Multiplier"));
            matEditor.ShaderProperty(_Saturation, new GUIContent("Lens Color Saturation", "General slider for adjusting the saturation of the lens"));
           // matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the lens brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        showFixtureHousingControls = VRSLStyles.ShurikenFoldout("Fixture Housing Settings", showFixtureHousingControls);
        if(showFixtureHousingControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            if(target.shader.name.Contains("Transparent") || target.shader.name.Contains("Cutout") ){
            matEditor.ShaderProperty(_EnableAlphaDMX, new GUIContent("Enable DMX Controlled Alpha", "Control Alpha channel with dmx (channel = +1 of last channel.)"));
            matEditor.ShaderProperty(_AlphaIntensity, new GUIContent("Fixture Housing Alpha", "The main Alpha Channel for the fixture housing"));
            }
            if(target.shader.name.Contains("Cutout")){
            matEditor.ShaderProperty(_Cutoff, new GUIContent("Alpha Cutoff", "Threshold to discard pixels for alpha cutoff shaders."));
            }
            matEditor.ShaderProperty(_Color, new GUIContent("Fixture Housing Color Tint", "The main diffuse color for the fixture housing"));
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Diffuse Map", "The main diffuse texture for the fixture housing."), _MainTex);
            matEditor.TextureScaleOffsetProperty(_MainTex);
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Normal Map", "The normal map for the fixture housing."), _NormalMap);
            matEditor.TextureScaleOffsetProperty(_NormalMap);
            //matEditor.ShaderProperty(_BumpScale, new GUIContent("Normal Map Strength", "The strength of the normal map for the fixture housing."));
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Emission Mask", "A mask to choose where to set the emission on the fixture."), _EmissionMask);
            matEditor.TextureScaleOffsetProperty(_EmissionMask);
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Metallic(R) / Smoothness(A) Map", "The main Metallic/Smoothness texture for the fixture housing."), _MetallicSmoothness);
            matEditor.TextureScaleOffsetProperty(_MetallicSmoothness);
            matEditor.ShaderProperty(_Metallic, new GUIContent("Fixture Housing Metallic Level", "The metallic level of the fixture housing."));
            matEditor.ShaderProperty(_Glossiness, new GUIContent("Fixture Housing Glossiness Level", "The glossines of the fixture housing."));
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
    }

    public void StaticLightProjectionGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 15 and 49 channel mode."));
                if(target.GetInt("_ChannelMode") == 1)
                {
                    target.EnableKeyword("_CHANNEL_MODE");
                }
                else
                {
                    target.DisableKeyword("_CHANNEL_MODE");
                }
                //matEditor.ShaderProperty(_LegacyGoboRange, new GUIContent("Enable Legacy Gobo Range", "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
        //GENERAL CONTROLS
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            GUILayout.Space(10);
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            ColorTextureSamplingGUI(matEditor, props, target);

            //matEditor.ShaderProperty(_CurveMod, new GUIContent("Light Intensity Curve Modifier", "Curve modifier for light intensity."));
            matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
           // matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the lens brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        //PROJECTION CONTROLS
        showProjectionControls = VRSLStyles.ShurikenFoldout("Projection Settings", showProjectionControls);
        if(showProjectionControls)
        {
            // if(isDMXCompatible)
            // {
                matEditor.ShaderProperty(_MultiSampleDepth, new GUIContent("Depth Multi-Sampling", "Sample the depth texture multiple times to prevent artifacting on edges of the projection. This does incurr a slight cost."));
                if(target.GetInt("_MultiSampleDepth") == 1)
                {
                    target.EnableKeyword("_MULTISAMPLEDEPTH");
                }
                else
                {
                    target.DisableKeyword("_MULTISAMPLEDEPTH");
                }
                matEditor.ShaderProperty(_RenderMode, new GUIContent("Render Mode", "Choose between a fully transparent shader, or one that is opaque with a dithering technique."));
                if(target.GetInt("_RenderMode") == 1) 
                {
                    target.SetOverrideTag("RenderType", "Transparent");
                    target.DisableKeyword("_ALPHATEST_ON");  
                    //target.SetInt("_BlendSrc", 1);
                    target.SetInt("_BlendDst", 1);
                    target.SetInt("_ZWrite", 0);
                    target.SetInt("_AlphaToCoverage", 0);
                    target.SetInt("_HQMode", 0);
                    target.renderQueue = 3001;
                }
                else
                {
                    matEditor.ShaderProperty(_ClippingThreshold, new GUIContent("Alpha Clip Threshold", "Adjust the clipping threshold of the dither effect."));
                    matEditor.ShaderProperty(_AlphaProjectionIntensity, new GUIContent("Alpha Projection Intesnity", "Adjust the projection intensity for alphat2coverage mode."));
                    target.SetOverrideTag("RenderType", "Opaque");
                    target.EnableKeyword("_ALPHATEST_ON");
                    //target.SetInt("_BlendSrc", 0);
                    target.SetInt("_BlendDst", 0);
                    target.SetInt("_ZWrite", 1);
                    target.SetInt("_AlphaToCoverage", 1);
                    target.SetInt("_HQMode", 0);
                    target.renderQueue = 2451;
                }
            // }
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.TextureProperty(_ProjectionMainTex, "Projection Texture Atlas");
            matEditor.ShaderProperty(_ProjectionUVMod, new GUIContent("Projection Texture", "The scale of Projection Texture/Gobo 1"));
            matEditor.ShaderProperty(_ProjectionRotation, new GUIContent("Static Projection UV Rotation", "Changes the default angle the projected image is at."));
            GUILayout.Space(10);
            matEditor.ShaderProperty(_EnableSpin, new GUIContent("Enable Auto Spin", "Enable/Disable the projection's automatic spinning. Usually controlled by Udon."));
            matEditor.ShaderProperty(_SpinSpeed, new GUIContent("Auto Spin Speed", "The speed at which it the projection will spin when auto spin is enabled."));
            matEditor.ShaderProperty(_ProjectionMaxIntensity, new GUIContent("Projection Intensity", "Changes how bright the projection is"));
            matEditor.ShaderProperty(_Fade, new GUIContent("Light Range", "Changes the attenuation linearly."));
            matEditor.ShaderProperty(_FeatherOffset, new GUIContent("Attenuation Quadratic", "Changes the quadratic attenuation."));
            matEditor.ShaderProperty(_ProjectionDistanceFallOff, new GUIContent("Attenuation Constant", "How quickly the the projection loses strength the further away it is from the source."));

            matEditor.ShaderProperty(_ModX, new GUIContent("Projection UV X Stretch", "Stretches the projection texture in the X direction."));
            matEditor.ShaderProperty(_ModY, new GUIContent("Projection UV Y Stretch", "Stretches the projection texture in the Y direciton."));
            matEditor.ShaderProperty(_XOffset, new GUIContent("Projection UV X Offset", "Offsets the projection texture in the X direction."));
            matEditor.ShaderProperty(_YOffset, new GUIContent("Projection UV Y Offset", "Offsets the projection texture in the Y direciton."));
            matEditor.ShaderProperty(_ProjectionRange, new GUIContent("Projection Range", "Changes the length of the projecction mesh itself. The longer it is, the further away the projection will reach before clipping, but also more pixels on the screen the projection will take up. \nIncrease this if you see the edges of the projection clipping too much from being too far away."));
            GUILayout.Space(5);
            VRSLStyles.PartingLine();
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("''Cone Width'' is usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
            matEditor.ShaderProperty(_ConeWidth, new GUIContent("Cone Width", "Changes the radius of the cone to be larger or smaller."));
            GUILayout.Space(5);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
            GUILayout.Space(5);
            VRSLStyles.PartingLine();
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("Use these sliders to increase the intensity rate of certain color channels. \nThis allows to for countering the blending bias of the projection towards certain colors. \nExample: Blue enviorments will cancel out red, so increase the red multiplier.",MessageType.None,true);
            matEditor.ShaderProperty(_RedMultiplier, new GUIContent("Red Channel Multiplier", "Increases the rate of the red channel."));
            matEditor.ShaderProperty(_GreenMultiplier, new GUIContent("Green Channel Multiplier", "Increases the rate of the green channel."));
            matEditor.ShaderProperty(_BlueMultiplier, new GUIContent("Blue Channel Multiplier", "Increases the rate of the blue channel."));
            GUILayout.Space(5);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        GUILayout.Space(15);
        // EditorGUI.indentLevel++;

        // EditorGUI.indentLevel--;
    }
    public void FlasherLightGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 15 and 49 channel mode."));
                if(target.GetInt("_ChannelMode") == 1)
                {
                    target.EnableKeyword("_CHANNEL_MODE");
                }
                else
                {
                    target.DisableKeyword("_CHANNEL_MODE");
                }
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
            showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
            if(showGeneralControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
                EditorGUI.indentLevel++;  
                matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
                EditorGUI.indentLevel--; 
                matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
                matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
                matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
                ColorTextureSamplingGUI(matEditor, props, target);
                matEditor.ShaderProperty(_CurveMod, new GUIContent("Light Intensity Curve Modifier", "Curve modifier for light intensity."));
                matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
                matEditor.EnableInstancingField();
                matEditor.RenderQueueField();
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
            showFixtureHousingControls = VRSLStyles.ShurikenFoldout("Fixture Housing Settings", showFixtureHousingControls);
            if(showFixtureHousingControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_Color, new GUIContent("Fixture Housing Color Tint", "The main diffuse color for the fixture housing"));
                matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Diffuse Map", "The main diffuse texture for the fixture housing."), _MainTex);
                matEditor.TextureScaleOffsetProperty(_MainTex);
                GUILayout.Space(5);
                matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Normal Map", "The normal map for the fixture housing."), _NormalMap);
                matEditor.TextureScaleOffsetProperty(_NormalMap);
                //matEditor.ShaderProperty(_BumpScale, new GUIContent("Normal Map Strength", "The strength of the normal map for the fixture housing."));
                GUILayout.Space(5);
                matEditor.ShaderProperty(_Metallic, new GUIContent("Fixture Housing Metallic Level", "The metallic level of the fixture housing."));
                matEditor.ShaderProperty(_Glossiness, new GUIContent("Fixture Housing Glossiness Level", "The glossines of the fixture housing."));
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
    }
    

    public void StaticLightFixtureGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                matEditor.ShaderProperty(_ChannelMode, new GUIContent("Channel Mode", "Choose between 15 and 49 channel mode."));
                if(target.GetInt("_ChannelMode") == 1)
                {
                    target.EnableKeyword("_CHANNEL_MODE");
                }
                else
                {
                    target.DisableKeyword("_CHANNEL_MODE");
                }
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
        //GENERAL CONTROLS
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            ColorTextureSamplingGUI(matEditor, props, target);
            matEditor.ShaderProperty(_CurveMod, new GUIContent("Light Intensity Curve Modifier", "Curve modifier for light intensity."));
            matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
            matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the lens brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        //FIXTURE HOUSING SETTINGS
        showFixtureHousingControls = VRSLStyles.ShurikenFoldout("Fixture Housing Settings", showFixtureHousingControls);
        if(showFixtureHousingControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_Color, new GUIContent("Fixture Housing Color Tint", "The main diffuse color for the fixture housing"));
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Diffuse Map", "The main diffuse texture for the fixture housing."), _MainTex);
            matEditor.TextureScaleOffsetProperty(_MainTex);
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Normal Map", "The normal map for the fixture housing."), _NormalMap);
            matEditor.TextureScaleOffsetProperty(_NormalMap);
            //matEditor.ShaderProperty(_BumpScale, new GUIContent("Normal Map Strength", "The strength of the normal map for the fixture housing."));
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Emission Mask", "A mask to choose where to set the emission on the fixture."), _EmissionMask);
            matEditor.TextureScaleOffsetProperty(_EmissionMask);
            matEditor.ShaderProperty(_Metallic, new GUIContent("Fixture Housing Metallic Level", "The metallic level of the fixture housing."));
            matEditor.ShaderProperty(_Glossiness, new GUIContent("Fixture Housing Glossiness Level", "The glossines of the fixture housing."));
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();  
    } 

    public void MoverLightProjectionGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
         //DMX CONTROLS
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                matEditor.ShaderProperty(_LegacyGoboRange, new GUIContent("Enable Legacy Gobo Range", "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."));
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out heavily by a custom render texture. Used for movement."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Spin Timer", "DMX Grid with GOBO Spin timings embedded"),_Udon_DMXGridSpinTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }


        //GENERAL CONTROLS
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_FixtureBaseRotationY, new GUIContent("Rotation Y Offset", "Offset the Y Rotation of the fixture."));
            matEditor.ShaderProperty(_FixtureRotationX, new GUIContent("Rotation X Offset", "Offset the X Rotation of the fixture."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            ColorTextureSamplingGUI(matEditor, props, target);
            //matEditor.ShaderProperty(_, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            //matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        showProjectionControls = VRSLStyles.ShurikenFoldout("Projection Controls", showProjectionControls);
        if(showProjectionControls)
        {
            // if(isDMXCompatible)
            // {
                matEditor.ShaderProperty(_MultiSampleDepth, new GUIContent("Depth Multi-Sampling", "Sample the depth texture multiple times to prevent artifacting on edges of the projection. This does incurr a slight cost."));
                if(target.GetInt("_MultiSampleDepth") == 1)
                {
                    target.EnableKeyword("_MULTISAMPLEDEPTH");
                }
                else
                {
                    target.DisableKeyword("_MULTISAMPLEDEPTH");
                }
                matEditor.ShaderProperty(_RenderMode, new GUIContent("Render Mode", "Choose between a fully transparent shader, or one that is opaque with a dithering technique."));
                if(target.GetInt("_RenderMode") == 1) 
                {
                    target.SetOverrideTag("RenderType", "Transparent");
                    target.DisableKeyword("_ALPHATEST_ON");  
                    //target.SetInt("_BlendSrc", 1);
                    target.SetInt("_BlendDst", 1);
                    target.SetInt("_ZWrite", 0);
                    target.SetInt("_AlphaToCoverage", 0);
                    target.SetInt("_HQMode", 0);
                    target.renderQueue = 3001;
                }
                else
                {
                    matEditor.ShaderProperty(_ClippingThreshold, new GUIContent("Alpha Clip Threshold", "Adjust the clipping threshold of the dither effect."));
                    target.SetOverrideTag("RenderType", "Opaque");
                    target.EnableKeyword("_ALPHATEST_ON");
                    //target.SetInt("_BlendSrc", 0);
                    target.SetInt("_BlendDst", 0);
                    target.SetInt("_ZWrite", 1);
                    target.SetInt("_AlphaToCoverage", 1);
                    target.SetInt("_HQMode", 0);
                    target.renderQueue = 2451;
                }
            // }
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_ProjectionRotation, new GUIContent("Static Projection UV Rotation", "Changes the default angle the projected image is at."));
            matEditor.ShaderProperty(_EnableSpin, new GUIContent("Enable Auto Spin", "Enable/Disable the projection's automatic spinning. Usually controlled by Udon."));
            matEditor.ShaderProperty(_MinimumBeamRadius, new GUIContent("Minimum Beam Radius", "Minimum Beam Radius."));
            matEditor.ShaderProperty(_SpinSpeed, new GUIContent("Auto Spin Speed", "The speed at which it the projection will spin when auto spin is enabled."));
            matEditor.ShaderProperty(_ProjectionIntensity, new GUIContent("Projection Intensity", "Changes how bright the projection is"));
            matEditor.ShaderProperty(_ProjectionFade, new GUIContent("Projection Edge Fade", "Changes how much the edges of the projection are faded away."));
            matEditor.ShaderProperty(_ProjectionFadeCurve, new GUIContent("Projection Edge Fade Harshness", "Changes the rate at which the edges of the projection are faded."));
            matEditor.ShaderProperty(_ProjectionDistanceFallOff, new GUIContent("Projection Distance Fallof Strength", "How quickly the the projection loses strength the further away it is from the source."));
            matEditor.ShaderProperty(_ProjectionRange, new GUIContent("Projection Drawing Range", "Changes the length of the projecction mesh itself. The longer it is, the further away the projection will reach before clipping, but also more pixels on the screen the projection will take up. \nIncrease this if you see the edges of the projection clipping too much from the light being too far away."));
            matEditor.ShaderProperty(_ProjectionRangeOrigin, new GUIContent("Projection Drawing Range Scale Origin", "The place where the projection mesh is being scaled from. Don't change this unless you know what you are doing. "));
            // if(isDMXCompatible)
            // {
                matEditor.ShaderProperty(_ProjectionCutoff, new GUIContent("Projection Fixture Source Cutoff", "This is where the projector actually begins drawing the projection. Use this to prevent the projection from bleeding on to the fixture mesh."));
                matEditor.ShaderProperty(_ProjectionOriginCutoff, new GUIContent("Projection Fixture Origin Cutoff", "This is the area between the origin of the fixture and where the projection mesh is. Use this to prevent the projection from bleeding on to the fixture mesh relative to the origin."));
            // }
           // matEditor.DefaultShaderProperty
          //  matEditor.ShaderProperty(_BlendSrc, new GUIContent("Projection Blend Source", "Projection Transparency Blend Options (Soruce)"));
            matEditor.ShaderProperty(_BlendDst, new GUIContent("Projection Blend Destination", "Projection Transparency Blend Options (Destination)"));
           // matEditor.ShaderProperty(_BlendOp, new GUIContent("Projection Blend Operation", "Projection Transparency Blend Options (Operation)"));
            VRSLStyles.PartingLine();
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("''Cone Width'' and ''Cone Length'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
            matEditor.ShaderProperty(_ConeWidth, new GUIContent("Cone Width", "Changes the radius of the cone to be larger or smaller."));
            matEditor.ShaderProperty(_ConeLength, new GUIContent("Cone Length", "Changes how long the volumetric cone via the texture coordinates."));
           // matEditor.ShaderProperty(_MaxConeLength, new GUIContent("Max Cone Length", "Changes how long the volumetric cone is via the mesh"));
            matEditor.ShaderProperty(_ConeSync, new GUIContent("Cone Scale Sync", "Changes the rate at which the cone scales from source to the end of the cone. Highly recommened to use default settings if unsure."));
            GUILayout.Space(5);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        showProjectionTextureSettings = VRSLStyles.ShurikenFoldout("Projection Texture Settings", showProjectionTextureSettings);
        if(showProjectionTextureSettings)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_ProjectionSelection, new GUIContent("Projection Selection", "Use this to change what projection is selected. Usually overridden by Udon"));
            EditorGUILayout.HelpBox("This is where you can set the images that the shader projects on to other objects.",MessageType.None,true);
            GUILayout.Space(5);
            matEditor.TextureProperty(_ProjectionMainTex, "Projection Texture Atlas");
            matEditor.ShaderProperty(_ProjectionUVMod, new GUIContent("Projection Texture/GOBO 1 Scale", "The scale of Projection Texture/Gobo 1"));
           // matEditor.TextureProperty(_ProjectionTex2, "Projection Texture/Gobo 2");
            matEditor.ShaderProperty(_ProjectionUVMod2, new GUIContent("Projection Texture/GOBO 2 Scale", "The scale of Projection Texture/Gobo 2"));
            //matEditor.TextureProperty(_ProjectionTex3, "Projection Texture/Gobo 3");
            matEditor.ShaderProperty(_ProjectionUVMod3, new GUIContent("Projection Texture/GOBO 3 Scale", "The scale of Projection Texture/Gobo 3"));
            //matEditor.TextureProperty(_ProjectionTex4, "Projection Texture/Gobo 4");
            matEditor.ShaderProperty(_ProjectionUVMod4, new GUIContent("Projection Texture/GOBO 4 Scale", "The scale of Projection Texture/Gobo 4"));
            //matEditor.TextureProperty(_ProjectionTex5, "Projection Texture/Gobo 5");
            matEditor.ShaderProperty(_ProjectionUVMod5, new GUIContent("Projection Texture/GOBO 5 Scale", "The scale of Projection Texture/Gobo 5"));
            //matEditor.TextureProperty(_ProjectionTex6, "Projection Texture/Gobo 6");
            matEditor.ShaderProperty(_ProjectionUVMod6, new GUIContent("Projection Texture/GOBO 6 Scale", "The scale of Projection Texture/Gobo 6"));
            matEditor.ShaderProperty(_ProjectionUVMod7, new GUIContent("Projection Texture/GOBO 7 Scale", "The scale of Projection Texture/Gobo 7"));
            matEditor.ShaderProperty(_ProjectionUVMod8, new GUIContent("Projection Texture/GOBO 8 Scale", "The scale of Projection Texture/Gobo 8"));
            GUILayout.Space(5);
            VRSLStyles.PartingLine();
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("Use these sliders to increase the intensity rate of certain color channels. \nThis allows to for countering the blending bias of the projection towards certain colors. \nExample: Blue enviorments will cancel out red, so increase the red multiplier.",MessageType.None,true);
            matEditor.ShaderProperty(_RedMultiplier, new GUIContent("Red Channel Multiplier", "Increases the rate of the red channel."));
            matEditor.ShaderProperty(_GreenMultiplier, new GUIContent("Green Channel Multiplier", "Increases the rate of the green channel."));
            matEditor.ShaderProperty(_BlueMultiplier, new GUIContent("Blue Channel Multiplier", "Increases the rate of the blue channel."));
            GUILayout.Space(5);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        //MOVER CONTROLS
        if(isStaticLight == false && isAudioLink == false)
        {
            showMoverControls = VRSLStyles.ShurikenFoldout("Movement Settings", showMoverControls);
            if(showMoverControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_FixtureRotationOrigin, new GUIContent("Fixture Pivot Origin", "Sets the rotation point of the fixture for tilt in object space. Do not change this unless you are trying to make a custom housing."));
                matEditor.ShaderProperty(_MaxMinPanAngle, new GUIContent("Max/Min Pan Angles (Left/Right)", "Sets the Left/Right rotation range of the fixture by ''-Value to Value''"));
                matEditor.ShaderProperty(_MaxMinTiltAngle, new GUIContent("Max/Min Tilt Angles (Up/Down)", "Sets the Up/Down rotation range of the fixture by ''-Value to Value''"));
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }

        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();       
    }

    public void MoverLightVolumetricGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                matEditor.ShaderProperty(_EnableExtraChannels, new GUIContent("Enable Cone Length DMX Controls","Enable this if you want to be able to extend the lenghth of the cone on Channel 2!"));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out heavily by a custom render texture. Used for movement."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Spin Timer", "DMX Grid with GOBO Spin timings embedded"),_Udon_DMXGridSpinTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }

        //GENERAL CONTROLS
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            matEditor.ShaderProperty(_FixtureBaseRotationY, new GUIContent("Rotation Y Offset", "Offset the Y Rotation of the fixture."));
            matEditor.ShaderProperty(_FixtureRotationX, new GUIContent("Rotation X Offset", "Offset the X Rotation of the fixture."));
            ColorTextureSamplingGUI(matEditor, props, target);
            matEditor.ShaderProperty(_Saturation, new GUIContent("Saturation", "Saturation modifier for light color."));
            matEditor.ShaderProperty(_SaturationLength, new GUIContent("Saturation Length", "Har far from the source does the saturation slider affect the shader."));
            matEditor.ShaderProperty(_LensMaxBrightness, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
            //matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the brightness. Good for adjusting to increase bloom"));
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        //VOLUMETRIC TEXTURE SETTINGS
        showVolumetricTextureSettings = VRSLStyles.ShurikenFoldout("Cone Texture/Render Settings", showVolumetricTextureSettings);
        if(showVolumetricTextureSettings)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            GUILayout.Space(5);
            //matEditor.TextureScaleOffsetProperty(_LightMainTex);
            matEditor.ShaderProperty(_RenderMode, new GUIContent("Render Mode", "Choose between a fully transparent shader, or one that is opaque with a dithering technique."));
            if(target.GetInt("_RenderMode") == 0)
            {
                target.SetOverrideTag("RenderType", "Transparent");
                target.DisableKeyword("_ALPHATEST_ON");  
                //target.SetInt("_BlendSrc", 1);
                target.SetInt("_BlendDst", 1);
                target.SetInt("_ZWrite", 0);
                target.SetInt("_AlphaToCoverage", 0);
                target.SetInt("_HQMode", 1);
                target.renderQueue = 3002;
            }
            else if(target.GetInt("_RenderMode") == 1) 
            {
                target.SetOverrideTag("RenderType", "Transparent");
                target.DisableKeyword("_ALPHATEST_ON");  
                //target.SetInt("_BlendSrc", 1);
                target.SetInt("_BlendDst", 1);
                target.SetInt("_ZWrite", 0);
                target.SetInt("_AlphaToCoverage", 0);
                target.SetInt("_HQMode", 0);
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
                target.renderQueue = 2452;
            }
            GUILayout.Space(5);
            matEditor.ShaderProperty(_UseDepthLight, new GUIContent("Use Depth Light", "Enable/Disable the reliance of the depth light for this volumetric shader."));
            GUILayout.Space(5);
            matEditor.ShaderProperty(_GradientMod, new GUIContent("Gradient Modifier", "Controls the general gradient of the cone."));
            matEditor.ShaderProperty(_GradientModGOBO, new GUIContent("Gradient Modifier With GOBO", "Controls the general gradient of the cone when using a GOBO."));
             GUILayout.Space(5);
            matEditor.ShaderProperty(_2D_NOISE_ON, new GUIContent("Enable 2D Noise", "Enable first layer of world space, 2D Noise"));
            if((Mathf.FloorToInt(target.GetInt("_2D_NOISE_ON"))) == 1)
            {
                EditorGUI.indentLevel++;  
                if(target.GetInt("_RenderMode") == 0)
                {
                    matEditor.TexturePropertySingleLine(new GUIContent("Noise Texture HQ", "Alpha Noise Texture used for adding variation to the cone."), _NoiseTexHigh);
                    matEditor.TextureScaleOffsetProperty(_NoiseTexHigh);
                }
                else
                {
                    matEditor.TexturePropertySingleLine(new GUIContent("Noise Texture", "Alpha Noise Texture used for adding variation to the cone."), _NoiseTex);
                    matEditor.TextureScaleOffsetProperty(_NoiseTex);                   
                }

                matEditor.ShaderProperty(_NoisePower, new GUIContent("Noise Strength", "Controls how much the noise texture affects the cone"));
                EditorGUI.indentLevel--;  
            }
           // matEditor.ShaderProperty(_NoiseSeed, new GUIContent("Noise Randomization", "Adds randomness to the noise for more variation")); 
            GUILayout.Space(5);

            
            //if(!isDMXCompatible)
            //{
                string magicNoiseString = "_MAGIC_NOISE_ON_MED";
                MaterialProperty magicNoiseProp = _MAGIC_NOISE_ON_MED;
                string magicNoiseSuffix = "Transparent";
                if(target.GetInt("_RenderMode") == 0)
                {
                    magicNoiseString = "_MAGIC_NOISE_ON_HIGH";
                    magicNoiseProp = _MAGIC_NOISE_ON_HIGH;
                    magicNoiseSuffix = "HQTransparent";

                }


                if((Mathf.FloorToInt(target.GetInt(magicNoiseString))) == 1 && target.GetInt("_RenderMode") != 2)
                {  
                    
                    matEditor.ShaderProperty(magicNoiseProp, new GUIContent("Enable Magic 3D Noise For:  " + magicNoiseSuffix, "Enable Second layer of world space, faux 3D Noise"));
                    EditorGUILayout.LabelField("Potato Mode is unavailable. Disable Magic 3D Noise to enable Potato Mode.");
                     
                    matEditor.TexturePropertySingleLine(new GUIContent("Magic 3D Noise Texture", "A magical texture for generating 3D Perlin Noise at runtime! Code and texture based on https://www.shadertoy.com/view/4sfGzS by iq!"), _LightMainTex);
                    EditorGUI.indentLevel++;   
                    // if((Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1)
                    // {
                    //     EditorGUILayout.LabelField("HQ Mode is unavailable. Disable Potato Mode to enable Potato HQ.");
                    //     matEditor.ShaderProperty(_PotatoMode, new GUIContent("Potato Mode", "Reduces the overhead on the fragment shader by removing both noise components to extra texture sampling."));
                    // }
                    // else if((Mathf.FloorToInt(target.GetInt("_HQMode"))) == 1) 
                    // {
                    //     matEditor.ShaderProperty(_HQMode, new GUIContent("HQ Mode", "A higher quality volumetric mode (Experimental)."));
                    //     EditorGUILayout.LabelField("Potato Mode is unavailable. Disable HQ Mode to enable Potato Mode.");
                    // }
                    // else
                    // {
                    matEditor.ShaderProperty(_HQMode, new GUIContent("HQ Mode", "A higher quality volumetric mode (Experimental)."));
                    //     matEditor.ShaderProperty(_PotatoMode, new GUIContent("Potato Mode", "Reduces the overhead on the fragment shader by removing both noise components to extra texture sampling."));

                    // }

                    
                    // volumetricQuality = (VolumetricQuality) (EditorGUILayout.EnumPopup("Volumetric Quality",volumetricQuality));
                    // switch(volumetricQuality)
                    // {
                    //     case VolumetricQuality.Potato:
                    //         target.SetInt("_PotatoMode", 1);
                    //         target.SetInt("_HQMode", 0);            
                    //         break;
                    //     case VolumetricQuality.Default:
                    //         target.SetInt("_PotatoMode", 0);
                    //         target.SetInt("_HQMode", 0);
                    //         break;
                    //     case VolumetricQuality.HQ:
                    //         target.SetInt("_PotatoMode", 0);
                    //         target.SetInt("_HQMode", 1);
                    //         break;
                    //     default:
                    //         break;  
                    // }




                    // if((Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1)
                    // {
                    //     matEditor.ShaderProperty(_Noise2StretchPotato, new GUIContent("Outside Magic Noise Scale", "Second Layer of Noise Scale"));
                    //     matEditor.ShaderProperty(_Noise2StretchInsidePotato, new GUIContent("Inside Magic Noise Scale", "Second Layer of Noise Scale"));
                    //     matEditor.ShaderProperty(_Noise2XPotato, new GUIContent("Magic Noise X Scroll", "Second Layer of Noise Scroll X Axis"));
                    //     matEditor.ShaderProperty(_Noise2YPotato, new GUIContent("Magic Noise Y Scroll", "Second Layer of Noise Scroll Y Axis"));
                    //     matEditor.ShaderProperty(_Noise2ZPotato, new GUIContent("Magic Noise Z Scroll", "Second Layer of Noise Scroll Y Axis"));
                    //     matEditor.ShaderProperty(_Noise2PowerPotato, new GUIContent("Magic Noise Strength", "Controls how much the second layer of noise affects the cone"));
                    // }
                    if((Mathf.FloorToInt(target.GetInt("_HQMode"))) == 1)
                    {
                        matEditor.ShaderProperty(_Noise2Stretch, new GUIContent("HQ Outside Magic Noise Scale", "Second Layer of Noise Scale"));
                        matEditor.ShaderProperty(_Noise2StretchInside, new GUIContent("HQ Inside Magic Noise Scale", "Second Layer of Noise Scale"));
                        matEditor.ShaderProperty(_Noise2X, new GUIContent("HQ Magic Noise X Scroll", "Second Layer of Noise Scroll X Axis"));
                        matEditor.ShaderProperty(_Noise2Y, new GUIContent("HQ Magic Noise Y Scroll", "Second Layer of Noise Scroll Y Axis"));
                        matEditor.ShaderProperty(_Noise2Z, new GUIContent("HQ Magic Noise Z Scroll", "Second Layer of Noise Scroll Y Axis"));
                        matEditor.ShaderProperty(_Noise2Power, new GUIContent("HQ Magic Noise Strength", "Controls how much the second layer of noise affects the cone"));
                    }
                    else
                    {
                        matEditor.ShaderProperty(_Noise2StretchDefault, new GUIContent("Outside Magic Noise Scale", "Second Layer of Noise Scale"));
                        matEditor.ShaderProperty(_Noise2StretchInsideDefault, new GUIContent("Inside Magic Noise Scale", "Second Layer of Noise Scale"));
                        matEditor.ShaderProperty(_Noise2XDefault, new GUIContent("Magic Noise X Scroll", "Second Layer of Noise Scroll X Axis"));
                        matEditor.ShaderProperty(_Noise2YDefault, new GUIContent("Magic Noise Y Scroll", "Second Layer of Noise Scroll Y Axis"));
                        matEditor.ShaderProperty(_Noise2ZDefault, new GUIContent("Magic Noise Z Scroll", "Second Layer of Noise Scroll Y Axis"));
                        matEditor.ShaderProperty(_Noise2PowerDefault, new GUIContent("Magic Noise Strength", "Controls how much the second layer of noise affects the cone"));
                    }
                    EditorGUI.indentLevel--;
                }
                else if((Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1 && target.GetInt("_RenderMode") != 2)
                {
                    EditorGUILayout.LabelField("Magic 3D Noise is unavailable. Disable Potato Mode to enable Magic 3D Noise.");
                    matEditor.ShaderProperty(_PotatoMode, new GUIContent("Potato Mode", "Reduces the overhead on the fragment shader by removing both noise components to extra texture sampling."));
                }
                else if(target.GetInt("_RenderMode") ==2)
                {
                    EditorGUILayout.LabelField("Magic 3D Noise is unavailable. Use Transparent or HQTransparent Render Mode to enable Magic 3D Noise.");
                }
                else
                {
                    matEditor.ShaderProperty(magicNoiseProp, new GUIContent("Enable Magic 3D Noise For:  " + magicNoiseSuffix, "Enable Second layer of world space, faux 3D Noise"));
                    matEditor.ShaderProperty(_PotatoMode, new GUIContent("Potato Mode", "Reduces the overhead on the fragment shader by removing both noise components to extra texture sampling."));
                }
                
                SetKeyword(target, "_MAGIC_NOISE_ON_HIGH", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_HIGH"))) == 1 ? true : false);
                SetKeyword(target, "_MAGIC_NOISE_ON_MED", (Mathf.FloorToInt(target.GetInt("_MAGIC_NOISE_ON_MED"))) == 1 ? true : false);


                SetKeyword(target, "_USE_DEPTH_LIGHT", (Mathf.FloorToInt(target.GetInt("_UseDepthLight"))) == 1 ? true : false);
                SetKeyword(target, "_POTATO_MODE_ON", (Mathf.FloorToInt(target.GetInt("_PotatoMode"))) == 1 ? true : false);
                SetKeyword(target, "_HQ_MODE", (Mathf.FloorToInt(target.GetInt("_HQMode"))) == 1 ? true : false);
                SetKeyword(target, "_2D_NOISE_ON", (Mathf.FloorToInt(target.GetInt("_2D_NOISE_ON"))) == 1 ? true : false);



            //}
            GUILayout.Space(5);
            //matEditor.ShaderProperty(_BlendSrc, new GUIContent("Volumetric Blend Source", "Volumetric Transparency Blend Options (Soruce)"));
            //matEditor.ShaderProperty(_BlendDst, new GUIContent("Volumetric Blend Destination", "Volumetric Transparency Blend Options (Destination)"));
            //matEditor.ShaderProperty(_BlendOp, new GUIContent("Volumetric Blend Operation", "Volumetric Transparency Blend Options (Operation)"));
           // matEditor.TexturePropertySingleLine(new GUIContent("Inside Cone Normal Map", "Normal map to adjust direction of normals to increase the brightness when looking down cone."), _InsideConeNormalMap);
            GUILayout.Space(5);
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        //VOLUMETRIC CONE SETTINGS
        showVolumetricControls = VRSLStyles.ShurikenFoldout("Volumetric Settings", showVolumetricControls);
        if(showVolumetricControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_MinimumBeamRadius, new GUIContent("Minimum Beam Radius", "Minimum Beam Radius."));
         //   matEditor.ShaderProperty(_FixtureLensOrigin, new GUIContent("Center Of Fixture Lens (For Blinding Effect)", "This value sets where the brightest spot in the fixture should be. This helps with the blinding effect."));
            matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Max Cone Intensity", "Maximum light intensity for the volumetric cone."));            
            matEditor.ShaderProperty(_FadeStrength, new GUIContent("Edge Fade Amount", "Outer and Inner edge fade strength."));
            matEditor.ShaderProperty(_InnerFadeStrength, new GUIContent("Inner Edge Fade Amount", "Inner edge fade only."));
            matEditor.ShaderProperty(_InnerIntensityCurve, new GUIContent("Inner Edge Fade Intensity Curve", "Inner edge fade intensity curve."));
            matEditor.ShaderProperty(_DistFade, new GUIContent("Distance Fade", "How close the camera needs to be before the cone starts fading away."));
            matEditor.ShaderProperty(_FadeAmt, new GUIContent("Blend Amount", "How much does the cone blend with what's behind it."));
            matEditor.ShaderProperty(_BlindingAngleMod, new GUIContent("Blinding Angle Modification", "Changes the angle at which the fixture starts to become blinding when looking direcily into it."));
            matEditor.ShaderProperty(_BlindingStrength, new GUIContent("Blinding Strength", "Changes how strong the blinding effect is."));
//            matEditor.ShaderProperty(_IntersectionMod, new GUIContent("Intersection Modification", "The rate at which the volumetric fades away when intersecting with other objects."));
            GUILayout.Space(10);
            matEditor.ShaderProperty(_GoboBeamSplitEnable, new GUIContent("Enable Gobo Beam Split", "Enable beam splitting on gobos 2-6 (Global)"));     
            matEditor.ShaderProperty(_ProjectionSelection, new GUIContent("Projection Selection", "Use this to change what projection is selected. Usually overridden by Udon"));
            matEditor.ShaderProperty(_StripeSplit, new GUIContent("Stripe Count GOBO 2", "Number of alpha stripes to appear in the cone."));
            matEditor.ShaderProperty(_StripeSplitStrength, new GUIContent("Stripe Split Strength GOBO 2", "How strong the stripes appear in the cone."));
            
            // if(!isDMXCompatible)
            // {
                matEditor.ShaderProperty(_StripeSplit2, new GUIContent("Stripe Count GOBO 3", "Number of alpha stripes to appear in the cone."));
                matEditor.ShaderProperty(_StripeSplitStrength2, new GUIContent("Stripe Split Strength GOBO 3", "How strong the stripes appear in the cone."));
                 matEditor.ShaderProperty(_StripeSplit3, new GUIContent("Stripe Count GOBO 4", "Number of alpha stripes to appear in the cone."));
                matEditor.ShaderProperty(_StripeSplitStrength3, new GUIContent("Stripe Split Strength GOBO 4", "How strong the stripes appear in the cone."));
                 matEditor.ShaderProperty(_StripeSplit4, new GUIContent("Stripe Count GOBO 5", "Number of alpha stripes to appear in the cone."));
                matEditor.ShaderProperty(_StripeSplitStrength4, new GUIContent("Stripe Split Strength GOBO 5", "How strong the stripes appear in the cone."));
                 matEditor.ShaderProperty(_StripeSplit5, new GUIContent("Stripe Count GOBO 6", "Number of alpha stripes to appear in the cone."));
                matEditor.ShaderProperty(_StripeSplitStrength5, new GUIContent("Stripe Split Strength GOBO 6", "How strong the stripes appear in the cone."));
                 matEditor.ShaderProperty(_StripeSplit6, new GUIContent("Stripe Count GOBO 7", "Number of alpha stripes to appear in the cone."));
                matEditor.ShaderProperty(_StripeSplitStrength6, new GUIContent("Stripe Split Strength GOBO 7", "How strong the stripes appear in the cone."));
                 matEditor.ShaderProperty(_StripeSplit7, new GUIContent("Stripe Count GOBO 8", "Number of alpha stripes to appear in the cone."));
                matEditor.ShaderProperty(_StripeSplitStrength7, new GUIContent("Stripe Split Strength GOBO 8", "How strong the stripes appear in the cone."));
            // }
            matEditor.ShaderProperty(_EnableSpin, new GUIContent("Enable Auto Spin", "Enable/Disable the projection's automatic spinning. Usually controlled by Udon."));
            matEditor.ShaderProperty(_SpinSpeed, new GUIContent("Auto Spin Speed", "The speed at which it the projection will spin when auto spin is enabled."));
            GUILayout.Space(5);
            VRSLStyles.PartingLine();
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("''Cone Width'' and ''Cone Length'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
            matEditor.ShaderProperty(_ConeWidth, new GUIContent("Cone Width", "Changes the radius of the cone to be larger or smaller."));
            matEditor.ShaderProperty(_ConeLength, new GUIContent("Cone Length", "Changes how long the volumetric cone is via the texture coordinates"));
            matEditor.ShaderProperty(_MaxConeLength, new GUIContent("Max Cone Length", "Changes how long the volumetric cone is via the mesh."));
            matEditor.ShaderProperty(_ConeSync, new GUIContent("Cone Scale Sync", "Changes the rate at which the cone scales from source to the end of the cone. Highly recommened to use default settings if unsure."));
            GUILayout.Space(5);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        //MOVER CONTROLS
        if(isStaticLight == false && isAudioLink == false)
        {
            showMoverControls = VRSLStyles.ShurikenFoldout("Movement Settings", showMoverControls);
            if(showMoverControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_FixtureRotationOrigin, new GUIContent("Fixture Pivot Origin", "Sets the rotation point of the fixture for tilt in object space. Do not change this unless you are trying to make a custom housing."));
                matEditor.ShaderProperty(_MaxMinPanAngle, new GUIContent("Max/Min Pan Angles (Left/Right)", "Sets the Left/Right rotation range of the fixture by ''-Value to Value''"));
                matEditor.ShaderProperty(_MaxMinTiltAngle, new GUIContent("Max/Min Tilt Angles (Up/Down)", "Sets the Up/Down rotation range of the fixture by ''-Value to Value''"));
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }

        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();
        
    }

    public void MoverLightFixtureGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
           
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_Udon_DMXGridRenderTextureMovement);
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_Udon_DMXGridStrobeTimer);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
        //GENERAL CONTROLS
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            matEditor.ShaderProperty(_FixtureBaseRotationY, new GUIContent("Rotation Y Offset", "Offset the Y Rotation of the fixture."));
            matEditor.ShaderProperty(_FixtureRotationX, new GUIContent("Rotation X Offset", "Offset the X Rotation of the fixture."));
            ColorTextureSamplingGUI(matEditor, props, target);
            matEditor.ShaderProperty(_Saturation, new GUIContent("Saturation", "Saturation modifier for light color."));
            matEditor.ShaderProperty(_LensMaxBrightness, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
            matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the lens brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        //MOVER CONTROLS
        if(isStaticLight == false && isAudioLink == false)
        {
            showMoverControls = VRSLStyles.ShurikenFoldout("Movement Settings", showMoverControls);
            if(showMoverControls)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_FixtureRotationOrigin, new GUIContent("Fixture Pivot Origin", "Sets the rotation point of the fixture for tilt in object space. Do not change this unless you are trying to make a custom housing."));
                matEditor.ShaderProperty(_MaxMinPanAngle, new GUIContent("Max/Min Pan Angles (Left/Right)", "Sets the Left/Right rotation range of the fixture by ''-Value to Value''"));
                matEditor.ShaderProperty(_MaxMinTiltAngle, new GUIContent("Max/Min Tilt Angles (Up/Down)", "Sets the Up/Down rotation range of the fixture by ''-Value to Value''"));
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }

        //FIXTURE HOUSING SETTINGS
        showFixtureHousingControls = VRSLStyles.ShurikenFoldout("Fixture Housing Settings", showFixtureHousingControls);
        if(showFixtureHousingControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_LightingModel, new GUIContent("Lighting Model", "Choose the type of lighting this fixture mesh recieves when interacting with the world."));
            SetKeyword(target, "_LIGHTING_MODEL", (Mathf.FloorToInt(target.GetInt("_LightingModel"))) == 1 ? true : false);
            matEditor.ShaderProperty(_LightProbeMethod, new GUIContent("Light Probe Sampling Method", "Choose the light probe sampling method for the fixture housing."));
            matEditor.ShaderProperty(_Color, new GUIContent("Fixture Housing Color Tint", "The main diffuse color for the fixture housing"));
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Diffuse Map", "The main diffuse texture for the fixture housing."), _MainTex);
            matEditor.TextureScaleOffsetProperty(_MainTex);
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Normal Map", "The normal map for the fixture housing."), _BumpMap);
            matEditor.TextureScaleOffsetProperty(_BumpMap);
            matEditor.ShaderProperty(_BumpScale, new GUIContent("Normal Map Strength", "The strength of the normal map for the fixture housing."));
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Metallic/Gloss Map", "The Metallic/Gloss map for the fixture housing."), _MetallicGlossMap);
            matEditor.TextureScaleOffsetProperty(_MetallicGlossMap);
            matEditor.ShaderProperty(_Metallic, new GUIContent("Fixture Housing Metallic Level", "The metallic level of the fixture housing."));
            matEditor.ShaderProperty(_Glossiness, new GUIContent("Fixture Housing Glossiness Level", "The glossines of the fixture housing."));
            matEditor.TexturePropertySingleLine(new GUIContent("Decorative Emissive Map", "Decorative Emissive Map for the housing"), _DecorativeEmissiveMap);
            matEditor.TextureScaleOffsetProperty(_DecorativeEmissiveMap);
            matEditor.ShaderProperty(_DecorativeEmissiveMapStrength, new GUIContent("Decorative Emissive Map Strength", "The strength of the decorative emissive map."));
            if((Mathf.FloorToInt(target.GetInt("_LightingModel"))) == 1)
            {
                matEditor.TexturePropertySingleLine(new GUIContent("Fixture Housing Occlusion Map", "The Occlussion Map for the fixture housing."), _OcclusionMap);
                matEditor.ShaderProperty(_OcclusionStrength, new GUIContent("Fixture Housing Occlusion Strength", "The strength of the Occlusion Map for the fixture housing."));
            }
            GUILayout.Space(5);
            // if (Shader.Find("AreaLit/Standard") != null)
            // {
            //     matEditor.ShaderProperty(_AreaLitToggle, new GUIContent("Enable AreaLit", "Enable Area Lit"));
            //     if(target.GetInt("_AreaLitToggle") == 1)
            //     {
            //         EditorGUI.indentLevel++;
            //         matEditor.ShaderProperty(_AreaLitStrength, new GUIContent("Strength", "Area Lit Strength"));
            //         matEditor.ShaderProperty(_AreaLitRoughnessMult, new GUIContent("Roughness Mulitplier", "Area Lit Roughness Multiplier"));
            //         matEditor.ShaderProperty(_OpaqueLights, new GUIContent("Opaque Lights", "Enable Area Lit Opaque Lights Feature"));
            //         matEditor.TexturePropertySingleLine(new GUIContent("Light Mesh", "Area Lit Light Mesh Texture"), _LightMesh);
            //         matEditor.TexturePropertySingleLine(new GUIContent("Light Tex 0", "Area Lit Texture 0"), _LightTex0);
            //         matEditor.TexturePropertySingleLine(new GUIContent("Light Tex 1", "Area Lit Texture 1"), _LightTex1);
            //         matEditor.TexturePropertySingleLine(new GUIContent("Light Tex 2", "Area Lit Texture 2"), _LightTex2);
            //         matEditor.TexturePropertySingleLine(new GUIContent("Light Tex 3+", "Area Lit Texture 3"), _LightTex3);
            //         matEditor.TexturePropertySingleLine(new GUIContent("Mask", "Area Lit Mask"), _AreaLitMask);
            //         matEditor.TexturePropertySingleLine(new GUIContent("Occlusion", "Area Lit Occlusion Texture"), _AreaLitOcclusion);
            //         if(_AreaLitOcclusion.textureValue){
            //             matEditor.ShaderProperty(_OcclusionUVSet, new GUIContent("Occlussion UV Set", "Area Lit Occlussion UV Set"));
            //         }
            //         EditorGUILayout.HelpBox("Note that the AreaLit package files MUST be inside a folder named AreaLit (case sensitive) directly in the Assets folder (Assets/AreaLit)", MessageType.Info);
            //         EditorGUI.indentLevel--;
            //     }
                
            // }
            // else {
            //     _AreaLitToggle.floatValue = 0f;
			// 	target.SetInt("_AreaLitToggle", 0);
            // }
            // SetKeyword(target, "_AREALIT_ON", (Mathf.FloorToInt(target.GetInt("_AreaLitToggle"))) == 1 ? true : false);
            EditorGUI.indentLevel--;


            GUILayout.Space(5);
        }
        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();
    }

    public void ColorTextureSamplingGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        if(isDMXCompatible || isRTShader || isDiscoBall) return;
            matEditor.ShaderProperty(_EnableColorTextureSample, new GUIContent("Enable Color Texture Sampling", "Check this box if you wish to sample seperate texture for the color. The color will be influenced by the intensity of the original emission color!"));
            EditorGUI.indentLevel++;
            matEditor.TexturePropertySingleLine(new GUIContent("Color Sampling Texture", "The texture to sample the color from when ''Enable Color Texture Sampling'' is enabled"),_SamplingTexture);
            matEditor.ShaderProperty(_TextureColorSampleX, new GUIContent("X UV Coordinate", "The x uv coordinate for where on the texture to sample from (0 to 1)."));
            matEditor.ShaderProperty(_TextureColorSampleY, new GUIContent("Y UV Coordinate", "The y uv coordinate for where on the texture to sample from (0 to 1)."));
            matEditor.ShaderProperty(_RenderTextureMultiplier, new GUIContent("Render Texture Multiplier", "Increase the strength of the render texture color"));
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        
    }




    public void DiscoballGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        AudioLinkGUI(matEditor, props, target);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            // if(props[1].floatValue > 0)
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                VRSLStyles.PartingLine();
                // matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_Udon_DMXGridRenderTexture);
                GUILayout.Space(5);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
        //GENERAL CONTROLS
        showGeneralControls = VRSLStyles.ShurikenFoldout("General Controls", showGeneralControls);
        if(showGeneralControls)
        {
            // if(isDMXCompatible)
            // {
                matEditor.ShaderProperty(_RenderMode, new GUIContent("Render Mode", "Choose between a fully transparent shader, or one that is opaque with a dithering technique."));
                if(target.GetInt("_RenderMode") == 1) 
                {
                    target.SetOverrideTag("RenderType", "Transparent");
                    target.DisableKeyword("_ALPHATEST_ON");  
                    //target.SetInt("_BlendSrc", 1);
                    target.SetInt("_BlendDst", 1);
                    target.SetInt("_ZWrite", 0);
                    target.SetInt("_AlphaToCoverage", 0);
                    target.SetInt("_HQMode", 0);
                    target.renderQueue = 3001;
                }
                else
                {
                    matEditor.ShaderProperty(_ClippingThreshold, new GUIContent("Alpha Clip Threshold", "Adjust the clipping threshold of the dither effect."));
                    target.SetOverrideTag("RenderType", "Opaque");
                    target.EnableKeyword("_ALPHATEST_ON");
                    //target.SetInt("_BlendSrc", 0);
                    target.SetInt("_BlendDst", 0);
                    target.SetInt("_ZWrite", 1);
                    target.SetInt("_AlphaToCoverage", 1);
                    target.SetInt("_HQMode", 0);
                    target.renderQueue = 2451;
                }
            // }
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_GlobalIntensity, new GUIContent("Global Intensity", "Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity."));
            EditorGUI.indentLevel++;  
            matEditor.ShaderProperty(_GlobalIntensityBlend, new GUIContent("Global Intensity Blend", "Sets the overall intensity of the shader. Controls how much the Global Intesnity slider actually affects the output. Good for temporarily disabling animations that use the Global Intesnity property."));
            EditorGUI.indentLevel--; 
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Multiplier, new GUIContent("Intensity Multiplier", "General purpose intensity multiplier."));

            EditorGUI.indentLevel--;
            VRSLStyles.PartingLine();
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_Emission, new GUIContent("Emission Color", "The overall emissive color of the shader. Use this to tint the shader."));
            matEditor.TexturePropertySingleLine(new GUIContent("Discoball Projection Cube Map", "The cube map used to project a 360 degree image from the center of the object onto other objects."), _Cube);
            matEditor.TextureScaleOffsetProperty(_Cube);
            matEditor.ShaderProperty(_RotationSpeed, new GUIContent("Discoball Rotation Speed", "The speed at which the discoball spins."));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();


    }

    public void DMXInterpolationGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        matEditor.ShaderProperty(_UseOldSchoolSmoothing, new GUIContent("Use Old School Smoothing Technique", "Uses the old smoothing technique. Recommended for Light+Color Textures."));
        matEditor.ShaderProperty(_EnableLegacyGlobalMovementSpeedChannel, new GUIContent("Enable Legacy Global Movement Speed", "Enables the use of the old Global Movement Speed Channel (DMX Channel 511) instead of having each sector have its own movement speed control. /nThis will always be true when compatibility mode is enabled"));
        matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
        matEditor.ShaderProperty(_EnableDMX, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
        matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
        matEditor.ShaderProperty(_DMXChannel, new GUIContent("Sector","for legacy global movement speed"));
        matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Raw", "The raw DMX Render texture from the camera."),_DMXTexture);
        matEditor.ShaderProperty(_SmoothValue, new GUIContent("Smoothness Level", "Changes how much interpolated smoothing is applied to the texture. The closer to 0, the more smoothing applied, the closer to 1, the less smoothing applied. \nThis value is usually controlled by a seperate DMX signal to control the movement speed of the movers. "));
        matEditor.ShaderProperty(_MinimumSmoothnessDMX, new GUIContent("Minimum Smoothness Value", "Sets the minimum amount of smoothing applied to the texture by default."));
        matEditor.ShaderProperty(_MaximumSmoothnessDMX, new GUIContent("Maximum Smoothness Value", "Sets the maximum amount of smoothing applied to the texture by default."));
        matEditor.RenderQueueField();
        EditorGUI.indentLevel--;
        GUILayout.Space(5);
    }
    public void DMXStrobeGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Raw", "The raw DMX Render texture from the camera."),_DMXTexture);
        matEditor.ShaderProperty(_StrobeType, new GUIContent("Strobe Mode", "Choose between a smooth dynamic strobe rate or static pre set rates. Static rates are frame rate independent."));
        SetKeyword(target, "_VRSL_STATICFREQUENCIES",target.GetInt("_StrobeType") == 1);
        if(target.GetInt("_StrobeType") == 1)
        {
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_LowFrequency, new GUIContent("Low Frequency"));
            matEditor.ShaderProperty(_MedFrequency, new GUIContent("Medium Frequency"));
            matEditor.ShaderProperty(_HighFrequency, new GUIContent("High Frequency"));
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_MaxStrobeFreq, new GUIContent("Maximum Strobe Frequency", "The maximum strobing frequency of all fixtures."));
            EditorGUI.indentLevel--;
        }
        matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
        matEditor.RenderQueueField();
        EditorGUI.indentLevel--;
        GUILayout.Space(5);
    }

    public void DMXSpinnerGUI(MaterialEditor matEditor, MaterialProperty[] props, Material target)
    {
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Raw", "The raw DMX Render texture from the camera."),_DMXTexture);
        matEditor.ShaderProperty(_NineUniverseMode, new GUIContent("Enable Extended Universe Mode", "Enables or Disables extended universe mode (9-universes via RGB)"));
        matEditor.RenderQueueField();
        EditorGUI.indentLevel--;
        GUILayout.Space(5);
    }

    public string GetShaderType()
    {
        string lightType = "";
        string shaderType = "";
        string dmx = "";
        if(isMoverLight)
        {
            lightType = "Mover";
        }
        if(isStaticLight)
        {
            lightType = "Static";
            if(isFlasher)
            {
                lightType = lightType + " Flasher";
            }
        }
        if(isLensFlare)
        {
            lightType = "Lens Flare";
        }
        if(isSurfaceStatic)
        {
            if(isMultiChannelBar)
            {
                lightType = "Multi-Channel Bar";
            }
            else
            {
                lightType = "Surface Static";
                shaderType = "Fixture";
            }
        }
        if(isProjection)
        {
            shaderType = "Projection";
        }
        if(isVolumetric)
        {
            shaderType = "Volumetric";
        }
        if(isFixture)
        {
            shaderType = "Fixture";
        }
        // if(isStaticLight)
        // {
        //     shaderType = "Static";
        // }

        if(isDMXCompatible && !isAudioLink)
        {
            dmx = "DMX";
        }
        else if (isAudioLink)
        {
            dmx = "AudioLink";
        }
        else
        {
            dmx = "Non-DMX";
        }
        if(isDiscoBall)
        {
            if(!isAudioLink)
            {
                lightType = "Discoball";
                shaderType = "Projection";
                return "Shader: " + dmx + " " + lightType;
            }
            else
            {
                lightType = "Discoball";
                shaderType = "Projection";
                dmx = "AudioLink";
                return "Shader: " + dmx + " " + lightType;
            }

        }
        if(isRTShader && isRTStrobe && !isRTSpin)
        {
            dmx = "DMX";
            lightType = "CRT";
            shaderType = "Strobe Timer";
        }
        if(isRTShader && !isRTStrobe && isRTSpin)
        {
            dmx = "DMX";
            lightType = "CRT";
            shaderType = "Spin Timer";
        }

        if(isRTShader && !isRTStrobe && !isRTSpin)
        {
            dmx = "DMX";
            lightType = "CRT";
            shaderType = "Signal Interpolation/Smoothing";
        }
        return "Shader: " + dmx + " " + lightType + ": " + shaderType;

    }

        public static void SetKeyword(Material mat, string keyword, bool status)
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
#endif