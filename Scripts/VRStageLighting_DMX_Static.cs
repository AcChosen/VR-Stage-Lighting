
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
//using UnityEngine.UI;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using System.Collections.Immutable;
#endif
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]

public class VRStageLighting_DMX_Static : UdonSharpBehaviour
{
    //////////////////Public Variables////////////////////
    [Header("DMX Settings")]
    [Tooltip ("Enables DMX mode for this fixture.")]
    public bool enableDMXChannels = true;
    [Tooltip ("The industry standard DMX Channel this fixture begins on. Most standard VRSL fixtures are 13 channels")]
    public int dmxChannel = 1;
    [Tooltip ("The industry standard Artnet Universe. Use this to choose which universe to read the DMX Channel from.")]
    public int dmxUniverse = 1;
    [Tooltip ("Enables the legacy 'Sector' based method of assigning DMX Channels. Keep this unchecked to use industry standard DMX Channels.")]

    public bool useLegacySectorMode = false;
    [Tooltip ("Enables single channel DMX mode for this fixture. This is for single channeled fixtures instead of the standard 13-channeled ones. Currently, the 'Flasher' fixture is the only single-channeled fixture at the moment")]
    public bool singleChannelMode = false;
    [Tooltip ("Chooses the DMX Address to start this fixture at. A Sector in this context is every 13 Channels. I.E Sector 0 is channels 1-13, Sector 1 is channels 14-26, etc.")]
    public int sector;
    [Tooltip ("Chooses the which of the 13 Channels of the current sector to sample from when single channel mode is enabled. Do not worry about this value if you are not using a single-channeled fixture.")]
    [Range(0.0f, 12.0f)]
    public int Channel = 0;
    public bool legacyGoboRange;
    [Space(5)]
    [Header("General Settings")]
    [Range(0,1)]
    [Tooltip ("Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity.")]
    public float globalIntensity = 1; 
    [Range(0,1)]
    [Tooltip ("Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI.")]
    public float finalIntensity = 1;
    [Tooltip ("The main color of the light. Leave it at default white for DMX mode.")]
    [ColorUsage(false,true)]
    public Color lightColorTint = Color.white * 2.0f;
    [Space(5)]
    [Header("Movement Settings")]
    [Tooltip ("Invert the pan values (Left/Right Movement) for movers.")]
    public bool invertPan;
    [Tooltip ("Invert the tilt values (Up/Down Movement) for movers.")]
    public bool invertTilt;
    [Tooltip ("Enable this if the mover is hanging upside down.")]
    public bool isUpsideDown;
    [Space(5)]
    [Header("Fixture Settings")]
    [Tooltip ("Enable projection spinning (Udon Override Only).")]
    public bool enableAutoSpin = true;
    [Tooltip ("Enable strobe effects (via DMX Only).")]
    public bool enableStrobe = true;
    [Range(0,360.0f)]
    [Tooltip ("Tilt (Up/Down) offset/movement. Directly controls tilt when in Udon Mode; is an offset when in DMX mode.")]
    public float tiltOffsetBlue = 90.0f;
    float startTiltOffset;

    [Range(0,360.0f)]
    [Tooltip ("Pan (Left/Right) offset/movement. Directly controls pan when in Udon Mode; is an offset when in DMX mode.")]
    public float panOffsetBlueGreen = 0.0f;
    float startPanOffset;
    [Range(1,8)]
    [Tooltip ("Use this to change what projection is selected. This is overridden in DMX mode.")]
    public int selectGOBO = 1;
    
    //[Header("Mesh Settings")]
    [Tooltip ("The meshes used to make up the light. You need atleast 1 mesh in this group for the script to work properly.")]
    public MeshRenderer[] objRenderers;

    [Range(0, 5.5f)]
    [Tooltip ("Controls the radius of a mover/spot light.")]
    public float coneWidth = 2.5f;

    [Range(0.5f,10.0f)]
    [Tooltip ("Controls the length of the cone of a mover/spot light.")]
    public float coneLength = 8.5f;

