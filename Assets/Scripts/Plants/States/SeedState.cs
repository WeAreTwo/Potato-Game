using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
   [System.Serializable]
    public class SeedState<T> : State where T : PlantFSM
    {
        //MEMBERS
        protected T component;
        protected const string name = "Seed";

        [SerializeField] protected bool growing;
        [SerializeField] protected bool growthCompleted;

        //CONSTRUCTOR
        public SeedState(T component)
        {
            this.component = component;
        }

        //CALL METHODS 
        public override void OnStateStart()
        {
            base.OnStateStart();
            // component.transform.localScale *= component.GrowthParams.seedSize;
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

            if (component.Planted)
            {
                // PlantedSettings();
                Grow();
                UpdateGrowthRadius();
            }
            else
            {
                // UprootedSettings();
            }
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
        }

        public override void OnCollisionEnter(Collision col)
        {
            var plantComponent = col.gameObject.GetComponent<PlantFSM>();
            //if its also in the same state (this is what get type does)
            // if (plantComponent != null && plantComponent.States.Current.Name == name)
            // {
            //     growthCompleted = true;
            // }
            if (plantComponent != null)
            {
                growthCompleted = true;
            }
        }

        protected virtual void UprootedSettings()
        {
            // Deactivate gravity and freeze all
            if (component.Rb.useGravity == false) component.Rb.useGravity = true;
            if (component.Rb.constraints != RigidbodyConstraints.None) component.Rb.constraints = RigidbodyConstraints.None;
        }

        protected virtual void PlantedSettings()
        {
            // Deactivate gravity and freeze all
            if (component.Rb.useGravity == true) component.Rb.useGravity = false;
            if (component.Rb.constraints != RigidbodyConstraints.FreezeAll) component.Rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected void Grow()
        {
            
            //growth period 
            if (component.GrowthSettings.growthTime <= component.GrowthSettings.growthCompletionTime && !growthCompleted)
            {
                component.GrowthSettings.growthTime += Time.deltaTime;
                growing = true;
            }
            else if (component.GrowthSettings.growthTime >= component.GrowthSettings.growthCompletionTime)
            {
                growthCompleted = true;
                growing = false;
                TriggerExit(PlantStates.Grown);
            }
        }

        protected void UpdateGrowthRadius()
        {
            if (growing && !growthCompleted)
            {
                float dt = component.GrowthSettings.growthTime / component.GrowthSettings.growthCompletionTime;
                // this.component.transform.localScale = Vector3.Lerp(
                //     Vector3.one * component.GrowthParams.seedSize,
                //     Vector3.one * component.GrowthParams.growthSize,
                //     dt
                //     );
                //
                // this.component.transform.localScale *= component.GrowthParams.growthPace;
                component.GrowthSettings.growthRadius *= component.GrowthSettings.growthPace;
            }
        }

        protected void SetGrowthAxis()
        {
            component.GrowthSettings.growingAxis.x = Random.Range(-0.50f, 0.50f);
            component.GrowthSettings.growingAxis.z = Random.Range(-0.50f, 0.50f);
        }

        protected void GrowAlongAxis()
        {
            this.component.transform.position += component.GrowthSettings.growingAxis * 0.001f;
        }

        //GIZMOS
        public override void DrawGizmos()
        {
            //GROWTH RADIUS
            if (!growthCompleted) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.component.transform.position, component.GrowthSettings.growthRadius);

            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.component.transform.position,
                this.component.transform.position + component.GrowthSettings.growingAxis * 3.0f);

        }

    }

}