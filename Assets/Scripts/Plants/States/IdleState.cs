using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class Idle<T> : State where T : PotatoFSM
    {
        protected T component;

        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;
        
        protected float popOutTimer = 0.0f;
        protected float popOutTime = 3.0f;

        public Idle(T component)
        {
            this.component = component;
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if(component.Planted) PopOut();
            Wait();
        }
        
        protected void PopOut()
        {
            if (popOutTimer < popOutTime)
            {
                popOutTimer += Time.deltaTime;
            }
            else
            {
                PopOutOfTheGround();
                popOutTimer = 0;
            }
        }

        
        protected void PopOutOfTheGround()
        {
            //set the planted state to false 
            component.Planted = false;

            // pop out of the ground 
            component.transform.position += new Vector3(0, component.GrowthSettings.growthRadius * 2, 0);
            component.transform.rotation = Random.rotation;
    
            // Activate gravity and defreeze all
            component.Rb.ActivatePhysics();
    
            component.potatoEyes.SetActive(true);
        }

        //ACTIONS 
        protected virtual void Wait()
        {
            idleTimer += Time.deltaTime;
            //condition for completion
            if (idleTimer >= idleTime)
            {
                idleTimer = 0;
            }
            else
            {
                TriggerExit(PlantStates.MOVE);
            }
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthSettings.growthRadius);
        }
    }

}