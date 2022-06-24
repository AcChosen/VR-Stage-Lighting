
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UdonSharpEditor;
#endif

public class TargetFollower : UdonSharpBehaviour
{
    [Tooltip ("Enable this to follow whoever owns this object. Change the owner of this object to change who this object follows.")]
    public bool followOwner;

    void Update() 
    {
        if(Networking.GetOwner(this.gameObject) != null && followOwner)
        {
            this.transform.position = Networking.GetOwner(this.gameObject).GetPosition();
        }
    }
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnDrawGizmos()
        {
            this.UpdateProxy(ProxySerializationPolicy.RootOnly);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
    #endif
}