    [Range(0.275f,10.0f)]
    [Tooltip ("Controls the mesh length of the cone of a mover/spot light")]
    //[FieldChangeCallback(nameof(MaxConeLength))]
   // [SerializeField]
    public float maxConeLength = 1.0f;  
    [ColorUsage(true, true)]
    private int calculatedDMXChannel;
    private int calculatedDMXUniverse;

    public float maxMinPan = 180f;
    public float maxMinTilt = -180f;

    

    /////////////////Private Variables//////////////////
    private bool wasChanged;
    MaterialPropertyBlock props;
    bool enableInstancing;
    float targetPanAngle, targetTiltAngle;
    private Vector3 targetToFollowLast;
    private Color previousColorTint;
    private Transform previousTargetToFollowTransform;
    
    private float previousConeWidth, previousConeLength, previousGlobalIntensity, previousFinalIntensity, previousMaxConeLength;
    private int previousGOBOSelection;
    [HideInInspector]
    public bool foldout;

    void Start()
    {
        Init(true);
    }

    void Init(bool withDMX)
    {
        if(objRenderers.Length > 0 && objRenderers[0] != null)
        {
            _SetProps();
            previousColorTint = lightColorTint;
            previousConeWidth = coneWidth;
            previousConeLength = coneLength;
            previousMaxConeLength = maxConeLength;
            previousGOBOSelection = selectGOBO;
            previousGlobalIntensity = globalIntensity;
            previousFinalIntensity = finalIntensity;
            if(withDMX)
            {
                _UpdateInstancedProperties();
            }
            else
            {
                _UpdateInstancedPropertiesSansDMX();
            }
        }
        else
        {
            Debug.Log("Please add atleast one fixture renderer.");
            //enableInstancing = false;
        }
    }
    public void _SetProps()
    {
        props = new MaterialPropertyBlock();
    }

    int SectorConversion()
    {
        int x =  Mathf.Abs(Mathf.FloorToInt((int) sector * 13) + 1);
       // = calculatedDMXChannel;//outgoing channel
       // float z = calculatedDMXChannel/512.0f;
        //Debug.Log(z);
        #if UNITY_EDITOR //ALL BELOW IS FOR INSPECTOR ONLY
        //TODO: FIND A BETTER WAY TO CALCULATE THIS
        calculatedDMXChannel = x;
        calculatedDMXUniverse = 1;
       // int u = Mathf.FloorToInt(z); //universe
       // int c = calculatedDMXChannel - ((u - 1) * 512);
       if(calculatedDMXChannel > 512 && calculatedDMXChannel < (512 * 2) + 8 )
       {
           calculatedDMXChannel -= (512+8); //universe 2
           calculatedDMXUniverse = 2;
       }
        else if (calculatedDMXChannel > (512 * 2) && calculatedDMXChannel < (512 * 3) + 13 )
       {
           calculatedDMXChannel -= ((512*2)+3) + (13 * 1); //universe 3
           calculatedDMXUniverse = 3;
       }
        else if (calculatedDMXChannel > 512 * 3 + 13 && calculatedDMXChannel < (512 * 4) + 21)
       {
           calculatedDMXChannel -= ((512*3)+11) + (13 * 1); // universe 4
           calculatedDMXUniverse = 4;
       }
        else if (calculatedDMXChannel > 512 * 4 + 21 && calculatedDMXChannel < (512 * 5) + 16 )
       {
           calculatedDMXChannel -= ((512*4)+6) + (13 * 2);
           calculatedDMXUniverse = 5;
       }
        else if (calculatedDMXChannel > 512 * 5 && calculatedDMXChannel < 512 * 6 )
       {
           calculatedDMXChannel -= ((512*5)+1);
           calculatedDMXUniverse = 6;
       }
        else if (calculatedDMXChannel > 512 * 6 && calculatedDMXChannel < 512 * 7 )
       {
           calculatedDMXChannel -= ((512*6)+9);
           calculatedDMXUniverse = 7;
       }
        else if (calculatedDMXChannel > 512 * 7 && calculatedDMXChannel < 512 * 8 )
       {
           calculatedDMXChannel -= ((512*7)+4);
           calculatedDMXUniverse = 8;
       }
        else if (calculatedDMXChannel > 512 * 8 && calculatedDMXChannel < 512 * 9 )
       {
           calculatedDMXChannel -= ((512*8)-1);
           calculatedDMXUniverse = 9;
       }
       #endif
      //  Debug.Log("Current Channel: " + x);
        return x;
    }

