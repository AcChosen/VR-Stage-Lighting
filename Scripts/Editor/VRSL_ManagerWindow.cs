using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System.Threading;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
using UnityEngine.SceneManagement;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using System.Collections.Immutable;
using System.Collections.Generic;
using System;
using System.Linq;
#endif
#if !COMPILER_UDONSHARP && UNITY_EDITOR

 public static class Extensions
 {
     // Extensions class by Bunny83 
     // https://answers.unity.com/questions/960413/editor-window-how-to-center-a-window.html
 public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
 {
     var result = new List<System.Type>();
     var assemblies = aAppDomain.GetAssemblies();
     foreach (var assembly in assemblies)
     {
         var types = assembly.GetTypes();
         foreach (var type in types)
         {
             if (type.IsSubclassOf(aType))
                 result.Add(type);
         }
     }
     return result.ToArray();
 }

 public static Rect GetEditorMainWindowPos()
 {
     var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
     if (containerWinType == null)
         throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
     var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
     var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
     if (showModeField == null || positionProperty == null)
         throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
     var windows = Resources.FindObjectsOfTypeAll(containerWinType);
     foreach (var win in windows)
     {
         var showmode = (int)showModeField.GetValue(win);
         if (showmode == 4) // main window
         {
             var pos = (Rect)positionProperty.GetValue(win, null);
             return pos;
         }
     }
     throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
 }

 public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
 {
     var main = GetEditorMainWindowPos();
     var pos = aWin.position;
     float w = (main.width - pos.width)*0.5f;
     float h = (main.height - pos.height)*0.5f;
     pos.x = main.x + w;
     pos.y = main.y + h;
     aWin.position = pos;
     }
 }


class DMXListItem
{
    public VRStageLighting_DMX_Static light;
    public bool foldout;
    ///////////////////////////////////////////////////////////////////////
    private bool Z_enableDMXChannels; public bool P_enableDMXChannels; 
    private int Z_dmxChannel; public int P_dmxChannel;
    private int Z_dmxUniverse; public int P_dmxUniverse;
    private bool Z_useLegacySectorMode; public bool P_useLegacySectorMode;
    private bool Z_singleChannelMode; public bool P_singleChannelMode;
    private int Z_sector; public int P_sector;
    private int Z_Channel; public int P_Channel;
    private bool Z_legacyGoboRange; public bool P_legacyGoboRange;
    private float Z_globalIntensity; public float P_globalIntensity;
    private float Z_finalIntensity; public float P_finalIntensity;
    private Color Z_lightColorTint; public Color P_lightColorTint;
    private bool Z_invertPan; public bool P_invertPan;
    private bool Z_invertTilt; public bool P_invertTilt;
    private bool Z_isUpsideDown; public bool P_isUpsideDown;
    private bool Z_enableAutoSpin; public bool P_enableAutoSpin;
    private bool Z_enableStrobe; public bool P_enableStrobe;
    private float Z_tiltOffsetBlue; public float P_tiltOffsetBlue;
    private float Z_panOffsetBlueGreen; public float P_panOffsetBlueGreen;
    private int Z_selectGOBO; public int P_selectGOBO;
    private float Z_coneWidth; public float P_coneWidth;
    private float Z_coneLength; public float P_coneLength;
    private float Z_maxConeLength; public float P_maxConeLength;
    private float Z_maxMinPan; public float P_maxMinPan;
    private float Z_maxMinTilt; public float P_maxMinTilt;
    //////////////////////////////////////////////////////////////
    

    public DMXListItem(VRStageLighting_DMX_Static light, bool foldout)
    {
        this.light = light;
        this.foldout = this.light.foldout = foldout;
        Z_enableDMXChannels = P_enableDMXChannels = this.light.enableDMXChannels;
        Z_dmxChannel = P_dmxChannel = this.light.dmxChannel;
        Z_dmxUniverse = P_dmxUniverse = this.light.dmxUniverse;
        Z_useLegacySectorMode = P_useLegacySectorMode =  this.light.useLegacySectorMode;
        Z_singleChannelMode = P_singleChannelMode = this.light.singleChannelMode;
        Z_sector = P_sector = this.light.sector;
        Z_Channel = P_Channel = this.light.Channel;
        Z_legacyGoboRange = P_legacyGoboRange = this.light.legacyGoboRange;
        Z_globalIntensity = P_globalIntensity = this.light.globalIntensity;
        Z_finalIntensity = P_finalIntensity = this.light.finalIntensity;
        Z_lightColorTint = P_lightColorTint = this.light.lightColorTint;
        Z_invertPan = P_invertPan = this.light.invertPan;
        Z_invertTilt = P_invertTilt = this.light.invertTilt;
        Z_isUpsideDown = P_isUpsideDown = this.light.isUpsideDown;
        Z_enableAutoSpin = P_enableAutoSpin = this.light.enableAutoSpin;
        Z_enableStrobe = P_enableStrobe = this.light.enableStrobe;
        Z_tiltOffsetBlue = P_tiltOffsetBlue = this.light.tiltOffsetBlue;
        Z_panOffsetBlueGreen = P_panOffsetBlueGreen = this.light.panOffsetBlueGreen;
        Z_selectGOBO = P_selectGOBO = this.light.selectGOBO;
        Z_coneWidth = P_coneWidth = this.light.coneWidth;
        Z_coneLength = P_coneLength = this.light.coneLength;
        Z_maxConeLength = P_maxConeLength = this.light.maxConeLength;
        Z_maxMinPan = P_maxMinPan = this.light.maxMinPan;
        Z_maxMinTilt = P_maxMinTilt = this.light.maxMinTilt;
    }

    public void ResetChanges(bool closeMenus)
    {
        if(closeMenus)
        {
            var so = new SerializedObject(light);
            so.FindProperty("foldout").boolValue = false;
            so.ApplyModifiedProperties();
        }

        light.UpdateProxy();
        light.enableDMXChannels = P_enableDMXChannels = Z_enableDMXChannels;
        light.dmxChannel = P_dmxChannel = Z_dmxChannel;
        light.dmxUniverse = P_dmxUniverse = Z_dmxUniverse;
        light.useLegacySectorMode = P_useLegacySectorMode = Z_useLegacySectorMode;
        light.singleChannelMode = P_singleChannelMode = Z_singleChannelMode;
        light.sector = P_sector = Z_sector;
        light.Channel = P_Channel = Z_Channel;
        light.legacyGoboRange = P_legacyGoboRange = Z_legacyGoboRange;
        light.globalIntensity = P_globalIntensity = Z_globalIntensity;
        light.finalIntensity = P_finalIntensity = Z_finalIntensity;
        light.lightColorTint = P_lightColorTint = Z_lightColorTint;
        light.invertPan = P_invertPan = Z_invertPan;
        light.invertTilt = P_invertTilt = Z_invertTilt;
        light.isUpsideDown = P_isUpsideDown = Z_isUpsideDown;
        light.enableAutoSpin = P_enableAutoSpin = Z_enableAutoSpin;
        light.enableStrobe = P_enableStrobe = Z_enableStrobe;
        light.tiltOffsetBlue = P_tiltOffsetBlue = Z_tiltOffsetBlue;
        light.panOffsetBlueGreen = P_panOffsetBlueGreen = Z_panOffsetBlueGreen;
        light.selectGOBO = P_selectGOBO = Z_selectGOBO;
        light.coneWidth = P_coneWidth = Z_coneWidth;
        light.coneLength = P_coneLength = Z_coneLength;
        light.maxConeLength = P_maxConeLength = Z_maxConeLength;
        light.maxMinPan = P_maxMinPan = Z_maxMinPan;
        light.maxMinTilt = P_maxMinTilt = Z_maxMinTilt; 
        light.foldout = false;
        light.ApplyProxyModifications();
    }
    public void ApplyChanges()
    {
      //  Undo.RecordObject(light, "Undo Apply Changes");
      //  PrefabUtility.RecordPrefabInstancePropertyModifications(light);

        var so = new SerializedObject(light);
        so.FindProperty("enableDMXChannels").boolValue = P_enableDMXChannels;
        so.FindProperty("dmxChannel").intValue = P_dmxChannel;
        so.FindProperty("useLegacySectorMode").boolValue = P_useLegacySectorMode;
        so.FindProperty("singleChannelMode").boolValue = P_singleChannelMode;
        so.FindProperty("sector").intValue = P_sector; 
        so.FindProperty("Channel").intValue = P_Channel;
        so.FindProperty("legacyGoboRange").boolValue = P_legacyGoboRange;
        so.FindProperty("globalIntensity").floatValue = P_globalIntensity;
        so.FindProperty("finalIntensity").floatValue = P_finalIntensity;
        so.FindProperty("lightColorTint").colorValue = P_lightColorTint;
        so.FindProperty("invertPan").boolValue = P_invertPan;
        so.FindProperty("invertTilt").boolValue = P_invertTilt;
        so.FindProperty("isUpsideDown").boolValue = P_isUpsideDown;
        so.FindProperty("enableAutoSpin").boolValue = P_enableAutoSpin;
        so.FindProperty("enableStrobe").boolValue = P_enableStrobe;
        so.FindProperty("tiltOffsetBlue").floatValue = P_tiltOffsetBlue;
        so.FindProperty("panOffsetBlueGreen").floatValue = P_panOffsetBlueGreen;
        so.FindProperty("selectGOBO").intValue = P_selectGOBO;
        so.FindProperty("coneWidth").floatValue = P_coneWidth;
        so.FindProperty("coneLength").floatValue = P_coneLength;
        so.FindProperty("maxConeLength").floatValue = P_maxConeLength;
        so.FindProperty("maxMinPan").floatValue = P_maxMinPan;
        so.FindProperty("maxMinTilt").floatValue = P_maxMinTilt;
        so.FindProperty("foldout").boolValue = foldout;
        so.ApplyModifiedProperties();
        

        light.UpdateProxy();
        light.enableDMXChannels = Z_enableDMXChannels = P_enableDMXChannels;
        light.dmxChannel = Z_dmxChannel = P_dmxChannel;
        light.dmxUniverse = Z_dmxUniverse = P_dmxUniverse;
        light.useLegacySectorMode = Z_useLegacySectorMode = P_useLegacySectorMode;
        light.singleChannelMode = Z_singleChannelMode = P_singleChannelMode;
        light.sector = Z_sector = P_sector;
        light.Channel = Z_Channel = P_Channel;
        light.legacyGoboRange = Z_legacyGoboRange = P_legacyGoboRange;
        light.globalIntensity = Z_globalIntensity = P_globalIntensity;
        light.finalIntensity = Z_finalIntensity = P_finalIntensity;
        light.lightColorTint = Z_lightColorTint = P_lightColorTint;
        light.invertPan = Z_invertPan = P_invertPan;
        light.invertTilt = Z_invertTilt = P_invertTilt;
        light.isUpsideDown = Z_isUpsideDown = P_isUpsideDown;
        light.enableAutoSpin = Z_enableAutoSpin = P_enableAutoSpin;
        light.enableStrobe = Z_enableStrobe = P_enableStrobe;
        light.tiltOffsetBlue = Z_tiltOffsetBlue = P_tiltOffsetBlue;
        light.panOffsetBlueGreen = Z_panOffsetBlueGreen = P_panOffsetBlueGreen;
        light.selectGOBO = Z_selectGOBO = P_selectGOBO;
        light.coneWidth = Z_coneWidth = P_coneWidth;
        light.coneLength = Z_coneLength = P_coneLength;
        light.maxConeLength = Z_maxConeLength = P_maxConeLength;
        light.maxMinPan = Z_maxMinPan = P_maxMinPan;
        light.maxMinTilt = Z_maxMinTilt = P_maxMinTilt; 
        light.foldout = foldout;
        light.ApplyProxyModifications();
    }
}
class AudioLinkListItem
{
    public VRStageLighting_AudioLink_Static light;
    public bool foldout;
    public bool isLaser;
    public VRStageLighting_AudioLink_Laser laser;
    //////////////////////////////////////////////////////////////////////////
    private bool Z_enableAudioLink; public bool P_enableAudioLink;
    private int Z_band; public int P_band;
    private int Z_delay; public int P_delay;
    private float Z_bandMultiplier; public float P_bandMultiplier;
    private bool Z_enableColorChord; public bool P_enableColorChord;
    private float Z_globalIntensity; public float P_globalIntensity;
    private float Z_finalIntensity; public float P_finalIntensity;
    private Color Z_lightColorTint; public Color P_lightColorTint;
    private bool Z_enableColorTextureSampling; public bool P_enableColorTextureSampling;
    private Vector2 Z_textureSamplingCoordinates; public Vector2 P_textureSamplingCoordinates;
    private Transform Z_targetToFollow; public Transform P_targetToFollow;
    private float Z_spinSpeed; public float P_spinSpeed;
    private bool Z_enableAutoSpin; public bool P_enableAutoSpin;
    private int Z_selectGOBO; public int P_selectGOBO;
    private float Z_coneWidth; public float P_coneWidth;
    private float Z_coneLength; public float P_coneLength;
    private float Z_maxConeLength; public float P_maxConeLength;
    ///////////////////////////////////////////////////////////////////////
    private float Z_coneFlatness; public float P_coneFlatness;
    private float Z_coneXRotation; public float P_coneXRotation;
    private float Z_coneYRotation; public float P_coneYRotation;
    private float Z_coneZRotation; public float P_coneZRotation;
    private int Z_laserCount; public int P_laserCount;
    private float Z_laserScroll; public float P_laserScroll;
    private float Z_laserThickness; public float P_laserThickness;

