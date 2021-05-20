
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_BPMCounter : UdonSharpBehaviour
{
    public UnityEngine.UI.Text text;
    public UnityEngine.UI.Text beatText;
    [HideInInspector]
    public bool resetCounter = false;
    public bool manualStartBeat;
    public bool _customBeatEnabled;
    private float _beatInterval, _beatTimer, _beatIntervalDivide8, _beatTimerDivide8;
    public bool _beatFull, _beatDivide8;
    public int _beatCountFull, _beatCountDivide8;
    private bool captureNextDownbeat;
    public int _quarterNoteCount; // counts 1 , 2 , 3 , 4
    public int _measureCount = 1; //Counts every 4 measures
    private double lastDownbeatTime;
    private double lastDownbeatTimePrev;   //Used by clients to detect new sync info

    public float newAnimationSpeed;

    // [UdonSynced]
    // private double lastDownbeatTimeSynced;

    [UdonSynced]
    public float _bpm;
    
    private float unsyncbpm;
    void Start()
    {
        setAnimSpeed();
        unsyncbpm = _bpm;  
    }

    public override void OnDeserialization()
    {
        if(_bpm != unsyncbpm)
        {
            captureNextDownbeat = true;
            _quarterNoteCount = 4;
            setAnimSpeed();
            unsyncbpm = _bpm;
        }
    }


    void Update() 
    {
        resetCounter = false;
        BeatDetection();
    }

    public void ResetTimerAndBeats()
    {
        _beatInterval = 0;
        _beatTimer = 0;
        _beatTimerDivide8 = 0;
        _beatCountFull = 0;
        _beatCountDivide8 = 0;
        //_measureCount = 1;
    }
    public void ResetNoteCounters()
    {
        resetCounter = true;
        _quarterNoteCount = 1;
        //if (Networking.IsMaster)
        //{
            // foreach (UdonSharpBehaviour ub in udonbeatTargets)
            // {
            //     ub.SendCustomEvent("CheckBeat");
            // }
        //}
    }

        void BeatDetection()
    {
        text.text = "BPM: " + _bpm.ToString();
        beatText.text = "Beat: " + _quarterNoteCount.ToString();


        //assume there is no beat this frame
        _beatFull = false;
        //every second there will be a beat interval if bpm 60. a beat every half second at 120, etc. This sets what consititutes what bpm is a "second". Default is 60.
        _beatInterval = 60 / _bpm;
        //increment timer every frame by however many seconds have passed.
        //_beatTimer += Time.fixedDeltaTime;
        //_beatTimer += Time.fixedUnscaledDeltaTime;
        _beatTimer += Time.deltaTime;

        while (_beatTimer >= _beatInterval || manualStartBeat) // if it is time for a beat...
        {
            if(manualStartBeat)
            {
                
                _quarterNoteCount = 4;
                manualStartBeat = false;
                _beatInterval = 60 / _bpm;
                _beatTimer += Time.deltaTime;
                //setAnimSpeed();
            }

            _beatTimer -= _beatInterval; // subtract from the beat timer that interval that just passed
            _beatFull = true; //we are now on a beat
            _beatCountFull++; //Increment the number of beats that have passed.

            if (_quarterNoteCount != 4)//Counting quarter notes!
            {
                _quarterNoteCount++;
            }
            else
            {
                setAnimSpeed();
                DownBeat();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setAnimSpeed");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DownBeat");

                

 


                
            }

            // If this is a downbeat, save the server time when this occured (only the master uses this value)
            if (captureNextDownbeat && _quarterNoteCount == 1)
            {
                captureNextDownbeat = false;
                // lastDownbeatTimeSynced = Networking.GetServerTimeInSeconds();
            }

            // foreach (UdonSharpBehaviour ub in udonbeatTargets)
            // {
            //     ub.SendCustomEvent("CheckBeat");
            // }
            //Debug.Log("Beat: " + _quarterNoteCount);

        }


        //do the same but divide everything by 8 to get 1/8ths and 16ths.
        _beatDivide8 = false;
        _beatIntervalDivide8 = _beatInterval / 8;
        _beatTimerDivide8 += Time.deltaTime;
        if(_beatTimerDivide8 >= _beatIntervalDivide8)
        {
            _beatTimerDivide8 -= _beatIntervalDivide8;
            _beatDivide8 = true;
            if(_beatCountDivide8 != 8)
            {
                _beatCountDivide8++;
            }
            else
            {
                _beatCountDivide8 = 1;
            }

        }

    }

        public void ToggleCustomBeatEnabled()
    {
        _customBeatEnabled = !_customBeatEnabled;
    }

    public void SetBPM(float value)
    {
        // if (Networking.IsMaster)
        // {
            _bpm = value;
            captureNextDownbeat = true;
            _quarterNoteCount = 4;
            setAnimSpeed();
            unsyncbpm = _bpm;
            
        //}
        //bpmtapperer.UpdateText();
    }

    void setAnimSpeed()
    {
        newAnimationSpeed = _bpm/ 60;
    }

    void DownBeat()
    {
        _quarterNoteCount = 1;
        if(_measureCount != 4)
        {
            _measureCount++;
        }
        else
        {
            _measureCount = 1;

        }
    }
}
