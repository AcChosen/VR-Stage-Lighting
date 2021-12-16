

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

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]

public class VRStageLighting_Animated_Static : UdonSharpBehaviour
{
     //////////////////Public Variables////////////////////
    [Header("Audio Link Settings")]
    [Tooltip("Enable or disable Audio Link Reaction for this fixture.")]
    public bool enableAudioLink;


    //[Tooltip("The Audio Link Script to react to.")]
    //public AudioLink audioLink;


    [Tooltip("The frequency band of the spectrum to react to.")]
    public int band;


    [Range(0, 31)]
    [Tooltip("The level of delay to add to the reaction.")]
    public int delay;


    [Tooltip("Multiplier for the sensativity of the reaction.")]
    [Range(1.0f, 15.0f)]
    public float bandMultiplier = 1.0f;


    [Header("Animation Settings")]
    [Tooltip("Enable or disable the use of the animator on this fixture.")]
    public bool enableAnimation;

    [Tooltip("Mirror the animations on this fixture.")]
    public bool mirrorAnimations;
    [Tooltip("The animator to animate this fixture.")]
    public Animator animator;
    [Tooltip ("Set the color, intensity, and gobo animation.")]
    public int _colorAnimation, _intensityAnimation, _goboAnimation;
    [Tooltip ("Sets the color of the light when Color Animation = 0. ")]
    [Range(0,1)]
    public float _solidColorAnimSelction;
    [Tooltip ("Set the which pan tilt animation set to use (1 bar, 2 bar, or 4 bar. 3 will auto set it to 4).")]
    [Range(1,4)]
    public int _panTiltMeasureCount = 1;
    [Tooltip ("Set the 1 bar, 2 bar, and 4 bar pan tilt animations.")]
    public int _panTiltSingleMeasureAnim, _panTiltDualMeasureAnim, _panTiltQuadMeasureAnim;
    [Tooltip ("Speed of the animations. (1.0 = 60bpm)")]
    public float animationSpeed = 1.0f;
    [Tooltip ("BPM Counter for controlling animation speed/timing.")]
    public VRSL_BPMCounter bPMCounter;

    [Space(5)]

    [Header("General Settings")]
    [Range(0,1)]
    [Tooltip ("Sets the overall intensity of the shader. Good for animating or scripting effects related to intensity. Its max value is controlled by Final Intensity.")]
    public float globalIntensity = 1; 
    [Range(0,1)]
    [Tooltip ("Sets the maximum brightness value of Global Intensity. Good for personalized settings of the max brightness of the shader by other users via UI.")]
    public float finalIntensity = 1;
    [Tooltip ("The main color of the light.")]
    [ColorUsage(false,true)]
    public Color lightColorTint = Color.white * 2.0f;

    [Tooltip ("Check this box if you wish to sample seperate texture for the color. The color will be influenced by the intensity of the original emission color! The texture is set in the shader itself.")]
    public bool enableColorTextureSampling;


    [Tooltip ("The UV coordinates to sample the color from on the texture.")]
    public Vector2 textureSamplingCoordinates = new Vector2(0.5f, 0.5f);
    [Space(5)]


    [Header("Movement Settings")]
    // [Tooltip ("Invert the pan values (Left/Right Movement) for movers.")]
    // public bool invertPan;
    // [Tooltip ("Invert the tilt values (Up/Down Movement) for movers.")]
    // public bool invertTilt;
    // [Tooltip ("Enable this if the mover is hanging upside down.")]
    // public bool isUpsideDown;
    // [Tooltip ("Enable target following for a mover while in Udon mode.")]
    // public bool followTarget;
    [Tooltip ("The target for this mover to follow.")]
    public Transform targetToFollow;
    // [Tooltip ("The speed at which the target is followed, higher = slower.")]
    // public float targetFollowLerpSpeed = 20.0f;
    // [Tooltip ("An offset of where the mover will point at relative to the target.")]
    // public Vector3 targetOffset;
    [Space(5)]


    [Header("Fixture Settings")]
    [Tooltip ("Enable projection spinning (Udon Override Only).")]
    public bool enableAutoSpin;
    // [Range(0,360.0f)]
    // [Tooltip ("Tilt (Up/Down) offset/movement. Directly controls tilt when in Udon Mode.")]
    // public float tiltOffsetBlue = 90.0f;
    // float startTiltOffset;

