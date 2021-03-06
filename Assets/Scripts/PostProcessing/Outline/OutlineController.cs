﻿using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PotatoGame
{
    [ExecuteInEditMode]
    public class OutlineController : MonoBehaviour
    {
        [SerializeField] protected VolumeProfile volumeProfile;
        [SerializeField] protected bool isEnabled = true;

        protected Outline outline;
        [SerializeField] protected float delta = 5;
        
        protected void Update()
        {
            this.SetParams();
        }

        protected void SetParams()
        {
            if (!this.isEnabled) return; 
            if (this.volumeProfile == null) return;
            if (this.outline == null) volumeProfile.TryGet<Outline>(out this.outline);
            if (this.outline == null) return;
            
            
            //ACCESSING PARAMS 
            this.outline.delta.value = this.delta;
            
        }
    }
}