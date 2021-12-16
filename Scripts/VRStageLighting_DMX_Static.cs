
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
//using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using System.Collections.Immutable;
#endif

public class VRStageLighting_DMX_Static : UdonSharpBehaviour
{
    //////////////////Public Variables////////////////////
    [Header("DMX Settings")]
    [Tooltip ("Enables DMX mode for this fixture.")]
    public bool enableDMXChannels = true;
    [Tooltip ("Enables single channel DMX mode for this fixture. This is for single channeled fixtures instead of the standard 13-channeled ones. Currently, the 'Flasher' fixture is the only single-channeled fixture at the moment")]
    public bool singleChannelMode = false;
    [Tooltip ("Chooses the DMX Address to start this fixture at. A Sector in this context is every 13 Channels. I.E Sector 0 is channels 1-13, Sector 1 is channels 14-26, etc.")]
    public uint sector;
    [Tooltip ("Chooses the which of the 13 Channels of the current sector to sample from when single channel mode is enabled. Do not worry about this value if you are not using a single-channeled fixture.")]
    [Range(0.0f, 12.0f)]
    public uint channel = 0;
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
    [Tooltip ("Enable target following for a mover while in Udon mode.")]
    public bool followTarget;
    [Tooltip ("The target for this mover to follow.")]
    public Transform targetToFollow;
    [Tooltip ("The speed at which the target is followed, higher = slower.")]
    public float targetFollowLerpSpeed = 20.0f;
    [Tooltip ("An offset of where the mover will point at relative to the target.")]
    public Vector3 targetOffset;
    [Space(5)]
    [Header("Fixture Settings")]
    [Tooltip ("Enable projection spinning (Udon Override Only).")]
    public bool enableAutoSpin;
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
    [Range(1,6)]
    [Tooltip ("Use this to change what projection is selected. This is overridden in DMX mode.")]
    public int selectGOBO = 1;
    
    [Header("Mesh Settings")]
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
    [FieldChangeCallback(nameof(MaxConeLength))]
    [SerializeField]
    private float maxConeLength = 1.0f;  
    [ColorUsage(true, true)]

    

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

    void Start()
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
            _UpdateInstancedProperties();
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
        props.SetInt("_Sector", (int) sector);
        if(singleChannelMode)
        {
            props.SetInt("_Channel", (int) channel);
        }
        props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
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
        switch(objRenderers.Length)
        {
            case 1:
                objRenderers[0].SetPropertyBlock(props);
                break;
            case 2:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                break;
            case 3:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                objRenderers[2].SetPropertyBlock(props);
                break;
            case 4:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                objRenderers[2].SetPropertyBlock(props);
                objRenderers[3].SetPropertyBlock(props);
                break;
            case 5:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                objRenderers[2].SetPropertyBlock(props);
                objRenderers[3].SetPropertyBlock(props);
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
        props.SetInt("_Sector", (int) sector);
        if(singleChannelMode)
        {
            props.SetInt("_Channel", (int) channel);
        }
        props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
        props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
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
        switch(objRenderers.Length)
        {
            case 1:
                objRenderers[0].SetPropertyBlock(props);
                break;
            case 2:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                break;
            case 3:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                objRenderers[2].SetPropertyBlock(props);
                break;
            case 4:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                objRenderers[2].SetPropertyBlock(props);
                objRenderers[3].SetPropertyBlock(props);
                break;
            case 5:
                objRenderers[0].SetPropertyBlock(props);
                objRenderers[1].SetPropertyBlock(props);
                objRenderers[2].SetPropertyBlock(props);
                objRenderers[3].SetPropertyBlock(props);
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

/////////////////////////////////////////////////////////////////////////END PROPERTIES///////////////////////////////////////////////////////////////////////////////////////////////
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnDrawGizmos()
        {
            this.UpdateProxy(ProxySerializationPolicy.RootOnly);
            Gizmos.color = lightColorTint;
            if(targetToFollow != null)
            {
                Gizmos.DrawWireSphere(targetToFollow.position, 0.25f);
            }
            Gizmos.DrawWireSphere(transform.position, 0.05f);
        }
    #endif
}