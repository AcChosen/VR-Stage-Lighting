using UnityEngine;

#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;
#endif

#if UDONSHARP
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
#endif
public class VRSL_DMXGlobalExport
#if UDONSHARP
    : UdonSharpBehaviour
#else
    : MonoBehaviour
#endif
{
    public CustomRenderTexture dmxExportTexture;
    void Start()
    {
            gameObject.SetActive(true); // client disables extra cameras, so set it true
            transform.position = new Vector3(1000f, 10000000f, 0f); // keep this in a far away place
            dmxExportTexture.updateMode = CustomRenderTextureUpdateMode.Realtime;
    }
}
