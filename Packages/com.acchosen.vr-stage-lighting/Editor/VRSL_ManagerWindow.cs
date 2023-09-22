using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System.Threading;
using VRSL;

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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine.UIElements;
using System.IO;
#endif
#if !COMPILER_UDONSHARP && UNITY_EDITOR


namespace VRSL.EditorScripts
{

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


public class DMXListItem
{
    public VRStageLighting_DMX_Static light;
    public bool foldout;
    ///////////////////////////////////////////////////////////////////////
    private bool Z_enableDMXChannels; public bool P_enableDMXChannels; 
    private int Z_fixtureID; public int P_fixtureID;
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
    private bool Z_nineUniverseMode; public bool P_nineUniverseMode;
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
        Z_fixtureID = P_fixtureID = this.light.fixtureID;
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
        Z_nineUniverseMode = P_nineUniverseMode = this.light.nineUniverseMode;
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
        try
        {       
            if(closeMenus)
            {
                var so = new SerializedObject(light);
                so.FindProperty("foldout").boolValue = false;
                so.ApplyModifiedProperties();
            }
        }
        catch(ArgumentException e)
        {
            e.GetType();
        }
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.UpdateProxy();
        #pragma warning restore 0618 //suppressing obsoletion warnings
        light.enableDMXChannels = P_enableDMXChannels = Z_enableDMXChannels;
        light.fixtureID = P_fixtureID = Z_fixtureID;
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
        light.nineUniverseMode = P_nineUniverseMode = Z_nineUniverseMode;
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
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.ApplyProxyModifications();
        #pragma warning restore 0618 //suppressing obsoletion warnings
    }
    public void ApplyChanges()
    {
      //  Undo.RecordObject(light, "Undo Apply Changes");
      //  PrefabUtility.RecordPrefabInstancePropertyModifications(light);
        try{
        var so = new SerializedObject(light);
        so.FindProperty("enableDMXChannels").boolValue = P_enableDMXChannels;
        so.FindProperty("fixtureID").intValue = P_fixtureID;
        so.FindProperty("dmxChannel").intValue = P_dmxChannel;
        so.FindProperty("useLegacySectorMode").boolValue = P_useLegacySectorMode;
        so.FindProperty("singleChannelMode").boolValue = P_singleChannelMode;
        so.FindProperty("sector").intValue = P_sector; 
        so.FindProperty("Channel").intValue = P_Channel;
        so.FindProperty("legacyGoboRange").boolValue = P_legacyGoboRange;
        so.FindProperty("globalIntensity").floatValue = P_globalIntensity;
        so.FindProperty("finalIntensity").floatValue = P_finalIntensity;
        so.FindProperty("lightColorTint").colorValue = P_lightColorTint;
        so.FindProperty("nineUniverseMode").boolValue = P_nineUniverseMode;
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
        
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.UpdateProxy();
        #pragma warning restore 0618 //suppressing obsoletion warnings
        light.enableDMXChannels = Z_enableDMXChannels = P_enableDMXChannels;
        light.fixtureID = Z_fixtureID = P_fixtureID;
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
        light.nineUniverseMode = Z_nineUniverseMode = P_nineUniverseMode;
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
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.ApplyProxyModifications();
        #pragma warning restore 0618 //suppressing obsoletion warnings
        if(PrefabUtility.IsPartOfAnyPrefab(light))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(light);
        }
        }
        catch(Exception e)
        {
            e.GetType();
        }
    }
}
public class AudioLinkListItem
{
    public VRStageLighting_AudioLink_Static light;
    public bool foldout;
    public bool isLaser;
    public VRStageLighting_AudioLink_Laser laser;
    //////////////////////////////////////////////////////////////////////////
    private bool Z_enableAudioLink; public bool P_enableAudioLink;
    private AudioLinkBandState Z_band; public AudioLinkBandState P_band;
    private int Z_delay; public int P_delay;
    private float Z_bandMultiplier; public float P_bandMultiplier;
    private bool Z_enableColorChord; public bool P_enableColorChord;
    private float Z_globalIntensity; public float P_globalIntensity;
    private float Z_finalIntensity; public float P_finalIntensity;
    private Color Z_lightColorTint; public Color P_lightColorTint;
    private bool Z_enableColorTextureSampling; public bool P_enableColorTextureSampling;
    private bool Z_enableThemeColorSampling; public bool P_enableThemeColorSampling;
    private Vector2 Z_textureSamplingCoordinates; public Vector2 P_textureSamplingCoordinates;
    private int Z_themeColorTarget; public int P_themeColorTarget;
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
        Z_enableThemeColorSampling = P_enableThemeColorSampling = this.light.ThemeColorSampling;
        Z_themeColorTarget = P_themeColorTarget = this.light.ThemeColorTarget;
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
        Z_enableThemeColorSampling = P_enableThemeColorSampling = this.laser.ThemeColorSampling;
        Z_themeColorTarget = P_themeColorTarget = this.laser.ThemeColorTarget;
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
        try
        {
            if(closeMenus)
            {
                    var so = new SerializedObject(light);
                    so.FindProperty("foldout").boolValue = false;
                    so.ApplyModifiedProperties();
            }
        }
        catch(ArgumentException e)
        {
            e.GetType();
        }

        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.UpdateProxy();
        #pragma warning restore 0618 //suppressing obsoletion warnings
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
        light.ThemeColorSampling = P_enableThemeColorSampling = Z_enableThemeColorSampling;
        light.ThemeColorTarget = P_themeColorTarget = Z_themeColorTarget;
        light.targetToFollow = P_targetToFollow = Z_targetToFollow;
        light.ProjectionSpin = P_enableAutoSpin = Z_enableAutoSpin;
        light.SpinSpeed = P_spinSpeed = Z_spinSpeed;
        light.SelectGOBO = P_selectGOBO = Z_selectGOBO;
        light.ConeWidth = P_coneWidth = Z_coneWidth;
        light.ConeLength = P_coneLength = Z_coneLength;
        light.MaxConeLength = P_maxConeLength = Z_maxConeLength;
        light.foldout = false;
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.ApplyProxyModifications();
        #pragma warning restore 0618 //suppressing obsoletion warnings
    }
    public void ResetChanges(VRStageLighting_AudioLink_Laser li , bool closeMenus)
    {
        try
        {
            if(closeMenus)
            {
                var so = new SerializedObject(laser);
                so.FindProperty("foldout").boolValue = false;
                so.ApplyModifiedProperties();
            }
        }
        catch(ArgumentException e)
        {
            e.GetType();
        }
        #pragma warning disable 0618 //suppressing obsoletion warnings
        laser.UpdateProxy();
        #pragma warning restore 0618 //suppressing obsoletion warnings
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
        laser.ThemeColorSampling = P_enableThemeColorSampling = Z_enableThemeColorSampling;
        laser.ThemeColorTarget = P_themeColorTarget = Z_themeColorTarget;
        laser.ConeXRotation = P_coneXRotation = Z_coneXRotation;
        laser.ConeYRotation = P_coneYRotation = Z_coneYRotation;
        laser.ConeZRotation = P_coneZRotation = Z_coneZRotation;
        laser.LaserCount = P_laserCount = Z_laserCount;
        laser.ConeWidth = P_coneWidth = Z_coneWidth;
        laser.ConeLength = P_coneLength = Z_coneLength;
        laser.LaserScroll = P_laserScroll = Z_laserScroll;
        laser.foldout = false;
        #pragma warning disable 0618 //suppressing obsoletion warnings
        laser.ApplyProxyModifications();
        #pragma warning restore 0618 //suppressing obsoletion warnings
    }

    public void ApplyChanges(VRStageLighting_AudioLink_Static li)
    {
      //  Undo.RecordObject(light, "Undo Apply Changes");
      //  PrefabUtility.RecordPrefabInstancePropertyModifications(light);

        var so = new SerializedObject(light);
        so.FindProperty("enableAudioLink").boolValue = P_enableAudioLink;
        so.FindProperty("band").enumValueIndex = (int) P_band;
        so.FindProperty("delay").intValue = P_delay;
        so.FindProperty("bandMultiplier").floatValue = P_bandMultiplier;
        so.FindProperty("enableColorChord").boolValue = P_enableColorChord; 
        so.FindProperty("globalIntensity").floatValue = P_globalIntensity;
        so.FindProperty("finalIntensity").floatValue = P_finalIntensity;
        so.FindProperty("lightColorTint").colorValue = P_lightColorTint;
        so.FindProperty("enableColorTextureSampling").boolValue = P_enableColorTextureSampling;
        so.FindProperty("textureSamplingCoordinates").vector2Value = P_textureSamplingCoordinates;
        so.FindProperty("enableThemeColorSampling").boolValue = P_enableThemeColorSampling;
        so.FindProperty("themeColorTarget").intValue = P_themeColorTarget;
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

    
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.UpdateProxy();
        #pragma warning restore 0618 //suppressing obsoletion warnings
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
        light.ThemeColorSampling = Z_enableThemeColorSampling = P_enableThemeColorSampling;
        light.ThemeColorTarget = Z_themeColorTarget = P_themeColorTarget;
        light.targetToFollow = Z_targetToFollow = P_targetToFollow;
        light.ProjectionSpin = Z_enableAutoSpin = P_enableAutoSpin;
        light.SpinSpeed = Z_spinSpeed = P_spinSpeed;
        light.SelectGOBO = Z_selectGOBO = P_selectGOBO;
        light.ConeWidth = Z_coneWidth = P_coneWidth;
        light.ConeLength = Z_coneLength = P_coneLength;
        light.MaxConeLength = Z_maxConeLength = P_maxConeLength;
        light.foldout = foldout;
        #pragma warning disable 0618 //suppressing obsoletion warnings
        light.ApplyProxyModifications();
        #pragma warning restore 0618 //suppressing obsoletion warnings
        if(PrefabUtility.IsPartOfAnyPrefab(light))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(light);
        }
    }

    public void ApplyChanges(VRStageLighting_AudioLink_Laser li)
    {
      //  Undo.RecordObject(light, "Undo Apply Changes");
      //  PrefabUtility.RecordPrefabInstancePropertyModifications(light);

        var so = new SerializedObject(laser);
        so.FindProperty("enableAudioLink").boolValue = P_enableAudioLink;
        so.FindProperty("band").enumValueIndex = (int) P_band;
        so.FindProperty("delay").intValue = P_delay;
        so.FindProperty("bandMultiplier").floatValue = P_bandMultiplier;
        so.FindProperty("enableColorChord").boolValue = P_enableColorChord; 
        so.FindProperty("globalIntensity").floatValue = P_globalIntensity;
        so.FindProperty("finalIntensity").floatValue = P_finalIntensity;
        so.FindProperty("lightColorTint").colorValue = P_lightColorTint;
        so.FindProperty("enableColorTextureSampling").boolValue = P_enableColorTextureSampling;
        so.FindProperty("textureSamplingCoordinates").vector2Value = P_textureSamplingCoordinates;
        so.FindProperty("enableThemeColorSampling").boolValue = P_enableThemeColorSampling;
        so.FindProperty("themeColorTarget").intValue = P_themeColorTarget;

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

    
        #pragma warning disable 0618 //suppressing obsoletion warnings
        laser.UpdateProxy();
        #pragma warning restore 0618 //suppressing obsoletion warnings
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
        laser.ThemeColorSampling = Z_enableThemeColorSampling = P_enableThemeColorSampling;
        laser.ThemeColorTarget = Z_themeColorTarget = P_themeColorTarget;

        laser.ConeXRotation = Z_coneXRotation = P_coneXRotation;
        laser.ConeYRotation = Z_coneYRotation = P_coneYRotation;
        laser.ConeZRotation = Z_coneZRotation = P_coneZRotation;

        laser.LaserCount = Z_laserCount = P_laserCount;
        laser.ConeWidth = Z_coneWidth = P_coneWidth;
        laser.ConeLength = Z_coneLength = P_coneLength;
        laser.LaserScroll = Z_laserScroll = P_laserScroll;
        laser.LaserThickness = Z_laserThickness = P_laserThickness;
        laser.foldout = foldout;
        #pragma warning disable 0618 //suppressing obsoletion warnings
        laser.ApplyProxyModifications();
        #pragma warning restore 0618 //suppressing obsoletion warnings

        if(PrefabUtility.IsPartOfAnyPrefab(light))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(light);
        }
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
public class VRSL_ManagerWindow : EditorWindow {
    static float panRangeOff = 180f;
    static float tiltRangeOff = -180f;
    public static Texture logo, github, twitter, discord;
    public bool legacyFixtures;
    //public static string ver = "VR Stage Lighting ver:" + " <b><color=#6a15ce> 2.4.1</color></b>";

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
    private static List<DMXListItem>[] universes =  new List<DMXListItem>[9];
    private static bool[] universeFold = new bool[9];
    private static bool universeFourFold;
    private static bool[] bandFold = new bool[4];
    static GUIContent colorLabel;
    private string[] dmxModes = new string[]{"Horizontal", "Vertical", "Legacy"};
    private string[] materialChooserList = new string[]{"Off","Intensity+Color", "Movement", "Spin", "Strobe Timing", "Strobe Output", "AudioLink Interpolation"};
    private string [] dmxGizmoInfo = new string[]{"None", "Channel Only", "Universe + Channel"};
    private static UnityEngine.Object controlPanelUiPrefab, directionalLightPrefab, uDesktopHorizontalPrefab, uDesktopVerticalPrefab, uDesktopLegacyPrefab, uVidHorizontalPrefab, uVidVerticalPrefab, uVidLegacyPrefab,
    audioLinkPrefab, audioLinkControllerPrefab, standardAudioLinkControllerPrefab, oscGridReaderHorizontalPrefab, oscGridReaderVerticalPrefab, audioLinkVRSLPrefab, cubeMask, cylinderMask, capsuleMask, sphereMask;
    private static bool dmxSpawnsFoldout, audioLinkSpawnsFoldout, mainOptionsFoldout, stencilFoldout;
    Vector2 dmxScrollPos, audioLinkScrollPos, mainScrollPos;
    private static bool dmxHover, audioLinkHover;
    private static bool last9UniverseStatus;
    private static bool lastDepthLightRequirement, lastVolumetricNoiseToggle;

