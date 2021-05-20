
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_CardSet : UdonSharpBehaviour
{
    public VRSL_CardObject[] cards;

    public void ResetAllCards()
    {
        foreach(VRSL_CardObject card in cards)
        {
            card.ReturnToStart();
        }
    }
}
