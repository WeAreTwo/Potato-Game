using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class GrownState<T> : State where T : PlantFSM
    {

        //MEMBERS
        protected T component;

        [SerializeField] protected bool harvestable;
        [SerializeField] protected bool harvestPeriodCompleted;

        //CONSTRUCTOR
        public GrownState(T component)
        {
            this.component = component;
        }

        //CALL METHODS 
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            Harvest();
        }

        protected void Harvest()
        {
            //harvest period
            if (component.GrowthParams.harvestTime <= component.GrowthParams.harvestPeriod && !harvestPeriodCompleted)
            {
                component.GrowthParams.harvestTime += Time.deltaTime;
                harvestable = true;
            }
            else if (component.GrowthParams.harvestTime >= component.GrowthParams.harvestPeriod)
            {
                harvestPeriodCompleted = true;
                harvestable = false;
                TriggerExit(PlantStates.Autonomous);
            }
        }

        //GIZMOS
        public override void DrawGizmos()
        {
            //HARVESTABLES
            if (harvestable)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(this.component.transform.position + Vector3.up * component.GrowthParams.growthRadius, 0.2f);
            }
        }

    }
}
