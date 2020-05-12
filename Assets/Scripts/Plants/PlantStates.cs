﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public enum PlantStates
    {
        Seed,
        Grown, 
        Autonomous
    }

    [System.Serializable]
    public class SeedState : State
    {
        //MEMBERS
        protected const string name = "Seed";
        protected GrowthParams growthParams;
        protected Rigidbody rb;
        [SerializeField] protected bool growing;
        [SerializeField] protected bool growthCompleted;

        //CONSTRUCTOR
        public SeedState(GrowthParams growthParams)
        {
            this.growthParams = growthParams;
        }
        
        //CALL METHODS 
        public override void OnStateStart()
        {
            base.OnStateStart();
            rb = component.GetComponent<Rigidbody>();
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            PlantedSettings();
            Grow();
            UpdateGrowthRadius();
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

        protected virtual void PlantedSettings()
        {
            // Deactivate gravity and freeze all
            if(rb.useGravity == true) rb.useGravity = false;
            if(rb.constraints != RigidbodyConstraints.FreezeAll) rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected void Grow()
        {
            //growth period 
            if (growthParams.growthTime <= growthParams.growthCompletionTime && !growthCompleted)
            {
                growthParams.growthTime += Time.deltaTime;
                growing = true;
            }
            else if(growthParams.growthTime >= growthParams.growthCompletionTime)
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
                this.component.transform.localScale *= growthParams.growthPace;
                growthParams.growthRadius *= growthParams.growthPace;
            }
        }

        protected void SetGrowthAxis()
        {
            growthParams.growingAxis.x = Random.Range(-0.50f, 0.50f);
            growthParams.growingAxis.z = Random.Range(-0.50f, 0.50f);
        }

        protected void GrowAlongAxis()
        {
            this.component.transform.position += growthParams.growingAxis * 0.001f;
        }

        //GIZMOS
        public override void DrawGizmos()
        {
            //GROWTH RADIUS
            if(!growthCompleted) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.component.transform.position, growthParams.growthRadius);
            
            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.component.transform.position, this.component.transform.position + growthParams.growingAxis * 3.0f);

        }

    }   
    
    [System.Serializable]
    public class GrownState : State
    {
        
        //MEMBERS
        protected GrowthParams growthParams;
        [SerializeField] protected bool harvestable;
        [SerializeField] protected bool harvestPeriodCompleted;
   
        //CONSTRUCTOR
        public GrownState(GrowthParams growthParams)
        {
            this.growthParams = growthParams;
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
            if (growthParams.harvestTime <= growthParams.harvestPeriod && !harvestPeriodCompleted)
            {
                growthParams.harvestTime += Time.deltaTime;
                harvestable = true;
            }
            else if(growthParams.harvestTime >= growthParams.harvestPeriod)
            {
                harvestPeriodCompleted = true;
                harvestable = false;
                TriggerExit(PlantState.Autonomous);
            }
        }
        
        //GIZMOS
        public override void DrawGizmos()
        {
            //HARVESTABLES
            if (harvestable)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(this.component.transform.position + Vector3.up * growthParams.growthRadius, 0.2f);
            }
        }
        
    }    
    
    [System.Serializable]
    public class AutonomousState : State
    {
        
        //MEMBERS
        protected GrowthParams growthParams;
   
        //CONSTRUCTOR
        public AutonomousState(GrowthParams growthParams)
        {
            this.growthParams = growthParams;
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }

        //GIZMOS
        public override void DrawGizmos()
        {
            
        }
    }



}
