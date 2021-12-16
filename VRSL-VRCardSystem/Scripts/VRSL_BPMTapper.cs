
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_BPMTapper : UdonSharpBehaviour
{
    public MeshRenderer buttonRenderer;
    public bool resetEveryMeasure = true;
    public VRSL_BPMCounter _bpmCounter;
    //public TappingEnabler tapperEnabler;
    private float[] tapTime = new float[4];
    private int tap;

    private UnityEngine.UI.Text thetext ;

    // Variable for cumulative moving average
    // https://en.wikipedia.org/wiki/Moving_average

    private float lastTapTime; // Used to make sure 
    private float prevAvg;

    [SerializeField]
    private float displayBPM;

    private void Start()
    {
        //thetext = this.GetComponentInChildren<Text>();
        //thetext.text = _bpmc.GetBPM().ToString();
        displayBPM = _bpmCounter.BPM;
    }

    public override void Interact()
    {
        if(!Networking.IsOwner(Networking.LocalPlayer, this.gameObject) || !Networking.IsOwner(Networking.LocalPlayer, _bpmCounter.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, _bpmCounter.gameObject);
            Debug.Log("Setting Onwer!");
        }
        Debug.Log("I now own  everything!");
        if (_bpmCounter._customBeatEnabled)
        {
            Debug.Log("Attempting to Tap!");
            if(tap < 3)
            {

                Debug.Log("Tap!: " + tap);
                tapTime[tap] = Time.realtimeSinceStartup;
                tap++;
                ChangeButtonColor();
            }
            else if (tap == 3)
            {
                Debug.Log("Tap!: " + tap);
                tapTime[tap] = Time.realtimeSinceStartup;
                //tap++;
                float averageTime = ((tapTime[1] - tapTime[0]) + (tapTime[2] - tapTime[1]) + (tapTime[3] - tapTime[2])) / 3;

                _bpmCounter.SetBPM(Mathf.Clamp(Mathf.Round((60 / averageTime)*100.0f)/100.0f, 20.0f, 200.0f));
                //UpdateText();
                _bpmCounter.ResetTimerAndBeats();
                //_bpmc._customBeatEnabled = false;
                //tapperEnabler.ResetEnabler();

                lastTapTime = tapTime[3];
                prevAvg = averageTime;
                if(resetEveryMeasure)
                {
                    Reset();
                }
                ChangeButtonColor();

            }
            else
            {
                float tapTime = Time.realtimeSinceStartup;
                float averageTime = (((tapTime - lastTapTime) + ((tap - 1) * prevAvg))) / (tap);
                _bpmCounter.SetBPM(((Mathf.Round((60 / averageTime)*100.0f)/100.0f) + _bpmCounter.BPM)/2);
                tap++;
                lastTapTime = tapTime;
                prevAvg = averageTime;
                if(resetEveryMeasure)
                {
                    Reset();
                }
                //UpdateText();
            }
        }
        displayBPM = _bpmCounter.BPM;
        
    }
    // public void UpdateText()
    // {
    //     thetext.text = _bpmc.GetBPM().ToString("0.00");

    // }

    public void Reset()
    {
        tap = 0;
    }

    void ChangeButtonColor()
    {
        if(tap == 0 || tap == 2)
        {
            buttonRenderer.material.color = Color.green;
        }
        if(tap == 1 || tap == 3)
        {
            buttonRenderer.material.color = Color.red;
        }
    }
}
