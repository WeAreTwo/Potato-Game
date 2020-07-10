using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
   [System.Serializable]
    public class AutonomousState<T> : State where T : PotatoFSM
    {
        protected T component;
        protected Vector3 seekPosition;
        protected float transitionTime = 0;
        protected float transitionTimer = 5.0f;
        
        //CONSTRUCTOR
        public AutonomousState(T component)
        {
            this.component = component;
        }
    
        public override void OnStateStart()
        {
            base.OnStateStart();
            PopOutOfTheGround();
        }
    
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            Transition();
        }

        protected virtual void Transition()
        {
            if (transitionTime < transitionTimer)
            {
                transitionTime += Time.deltaTime;
            }
            else
            {
                transitionTime = 0;
                TriggerExit(PlantStates.Idle);
            }
        }
    
        protected void PopOutOfTheGround()
        {
            //set the planted state to false 
            component.Planted = false;

            // pop out of the ground 
            component.transform.position += new Vector3(0, component.GrowthSettings.growthRadius * 2, 0);
            component.transform.rotation = Random.rotation;
            
            //particles
            ParticleController.Instance.EmitAt(component.transform.position);
    
            // Activate gravity and defreeze all
            component.Rb.ActivatePhysics();
    
            PickRandomPosition();
            component.potatoEyes.SetActive(true);
        }
    
        protected bool CheckLineOfSight(Vector3 target)
        {
            //check to see if the victim is in the line of sight 
            RaycastHit hit;
            if (Physics.Raycast(component.transform.position, target, out hit, component.seekRange))
            {
                return true;
            }
    
            return false;
        }
    
        protected void PickRandomPosition()
        {
            float randX = Random.Range(-1.0f, 1.0f);
            float randY = Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX, 0, randY);
            Vector3 currentPosXZ = new Vector3(component.transform.position.x, 0, component.transform.position.z);
            seekPosition = currentPosXZ + (randomPos * component.seekRange);
        }
        
    }
}
