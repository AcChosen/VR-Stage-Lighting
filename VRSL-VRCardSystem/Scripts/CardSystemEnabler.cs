
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CardSystemEnabler : UdonSharpBehaviour
{

    public VRSL_BPMCounter bPMCounter;
    [UdonSynced]
    public bool enableBPM;
    bool enableBPMLocal;
    Material buttonMat;
    void Start()
    {
        buttonMat = GetComponent<MeshRenderer>().material;
        ChangeMaterial(enableBPM);
    }
    public override void Interact()
    {
        if(!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        enableBPM = !enableBPM;
        bPMCounter.isCounting = enableBPM;
        enableBPMLocal = enableBPM;
        ChangeMaterial(enableBPM);
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        _QueueSerialize();
    }

    public void _QueueSerialize()
    {
        if(Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            return;
        }
        if(enableBPM != enableBPMLocal)
        {
            bPMCounter.isCounting = enableBPM;
            ChangeMaterial(enableBPM);
            enableBPMLocal = enableBPM;
        }
    }

    void ChangeMaterial(bool input)
    {
        if(input)
        {
            buttonMat.SetColor("_EmissionColor", Color.green);
        }
        else
        {
            buttonMat.SetColor("_EmissionColor", Color.red);
        }
    }
}
