
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class Slicer : InteractableStationary
    {
        [Header("SLICER PARAMETERS")] 
        [SerializeField] protected Transform feeder;
        [SerializeField] protected Transform output;
        [SerializeField] protected float ejectionForce = 3.0f;
        
        [SerializeField] protected bool isProcessing;
        [SerializeField] protected float processingTime = 3.0f;
        [SerializeField] protected float processingTimer = 0;

        public override void Interact(params object[] args)
        {
            // int yield = plantObj.GrowthParams.harvestYield;
            if (args[0] == typeof(Plant))
            {
            }

        }

        public void SlicePotato(Potato potato)
        {
            int yield = potato.GrowthParams.harvestYield;
        }

        Potato outputPotato()
        {
            Potato seedPotato = new Potato();
            // set init state to seed
    
            return seedPotato;
        }

        protected void EjectPotato()
        {
            // instantiate
            // eject with force
        }
    }

}