    // [Range(0,360.0f)]
    // [Tooltip ("Pan (Left/Right) offset/movement. Directly controls pan when in Udon Mode; is an offset when in DMX mode.")]
    // public float panOffsetBlueGreen = 0.0f;
    // float startPanOffset;
    [Range(1,6)]
    [Tooltip ("Use this to change what projection is selected")]
    public int selectGOBO = 1;


    [Header("Mesh Settings")]
    [Tooltip ("The meshes used to make up the light. You need atleast 1 mesh in this group for the script to work properly.")]
    public MeshRenderer[] objRenderers;

    [Tooltip ("Controls the radius of a mover/spot light.")]
    [Range(0, 5.5f)]
    public float coneWidth = 2.5f;

    [Range(0.001f,10.0f)]
    [Tooltip ("Controls the length of the cone of a mover/spot light.")]
    public float coneLength = 8.5f; 

    [Range(0.275f,10.0f)]
    [Tooltip ("Controls the mesh length of the cone of a mover/spot light")]
    [SerializeField]
    private float maxConeLength = 1.0f;

    [ColorUsage(true, true)]
    

    /////////////////Private Variables//////////////////
    
    MaterialPropertyBlock props;
    //bool enableInstancing;
    // float targetPanAngle, targetTiltAngle;
    // private Vector3 targetToFollowLast;
    private bool wasChanged;

    void Start()
    {
        if(objRenderers.Length > 0 && objRenderers[0] != null)
        {
            _SetProps();
            
            //enableInstancing = true;
            _UpdateInstancedProperties();
        }
        else
        {
            Debug.Log("Please add atleast one fixture renderer.");
            //enableInstancing = false;
        }
        if(animator)
        {
            animator.SetBool("Mirror", mirrorAnimations);
        }


        // if(followTarget)
        // {
        //     GetTargetAngles();
        //     LerpToDefaultTarget();
        //     targetToFollowLast = targetToFollow.position;
        // }
    }

    public void _SetProps()
    {
        props = new MaterialPropertyBlock();
    }
    void CheckAnimator()
    {
        if(bPMCounter)
        {
            if(bPMCounter._quarterNoteCount == 1 && bPMCounter._beatFull || bPMCounter.resetCounter)
            {
                animationSpeed = bPMCounter.newAnimationSpeed;
                animator.speed = animationSpeed;
                RestartAnimation();
            }
            _UpdateIntensityAnims();
            _UpdateGoboAnims();
            _UpdateColorAnims();
            _UpdatePanTiltAnims();
            animator.SetBool("Mirror", mirrorAnimations);
        }
        else
        {
            Debug.Log("Please assign a BPM Counter object to this animated fixture!");
        }     
    }
    public void _UpdateSliders()
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
        props.SetFloat("_FinalIntensity", Mathf.Clamp01(finalIntensity));
        props.SetFloat("_ConeWidth", coneWidth);
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

    public void CheckBPM()
    {
        if(bPMCounter)
        {
            //animator.speed = animationSpeed;

            if(bPMCounter._quarterNoteCount == 1 && bPMCounter._beatFull || bPMCounter.resetCounter)
            {
                animationSpeed = bPMCounter.newAnimationSpeed;
                animator.speed = animationSpeed;
                RestartAnimation();
            }
            _UpdateIntensityAnims();
            _UpdateGoboAnims();
            _UpdateColorAnims();
            _UpdatePanTiltAnims();
        }
        else
        {
            Debug.Log("Please assign a BPM Counter object to this animated fixture!");
        }
    }

    public void _DownBeat()
    {
        if(!bPMCounter)
        {
            Debug.Log("Please assign a BPM Counter object to this animated fixture!");
            return;
        }
        if(bPMCounter._quarterNoteCount == 1 && bPMCounter._beatFull || bPMCounter.resetCounter)
        {
            animationSpeed = bPMCounter.newAnimationSpeed;
            animator.speed = animationSpeed;
            RestartAnimation();
        }
        _UpdateAnimations();
    }

