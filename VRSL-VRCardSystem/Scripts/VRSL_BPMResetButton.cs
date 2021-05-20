
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_BPMResetButton : UdonSharpBehaviour
{
    public VRSL_BPMCounter bPMCounter;
    public VRSL_BPMTapper bPMTapper;

    public override void Interact()
    {
        if(!Networking.IsOwner(Networking.LocalPlayer, bPMCounter.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, bPMCounter.gameObject);
        }
        // bPMCounter.ResetTimerAndBeats();
        // bPMCounter.ResetNoteCounters();
        // bPMTapper.Reset();
        
        bPMCounter.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTimerAndBeats");
        bPMCounter.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetNoteCounters");
        bPMTapper.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Reset");



    }
}
