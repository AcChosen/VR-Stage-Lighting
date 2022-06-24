using System.Reflection;
using UnityEditor;
using UnityEngine;

// Based on Morioh's toon shader GUI.
// This code is based off synqark's arktoon-shaders and Xiexe. 
// Citation to "https://github.com/synqark", "https://github.com/synqark/arktoon-shaders", https://gitlab.com/xMorioh/moriohs-toon-shader.
public class VRSLInspector : ShaderGUI
{
    
    BindingFlags bindingFlags = BindingFlags.Public |
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Static;
    MaterialProperty _DMXChannel = null;
    MaterialProperty _EnableOSC = null;
    MaterialProperty _EnableExtraChannels = null;
    MaterialProperty _OSCGridRenderTextureRAW = null;
    MaterialProperty _OSCGridRenderTexture = null;
    MaterialProperty _OSCGridStrobeTimer = null;
    MaterialProperty _OSCGridSpinTimer = null;
    MaterialProperty _UseRawGrid = null;
    MaterialProperty _EnableCompatibilityMode = null;
    MaterialProperty _EnableVerticalMode = null;
    MaterialProperty _EnableLegacyGlobalMovementSpeedChannel = null;

    MaterialProperty _GlobalIntensity = null;
    MaterialProperty _FinalIntensity = null;
    MaterialProperty _UniversalIntensity = null;
    MaterialProperty _Emission = null;
    MaterialProperty _CurveMod = null;
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

    MaterialProperty _MainTex = null;
    MaterialProperty _Color = null;
    MaterialProperty _BumpMap = null;
    MaterialProperty _BumpScale = null;
    MaterialProperty _NormalMap = null;
    MaterialProperty _MetallicGlossMap = null;
    MaterialProperty _EmissionMask = null;
    MaterialProperty _Metallic = null;
    MaterialProperty _Glossiness = null;
    //Volumetric Texture specific

    MaterialProperty _LightMainTex = null;
    MaterialProperty _NoiseTex = null;
    MaterialProperty _NoisePower = null;
    //MaterialProperty _NoiseSeed = null;
    MaterialProperty _Noise2X = null;
    MaterialProperty _Noise2Y = null;
    MaterialProperty _Noise2Z = null;
    MaterialProperty _Noise2Stretch = null;
    MaterialProperty _Noise2StretchInside = null;
    MaterialProperty _Noise2Power = null;
    MaterialProperty _ToggleMagicNoise = null;
   // MaterialProperty _InsideConeNormalMap = null;

    //Volumetric Control Specific
    MaterialProperty _FixtureMaxIntensity = null;
    MaterialProperty _FadeStrength = null;
    MaterialProperty _InnerFadeStrength = null;
    MaterialProperty _InnerIntensityCurve = null;
    MaterialProperty _DistFade = null;
    MaterialProperty _FadeAmt = null;
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
    MaterialProperty _BlendSrc = null;
    MaterialProperty _BlendDst = null;
    MaterialProperty _BlendOp = null;

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
    MaterialProperty _MinimumSmoothnessOSC = null;
    MaterialProperty _MaximumSmoothnessOSC = null;

    //Strobe RenderTexture
    MaterialProperty _MaxStrobeFreq = null;

    //Texture Color Sampling Stuff
    MaterialProperty _TextureColorSampleX = null;
    MaterialProperty _TextureColorSampleY = null;
    MaterialProperty _SamplingTexture = null;
    MaterialProperty _EnableColorTextureSample = null;



    
    //DiscoBall Exclusives
    MaterialProperty _Cube = null;
    MaterialProperty _RotationSpeed = null;
    MaterialProperty _Multiplier = null;
    //END Discoball Exclusives

    //Shader Type Identifiers
    bool isDiscoBall = false;
    bool isMoverLight = false;
    bool isStaticLight = false;
    bool isProjection = false;
    bool isFixture = false;
    bool isVolumetric = false;
    bool isDMXCompatible = false;

    bool isRTShader = false;
    bool isRTStrobe = false;
    bool isRTSpin = false;
    bool isAudioLink = false;

    
    //END Shader Type Identifiers

