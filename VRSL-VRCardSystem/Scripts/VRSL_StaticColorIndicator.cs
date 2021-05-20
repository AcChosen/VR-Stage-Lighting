
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_StaticColorIndicator : UdonSharpBehaviour
{
    Material material;
    public VRSL_CardObject card;
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;


    } 
    void Update() 
    {
        //material
        material.mainTextureOffset = new Vector2(card.rotationValue, 0.0f);
    }
}