    private static UnityEngine.Object spotlight_h, spotlight_v, spotlight_l, spotlight_a;
    private static UnityEngine.Object washlight_h, washlight_v, washlight_l, washlight_a;
    private static UnityEngine.Object blinder_h, blinder_v, blinder_l, blinder_a;
    private static UnityEngine.Object flasher_h, flasher_v, flasher_l, flasher_a;
    private static UnityEngine.Object parlight_h, parlight_v, parlight_l, parlight_a;
    private static UnityEngine.Object lightbar_h, lightbar_v, lightbar_l, lightbar_a;
    private static UnityEngine.Object discoball_h, discoball_v, discoball_l, discoball_a;
    private static UnityEngine.Object laser_h, laser_v, laser_l, laser_a;
    private static UnityEngine.Object sixFour_h, sixFour_v, sixFour_l;
    private static UnityEngine.Object multiLightbar_h, multiLightbar_v, multiLightbar_l;
    private Material dmx_H_CRT_Color_Mat, dmx_V_CRT_Color_Mat, dmx_L_CRT_Color_Mat,
    dmx_H_CRT_Mvmt_Mat, dmx_V_CRT_Mvmt_Mat, dmx_L_CRT_Mvmt_Mat,
    dmx_H_CRT_Spin_Mat,dmx_V_CRT_Spin_Mat,dmx_L_CRT_Spin_Mat,
    dmx_H_CRT_StrobeTime_Mat, dmx_V_CRT_StrobeTime_Mat, dmx_L_CRT_StrobeTime_Mat,
    dmx_H_CRT_StrobeOut_Mat, dmx_V_CRT_StrobeOut_Mat, dmx_L_CRT_StrobeOut_Mat,
    audiolink_CRT_InterpolationMat;

    private MaterialEditor _materialEditor; 
    private bool showMaterialEditor;

    private static DMXListItem copyDMXListProx;
    private static AudioLinkListItem copyAudioLinkListProx;

    private static Shader GIDMXLightTextureShader;
    private static bool hasDMXGI;

    private int materialChooser;
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

