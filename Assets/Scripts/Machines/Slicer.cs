
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PotatoGame
{

    public class Slicer : MachineBase
    {
        [Header("SLICER PARAMETERS")] 
        [SerializeField] protected Transform feeder;
        [SerializeField] protected Transform output;
        [SerializeField] protected float ejectionForce = 3.0f;

        [Header("OUTPUT OBJECT")] 
        [SerializeField] protected GameObject outputPrefab;
        
        protected void Update()
        {
            if (isProcessing && processingTimer < processingTime)
            {
                processingTimer += Time.deltaTime;
            }

            if (isProcessing && processingTimer >= processingTime)
            {
                EjectPotato();
                isProcessing = false;
            }
            
            //TEST 
            if(Input.GetKeyDown(KeyCode.K))
                EjectPotato();
        }

        public override void Interact()
        {
            isProcessing = true;
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
            // find the direction of ejection 
            var direction = output.forward;
            // instantiate
            GameObject outputObj = Instantiate(outputPrefab, output.position, Random.rotation);
            // eject with force
            outputObj.ThrowObject(direction, ejectionForce);
        }

        protected void OnDrawGizmos()
        {
            if (feeder)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(feeder.position, 0.25f);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(feeder.position, feeder.position + feeder.forward * 1.5f);
            }

            if (output)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(output.position, 0.25f);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(output.position, output.position + output.forward * 1.5f);
            }
        }
    }

}
