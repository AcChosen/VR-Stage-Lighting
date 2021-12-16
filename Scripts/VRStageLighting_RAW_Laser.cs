
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

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class VRStageLighting_RAW_Laser : UdonSharpBehaviour
{
    [Header("General Settings")]
    [Range(0,1)]
    [Tooltip ("Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity.")]
    [FieldChangeCallback(nameof(GlobalIntensity))]
    [SerializeField]
    private float globalIntensity = 1; 


    [Range(0,1)]
    [Tooltip ("Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI.")]
    [FieldChangeCallback(nameof(FinalIntensity))]
    [SerializeField]
    private float finalIntensity = 1;


    [Tooltip ("The main color of the light.")]
    [ColorUsage(false,false)]
    [FieldChangeCallback(nameof(LightColorTint))]
    [SerializeField]
    private Color lightColorTint = Color.white * 2.0f;


    [Tooltip ("Check this box if you wish to sample seperate texture for the color. The color will be influenced by the intensity of the original emission color! The texture is set in the shader itself.")]
    [FieldChangeCallback(nameof(ColorTextureSampling))]
    [SerializeField]
    private bool enableColorTextureSampling;

    [Tooltip ("The UV coordinates to sample the color from on the texture.")]
    [FieldChangeCallback(nameof(TextureSamplingCoordinates))]
    [SerializeField]
    private Vector2 textureSamplingCoordinates = new Vector2(0.5f, 0.5f);

    [Tooltip ("Controls the radius of the laser cone.")]
    [Range(-3.75f, 20.0f)]
    [FieldChangeCallback(nameof(ConeWidth))]
    [SerializeField]
    private float coneWidth = 2.5f;

    [Range(-0.5f,5.0f)]
    [Tooltip ("Controls the length of the laser cone")]
    [FieldChangeCallback(nameof(ConeLength))]
    [SerializeField]
    private float coneLength = 8.5f; 

    [Range(0.0f,1.999f)]
    [Tooltip ("Controls how flat or round the cone is.")]
    [FieldChangeCallback(nameof(ConeFlatness))]
    [SerializeField]
    private float coneFlatness = 0.0f;

    [Range(-90.0f,90.0f)]
    [Tooltip ("X rotation coffset for cone")]
    [FieldChangeCallback(nameof(ConeXRotation))]
    [SerializeField]
    private float coneXRotation = 0.0f; 

    [Range(-90.0f,90.0f)]
    [Tooltip ("Y rotation offset for cone")]
    [FieldChangeCallback(nameof(ConeYRotation))]
    [SerializeField]
    private float coneYRotation = 0.0f;

    [Range(-90.0f,90.0f)]
    [Tooltip ("Z rotation offset for cone")]
    [FieldChangeCallback(nameof(ConeZRotation))]
    [SerializeField]
    private float coneZRotation = 0.0f;  

    [Range(4.0f,68f)]
    [Tooltip ("Number of laser beams in cone")]
    [FieldChangeCallback(nameof(LaserCount))]
    [SerializeField]
    private int laserCount = 14;  

    [Range(0.003f,0.25f)]
    [Tooltip ("Controls how thick/thin the lasers are")]
    [FieldChangeCallback(nameof(LaserThickness))]
    [SerializeField]
    private float laserThickness = 0.125f; 

    [Range(-1.0f,1.0f)]
    [Tooltip ("Controls the speed of laser scroll animation. Negative goes left, positive goes right, 0 means no scroll")]
    [FieldChangeCallback(nameof(LaserScroll))]
    [SerializeField]
    private float laserScroll = 0.0f; 


 



    [Header("Mesh Settings")]
    [Tooltip ("The meshes used to make up the light. You need atleast 1 mesh in this group for the script to work properly.")]
    public MeshRenderer[] objRenderers;




    private float previousConeWidth, previousConeLength, previousGlobalIntensity, previousFinalIntensity, previousConeFlatness, previousConeXRotation, previousConeYRotation, previousConeZRotation, previousLaserThickness, previousLaserScroll;
    private int previousLaserCount;
    private Color previousColorTint;
    MaterialPropertyBlock props;



    ////////////////////////////////////////////////////////////////////////////////
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
    
    public float ConeFlatness
    {
        get
        {
            return coneFlatness;
        }
        set
        {
            previousConeFlatness = coneFlatness;
            coneFlatness = value;
            _UpdateInstancedProperties();
        }
    }
    public float ConeXRotation
    {
        get
        {
            return coneXRotation;
        }
        set
        {
            previousConeXRotation = coneXRotation;
            coneXRotation = value;
            _UpdateInstancedProperties();
        }
    }
    public float ConeYRotation
    {
        get
        {
            return coneYRotation;
        }
        set
        {
            previousConeYRotation = coneYRotation;
            coneYRotation = value;
            _UpdateInstancedProperties();
        }
    }
    public float ConeZRotation
    {
        get
        {
            return coneZRotation;
        }
        set
        {
            previousConeZRotation = coneZRotation;
            coneZRotation = value;
            _UpdateInstancedProperties();
        }
    }
    public int LaserCount
    {
        get
        {
            return laserCount;
        }
        set
        {
            previousLaserCount = laserCount;
            laserCount = value;
            _UpdateInstancedProperties();
        }
    }
    public float LaserThickness
    {
        get
        {
            return laserThickness;
        }
        set
        {
            previousLaserThickness = laserThickness;
            laserThickness = value;
            _UpdateInstancedProperties();
        }
    }
    public float LaserScroll
    {
        get
        {
            return laserScroll;
        }
        set
        {
            previousLaserScroll = laserScroll;
            laserScroll = value;
            _UpdateInstancedProperties();
        }
    }
    public bool ColorTextureSampling
    {
        get
        {
            return enableColorTextureSampling;
        }
        set
        {
            enableColorTextureSampling = value;
            _UpdateInstancedProperties();
        }
    }
    public Vector2 TextureSamplingCoordinates
    {
        get
        {
            return textureSamplingCoordinates;
        }
        set
        {
            textureSamplingCoordinates = value;
            _UpdateInstancedProperties();
        }
    }

    ///////////////////////////////////////////////////////////

    void Start()
    {
        if(objRenderers.Length > 0 && objRenderers[0] != null)
        {
            _SetProps();
            previousColorTint = lightColorTint;
            previousConeWidth = coneWidth;
            previousConeLength = coneLength;
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
        //Color Texture Sampling
        props.SetFloat("_TextureColorSampleX", textureSamplingCoordinates.x);
        props.SetFloat("_TextureColorSampleY", textureSamplingCoordinates.y);
        props.SetInt("_EnableColorTextureSample", enableColorTextureSampling == true ? 1 : 0);
        //General Light Stuff
        props.SetColor("_Emission", lightColorTint);
        props.SetFloat("_VertexConeWidth", coneWidth);
        props.SetFloat("_GlobalIntensity", globalIntensity);
        props.SetFloat("_FinalIntensity", finalIntensity);
        props.SetFloat("_VertexConeLength", coneLength);
        props.SetFloat("_ZConeFlatness", coneFlatness);
        props.SetFloat("_XRotation", coneXRotation);
        props.SetFloat("_YRotation", coneYRotation);
        props.SetFloat("_ZRotation", coneZRotation);
        props.SetInt("_LaserCount", laserCount);
        props.SetFloat("_LaserThickness", laserThickness);
        props.SetFloat("_Scroll", laserScroll);
        // for(int i = 0; i < objRenderers.Length; i++)
        // {
        //     objRenderers[i].SetPropertyBlock(props);
        // }
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

}
