
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    

    public class Slicer : MachineBase
    {
        [Header("OUTPUT OBJECT")] [SerializeField]
        protected GameObject feederPrefab;

        [SerializeField] protected GameObject outputPrefab;

        public Dictionary<SlicerProcessingTask, GameObject> processingQueue =
            new Dictionary<SlicerProcessingTask, GameObject>();

        protected void Update()
        {
            CheckQueue();

            //TEST 
            if (Input.GetKeyDown(KeyCode.J))
                InsertPlant();
            // if(Input.GetKeyDown(KeyCode.K))
            //     EjectPotato();
        }


        public override void Interact()
        {
            InsertPlant();
        }

        public void SlicePotato(Potato potato)
        {
            int yield = potato.GrowthParams.harvestYield;
        }

        // Potato outputPotato()
        // {
        //     Potato seedPotato = new Potato();
        //     // set init state to seed

        //     return seedPotato;
        // }

        protected void CheckQueue()
        {
            var queue = processingQueue.Keys.ToList();

            if (processingQueue.Count > 0)
            {
                isProcessing = true;
                queue[0].Update();

                if (!queue[0].IsProcessing)
                {
                    Destroy(processingQueue[queue[0]]);
                    processingQueue.Remove(queue[0]);
                }
            }
            else
            {
                isProcessing = false;
            }
        }

        IEnumerator ProcessPlant(SlicerProcessingTask task)
        {
            while (task.IsProcessing)
            {
                task.Update();
            }

            yield return null;
        }
        
        public override void InsertPlant(Plant plant)
        {
            plant.transform.position = feeder.transform.position;
            plant.transform.rotation = Random.rotation;
            plant.gameObject.ActivatePhysics();
            processingQueue.Add(new SlicerProcessingTask(this), plant.gameObject);
        }

        public override void InsertPlant()
        {
            var direction = feeder.forward; // find the direction of ejection 
            GameObject outputObj = Instantiate(feederPrefab, feeder.position, Random.rotation); // instantiate
            processingQueue.Add(new SlicerProcessingTask(this), outputObj);
            InputSettings(ref outputObj);
        }

        public override void EjectPlant()
        {
            //TODO NEED TO INIT FSM ON THOSE NEW POTATOES
            var direction = output.forward; // find the direction of ejection 
            GameObject outputObj = Instantiate(outputPrefab, output.position, Random.rotation); // instantiate
            outputObj.ThrowObject(direction, ejectionForce); // eject with force
            OutputSettings(ref outputObj);
        }
        
        
    }
}
