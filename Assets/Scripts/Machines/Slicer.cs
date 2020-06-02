
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PotatoGame
{

    [System.Serializable]
    public class SlicerProcessingTask
    {
        protected Slicer component;
        [SerializeField] protected bool isProcessing;
        [SerializeField] protected float processingTime = 3.0f;
        [SerializeField] protected float processingTimer = 0;
        
        public bool IsProcessing { get => isProcessing; set => isProcessing = value; }

        public SlicerProcessingTask(Slicer component)
        {
            this.component = component;
            isProcessing = true;
        }

        public void Update()
        {
            if (isProcessing && processingTimer < processingTime)
            {
                processingTimer += Time.deltaTime;
            }

            if (isProcessing && processingTimer >= processingTime)
            {
                isProcessing = false;
                component.EjectPotato();
            }
        }
    }

    public class Slicer : MachineBase
    {
        [Header("SLICER PARAMETERS")] 
        [SerializeField] protected Transform feeder;
        [SerializeField] protected Transform output;
        [SerializeField] protected float ejectionForce = 3.0f;

        [Header("OUTPUT OBJECT")] 
        [SerializeField] protected GameObject outputPrefab;
        
        //Event
        public delegate void ProcessFinishedAction();
        public static event ProcessFinishedAction OnProcessFinished;

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

        //SUBSCRIBE OUR EVENT TO EJECT
        protected void OnEnable()
        {
            OnProcessFinished += EjectPotato;
        }

        protected void OnDisable()
        {
            OnProcessFinished -= EjectPotato;
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

        public void EjectPotato()
        {
            var direction = output.forward; // find the direction of ejection 
            GameObject outputObj = Instantiate(outputPrefab, output.position, Random.rotation); // instantiate
            outputObj.ThrowObject(direction, ejectionForce); // eject with force
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
