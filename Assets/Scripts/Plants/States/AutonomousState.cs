using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
   [System.Serializable]
    public class AutonomousState<T> : State where T : PotatoFSM
    {
        protected T component;
        
        [Header("AUTONOMOUS AGENT")]
        [SerializeField] protected Vector3 seekPosition;
        [SerializeField] protected float seekRange = 5.0f;
        [SerializeField] protected float seekForce = 5.0f;

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
    
        protected virtual void PopOutOfTheGround()
        {
            // pop out of the ground 
            component.transform.position += new Vector3(0, component.GrowthParams.growthRadius, 0);
            component.transform.rotation = Random.rotation;
    
            // Activate gravity and defreeze all
            component.Rb.useGravity = true;
            component.Rb.constraints = RigidbodyConstraints.None;
    
            PickRandomPosition();
            component.potatoEyes.SetActive(true);
        }
    
        protected virtual bool CheckLineOfSight(Vector3 target)
        {
            //check to see if the victim is in the line of sight 
            RaycastHit hit;
            if (Physics.Raycast(component.transform.position, target, out hit, seekRange))
            {
                return true;
            }
    
            return false;
        }
    
        protected virtual void PickRandomPosition()
        {
            float randX = Random.Range(-1.0f, 1.0f);
            float randY = Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX, 0, randY);
            Vector3 currentPosXZ = new Vector3(component.transform.position.x, 0, component.transform.position.z);
            seekPosition = currentPosXZ + (randomPos * seekRange);
        }
        
    }
}