    public void RestartAnimation()
    {
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash,0,0.0f); //Restart Intensity Anim
        animator.Play(animator.GetCurrentAnimatorStateInfo(1).shortNameHash,1,0.0f); //Restart Color Anim
        //Restart PanTilt Anim
        if(_panTiltMeasureCount == 1)
        {
            animator.Play(animator.GetCurrentAnimatorStateInfo(3).shortNameHash,3,0.0f);
            return;
        }
        if(_panTiltMeasureCount == 2)
        {
            if(bPMCounter._measureCount == 1 || bPMCounter._measureCount == 3)
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(3).shortNameHash,3,0.0f);
                return;
            }
            else
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(3).shortNameHash,3,0.5f);
                return;
            }
        }
        if(_panTiltMeasureCount == 4 && bPMCounter._measureCount > 3)
        {
                animator.Play(animator.GetCurrentAnimatorStateInfo(3).shortNameHash,3,0.0f);
                return;
        }
        //animator.CrossFade(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0.0f, 0, 0.0f, 0.0f);
    }

    public void _UpdateAnimations()
    {
        animator.SetFloat("IntensitySelection",_intensityAnimation);
        animator.SetFloat("GoboSelection",_goboAnimation);
        animator.SetFloat("ColorSelection",_colorAnimation);
        animator.SetFloat("SolidColorBlend",_solidColorAnimSelction);
        animator.SetFloat("PanTiltMeasureLength",_panTiltMeasureCount);
        animator.SetFloat("PanTilt-1Measure", _panTiltSingleMeasureAnim);
        animator.SetFloat("PanTilt-2Measures", _panTiltDualMeasureAnim);
        animator.SetFloat("PanTilt-4Measures", _panTiltQuadMeasureAnim);
        animator.SetBool("Mirror", mirrorAnimations);
    }
    public void _UpdateColorAnims()
    {
        animator.SetFloat("ColorSelection",_colorAnimation);
        animator.SetFloat("SolidColorBlend",_solidColorAnimSelction);
    }
    public void _UpdateGoboAnims()
    {
        animator.SetFloat("GoboSelection",_goboAnimation);
    }
    public void _UpdateIntensityAnims()
    {
        switch(_intensityAnimation)
        {
            case 8:
                animator.SetLayerWeight(1,0.0f);
                animator.SetFloat("IntensitySelection",0);
                _AudioLinkOn(0);
                break;
            case 9:
                animator.SetLayerWeight(1,0.0f);
                animator.SetFloat("IntensitySelection",0);
                _AudioLinkOn(1);
                break;
            case 10:
                animator.SetLayerWeight(1,0.0f);
                animator.SetFloat("IntensitySelection",0);
                _AudioLinkOn(2);
                break;
            case 11:
                animator.SetLayerWeight(1,0.0f);
                animator.SetFloat("IntensitySelection",0);
                _AudioLinkOn(3);
                break;
            default:
                animator.SetLayerWeight(1,1.0f);
                animator.SetFloat("IntensitySelection",_intensityAnimation);
                _AudioLinkOff();
                break;
        }
    }
    public void _UpdatePanTiltAnims()
    {
        animator.SetFloat("PanTiltMeasureLength",_panTiltMeasureCount);
        animator.SetFloat("PanTilt-1Measure", _panTiltSingleMeasureAnim);
        animator.SetFloat("PanTilt-2Measures", _panTiltDualMeasureAnim);
        animator.SetFloat("PanTilt-4Measures", _panTiltQuadMeasureAnim);
    }
    // void ChangeCheckPosition()
    // {
    //     GetTargetAngles();
    //     if((targetToFollow.position != targetToFollowLast) || (panOffsetBlueGreen != targetPanAngle || tiltOffsetBlue != targetTiltAngle))
    //     {
    //         LerpToDefaultTarget();
    //         targetToFollowLast = targetToFollow.position;
    //     }
        
    //     targetToFollow.transform.hasChanged = false;
    //     wasChanged = true;
    //     return;
    // }

    void _AudioLinkOn(int inputBand)
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
        enableAudioLink = true;
        band = inputBand;
        props.SetFloat("_EnableAudioLink", enableAudioLink == true ? 1.0f : 0.0f);
        props.SetFloat("_Band", band);
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
    void _AudioLinkOff()
    {
        if(!enableAudioLink)
        {
            return;
        }
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
        enableAudioLink = false;
        props.SetFloat("_EnableAudioLink", enableAudioLink == true ? 1.0f : 0.0f);
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



    public void _SetAnimationSpeed(float speed)
    {
        animationSpeed = speed;
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
        //AudioLink Stuff
        props.SetFloat("_EnableAudioLink", enableAudioLink == true ? 1.0f : 0.0f);
        props.SetFloat("_Delay", delay);
        props.SetFloat("_BandMultiplier", bandMultiplier);
        props.SetFloat("_Band", band);
        //Movement Stuff
        // props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
        // props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
        props.SetInt("_EnableSpin", enableAutoSpin == true ? 1 : 0);
        props.SetInt("_ProjectionSelection", selectGOBO);
        // props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
        // props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
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

    public void _UpdateInstancedPropertiesSansAudioLink()
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
        //AudioLink Stuff
        props.SetFloat("_EnableAudioLink", 0.0f);
        props.SetFloat("_Delay", delay);
        props.SetFloat("_BandMultiplier", bandMultiplier);
        props.SetFloat("_Band", band);
        //Movement Stuff
        // props.SetInt("_PanInvert", invertPan == true ? 1 : 0);
        // props.SetInt("_TiltInvert", invertTilt == true ? 1 : 0);
        props.SetInt("_EnableSpin", enableAutoSpin == true ? 1 : 0);
        props.SetInt("_ProjectionSelection", selectGOBO);
        // props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
        // props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
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

    

    // void _UpdateInstancedPropertiesPanTilt()
    // {
    //     props.SetFloat("_FixtureRotationX", tiltOffsetBlue);
    //     props.SetFloat("_FixtureBaseRotationY", panOffsetBlueGreen);
    //     switch(objRenderers.Length)
    //     {
    //         case 1:
    //             objRenderers[0].SetPropertyBlock(props);
    //             break;
    //         case 2:
    //             objRenderers[0].SetPropertyBlock(props);
    //             objRenderers[1].SetPropertyBlock(props);
    //             break;
    //         case 3:
    //             objRenderers[0].SetPropertyBlock(props);
    //             objRenderers[1].SetPropertyBlock(props);
    //             objRenderers[2].SetPropertyBlock(props);
    //             break;
    //         case 4:
    //             objRenderers[0].SetPropertyBlock(props);
    //             objRenderers[1].SetPropertyBlock(props);
    //             objRenderers[2].SetPropertyBlock(props);
    //             objRenderers[3].SetPropertyBlock(props);
    //             break;
    //         case 5:
    //             objRenderers[0].SetPropertyBlock(props);
    //             objRenderers[1].SetPropertyBlock(props);
    //             objRenderers[2].SetPropertyBlock(props);
    //             objRenderers[3].SetPropertyBlock(props);
    //             objRenderers[4].SetPropertyBlock(props);
    //             break;
    //         default:
    //             Debug.Log("Too many mesh renderers for this fixture!");
    //             break;  
    //     } 
    // }
    // void GetTargetAngles()
    // {
    //         Vector3 dir = ((targetToFollow.position + targetOffset) - this.transform.position).normalized;
    //         float dist = Vector3.Distance(targetToFollow.position,this.transform.position);       
    //         Quaternion rot = Quaternion.LookRotation(dir,Vector3.up);
    //         Vector3 angles = rot.eulerAngles;
    //         targetPanAngle = invertPan == true ? (1.0f-((angles.y))*-1.0f) : angles.y;
    //         targetTiltAngle = invertTilt == true ? (angles.x-180.0f)*-1.0f : angles.x;
    //         targetPanAngle = isUpsideDown == false ? targetPanAngle+180.0f : targetPanAngle;
    //         targetTiltAngle = isUpsideDown == false ? -targetTiltAngle : targetTiltAngle;
    //         targetPanAngle = targetPanAngle - this.transform.localEulerAngles.y;
    //         return;
    // }
    // void LerpToDefaultTarget()
    // {
    //         float previousPanAngle = panOffsetBlueGreen;
    //         float previousTiltAngle = tiltOffsetBlue;
    //         panOffsetBlueGreen = Mathf.LerpAngle(previousPanAngle, targetPanAngle, targetFollowLerpSpeed * Time.deltaTime);
    //         tiltOffsetBlue = Mathf.Clamp(Mathf.LerpAngle(previousTiltAngle, targetTiltAngle, targetFollowLerpSpeed * Time.deltaTime),0.0f, 180.0f);
    //         return;
    // }
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
