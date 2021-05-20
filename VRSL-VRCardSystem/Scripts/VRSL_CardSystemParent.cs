﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRSL_CardSystemParent : UdonSharpBehaviour
{
    public VRStageLighting_RAW[] lights;
    public GameObject parent;
    public bool status;

    void Start() {
        status = true;
    }

    public void _ToggleAll()
    {
        status = !status;
        if(status)
        {
            parent.SetActive(true);
            foreach(VRStageLighting_RAW light in lights)
            {
                
                // MeshRenderer[] renderers = light.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer renderer in light.objRenderers)
                {
                    renderer.enabled = true;
                }
                //light.gameObject.SetActive(true);
                light.enabled = true;
                
            }
            
        }
        else
        {
            foreach(VRStageLighting_RAW light in lights)
            {
                // MeshRenderer[] renderers = light.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer renderer in light.objRenderers)
                {
                    renderer.enabled = false;
                }
                light.enabled = false;
                //light.gameObject.SetActive(false);
                // light.GetComponent<MeshRenderer>().enabled = false;
            }
            parent.SetActive(false);
        }

    }

        public void _SetStatus(bool input)
    {
        status = input;
        if(status)
        {
            parent.SetActive(true);
            foreach(VRStageLighting_RAW light in lights)
            {
                
                // MeshRenderer[] renderers = light.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer renderer in light.objRenderers)
                {
                    renderer.enabled = true;
                }
                //light.gameObject.SetActive(true);
                light.enabled = true;
                
            }
            
        }
        else
        {
            foreach(VRStageLighting_RAW light in lights)
            {
                // MeshRenderer[] renderers = light.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer renderer in light.objRenderers)
                {
                    renderer.enabled = false;
                }
                light.enabled = false;
                //light.gameObject.SetActive(false);
                // light.GetComponent<MeshRenderer>().enabled = false;
            }
            parent.SetActive(false);
        }

    }
}