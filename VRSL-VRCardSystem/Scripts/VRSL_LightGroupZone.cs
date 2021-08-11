
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
    public VRStageLighting_Animated_Static[] staticStageLightsList;
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
                foreach(VRStageLighting_Animated_Static light in staticStageLightsList)
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
        foreach(VRStageLighting_Animated_Static light in staticStageLightsList)
        {
            light.coneWidth = Mathf.Lerp(0.0f, 5.5f, coneWidthSlider.value);
            //light._UpdateProps();
        }

        foreach(VRStageLighting_Animated light in stageLightsList)
        {
            light.finalIntensity = Mathf.Lerp(0.0f, 1.0f, Mathf.Clamp(Mathf.Pow(finalIntensitySlider.value, intensityCurve),0.0f, Mathf.Pow(masterIntensitySlider.value, intensityCurve)));
            //light._UpdateProps();
        }
                foreach(VRStageLighting_Animated_Static light in staticStageLightsList)
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
                        if(colorCard == card)
                        {
                            return;
                        }
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
                        if(intensityCard == card)
                        {
                            return;
                        }
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
                        if(goboCard == card)
                        {
                            return;
                        }
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
                        if(panTiltCard == card)
                        {
                            return;
                        }
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

    void OnTriggerStay(Collider other)
    {
       VRSL_CardObject card = (VRSL_CardObject) other.gameObject.GetComponent<VRSL_CardObject>();
       if(card != null)
       {
            switch(card.CardType)
            {
                case 1:
                        if(colorCard == card)
                        {
                            return;
                        }
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
                        if(intensityCard == card)
                        {
                            return;
                        }
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
                        if(goboCard == card)
                        {
                            return;
                        }
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
                        if(panTiltCard == card)
                        {
                            return;
                        }
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
            stagelight._UpdateAnimations();
        }
        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
        {
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
                    stagelight._UpdateColorAnims();
                }
                foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                {
                    stagelight._colorAnimation = colorAnim;
                    stagelight._UpdateColorAnims();
                }
                string colorString = colorAnim.ToString("D3");
                animSync = colorString + animSync.Substring(3, 9);
                //lastColorCard = card;
                break;

            case 2:
                foreach(VRStageLighting_Animated stagelight in stageLightsList)
                {
                    stagelight._intensityAnimation = intensityAnim;
                    stagelight._UpdateIntensityAnims();
                }
                foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                {
                    stagelight._intensityAnimation = intensityAnim;
                    stagelight._UpdateIntensityAnims();
                }
                string intensitystring = intensityAnim.ToString("D3");
                animSync = animSync.Substring(0,3) + intensitystring + animSync.Substring(6,6);
               // lastIntensityCard = card;
                break;

            case 3:
                foreach(VRStageLighting_Animated stagelight in stageLightsList)
                {
                    stagelight._goboAnimation = gOBOAnim;
                    stagelight._UpdateGoboAnims();
                }
                foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                {
                    stagelight._goboAnimation = gOBOAnim;
                    stagelight._UpdateGoboAnims();
                }
                //lastGoboCard = card;
                string gobostring = gOBOAnim.ToString("D3");
                animSync = animSync.Substring(0,6) + gobostring + animSync.Substring(9,3);
                break;

            case 4:
               // lastPanTiltCard = card;
                switch (card.measureCount)
                {
                    case 1:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            stagelight._panTiltSingleMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            stagelight._UpdatePanTiltAnims();                           
                        }
                        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                        {
                            stagelight._panTiltSingleMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            stagelight._UpdatePanTiltAnims();                           
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = true;
                        // }
                        break;

                    case 2:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            stagelight._panTiltDualMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            stagelight._UpdatePanTiltAnims();                    
                        }
                        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                        {
                            stagelight._panTiltDualMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            stagelight._UpdatePanTiltAnims();                    
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = true;
                        // }
                        break;

                    case 4:
                        foreach(VRStageLighting_Animated stagelight in stageLightsList)
                        {
                            stagelight._panTiltQuadMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            stagelight._UpdatePanTiltAnims(); 
                        }
                        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                        {
                            stagelight._panTiltQuadMeasureAnim = card.CardID;
                            stagelight._panTiltMeasureCount = card.measureCount;
                            stagelight._UpdatePanTiltAnims(); 
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
                            stagelight._UpdatePanTiltAnims();
                        }
                        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
                        {
                            stagelight._panTiltQuadMeasureAnim = card.CardID;
                            stagelight._UpdatePanTiltAnims();
                        }
                        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
                        // {
                            followDefaultTargetSync = false;
                        // }
                        break;

                    default:
                        Debug.Log("Measure Count unrecogniable, aborting...");
                        break;
                        
                        
                }
                string pantiltstring = panTiltAnim.ToString("D3");
                animSync = animSync.Substring(0,9) + pantiltstring;
                break;

            default:
                Debug.Log("Could not identify animation type, aborting...");
                break;

        }

 
        // if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
        // {

           // string currentAnims = colorAnim.ToString("D3") + intensityAnim.ToString("D3") + gOBOAnim.ToString("D3") + panTiltAnim.ToString("D3");
            //animSync = colorAnim.ToString("D3") + intensityAnim.ToString("D3") + gOBOAnim.ToString("D3") + panTiltAnim.ToString("D3");
        // }
        
    }
    public void _CallDownBeat()
    {
        foreach(VRStageLighting_Animated stagelight in stageLightsList)
        {
            stagelight._DownBeat();
        }
        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
        {
            stagelight._DownBeat();
        }
    }
    public void _ResetBPM()
    {
        foreach(VRStageLighting_Animated stagelight in stageLightsList)
        {
            stagelight.CheckBPM();
        }
        foreach(VRStageLighting_Animated_Static stagelight in staticStageLightsList)
        {
            stagelight.CheckBPM();
        }
    }
}
