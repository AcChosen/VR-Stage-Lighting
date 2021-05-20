
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_NudgeableObject : UdonSharpBehaviour
{
    [UdonSynced]
     public float syncedHeightMod = 0.0f;
    private float heightMod = 0.0f;
    Vector3 startPos;

    void Start() {
        syncedHeightMod = 0.0f;
        heightMod = 0.0f;
        startPos = transform.position;

    }

    public override void OnDeserialization()
    {
        if(heightMod != syncedHeightMod)
        {
            _UpdateHeight();
        }
    }

    public void _UpdateHeight()
    {
        Vector3 newPos = new Vector3(startPos.x, startPos.y + syncedHeightMod, startPos.z);
        transform.position = newPos;
        heightMod = syncedHeightMod;
    }
}