    public AudioLinkListItem(VRStageLighting_AudioLink_Static light, bool foldout)
    {
        this.light = light;
        this.foldout = this.light.foldout = foldout;
        this.isLaser = false;
        Z_enableAudioLink = P_enableAudioLink = this.light.EnableAudioLink;
        Z_band = P_band = this.light.Band;
        Z_delay = P_delay = this.light.Delay;
        Z_bandMultiplier = P_bandMultiplier = this.light.BandMultiplier;
        Z_enableColorChord = P_enableColorChord = this.light.ColorChord;
        Z_globalIntensity = P_globalIntensity = this.light.GlobalIntensity;
        Z_finalIntensity = P_finalIntensity = this.light.FinalIntensity;
        Z_lightColorTint = P_lightColorTint = this.light.LightColorTint;
        Z_enableColorTextureSampling = P_enableColorTextureSampling = this.light.ColorTextureSampling;
        Z_textureSamplingCoordinates = P_textureSamplingCoordinates = this.light.TextureSamplingCoordinates;
        Z_targetToFollow = P_targetToFollow = this.light.targetToFollow;
        Z_spinSpeed = P_spinSpeed = this.light.SpinSpeed;
        Z_enableAutoSpin = P_enableAutoSpin =  this.light.ProjectionSpin;
        Z_selectGOBO = P_selectGOBO = this.light.SelectGOBO;
        Z_coneWidth = P_coneWidth = this.light.ConeWidth;
        Z_coneLength = P_coneLength = this.light.ConeLength;
        Z_maxConeLength = P_maxConeLength = this.light.MaxConeLength;
        

    }
    public AudioLinkListItem(VRStageLighting_AudioLink_Laser laser, bool foldout)
    {
        this.laser = laser;
        this.foldout = this.laser.foldout = foldout;
        this.isLaser = true;
        Z_enableAudioLink = P_enableAudioLink = this.laser.EnableAudioLink;
        Z_band = P_band = this.laser.Band;
        Z_delay = P_delay = this.laser.Delay;
        Z_bandMultiplier = P_bandMultiplier = this.laser.BandMultiplier;
        Z_enableColorChord = P_enableColorChord = this.laser.ColorChord;
        Z_globalIntensity = P_globalIntensity = this.laser.GlobalIntensity;
        Z_finalIntensity = P_finalIntensity = this.laser.FinalIntensity;
        Z_lightColorTint = P_lightColorTint = this.laser.LightColorTint;
        Z_enableColorTextureSampling = P_enableColorTextureSampling = this.laser.ColorTextureSampling;
        Z_textureSamplingCoordinates = P_textureSamplingCoordinates = this.laser.TextureSamplingCoordinates;
        Z_coneFlatness = P_coneFlatness = this.laser.ConeFlatness;
        Z_coneXRotation = P_coneXRotation =  this.laser.ConeXRotation;
        Z_coneYRotation = P_coneYRotation = this.laser.ConeYRotation;
        Z_coneZRotation = P_coneZRotation = this.laser.ConeZRotation;
        Z_laserCount = P_laserCount = this.laser.LaserCount;
        Z_laserScroll = P_laserScroll = this.laser.LaserScroll;
        Z_coneWidth = P_coneWidth = this.laser.ConeWidth;
        Z_coneLength = P_coneLength = this.laser.ConeLength;
        Z_laserThickness = P_laserThickness = this.laser.LaserThickness;
    }
    public void ResetChanges(VRStageLighting_AudioLink_Static li , bool closeMenus)
    {
        if(closeMenus)
        {
            var so = new SerializedObject(light);
            so.FindProperty("foldout").boolValue = false;
            so.ApplyModifiedProperties();
        }

        light.UpdateProxy();
        light.EnableAudioLink = P_enableAudioLink = Z_enableAudioLink;
        light.Band = P_band = Z_band;
        light.Delay = P_delay = Z_delay;
        light.BandMultiplier = P_bandMultiplier = Z_bandMultiplier;
        light.ColorChord = P_enableColorChord = Z_enableColorChord;
        light.GlobalIntensity = P_globalIntensity = Z_globalIntensity;
        light.FinalIntensity = P_finalIntensity = Z_finalIntensity;
        light.LightColorTint = P_lightColorTint = Z_lightColorTint;
        light.ColorTextureSampling = P_enableColorTextureSampling = Z_enableColorTextureSampling;
        light.TextureSamplingCoordinates = P_textureSamplingCoordinates = Z_textureSamplingCoordinates;
        light.targetToFollow = P_targetToFollow = Z_targetToFollow;
        light.ProjectionSpin = P_enableAutoSpin = Z_enableAutoSpin;
        light.SpinSpeed = P_spinSpeed = Z_spinSpeed;
        light.SelectGOBO = P_selectGOBO = Z_selectGOBO;
        light.ConeWidth = P_coneWidth = Z_coneWidth;
        light.ConeLength = P_coneLength = Z_coneLength;
        light.MaxConeLength = P_maxConeLength = Z_maxConeLength;
        light.foldout = false;
        light.ApplyProxyModifications();
    }
    public void ResetChanges(VRStageLighting_AudioLink_Laser li , bool closeMenus)
    {
        if(closeMenus)
        {
            var so = new SerializedObject(laser);
            so.FindProperty("foldout").boolValue = false;
            so.ApplyModifiedProperties();
        }

        laser.UpdateProxy();
        laser.EnableAudioLink = P_enableAudioLink = Z_enableAudioLink;
        laser.Band = P_band = Z_band;
        laser.Delay = P_delay = Z_delay;
        laser.BandMultiplier = P_bandMultiplier = Z_bandMultiplier;
        laser.ColorChord = P_enableColorChord = Z_enableColorChord;
        laser.GlobalIntensity = P_globalIntensity = Z_globalIntensity;
        laser.FinalIntensity = P_finalIntensity = Z_finalIntensity;
        laser.LightColorTint = P_lightColorTint = Z_lightColorTint;
        laser.ColorTextureSampling = P_enableColorTextureSampling = Z_enableColorTextureSampling;
        laser.TextureSamplingCoordinates = P_textureSamplingCoordinates = Z_textureSamplingCoordinates;
        laser.ConeXRotation = P_coneXRotation = Z_coneXRotation;
        laser.ConeYRotation = P_coneYRotation = Z_coneYRotation;
        laser.ConeZRotation = P_coneZRotation = Z_coneZRotation;
        laser.LaserCount = P_laserCount = Z_laserCount;
        laser.ConeWidth = P_coneWidth = Z_coneWidth;
        laser.ConeLength = P_coneLength = Z_coneLength;
        laser.LaserScroll = P_laserScroll = Z_laserScroll;
        laser.foldout = false;
        laser.ApplyProxyModifications();
    }

    public void ApplyChanges(VRStageLighting_AudioLink_Static li)
    {
      //  Undo.RecordObject(light, "Undo Apply Changes");
      //  PrefabUtility.RecordPrefabInstancePropertyModifications(light);

        var so = new SerializedObject(light);
        so.FindProperty("enableAudioLink").boolValue = P_enableAudioLink;
        so.FindProperty("band").intValue = P_band;
        so.FindProperty("delay").intValue = P_delay;
        so.FindProperty("bandMultiplier").floatValue = P_bandMultiplier;
        so.FindProperty("enableColorChord").boolValue = P_enableColorChord; 
        so.FindProperty("globalIntensity").floatValue = P_globalIntensity;
        so.FindProperty("finalIntensity").floatValue = P_finalIntensity;
        so.FindProperty("lightColorTint").colorValue = P_lightColorTint;
        so.FindProperty("enableColorTextureSampling").boolValue = P_enableColorTextureSampling;
        so.FindProperty("textureSamplingCoordinates").vector2Value = P_textureSamplingCoordinates;
        so.FindProperty("targetToFollow").objectReferenceValue = P_targetToFollow;
        so.FindProperty("enableAutoSpin").boolValue = P_enableAutoSpin;
        so.FindProperty("spinSpeed").floatValue = P_spinSpeed;
        so.FindProperty("selectGOBO").intValue = P_selectGOBO;
        so.FindProperty("coneWidth").floatValue = P_coneWidth;
        so.FindProperty("coneLength").floatValue = P_coneLength;
        so.FindProperty("maxConeLength").floatValue = P_maxConeLength;
        so.FindProperty("foldout").boolValue = foldout;
        so.ApplyModifiedProperties();

        //var soTarget = new SerializedObject(light.targetToFollow.gameObject);

    

        light.UpdateProxy();
        light.EnableAudioLink = Z_enableAudioLink = P_enableAudioLink;
        light.Band = Z_band = P_band;
        light.Delay = Z_delay = P_delay;
        light.BandMultiplier = Z_bandMultiplier = P_bandMultiplier;
        light.ColorChord = Z_enableColorChord = P_enableColorChord;
        light.GlobalIntensity = Z_globalIntensity = P_globalIntensity;
        light.FinalIntensity = Z_finalIntensity = P_finalIntensity;
        light.LightColorTint = Z_lightColorTint = P_lightColorTint;
        light.ColorTextureSampling = Z_enableColorTextureSampling = P_enableColorTextureSampling;
        light.TextureSamplingCoordinates = Z_textureSamplingCoordinates = P_textureSamplingCoordinates;
        light.targetToFollow = Z_targetToFollow = P_targetToFollow;
        light.ProjectionSpin = Z_enableAutoSpin = P_enableAutoSpin;
        light.SpinSpeed = Z_spinSpeed = P_spinSpeed;
        light.SelectGOBO = Z_selectGOBO = P_selectGOBO;
        light.ConeWidth = Z_coneWidth = P_coneWidth;
        light.ConeLength = Z_coneLength = P_coneLength;
        light.MaxConeLength = Z_maxConeLength = P_maxConeLength;
        light.foldout = foldout;
        
        light.ApplyProxyModifications();
    }

    public void ApplyChanges(VRStageLighting_AudioLink_Laser li)
    {
      //  Undo.RecordObject(light, "Undo Apply Changes");
      //  PrefabUtility.RecordPrefabInstancePropertyModifications(light);

        var so = new SerializedObject(laser);
        so.FindProperty("enableAudioLink").boolValue = P_enableAudioLink;
        so.FindProperty("band").intValue = P_band;
        so.FindProperty("delay").intValue = P_delay;
        so.FindProperty("bandMultiplier").floatValue = P_bandMultiplier;
        so.FindProperty("enableColorChord").boolValue = P_enableColorChord; 
        so.FindProperty("globalIntensity").floatValue = P_globalIntensity;
        so.FindProperty("finalIntensity").floatValue = P_finalIntensity;
        so.FindProperty("lightColorTint").colorValue = P_lightColorTint;
        so.FindProperty("enableColorTextureSampling").boolValue = P_enableColorTextureSampling;
        so.FindProperty("textureSamplingCoordinates").vector2Value = P_textureSamplingCoordinates;

        so.FindProperty("coneXRotation").floatValue = P_coneXRotation;
        so.FindProperty("coneYRotation").floatValue = P_coneYRotation;
        so.FindProperty("coneZRotation").floatValue = P_coneZRotation;

        so.FindProperty("coneWidth").floatValue = P_coneWidth;
        so.FindProperty("coneLength").floatValue = P_coneLength;
        so.FindProperty("laserCount").intValue = P_laserCount;
        so.FindProperty("laserScroll").floatValue = P_laserScroll;
        so.FindProperty("laserThickness").floatValue = P_laserThickness;
        so.FindProperty("foldout").boolValue = foldout;
        so.ApplyModifiedProperties();

        //var soTarget = new SerializedObject(light.targetToFollow.gameObject);

    

        laser.UpdateProxy();
        laser.EnableAudioLink = Z_enableAudioLink = P_enableAudioLink;
        laser.Band = Z_band = P_band;
        laser.Delay = Z_delay = P_delay;
        laser.BandMultiplier = Z_bandMultiplier = P_bandMultiplier;
        laser.ColorChord = Z_enableColorChord = P_enableColorChord;
        laser.GlobalIntensity = Z_globalIntensity = P_globalIntensity;
        laser.FinalIntensity = Z_finalIntensity = P_finalIntensity;
        laser.LightColorTint = Z_lightColorTint = P_lightColorTint;
        laser.ColorTextureSampling = Z_enableColorTextureSampling = P_enableColorTextureSampling;
        laser.TextureSamplingCoordinates = Z_textureSamplingCoordinates = P_textureSamplingCoordinates;

        laser.ConeXRotation = Z_coneXRotation = P_coneXRotation;
        laser.ConeYRotation = Z_coneYRotation = P_coneYRotation;
        laser.ConeZRotation = Z_coneZRotation = P_coneZRotation;

        laser.LaserCount = Z_laserCount = P_laserCount;
        laser.ConeWidth = Z_coneWidth = P_coneWidth;
        laser.ConeLength = Z_coneLength = P_coneLength;
        laser.LaserScroll = Z_laserScroll = P_laserScroll;
        laser.LaserThickness = Z_laserThickness = P_laserThickness;
        laser.foldout = foldout;
        laser.ApplyProxyModifications();
    }


}
// class AudioLinkLaserListItem
// {
//     public VRStageLighting_AudioLink_Laser laser;
//     public bool foldout;

