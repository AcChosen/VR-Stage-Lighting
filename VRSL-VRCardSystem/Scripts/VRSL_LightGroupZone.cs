
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_LightGroupZone : UdonSharpBehaviour
{
    [SerializeField]
    bool hasColorAnim, hasIntensityAnim, hasGOBOAnim;
   [SerializeField]
    int colorAnim, intensityAnim, gOBOAnim, panTiltAnim;
    [UdonSynced]
    public string animSync = "001001001001";
    [UdonSynced]
    public bool followDefaultTargetSync = true;

    [SerializeField]
    VRSL_CardObject colorCard, intensityCard, goboCard, panTiltCard;

    public VRStageLighting_Animated[] stageLightsList;
    public VRSL_SyncedSlider finalIntensitySlider;
    public VRSL_SyncedSlider coneWidthSlider;
    public VRSL_SyncedSlider masterIntensitySlider;
    public float intensityCurve = 1.0f;
    int countDownToInitialize = 10;
    bool isCounting;

    void Start()
    {
        hasColorAnim = hasIntensityAnim = hasGOBOAnim = false;
        //UpdateSliders();
        countDownToInitialize = 10;
        isCounting = true;
        animSync = colorAnim.ToString("D3") + intensityAnim.ToString("D3") + gOBOAnim.ToString("D3") + panTiltAnim.ToString("D3");

    }

    void Update() 
    {
        if(colorCard != null)
        {
            if(colorCard.rotationMod)
            {
                foreach(VRStageLighting_Animated light in stageLightsList)
                {
                    light._solidColorAnimSelction = colorCard.rotationValue;
                }
            }
        }
        if(isCounting)
        {
            if(countDownToInitialize != 0)
            {
                countDownToInitialize--;
            }
            else
            {
                UpdateSliders();
                isCounting = false;
            }
        }
        //UpdateSliders();
    }

    public void UpdateSliders()
    {

        foreach(VRStageLighting_Animated light in stageLightsList)
        {
            light.coneWidth = Mathf.Lerp(0.0f, 5.5f, coneWidthSlider.value);
            //light._UpdateProps();
        }

        foreach(VRStageLighting_Animated light in stageLightsList)
        {
            light.finalIntensity = Mathf.Lerp(0.0f, 1.0f, Mathf.Clamp(Mathf.Pow(finalIntensitySlider.value, intensityCurve),0.0f, Mathf.Pow(masterIntensitySlider.value, intensityCurve)));
            //light._UpdateProps();
        }
    
    }
    void OnTriggerEnter(Collider other) 
    {
       VRSL_CardObject card = (VRSL_CardObject) other.gameObject.GetComponent<VRSL_CardObject>();
       if(card != null)
       {
            switch(card.CardType)
            {
                case 1:
                        if(colorCard != null)
                        {
                            colorCard.ReturnToStart();       
                        }
                        hasColorAnim = true;
                        colorAnim =  card.CardID-1;
                        colorCard = card;
                        UpdateStageLightAnims(card);
                        break;
                        
                case 2:
                        if(intensityCard != null)
                        {
                            intensityCard.ReturnToStart();
                        }
                        hasIntensityAnim = true;
                        intensityAnim = card.CardID;
                        intensityCard = card;
                        UpdateStageLightAnims(card);
                        break;

                case 3:
                        if(goboCard != null)
                        {
                            goboCard.ReturnToStart();
                        }
                        hasGOBOAnim = true;
                        gOBOAnim = card.CardID;
                        goboCard = card;
                        UpdateStageLightAnims(card);
                        break;

                case 4:
                        if(panTiltCard != null)
                        {
                            panTiltCard.ReturnToStart();
                        }
                        //hasPanTiltAnim = true;
                        panTiltAnim = card.CardID;
                        panTiltCard = card;
                        UpdateStageLightAnims(card);
                        break;

                default:
                        Debug.Log("Card type not found...");
                        break;
            }
       }
        
    }

    void OnTriggerExit(Collider other) 
    {
        VRSL_CardObject card = (VRSL_CardObject) other.gameObject.GetComponent<VRSL_CardObject>();
        if(card != null)
        {
            switch(card.CardType)
            {
                case 1:
                        hasColorAnim = false;
                        colorAnim = 0;
                        colorCard = null;
                        break;
                        
                case 2:
                        hasIntensityAnim = false;
                        intensityAnim = 0;
                        intensityCard = null;
                        break;

                case 3:
                        hasGOBOAnim = false;
                        gOBOAnim = 0;
                        goboCard = null;
                        break;

                case 4:
                        //hasPanTiltAnim = false;
                        panTiltAnim = 0;
                        panTiltCard = null;
                        break;

                default:
                        Debug.Log("Card type not found...");
                        break;
            }
        }
    }
    //For late joiners...
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(Networking.LocalPlayer == player)
        {
            SynAnimVars();
        }
    }
    //For late joiners...
    void SynAnimVars()
    {
        //if(colorAnimSync >= 1)
            colorAnim = int.Parse(animSync.Substring(0,3));
        //if(intensityAnimSync >= 1)
            intensityAnim = int.Parse(animSync.Substring(4,2));
        //if(panTiltAnimSync >= 1)
            panTiltAnim = int.Parse(animSync.Substring(6,3));
        //if(gOBOAnimSync >= 1)
            gOBOAnim = int.Parse(animSync.Substring(9,3));
        foreach(VRStageLighting_Animated stagelight in stageLightsList)
        {
            // stagelight._colorAnimation = colorAnim;
            // stagelight._intensityAnimation = intensityAnim; 
            // stagelight._goboAnimation = gOBOAnim;
            // stagelight._panTiltSingleMeasureAnim = panTiltAnim;
            // stagelight._panTiltDualMeasureAnim = panTiltAnim;
            // stagelight._panTiltQuadMeasureAnim = panTiltAnim;
            // stagelight.followDefaultTarget = followDefaultTargetSync;
            stagelight._UpdateAnimations();
        }
    }
 
    void UpdateStageLightAnims(VRSL_CardObject card)
    {
        switch (card.CardType)
        {
            case 1:
                foreach(VRStageLighting_Animated stagelight in stageLightsList)
                {
                    stagelight._colorAnimation = colorAnim; 
                  //  stagelight._UpdateColorAnims();
                }
                //lastColorCard = card;
                break;

            case 2:
                foreach(VRStageLighting_Animated stagelight in stageLightsList)
                {
                    stagelight._intensityAnimation = intensityAnim;
                   // stagelight._UpdateIntensityAnims();
                    
                }
               // lastIntensityCard = card;
                break;

            case 3:
                foreach(VRStageLighting_Animated stagelight in stageLightsList)
                {
                    stagelight._goboAnimation = gOBOAnim;
                   // stagelight._UpdateGoboAnims();
                }
                //lastGoboCard = card;
                break;

            case 4:
               // lastPanTiltCard = card;
                switch (card.measureCount)
                {
                    case 1:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            //stagelight.followDefaultTarget = true;
                            stagelight._panTiltSingleMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                           // stagelight._UpdateIntensityAnims();                            
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = true;
                        // }
                        break;

                    case 2:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            //stagelight.followDefaultTarget = true;
                            stagelight._panTiltDualMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            //stagelight._UpdateIntensityAnims();                     
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = true;
                        // }
                        break;

                    case 4:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            //stagelight.followDefaultTarget = true;
                            stagelight._panTiltQuadMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                           // stagelight._UpdateIntensityAnims();  
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = true;
                        // }
                        break;

                    case 8:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            stagelight._panTiltQuadMeasureAnim = card.CardID;
                          //  stagelight._UpdateIntensityAnims();
                            //stagelight.followDefaultTarget = false;
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = false;
                        // }
                        break;

                    default:
                        Debug.Log("Measure Count unrecogniable, aboting...");
                        break;
                        
                }
                break;

            default:
                Debug.Log("Could not identify animation type, aborting...");
                break;

        }
        if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
        {
            animSync = colorAnim.ToString("D3") + intensityAnim.ToString("D3") + gOBOAnim.ToString("D3") + panTiltAnim.ToString("D3");
        }
        
    }
}
