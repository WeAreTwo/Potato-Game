using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class SlicerProcessingTask
    {
        protected Slicer component;
        public GameObject slicedObject;
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
                component.EjectPlant();
            }
            
        }
    }

    public class MachineBase : InteractableStationary
    {

        [Header("MACHINE BASIC PARAMS")]
        [SerializeField] protected bool isProcessing;
        [SerializeField] protected float processingTime = 3.0f;
        [SerializeField] protected float processingTimer = 0;
        
        [Header("SLICER PARAMETERS")] 
        [SerializeField] protected Transform feeder;
        [SerializeField] protected Transform output;
        [SerializeField] protected float ejectionForce = 3.0f;

        public bool IsProcessing { get => isProcessing; set => isProcessing = value; }

        public override void Interact()
        {

        }

        public virtual void InsertPlant(Plant plant)
        {
            plant.transform.position = feeder.transform.position;
            plant.transform.rotation = Random.rotation;
            plant.gameObject.ActivatePhysics();
        }
        
        public virtual void InsertPlant()
        {
            var direction = feeder.forward; // find the direction of ejection 
        }

        public virtual void EjectPlant()
        {
            //TODO NEED TO INIT FSM ON THOSE NEW POTATOES
            var direction = output.forward; // find the direction of ejection 

        }

        
        // handles the scripts specific input when inserted
        protected virtual void InputSettings(ref GameObject obj)
        {
            if(obj.TryGetComponent<Plant>(out Plant plant))
            {
                //do something for plants 
                plant.PickedUp = false;
                plant.Planted = false;
            }
            
        }

        //handles the scripts specific output when instantiated
        protected virtual void OutputSettings(ref GameObject obj)
        {
            if(obj.TryGetComponent<Plant>(out Plant plant))
            {
                //do something for plants 
                plant.PickedUp = false;
                plant.Planted = false;
            }
            
        }

        
        protected virtual void OnDrawGizmos()
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