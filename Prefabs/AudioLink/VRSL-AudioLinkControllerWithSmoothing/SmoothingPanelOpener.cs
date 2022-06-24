
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]

public class SmoothingPanelOpener : UdonSharpBehaviour
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

    public override void Interact()
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