    //Foldout Bools
    static bool showDMXSettings = true;
    static bool showGeneralControls = true;
    static bool showMoverControls = true;
    static bool showFixtureHousingControls = true;
    static bool showVolumetricTextureSettings = true;
    static bool showVolumetricControls = true;

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
            VRSLStyles.ShurikenHeaderCentered(VRSLStyles.ver);
            VRSLStyles.ShurikenHeaderCentered(GetShaderType());
            VRSLStyles.PartingLine();
            VRSLStyles.DepthPassWarning();
            GUILayout.Space(5);
            if(isDiscoBall)
            {
                    DiscoballGUI(materialEditor, props);
                    return;

            }
            if(isMoverLight)
            {
                if(isMoverLight && isFixture)
                {
                    MoverLightFixtureGUI(materialEditor, props);
                    return;
                }
                if(isMoverLight && isVolumetric)
                {
                    MoverLightVolumetricGUI(materialEditor, props);
                    return;
                }
                if(isMoverLight && isProjection)
                {
                    MoverLightProjectionGUI(materialEditor,props);
                    return;
                }
            }
            if(isStaticLight)
            {
                if(isStaticLight && isFixture)
                {
                    StaticLightFixtureGUI(materialEditor,props);
                    return;
                }
                if(isStaticLight && isProjection)
                {
                    StaticLightProjectionGUI(materialEditor,props);
                    return;
                }
            }
            if(isRTShader)
            {
                if(isRTStrobe && !isRTSpin)
                {
                    DMXStrobeGUI(materialEditor,props);
                    return;
                }
                else if(isRTSpin && !isRTStrobe)
                {
                    DMXSpinnerGUI(materialEditor, props);
                    return;
                }
                else
                {
                    DMXInterpolationGUI(materialEditor,props);
                    return;
                }
            }
        }
        //END GUI STuff

    }

    public void AudioLinkGUI(MaterialEditor matEditor, MaterialProperty[] props)
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
                if(!isDiscoBall)
                    matEditor.ShaderProperty(_EnableColorChord, new GUIContent("Enable Color Chord", "Enables or disables Color Chord tinting. \nThis will tint the final color of the light after texture tinting."));
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }
        else
        {
            return;
        }
    }

    public void StaticLightProjectionGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        AudioLinkGUI(matEditor, props);
        //DMX CONTROLS
        if(isDMXCompatible)
        {
            showDMXSettings = VRSLStyles.ShurikenFoldout("DMX Settings", showDMXSettings);
            if(showDMXSettings && isDMXCompatible)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("''Sector'' and ''Enable DMX'' are usually overridden by their corresponding Udon Script. \nAdjust these at your own risk.", MessageType.Info,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
                matEditor.ShaderProperty(_EnableVerticalMode, new GUIContent("Enable Vertical Mode", "Switches this material to read from the vertical grid instead of the horizontal when not in legacy mode."));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                //matEditor.ShaderProperty(_LegacyGoboRange, new GUIContent("Enable Legacy Gobo Range", "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_OSCGridRenderTextureRAW);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_OSCGridRenderTexture);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_OSCGridStrobeTimer);
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
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            GUILayout.Space(10);
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            ColorTextureSamplingGUI(matEditor, props);

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

    public void StaticLightFixtureGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        AudioLinkGUI(matEditor, props);
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
                matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_OSCGridRenderTextureRAW);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_OSCGridRenderTexture);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_OSCGridStrobeTimer);
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
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            ColorTextureSamplingGUI(matEditor, props);
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

    public void MoverLightProjectionGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        AudioLinkGUI(matEditor, props);
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
                matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                matEditor.ShaderProperty(_LegacyGoboRange, new GUIContent("Enable Legacy Gobo Range", "Use Only the first 6 gobos instead of all. This is for legacy content where only 6 gobos were originally supported and the channel range was different."));
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_OSCGridRenderTextureRAW);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out heavily by a custom render texture. Used for movement."),_OSCGridRenderTexture);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_OSCGridStrobeTimer);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Spin Timer", "DMX Grid with GOBO Spin timings embedded"),_OSCGridSpinTimer);
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
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_FixtureBaseRotationY, new GUIContent("Rotation Y Offset", "Offset the Y Rotation of the fixture."));
            matEditor.ShaderProperty(_FixtureRotationX, new GUIContent("Rotation X Offset", "Offset the X Rotation of the fixture."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            ColorTextureSamplingGUI(matEditor, props);
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
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            matEditor.ShaderProperty(_ProjectionRotation, new GUIContent("Static Projection UV Rotation", "Changes the default angle the projected image is at."));
            matEditor.ShaderProperty(_EnableSpin, new GUIContent("Enable Auto Spin", "Enable/Disable the projection's automatic spinning. Usually controlled by Udon."));
            
            matEditor.ShaderProperty(_SpinSpeed, new GUIContent("Auto Spin Speed", "The speed at which it the projection will spin when auto spin is enabled."));
            matEditor.ShaderProperty(_ProjectionIntensity, new GUIContent("Projection Intensity", "Changes how bright the projection is"));
            matEditor.ShaderProperty(_ProjectionFade, new GUIContent("Projection Edge Fade", "Changes how much the edges of the projection are faded away."));
            matEditor.ShaderProperty(_ProjectionFadeCurve, new GUIContent("Projection Edge Fade Harshness", "Changes the rate at which the edges of the projection are faded."));
            matEditor.ShaderProperty(_ProjectionDistanceFallOff, new GUIContent("Projection Distance Fallof Strength", "How quickly the the projection loses strength the further away it is from the source."));
            matEditor.ShaderProperty(_ProjectionRange, new GUIContent("Projection Drawing Range", "Changes the length of the projecction mesh itself. The longer it is, the further away the projection will reach before clipping, but also more pixels on the screen the projection will take up. \nIncrease this if you see the edges of the projection clipping too much from the light being too far away."));
            matEditor.ShaderProperty(_ProjectionRangeOrigin, new GUIContent("Projection Drawing Range Scale Origin", "The place where the projection mesh is being scaled from. Don't change this unless you know what you are doing. "));
           // matEditor.DefaultShaderProperty
            matEditor.ShaderProperty(_BlendSrc, new GUIContent("Projection Blend Source", "Projection Transparency Blend Options (Soruce)"));
            matEditor.ShaderProperty(_BlendDst, new GUIContent("Projection Blend Destination", "Projection Transparency Blend Options (Destination)"));
            matEditor.ShaderProperty(_BlendOp, new GUIContent("Projection Blend Operation", "Projection Transparency Blend Options (Operation)"));
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

        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();       
    }

    public void MoverLightVolumetricGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        AudioLinkGUI(matEditor, props);
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
                matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                matEditor.ShaderProperty(_EnableExtraChannels, new GUIContent("Enable Cone Length DMX Controls","Enable this if you want to be able to extend the lenghth of the cone on Channel 2!"));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_OSCGridRenderTextureRAW);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out heavily by a custom render texture. Used for movement."),_OSCGridRenderTexture);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_OSCGridStrobeTimer);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Spin Timer", "DMX Grid with GOBO Spin timings embedded"),_OSCGridSpinTimer);
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
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            matEditor.ShaderProperty(_FixtureBaseRotationY, new GUIContent("Rotation Y Offset", "Offset the Y Rotation of the fixture."));
            matEditor.ShaderProperty(_FixtureRotationX, new GUIContent("Rotation X Offset", "Offset the X Rotation of the fixture."));
            ColorTextureSamplingGUI(matEditor, props);
            matEditor.ShaderProperty(_Saturation, new GUIContent("Saturation", "Saturation modifier for light color."));
            matEditor.ShaderProperty(_SaturationLength, new GUIContent("Saturation Length", "Har far from the source does the saturation slider affect the shader."));
            matEditor.ShaderProperty(_LensMaxBrightness, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
            //matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        //VOLUMETRIC TEXTURE SETTINGS
        showVolumetricTextureSettings = VRSLStyles.ShurikenFoldout("Cone Texture Settings", showVolumetricTextureSettings);
        if(showVolumetricTextureSettings)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
            GUILayout.Space(5);
            //matEditor.TextureScaleOffsetProperty(_LightMainTex);
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Noise Texture", "Alpha Noise Texture used for adding variation to the cone."), _NoiseTex);
            matEditor.TextureScaleOffsetProperty(_NoiseTex);
            matEditor.ShaderProperty(_NoisePower, new GUIContent("Noise Strength", "Controls how much the noise texture affects the cone"));
           // matEditor.ShaderProperty(_NoiseSeed, new GUIContent("Noise Randomization", "Adds randomness to the noise for more variation")); 
            GUILayout.Space(5);
            matEditor.TexturePropertySingleLine(new GUIContent("Magic 3D Noise Texture", "A magical texture for generating 3D Perlin Noise at runtime! Code and texture based on https://www.shadertoy.com/view/4sfGzS by iq!"), _LightMainTex);
            //if(!isDMXCompatible)
            //{
                matEditor.ShaderProperty(_ToggleMagicNoise, new GUIContent("Enable Magic 3D Noise", "Enable Second layer of world space, faux 3D Noise"));
                //if(isDMXCompatible)
              //  {
                    matEditor.ShaderProperty(_Noise2Stretch, new GUIContent("Outside Magic Noise Scale", "Second Layer of Noise Scale"));
                    matEditor.ShaderProperty(_Noise2StretchInside, new GUIContent("Inside Magic Noise Scale", "Second Layer of Noise Scale"));
               // }
                matEditor.ShaderProperty(_Noise2X, new GUIContent("Magic Noise X Scroll", "Second Layer of Noise Scroll X Axis"));
                matEditor.ShaderProperty(_Noise2Y, new GUIContent("Magic Noise Y Scroll", "Second Layer of Noise Scroll Y Axis"));
                matEditor.ShaderProperty(_Noise2Z, new GUIContent("Magic Noise Z Scroll", "Second Layer of Noise Scroll Y Axis"));
                matEditor.ShaderProperty(_Noise2Power, new GUIContent("Magic Noise Strength", "Controls how much the second layer of noise affects the cone"));
            //}
            GUILayout.Space(5);
            matEditor.ShaderProperty(_BlendSrc, new GUIContent("Volumetric Blend Source", "Volumetric Transparency Blend Options (Soruce)"));
            matEditor.ShaderProperty(_BlendDst, new GUIContent("Volumetric Blend Destination", "Volumetric Transparency Blend Options (Destination)"));
            matEditor.ShaderProperty(_BlendOp, new GUIContent("Volumetric Blend Operation", "Volumetric Transparency Blend Options (Operation)"));
           // matEditor.TexturePropertySingleLine(new GUIContent("Inside Cone Normal Map", "Normal map to adjust direction of normals to increase the brightness when looking down cone."), _InsideConeNormalMap);
            GUILayout.Space(5);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        //VOLUMETRIC CONE SETTINGS
        showVolumetricControls = VRSLStyles.ShurikenFoldout("Volumetric Settings", showVolumetricControls);
        if(showVolumetricControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
         //   matEditor.ShaderProperty(_FixtureLensOrigin, new GUIContent("Center Of Fixture Lens (For Blinding Effect)", "This value sets where the brightest spot in the fixture should be. This helps with the blinding effect."));
            matEditor.ShaderProperty(_FixtureMaxIntensity, new GUIContent("Max Cone Intensity", "Maximum light intensity for the volumetric cone."));            
            matEditor.ShaderProperty(_FadeStrength, new GUIContent("Edge Fade Amount", "Outer and Inner edge fade strength."));
            matEditor.ShaderProperty(_InnerFadeStrength, new GUIContent("Inner Edge Fade Amount", "Inner edge fade only."));
            matEditor.ShaderProperty(_InnerIntensityCurve, new GUIContent("Inner Edge Fade Intensity Curve", "Inner edge fade intensity curve."));
            matEditor.ShaderProperty(_DistFade, new GUIContent("Distance Fade", "How close the camera needs to be before the cone starts fading away."));
            matEditor.ShaderProperty(_FadeAmt, new GUIContent("Blend Amount", "How much does the cone blend with what's behind it."));

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

        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();
        
    }

    public void MoverLightFixtureGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        AudioLinkGUI(matEditor, props);
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
                matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                EditorGUI.indentLevel--;   
                VRSLStyles.PartingLine();
                EditorGUILayout.HelpBox("These are the render texture grids used to read DMX signals from a video panel.", MessageType.None,true);
                EditorGUI.indentLevel++;
                matEditor.ShaderProperty(_UseRawGrid, new GUIContent("Use Seperate Grid for Light Intensity and Color", "Use this to switch to the normal grid for light/color if smooothed is too slow"));
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_OSCGridRenderTextureRAW);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Smoothed", "DMX Render Texture smoothed out by a custom render texture."),_OSCGridRenderTexture);
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Strobe Timer", "DMX Grid with strobe timings embedded."),_OSCGridStrobeTimer);
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
            matEditor.ShaderProperty(_FinalIntensity, new GUIContent("Final Intensity", "Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI."));
            matEditor.ShaderProperty(_UniversalIntensity, new GUIContent("Universal Intensity", "Sets the maximum brightness value of both Final and GLobal Intensity. Good for personalized settings of the max brightness of the shader by other users via UI. Is non-instanced."));
            matEditor.ShaderProperty(_Emission, new GUIContent("Light Emission Color", "The color of the light!. Use this to color the emissive part of the material."));
            matEditor.ShaderProperty(_FixtureBaseRotationY, new GUIContent("Rotation Y Offset", "Offset the Y Rotation of the fixture."));
            matEditor.ShaderProperty(_FixtureRotationX, new GUIContent("Rotation X Offset", "Offset the X Rotation of the fixture."));
            ColorTextureSamplingGUI(matEditor, props);
            matEditor.ShaderProperty(_Saturation, new GUIContent("Saturation", "Saturation modifier for light color."));
            matEditor.ShaderProperty(_LensMaxBrightness, new GUIContent("Lens Max Brightness", "General slider for adjusting the max brightness of the lens"));
            matEditor.ShaderProperty(_FixutreIntensityMultiplier, new GUIContent ("Intensity Multiplier", "Multiplier for the lens brightness. Good for adjusting to increase bloom"));
            matEditor.EnableInstancingField();
            matEditor.RenderQueueField();
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }

        //MOVER CONTROLS
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

        //FIXTURE HOUSING SETTINGS
        showFixtureHousingControls = VRSLStyles.ShurikenFoldout("Fixture Housing Settings", showFixtureHousingControls);
        if(showFixtureHousingControls)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel++;
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
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        GUILayout.Space(15);
        // matEditor.EnableInstancingField();
        // matEditor.RenderQueueField();
    }

    public void ColorTextureSamplingGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        if(isDMXCompatible || isRTShader || isDiscoBall) return;
            matEditor.ShaderProperty(_EnableColorTextureSample, new GUIContent("Enable Color Texture Sampling", "Check this box if you wish to sample seperate texture for the color. The color will be influenced by the intensity of the original emission color!"));
            EditorGUI.indentLevel++;
            matEditor.TexturePropertySingleLine(new GUIContent("Color Sampling Texture", "The texture to sample the color from when ''Enable Color Texture Sampling'' is enabled"),_SamplingTexture);
            matEditor.ShaderProperty(_TextureColorSampleX, new GUIContent("X UV Coordinate", "The x uv coordinate for where on the texture to sample from (0 to 1)."));
            matEditor.ShaderProperty(_TextureColorSampleY, new GUIContent("Y UV Coordinate", "The y uv coordinate for where on the texture to sample from (0 to 1)."));
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
        
    }




    public void DiscoballGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        AudioLinkGUI(matEditor, props);
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
                matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
                matEditor.ShaderProperty(_DMXChannel, new GUIContent("DMX Channel","Chooses the DMX Address to start this fixture at."));
                VRSLStyles.PartingLine();
                matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid", "The DMX Render Texture to read from for color and intensity. Slightly smoothed."),_OSCGridRenderTextureRAW);
                GUILayout.Space(5);
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

    public void DMXInterpolationGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        matEditor.ShaderProperty(_EnableLegacyGlobalMovementSpeedChannel, new GUIContent("Enable Legacy Global Movement Speed", "Enables the use of the old Global Movement Speed Channel (DMX Channel 511) instead of having each sector have its own movement speed control. /nThis will always be true when compatibility mode is enabled"));
        matEditor.ShaderProperty(_EnableCompatibilityMode, new GUIContent("Enable Compatibility Mode", "Changes the grid from reading the new 208x1080 grid to the old 200x200 grid. \nThis property is not an instanced property."));
        matEditor.ShaderProperty(_EnableOSC, new GUIContent("Enable DMX", "Enables or Disables reading from the DMX Render Textures"));
        matEditor.ShaderProperty(_DMXChannel, new GUIContent("Sector","for legacy global movement speed"));
        matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Raw", "The raw DMX Render texture from the camera."),_OSCGridRenderTexture);
        matEditor.ShaderProperty(_SmoothValue, new GUIContent("Smoothness Level", "Changes how much interpolated smoothing is applied to the texture. The closer to 0, the more smoothing applied, the closer to 1, the less smoothing applied. \nThis value is usually controlled by a seperate DMX signal to control the movement speed of the movers. "));
        matEditor.ShaderProperty(_MinimumSmoothnessOSC, new GUIContent("Minimum Smoothness Value", "Sets the minimum amount of smoothing applied to the texture by default."));
        matEditor.ShaderProperty(_MaximumSmoothnessOSC, new GUIContent("Maximum Smoothness Value", "Sets the maximum amount of smoothing applied to the texture by default."));
        matEditor.RenderQueueField();
        EditorGUI.indentLevel--;
        GUILayout.Space(5);
    }
    public void DMXStrobeGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Raw", "The raw DMX Render texture from the camera."),_OSCGridRenderTexture);
        matEditor.ShaderProperty(_MaxStrobeFreq, new GUIContent("Maximum Strobe Frequency", "The maximum strobing frequency of all fixtures."));
        matEditor.RenderQueueField();
        EditorGUI.indentLevel--;
        GUILayout.Space(5);
    }

    public void DMXSpinnerGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        matEditor.TexturePropertySingleLine(new GUIContent("DMX Grid Raw", "The raw DMX Render texture from the camera."),_OSCGridRenderTexture);
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




}
