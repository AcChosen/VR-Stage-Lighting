
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_SyncedSlider : UdonSharpBehaviour
{
    public Transform minimum;
    public Transform maximum;
    
    public float value;
    public bool isGrabbed;
    float distance;

    public bool isMasterSlider;
    public VRSL_LightGroupZone[] masterListOfZones;

    public VRSL_LightGroupZone groupZone;
    void Start()
    {
        value = (transform.localPosition.x/Mathf.Abs(distance)) + 0.5f;
        distance = Vector3.Distance(minimum.localPosition, maximum.localPosition);
        isGrabbed = false;

    }

    void Update() 
    {
        if(isGrabbed)
        {
            if(Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                if(transform.localPosition.x > maximum.localPosition.x)
                {
                    transform.localPosition = new Vector3(maximum.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                }
                if(transform.localPosition.x < minimum.localPosition.x)
                {
                    transform.localPosition = new Vector3(minimum.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                }
                transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, 0.0f);
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            }
            value = (transform.localPosition.x/Mathf.Abs(distance)) + 0.5f;
            if(groupZone != null && !isMasterSlider)
            {
                groupZone.UpdateSliders();
                return;
            }
            if(isMasterSlider)
            {
                foreach(VRSL_LightGroupZone zone in masterListOfZones)
                {
                    zone.UpdateSliders();
                }
                return;
            }
        }
        else
        {
            value = (transform.localPosition.x/Mathf.Abs(distance)) + 0.5f;
        }

    }
    public void GrabStateOn()
    {
        isGrabbed = true;
    }

    public void GrabStateOff()
    {
        isGrabbed = false;
        if(transform.localPosition.x > maximum.localPosition.x)
        {
            transform.localPosition = new Vector3(maximum.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }
        if(transform.localPosition.x < minimum.localPosition.x)
        {
            transform.localPosition = new Vector3(minimum.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }
        transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, 0.0f);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }



    public override void OnPickup()
    {
        // if(!Networking.IsOwner(Networking.LocalPlayer,gameObject))
        // {
        //     Networking.SetOwner(Networking.LocalPlayer, gameObject);
        // }
        GrabStateOn();
        this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GrabStateOn");
        // SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GrabStateOn");
    }

    public override void OnDrop()
    {
       GrabStateOff();
       this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GrabStateOff");
    //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GrabStateOff");
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(Networking.LocalPlayer == player && !Networking.IsMaster)
        {
            if(groupZone != null && !isMasterSlider)
            {
                groupZone.UpdateSliders();
                return;
            }
            if(isMasterSlider)
            {
                foreach(VRSL_LightGroupZone zone in masterListOfZones)
                {
                    zone.UpdateSliders();
                }
                return;
            }
        }
    }

}
