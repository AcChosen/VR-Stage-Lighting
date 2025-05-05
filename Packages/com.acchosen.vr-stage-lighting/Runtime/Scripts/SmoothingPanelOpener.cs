using UnityEngine;

#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
#endif

public class SmoothingPanelOpener
#if UDONSHARP
    : UdonSharpBehaviour
#else
    : MonoBehaviour
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

    public
#if UDONSHARP
        override
#endif
        void Interact()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }
}