//     public AudioLinkLaserListItem(VRStageLighting_AudioLink_Laser laser, bool foldout)
//     {
//         this.laser = laser;
//         this.foldout = foldout;
//     }
// }
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
class VRSL_ManagerWindow : EditorWindow {

    static uDesktopDuplication.Texture runTimeScreen;
    static float panRangeOff = 180f;
    static float tiltRangeOff = -180f;
    public static Texture logo, github, twitter, discord;
    public bool legacyFixtures;
    public static string ver = "VR Stage Lighting ver:" + "<b><color=#6a15ce> 2.0</color></b>";

    public static bool hasLocalPanel, hasDepthLight;
    private static VRSL_LocalUIControlPanel panel;
    private static Light depthLight;
    //private static Texture2D redTex, greenTex, lightRedTex, lightGreenTex;

    //private static List<VRStageLighting_DMX_Static> dmxLights = new List<VRStageLighting_DMX_Static>();
    //private static List<VRStageLighting_AudioLink_Static> audioLinkLights = new List<VRStageLighting_AudioLink_Static>();
    //private static List<VRStageLighting_AudioLink_Laser> audioLinkLasers = new List<VRStageLighting_AudioLink_Laser>();
    private static List<DMXListItem> dmxLights = new List<DMXListItem>();
    private static List<AudioLinkListItem> audioLinkLights = new List<AudioLinkListItem>();
    //private static List<AudioLinkLaserListItem> audioLinkLasers = new List<AudioLinkLaserListItem>();
    private static List<GameObject> sceneObjects = new List<GameObject>();
    private static List<DMXListItem>[] universes =  new List<DMXListItem>[3];
    private static bool[] universeFold = new bool[3];
    private static bool universeFourFold;
    private static bool[] bandFold = new bool[4];
    static GUIContent colorLabel;
    private string[] dmxModes = new string[]{"Horizontal", "Vertical", "Legacy"};
    private string [] dmxGizmoInfo = new string[]{"None", "Channel Only", "Universe + Channel"};
    private static UnityEngine.Object controlPanelUiPrefab, directionalLightPrefab, uDesktopHorizontalPrefab, uDesktopVerticalPrefab, uDesktopLegacyPrefab, uVidHorizontalPrefab, uVidVerticalPrefab, uVidLegacyPrefab,
    audioLinkPrefab, audioLinkControllerPrefab, standardAudioLinkControllerPrefab;
    private static bool dmxSpawnsFoldout, audioLinkSpawnsFoldout, mainOptionsFoldout;
    Vector2 dmxScrollPos, audioLinkScrollPos, mainScrollPos;
    private static bool dmxHover, audioLinkHover;
    static bool hasDesktopDuplication;

    private static UnityEngine.Object spotlight_h, spotlight_v, spotlight_l, spotlight_a;
    private static UnityEngine.Object washlight_h, washlight_v, washlight_l, washlight_a;
    private static UnityEngine.Object blinder_h, blinder_v, blinder_l, blinder_a;
    private static UnityEngine.Object flasher_h, flasher_v, flasher_l, flasher_a;
    private static UnityEngine.Object parlight_h, parlight_v, parlight_l, parlight_a;
    private static UnityEngine.Object lightbar_h, lightbar_v, lightbar_l, lightbar_a;
    private static UnityEngine.Object discoball_h, discoball_v, discoball_l, discoball_a;
    private static UnityEngine.Object laser_h, laser_v, laser_l, laser_a;
    // private float panRangeTarget = 90f; 
    // private float tiltRangeTarget = -90f;


    [MenuItem("VRSL/Control Panel")]
    static void ShowWindow() {
        EditorWindow window = GetWindow<VRSL_ManagerWindow>();
        window.titleContent = new GUIContent("VRSL Control Panel");
        if(!Application.isPlaying)
        {
            sceneObjects = GetAllObjectsOnlyInScene();
            CheckForLocalPanel();
            CheckForDepthLight();
            if(hasLocalPanel)
            {
                if(panel.isUsingDMX)
                    GetDMXLights(true);
                if(panel.isUsingAudioLink)
                    GetAudioLinkLights(true);
                    
            }    
            LoadPrefabs();
            //Debug.Log("VRSL Control Panel: Initializing!");
            ApplyChangesToFixtures(true, true, false);
        }
        else
        {
            CheckForDesktopScreen();
        }



        window.minSize = new Vector2(925f, 700f);
       // window.position = Rect.zero;
        Extensions.CenterOnMainWin(window);
        window.Show();
        //EditorApplication.playModeStateChanged += LogPlayModeState;
        

    }


    // static VRSL_ManagerWindow()
    // {
    //     base.
       
