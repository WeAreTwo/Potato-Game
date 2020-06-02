
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (isProcessing && processingTimer <= processingTime)
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
        [SerializeField] protected GameObject feederPrefab;
        [SerializeField] protected GameObject outputPrefab;
        
        public Dictionary<SlicerProcessingTask, GameObject> processingQueue = new Dictionary<SlicerProcessingTask, GameObject>();
        
        protected void Update()
        {

            //TEST 
            CheckQueue();
            if(Input.GetKeyDown(KeyCode.J))
                InsertPotato();
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

        protected void CheckQueue()
        {
            foreach (var item in processingQueue.Keys.ToList())
            {
                item.Update();

                if (!item.IsProcessing)
                {
                    Destroy(processingQueue[item]);
                    processingQueue.Remove(item);
                }
            }
        }

        public void InsertPotato()
        {
            var direction = feeder.forward; // find the direction of ejection 
            GameObject outputObj = Instantiate(feederPrefab, feeder.position, Random.rotation); // instantiate
            processingQueue.Add(new SlicerProcessingTask(this), outputObj );
        }

        public void EjectPotato()
        {
            //TODO NEED TO INIT FSM ON THOSE NEW POTATOES
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
