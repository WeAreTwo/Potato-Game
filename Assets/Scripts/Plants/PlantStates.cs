using System.Collections;
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
            if (rb.useGravity == true) rb.useGravity = false;
            if (rb.constraints != RigidbodyConstraints.FreezeAll) rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected void Grow()
        {
            //growth period 
            if (growthParams.growthTime <= growthParams.growthCompletionTime && !growthCompleted)
            {
                growthParams.growthTime += Time.deltaTime;
                growing = true;
            }
            else if (growthParams.growthTime >= growthParams.growthCompletionTime)
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
            if (!growthCompleted) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.component.transform.position, growthParams.growthRadius);

            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.component.transform.position,
                this.component.transform.position + growthParams.growingAxis * 3.0f);

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
            else if (growthParams.harvestTime >= growthParams.harvestPeriod)
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
        protected StateMachine<PlantStates> potatoFSM;

        [Header("POTATO PARTS")] [SerializeField]
        protected GameObject potatoEyes;

        [SerializeField] protected GameObject eatingEffect;

        [Header("AUTONOMOUS AGENT")] [SerializeField]
        protected bool poppedOut;

        [SerializeField] protected Plant victim;
        [SerializeField] protected Vector3 seekPosition;
        [SerializeField] protected float seekRange = 5.0f;
        [SerializeField] protected float seekForce = 5.0f;

        //TIMERS
        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;
        protected float moveTimer = 0.0f;
        protected float moveTime = 5.0f;
        protected float eatTimer = 0.0f;
        protected float eatTime = 3.0f;

        protected Rigidbody rb;

        //CONSTRUCTOR
        public AutonomousState(GrowthParams growthParams)
        {
            this.growthParams = growthParams;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
            rb = component.GetComponent<Rigidbody>();
            PopOutOfTheGround();
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }

        protected virtual void PopOutOfTheGround()
        {
            // pop out of the ground 
            component.transform.position += new Vector3(0, growthParams.growthRadius, 0);
            component.transform.rotation = Random.rotation;

            // Activate gravity and defreeze all
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;

            PickRandomPosition();
            // potatoEyes.SetActive(true);
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

        //GIZMOS
        public override void DrawGizmos()
        {
            if(potatoFSM == null)
                return;
            
            switch (potatoFSM.Current.Name)
            {
                case "Idling":
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(component.transform.position, Vector3.one * growthParams.growthRadius);
                    break;
                case "Moving":
                    //draw the seek range 
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireSphere(component.transform.position, seekRange);
                    //draw the target 
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(seekPosition, 0.2f);
                    //draw a line to target
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(component.transform.position, seekPosition);

                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(component.transform.position, Vector3.one * growthParams.growthRadius);
                    break;
                case "Eating":
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(component.transform.position, Vector3.one * growthParams.growthRadius);

                    if (victim != null)
                    {
                        //draw a line to target
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(component.transform.position, victim.transform.position);
                    }

                    break;
                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public class Idle : State
    {
        

        //When you first switch into this state
        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }
        
        //When you exit this state
        public override void OnStateExit()
        {
            base.OnStateExit();
        }
    }
    
    [System.Serializable]
    public class Move : State
    {
        

        //When you first switch into this state
        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }
        
        //When you exit this state
        public override void OnStateExit()
        {
            base.OnStateExit();
        }
    }
    
    [System.Serializable]
    public class Eat : State
    {
        

        //When you first switch into this state
        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }
        
        //When you exit this state
        public override void OnStateExit()
        {
            base.OnStateExit();
        }
    }
    
    
}