    // }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
       // Debug.Log(state);
        if(state == PlayModeStateChange.EnteredPlayMode)
        {
            CheckForDesktopScreen();
        }

    }

    static void CheckForDesktopScreen()
    {
        // if(sceneObjects != null)
        // {
        //     return;
        // }
        sceneObjects = GetAllObjectsOnlyInScene();
        foreach(GameObject go in sceneObjects)
        {   
            runTimeScreen = go.GetComponent<uDesktopDuplication.Texture>();
            if(runTimeScreen != null)
            {
                break;
            }
        }
        if(runTimeScreen != null)
        {
            Debug.Log("Found uDesktopDuplication Screen!");
            hasDesktopDuplication = true;
        }
        else
        {
            Debug.Log("Could not find uDesktopDuplication Screen!");
            hasDesktopDuplication = false;
        }
    }

    static bool LoadPrefabs()
    {
        bool result = true;
        string controlPanelPath = "Assets/VR-Stage-Lighting/Prefabs/VRSL-LocalUIControlPanel.prefab";
        string directionalLightPath = "Assets/VR-Stage-Lighting/Prefabs/Directional Light (For Depth).prefab";
        string udeskHorizontalPath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Horizontal Mode/DMX Reader Screens/VRSL-DMX-uDesktopDuplicationReaderScreen-Horizontal.prefab";
        string udeskVerticalPath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Vertical Mode/DMX Reader Screens/VRSL-DMX-uDesktopDuplicationReaderScreen-Vertical.prefab";
        string udeskLegacyPath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Legacy Mode/DMX Reader Screens/VRSL-DMX-uDesktopDuplicationReader-Legacy.prefab";
        string vidHorizontalPath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Horizontal Mode/DMX Reader Screens/VRSL-DMX-USharpVideoReaderScreen-Horizontal.prefab";
        string vidVerticalPath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Vertical Mode/DMX Reader Screens/VRSL-DMX-USharpVideoReaderScreen-Vertical.prefab";
        string vidLegacyPath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Legacy Mode/DMX Reader Screens/VRSL-DMX-USharpVideoReaderScreen-Legacy.prefab";
        string audioLinkPath = "Assets/AudioLink/Audiolink.prefab";
        string audioLinkControllerPath = "Assets/VR-Stage-Lighting/Prefabs/AudioLink/VRSL-AudioLinkControllerWithSmoothing/AudioLinkController-WithVRSLSmoothing.prefab";
        string standardAudioLinkControllerPath = "Assets/AudioLink/AudioLinkController.prefab";
        controlPanelUiPrefab = AssetDatabase.LoadAssetAtPath(controlPanelPath, typeof(GameObject));
        directionalLightPrefab = AssetDatabase.LoadAssetAtPath(directionalLightPath, typeof(GameObject));
        uDesktopHorizontalPrefab = AssetDatabase.LoadAssetAtPath(udeskHorizontalPath, typeof(GameObject));
        uDesktopVerticalPrefab = AssetDatabase.LoadAssetAtPath(udeskVerticalPath, typeof(GameObject));
        uDesktopLegacyPrefab = AssetDatabase.LoadAssetAtPath(udeskLegacyPath, typeof(GameObject));
        uVidHorizontalPrefab = AssetDatabase.LoadAssetAtPath(vidHorizontalPath, typeof(GameObject));
        uVidVerticalPrefab = AssetDatabase.LoadAssetAtPath(vidVerticalPath, typeof(GameObject));
        uVidLegacyPrefab = AssetDatabase.LoadAssetAtPath(vidLegacyPath, typeof(GameObject));
        audioLinkPrefab = AssetDatabase.LoadAssetAtPath(audioLinkPath, typeof(GameObject));
        audioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(audioLinkControllerPath, typeof(GameObject));
        standardAudioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(standardAudioLinkControllerPath, typeof(GameObject));
        if(controlPanelUiPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + controlPanelPath);
            result = false;
        }
        if(directionalLightPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + directionalLightPath);
            result = false;
        }
        if(uDesktopHorizontalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load  " + udeskHorizontalPath);
            result = false;
        }
        if(uDesktopVerticalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + udeskVerticalPath);
            result = false;
        }
        if(uDesktopLegacyPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + udeskLegacyPath);
            result = false;
        }
        if(uVidHorizontalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + vidHorizontalPath);
            result = false;
        }
        if(uVidVerticalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + vidVerticalPath);
            result = false;
        }
        if(uVidLegacyPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + vidLegacyPath);
            result = false;
        }
        if(audioLinkPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + audioLinkPath);
            result = false;
        }
        if(audioLinkControllerPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + audioLinkControllerPath);
            result = false;
        }
        if(standardAudioLinkControllerPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + standardAudioLinkControllerPath);
            result = false;
        }
        return result;
    }


    private static void CheckForLocalPanel()
    {
        hasLocalPanel = false;
        panel = null;
        colorLabel = new GUIContent();
        colorLabel.text = "Emission Color";
     //   List<GameObject> sceneObjects = GetAllObjectsOnlyInScene();
        foreach (GameObject go in sceneObjects)
        {
           // if(go.name = "")
           panel =  go.GetUdonSharpComponent<VRSL_LocalUIControlPanel>();
           if(panel != null)
           {
               hasLocalPanel = true;
               break;
           }
        }
        return;
    }

    private static void CheckForDepthLight()
    {
        hasDepthLight = false;
        depthLight = null;
      //  List<GameObject> sceneObjects = GetAllObjectsOnlyInScene();
        foreach (GameObject go in sceneObjects)
        {
           if(go.GetComponent<Light>() != null)
           {
               depthLight = go.GetComponent<Light>();
               if(depthLight.type == LightType.Directional && depthLight.shadows != LightShadows.None)
               {
                   hasDepthLight = true;
                   break;
               }
               else
               {
                   depthLight = null;
               }
           }
        }

    }

    private static void GetDMXLights(bool updateBools)
    {
        if(updateBools)
        {
            dmxLights.Clear();
            foreach(GameObject go in sceneObjects)
            {
                VRStageLighting_DMX_Static lightScript = go.GetUdonSharpComponent<VRStageLighting_DMX_Static>();
                if(lightScript != null)
                {
                    //dmxLights.Add(go.GetUdonSharpComponent<VRStageLighting_DMX_Static>());
                    dmxLights.Add(
                        new DMXListItem(lightScript, lightScript.foldout)
                    );

                }
            }
            dmxLights.Sort((x, y) => x.light._GetDMXChannel().CompareTo(y.light._GetDMXChannel()));
        }
        else
        {
            dmxLights.Sort((x, y) => x.light._GetDMXChannel().CompareTo(y.light._GetDMXChannel()));
        }
        for(int i = 0; i < universes.Length; i++)
        {
            universes[i] = new List<DMXListItem>();
            foreach(DMXListItem fixture in dmxLights)
            {
                if(fixture.light._GetUniverse() == i+1)
                {
                    universes[i].Add(fixture);
                }
            }
        }
    }
    private static void GetAudioLinkLights(bool updateBools)
    {
        if(updateBools)
        {
            audioLinkLights.Clear();
            foreach(GameObject go in sceneObjects)
            {
                VRStageLighting_AudioLink_Static audioScript = go.GetUdonSharpComponent<VRStageLighting_AudioLink_Static>();
                if(audioScript != null)
                {
                    audioLinkLights.Add(
                        new AudioLinkListItem(audioScript, audioScript.foldout)
                    );
                    continue;
                }
                VRStageLighting_AudioLink_Laser laserScript = go.GetUdonSharpComponent<VRStageLighting_AudioLink_Laser>();
                if(laserScript != null)
                {
                    audioLinkLights.Add(
                        new AudioLinkListItem(laserScript, laserScript.foldout)
                    );
                    continue;
                }
            }
        }
        //GetAudioLinkLasers(updateBools);
    }
    // private static void GetAudioLinkLasers(bool updateBools)
    // {
    //     if(updateBools)
    //     {
    //         audioLinkLasers.Clear();
    //         foreach(GameObject go in sceneObjects)
    //         {
    //             VRStageLighting_AudioLink_Laser laserScript = go.GetUdonSharpComponent<VRStageLighting_AudioLink_Laser>();
    //             if(laserScript != null)
    //             {
    //                 audioLinkLasers.Add(
    //                     new AudioLinkLaserListItem(laserScript, false)
    //                 );
    //             }
    //         }
    //     }
    // }

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

    private static void LoadLogo()
    {
        logo = Resources.Load("VRStageLighting-Logo") as Texture;
        github = Resources.Load("github-logo") as Texture;
        discord = Resources.Load("discord-logo") as Texture;
        twitter = Resources.Load("twitter-logo") as Texture;
        // redTex = Resources.Load("redTex") as Texture2D;
        // lightRedTex = Resources.Load("light_redTex") as Texture2D;
        // greenTex = Resources.Load("greenTex") as Texture2D;
        // lightGreenTex = Resources.Load("light_greenTex") as Texture2D;
    }
    private static void DrawLogo()
    {
        ///GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
        // GUILayout.FlexibleSpace();
        //GUI.DrawTexture(pos,logo,ScaleMode.ScaleToFit);
        //EditorGUI.DrawPreviewTexture(new Rect(0,0,400,150), logo);.
        float size = 30f;
        float space = 15f;
        float outx = 10f;
      //  EditorGUILayout.Space(10f);
      //  Color oldColor = GUI.backgroundColor;
        LoadLogo();
       // GUI.backgroundColor = Color.black;
        // Vector2 contentOffset = new Vector2(0f, -2f);
        // GUIStyle style = new GUIStyle();
        // style.fixedHeight = 100;

        // style.contentOffset = contentOffset;
        // style.alignment = TextAnchor.MiddleCenter;

        // var rect = GUILayoutUtility.GetRect(300f, 90f, style);
        // GUILayout.BeginArea(rect, logo);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(outx);
        EditorGUILayout.BeginVertical();
       // GUILayout.FlexibleSpace();
       GUILayout.Space(space);
        if(GUILayout.Button(new GUIContent(discord, "Join the VRSL Discord!"), GUILayout.MaxWidth(size), GUILayout.MaxHeight(size)))
        {
            Application.OpenURL("http://vrsl.dev");
        }
        //GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Space(space);
        if(GUILayout.Button(new GUIContent(twitter, "Follow AcChosen on Twitter for all the latest VRSL updates!"), GUILayout.MaxWidth(size), GUILayout.MaxHeight(size)))
        {
            Application.OpenURL("https://twitter.com/ac_chosen");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Space(space);
        if(GUILayout.Button(new GUIContent(github, "Visit the official VRSL GitHub Repository"), GUILayout.MaxWidth(size), GUILayout.MaxHeight(size)))
        {
            Application.OpenURL("https://github.com/AcChosen/VR-Stage-Lighting");
        }
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        //GUILayout.Space(200f);
        Color oldColor = GUI.backgroundColor;
        LoadLogo();
        GUI.backgroundColor = Color.black;
        Vector2 contentOffset = new Vector2(0f, -2f);
        GUIStyle style = new GUIStyle();
        style.fixedHeight = 100;

        style.contentOffset = contentOffset;
        //style.alignment = TextAnchor.MiddleCenter;

        var rect = GUILayoutUtility.GetRect(200f, 90f, style);
        
        GUI.Box(rect, logo,style);
        GUI.backgroundColor = oldColor;
        GUILayout.Space(45f + outx);
        GUILayout.FlexibleSpace();
        //GUILayout.EndArea();
        EditorGUILayout.EndHorizontal();

        // Color oldColor = GUI.backgroundColor;
        // LoadLogo();
        // GUI.backgroundColor = Color.black;
        // Vector2 contentOffset = new Vector2(0f, -2f);
        // GUIStyle style = new GUIStyle();
        // style.fixedHeight = 100;

        // style.contentOffset = contentOffset;
        // style.alignment = TextAnchor.MiddleCenter;

        // var rect = GUILayoutUtility.GetRect(300f, 90f, style);
        
        // GUI.Box(rect, logo,style);
        // GUI.backgroundColor = oldColor;
        
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
        public static GUIStyle SectionLabel()
    {
        GUIStyle g = new GUIStyle();
        g.fontSize = 15;
        g.fontStyle = FontStyle.Bold;
        g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleCenter;
        return g;
    }
        static GUIStyle SecLabel()
        {
            GUIStyle g = new GUIStyle();
            g.fontSize = 15;
            g.fontStyle = FontStyle.Italic;
            g.normal.textColor = Color.white;
            g.alignment = TextAnchor.MiddleCenter;
            return g; 
        }
        public static GUIStyle WarningLabel()
    {
        GUIStyle g = new GUIStyle();
        g.fontSize = 16;
        g.fontStyle = FontStyle.Italic;
        g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleCenter;
        g.wordWrap = true;
        return g;
    }
        public static GUIStyle BigButton()
        {
            GUIStyle g = new GUIStyle("Button");
            g.fixedHeight = 26;
            g.fontSize = 17;
            return g;
        }
        public static GUIStyle HalfButton()
        {
            GUIStyle g = new GUIStyle("Button");
            g.fixedHeight = 20;
            g.fontSize = 12;
            return g;
        }
        public static GUIStyle NormalButton()
        {
            GUIStyle g = new GUIStyle("Button");
            g.fixedHeight = 21;
            g.fontSize = 14;
            g.richText = true;
            return g;
        }
        public static GUIStyle NormalButtonToggle(bool enabled, bool isHover)
        {
            GUIStyle g = new GUIStyle("Button");
            g.fixedHeight = 21;
            g.fontSize = 14;
            g.richText = true;
            //bool isHover = false;

            
            if(enabled)
            {
                if(isHover){
                    GUI.backgroundColor = new Color(0.55f, 0.25f, 1f, 1f);
                }
                else{
                    GUI.backgroundColor = new Color(0.45f, 0.25f, 0.75f, 1f);
                }
                // g.normal.background = greenTex;
                // g.hover.background = lightGreenTex;
            }
            else
            {
                if(isHover){
                    GUI.backgroundColor = new Color(0.75f, 0.25f, 0.3f, 1f);
                }
                else{
                    GUI.backgroundColor = new Color(0.5f, 0.25f, 0.2f, 1f);
                }
                // g.normal.background = redTex;
                // g.hover.background = lightRedTex;
            }
            return g;
        }

    public static GUIStyle Title1()
    {
        GUIStyle g = new GUIStyle();
        g.fontSize = 26;
        g.fixedHeight = 45;
        //g.fontStyle = FontStyle.Bold;
        g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleCenter;
        return g;
    }
    public static GUIStyle Title2()
    {
        GUIStyle g = new GUIStyle();
        g.fontSize = 24;
        g.fixedHeight = 35;
        //g.fontStyle = FontStyle.Bold;
        g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleCenter;
        return g;
    }
    public static GUIStyle Title3()
    {
        GUIStyle g = new GUIStyle(EditorStyles.label);
        g.fontSize = 12;
       // g.fixedHeight = 35;
      //  g.fontStyle = FontStyle.Italic;
        g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleCenter;
        return g;
    }
    public static GUIStyle Title1Foldout()
    {
        GUIStyle g = new GUIStyle(EditorStyles.foldoutHeader);
         g.fontSize = 20;
        // g.fontStyle = FontStyle.Bold;
        // g.normal.textColor = Color.white;
         g.alignment = TextAnchor.MiddleCenter;
         g.fixedHeight = 45;
         g.fontStyle = FontStyle.Bold;
        g.stretchWidth = true;
        
        return g;
    }
    public static GUIStyle Title2Foldout()
    {
        GUIStyle g = new GUIStyle(EditorStyles.foldoutHeader);
         g.fontSize = 14;
         //g.fontStyle = FontStyle.Italic;
         //g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleCenter;
        g.fixedHeight = 20;
         //g.fontStyle = FontStyle.Bold;
        g.stretchWidth = true;
        
        return g;
    }
    public static GUIStyle SelectionFoldout()
    {
        GUIStyle g = new GUIStyle(EditorStyles.foldoutHeader);
        g.fontSize = 13;
        g.fixedHeight = 30;
        g.alignment = TextAnchor.MiddleLeft;
        g.fontStyle = FontStyle.Normal;
        g.richText = true;
        g.stretchWidth = true;
        return g;
    }

    static void SetRangeButton(float range, bool isPan)
    {
        if(GUILayout.Button(Label(range.ToString(), "Set the field to " + range.ToString() + "°!"), GUILayout.MaxWidth(50f)))
        {
            //target.UpdateProxy();
            if(isPan)
            {
                //target.panRangeTarget = range;
                panRangeOff = range;
            }
            else
            {
                //target.tiltRangeTarget = range;
                tiltRangeOff = range;
            }
            //target.ApplyProxyModifications();
        }
    }


    void OnEnable() 
    {
     EditorApplication.hierarchyChanged += HierarchyChanged;
     EditorApplication.playModeStateChanged += LogPlayModeState;
     SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDisable( )
    {
        EditorApplication.hierarchyChanged -= HierarchyChanged;
        EditorApplication.playModeStateChanged -= LogPlayModeState;
        if((panel != null))
        {
            if(panel.fixtureGizmos == 0)
                SceneView.duringSceneGui -= this.OnSceneGUI;
        }
    }
    private void HierarchyChanged()
    {
        ResetItems(true);
    }
     static void ResetItems(bool updateBools)
     {
        sceneObjects = GetAllObjectsOnlyInScene();
        CheckForLocalPanel();
        CheckForDepthLight();
        if(hasLocalPanel)
        {
            if(panel.isUsingDMX)
                GetDMXLights(updateBools);
            if(panel.isUsingAudioLink)
                GetAudioLinkLights(updateBools);
        }
     }

    static void MassApplyPTRange(bool isPan)
    {
        for(int i = 0; i < universes.Length; i++)
        {
            if(universes[i] == null)
            {
                continue;
            }
            foreach(DMXListItem fixture in universes[i])
            {
                if(fixture == null)
                {
                    continue;
                }
                if(fixture.light == null)
                {
                    continue;
                }
                if(isPan)
                {
                    fixture.P_maxMinPan = panRangeOff;
                }
                else
                {
                    fixture.P_maxMinTilt = tiltRangeOff;
                }

            }
        }

    }

    private static void SpawnPrefabWithUndo(UnityEngine.Object obj, string undoMessage)
    {
        GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(obj as GameObject);
        Undo.RegisterCreatedObjectUndo (instance, undoMessage);
        Selection.SetActiveObjectWithContext(instance, null);       
    }

     private static void ApplyChangesToFixtures(bool acceptChanges, bool initialize, bool closeMenus)
     {
        if(hasLocalPanel)
        {
            foreach(DMXListItem fixture in dmxLights)
            {
                if(!initialize)
                {
                    if(acceptChanges)
                    {
                        fixture.ApplyChanges();
                    
                    }
                    else
                    {
                        fixture.ResetChanges(closeMenus);
                    }
                }
                fixture.light._SetProps();
                if(Application.isPlaying)
                {
                    fixture.light._UpdateInstancedProperties();
                }
                else
                {
                    fixture.light._UpdateInstancedPropertiesSansDMX();
                }
            }
            foreach(AudioLinkListItem fixture in audioLinkLights)
            {
                if(fixture.isLaser)
                {
                    if(!initialize)
                    {
                        if(acceptChanges)
                        {
                            fixture.ApplyChanges(fixture.laser);
                        
                        }
                        else
                        {
                            fixture.ResetChanges(fixture.laser, closeMenus);
                        }
                    }   
                    fixture.laser._SetProps();
                    if(Application.isPlaying)
                    {
                        fixture.laser._UpdateInstancedProperties();
                    }
                    else
                    {
                        fixture.laser._UpdateInstancedPropertiesSansAudioLink();
                    }
                }
                else
                {
                    if(!initialize)
                    {
                        if(acceptChanges)
                        {
                            fixture.ApplyChanges(fixture.light);
                        
                        }
                        else
                        {
                            fixture.ResetChanges(fixture.light, closeMenus);
                        }
                    }   
                    fixture.light._SetProps();
                    if(Application.isPlaying)
                    {
                        fixture.light._UpdateInstancedProperties();
                    }
                    else
                    {
                        fixture.light._UpdateInstancedPropertiesSansAudioLink();
                    }
                }
            }
            // foreach(AudioLinkLaserListItem fixture in audioLinkLasers)
            // {
            //     fixture.laser._SetProps();
            //     if(Application.isPlaying)
            //     {
            //         fixture.laser._UpdateInstancedProperties();
            //     }
            //     else
            //     {
            //         fixture.laser._UpdateInstancedPropertiesSansAudioLink();
            //     }
            // }
        }
     }

     static GUIContent Label(string label, string tooltip)
     {
         return new GUIContent(label, tooltip);
     }
     
    static String ToggleText(bool enable, string text)
    {
        if(enable)
        {
            return text + ": <b><i>ENABLED</i></b>"; 
        }
        else
        {
            return text + ": <b><i>DISABLED</i></b>"; 
        }
    }
    void GuiLine( int i_height = 1 )

   {
       //GUIStyle g = GUIStyle.none;
       //g.fixedHeight = 6;
       Rect rect = EditorGUILayout.GetControlRect(false, i_height);

       rect.height = i_height;

       EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

   }

   
    private void DrawProperties(SerializedProperty prop, bool drawChildren )
    {
        string lastPropPath = string.Empty;
        foreach(SerializedProperty p in prop)
        {
            if(p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUI.indentLevel++;
                DrawProperties(p, drawChildren);
                EditorGUI.indentLevel--;
            }
            else
            {
                if( !string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath))
                {
                    continue;
                } 
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    } 

    private bool LoadFixturePrefabs(int a)
    {
        bool loadSuccessful = true;

        string horizontalpath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Horizontal Mode/";
        string verticalpath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Vertical Mode/";
        string legacypath = "Assets/VR-Stage-Lighting/Prefabs/DMX/Legacy Mode/";
        string audiopath = "Assets/VR-Stage-Lighting/Prefabs/AudioLink/";

        string spot_h_path = horizontalpath + "VRSL-DMX-Mover-Spotlight-H-13CH.prefab";
        string spot_v_path = verticalpath + "VRSL-DMX-Mover-Spotlight-V-13CH.prefab";
        string spot_l_path = legacypath + "VRSL-DMX-Mover-Spotlight-L-13CH.prefab";
        string spot_a_path = audiopath + "VRSL-AudioLink-Mover-Spotlight.prefab";

        string wash_h_path = horizontalpath + "VRSL-DMX-Mover-WashLight-H-13CH.prefab";
        string wash_v_path = verticalpath + "VRSL-DMX-Mover-WashLight-V-13CH.prefab";
        string wash_l_path = legacypath + "VRSL-DMX-Mover-WashLight-L-13CH.prefab";
        string wash_a_path = audiopath + "VRSL-AudioLink-Mover-Washlight.prefab";
        string blind_h_path, blind_v_path, blind_l_path, blind_a_path;
        string par_h_path, par_v_path, par_l_path, par_a_path;
        string bar_h_path, bar_v_path, bar_l_path, bar_a_path;
        if(legacyFixtures)
        {
            blind_h_path = horizontalpath + "VRSL-DMX-Static-Blinder-H-13CH.prefab";
            blind_v_path = verticalpath + "VRSL-DMX-Static-Blinder-V-13CH.prefab";
            blind_l_path = legacypath + "VRSL-DMX-Static-Blinder-L-13CH.prefab";
            blind_a_path = audiopath + "VRSL-AudioLink-Static-Blinder.prefab";

            par_h_path = horizontalpath + "VRSL-DMX-Static-ParLight-H-13CH.prefab";
            par_v_path = verticalpath + "VRSL-DMX-Static-ParLight-V-13CH.prefab";
            par_l_path = legacypath + "VRSL-DMX-Static-ParLight-L-13CH.prefab";
            par_a_path = audiopath + "VRSL-AudioLink-Static-ParLight.prefab";

            bar_h_path = horizontalpath + "VRSL-DMX-Static-LightBar-H-13CH.prefab";
            bar_v_path = verticalpath + "VRSL-DMX-Static-LightBar-V-13CH.prefab";
            bar_l_path = legacypath + "VRSL-DMX-Static-LightBar-L-13CH.prefab";
            bar_a_path = audiopath + "VRSL-AudioLink-Static-Lightbar.prefab";
        }
        else
        {
            blind_h_path = horizontalpath + "5-Channel Statics/" + "VRSL-DMX-Static-Blinder-H-5CH.prefab";
            blind_v_path = verticalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-Blinder-V-5CH.prefab";
            blind_l_path = legacypath + "VRSL-DMX-Static-Blinder-L-13CH.prefab";
            blind_a_path = audiopath + "VRSL-AudioLink-Static-Blinder.prefab";

            par_h_path = horizontalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-ParLight-H-5CH.prefab";
            par_v_path = verticalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-ParLight-V-5CH.prefab";
            par_l_path = legacypath + "VRSL-DMX-Static-ParLight-L-13CH.prefab";
            par_a_path = audiopath + "VRSL-AudioLink-Static-ParLight.prefab";

            bar_h_path = horizontalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-LightBar-H-5CH.prefab";
            bar_v_path = verticalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-LightBar-V-5CH.prefab";
            bar_l_path = legacypath + "VRSL-DMX-Static-LightBar-L-13CH.prefab";
            bar_a_path = audiopath + "VRSL-AudioLink-Static-Lightbar.prefab";

        }

        string flash_h_path = horizontalpath + "VRSL-DMX-Static-Flasher-H-1CH.prefab";
        string flash_v_path = verticalpath + "VRSL-DMX-Static-Flasher-V-1CH.prefab";
        string flash_l_path = legacypath + "VRSL-DMX-Static-Flasher-L-1CH.prefab";
        string flash_a_path = audiopath + "VRSL-AudioLink-Static-Flasher.prefab";

        string disco_h_path = horizontalpath + "VRSL-DMX-Static-DiscoBall-H-1CH.prefab";
        string disco_v_path = verticalpath + "VRSL-DMX-Static-DiscoBall-V-1CH.prefab";
        string disco_l_path = legacypath + "VRSL-DMX-Static-DiscoBall-L-1CH.prefab";
        string disco_a_path = audiopath + "VRSL-AudioLink-DiscoBall.prefab";

        string laser_h_path = horizontalpath + "VRSL-DMX-Static-Laser-H-13CH.prefab";
        string laser_v_path = verticalpath + "VRSL-DMX-Static-Laser-V-13CH.prefab";
        string laser_l_path = legacypath + "VRSL-DMX-Static-Laser-L-13CH.prefab";
        string laser_a_path = audiopath + "VRSL-AudioLink-BasicLaser.prefab";
        

        switch(a)
        {
            //horizontal
            case 0:
                try{
                    spotlight_h = AssetDatabase.LoadAssetAtPath(spot_h_path, typeof(GameObject));
                    washlight_h = AssetDatabase.LoadAssetAtPath(wash_h_path, typeof(GameObject));
                    blinder_h = AssetDatabase.LoadAssetAtPath(blind_h_path, typeof(GameObject));
                    flasher_h = AssetDatabase.LoadAssetAtPath(flash_h_path, typeof(GameObject));
                    parlight_h = AssetDatabase.LoadAssetAtPath(par_h_path, typeof(GameObject));
                    lightbar_h = AssetDatabase.LoadAssetAtPath(bar_h_path, typeof(GameObject));
                    discoball_h = AssetDatabase.LoadAssetAtPath(disco_h_path, typeof(GameObject));
                    laser_h = AssetDatabase.LoadAssetAtPath(laser_h_path, typeof(GameObject));
                }
                catch(Exception e)
                {
                    loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();
                }
                break;
            //vertical
            case 1:
                try{
                    spotlight_v = AssetDatabase.LoadAssetAtPath(spot_v_path, typeof(GameObject));
                    washlight_v = AssetDatabase.LoadAssetAtPath(wash_v_path, typeof(GameObject));
                    blinder_v = AssetDatabase.LoadAssetAtPath(blind_v_path, typeof(GameObject));
                    flasher_v = AssetDatabase.LoadAssetAtPath(flash_v_path, typeof(GameObject));
                    parlight_v = AssetDatabase.LoadAssetAtPath(par_v_path, typeof(GameObject));
                    lightbar_v = AssetDatabase.LoadAssetAtPath(bar_v_path, typeof(GameObject));
                    discoball_v = AssetDatabase.LoadAssetAtPath(disco_v_path, typeof(GameObject));
                    laser_v = AssetDatabase.LoadAssetAtPath(laser_v_path, typeof(GameObject));
                }
                catch(Exception e)
                {
                    loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();                    
                }
                break;
            //legacy
            case 2:
                try{
                    spotlight_l = AssetDatabase.LoadAssetAtPath(spot_l_path, typeof(GameObject));
                    washlight_l = AssetDatabase.LoadAssetAtPath(wash_l_path, typeof(GameObject));
                    blinder_l = AssetDatabase.LoadAssetAtPath(blind_l_path, typeof(GameObject));
                    flasher_l = AssetDatabase.LoadAssetAtPath(flash_l_path, typeof(GameObject));
                    parlight_l = AssetDatabase.LoadAssetAtPath(par_l_path, typeof(GameObject));
                    lightbar_l = AssetDatabase.LoadAssetAtPath(bar_l_path, typeof(GameObject));
                    discoball_l = AssetDatabase.LoadAssetAtPath(disco_l_path, typeof(GameObject));
                    laser_l = AssetDatabase.LoadAssetAtPath(laser_l_path, typeof(GameObject));
                }
                catch(Exception e)
                {
                    loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();                    
                }
                break;
            //audiolink
            case 3:
                try{
                    spotlight_a = AssetDatabase.LoadAssetAtPath(spot_a_path, typeof(GameObject));
                    washlight_a = AssetDatabase.LoadAssetAtPath(wash_a_path, typeof(GameObject));
                    blinder_a = AssetDatabase.LoadAssetAtPath(blind_a_path, typeof(GameObject));
                    flasher_a = AssetDatabase.LoadAssetAtPath(flash_a_path, typeof(GameObject));
                    parlight_a = AssetDatabase.LoadAssetAtPath(par_a_path, typeof(GameObject));
                    lightbar_a = AssetDatabase.LoadAssetAtPath(bar_a_path, typeof(GameObject));
                    discoball_a = AssetDatabase.LoadAssetAtPath(disco_a_path, typeof(GameObject));
                    laser_a = AssetDatabase.LoadAssetAtPath(laser_a_path, typeof(GameObject));
                }
                catch(Exception e)
                {
                    loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();                    
                }
                break;
            default:
                break;
        }
        return loadSuccessful;
    }

    void OnGUI() {
        DrawLogo();
        ShurikenHeaderCentered(ver);
        GUILayout.Label("Control Panel",Title1());

        if(Application.isPlaying)
        {
            //CheckForDesktopScreen();
            GUILayout.Label("Control Panel Disabled while Editor is playing!", WarningLabel());
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical("box");
            if(hasDesktopDuplication && runTimeScreen !=null)
            {
                GUILayout.Label("Scene Desktop Duplication Screen.", SectionLabel());
                //CreateEditor(runTimeScreen, typeof(Editor).Assembly.GetType("UnityEditor.RectTransformEditor")).OnInspectorGUI();
                var monitor = runTimeScreen.monitor;
                //SerializedObject obj = new SerializedObject(monitor);
                var id = EditorGUILayout.Popup("Monitor", monitor.id, uDesktopDuplication.Manager.monitors.Select(x => x.name).ToArray());
                if (id != monitor.id) { runTimeScreen.monitorId = id; }
                //obj.Update();
                //SerializedProperty prop = obj.FindProperty("monitor");
                //EditorGUILayout.PropertyField(prop, true);
                //obj.ApplyModifiedProperties();

                // DrawProperties(prop, true);
            }
            EditorGUILayout.EndVertical();
            return;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        EditorGUI.BeginDisabledGroup(true);
        if(hasLocalPanel)
        {
            EditorGUILayout.ObjectField(Label("Current Local UI Panel", "The Current VRSL Control Panel in the scene. This is where most of the settings for your VRSL enabled scene will be stored."),panel, panel.GetType(), true);
        }
        if(hasDepthLight)
        {
            EditorGUILayout.ObjectField(Label("Current Depth Light", "The main depth enabled directioanl light in the scene. This ensures that the PC quality lighting effects are working properly."),depthLight, depthLight.GetType(), true);
        }
        EditorGUI.EndDisabledGroup();
        mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos, false, false);
        if(!hasDepthLight)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Please ensure the ''Directional Light (For Depth)'' object is in your scene to ensure your lights work properly!", WarningLabel());
            if(GUILayout.Button(Label("Spawn Depth Light Prefab!", "Spawn the VRSL depth light prefab! A directional light that emits no light/shadows, but still enables the camera's depth texture for the scene.")))
            {
                    Debug.Log("VRSL Control Panel: Spawning Depth Light Prefab...");
                    if(LoadPrefabs())
                        SpawnPrefabWithUndo(directionalLightPrefab, "Spawn Directional Light");
                        //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(directionalLightPrefab as GameObject) ,null);
                    Repaint();
            }
            EditorGUILayout.EndVertical();
        }
        if(hasLocalPanel && panel != null)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(!panel.isUsingDMX);
            var so = new SerializedObject(panel);
            so.FindProperty("DMXMode").intValue = EditorGUILayout.Popup(Label("DMX Grid Mode", "Choose what grid type textures should be enabled for DMX mode. Unused textures will be disabled to save editor performance!"),panel.DMXMode, dmxModes);
            so.FindProperty("fixtureGizmos").intValue = EditorGUILayout.Popup(Label("Show DMX Info In Scene", "Display DMX Channel and/or Universe information above each fixture in the scene view!"), panel.fixtureGizmos, dmxGizmoInfo);
            so.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            legacyFixtures = so.FindProperty("useLegacyStaticLights").boolValue;

            string o = mainOptionsFoldout ? "Hide Options" : "Show Options";
            mainOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(mainOptionsFoldout,Label(o, "Show/Hide Global VRSL Settings and Prefab Spawners."));
            EditorGUILayout.EndFoldoutHeaderGroup();
            if(mainOptionsFoldout)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.BeginDisabledGroup(!panel.isUsingDMX);

                
                EditorGUILayout.BeginHorizontal("box");
                var soptr = new SerializedObject(panel);
                soptr.FindProperty("panRangeTarget").floatValue = EditorGUILayout.FloatField(Label("DMX Target Pan Range", "Field to enter in the pan range to apply to all fixtures"), panRangeOff, GUILayout.MaxWidth(200f));
                SetRangeButton(-90f, true);
                SetRangeButton(90f, true);
                SetRangeButton(180f, true);
                SetRangeButton(360f, true);
                SetRangeButton(540f, true);
                if(GUILayout.Button(Label("Apply To All Fixtures (Pan)", "Click to apply the displayed pan range to all fixtures in the scene!"), GUILayout.MaxWidth(170f)))
                {
                    MassApplyPTRange(true);
                    Debug.Log("VRSL Control Panel: Updating Tilt Range of All Fixtures and Applying Changes!");
                    ApplyChangesToFixtures(true, false, false);
                    ResetItems(false);
                    Repaint();   
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal("box");
                soptr.FindProperty("tiltRangeTarget").floatValue = EditorGUILayout.FloatField(Label("DMX Target Tilt Range", "Field to enter in the tilt range to apply to all fixtures"), tiltRangeOff, GUILayout.MaxWidth(200f));
                SetRangeButton(90f, false);
                SetRangeButton(-90f, false);
                SetRangeButton(-180f, false);
                SetRangeButton(-250f, false);
                SetRangeButton(-270f, false);
                if(GUILayout.Button(Label("Apply To All Fixtures (Tilt)", "Click to apply the displayed tilt range to all fixtures in the scene!"), GUILayout.MaxWidth(170f)))
                {
                    MassApplyPTRange(false);
                    Debug.Log("VRSL Control Panel: Updating Tilt Range of All Fixtures and Applying Changes!");
                    ApplyChangesToFixtures(true, false, false);
                    ResetItems(false);
                    Repaint();   
                }

                EditorGUILayout.EndHorizontal();


                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
                //EditorGUILayout.Space();
                
                //GuiLine();
                //GUILayout.Label("Prefab Spawn List", Title2());
                panel.UpdateProxy();
                panel.DMXMode = so.FindProperty("DMXMode").intValue;
                panel.fixtureGizmos = so.FindProperty("fixtureGizmos").intValue;
                panel.ApplyProxyModifications();
                EditorGUILayout.BeginHorizontal("box");


                //EditorGUILayout.Space();
                string t = "Show DMX Prefab Spawn Buttons";
                if(dmxSpawnsFoldout)
                    t = "Hide DMX Prefab Spawn Buttons";
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(((position.width/2f)-30f)));
                dmxSpawnsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(dmxSpawnsFoldout,Label(t, "Show/Hide DMX Prefab Spawn Buttons!"), Title2Foldout());
                
                if(dmxSpawnsFoldout)
                {
                    
                    GUILayout.Label("DMX Reader Screens (Desktop To Editor)", Title3());
                    
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Horizontal", "Spawn the horizontal version of the uDesktop DMX Reader screen! Use send your DMX stream directly to the Unity Editor!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Horizontal Desktop to Editor DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uDesktopHorizontalPrefab, "Spawn Desktop To Editor Screen");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uDesktopHorizontalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Vertical", "Spawn the vertical version of the uDesktop DMX Reader screen! Use send your DMX stream directly to the Unity Editor!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Vertical Desktop to Editor DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uDesktopVerticalPrefab, "Spawn Desktop To Editor Screen");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uDesktopVerticalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Legacy", "Spawn the legacy version of the uDesktop DMX Reader screen! Use send your DMX stream directly to the Unity Editor!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Legacy Desktop to Editor DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uDesktopLegacyPrefab, "Spawn Desktop To Editor Screen");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uDesktopLegacyPrefab as GameObject), null);
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                   // EditorGUILayout.EndVertical();
                    GUILayout.Label("DMX Reader Screens (USharp Video Player)", Title3());
                    //EditorGUILayout.BeginVertical("box");
                    
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Horizontal", "Spawn the horizontal version of the USharp Video Player DMX Reader Screen. Use this video player prefab to stream DMX data to VRChat via Twitch/VRCDN! You can replace the video player in this prefab with any other video player you want as long as the screen gets duplicated to the larger plane!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Horizontal Usharp Video DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uVidHorizontalPrefab, "Spawn Usharp Video DMX Screen");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uVidHorizontalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Vertical", "Spawn the vertical version of the USharp Video Player DMX Reader Screen. Use this video player prefab to stream DMX data to VRChat via Twitch/VRCDN! You can replace the video player in this prefab with any other video player you want as long as the screen gets duplicated to the larger plane!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Vertical Usharp Video DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uVidVerticalPrefab, "Spawn Usharp Video DMX Screen");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uVidVerticalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Legacy", "Spawn the legacy version of the USharp Video Player DMX Reader Screen. Use this video player prefab to stream DMX data to VRChat via Twitch/VRCDN! You can replace the video player in this prefab with any other video player you want as long as the screen gets duplicated to the larger plane!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Legacy Usharp Video DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uVidLegacyPrefab, "Spawn Usharp Video DMX Screen");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uVidLegacyPrefab as GameObject), null);
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                    string fp = "";
                    switch(panel.DMXMode)
                    {
                        case 0:
                            fp = "Horizontal";
                            break;
                        case 1:
                            fp = "Vertical";
                            break;
                        case 2:
                            fp = "Legacy";
                            break;
                        default:
                            break;
                    }  
                    //EditorGUILayout.BeginHorizontal(); 
                    GUILayout.Label("Fixture Prefabs: " + fp, Title3());
                    //EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    int mode = panel.DMXMode;

                    string legacyfixString = legacyFixtures || mode == 2 ? "(13CH)" : "(5CH)";


                    if(GUILayout.Button(Label("SpotLight (13CH)", "Spawn the " + fp + " version of the DMX Spotlight prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Spotlight Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(spotlight_h, "Spawn Horizontal Spotlight");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(spotlight_v, "Spawn Vertical Spotlight");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(spotlight_l, "Spawn Legacy Spotlight");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("WashLight (13CH)", "Spawn the " + fp + " version of the DMX Moving Washlight prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Washlight Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(washlight_h, "Spawn Horizontal Washlight");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(washlight_v, "Spawn Vertical Washlight");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(washlight_l, "Spawn Legacy Washlight");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Blinder " + legacyfixString, "Spawn the " + fp + " version of the DMX Blinder prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Blinder Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(blinder_h, "Spawn Horizontal Blinder");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(blinder_v, "Spawn Vertical Blinder");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(blinder_l, "Spawn Legacy Blinder");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Flasher (1CH)", "Spawn the " + fp + " version of the DMX Flasher prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Flasher Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(flasher_h, "Spawn Horizontal Flasher");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(flasher_v, "Spawn Vertical Flasher");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(flasher_l, "Spawn Legacy Flasher");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("ParLight " + legacyfixString, "Spawn the " + fp + " version of the DMX Parlight prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Parlight Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(parlight_h, "Spawn Horizontal Parlight");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(parlight_v, "Spawn Vertical Parlight");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(parlight_l, "Spawn Legacy Parlight");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("LightBar " + legacyfixString, "Spawn the " + fp + " version of the DMX LightBar prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " LightBar Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(lightbar_h, "Spawn Horizontal LightBar");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(lightbar_v, "Spawn Vertical LightBar");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(lightbar_l, "Spawn Legacy LightBar");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Discoball (1CH)", "Spawn the " + fp + " version of the DMX Discoball prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Discoball Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(discoball_h, "Spawn Horizontal Discoball");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(discoball_v, "Spawn Vertical Discoball");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(discoball_l, "Spawn Legacy Discoball");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Laser (13CH)", "Spawn the " + fp + " version of the DMX Basic Laser prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Laser Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(laser_h, "Spawn Horizontal Laser");
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(laser_v, "Spawn Vertical Laser");
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(laser_l, "Spawn Legacy Laser");
                                        break; 
                                    default:
                                        break;                                    
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                  //  panel.useLegacyStaticLights = so.FindProperty("useLegacyStaticLights").boolValue;
                 //   panel.useLegacyStaticLights = EditorGUILayout.Toggle("Use Legacy Static Lights", panel.useLegacyStaticLights);
                    soptr.FindProperty("useLegacyStaticLights").boolValue = EditorGUILayout.ToggleLeft("Use Old 13 Channel Static Lights (Not Recommended)", panel.useLegacyStaticLights);
                    soptr.ApplyModifiedProperties();
                    panel.UpdateProxy();
                    panel.panRangeTarget = soptr.FindProperty("panRangeTarget").floatValue;
                    panel.tiltRangeTarget = soptr.FindProperty("tiltRangeTarget").floatValue;
                    panel.useLegacyStaticLights = soptr.FindProperty("useLegacyStaticLights").boolValue;
                    panel.ApplyProxyModifications();

                    
                }        
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
               //EditorGUILayout.Space();
                string a = "Show AudioLink Prefab Spawn Buttons";
                if(audioLinkSpawnsFoldout)
                    a = "Hide AudioLink Prefab Spawn Buttons";
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth((position.width/2)));
                audioLinkSpawnsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(audioLinkSpawnsFoldout,Label(a, "Show/Hide AudioLink Prefab Spawn Buttons!"), Title2Foldout());
                if(audioLinkSpawnsFoldout)
                {
                    GUILayout.Label("AudioLink Prefabs", Title3());
                    
                    if(GUILayout.Button(Label("Spawn Standard AudioLink Prefab", "Spawn the standard AudioLink prefab that comes with AudioLink."), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Standard AudioLink Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(audioLinkPrefab, "Spawn AudioLink Prefab");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(audioLinkPrefab as GameObject), null);
                        Repaint();
                    }

                    GUILayout.Label("Controller Prefabs", Title3());
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Spawn Standard AudioLink Controller", "Spawn the standard AudioLink Controller Prefab that comes with AudioLink."), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning VRSL AudioLink Controller Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(standardAudioLinkControllerPrefab, "Spawn AudioLink Controller");
                        Repaint();
                    }

                    if(GUILayout.Button(Label("Spawn VRSL AudioLink Controller", "Spawn the VRS version of the AudioLink Controller Prefab. This version has extra smoothing sliders to smooth out the pulsing from AudioLink for the lights."), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning VRSL AudioLink Controller Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(audioLinkControllerPrefab, "Spawn VRSL AudioLink Controller");
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(audioLinkControllerPrefab as GameObject), null);
                        Repaint();
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Label("Fixture Prefabs: ", Title3());
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("SpotLight", "Spawn the AudioLink version of the VRSL Spotlight prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Spolight Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(spotlight_a, "Spawn AudioLink Spotlight");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("WashLight", "Spawn the AudioLink version of the VRSL WashLight prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Washlight Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(washlight_a, "Spawn AudioLink Washlight");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Blinder", "Spawn the AudioLink version of the VRSL Blinder prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Blinder Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(blinder_a, "Spawn AudioLink Blinder");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Flasher", "Spawn the AudioLink version of the VRSL Flasher prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Flasher Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(flasher_a, "Spawn AudioLink Flasher");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("ParLight", "Spawn the AudioLink version of the VRSL ParLight prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Parlight Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(parlight_a, "Spawn AudioLink Parlight");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("LightBar", "Spawn the AudioLink version of the VRSL LightBar prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink LightBar Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(lightbar_a, "Spawn AudioLink LightBar");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Discoball" ,"Spawn the AudioLink version of the VRSL Discoball prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Discoball Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(discoball_a, "Spawn AudioLink Discoball");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    if(GUILayout.Button(Label("Laser", "Spawn the AudioLink version of the VRSL Basic Laser prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning AudioLink Laser Prefab...");
                        if(LoadFixturePrefabs(3))
                        {
                            try
                            {
                                SpawnPrefabWithUndo(laser_a, "Spawn AudioLink Laser");
                            }
                            catch(Exception e)
                            {
                                Debug.Log("VRSL Control Panel: DMX Fixture Prefab Spawn Failed!");
                                e.ToString();
                            }
                            Repaint();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();


              //  EditorGUI.BeginDisabledGroup(!panel.isUsingDMX);

                //EditorGUI.EndDisabledGroup();



                

                //EditorGUILayout
                
            }
        }


        if(hasLocalPanel && panel != null)
        {
            GuiLine();
            GUILayout.Label("Fixture List", Title2());
            EditorGUILayout.Space(5f);
            if(GUILayout.Button(Label("Apply Changes", "Apply all changes in the fixture list to all affected fixtures in the scene!"), BigButton()))
            {

                Debug.Log("VRSL Control Panel: Applying Changes!");
                ApplyChangesToFixtures(true, false, false);
                ResetItems(false);
                Repaint();
            }
            if(GUILayout.Button(Label("Discard Changes", "Discard all changes in the fixture list to all affected fixtures in the scene!"), NormalButton()))
            {
                Debug.Log("VRSL Control Panel: Discarding Changes!");
                ApplyChangesToFixtures(false, false, false);
                ResetItems(false);
                Repaint();
            }
            GUILayout.BeginHorizontal();

            var so = new SerializedObject(panel);

////////////////////////////////////////////////////////////////////////
            Color bg = GUI.backgroundColor;
            GUIContent dmxRectText = new GUIContent(ToggleText(panel.isUsingDMX,"DMX"), "Enable/Disable DMX mode from this scene. This will disable all corresponding DMX render textures and prevent them from updating at runtime.");
            Rect dmxRect = GUILayoutUtility.GetRect(dmxRectText, NormalButtonToggle(panel.isUsingDMX, dmxHover));
            if (Event.current.type == EventType.Repaint && dmxRect.Contains(Event.current.mousePosition))
            {
                dmxHover = true;
            }
            else
            {
                dmxHover = false;
            }
            so.FindProperty("isUsingDMX").boolValue = GUI.Toggle(dmxRect, panel.isUsingDMX, dmxRectText, NormalButtonToggle(panel.isUsingDMX, dmxHover));
            //so.FindProperty("isUsingDMX").boolValue = GUILayout.Toggle(panel.isUsingDMX, ToggleText(panel.isUsingDMX,"DMX"), NormalButtonToggle(panel.isUsingDMX, dmxHover));
            
            GUIContent audioRectText = new GUIContent(ToggleText(panel.isUsingAudioLink,"AUDIOLINK"), "Enable/Disable AudioLink MOde from this scene. This will diable all corresponding AudioLink render textures and prevent them from updating at runtime.");
            Rect audioRect = GUILayoutUtility.GetRect(audioRectText, NormalButtonToggle(panel.isUsingAudioLink, audioLinkHover));
            if (Event.current.type == EventType.Repaint && audioRect.Contains(Event.current.mousePosition))
            {
                audioLinkHover = true;
            }
            else
            {
                audioLinkHover = false;
            }
            so.FindProperty("isUsingAudioLink").boolValue = GUI.Toggle(audioRect, panel.isUsingAudioLink, audioRectText, NormalButtonToggle(panel.isUsingAudioLink, audioLinkHover));
            //so.FindProperty("isUsingAudioLink").boolValue = GUILayout.Toggle(panel.isUsingAudioLink, ToggleText(panel.isUsingAudioLink,"AUDIOLINK"), NormalButtonToggle(panel.isUsingAudioLink, audioLinkHover));

            GUI.backgroundColor = bg;
/////////////////////////////////////////////////////////////////////////////////



            so.ApplyModifiedProperties();
            panel.UpdateProxy();
            panel.isUsingDMX = so.FindProperty("isUsingDMX").boolValue;
            panel.isUsingAudioLink = so.FindProperty("isUsingAudioLink").boolValue;
            panel.ApplyProxyModifications();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            float sectionWidth = position.width /2;
            dmxScrollPos = EditorGUILayout.BeginScrollView(dmxScrollPos, true, true, GUILayout.MaxWidth((sectionWidth)));
            if(panel.isUsingDMX)
            {

                for(int i = 0; i < universes.Length; i++)
                {
                    // if(i == 0)
                    //     GUILayout.Space(3.0f);
                   // GUILayout.Space(15.0f);
                    //GUILayout.Label("Universe " + (i + 1), Title1());
                    universeFold[i] = EditorGUILayout.BeginFoldoutHeaderGroup(universeFold[i], Label("Universe " + (i + 1), "Show/Hide all fixtures in in DMX Universe " + (i+1)), Title1Foldout());
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    GuiLine();
                    //EditorGUILayout.BeginHorizontal();
                    //EditorGUILayout.Space();
                   // EditorGUILayout.BeginVertical();
                    if(universeFold[i])
                    {
                        //if(i == 0)
                        // EditorGUILayout.BeginHorizontal();
                        // EditorGUILayout.Space();
                        // EditorGUILayout.BeginVertical();
                        if(universes[i] == null)
                        {
                            continue;
                        }
                        foreach(DMXListItem fixture in universes[i])
                        {
                            //bool showPosition = false;
                            
                            //GUILayout.Space(2.0f);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(15f);
                            fixture.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(fixture.foldout,"<b><i>Channel: " + fixture.light._GetDMXChannel() + "</i></b> \n" + fixture.light.name, SelectionFoldout());
                            EditorGUILayout.EndHorizontal();
                        // EditorGUILayout.Foldout(dmxBoolList[dmxLights.IndexOf(light)],"U: " + light._GetUniverse() + " CH: " + light._GetDMXChannel() + " " + light.name, FoldOutStyle());
                            if(fixture.foldout)
                            {
                                EditorGUILayout.BeginVertical("box");
                            // fixture.light.UpdateProxy();
                            // GUILayout.Label("U: " + light._GetUniverse() + " CH: " + light._GetDMXChannel() + " " + light.name, SectionLabel());
                                GUILayout.Space(8.0f);
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.ObjectField("Selected Fixture",fixture.light, fixture.light.GetType(), true, GUILayout.MaxWidth(sectionWidth -10));
                                EditorGUI.EndDisabledGroup();


                                GUILayout.Space(15.0f);
                                GUILayout.Label("DMX Settings", SecLabel());
                                GUILayout.Space(8.0f);
                                fixture.P_enableDMXChannels = EditorGUILayout.Toggle("Enable DMX", fixture.P_enableDMXChannels);
                                fixture.P_useLegacySectorMode = EditorGUILayout.Toggle("Legacy Sector Mode",fixture.P_useLegacySectorMode);
                                if(fixture.P_useLegacySectorMode)
                                {
                                    fixture.P_sector = EditorGUILayout.IntField("Sector", fixture.P_sector, GUILayout.MaxWidth(sectionWidth - 10));
                                    fixture.P_singleChannelMode = EditorGUILayout.Toggle("Single Channel Mode",fixture.P_singleChannelMode);
                                    if(fixture.P_singleChannelMode)
                                    {
                                        fixture.P_Channel = EditorGUILayout.IntSlider("Single Channel CH",fixture.P_Channel, 0, 12, GUILayout.MaxWidth(sectionWidth - 10));
                                    }
                                }
                                else
                                {
                                    fixture.P_dmxChannel = EditorGUILayout.IntSlider("DMX Channel",fixture.P_dmxChannel, 1, 512, GUILayout.MaxWidth(sectionWidth - 10));
                                    fixture.P_dmxUniverse = EditorGUILayout.IntSlider("Universe",fixture.P_dmxUniverse, 1, 3, GUILayout.MaxWidth(sectionWidth - 10));
                                }
                                fixture.P_legacyGoboRange = EditorGUILayout.Toggle("Legacy Gobo Range",fixture.P_legacyGoboRange);
                                GUILayout.Space(4.0f);

                                GUILayout.Space(4.0f);
                                GUILayout.Label("General Settings", SecLabel());
                                GUILayout.Space(8.0f);
                                fixture.P_globalIntensity = EditorGUILayout.Slider("Global Intensity",fixture.P_globalIntensity, 0f, 1f, GUILayout.MaxWidth(sectionWidth - 10));
                                fixture.P_finalIntensity = EditorGUILayout.Slider("Final Intensity",fixture.P_finalIntensity, 0f, 1f, GUILayout.MaxWidth(sectionWidth - 10));
                                fixture.P_lightColorTint = EditorGUILayout.ColorField(colorLabel,fixture.P_lightColorTint, true,true,true, GUILayout.MaxWidth(sectionWidth - 10));
                                GUILayout.Space(4.0f);
                                GUILayout.Label("Movement Settings", SecLabel());
                                GUILayout.Space(8.0f);
                                fixture.P_invertPan = EditorGUILayout.Toggle("Invert Pan", fixture.P_invertPan);
                                fixture.P_invertTilt = EditorGUILayout.Toggle("Invert Tilt", fixture.P_invertTilt);
                                fixture.P_isUpsideDown = EditorGUILayout.Toggle("Is Upside Down?", fixture.P_isUpsideDown);
                                fixture.P_maxMinPan = EditorGUILayout.FloatField("Max/Min Pan Range", fixture.P_maxMinPan);
                                fixture.P_maxMinTilt = EditorGUILayout.FloatField("Max/Min Tilt Range", fixture.P_maxMinTilt);
                                GUILayout.Space(4.0f);
                                GUILayout.Space(4.0f);
                                GUILayout.Label("Fixture Settings", SecLabel());
                                GUILayout.Space(8.0f);
                                fixture.P_enableAutoSpin = EditorGUILayout.Toggle("Projection Spin", fixture.P_enableAutoSpin);
                                fixture.P_enableStrobe = EditorGUILayout.Toggle("Strobe Function", fixture.P_enableStrobe);
                                fixture.P_tiltOffsetBlue = EditorGUILayout.Slider("Tilt Offset",fixture.P_tiltOffsetBlue, 0f, 360f, GUILayout.MaxWidth(sectionWidth - 10));
                                fixture.P_panOffsetBlueGreen = EditorGUILayout.Slider("Pan Offset",fixture.P_panOffsetBlueGreen, 0f, 360f, GUILayout.MaxWidth(sectionWidth - 10));
                                fixture.P_selectGOBO = EditorGUILayout.IntSlider("GOBO Selection",fixture.P_selectGOBO, 1, 8, GUILayout.MaxWidth(sectionWidth - 10));
                                GUILayout.Space(4.0f);
                                GUILayout.Space(4.0f);
                                GUILayout.Label("Mesh Settings", SecLabel());
                                GUILayout.Space(8.0f);
                                fixture.P_coneWidth = EditorGUILayout.Slider("Cone Width",fixture.P_coneWidth, 0f, 5.5f, GUILayout.MaxWidth(sectionWidth - 10));
                                fixture.P_coneLength = EditorGUILayout.Slider("Cone Length",fixture.P_coneLength, 0.5f, 10.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                fixture.P_maxConeLength = EditorGUILayout.Slider("Max Cone Length",fixture.P_maxConeLength, 0.275f, 10.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                GUILayout.Space(15.0f);

                                EditorGUILayout.EndVertical();
                            // fixture.light.ApplyProxyModifications();
                            }

                            GuiLine();
                            EditorGUILayout.EndFoldoutHeaderGroup();

                            //GUILayout.Space(2.0f);

                        }
                            // EditorGUILayout.EndVertical();
                            // EditorGUILayout.EndHorizontal();

                    }
                   // EditorGUILayout.EndVertical();
                   // EditorGUILayout.EndHorizontal();
                }
                universeFourFold = EditorGUILayout.BeginFoldoutHeaderGroup(universeFourFold, Label("Universe 4 (Experimental)", "DMX Universe 4 is an experimental universe used for testing out the new DMX via Audio Amplitude feature. Any Audio Amplitude based scripts will appear here."), Title1Foldout());
                GuiLine();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.EndScrollView();
            audioLinkScrollPos = EditorGUILayout.BeginScrollView(audioLinkScrollPos, true, true,GUILayout.MaxWidth((position.width / 2) + 25f));
                if(panel.isUsingAudioLink)
                {

                    for(int i = 0; i < 4 ; i++)
                    {
                        string b = "";
                        switch(i)
                        {
                            case 0:
                                b = "(Bass)";
                                break;
                            case 1:
                                b = "(Low Mids)";
                                break;
                            case 2:
                                b = "(High Mids)";
                                break;
                            case 3:
                                b = "(Treble)";
                                break;
                            default:
                                break;
                        }
                        bandFold[i] = EditorGUILayout.BeginFoldoutHeaderGroup(bandFold[i], Label("Band " + (i) + " " + b, "Show/Hide all fixtures on AudioLink Band " + i), Title1Foldout());
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        GuiLine();
                        if(bandFold[i]){
                            foreach(AudioLinkListItem fixture in audioLinkLights)
                            {
                                
                                if(fixture.isLaser)
                                {
                                    if(fixture.laser.Band == i)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(15f);
                                        fixture.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(fixture.foldout, fixture.laser.name, SelectionFoldout());
                                        EditorGUILayout.EndHorizontal();
                                        if(fixture.foldout)
                                        {
                                            EditorGUILayout.BeginHorizontal("box");
                                            GUILayout.Space(30f);
                                            
                                            EditorGUILayout.BeginVertical();
                                            EditorGUI.BeginDisabledGroup(true);
                                            EditorGUILayout.ObjectField("Band: " + fixture.laser.Band + " Delay: " + fixture.laser.Delay,fixture.laser, fixture.laser.GetType(), true, GUILayout.MaxWidth(sectionWidth - 10));
                                            EditorGUI.EndDisabledGroup();
                                            
                                            GUILayout.Space(15.0f);
                                            GUILayout.Label("AudioLink Settings", SecLabel());
                                            GUILayout.Space(8.0f);
                                            fixture.P_enableAudioLink = EditorGUILayout.Toggle("Enable AudioLink", fixture.P_enableAudioLink);
                                            fixture.P_band = EditorGUILayout.IntField("Band", fixture.P_band, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_delay = EditorGUILayout.IntSlider("Delay",fixture.P_delay, 0, 31, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_bandMultiplier = EditorGUILayout.Slider("Band Multiplier",fixture.P_bandMultiplier, 1f, 15f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_enableColorChord = EditorGUILayout.Toggle("Enable Color Chord", fixture.P_enableColorChord);
                                            GUILayout.Space(8.0f);
                                            GUILayout.Label("General Settings", SecLabel());
                                            GUILayout.Space(8.0f);

                                            fixture.P_globalIntensity = EditorGUILayout.Slider("Global Intensity",fixture.P_globalIntensity, 0f, 1f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_finalIntensity = EditorGUILayout.Slider("Final Intensity",fixture.P_finalIntensity, 0f, 1f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_lightColorTint = EditorGUILayout.ColorField(colorLabel,fixture.P_lightColorTint, true,true,true, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_enableColorTextureSampling = EditorGUILayout.Toggle("Enable Texture Sampling", fixture.P_enableColorTextureSampling);
                                            fixture.P_textureSamplingCoordinates = EditorGUILayout.Vector2Field("Texture Sampling Coordinates", fixture.P_textureSamplingCoordinates, GUILayout.MaxWidth(sectionWidth - 10));
                                            GUILayout.Space(8.0f);
                                            GUILayout.Label("Mesh Settings", SecLabel());
                                            GUILayout.Space(8.0f);

                                            fixture.P_coneWidth = EditorGUILayout.Slider("Cone Width",fixture.P_coneWidth, -3.75f, 20.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_coneLength = EditorGUILayout.Slider("Cone Length",fixture.P_coneLength,-0.5f, 5.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_coneFlatness = EditorGUILayout.Slider("Cone Flatness",fixture.P_coneFlatness, 0.0f, 1.999f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_coneXRotation = EditorGUILayout.Slider("Cone X Rotation",fixture.P_coneXRotation, -90.0f, 90.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_coneYRotation = EditorGUILayout.Slider("Cone Y Rotation",fixture.P_coneYRotation, -90.0f, 90.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_coneZRotation = EditorGUILayout.Slider("Cone Z Rotation",fixture.P_coneZRotation, -90.0f, 90.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_laserCount = EditorGUILayout.IntSlider("Laser Count",fixture.P_laserCount, 4, 68, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_laserThickness = EditorGUILayout.Slider("Laser Thickness",fixture.P_laserThickness, 0.003f, 0.25f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_laserScroll = EditorGUILayout.Slider("Laser Scroll",fixture.P_laserScroll, -1.0f, 1.0f, GUILayout.MaxWidth(sectionWidth - 10));



                                            GUILayout.Space(15.0f);
                                            EditorGUILayout.EndVertical();
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        GuiLine();
                                        EditorGUILayout.EndFoldoutHeaderGroup();
                                    }
                                }
                                else
                                {
                                    if(fixture.light.Band == i)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(15f);
                                        fixture.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(fixture.foldout, fixture.light.name, SelectionFoldout());
                                        EditorGUILayout.EndHorizontal();
                                        if(fixture.foldout)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            GUILayout.Space(30f);
                                            
                                            EditorGUILayout.BeginVertical("box");
                                            EditorGUI.BeginDisabledGroup(true);
                                            EditorGUILayout.ObjectField("Band: " + fixture.light.Band + " Delay: " + fixture.light.Delay,fixture.light, fixture.light.GetType(), true, GUILayout.MaxWidth(sectionWidth - 10));
                                            EditorGUI.EndDisabledGroup();

                                            GUILayout.Space(15.0f);
                                            GUILayout.Label("AudioLink Settings", SecLabel());
                                            GUILayout.Space(8.0f);
                                            fixture.P_enableAudioLink = EditorGUILayout.Toggle("Enable AudioLink", fixture.P_enableAudioLink);
                                            fixture.P_band = EditorGUILayout.IntField("Band", fixture.P_band, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_delay = EditorGUILayout.IntSlider("Delay",fixture.P_delay, 0, 31, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_bandMultiplier = EditorGUILayout.Slider("Band Multiplier",fixture.P_bandMultiplier, 1f, 15f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_enableColorChord = EditorGUILayout.Toggle("Enable Color Chord", fixture.P_enableColorChord);
                                            GUILayout.Space(8.0f);
                                            GUILayout.Label("General Settings", SecLabel());
                                            GUILayout.Space(8.0f);

                                            fixture.P_globalIntensity = EditorGUILayout.Slider("Global Intensity",fixture.P_globalIntensity, 0f, 1f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_finalIntensity = EditorGUILayout.Slider("Final Intensity",fixture.P_finalIntensity, 0f, 1f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_lightColorTint = EditorGUILayout.ColorField(colorLabel,fixture.P_lightColorTint, true,true,true, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_enableColorTextureSampling = EditorGUILayout.Toggle("Enable Texture Sampling", fixture.P_enableColorTextureSampling);
                                            fixture.P_textureSamplingCoordinates = EditorGUILayout.Vector2Field("Texture Sampling Coordinates", fixture.P_textureSamplingCoordinates, GUILayout.MaxWidth(sectionWidth - 10));
                                            GUILayout.Space(8.0f);
                                            GUILayout.Label("Movement Settings", SecLabel());
                                            GUILayout.Space(8.0f);
                                            fixture.P_targetToFollow = EditorGUILayout.ObjectField("Target To Follow", fixture.P_targetToFollow, typeof(Transform), true, GUILayout.MaxWidth(sectionWidth - 10)) as Transform;
                                            GUILayout.Space(8.0f);
                                            GUILayout.Label("Fixture Settings", SecLabel());
                                            GUILayout.Space(8.0f);
                                            fixture.P_spinSpeed = EditorGUILayout.Slider("Spin Speed",fixture.P_spinSpeed, -10f, 10f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_enableAutoSpin = EditorGUILayout.Toggle("Enable Auto Spin", fixture.P_enableAutoSpin);
                                            fixture.P_selectGOBO = EditorGUILayout.IntSlider("Select GOBO",fixture.P_selectGOBO, 1, 8, GUILayout.MaxWidth(sectionWidth - 10));

                                            GUILayout.Space(8.0f);
                                            GUILayout.Label("Mesh Settings", SecLabel());
                                            GUILayout.Space(8.0f);

                                            fixture.P_coneWidth = EditorGUILayout.Slider("Cone Width",fixture.P_coneWidth, 0f, 5.5f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_coneLength = EditorGUILayout.Slider("Cone Length",fixture.P_coneLength, 0.5f, 10.0f, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_maxConeLength = EditorGUILayout.Slider("Max Cone Length",fixture.P_maxConeLength, 0.275f, 10.0f, GUILayout.MaxWidth(sectionWidth - 10));


                                            GUILayout.Space(15.0f);
                                            EditorGUILayout.EndVertical();
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        GuiLine();
                                        EditorGUILayout.EndFoldoutHeaderGroup();
                                    }
                                }
                                
                            }
                        }
                        
                    }
                }                
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();


        }
        else
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Please ensure the ''VRSL-LocalUIControlPanel'' prefab is in your scene!", WarningLabel());
            if(GUILayout.Button("Spawn VRSL Local UI Control Panel Prefab!"))
            {   
                if(controlPanelUiPrefab != null)
                {
                    Debug.Log("VRSL Control Panel: Spawning Control Panel Prefab...");
                    if(LoadPrefabs())
                        SpawnPrefabWithUndo(controlPanelUiPrefab, "Spawn VRSL Control Panel");
                        //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(controlPanelUiPrefab as GameObject), null);
                    Repaint();
                }
                // else
                // {
                //     Debug.Log("Failed to spawn control panel, please try again...");
                //     LoadPrefabs();
                //     Repaint();
                // }
                //Instantiate(controlPanelUiPrefab);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

    }
    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    void OnDestroy() 
    {
        if(hasLocalPanel && panel != null)
        {

               // Debug.Log("VRSL Control Panel: Discarding any unsaved changes!");
                ApplyChangesToFixtures(false, false, true);
                ResetItems(false);
        }
    }

    private GUIStyle SceneLabel()
    {
        GUIStyle g = new GUIStyle(EditorStyles.whiteLargeLabel);
        g.fontStyle = FontStyle.Bold;
        return g;
    }


     void OnSceneGUI(SceneView sceneView) 
     {
     // Do your drawing here using Handles.
     Handles.BeginGUI();
        if(hasLocalPanel && panel != null)
        {
            if(panel.fixtureGizmos == 1)
            {
                foreach(DMXListItem fixture in dmxLights)
                {
                    try{
                        if(fixture != null)
                        {
                            Vector3 pos = fixture.light.transform.position;
                            pos.y+= 0.1f;
                            pos.x -= 0.05f;
                            Handles.Label(pos, "CH: " + fixture.light._GetDMXChannel(), SceneLabel());
                        }
                    }
                    catch(MissingReferenceException e)
                    {
                        e.ToString();
                        continue;
                    }
                }
            }
            else if(panel.fixtureGizmos == 2)
            {
                foreach(DMXListItem fixture in dmxLights)
                {
                    try{
                        if(fixture != null)
                        {
                            Vector3 pos = fixture.light.transform.position;
                            pos.y+= 0.1f;
                            pos.x -= 0.05f;
                            Handles.Label(pos, "U: " + fixture.light._GetUniverse() +"\n" + "CH: " + fixture.light._GetDMXChannel(), SceneLabel());
                        }
                    }
                    catch(MissingReferenceException e)
                    {
                        e.ToString();
                        continue;
                    }
                }

            }
        }
     // Do your drawing here using GUI.
     Handles.EndGUI();    
    }
}
#endif