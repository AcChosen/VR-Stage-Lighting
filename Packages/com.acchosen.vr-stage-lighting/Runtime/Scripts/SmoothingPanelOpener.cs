using UnityEngine;
#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]

public class SmoothingPanelOpener : UdonSharpBehaviour
#else
public class SmoothingPanelOpener : MonoBehaviour
#endif
{
    public Animator animator;
    public bool isOpen;

    void Start() 
    {
        isOpen = false;
        ClosePanel();   
    }

    void OpenPanel()
    {
        animator.SetBool("isOpen", true);
    }

    void ClosePanel()
    {
        animator.SetBool("isOpen", false);
    }

#if UDONSHARP
    public override void Interact()
#else
    public void Interact()
#endif
    {
        isOpen = !isOpen;
        if(isOpen)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }
}
