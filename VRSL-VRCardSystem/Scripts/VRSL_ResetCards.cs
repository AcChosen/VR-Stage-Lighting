
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_ResetCards : UdonSharpBehaviour
{
    public VRSL_CardSet[] cardSets;

    public override void Interact()
    {
        foreach(VRSL_CardSet set in cardSets)
        {
            set.ResetAllCards();
        }
    }
}