                last9UniverseStatus = panel.useExtendedUniverses;
                lastDepthLightRequirement = panel.RequireDepthLight;
                lastVolumetricNoiseToggle = panel.VolumetricNoise;
                MassHideShowProjections();
                panel._CheckDepthLightStatus();   
            }    
            LoadPrefabs();
            //Debug.Log("VRSL Control Panel: Initializing!");
            ApplyChangesToFixtures(true, true, false);
        }

        hasDMXGI = CheckIfDMXGIAvailable();


        window.minSize = new Vector2(925f, 700f);
       // window.position = Rect.zero;
        Extensions.CenterOnMainWin(window);
        window.Show();
        //EditorApplication.playModeStateChanged += LogPlayModeState;
        

    }

    public static string GetVersion()
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

    static bool CheckIfDMXGIAvailable()
    {
        string path = "Packages/com.acchosen.vrsl-dmx-gi/Runtime/Shaders/VRSL_GI_LightTexture.shader";
        bool wasSuccessful = false;
        try{
            GIDMXLightTextureShader = (Shader)AssetDatabase.LoadAssetAtPath(path, typeof(Shader));
            wasSuccessful = true;
        }
        catch(Exception e)
        {
            e.GetType();
            wasSuccessful = false;
        }
        return wasSuccessful;
    }


    // static VRSL_ManagerWindow()
    // {
    //     base.
       
    // }

    static bool LoadPrefabs()
    {
        bool result = true;
        // string controlPanelPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/VRSL-LocalUIControlPanel.prefab";
        // string directionalLightPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/Directional Light (For Depth).prefab";
        // string udeskHorizontalPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Horizontal Mode/DMX Reader Screens/VRSL-DMX-uDesktopDuplicationReaderScreen-Horizontal.prefab";
        // string udeskVerticalPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Vertical Mode/DMX Reader Screens/VRSL-DMX-uDesktopDuplicationReaderScreen-Vertical.prefab";
        // string udeskLegacyPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Legacy Mode/DMX Reader Screens/VRSL-DMX-uDesktopDuplicationReader-Legacy.prefab";
        // string vidHorizontalPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Horizontal Mode/DMX Reader Screens/VRSL-DMX-USharpVideoReaderScreen-Horizontal.prefab";
        // string vidVerticalPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Vertical Mode/DMX Reader Screens/VRSL-DMX-USharpVideoReaderScreen-Vertical.prefab";
        // string vidLegacyPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Legacy Mode/DMX Reader Screens/VRSL-DMX-USharpVideoReaderScreen-Legacy.prefab";
        // string audioLinkPath = "Assets/AudioLink/Audiolink.prefab";
        // string audioLinkPathAlt = "Packages/com.llealloo.audiolink/Runtime/Audiolink.prefab";
        // string audioLinkControllerPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/AudioLink/VRSL-AudioLinkControllerWithSmoothing/AudioLinkController-WithVRSLSmoothing.prefab";
        // string standardAudioLinkControllerPath = "Assets/AudioLink/AudioLinkController.prefab";
        // string standardAudioLinkControllerPathAlt = "Packages/com.llealloo.audiolink/Runtime/AudioLinkController.prefab";
        // string oscGridReadHPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/GridReader/VRSL-DMX-TekOSCGridReader-H.prefab";
        // string oscGridReadVPath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/GridReader/VRSL-DMX-TekOSCGridReader-V.prefab";
        // controlPanelUiPrefab = AssetDatabase.LoadAssetAtPath(controlPanelPath, typeof(GameObject));
        // directionalLightPrefab = AssetDatabase.LoadAssetAtPath(directionalLightPath, typeof(GameObject));
        // oscGridReaderHorizontalPrefab = AssetDatabase.LoadAssetAtPath(oscGridReadHPath, typeof(GameObject));
        // oscGridReaderVerticalPrefab = AssetDatabase.LoadAssetAtPath(oscGridReadVPath, typeof(GameObject));
        // uDesktopHorizontalPrefab = AssetDatabase.LoadAssetAtPath(udeskHorizontalPath, typeof(GameObject));
        // uDesktopVerticalPrefab = AssetDatabase.LoadAssetAtPath(udeskVerticalPath, typeof(GameObject));
        // uDesktopLegacyPrefab = AssetDatabase.LoadAssetAtPath(udeskLegacyPath, typeof(GameObject));
        // uVidHorizontalPrefab = AssetDatabase.LoadAssetAtPath(vidHorizontalPath, typeof(GameObject));
        // uVidVerticalPrefab = AssetDatabase.LoadAssetAtPath(vidVerticalPath, typeof(GameObject));
        // uVidLegacyPrefab = AssetDatabase.LoadAssetAtPath(vidLegacyPath, typeof(GameObject));
        // audioLinkPrefab = AssetDatabase.LoadAssetAtPath(audioLinkPath, typeof(GameObject));
        // audioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(audioLinkControllerPath, typeof(GameObject));
        // standardAudioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(standardAudioLinkControllerPath, typeof(GameObject));
        controlPanelUiPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("dedfba01424b93148b3d9a42e95ed2f7"), typeof(GameObject));
        directionalLightPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("dc2b8d13712a0f3488413e49afae73ef"), typeof(GameObject));
        oscGridReaderHorizontalPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("f37d8aba2d9398d4aa2fc86f7d8c0cd4"), typeof(GameObject));
        oscGridReaderVerticalPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("2ed7323806c632f4294ff51d8af9a2b2"), typeof(GameObject));
        uDesktopHorizontalPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("d8bedad6090068740a8d3d9de3c84ea4"), typeof(GameObject));
        uDesktopVerticalPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("b55309c416955b044a48bbced977331d"), typeof(GameObject));
        uDesktopLegacyPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("a6013d8268c98274386159517c50aa09"), typeof(GameObject));
        uVidHorizontalPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("e62fe963d8da32147bbd2f1caa73a0de"), typeof(GameObject));
        uVidVerticalPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("91a099dcf3cff864a9bc08584904554a"), typeof(GameObject));
        uVidLegacyPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("dd476adc52e8f374da7dd7e4e9274f71"), typeof(GameObject));
        audioLinkPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("8c1f201f848804f42aa401d0647f8902"), typeof(GameObject));
        audioLinkVRSLPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("c341366b553ad354198357faa3169627"), typeof(GameObject));
        audioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("7156b82dc87d72144bdec0dc4f32a78a"), typeof(GameObject));
        standardAudioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("385ac04e8d2b6f84ea93cb8392fad970"), typeof(GameObject));
        cubeMask = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("14fd9d71e91903d4eb44d5289d78eba4"), typeof(GameObject));
        cylinderMask = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("de5b252ed1e43b14a9f655ac98144fff"), typeof(GameObject));
        capsuleMask = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("29158ac9808b32541ba787435bdfa109"), typeof(GameObject));
        sphereMask = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("fa0972d9c823bcf4293febe61ff79da6"), typeof(GameObject));

        if(controlPanelUiPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("dedfba01424b93148b3d9a42e95ed2f7"));
            result = false;
        }
        if(directionalLightPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("dc2b8d13712a0f3488413e49afae73ef"));
            result = false;
        }
        if(oscGridReaderHorizontalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load  " + AssetDatabase.GUIDToAssetPath("f37d8aba2d9398d4aa2fc86f7d8c0cd4"));
            result = false;
        }
        if(oscGridReaderVerticalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load  " + AssetDatabase.GUIDToAssetPath("2ed7323806c632f4294ff51d8af9a2b2"));
            result = false;
        }
        if(uDesktopHorizontalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load  " + AssetDatabase.GUIDToAssetPath("d8bedad6090068740a8d3d9de3c84ea4"));
            result = false;
        }
        if(uDesktopVerticalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("b55309c416955b044a48bbced977331d"));
            result = false;
        }
        if(uDesktopLegacyPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("a6013d8268c98274386159517c50aa09"));
            result = false;
        }
        if(uVidHorizontalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("e62fe963d8da32147bbd2f1caa73a0de"));
            result = false;
        }
        if(uVidVerticalPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("91a099dcf3cff864a9bc08584904554a"));
            result = false;
        }
        if(uVidLegacyPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("dd476adc52e8f374da7dd7e4e9274f71"));
            result = false;
        }
        if(audioLinkPrefab == null)
        {
            // audioLinkPrefab = AssetDatabase.LoadAssetAtPath(audioLinkPathAlt, typeof(GameObject));
            // if(audioLinkPrefab == null)
            // {
                Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("8c1f201f848804f42aa401d0647f8902"));
                result = false;
            // }
        }
        if(audioLinkVRSLPrefab == null)
        {
            // audioLinkPrefab = AssetDatabase.LoadAssetAtPath(audioLinkPathAlt, typeof(GameObject));
            // if(audioLinkPrefab == null)
            // {
                Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("c341366b553ad354198357faa3169627"));
                result = false;
            // }
        }
        if(audioLinkControllerPrefab == null)
        {
            Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("7156b82dc87d72144bdec0dc4f32a78a"));
            result = false;
        }
        if(standardAudioLinkControllerPrefab == null)
        {
            // standardAudioLinkControllerPrefab = AssetDatabase.LoadAssetAtPath(standardAudioLinkControllerPathAlt, typeof(GameObject));
            // if(standardAudioLinkControllerPrefab == null)
            // {
                Debug.LogError("VRSL Control Panel: Failed to load " + AssetDatabase.GUIDToAssetPath("385ac04e8d2b6f84ea93cb8392fad970"));
                result = false;
            // }
        }
        return result;
    }

    
    private static void CheckForLocalPanel()
    {
        hasLocalPanel = false;
        panel = null;
        colorLabel = new GUIContent();
        colorLabel.text = "Emission Color";
        foreach (GameObject go in sceneObjects)
        {
           #pragma warning disable 0618 //suppressing obsoletion warnings
           panel =  go.GetUdonSharpComponent<VRSL_LocalUIControlPanel>();
           #pragma warning restore 0618
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
                #pragma warning disable 0618 //suppressing obsoletion warnings
                VRStageLighting_DMX_Static lightScript = go.GetUdonSharpComponent<VRStageLighting_DMX_Static>();
                #pragma warning restore 0618 //suppressing obsoletion warnings
                if(lightScript != null)
                {
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
                if(panel != null)
                {
                    if(fixture.light.nineUniverseMode != panel.useExtendedUniverses)
                    {
                        fixture.P_nineUniverseMode = panel.useExtendedUniverses;
                        //fixture.light.nineUniverseMode = last9UniverseStatus;
                        fixture.ApplyChanges();

                    }
                }
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
                #pragma warning disable 0618 //suppressing obsoletion warnings
                VRStageLighting_AudioLink_Static audioScript = go.GetUdonSharpComponent<VRStageLighting_AudioLink_Static>();
                #pragma warning restore 0618 //suppressing obsoletion warnings
                if(audioScript != null)
                {
                    audioLinkLights.Add(
                        new AudioLinkListItem(audioScript, audioScript.foldout)
                    );
                    continue;
                }
                #pragma warning disable 0618 //suppressing obsoletion warnings
                VRStageLighting_AudioLink_Laser laserScript = go.GetUdonSharpComponent<VRStageLighting_AudioLink_Laser>();
                #pragma warning restore 0618 //suppressing obsoletion warnings
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
    public static void DrawLogo()
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
        public static GUIStyle Title4()
    {
        GUIStyle g = new GUIStyle(EditorStyles.label);
        g.fontSize = 14;
       // g.fixedHeight = 35;
      //  g.fontStyle = FontStyle.Italic;
        g.normal.textColor = Color.white;
        g.alignment = TextAnchor.MiddleLeft;
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
            if(isPan)
            {
                panRangeOff = range;
            }
            else
            {
                tiltRangeOff = range;
            }
        }
    }

    static int GetFixtureCount()
    {
        int fixtureCount = 0;
            for(int i = 0; i < universes.Length; i++)
            {
                if(universes[i] == null)
                {
                    continue;
                }
                foreach(DMXListItem fixture in universes[i])
                {
                    fixtureCount++;
                }
            }
        return fixtureCount;
    }
    static int GetAudioLinkFixtureCount()
    {
        int fixtureCount = 0;
        foreach(AudioLinkListItem fixture in audioLinkLights)
        {
            fixtureCount++;
        }
        return fixtureCount;
    }
    void OnEnable() 
    {
     //EditorApplication.playModeStateChanged += LogPlayModeState;
     EditorApplication.hierarchyChanged += HierarchyChanged;
   //  EditorApplication.playModeStateChanged += LogPlayModeState;
     SceneView.duringSceneGui += this.OnSceneGUI;
     LoadMaterials();
     hasDMXGI = CheckIfDMXGIAvailable();
    }
    void OnDisable( )
    {
        //EditorApplication.playModeStateChanged -= LogPlayModeState;
        EditorApplication.hierarchyChanged -= HierarchyChanged;
        //EditorApplication.playModeStateChanged -= LogPlayModeState;
        if((panel != null))
        {
            if(panel.fixtureGizmos == 0)
                SceneView.duringSceneGui -= this.OnSceneGUI;
        }
                if (_materialEditor != null) 
                {
            // Free the memory used by default MaterialEditor
                DestroyImmediate (_materialEditor);
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
    static void MassApplyExtendedUniverseStatus()
    {
        if(panel == null)
        {
            Debug.Log("Panel not found!");
            return;
        }
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
                if(fixture.P_nineUniverseMode != last9UniverseStatus)
                {
                    fixture.P_nineUniverseMode = last9UniverseStatus;
                }
            }
        }
        Debug.Log("Finished Updating Extended Universe Status!");
    }
    static void MassHideShowProjections()
    {
        if(panel == null)
        {
            Debug.Log("Panel not found!");
            return;
        }
        if(panel.isUsingDMX)
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
                    foreach(MeshRenderer rend in fixture.light.objRenderers)
                    {
                        if(rend.gameObject.name.Contains("Projection"))
                        {
                            if(rend.gameObject.name.Contains("Fixture"))
                            {
                                continue;
                            }
                            rend.gameObject.SetActive(lastDepthLightRequirement);
                            continue;
                        }
                        if(rend.gameObject.name.Contains("Disco"))
                        {
                            rend.gameObject.SetActive(lastDepthLightRequirement);
                            continue;                        
                        }
                    }
                }
            }
        }
        if(panel.isUsingAudioLink)
        {
            foreach(AudioLinkListItem fixture in audioLinkLights)
            {
                if(!fixture.isLaser)
                {
                    if(fixture.light!=null)
                    {
                        foreach(MeshRenderer rend in fixture.light.objRenderers)
                        {
                            if(rend.gameObject.name.Contains("Projection"))
                            {
                                if(rend.gameObject.name.Contains("Fixture"))
                                {
                                    continue;
                                }
                                rend.gameObject.SetActive(lastDepthLightRequirement);
                                continue;
                            }
                            if(rend.gameObject.name.Contains("Disco"))
                            {
                                rend.gameObject.SetActive(lastDepthLightRequirement);
                                continue;                        
                            }
                        }
                    }
                }
            }
        }
        if(hasDepthLight && depthLight != null)
        {
            depthLight.gameObject.SetActive(lastDepthLightRequirement);
        }
    }
    public class FixturePrefabID
    {
        public int prefabID;
        public List<DMXListItem> fixtures = new List<DMXListItem>();
        public FixturePrefabID(int p_ID)
        {
            prefabID = p_ID;
        }
        public void AssignIDs()
        {
            int id = 0;
            foreach(DMXListItem fixture in fixtures)
            {
                if(fixture == null)
                {
                    continue;
                }
                if(fixture.light == null)
                {
                    continue;
                }
                fixture.P_fixtureID = prefabID + id;
                id++;
            }
        }
    }
    static void MassApplyIDs()
    {
        int prefabID = 100;
       // int nullPrefabID = 0;
        Dictionary<UnityEngine.Object, FixturePrefabID> prefabDict = new Dictionary<UnityEngine.Object, FixturePrefabID>();
        for(int i = 0; i < universes.Length; i++)
        {
            if(universes[i] == null)
            {
                continue;
            }
            foreach(DMXListItem fixture in universes[i])
            {
                //Find the prefab this fixture is connected to
                UnityEngine.Object prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(fixture.light.gameObject);
                //If the prefab is in the dictionary...
                if(prefabDict.ContainsKey(prefab))
                {
                    //add the current fixture to the list in the struct key of the dictionary
                    prefabDict[prefab].fixtures.Add(fixture);
                }
                //otherwise...
                else
                {
                    
                    ///Create a new dictionary entry with the prefab origin as the key, and a new struct with the a new base prefab ID
                    if(prefab == null)
                    {
                        prefabDict.Add(prefab, new FixturePrefabID(0));
                    }
                    else
                    {
                        prefabDict.Add(prefab, new FixturePrefabID(prefabID));
                    }
                    //increment the base ID by 100
                    prefabID += 100;
                    //then add the fixture to the list in the struct key of the dictionary
                    prefabDict[prefab].fixtures.Add(fixture);

                }
            }
        }
        foreach(var prefabEntry in prefabDict)
        {
            prefabEntry.Value.AssignIDs();
        }
    }

    private static void SpawnPrefabWithUndo(UnityEngine.Object obj, string undoMessage, bool isFixture, bool isAudioLinkFixture)
    {
        GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(obj as GameObject);
        Undo.RegisterCreatedObjectUndo (instance, undoMessage);
        Selection.SetActiveObjectWithContext(instance, null);
        //MassHideShowProjections();
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
        try{
       //GUIStyle g = GUIStyle.none;
       //g.fixedHeight = 6;
       Rect rect = EditorGUILayout.GetControlRect(false, i_height);

       rect.height = i_height;

       EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
        }
        catch(Exception e)
        {
            e.GetType();
        }

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
    private void CopyPasteFunctionAudioLinkNoBandLaser(AudioLinkListItem copyTofixture, AudioLinkListItem copyFromfixture)
    {
        if(panel == null)
        {
            return;
        }
        if(copyTofixture.laser != null)
        {
            copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
            copyTofixture.P_coneFlatness = copyFromfixture.P_coneFlatness;
            copyTofixture.P_coneXRotation = copyFromfixture.P_coneXRotation;
            copyTofixture.P_coneYRotation = copyFromfixture.P_coneYRotation;
            copyTofixture.P_coneZRotation = copyFromfixture.P_coneZRotation;
            copyTofixture.P_laserCount = copyFromfixture.P_laserCount;
            copyTofixture.P_laserScroll = copyFromfixture.P_laserScroll;
            copyTofixture.P_laserThickness = copyFromfixture.P_laserThickness;
        }
       // copyTofixture.P_band = copyFromfixture.P_band;
        copyTofixture.P_coneLength = copyFromfixture.P_coneLength;
       // copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
       // copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
        copyTofixture.P_bandMultiplier = copyFromfixture.P_bandMultiplier;
        copyTofixture.P_finalIntensity = copyFromfixture.P_finalIntensity;
        copyTofixture.P_globalIntensity = copyFromfixture.P_globalIntensity;
       // copyTofixture.P_coneFlatness = copyFromfixture.P_coneFlatness;
       // copyTofixture.P_coneXRotation = copyFromfixture.P_coneXRotation;
       // copyTofixture.P_coneYRotation = copyFromfixture.P_coneYRotation;
        //copyTofixture.P_coneZRotation = copyFromfixture.P_coneZRotation;
        copyTofixture.P_lightColorTint = copyFromfixture.P_lightColorTint;
       // copyTofixture.P_maxConeLength = copyFromfixture.P_maxConeLength;
        //copyTofixture.P_delay = copyFromfixture.P_delay;
        copyTofixture.P_enableAudioLink = copyFromfixture.P_enableAudioLink;
        copyTofixture.P_enableColorChord = copyFromfixture.P_enableColorChord;
        //copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
      //  copyTofixture.P_selectGOBO = copyFromfixture.P_selectGOBO;
       // copyTofixture.P_laserCount = copyFromfixture.P_laserCount;
      //  copyTofixture.P_laserScroll = copyFromfixture.P_laserScroll;
      //  copyTofixture.P_laserThickness = copyFromfixture.P_laserThickness;
        copyTofixture.P_textureSamplingCoordinates = copyFromfixture.P_textureSamplingCoordinates;
        copyTofixture.P_themeColorTarget = copyFromfixture.P_themeColorTarget;
        //copyTofixture.P_spinSpeed = copyFromfixture.P_spinSpeed;
        Debug.Log("VRSL Control Panel: Applying Changes!");
        ApplyChangesToFixtures(true, false, false);
        ResetItems(false);
        Repaint();
    }
    private void CopyPasteFunctionAudioLinkNoBandFixture(AudioLinkListItem copyTofixture, AudioLinkListItem copyFromfixture)
    {
        if(panel == null)
        {
            return;
        }
        if(copyTofixture.light != null)
        {
            copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
            copyTofixture.P_coneLength = copyFromfixture.P_coneLength;
            copyTofixture.P_maxConeLength = copyFromfixture.P_maxConeLength;
            copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
            copyTofixture.P_selectGOBO = copyFromfixture.P_selectGOBO;
            copyTofixture.P_spinSpeed = copyFromfixture.P_spinSpeed;
        }
       // copyTofixture.P_band = copyFromfixture.P_band;


       // copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
        copyTofixture.P_bandMultiplier = copyFromfixture.P_bandMultiplier;
        copyTofixture.P_finalIntensity = copyFromfixture.P_finalIntensity;
        copyTofixture.P_globalIntensity = copyFromfixture.P_globalIntensity;
       // copyTofixture.P_coneFlatness = copyFromfixture.P_coneFlatness;
        //copyTofixture.P_coneXRotation = copyFromfixture.P_coneXRotation;
       // copyTofixture.P_coneYRotation = copyFromfixture.P_coneYRotation;
       // copyTofixture.P_coneZRotation = copyFromfixture.P_coneZRotation;
        copyTofixture.P_lightColorTint = copyFromfixture.P_lightColorTint;

        //copyTofixture.P_delay = copyFromfixture.P_delay;
        copyTofixture.P_enableAudioLink = copyFromfixture.P_enableAudioLink;
        copyTofixture.P_enableColorChord = copyFromfixture.P_enableColorChord;

        //copyTofixture.P_laserCount = copyFromfixture.P_laserCount;
       // copyTofixture.P_laserScroll = copyFromfixture.P_laserScroll;
       // copyTofixture.P_laserThickness = copyFromfixture.P_laserThickness;
        copyTofixture.P_textureSamplingCoordinates = copyFromfixture.P_textureSamplingCoordinates;
        copyTofixture.P_themeColorTarget = copyFromfixture.P_themeColorTarget;

        Debug.Log("VRSL Control Panel: Applying Changes!");
        ApplyChangesToFixtures(true, false, false);
        ResetItems(false);
        Repaint();
    }
        private void CopyPasteFunctionAudioLinkLaser(AudioLinkListItem copyTofixture, AudioLinkListItem copyFromfixture)
    {
        if(panel == null)
        {
            return;
        }
        if(copyTofixture.laser != null)
        {
            copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
            copyTofixture.P_coneFlatness = copyFromfixture.P_coneFlatness;
            copyTofixture.P_coneXRotation = copyFromfixture.P_coneXRotation;
            copyTofixture.P_coneYRotation = copyFromfixture.P_coneYRotation;
            copyTofixture.P_coneZRotation = copyFromfixture.P_coneZRotation;
            copyTofixture.P_laserCount = copyFromfixture.P_laserCount;
            copyTofixture.P_laserScroll = copyFromfixture.P_laserScroll;
            copyTofixture.P_laserThickness = copyFromfixture.P_laserThickness;
        }
        copyTofixture.P_band = copyFromfixture.P_band;
        copyTofixture.P_coneLength = copyFromfixture.P_coneLength;

       // copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
        copyTofixture.P_bandMultiplier = copyFromfixture.P_bandMultiplier;
        copyTofixture.P_finalIntensity = copyFromfixture.P_finalIntensity;
        copyTofixture.P_globalIntensity = copyFromfixture.P_globalIntensity;

        copyTofixture.P_lightColorTint = copyFromfixture.P_lightColorTint;
       // copyTofixture.P_maxConeLength = copyFromfixture.P_maxConeLength;
        copyTofixture.P_delay = copyFromfixture.P_delay;
        copyTofixture.P_enableAudioLink = copyFromfixture.P_enableAudioLink;
        copyTofixture.P_enableColorChord = copyFromfixture.P_enableColorChord;
        copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
        //copyTofixture.P_selectGOBO = copyFromfixture.P_selectGOBO;

        copyTofixture.P_textureSamplingCoordinates = copyFromfixture.P_textureSamplingCoordinates;
        copyTofixture.P_themeColorTarget = copyFromfixture.P_themeColorTarget;
      //  copyTofixture.P_spinSpeed = copyFromfixture.P_spinSpeed;
        Debug.Log("VRSL Control Panel: Applying Changes!");
        ApplyChangesToFixtures(true, false, false);
        ResetItems(false);
        Repaint();
    }
    private void CopyPasteFunctionAudioLinkFixture(AudioLinkListItem copyTofixture, AudioLinkListItem copyFromfixture)
    {
        if(panel == null)
        {
            return;
        }
        if(copyTofixture.light != null)
        {
            copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
            copyTofixture.P_coneLength = copyFromfixture.P_coneLength;
            copyTofixture.P_maxConeLength = copyFromfixture.P_maxConeLength;
            copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
            copyTofixture.P_selectGOBO = copyFromfixture.P_selectGOBO;
            copyTofixture.P_spinSpeed = copyFromfixture.P_spinSpeed;
        }
        copyTofixture.P_band = copyFromfixture.P_band;
      //  copyTofixture.P_coneLength = copyFromfixture.P_coneLength;
      //  copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
      //  copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
        copyTofixture.P_bandMultiplier = copyFromfixture.P_bandMultiplier;
        copyTofixture.P_finalIntensity = copyFromfixture.P_finalIntensity;
        copyTofixture.P_globalIntensity = copyFromfixture.P_globalIntensity;
       // copyTofixture.P_coneFlatness = copyFromfixture.P_coneFlatness;
       // copyTofixture.P_coneXRotation = copyFromfixture.P_coneXRotation;
        //copyTofixture.P_coneYRotation = copyFromfixture.P_coneYRotation;
       // copyTofixture.P_coneZRotation = copyFromfixture.P_coneZRotation;
        copyTofixture.P_lightColorTint = copyFromfixture.P_lightColorTint;
       // copyTofixture.P_maxConeLength = copyFromfixture.P_maxConeLength;
       // copyTofixture.P_delay = copyFromfixture.P_delay;
        copyTofixture.P_enableAudioLink = copyFromfixture.P_enableAudioLink;
        copyTofixture.P_enableColorChord = copyFromfixture.P_enableColorChord;
       // copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
       // copyTofixture.P_selectGOBO = copyFromfixture.P_selectGOBO;
     //   copyTofixture.P_laserCount = copyFromfixture.P_laserCount;
        //copyTofixture.P_laserScroll = copyFromfixture.P_laserScroll;
       // copyTofixture.P_laserThickness = copyFromfixture.P_laserThickness;
        copyTofixture.P_textureSamplingCoordinates = copyFromfixture.P_textureSamplingCoordinates;
        copyTofixture.P_themeColorTarget = copyFromfixture.P_themeColorTarget;
       // copyTofixture.P_spinSpeed = copyFromfixture.P_spinSpeed;
        Debug.Log("VRSL Control Panel: Applying Changes!");
        ApplyChangesToFixtures(true, false, false);
        ResetItems(false);
        Repaint();
    }

    private void CopyPasteFunction(DMXListItem copyTofixture, DMXListItem copyFromfixture)
    {
        if(panel == null)
        {
            return;
        }
        copyTofixture.P_enableDMXChannels = copyFromfixture.P_enableDMXChannels;
        copyTofixture.P_coneLength = copyFromfixture.P_coneLength;
        copyTofixture.P_coneWidth = copyFromfixture.P_coneWidth;
        copyTofixture.P_enableAutoSpin = copyFromfixture.P_enableAutoSpin;
        copyTofixture.P_enableStrobe = copyFromfixture.P_enableStrobe;
        copyTofixture.P_finalIntensity = copyFromfixture.P_finalIntensity;
        copyTofixture.P_globalIntensity = copyFromfixture.P_globalIntensity;
        copyTofixture.P_invertPan = copyFromfixture.P_invertPan;
        copyTofixture.P_invertTilt = copyFromfixture.P_invertTilt;
        copyTofixture.P_isUpsideDown = copyFromfixture.P_isUpsideDown;
        copyTofixture.P_legacyGoboRange = copyFromfixture.P_legacyGoboRange;
        copyTofixture.P_lightColorTint = copyFromfixture.P_lightColorTint;
        copyTofixture.P_maxConeLength = copyFromfixture.P_maxConeLength;
        copyTofixture.P_maxMinPan = copyFromfixture.P_maxMinPan;
        copyTofixture.P_maxMinTilt = copyFromfixture.P_maxMinTilt;
        copyTofixture.P_nineUniverseMode = copyFromfixture.P_nineUniverseMode;
        copyTofixture.P_panOffsetBlueGreen = copyFromfixture.P_panOffsetBlueGreen;
        copyTofixture.P_selectGOBO = copyFromfixture.P_selectGOBO;
        copyTofixture.P_tiltOffsetBlue = copyFromfixture.P_tiltOffsetBlue;
        if(panel.isUsingDMX)
        {
            MassApplyExtendedUniverseStatus();
        }
        Debug.Log("VRSL Control Panel: Applying Changes!");
        ApplyChangesToFixtures(true, false, false);
        ResetItems(false);
        Repaint();
    }

    private void LoadMaterials()
    {
        string dmx_H_CRT_Color_Mat_path = AssetDatabase.GUIDToAssetPath("c23ee34d1d3977548829651c8cceea33");
        string dmx_V_CRT_Color_Mat_path = AssetDatabase.GUIDToAssetPath("d2a0ea204b6092d49971eacf996dcec3");
        string dmx_L_CRT_Color_Mat_path = AssetDatabase.GUIDToAssetPath("9a42fdd188c84e542be2a455485423a8");

        string dmx_H_CRT_Mvmt_Mat_path = AssetDatabase.GUIDToAssetPath("144ac9f77364a7d4ea6e607f40c31505");
        string dmx_V_CRT_Mvmt_Mat_path = AssetDatabase.GUIDToAssetPath("a949afd894bf9384bb57422931f130fc");
        string dmx_L_CRT_Mvmt_Mat_path = AssetDatabase.GUIDToAssetPath("e79b2b00c4751b74ea6dacd87a9f41dd");

        string dmx_H_CRT_Spin_Mat_path = AssetDatabase.GUIDToAssetPath("0de093d844c8ac146b98341787214c64");
        string dmx_V_CRT_Spin_Mat_path = AssetDatabase.GUIDToAssetPath("1e05cea1a32288a47b1612ca4725ae2e");
        string dmx_L_CRT_Spin_Mat_path = AssetDatabase.GUIDToAssetPath("d80a528643bc1c2418a6986cd3cf0141");

        string dmx_H_CRT_StrobeTime_Mat_path = AssetDatabase.GUIDToAssetPath("742ce52797fea8948a8f4c438b0c3b69");
        string dmx_V_CRT_StrobeTime_Mat_path = AssetDatabase.GUIDToAssetPath("05d3c32dd6873684283c962951dc067a");
        string dmx_L_CRT_StrobeTime_Mat_path = AssetDatabase.GUIDToAssetPath("0f235f1aadc897344a6c8b3301b6f79e");

        string dmx_H_CRT_StrobeOut_Mat_path = AssetDatabase.GUIDToAssetPath("038cddd0ea70e1d41ad37272c1e7c31c");
        string dmx_V_CRT_StrobeOut_Mat_path = AssetDatabase.GUIDToAssetPath("fafb9a56ddc548e4dafd9cb0befa0e2e");
        string dmx_L_CRT_StrobeOut_Mat_path = AssetDatabase.GUIDToAssetPath("8af3b80e2a7dd3e458aacc6701d4c657");

        string audiolink_CRT_InterpolationMat_path = AssetDatabase.GUIDToAssetPath("91f76b2e00433a141b2ad6ada0c59a80");

        //  switch(a)
        // {
            //horizontal
            // case 0:
                try{

                    dmx_H_CRT_Color_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_H_CRT_Color_Mat_path, typeof(Material));
                    dmx_H_CRT_Mvmt_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_H_CRT_Mvmt_Mat_path, typeof(Material));
                    dmx_H_CRT_Spin_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_H_CRT_Spin_Mat_path, typeof(Material));
                    dmx_H_CRT_StrobeTime_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_H_CRT_StrobeTime_Mat_path, typeof(Material));
                    dmx_H_CRT_StrobeOut_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_H_CRT_StrobeOut_Mat_path, typeof(Material));
                }
                catch(Exception e)      
                {
                    // loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();
                }
                // break;
            //vertical
            // case 1:
                try{

                    dmx_V_CRT_Color_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_V_CRT_Color_Mat_path, typeof(Material));
                    dmx_V_CRT_Mvmt_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_V_CRT_Mvmt_Mat_path, typeof(Material));
                    dmx_V_CRT_Spin_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_V_CRT_Spin_Mat_path, typeof(Material));
                    dmx_V_CRT_StrobeTime_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_V_CRT_StrobeTime_Mat_path, typeof(Material));
                    dmx_V_CRT_StrobeOut_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_V_CRT_StrobeOut_Mat_path, typeof(Material));
                }
                catch(Exception e)
                {
                    // loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();                    
                }
                // break;
            //legacy
            // case 2:
                try{

                    dmx_L_CRT_Color_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_L_CRT_Color_Mat_path, typeof(Material));
                    dmx_L_CRT_Mvmt_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_L_CRT_Mvmt_Mat_path, typeof(Material));
                    dmx_L_CRT_Spin_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_L_CRT_Spin_Mat_path, typeof(Material));
                    dmx_L_CRT_StrobeTime_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_L_CRT_StrobeTime_Mat_path, typeof(Material));
                    dmx_L_CRT_StrobeOut_Mat = (Material)AssetDatabase.LoadAssetAtPath(dmx_L_CRT_StrobeOut_Mat_path, typeof(Material));
                }
                catch(Exception e)
                {
                    // loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();                    
                }
                // break;
            //audiolink
            // case 3:
                try{

                    audiolink_CRT_InterpolationMat = (Material)AssetDatabase.LoadAssetAtPath(audiolink_CRT_InterpolationMat_path, typeof(Material));
                }
                catch(Exception e)
                {
                    // loadSuccessful = false;
                    Debug.Log("Could not load fixture prefab!");
                    e.ToString();                    
                }
            //     break;
            // default:
            //     break;
        // }
    }

    private bool LoadFixturePrefabs(int a)
    {
        bool loadSuccessful = true;

        // string horizontalpath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Horizontal Mode/";
        // string verticalpath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Vertical Mode/";
        // string legacypath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Legacy Mode/";
        // string audiopath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/AudioLink/";

        // string spot_h_path = horizontalpath + "VRSL-DMX-Mover-Spotlight-H-13CH.prefab";
        // string spot_v_path = verticalpath + "VRSL-DMX-Mover-Spotlight-V-13CH.prefab";
        // string spot_l_path = legacypath + "VRSL-DMX-Mover-Spotlight-L-13CH.prefab";
        // string spot_a_path = audiopath + "VRSL-AudioLink-Mover-Spotlight.prefab";

        // string wash_h_path = horizontalpath + "VRSL-DMX-Mover-WashLight-H-13CH.prefab";
        // string wash_v_path = verticalpath + "VRSL-DMX-Mover-WashLight-V-13CH.prefab";
        // string wash_l_path = legacypath + "VRSL-DMX-Mover-WashLight-L-13CH.prefab";
        // string wash_a_path = audiopath + "VRSL-AudioLink-Mover-Washlight.prefab";
        // string blind_h_path, blind_v_path, blind_l_path, blind_a_path;
        // string par_h_path, par_v_path, par_l_path, par_a_path;
        // string bar_h_path, bar_v_path, bar_l_path, bar_a_path;
        // string six_h_path, six_v_path, six_l_path;
        // string multibar_h_path, multibar_v_path, multibar_l_path;
        // if(legacyFixtures)
        // {
        //     blind_h_path = horizontalpath + "VRSL-DMX-Static-Blinder-H-13CH.prefab";
        //     blind_v_path = verticalpath + "VRSL-DMX-Static-Blinder-V-13CH.prefab";
        //     blind_l_path = legacypath + "VRSL-DMX-Static-Blinder-L-13CH.prefab";
        //     blind_a_path = audiopath + "VRSL-AudioLink-Static-Blinder.prefab";

        //     par_h_path = horizontalpath + "VRSL-DMX-Static-ParLight-H-13CH.prefab";
        //     par_v_path = verticalpath + "VRSL-DMX-Static-ParLight-V-13CH.prefab";
        //     par_l_path = legacypath + "VRSL-DMX-Static-ParLight-L-13CH.prefab";
        //     par_a_path = audiopath + "VRSL-AudioLink-Static-ParLight.prefab";

        //     bar_h_path = horizontalpath + "VRSL-DMX-Static-LightBar-H-13CH.prefab";
        //     bar_v_path = verticalpath + "VRSL-DMX-Static-LightBar-V-13CH.prefab";
        //     bar_l_path = legacypath + "VRSL-DMX-Static-LightBar-L-13CH.prefab";
        //     bar_a_path = audiopath + "VRSL-AudioLink-Static-Lightbar.prefab";
            
        //     six_h_path = horizontalpath + "5-Channel Statics/" + "VRSL-DMX-Static-6x4StrobeLight-H-5CH.prefab";
        //     six_v_path = verticalpath + "5-Channel Statics/" + "VRSL-DMX-Static-6x4StrobeLight-V-5CH.prefab";
        //     six_l_path = legacypath + "5-Channel Statics/" + "VRSL-DMX-Static-6x4StrobeLight-L-5CH.prefab";

        //     multibar_h_path = horizontalpath + "VRSL-DMX-Static-MultiLightBar-H-15CH.prefab";
        //     multibar_v_path = verticalpath + "VRSL-DMX-Static-MultiLightBar-V-15CH.prefab";
        //     multibar_l_path = legacypath + "VRSL-DMX-Static-MultiLightBar-L-15CH.prefab";
        // }
        // else
        // {
        //     blind_h_path = horizontalpath + "5-Channel Statics/" + "VRSL-DMX-Static-Blinder-H-5CH.prefab";
        //     blind_v_path = verticalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-Blinder-V-5CH.prefab";
        //     blind_l_path = legacypath + "VRSL-DMX-Static-Blinder-L-13CH.prefab";
        //     blind_a_path = audiopath + "VRSL-AudioLink-Static-Blinder.prefab";

        //     par_h_path = horizontalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-ParLight-H-5CH.prefab";
        //     par_v_path = verticalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-ParLight-V-5CH.prefab";
        //     par_l_path = legacypath + "VRSL-DMX-Static-ParLight-L-13CH.prefab";
        //     par_a_path = audiopath + "VRSL-AudioLink-Static-ParLight.prefab";

        //     bar_h_path = horizontalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-LightBar-H-5CH.prefab";
        //     bar_v_path = verticalpath + "5-Channel Statics/" +  "VRSL-DMX-Static-LightBar-V-5CH.prefab";
        //     bar_l_path = legacypath + "VRSL-DMX-Static-LightBar-L-13CH.prefab";
        //     bar_a_path = audiopath + "VRSL-AudioLink-Static-Lightbar.prefab";

        //     six_h_path = horizontalpath + "5-Channel Statics/" + "VRSL-DMX-Static-6x4StrobeLight-H-5CH.prefab";
        //     six_v_path = verticalpath + "5-Channel Statics/" + "VRSL-DMX-Static-6x4StrobeLight-V-5CH.prefab";
        //     six_l_path = legacypath + "5-Channel Statics/" + "VRSL-DMX-Static-6x4StrobeLight-L-5CH.prefab";

        //     multibar_h_path = horizontalpath + "VRSL-DMX-Static-MultiLightBar-H-15CH.prefab";
        //     multibar_v_path = verticalpath + "VRSL-DMX-Static-MultiLightBar-V-15CH.prefab";
        //     multibar_l_path = legacypath + "VRSL-DMX-Static-MultiLightBar-L-15CH.prefab";

        // }

        // string flash_h_path = horizontalpath + "VRSL-DMX-Static-Flasher-H-1CH.prefab";
        // string flash_v_path = verticalpath + "VRSL-DMX-Static-Flasher-V-1CH.prefab";
        // string flash_l_path = legacypath + "VRSL-DMX-Static-Flasher-L-1CH.prefab";
        // string flash_a_path = audiopath + "VRSL-AudioLink-Static-Flasher.prefab";

        // string disco_h_path = horizontalpath + "VRSL-DMX-Static-DiscoBall-H-1CH.prefab";
        // string disco_v_path = verticalpath + "VRSL-DMX-Static-DiscoBall-V-1CH.prefab";
        // string disco_l_path = legacypath + "VRSL-DMX-Static-DiscoBall-L-1CH.prefab";
        // string disco_a_path = audiopath + "VRSL-AudioLink-DiscoBall.prefab";

        // string laser_h_path = horizontalpath + "VRSL-DMX-Static-Laser-H-13CH.prefab";
        // string laser_v_path = verticalpath + "VRSL-DMX-Static-Laser-V-13CH.prefab";
        // string laser_l_path = legacypath + "VRSL-DMX-Static-Laser-L-13CH.prefab";
        // string laser_a_path = audiopath + "VRSL-AudioLink-BasicLaser.prefab";


        









        // string horizontalpath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Horizontal Mode/";
        // string verticalpath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Vertical Mode/";
        // string legacypath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/DMX/Legacy Mode/";
        // string audiopath = "Assets/VR-Stage-Lighting/Runtime/Prefabs/AudioLink/";

        string spot_h_path = AssetDatabase.GUIDToAssetPath("f5be3cfe3f15bfb4e9477904c5af9daf");
        string spot_v_path = AssetDatabase.GUIDToAssetPath("9a6d4144bda0d3c4ba95593af446b653");
        string spot_l_path = AssetDatabase.GUIDToAssetPath("d9cab657bd2dff14ea5425c2c4c4679e");
        string spot_a_path = AssetDatabase.GUIDToAssetPath("2aa50be2d32099842af2903a918a56f7");

        string wash_h_path = AssetDatabase.GUIDToAssetPath("b3e8ff051cc2d684aa255ceccce9b96f");
        string wash_v_path = AssetDatabase.GUIDToAssetPath("88bee1a0ddf090d4bb0721b30240c949");
        string wash_l_path = AssetDatabase.GUIDToAssetPath("41c8453c8957aec4292212174d351a36");
        string wash_a_path = AssetDatabase.GUIDToAssetPath("dd0fe316ce2ca824ead0901561087fd3");


        string blind_h_path, blind_v_path, blind_l_path, blind_a_path;
        string par_h_path, par_v_path, par_l_path, par_a_path;
        string bar_h_path, bar_v_path, bar_l_path, bar_a_path;
        string six_h_path, six_v_path, six_l_path;
        string multibar_h_path, multibar_v_path, multibar_l_path;
        if(legacyFixtures)
        {
            blind_h_path = AssetDatabase.GUIDToAssetPath("e9dde3e86ccb8ca4bb4ecbe35a6fa7b1");
            blind_v_path = AssetDatabase.GUIDToAssetPath("d7a8bacd5310e8e499962549ef931c57");
            blind_l_path = AssetDatabase.GUIDToAssetPath("9310469001d6cdf4db2145f9fddd7933");
            blind_a_path = AssetDatabase.GUIDToAssetPath("269647a339f4d1c47951638c83aa839b");

            par_h_path = AssetDatabase.GUIDToAssetPath("946b3c09cfa93244c90a4b0ac7764b44");
            par_v_path = AssetDatabase.GUIDToAssetPath("2ff8eb277ef9d7047b12d127b2eaeb36");
            par_l_path = AssetDatabase.GUIDToAssetPath("dd7cad5fc7f12624ea58efde5c3cd633");
            par_a_path = AssetDatabase.GUIDToAssetPath("161d81f8a11b22d42ae4e81f522939d3");

            bar_h_path = AssetDatabase.GUIDToAssetPath("96ffbd2a722ae324e892d303e2ee9a2a");
            bar_v_path = AssetDatabase.GUIDToAssetPath("b2a0b640363bc10408fb7a3803939fa0");
            bar_l_path = AssetDatabase.GUIDToAssetPath("b1b81594f59ca5d469ad06808051c682");
            bar_a_path = AssetDatabase.GUIDToAssetPath("c33f8d4d996a9ba47b86d420e4cdb05b");
            
            six_h_path = AssetDatabase.GUIDToAssetPath("46d1954298362974887b80dc3d70ee5f");
            six_v_path = AssetDatabase.GUIDToAssetPath("51b428740444288448e19c88c64d5311");
            six_l_path = AssetDatabase.GUIDToAssetPath("6f5c5d0af7e69e242ad56ac13882e04e");

            multibar_h_path = AssetDatabase.GUIDToAssetPath("c19e8fd46b4abdf49bb7b6cdc62acdde");
            multibar_v_path = AssetDatabase.GUIDToAssetPath("b8873da88b401dd4ab93b061c5ddf750");
            multibar_l_path = AssetDatabase.GUIDToAssetPath("dd68d30b9f0b34442aac2fb4540ae553");
        }
        else
        {
            //5channel modes
            blind_h_path = AssetDatabase.GUIDToAssetPath("5ae312c8e69488842994fd62a7609adc");
            blind_v_path = AssetDatabase.GUIDToAssetPath("94d6ff221dc5748458941750e422114f");
            blind_l_path = AssetDatabase.GUIDToAssetPath("9310469001d6cdf4db2145f9fddd7933");
            blind_a_path = AssetDatabase.GUIDToAssetPath("269647a339f4d1c47951638c83aa839b");

            par_h_path = AssetDatabase.GUIDToAssetPath("6a94fea4f85300a44b9e29ba54430110");
            par_v_path = AssetDatabase.GUIDToAssetPath("3b7bdfab2bd7abf4295be3356f6f3617");
            par_l_path = AssetDatabase.GUIDToAssetPath("dd7cad5fc7f12624ea58efde5c3cd633");
            par_a_path = AssetDatabase.GUIDToAssetPath("161d81f8a11b22d42ae4e81f522939d3");

            bar_h_path = AssetDatabase.GUIDToAssetPath("fbb24c41d42d23c4296c31f5aca73942");
            bar_v_path = AssetDatabase.GUIDToAssetPath("78bf4380452cfbe4aa8154e17a189b28");
            bar_l_path = AssetDatabase.GUIDToAssetPath("b1b81594f59ca5d469ad06808051c682");
            bar_a_path = AssetDatabase.GUIDToAssetPath("c33f8d4d996a9ba47b86d420e4cdb05b");

            six_h_path = AssetDatabase.GUIDToAssetPath("46d1954298362974887b80dc3d70ee5f");
            six_v_path = AssetDatabase.GUIDToAssetPath("51b428740444288448e19c88c64d5311");
            six_l_path = AssetDatabase.GUIDToAssetPath("6f5c5d0af7e69e242ad56ac13882e04e");

            multibar_h_path = AssetDatabase.GUIDToAssetPath("c19e8fd46b4abdf49bb7b6cdc62acdde");
            multibar_v_path = AssetDatabase.GUIDToAssetPath("b8873da88b401dd4ab93b061c5ddf750");
            multibar_l_path = AssetDatabase.GUIDToAssetPath("dd68d30b9f0b34442aac2fb4540ae553");

        }

        string flash_h_path = AssetDatabase.GUIDToAssetPath("6d00d693f1608ab49ad08d18dbe1fa02");
        string flash_v_path = AssetDatabase.GUIDToAssetPath("1c08f57da0cd0414c85f64b373431921");
        string flash_l_path = AssetDatabase.GUIDToAssetPath("a38b2f56984259247bded9aa1b1ee149");
        string flash_a_path = AssetDatabase.GUIDToAssetPath("092158b73b160384f904e33d35a09123");

        string disco_h_path = AssetDatabase.GUIDToAssetPath("22192b7ad03f22a4db578b035fdca38d");
        string disco_v_path = AssetDatabase.GUIDToAssetPath("8bb1407f1f0e2cc48b9bbf35ca1951a6");
        string disco_l_path = AssetDatabase.GUIDToAssetPath("65a96b17618e51548a669749173d48ff");
        string disco_a_path = AssetDatabase.GUIDToAssetPath("a7acda2f5fe7dfd4aaa49ec10a2d5586");

        string laser_h_path = AssetDatabase.GUIDToAssetPath("3d6c0b40980bcd34aba9183a62ecbd21");
        string laser_v_path = AssetDatabase.GUIDToAssetPath("55058c5ef8c22d04991b48a99a10acfe");
        string laser_l_path = AssetDatabase.GUIDToAssetPath("55ac9bf95dc63bb4fb6ba2095d73cde2");
        string laser_a_path = AssetDatabase.GUIDToAssetPath("75c269de381facb4cae616c67f83f519");





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
                    sixFour_h = AssetDatabase.LoadAssetAtPath(six_h_path, typeof(GameObject));
                    multiLightbar_h = AssetDatabase.LoadAssetAtPath(multibar_h_path, typeof(GameObject));
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
                    sixFour_v = AssetDatabase.LoadAssetAtPath(six_v_path, typeof(GameObject));
                    multiLightbar_v = AssetDatabase.LoadAssetAtPath(multibar_v_path, typeof(GameObject));
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
                    sixFour_l = AssetDatabase.LoadAssetAtPath(six_l_path, typeof(GameObject));
                    multiLightbar_l = AssetDatabase.LoadAssetAtPath(multibar_l_path, typeof(GameObject));
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


    public void CreateMaterialGUI(Material _material)
    {
        //_material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material.mat");
        // if(_material == null)
        // {
        //     Debug.Log("Material is empty!");
        // }
        // else
        // {
        //     Debug.Log("Material found!");
        // }
        if (_materialEditor != null) 
        {
            DestroyImmediate(_materialEditor);
        }
        _materialEditor = (MaterialEditor)Editor.CreateEditor(_material);
        // if(_materialEditor != e)
        // {
        //   //  DestroyImmediate (_materialEditor);
        //     _materialEditor = e;
        // }
        // else
        // {
        //     DestroyImmediate (e);
        // }
        if (_materialEditor != null) 
        {
            // _container = new IMGUIContainer(() =>
            // {
                _materialEditor.DrawHeader();

                // bool isDefaultMaterial = false;

                // if(_material != null)
                // {
                //     isDefaultMaterial = !AssetDatabase.GetAssetPath (_material).StartsWith ("Assets");
                // }


                EditorGUILayout.BeginVertical();
                // using (new EditorGUI.DisabledGroupScope(isDefaultMaterial)) 
                // {
                    _materialEditor.OnInspectorGUI();
                // }
                EditorGUILayout.EndVertical();
            // });
            // rootVisualElement.Add(_container);
        }
    }
    GUIStyle LongLabel(int m)
    {
       GUIStyle output = new GUIStyle("Label");
    //    output.margin.left += m;
    //    output.margin.right += m;
    //    output.padding.left += m;
    //    output.padding.right += m;
        output.clipping = TextClipping.Overflow;
       return output;

    }

    void OnGUI() {
        DrawLogo();
        ShurikenHeaderCentered(GetVersion());
        GUILayout.Label("Control Panel",Title1());

        if(Application.isPlaying)
        {
            //CheckForDesktopScreen();
            GUILayout.Label("Control Panel Disabled while Editor is playing!", WarningLabel());
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.EndVertical();
            return;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        EditorGUI.BeginDisabledGroup(true);
        if(hasLocalPanel)
        {
            EditorGUILayout.ObjectField(Label("Current Local UI Panel", "The Current VRSL Control Panel in the scene. This is where most of the settings for your VRSL enabled scene will be stored."),panel, panel.GetType(), true);
        }
        EditorGUI.EndDisabledGroup();
        if(hasLocalPanel && panel != null)
        {
            var so = new SerializedObject(panel);
            EditorGUI.indentLevel++;
            so.FindProperty("_requireDepthLight").boolValue = EditorGUILayout.ToggleLeft(Label("Require Depth Light (Recommended: Enable on PC. Required: Disable on Quest)", "Require The Depth Texture to be used in this scene. This will remove the requirement for a directional light in the scene to generate a depth texture for the shaders. WARNING: This will remove the ability to use projections and make volumetrics clip through objects more aggressively. Disabling this is required for VRSL to work on Quest."),panel.RequireDepthLight);
            so.FindProperty("_volumetricNoise").boolValue = EditorGUILayout.ToggleLeft(Label("Use Volumetric Noise (Recomended: Enable on PC and Disable on Quest)", "Disables both 3D and 2D noise effects on all volumetric VRSL shaders in this scene at the shader level. Disable this for more performance if you do not care for the noise effect. Disabling this is highly recommend for quest."),panel.VolumetricNoise);
            EditorGUI.indentLevel--;
            so.ApplyModifiedProperties();
            if((so.FindProperty("_requireDepthLight").boolValue != lastDepthLightRequirement) || (so.FindProperty("_volumetricNoise").boolValue != lastVolumetricNoiseToggle))
            {
                #pragma warning disable 0618 //suppressing obsoletion warnings
                panel.UpdateProxy();
                panel.RequireDepthLight = so.FindProperty("_requireDepthLight").boolValue;
                panel.VolumetricNoise = so.FindProperty("_volumetricNoise").boolValue;
                panel.ApplyProxyModifications();
                #pragma warning restore 0618 //suppressing obsoletion warnings
                lastDepthLightRequirement = panel.RequireDepthLight;
                lastVolumetricNoiseToggle = panel.VolumetricNoise;
                MassHideShowProjections();
            }
        }
        EditorGUI.BeginDisabledGroup(true);
        if(hasDepthLight && panel != null)
        {
            if(panel.RequireDepthLight)
            {
                EditorGUILayout.ObjectField(Label("Current Depth Light", "The main depth enabled directioanl light in the scene. This ensures that the PC quality lighting effects are working properly."),depthLight, depthLight.GetType(), true);
            }
        }
        EditorGUI.EndDisabledGroup();
        mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos, false, false);
        if(!hasDepthLight && panel != null)
        {
            if(panel.RequireDepthLight)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label("Please ensure the ''Directional Light (For Depth)'' object is in your scene to ensure your lights work properly!", WarningLabel());
                if(GUILayout.Button(Label("Spawn Depth Light Prefab!", "Spawn the VRSL depth light prefab! A directional light that emits no light/shadows, but still enables the camera's depth texture for the scene.")))
                {
                        Debug.Log("VRSL Control Panel: Spawning Depth Light Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(directionalLightPrefab, "Spawn Directional Light", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(directionalLightPrefab as GameObject) ,null);
                        Repaint();
                }
                EditorGUILayout.EndVertical();
            }
        }
        if(panel != null)
        {
            if(panel.RequireDepthLight && !hasDepthLight)
            {
                EditorGUILayout.EndScrollView();
                return;
            }
            else if(!panel.RequireDepthLight)
            {
                GUILayout.Label("WARNING: Depth Light Requirement Disabled. All projection based shaders (such as spot lights, disco balls, par lights) will no longer work properly! Use at your own risk!", WarningLabel());
            }
        }
        if(hasLocalPanel && panel != null)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(!panel.isUsingDMX);
            var so = new SerializedObject(panel);
            so.FindProperty("DMXMode").intValue = EditorGUILayout.Popup(Label("DMX Grid Mode", "Choose what grid type textures should be enabled for DMX mode. Unused textures will be disabled to save editor performance!"),panel.DMXMode, dmxModes);

            so.FindProperty("fixtureGizmos").intValue = EditorGUILayout.Popup(Label("Show DMX Info In Scene", "Display DMX Channel and/or Universe information above each fixture in the scene view!"), panel.fixtureGizmos, dmxGizmoInfo);
            so.FindProperty("useExtendedUniverses").boolValue = EditorGUILayout.ToggleLeft(Label("Use RGB Extended Universes (9-Universe Mode)", "Enable Extended Universe Mode. This will convert all fixtures to read an RGB grid that contains 9 universes of information instead a grayscale grid with 3 universes." + 
            "This only applies to the Vertical and Horizontal grid modes."),panel.useExtendedUniverses);

            so.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            legacyFixtures = so.FindProperty("useLegacyStaticLights").boolValue;

            string o = mainOptionsFoldout ? "Hide Options" : "Show Options";
            mainOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(mainOptionsFoldout,Label(o, "Show/Hide Global VRSL Settings and Prefab Spawners."));
            EditorGUILayout.EndFoldoutHeaderGroup();
            #pragma warning disable 0618 //suppressing obsoletion warnings
            panel.UpdateProxy();
            #pragma warning restore 0618 //suppressing obsoletion warnings
            panel.DMXMode = so.FindProperty("DMXMode").intValue;
            panel.fixtureGizmos = so.FindProperty("fixtureGizmos").intValue;
            panel.useExtendedUniverses = so.FindProperty("useExtendedUniverses").boolValue;
            #pragma warning disable 0618 //suppressing obsoletion warnings
            panel.ApplyProxyModifications();
            #pragma warning restore 0618 //suppressing obsoletion warnings


            if((panel.useExtendedUniverses != last9UniverseStatus) && (EditorApplication.isPlayingOrWillChangePlaymode == false))
            {
                panel._CheckkExtendedUniverses();
                last9UniverseStatus = panel.useExtendedUniverses;
                MassApplyExtendedUniverseStatus();
                if(last9UniverseStatus)
                {
                    Debug.Log("VRSL Control Panel: Updating all fixtures to Extended 9 Universe Mode!");
                }
                else
                {
                    Debug.Log("VRSL Control Panel: Updating all fixtures to Standard 3 Universe Mode!");
                }
                ApplyChangesToFixtures(true, false, false);
                ResetItems(false);
                Repaint();  
            }



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
                EditorGUI.BeginDisabledGroup(!panel.isUsingDMX);      
                EditorGUILayout.BeginHorizontal("box");
                if(GUILayout.Button(Label("Auto Assign Fixture IDs.", "Automatically assigns fixture IDs to all fixtures in the scene based on what prefab they're linked to. If they are not linked to a prefab, their fixture ID will starts with a 0 in the hundreds place."), GUILayout.MaxWidth(170f)))
                {
                    MassApplyIDs();
                    Debug.Log("VRSL Control Panel: Auto assigning Fixture IDs!");
                    ApplyChangesToFixtures(true, false, false);
                    ResetItems(false);
                    Repaint();   
                }


                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();



                EditorGUI.BeginDisabledGroup(!panel.isUsingAudioLink);   
                Rect alr = EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(Label("VRSL AudioLink Target Sample Texture", "Use this field to set the texture that all AudioLink VRSL fixtures will sample from when texture sampling is enabled on them."));
                if (GUILayout.Button(new GUIContent("Force Update Target Sample Texture", "Updates all AudioLink VRSL Fixtures to sample from the selected target texture when texture sampling is enabled on the fixture."), GUILayout.MaxWidth(230f), GUILayout.MinHeight(20f))) { panel._ForceUpdateVideoSampleTexture(); }
                EditorGUILayout.EndVertical();
                Rect texPropRect = alr;
                texPropRect.width = 40f;
                texPropRect.height = 40f;
                texPropRect.x += 250f;
                texPropRect.y += 5f;
                soptr.FindProperty("videoSampleTargetTexture").objectReferenceValue = (Texture)EditorGUI.ObjectField(texPropRect, panel.videoSampleTargetTexture, typeof(Texture), true);
                
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                //GUILayout.Label("VRSL Stencil Masks",Title4());
                string stencilDef = "Spawn stencil masks that block out any transparent VRSL materials. Good for preventing VRSL lights from leaking into adjacent rooms and hallways.";
                //EditorGUILayout.LabelField(Label("VRSL Stencil Masks", stencilDef));
                string stencil = stencilFoldout ? "Hide Stencil Masks" : "Show Stencil Masks";
                stencilFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(stencilFoldout,Label(stencil, "Show/Hide VRSL Stencil Mask Prefab Spawners. " + stencilDef));
                EditorGUILayout.EndFoldoutHeaderGroup();
                if(stencilFoldout)
                {
                    
                    EditorGUILayout.BeginHorizontal("box");
                    float stencilMaxWidth = 100f;

                    if(GUILayout.Button(Label("Cube", "Spawn a cube stencil mask. " + stencilDef), GUILayout.MaxWidth(stencilMaxWidth)))
                    {
                            Debug.Log("VRSL Control Panel: Spawning Cube Stencil Mask");
                            if(LoadPrefabs())
                                SpawnPrefabWithUndo(cubeMask, "Spawn VRSL Cube Stencil Mask", false, false);
                            Repaint();
                    }
                    if(GUILayout.Button(Label("Sphere", "Spawn a sphere stencil mask. " + stencilDef), GUILayout.MaxWidth(stencilMaxWidth)))
                    {
                            Debug.Log("VRSL Control Panel: Spawning Sphere Stencil Mask");
                            if(LoadPrefabs())
                                SpawnPrefabWithUndo(sphereMask, "Spawn VRSL Sphere Stencil Mask", false, false);
                            Repaint();
                    }
                    if(GUILayout.Button(Label("Cylinder", "Spawn a cylinder stencil mask. " + stencilDef), GUILayout.MaxWidth(stencilMaxWidth)))
                    {
                            Debug.Log("VRSL Control Panel: Spawning Cylinder Stencil Mask");
                            if(LoadPrefabs())
                                SpawnPrefabWithUndo(cylinderMask, "Spawn VRSL Cylinder Stencil Mask", false, false);
                            Repaint();
                    }
                    if(GUILayout.Button(Label("Capusle", "Spawn a capsule stencil mask. " + stencilDef), GUILayout.MaxWidth(stencilMaxWidth)))
                    {
                            Debug.Log("VRSL Control Panel: Spawning Capsule Stencil Mask");
                            if(LoadPrefabs())
                                SpawnPrefabWithUndo(capsuleMask, "Spawn VRSL Capsule Stencil Mask", false, false);
                            Repaint();
                    }

                    EditorGUILayout.EndHorizontal();
                }
                

            EditorGUILayout.Space(4);
            string matEditorPrefix = showMaterialEditor ? "Hide " : "Show ";
            showMaterialEditor = EditorGUILayout.BeginFoldoutHeaderGroup(showMaterialEditor,Label(matEditorPrefix + "CRT Material Editor", "Show/Hide VRSL CRT Material Editors. "));
            EditorGUILayout.EndFoldoutHeaderGroup();
            if(showMaterialEditor)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                EditorGUILayout.PrefixLabel("Select A Custom Render Texture",followingStyle: "Popup", labelStyle: LongLabel(-10));
                materialChooser = EditorGUILayout.Popup(materialChooser,materialChooserList, GUILayout.MaxWidth(350f));
                switch(so.FindProperty("DMXMode").intValue)
                {
                    default:
                            switch(materialChooser)
                            {
                                default:
                                    break;
                                case 1:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_H_CRT_Color_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 2:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_H_CRT_Mvmt_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 3:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_H_CRT_Spin_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 4:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_H_CRT_StrobeTime_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 5:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_H_CRT_StrobeOut_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;      
                                case 6:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(audiolink_CRT_InterpolationMat);
                                    EditorGUILayout.EndVertical();
                                    break;                                                           
                            }
                        break;
                    case 1:
                            switch(materialChooser)
                            {
                                default:
                                    break;
                                case 1:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_V_CRT_Color_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 2:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_V_CRT_Mvmt_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 3:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_V_CRT_Spin_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 4:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_V_CRT_StrobeTime_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 5:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_V_CRT_StrobeOut_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;      
                                case 6:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(audiolink_CRT_InterpolationMat);
                                    EditorGUILayout.EndVertical();
                                    break;                                                           
                            }
                        break;
                    case 2:
                            switch(materialChooser)
                            {
                                default:
                                    break;
                                case 1:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_L_CRT_Color_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 2:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_L_CRT_Mvmt_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 3:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_L_CRT_Spin_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 4:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_L_CRT_StrobeTime_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 5:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(dmx_L_CRT_StrobeOut_Mat);
                                    EditorGUILayout.EndVertical();
                                    break;      
                                case 6:
                                    EditorGUILayout.BeginVertical();
                                    CreateMaterialGUI(audiolink_CRT_InterpolationMat);
                                    EditorGUILayout.EndVertical();
                                    break;                                                           
                            }
                        break;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
                
                EditorGUILayout.EndVertical();
                //EditorGUILayout.Space();
                
                //GuiLine();
                //GUILayout.Label("Prefab Spawn List", Title2());

                EditorGUILayout.BeginHorizontal("box");


                //EditorGUILayout.Space();
                string t = "Show DMX Prefab Spawn Buttons";
                if(dmxSpawnsFoldout)
                    t = "Hide DMX Prefab Spawn Buttons";
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(((position.width/2f)-30f)));
                dmxSpawnsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(dmxSpawnsFoldout,Label(t, "Show/Hide DMX Prefab Spawn Buttons!"), Title2Foldout());
                
                if(dmxSpawnsFoldout)
                {
                    GUILayout.Label("DMX Direct Readers (TekOSC To Editor)", Title3());
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Horizontal", "Spawn the horizontal version of the TekOSC DMX Reader! Use this to send DMX to the Unity Editor through OSC without OBS!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Horizontal OSC Grid Reader...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(oscGridReaderHorizontalPrefab, "Spawn OSC Grid Reader", false, false);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Vertical", "Spawn the vertical version of the TekOSC DMX Reader! Use this to send DMX to the Unity Editor through OSC without OBS!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Vertical OSC Grid Reader...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(oscGridReaderVerticalPrefab, "Spawn OSC Grid Reader", false, false);
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    GUILayout.Label("DMX Reader Screens (Desktop To Editor)", Title3());
                    
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Horizontal", "Spawn the horizontal version of the uDesktop DMX Reader screen! Use this send your DMX stream directly to the Unity Editor by copying your screen!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Horizontal Desktop to Editor DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uDesktopHorizontalPrefab, "Spawn Desktop To Editor Screen", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uDesktopHorizontalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Vertical", "Spawn the vertical version of the uDesktop DMX Reader screen! Use send your DMX stream directly to the Unity Editor by copying your screen!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Vertical Desktop to Editor DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uDesktopVerticalPrefab, "Spawn Desktop To Editor Screen", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uDesktopVerticalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Legacy", "Spawn the legacy version of the uDesktop DMX Reader screen! Use send your DMX stream directly to the Unity Editor by copying your screen!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Legacy Desktop to Editor DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uDesktopLegacyPrefab, "Spawn Desktop To Editor Screen", false, false);
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
                            SpawnPrefabWithUndo(uVidHorizontalPrefab, "Spawn Usharp Video DMX Screen", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uVidHorizontalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Vertical", "Spawn the vertical version of the USharp Video Player DMX Reader Screen. Use this video player prefab to stream DMX data to VRChat via Twitch/VRCDN! You can replace the video player in this prefab with any other video player you want as long as the screen gets duplicated to the larger plane!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Vertical Usharp Video DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uVidVerticalPrefab, "Spawn Usharp Video DMX Screen", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(uVidVerticalPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Legacy", "Spawn the legacy version of the USharp Video Player DMX Reader Screen. Use this video player prefab to stream DMX data to VRChat via Twitch/VRCDN! You can replace the video player in this prefab with any other video player you want as long as the screen gets duplicated to the larger plane!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Legacy Usharp Video DMX Screen...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(uVidLegacyPrefab, "Spawn Usharp Video DMX Screen", false, false);
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
                                        SpawnPrefabWithUndo(spotlight_h, "Spawn Horizontal Spotlight", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(spotlight_v, "Spawn Vertical Spotlight", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(spotlight_l, "Spawn Legacy Spotlight", true, false);
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
                                        SpawnPrefabWithUndo(washlight_h, "Spawn Horizontal Washlight", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(washlight_v, "Spawn Vertical Washlight", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(washlight_l, "Spawn Legacy Washlight", true, false);
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
                                        SpawnPrefabWithUndo(blinder_h, "Spawn Horizontal Blinder", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(blinder_v, "Spawn Vertical Blinder", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(blinder_l, "Spawn Legacy Blinder", true, false);
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
                                        SpawnPrefabWithUndo(flasher_h, "Spawn Horizontal Flasher", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(flasher_v, "Spawn Vertical Flasher", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(flasher_l, "Spawn Legacy Flasher", true, false);
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
                                        SpawnPrefabWithUndo(parlight_h, "Spawn Horizontal Parlight", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(parlight_v, "Spawn Vertical Parlight", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(parlight_l, "Spawn Legacy Parlight", true, false);
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
                                        SpawnPrefabWithUndo(lightbar_h, "Spawn Horizontal LightBar", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(lightbar_v, "Spawn Vertical LightBar", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(lightbar_l, "Spawn Legacy LightBar", true, false);
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
                                        SpawnPrefabWithUndo(discoball_h, "Spawn Horizontal Discoball", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(discoball_v, "Spawn Vertical Discoball", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(discoball_l, "Spawn Legacy Discoball", true, false);
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
                                        SpawnPrefabWithUndo(laser_h, "Spawn Horizontal Laser", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(laser_v, "Spawn Vertical Laser", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(laser_l, "Spawn Legacy Laser", true, false);
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
                    EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button(Label("6x4 Light (5CH)", "Spawn the " + fp + " version of the DMX 6x4 Light prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " 6x4 Light Prefab Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(sixFour_h, "Spawn Horizontal 6x4 Light", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(sixFour_v, "Spawn Vertical 6x4 Light", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(sixFour_l, "Spawn Legacy 6x4 Light", true, false);
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
                    if(GUILayout.Button(Label("Multi-Lightbar (5CH)", "Spawn the " + fp + " version of the DMX Multi-Lightbar prefab!"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning DMX " + fp + " Multi-Lightbar Prefab Prefab...");
                        if(LoadFixturePrefabs(mode))
                        {
                            try{
                                switch(mode)
                                {
                                    case 0:
                                        SpawnPrefabWithUndo(multiLightbar_h, "Spawn Horizontal Multi-Lightbar", true, false);
                                        break;
                                    case 1:
                                        SpawnPrefabWithUndo(multiLightbar_v, "Spawn Vertical Multi-Lightbar", true, false);
                                        break;
                                    case 2:
                                        SpawnPrefabWithUndo(multiLightbar_l, "Spawn Legacy Multi-Lightbar", true, false);
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
                    EditorGUILayout.BeginHorizontal();
                    soptr.FindProperty("useLegacyStaticLights").boolValue = EditorGUILayout.ToggleLeft("Use Old 13 Channel Static Lights (Not Recommended)", panel.useLegacyStaticLights);

                    // if(hasDMXGI)
                    // {
                    //     soptr.FindProperty("useDMXGI").boolValue = EditorGUILayout.ToggleLeft("Enable DMX GI Prefabs", panel.useDMXGI);
                    // }
                    EditorGUILayout.EndHorizontal();


                    
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
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Spawn Standard AudioLink Prefab", "Spawn the standard AudioLink prefab that comes with AudioLink."), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning Standard AudioLink Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(audioLinkPrefab, "Spawn AudioLink Prefab", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(audioLinkPrefab as GameObject), null);
                        Repaint();
                    }
                    if(GUILayout.Button(Label("Spawn VRSL AudioLink Prefab", "Spawn the slight modified version of the AudioLink Prefab. Use this one to have the smoothing sliders work"), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning VRSL AudioLink Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(audioLinkVRSLPrefab, "Spawn VRSL AudioLink Prefab", false, false);
                            //Selection.SetActiveObjectWithContext(PrefabUtility.InstantiatePrefab(audioLinkPrefab as GameObject), null);
                        Repaint();
                    }

                    EditorGUILayout.EndHorizontal();

                    GUILayout.Label("Controller Prefabs", Title3());
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth((position.width/2f)));
                    if(GUILayout.Button(Label("Spawn Standard AudioLink Controller", "Spawn the standard AudioLink Controller Prefab that comes with AudioLink."), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning VRSL AudioLink Controller Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(standardAudioLinkControllerPrefab, "Spawn AudioLink Controller", false, false);
                        Repaint();
                    }

                    if(GUILayout.Button(Label("Spawn VRSL AudioLink Controller", "Spawn the VRS version of the AudioLink Controller Prefab. This version has extra smoothing sliders to smooth out the pulsing from AudioLink for the lights."), HalfButton()))
                    {
                        Debug.Log("VRSL Control Panel: Spawning VRSL AudioLink Controller Prefab...");
                        if(LoadPrefabs())
                            SpawnPrefabWithUndo(audioLinkControllerPrefab, "Spawn VRSL AudioLink Controller", false, false);
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
                                SpawnPrefabWithUndo(spotlight_a, "Spawn AudioLink Spotlight", true, true);
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
                                SpawnPrefabWithUndo(washlight_a, "Spawn AudioLink Washlight", true, true);
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
                                SpawnPrefabWithUndo(blinder_a, "Spawn AudioLink Blinder", true, true);
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
                                SpawnPrefabWithUndo(flasher_a, "Spawn AudioLink Flasher", true, true);
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
                                SpawnPrefabWithUndo(parlight_a, "Spawn AudioLink Parlight", true, true);
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
                                SpawnPrefabWithUndo(lightbar_a, "Spawn AudioLink LightBar", true, true);
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
                                SpawnPrefabWithUndo(discoball_a, "Spawn AudioLink Discoball", true, true);
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
                                SpawnPrefabWithUndo(laser_a, "Spawn AudioLink Laser", true, true);
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
                soptr.ApplyModifiedProperties();
                #pragma warning disable 0618 //suppressing obsoletion warnings
                panel.UpdateProxy();
                #pragma warning restore 0618 //suppressing obsoletion warnings
                panel.panRangeTarget = soptr.FindProperty("panRangeTarget").floatValue;
                panel.tiltRangeTarget = soptr.FindProperty("tiltRangeTarget").floatValue;
                panel.useLegacyStaticLights = soptr.FindProperty("useLegacyStaticLights").boolValue;
                panel.videoSampleTargetTexture = (Texture)soptr.FindProperty("videoSampleTargetTexture").objectReferenceValue;
                #pragma warning disable 0618 //suppressing obsoletion warnings
                panel.ApplyProxyModifications(); 
                #pragma warning restore 0618 //suppressing obsoletion warnings
                
            }
            GuiLine();
            GUILayout.Label("Fixture List", Title2());
            EditorGUILayout.Space(5f);
           // EditorGUILayout.BeginHorizontal();
           // GUILayout.Label("DMX Fixture Count: " + GetFixtureCount());
          //  GUILayout.Label("Audiolink Fixture Count: " + GetAudioLinkFixtureCount());
           // EditorGUILayout.EndHorizontal();
           int dmxCount = GetFixtureCount();
           int audioLinkCount = GetAudioLinkFixtureCount();
            EditorGUILayout.Space(5f);
            if(GUILayout.Button(Label("Update Fixtures", "Apply all changes in the fixture list to all affected fixtures in the scene! Also ensures all fixtures are updated with the current global statuses of the control panel."), BigButton()))
            {
                if(panel.isUsingDMX)
                {
                    MassApplyExtendedUniverseStatus();
                }
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

            so = new SerializedObject(panel);

            ////////////////////////////////////////////////////////////////////////
            Color bg = GUI.backgroundColor;
            GUIContent dmxRectText = new GUIContent(ToggleText(panel.isUsingDMX,"DMX (" + dmxCount +")"), "Enable/Disable DMX mode from this scene. This will disable all corresponding DMX render textures and prevent them from updating at runtime.");
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
            
            GUIContent audioRectText = new GUIContent(ToggleText(panel.isUsingAudioLink,"AUDIOLINK (" + audioLinkCount + ")"), "Enable/Disable AudioLink MOde from this scene. This will diable all corresponding AudioLink render textures and prevent them from updating at runtime.");
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
            #pragma warning disable 0618 //suppressing obsoletion warnings
            panel.UpdateProxy();
            #pragma warning restore 0618 //suppressing obsoletion warnings
            panel.isUsingDMX = so.FindProperty("isUsingDMX").boolValue;
            panel.isUsingAudioLink = so.FindProperty("isUsingAudioLink").boolValue;
            #pragma warning disable 0618 //suppressing obsoletion warnings
            panel.ApplyProxyModifications();
            #pragma warning restore 0618 //suppressing obsoletion warnings
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            float sectionWidth = position.width /2;
            dmxScrollPos = EditorGUILayout.BeginScrollView(dmxScrollPos, true, true, GUILayout.MaxWidth((sectionWidth)));
            if(panel.isUsingDMX)
            {

                for(int i = 0; i < universes.Length; i++)
                {

                    if(!last9UniverseStatus && i >=3)
                    {
                        continue;
                    }

                    universeFold[i] = EditorGUILayout.BeginFoldoutHeaderGroup(universeFold[i], Label("Universe " + (i + 1), "Show/Hide all fixtures in in DMX Universe " + (i+1)), Title1Foldout());
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    GuiLine();

                    if(universeFold[i])
                    {

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
                            if(GUILayout.Button("Copy", GUILayout.MinWidth(45f), GUILayout.MaxWidth(45f),GUILayout.MinHeight(28f)))
                            {
                                //Debug.Log("Test Copy Button!~");
                                try{
                                copyDMXListProx = new DMXListItem(fixture.light,false);
                                Debug.Log("Copying properties from fixture: " + fixture.light.name);
                                }
                                catch(MissingReferenceException e)
                                {
                                    e.GetType();
                                    Debug.Log("Failed to copy properties over! Was the fixture deleted recently?");
                                }

                            }
                            if(GUILayout.Button("Paste", GUILayout.MinWidth(45f), GUILayout.MaxWidth(45f),GUILayout.MinHeight(28f)))
                            {
                                try{
                                    if(copyDMXListProx != null)
                                    {
                                        CopyPasteFunction(fixture, copyDMXListProx);
                                        Debug.Log("Pasting properties from fixture: " + copyDMXListProx.light.name);
                                    }
                                    else
                                    {
                                        Debug.Log("Copy from a fixture first before using this!");
                                    }
                                }
                                catch(MissingReferenceException e)
                                {
                                    e.GetType();
                                    Debug.Log("Failed to past properties over! Was the fixture deleted recently?");
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        // EditorGUILayout.Foldout(dmxBoolList[dmxLights.IndexOf(light)],"U: " + light._GetUniverse() + " CH: " + light._GetDMXChannel() + " " + light.name, FoldOutStyle());
                            if(fixture.foldout)
                            {
                                EditorGUILayout.BeginVertical("box");
                                GUILayout.Space(8.0f);
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.ObjectField("Selected Fixture",fixture.light, fixture.light.GetType(), true, GUILayout.MaxWidth(sectionWidth -10));
                                EditorGUI.EndDisabledGroup();


                                GUILayout.Space(15.0f);
                                GUILayout.Label("DMX Settings", SecLabel());
                                GUILayout.Space(8.0f);
                                fixture.P_enableDMXChannels = EditorGUILayout.Toggle("Enable DMX", fixture.P_enableDMXChannels);
                                fixture.P_nineUniverseMode = EditorGUILayout.Toggle("Extended Universe Mode", fixture.P_nineUniverseMode);
                                fixture.P_fixtureID = EditorGUILayout.IntField("Fixture ID", fixture.P_fixtureID, GUILayout.MaxWidth(sectionWidth - 10));
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
                            }

                            GuiLine();
                            EditorGUILayout.EndFoldoutHeaderGroup();

                            //GUILayout.Space(2.0f);
                            if(PrefabUtility.IsPartOfAnyPrefab(fixture.light))
                            {
                                PrefabUtility.RecordPrefabInstancePropertyModifications(fixture.light);
                            }
                        }
                            // EditorGUILayout.EndVertical();
                            // EditorGUILayout.EndHorizontal();

                    }
                   // EditorGUILayout.EndVertical();
                   // EditorGUILayout.EndHorizontal();
                }
                universeFourFold = EditorGUILayout.BeginFoldoutHeaderGroup(universeFourFold, Label("Universe 10 (Experimental)", "DMX Universe 10 is an experimental universe used for testing out the new DMX via Audio Amplitude feature. Any Audio Amplitude based scripts will appear here."), Title1Foldout());
                GuiLine();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.EndScrollView();
            audioLinkScrollPos = EditorGUILayout.BeginScrollView(audioLinkScrollPos, true, true,GUILayout.MaxWidth((position.width / 2)));
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
                                    if((int) fixture.laser.Band == i)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(15f);
                                        fixture.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(fixture.foldout, fixture.laser.name, SelectionFoldout());
                                        if(GUILayout.Button("Copy", GUILayout.MinWidth(45f), GUILayout.MaxWidth(45f),GUILayout.MinHeight(28f)))
                                        {
                                            try{
                                            copyAudioLinkListProx = new AudioLinkListItem(fixture.laser,false);
                                            Debug.Log("Copying properties from laser: " + fixture.laser.name);
                                            }
                                            catch(MissingReferenceException e)
                                            {
                                                e.GetType();
                                                Debug.Log("Failed to copy properties over! Was the fixture deleted recently?");
                                            }
                                        }
                                        if(GUILayout.Button("Paste", GUILayout.MinWidth(45f), GUILayout.MaxWidth(45f),GUILayout.MinHeight(28f)))
                                        {
                                            try{
                                                if(copyAudioLinkListProx != null)
                                                {
                                                    
                                                    if(copyAudioLinkListProx.light != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.light.name);
                                                        CopyPasteFunctionAudioLinkNoBandFixture(fixture, copyAudioLinkListProx);
                                                    }
                                                    else if(copyAudioLinkListProx.laser != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.laser.name);
                                                        CopyPasteFunctionAudioLinkNoBandLaser(fixture, copyAudioLinkListProx);
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.Log("Copy from a fixture first before using this!");
                                                }
                                            }
                                            catch(MissingReferenceException e)
                                            {
                                                e.GetType();
                                                Debug.Log("Failed to past properties over! Was the fixture deleted recently?");
                                            }
                                        }
                                        if(GUILayout.Button("Paste w/ Bands", GUILayout.MinWidth(105f), GUILayout.MaxWidth(105f),GUILayout.MinHeight(28f)))
                                        {
                                            try{
                                                if(copyAudioLinkListProx != null)
                                                {
                                                    
                                                    if(copyAudioLinkListProx.light != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.light.name);
                                                        CopyPasteFunctionAudioLinkFixture(fixture, copyAudioLinkListProx);
                                                    }
                                                    else if(copyAudioLinkListProx.laser != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.laser.name);
                                                        CopyPasteFunctionAudioLinkLaser(fixture, copyAudioLinkListProx);
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.Log("Copy from a fixture first before using this!");
                                                }
                                            }
                                            catch(MissingReferenceException e)
                                            {
                                                e.GetType();
                                                Debug.Log("Failed to past properties over! Was the fixture deleted recently?");
                                            }
                                        }
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
                                            //fixture.P_band = EditorGUILayout.IntField("Band", fixture.P_band, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_band = (AudioLinkBandState) EditorGUILayout.EnumPopup("Band", fixture.P_band, GUILayout.MaxWidth(sectionWidth - 10));

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
                                            fixture.P_enableThemeColorSampling = EditorGUILayout.Toggle("Enable Theme Color Sampling", fixture.P_enableThemeColorSampling);
                                            fixture.P_themeColorTarget = EditorGUILayout.IntSlider("Theme Color Target", fixture.P_themeColorTarget, 1, 4, GUILayout.MaxWidth(sectionWidth - 10));
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
                                    if((int) fixture.light.Band == i)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(15f);
                                        fixture.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(fixture.foldout, fixture.light.name, SelectionFoldout());
                                        if(GUILayout.Button("Copy", GUILayout.MinWidth(45f), GUILayout.MaxWidth(45f),GUILayout.MinHeight(28f)))
                                        {
                                            try{
                                            copyAudioLinkListProx = new AudioLinkListItem(fixture.light,false);
                                            Debug.Log("Copying properties from laser: " + fixture.light.name);
                                            }
                                            catch(MissingReferenceException e)
                                            {
                                                e.GetType();
                                                Debug.Log("Failed to copy properties over! Was the fixture deleted recently?");
                                            }
                                        }
                                        if(GUILayout.Button("Paste", GUILayout.MinWidth(45f), GUILayout.MaxWidth(45f),GUILayout.MinHeight(28f)))
                                        {
                                            try{
                                                if(copyAudioLinkListProx != null)
                                                {
                                                    //CopyPasteFunctionAudioLinkNoBand(fixture, copyAudioLinkListProx);
                                                    if(copyAudioLinkListProx.light != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.light.name);
                                                        CopyPasteFunctionAudioLinkNoBandFixture(fixture, copyAudioLinkListProx);
                                                    }
                                                    else if(copyAudioLinkListProx.laser != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.laser.name);
                                                        CopyPasteFunctionAudioLinkNoBandLaser(fixture, copyAudioLinkListProx);
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.Log("Copy from a fixture first before using this!");
                                                }
                                            }
                                            catch(MissingReferenceException e)
                                            {
                                                e.GetType();
                                                Debug.Log("Failed to past properties over! Was the fixture deleted recently?");
                                            }                                            
                                        }
                                        if(GUILayout.Button("Paste w/ Bands", GUILayout.MinWidth(105f), GUILayout.MaxWidth(105f),GUILayout.MinHeight(28f)))
                                        {
                                            try{
                                                if(copyAudioLinkListProx != null)
                                                {
                                                   // CopyPasteFunctionAudioLink(fixture, copyAudioLinkListProx);
                                                    if(copyAudioLinkListProx.light != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.light.name);
                                                        CopyPasteFunctionAudioLinkFixture(fixture, copyAudioLinkListProx);
                                                    }
                                                    else if(copyAudioLinkListProx.laser != null)
                                                    {
                                                        Debug.Log("Pasting properties from fixture: " + copyAudioLinkListProx.laser.name);
                                                        CopyPasteFunctionAudioLinkLaser(fixture, copyAudioLinkListProx);
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.Log("Copy from a fixture first before using this!");
                                                }
                                            }
                                            catch(MissingReferenceException e)
                                            {
                                                e.GetType();
                                                Debug.Log("Failed to past properties over! Was the fixture deleted recently?");
                                            }
                                        }
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
                                           // fixture.P_band = EditorGUILayout.IntField("Band", fixture.P_band, GUILayout.MaxWidth(sectionWidth - 10));
                                            fixture.P_band = (AudioLinkBandState) EditorGUILayout.EnumPopup("Band", fixture.P_band, GUILayout.MaxWidth(sectionWidth - 10));

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
                                            fixture.P_enableThemeColorSampling = EditorGUILayout.Toggle("Enable Theme Color Sampling", fixture.P_enableThemeColorSampling);
                                            fixture.P_themeColorTarget = EditorGUILayout.IntSlider("Theme Color Target", fixture.P_themeColorTarget, 1, 4, GUILayout.MaxWidth(sectionWidth - 10));
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
                                if(PrefabUtility.IsPartOfAnyPrefab(fixture.light))
                                {
                                    PrefabUtility.RecordPrefabInstancePropertyModifications(fixture.light);
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
                        SpawnPrefabWithUndo(controlPanelUiPrefab, "Spawn VRSL Control Panel", false, false);
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
                            Handles.Label(pos, "CH: " + fixture.light._GetDMXChannel() +"\n" + "ID: " + fixture.light.fixtureID, SceneLabel());
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
                            Handles.Label(pos, "U: " + fixture.light._GetUniverse() +"\n" + "CH: " + fixture.light._GetDMXChannel() +"\n" + "ID: " + fixture.light.fixtureID, SceneLabel());
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
}
#endif
