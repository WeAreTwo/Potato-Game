using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class ShaderController : MonoBehaviour
    {
        //GLOBAL SHADER VARIABLES
        [SerializeField] protected Texture bluenoise;

        void Start()
        {
            Shader.SetGlobalTexture("_BlueNoiseMap", bluenoise);
            Shader.SetGlobalTexture("_DetailMap", bluenoise);
        }

        void Update()
        {
        }
    }
}