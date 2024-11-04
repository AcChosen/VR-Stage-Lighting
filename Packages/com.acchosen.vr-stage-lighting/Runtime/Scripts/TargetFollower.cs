
using UnityEngine;
#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UdonSharpEditor;
#endif

#endif

namespace VRSL
{
#if UDONSHARP
    public class TargetFollower : UdonSharpBehaviour
#else
    public class TargetFollower : MonoBehaviour
#endif
    {
        [Tooltip ("Enable this to follow whoever owns this object. Change the owner of this object to change who this object follows.")]
        public bool followOwner;

#if UDONSHARP
        void Update() 
        {
            if(Networking.GetOwner(this.gameObject) != null && followOwner)
            {
                this.transform.position = Networking.GetOwner(this.gameObject).GetPosition();
            }
        }
#endif

        #if !COMPILER_UDONSHARP && UNITY_EDITOR
            private void OnDrawGizmos()
            {
#if UDONSHARP
                #pragma warning disable 0618 //suppressing obsoletion warnings
                this.UpdateProxy(ProxySerializationPolicy.RootOnly);
                #pragma warning restore 0618 //suppressing obsoletion warnings
#endif
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, 0.25f);
            }
        #endif
    }
}


