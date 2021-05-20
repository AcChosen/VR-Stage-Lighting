
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_HeightNudger : UdonSharpBehaviour
{
    public VRSL_NudgeableObject objToNudge;
    public bool isGoinUp;
    public float amountToNudge;
    public float heightCap;

    public override void Interact()
    {
        //IncrementHeight();
        this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IncrementHeight");
    }

    public void IncrementHeight()
    {
        if(isGoinUp)
        {
            if(objToNudge.syncedHeightMod <= heightCap)
            {
                objToNudge.syncedHeightMod += amountToNudge;
                objToNudge._UpdateHeight();

            }
        }
        else
        {
            if(objToNudge.syncedHeightMod >= heightCap)
            {
                objToNudge.syncedHeightMod += amountToNudge;
                objToNudge._UpdateHeight();
            }
        }
    }
}