    int RawDMXConversion()
    {
            calculatedDMXChannel = dmxChannel;
            calculatedDMXUniverse = dmxUniverse;
            int chan = Mathf.Abs(dmxChannel + ((dmxUniverse-1) * 512) + ((dmxUniverse-1) * 8));
          //  Debug.Log("Channel: " + chan);
            return chan;
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
        if(useLegacySectorMode)
        {
            if(singleChannelMode)
            {
                // calculatedDMXChannel = Mathf.Abs(Mathf.FloorToInt((int) sector * 13) + 1) + Mathf.Abs(Channel);
                props.SetInt("_DMXChannel", SectorConversion() + Mathf.Abs(Channel));
            }
            else
            {
                props.SetInt("_DMXChannel", SectorConversion());
            }
            // calculatedDMXUniverse = Mathf.FloorToInt(calculatedDMXChannel / 512) + 1;
            // calculatedDMXChannel = calculatedDMXChannel - ((calculatedDMXUniverse - 1) * 512);
        }
        else
        {
            props.SetInt("_DMXChannel", RawDMXConversion());
        }

        props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
        props.SetInt("_LegacyGoboRange", legacyGoboRange == true ? 1 : 0);
        props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
        props.SetInt("_EnableStrobe", enableStrobe == true ? 1 : 0);
        props.SetInt("_EnableSpin", enableAutoSpin == true ? 1 : 0);
        props.SetInt("_EnableOSC", enableDMXChannels == true ? 1 : 0);
        props.SetInt("_ProjectionSelection", selectGOBO);
        props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
        props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
        props.SetColor("_Emission", lightColorTint);
        props.SetFloat("_ConeWidth", coneWidth);
        props.SetFloat("_GlobalIntensity", globalIntensity);
        props.SetFloat("_FinalIntensity", finalIntensity);
        props.SetFloat("_ConeLength", Mathf.Abs(coneLength - 10.5f));
        props.SetFloat("_MaxConeLength", maxConeLength);
        props.SetFloat("_MaxMinPanAngle", (maxMinPan/2.0f));
        props.SetFloat("_MaxMinTiltAngle", (maxMinTilt/2.0f));
        switch(objRenderers.Length)
        {
            case 1:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                break;
            case 2:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[0])
                    objRenderers[1].SetPropertyBlock(props);
                break;
            case 3:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[1])
                    objRenderers[1].SetPropertyBlock(props);
                if(objRenderers[2])
                    objRenderers[2].SetPropertyBlock(props);
                break;
            case 4:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[1])
                    objRenderers[1].SetPropertyBlock(props);
                if(objRenderers[2])
                    objRenderers[2].SetPropertyBlock(props);
                if(objRenderers[3])
                    objRenderers[3].SetPropertyBlock(props);
                break;
            case 5:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[1])
                    objRenderers[1].SetPropertyBlock(props);
                if(objRenderers[2])
                    objRenderers[2].SetPropertyBlock(props);
                if(objRenderers[3])
                    objRenderers[3].SetPropertyBlock(props);
                if(objRenderers[4])
                    objRenderers[4].SetPropertyBlock(props);
                break;
            default:
                Debug.Log("Too many mesh renderers for this fixture!");
                break;   
        }
    }
    public void _UpdateInstancedPropertiesSansDMX()
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
        if(useLegacySectorMode)
        {
            if(singleChannelMode)
            {
                // calculatedDMXChannel = Mathf.Abs(Mathf.FloorToInt((int) sector * 13) + 1) + Mathf.Abs(Channel);
                props.SetInt("_DMXChannel", SectorConversion() + Mathf.Abs(Channel));
            }
            else
            {
                props.SetInt("_DMXChannel", SectorConversion());
            }
            // calculatedDMXUniverse = Mathf.FloorToInt(calculatedDMXChannel / 512) + 1;
            // calculatedDMXChannel = calculatedDMXChannel - ((calculatedDMXUniverse - 1) * 512);
        }
        else
        {
            props.SetInt("_DMXChannel", RawDMXConversion());
        }
        props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
        props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
        props.SetInt("_LegacyGoboRange", legacyGoboRange == true ? 1 : 0);
        props.SetInt("_EnableStrobe", 0);
        props.SetInt("_EnableSpin", enableAutoSpin == true ? 1 : 0);
        props.SetInt("_EnableOSC", 0);
        props.SetInt("_ProjectionSelection", selectGOBO);
        props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
        props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
        props.SetColor("_Emission", lightColorTint);
        props.SetFloat("_ConeWidth", coneWidth);
        props.SetFloat("_GlobalIntensity", globalIntensity);
        props.SetFloat("_FinalIntensity", finalIntensity);
        props.SetFloat("_ConeLength", Mathf.Abs(coneLength - 10.5f));
        props.SetFloat("_MaxConeLength", maxConeLength);
        props.SetFloat("_MaxMinPanAngle", (maxMinPan/2.0f));
        props.SetFloat("_MaxMinTiltAngle", (maxMinTilt/2.0f));
        switch(objRenderers.Length)
        {
            case 1:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                break;
            case 2:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[0])
                    objRenderers[1].SetPropertyBlock(props);
                break;
            case 3:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[1])
                    objRenderers[1].SetPropertyBlock(props);
                if(objRenderers[2])
                    objRenderers[2].SetPropertyBlock(props);
                break;
            case 4:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[1])
                    objRenderers[1].SetPropertyBlock(props);
                if(objRenderers[2])
                    objRenderers[2].SetPropertyBlock(props);
                if(objRenderers[3])
                    objRenderers[3].SetPropertyBlock(props);
                break;
            case 5:
                if(objRenderers[0])
                    objRenderers[0].SetPropertyBlock(props);
                if(objRenderers[1])
                    objRenderers[1].SetPropertyBlock(props);
                if(objRenderers[2])
                    objRenderers[2].SetPropertyBlock(props);
                if(objRenderers[3])
                    objRenderers[3].SetPropertyBlock(props);
                if(objRenderers[4])
                    objRenderers[4].SetPropertyBlock(props);
                break;
            default:
                Debug.Log("Too many mesh renderers for this fixture!");
                break;  
        }
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
            return ConeLength;
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
            return MaxConeLength;
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
    public bool InvertPan
    {
        get
        {
            return invertPan;
        }
        set
        {
            invertPan = value;
            _UpdateInstancedProperties();
        }
    }
    public bool InvertTilt
    {
        get
        {
            return invertTilt;
        }
        set
        {
            invertTilt = value;
            _UpdateInstancedProperties();
        }
    }
    public bool IsDMX
    {
        get
        {
            return enableDMXChannels;
        }
        set
        {
            enableDMXChannels = value;
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
        public float Pan
    {
        get
        {
            return panOffsetBlueGreen;
        }
        set
        {
            panOffsetBlueGreen = value;
            _UpdateInstancedProperties();
        }
    }
    public float Tilt
    {
        get
        {
            return tiltOffsetBlue;
        }
        set
        {
            tiltOffsetBlue = value;
            _UpdateInstancedProperties();
        }
    }

    public string _DMXChannelToString()
    {
        return "DMX Channel: " + calculatedDMXChannel + "  Universe: " + calculatedDMXUniverse;
    }

    public int _GetUniverse()
    {
        return calculatedDMXUniverse;
    }
    public int _GetDMXChannel()
    {
        return calculatedDMXChannel;
    }
/////////////////////////////////////////////////////////////////////////END PROPERTIES///////////////////////////////////////////////////////////////////////////////////////////////

#if !COMPILER_UDONSHARP && UNITY_EDITOR
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