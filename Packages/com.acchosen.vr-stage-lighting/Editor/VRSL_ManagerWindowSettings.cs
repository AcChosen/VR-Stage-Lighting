using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSL.EditorScripts
{
    public class VRSL_ManagerWindowSettings : ScriptableObject
    {

        [Header("DMX Volumetric Meshes")]
        public Mesh highPolySpotlightVolumetric;
        public Mesh mediumPolySpotlightVolumetric;
        public Mesh lowPolySpotlightVolumetric;
        public Mesh highPolyWashlightVolumetric;
        public Mesh mediumPolyWashlightVolumetric;
        public Mesh lowPolyWashlightVolumetric;

        [Header("AudioLink Volumetric Meshes")]
        public Mesh highPolySpotlightVolumetricAudioLink;
        public Mesh mediumPolySpotlightVolumetricAudioLink;
        public Mesh lowPolySpotlightVolumetricAudioLink;
        public Mesh highPolyWashlightVolumetricAudioLink;
        public Mesh mediumPolyWashlightVolumetricAudioLink;
        public Mesh lowPolyWashlightVolumetricAudioLink;

        public Mesh GetSpotLightMesh(int type)
        {
            switch(type)
            {
                default:
                    return highPolySpotlightVolumetric;
                case 1:
                    return mediumPolySpotlightVolumetric;
                case 2:
                    return lowPolySpotlightVolumetric;
            }
        }
        public Mesh GetWashLightMesh(int type)
        {
            switch(type)
            {
                default:
                    return highPolyWashlightVolumetric;
                case 1:
                    return mediumPolyWashlightVolumetric;
                case 2:
                    return lowPolyWashlightVolumetric;
            }
        }


        public Mesh GetAudioLinkSpotLightMesh(int type)
        {
            switch(type)
            {
                default:
                    return highPolySpotlightVolumetricAudioLink;
                case 1:
                    return mediumPolySpotlightVolumetricAudioLink;
                case 2:
                    return lowPolySpotlightVolumetricAudioLink;
            }
        }
        public Mesh GetAudioLinkWashLightMesh(int type)
        {
            switch(type)
            {
                default:
                    return highPolyWashlightVolumetricAudioLink;
                case 1:
                    return mediumPolyWashlightVolumetricAudioLink;
                case 2:
                    return lowPolyWashlightVolumetricAudioLink;
            }
        }
    }
}