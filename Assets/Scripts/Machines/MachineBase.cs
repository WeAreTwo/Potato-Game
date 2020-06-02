using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class MachineBase : InteractableStationary
    {

        [Header("MACHINE BASIC PARAMS")]
        [SerializeField] protected bool isProcessing;
        [SerializeField] protected float processingTime = 3.0f;
        [SerializeField] protected float processingTimer = 0;

        public bool IsProcessing { get => isProcessing; set => isProcessing = value; }

        public override void Interact()
        {

        }

    }
    
}