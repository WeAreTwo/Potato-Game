using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class Idle<T> : State where T : Potato
    {
        protected T component;

        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;

        public Idle(T component)
        {
            this.component = component;
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            Wait();
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
                TriggerExit(PlantStates.Move);
            }
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthParams.growthRadius);
        }
    }

}