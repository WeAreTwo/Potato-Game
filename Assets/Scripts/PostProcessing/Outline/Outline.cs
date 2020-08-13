using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PotatoGame
{
    public class Outline : VolumeComponent, IPostProcessComponent
    {
        //PIXELATION
        //public TextureParameter ditherTexture;
        public FloatParameter delta = new FloatParameter(10);

        //INTERFACE REQUIREMENT 
        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}