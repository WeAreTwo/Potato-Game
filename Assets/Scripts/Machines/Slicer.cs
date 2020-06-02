
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class Slicer : MachineBase
    {
        [Header("SLICER PARAMETERS")] 
        [SerializeField] protected Transform feeder;
        [SerializeField] protected Transform output;
        [SerializeField] protected float ejectionForce = 3.0f;
        
        public override void Interact()
        {

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
