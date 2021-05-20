
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_CardObject : UdonSharpBehaviour
{
    public int CardType;
    public int CardID;

    public GameObject CardMesh;
    public Transform lookAtPos;

    public bool meshLookAtToggle;

    Collider col;
    public int measureCount = 1;

    public bool rotationMod;

    [Range(0.0f,1.0f)]
    public float rotationValue;

    public float maxRotation;

    Vector3 startpos;

     void Start() {
        startpos = this.transform.localPosition;
        col = this.GetComponent<Collider>();
        meshLookAtToggle = false;
        
    }

    public void ReturnToStart()
    {
        if(this.transform.position != startpos)
        {
            if(Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                RTSNet();
            }
            else
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                RTSNet();
            }
        }
    }

    public void RTSNet()
    {
        if(!meshLookAtToggle)
        {
            col.enabled = false;    
            this.transform.localPosition = startpos;
            col.enabled = true;
        }
    }
    void Update() 
    {
        if(rotationMod)
        {
            GetRotationValue();
            //this.transform.rotation.eulerAngles.Set(this.transform.rotation.eulerAngles.x, Mathf.Clamp(this.transform.rotation.eulerAngles.y,0.0f, maxRotation), this.transform.rotation.eulerAngles.z );
        }
        if(meshLookAtToggle)
        {
            CardMeshLookAtPlayer();
        }
        
    }

    public override void OnPickup()
    {
        // if(!Networking.IsOwner(Networking.LocalPlayer,gameObject))
        // {
        //     Networking.SetOwner(Networking.LocalPlayer, gameObject);
        // }
         GrabOn();
        // SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GrabOn");
        
    }

    public override void OnDrop()
    {
         GrabOff();
        // SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GrabOff");
    }

    void GetRotationValue()
    {
        float angleY = this.transform.localRotation.eulerAngles.y;
        if(angleY > maxRotation || angleY < 0.0f)
        {
            angleY = angleY % maxRotation;
        }
       rotationValue = Mathf.Clamp01((angleY)/maxRotation);
    }

    void CardMeshLookAtPlayer()
    {
        if(CardMesh != null)
        {
            if(lookAtPos != null)
            {
                rotateTowards(lookAtPos.position, 7.0f);
            }
            else if(Networking.GetOwner(this.gameObject) != null)
            {
                if(Networking.GetOwner(gameObject) != null)
                    rotateTowards(Networking.GetOwner(gameObject).GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, 9.0f);
            }
            else
            {
                return;
            }

        }
    }

    void rotateTowards(Vector3 to, float turnspeed) {
 
     Quaternion _lookRotation = Quaternion.LookRotation((to - CardMesh.transform.position).normalized);
     _lookRotation.eulerAngles.Set(_lookRotation.eulerAngles.x + 90.0f, _lookRotation.eulerAngles.y, _lookRotation.eulerAngles.z);
     
     //over time
     CardMesh.transform.rotation = Quaternion.Slerp(CardMesh.transform.rotation, _lookRotation, Time.deltaTime * turnspeed);
    }

    public void GrabOn()
    {
        meshLookAtToggle = true;
    }
    public void GrabOff()
    {
        meshLookAtToggle = false;
    }
}
