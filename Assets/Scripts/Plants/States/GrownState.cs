using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class GrownState<T> : State where T : Plant
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
            if (component.GrowthCharacteristics.harvestTime <= component.GrowthCharacteristics.harvestPeriod && !harvestPeriodCompleted)
            {
                component.GrowthCharacteristics.harvestTime += Time.deltaTime;
                harvestable = true;
            }
            else if (component.GrowthCharacteristics.harvestTime >= component.GrowthCharacteristics.harvestPeriod)
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
                Gizmos.DrawSphere(this.component.transform.position + Vector3.up * component.GrowthCharacteristics.growthRadius, 0.2f);
            }
        }

    }
}
