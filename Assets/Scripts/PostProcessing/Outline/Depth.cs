using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [ExecuteAlways]
    public class Depth : MonoBehaviour
    {
        [SerializeField] protected Camera cam;

        private void Start()
        {
            // cam = this.GetComponent<Camera>();
            // cam.depthTextureMode = DepthTextureMode.Depth;
        }
    }

}
