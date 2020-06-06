using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    
    [System.Serializable]
    public class ColorProfile
    {
        public string name = "Primary";
        public Color color = Color.cyan;
        public Material mat;

        public void OnValidate()
        {
            if (mat)
            {
                ChangeName();
                ChangeColor();
            }
        }

        protected void ChangeName()
        {
            this.name = mat.name;
        }

        protected void ChangeColor()
        {
            mat.SetColor("_BaseColor", this.color);
        }
    }
    
    [ExecuteAlways]
    public class ShaderController : MonoBehaviour
    {
        //GLOBAL SHADER VARIABLES
        [Header("GLOBAL PROPERTIES")]
        [SerializeField] protected Texture bluenoise;

        [Header("COLOR PALLETTE")]
        [SerializeField] protected List<ColorProfile> colorProfiles = new List<ColorProfile>();
        
        void Start()    
        {
            Shader.SetGlobalTexture("_BlueNoiseMap", bluenoise);
            Shader.SetGlobalTexture("_DetailMap", bluenoise);
        }

        //when something changes in the inspector call this 
        void OnValidate()
        {
            foreach (var profile in colorProfiles)
            {
                profile.OnValidate();
            }
        }
    }
}