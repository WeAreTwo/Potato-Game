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
            if (component.Rb.useGravity == true) component.Rb.useGravity = false;
            if (component.Rb.constraints != RigidbodyConstraints.FreezeAll) component.Rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected void Grow()
        {
            //growth period 
            if (component.GrowthParams.growthTime <= component.GrowthParams.growthCompletionTime && !growthCompleted)
            {
                component.GrowthParams.growthTime += Time.deltaTime;
                growing = true;
            }
            else if (component.GrowthParams.growthTime >= component.GrowthParams.growthCompletionTime)
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
                this.component.transform.localScale *= component.GrowthParams.growthPace;
                component.GrowthParams.growthRadius *= component.GrowthParams.growthPace;
            }
        }

        protected void SetGrowthAxis()
        {
            component.GrowthParams.growingAxis.x = Random.Range(-0.50f, 0.50f);
            component.GrowthParams.growingAxis.z = Random.Range(-0.50f, 0.50f);
        }

        protected void GrowAlongAxis()
        {
            this.component.transform.position += component.GrowthParams.growingAxis * 0.001f;
        }

        //GIZMOS
        public override void DrawGizmos()
        {
            //GROWTH RADIUS
            if (!growthCompleted) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.component.transform.position, component.GrowthParams.growthRadius);

            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.component.transform.position,
                this.component.transform.position + component.GrowthParams.growingAxis * 3.0f);

        }

    }

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
                Gizmos.DrawSphere(this.component.transform.position + Vector3.up * component.GrowthParams.growthRadius, 0.2f);
            }
        }

    }

    [System.Serializable]
    public class AutonomousState<T> : State where T : PotatoFSM
    {
        protected T component;
        
        [Header("AUTONOMOUS AGENT")]
        [SerializeField] protected Plant victim;
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
            // component.potatoEyes.SetActive(true);
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

    [System.Serializable]
    public class Idle<T> : State where T : PotatoFSM
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
    
    [System.Serializable]
    public class Move<T> : State where T : PotatoFSM
    {
        protected T component;
        
        protected float moveTimer = 0.0f;
        protected float moveTime = 5.0f;
        
        protected Vector3 seekPosition; 
        protected float seekRange = 5.0f;
        protected float seekForce = 5.0f;

        public Move(T component)
        {
            this.component = component;
        }
    
        //When you first switch into this state
        public override void OnStateStart()
        {
            base.OnStateStart();
            PickRandomPosition();
        }
    
        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            MoveToPosition();
        }
        
        //When you exit this state
        public override void OnStateExit()
        {
            base.OnStateExit();
        }
        
        protected virtual void PickRandomPosition()
        {
            float randX = Random.Range(-1.0f, 1.0f);
            float randY = Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX,0,randY);
            Vector3 currentPosXZ = new Vector3(component.transform.position.x,0,component.transform.position.z);
            seekPosition = currentPosXZ + (randomPos * seekRange);
        }
        
        protected virtual void MoveToPosition()
        {
            if (moveTimer <= moveTime)
            {
                moveTimer += Time.deltaTime;
                seekPosition.y = component.transform.position.y;
                Vector3 force = (seekPosition - component.transform.position).normalized;
                component.Rb.AddForce(force * seekForce);            
            
            }
            //condition for completion 
            if (Vector3.Distance(component.transform.position, seekPosition) < 1.5f * component.GrowthParams.growthRadius || moveTimer >= moveTime)
            {
                moveTimer = 0;  // reset the timer 
                // MakeDecision(); // make new decision 
            }
        }
    
        public override void DrawGizmos()
        {
            base.DrawGizmos();
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
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthParams.growthRadius);
        }
    }
    
    [System.Serializable]
    public class Eat<T> : State where T : PotatoFSM
    {
        protected T component;

        protected float eatTimer = 0.0f;
        protected float eatTime = 3.0f;
        
        [Header("AUTONOMOUS AGENT")] 
        [SerializeField] protected Plant victim;
        [SerializeField] protected float seekForce = 5.0f;
    
        public Eat(T component)
        {
            this.component = component;
        }
    
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
        
        protected virtual void EatPotato()
        {
            //condition for completion
            if (victim == null)
            {
                component.eatingEffect.SetActive(false);
                // MakeDecision();
            }
            else
            {
                Vector3 targetPosition = victim.transform.position;
                if (Vector3.Distance(component.transform.position, targetPosition) < 2.5f * component.GrowthParams.growthRadius)
                {
                    eatTimer += Time.deltaTime;
                    if (eatTimer >= eatTime)
                    {
                        eatTimer = 0.0f;
                        if (victim.Health - 25.0f <= 0)
                        {
                            // killCount++;
                            victim.Health -= 25.0f;
                        }
                        else
                        {
                            victim.Health -= 25.0f;
                        }
                    }
                }
                else
                {
                    Vector3 force = (targetPosition - component.transform.position).normalized;
                    component.Rb.AddForce(force * seekForce);
                }
                
            }
        }
    
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one * component.GrowthParams.growthRadius);
    
            if (victim != null)
            {
                //draw a line to target
                Gizmos.color = Color.black;
                Gizmos.DrawLine(component.transform.position, victim.transform.position);
            }
    
        }
    }
    
    
}