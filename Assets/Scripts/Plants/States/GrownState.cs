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
            if (component.Planted) Harvest();
        }

        protected void Harvest()
        {
            //harvest period
            if (component.GrowthSettings.harvestTime <= component.GrowthSettings.harvestPeriod && !harvestPeriodCompleted)
            {
                component.GrowthSettings.harvestTime += Time.deltaTime;
                harvestable = true;
            }
            else if (component.GrowthSettings.harvestTime >= component.GrowthSettings.harvestPeriod)
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
                Gizmos.DrawSphere(this.component.transform.position + Vector3.up * component.GrowthSettings.growthRadius, 0.2f);
            }
        }

    }
}
