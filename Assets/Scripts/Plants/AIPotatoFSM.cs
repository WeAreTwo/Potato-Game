﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

namespace PotatoGame
{

    [System.Serializable]
    public enum PotatoStates
    {
        Idle,
        Move,
        MoveToBell,
        Follow,
        Look,
        Eat,
        Dying,
        PickedUp
    }

    public class AIPotatoFSM : AIControllerFSM
    {
        [SerializeField] protected GameObject player;
        [SerializeField] protected Vector3 bellPosition;

        [SerializeField] protected AIPotatoFSM feedTarget;
        [SerializeField] protected float feedDistance = 1.0f;
        [SerializeField] protected float hunger = 100.0f;
        [SerializeField] protected float hungerDecreaseFactor = 3.0f;

        #region Properties
        public GameObject Player { get => player; set => player = value; }
        public Vector3 BellPosition { get => bellPosition; set => bellPosition = value; }
        public AIPotatoFSM FeedTarget { get => feedTarget; set => feedTarget = value; }
        public float FeedDistance { get => feedDistance; set => feedDistance = value; }
        public float Hunger { get => hunger; set => hunger = value; }
        public float HungerDecreaseFactor { get => hungerDecreaseFactor; set => hungerDecreaseFactor = value; }
        #endregion

        #region Call Methods
        
        protected override void Awake()
        {
            base.Awake();
            player = GameObject.FindWithTag(ProjectTags.Player);
            bellPosition = GameObject.FindWithTag(ProjectTags.Bell).transform.position;
        }

        protected override void Start()
        {
            fsm = new StateMachine();
            fsm.Add(PotatoStates.Idle, new IdleAI<AIPotatoFSM>(this));
            fsm.Add(PotatoStates.Move, new MoveAI<AIPotatoFSM>(this));
            fsm.Add(PotatoStates.MoveToBell, new MoveToBell<AIPotatoFSM>(this));
            fsm.Add(PotatoStates.Follow, new Follow<AIPotatoFSM>(this));
            fsm.Add(PotatoStates.Eat, new Feed<AIPotatoFSM>(this));
            fsm.Add(PotatoStates.Look, new Look<AIPotatoFSM>(this));
            fsm.Initialize(PotatoStates.Idle);
        }


        protected override void Update()
        {
            base.Update();
            fsm.Update();
            CheckHunger();
        }

        //Adding to list
        protected virtual void OnEnable()
        {
            if(manager && manager.plantsControllerFsm)
                manager.plantsControllerFsm.AutonomousPotatoes.Add(this);
        }

        protected virtual void OnDisable()
        {
            if(manager && manager.plantsControllerFsm)
                manager.plantsControllerFsm.AutonomousPotatoes.Remove(this);
        }
        
        #endregion

        //Potato general behaviour
        protected virtual void CheckHunger()
        {
            //decrease hunger
            hunger -= Time.deltaTime / hungerDecreaseFactor;
            
            //if hunger depletes, die
            if(hunger <= 0.0f)
            {
                if(ParticleController.Instance) ParticleController.Instance.EmitAt(this.transform.position);
                Destroy(this.gameObject);
            }
        }

        // protected override void CheckAnim()
        // {
        //     if (navAgent.isStopped)
        //     {
        //         _mAnim.SetBool("walking", false);
        //     }
        //     else
        //     {
        //         _mAnim.SetBool("walking", true);
        //     }
        // }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (fsm != null && fsm.CurrentStateKey != null)
            {
                Handles.color = Color.red;
                Handles.Label(
                    transform.position + Vector3.up * 2, 
                    "\nState: " +
                    Fsm.CurrentStateKey.ToString());
            }
        }
    }

    #region Base State

    //THIS IS GOING TO BE THE BRAIN OF THE DECISION MAKING 
    //Todo implement different decision making based on different AI
    //Note: i changed from potato base state to AIbasestate
    [System.Serializable]
    public class AIBaseState<T> : State where T : AIPotatoFSM
    {
        protected T component;

        public AIBaseState(T component)
        {
            this.component = component;
        }
        
        protected virtual void MakeDecision()
        {
            switch (component.Fsm.CurrentStateKey)
            {
                case PotatoStates.Idle:
                    BinaryBranching(PotatoStates.Look, PotatoStates.Move);
                    break;                
                case PotatoStates.Move:
                    BinaryBranching(PotatoStates.Idle, PotatoStates.Look);
                    break;                
                case PotatoStates.MoveToBell:
                    SingleBranch(PotatoStates.Follow);
                    break;                
                case PotatoStates.Eat:
                    SingleBranch(PotatoStates.Move);
                    break;                
                case PotatoStates.Follow:
                    SingleBranch(PotatoStates.Look);
                    break;                
                case PotatoStates.Look:
                    BinaryBranching(PotatoStates.Move, PotatoStates.Idle);
                    break;
            }
        }

        protected bool CoinFlip(float weight = 0.5f)
        {
            weight = Mathf.Clamp(weight, 0, 1);
            float flip = Random.value;
            if (flip < weight)
                return true;
            else
                return false;
        }
        
        //pick one outcome
        protected void SingleBranch(PotatoStates outcome)
        {
            TriggerExit(outcome);
        }
        
        //picks between 2 outcomes
        protected void BinaryBranching(PotatoStates outcomeOne, PotatoStates outcomeTwo, float weight = 0.5f)
        {
            if (CoinFlip(weight))
            {
                TriggerExit(outcomeOne);
            }
            else
            {
                TriggerExit(outcomeTwo);
            }
        }

        //pick between 3 or more outcomes
        protected void MultipleBranching(PotatoStates[] outcomes)
        {
            // will do this later ;) 
        }
        
    }
    
    #endregion

    #region Move TO Bell
    [System.Serializable]
    public class MoveToBell<T> : MoveAI<T> where T : AIPotatoFSM
    {
        public MoveToBell(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            MoveToPosition();
        }

        protected override void MoveToPosition()
        {
            // component.NavMesh.destination = component.BellPosition;
            component.NavMesh.SetDestination(component.BellPosition);
            if (component.NavMesh.isStopped)
            {
                // component.NavMesh.isStopped = true;
                MakeDecision();
            }
        }
        
        // protected override void MoveToPosition()
        // {
        //     component.Heading = (component.BellPosition - component.transform.position).normalized;
        //     //condition for completion 
        //     if (Vector3.Distance(component.transform.position, component.BellPosition) < 1.5f)
        //     {
        //         MakeDecision(); // make new decision 
        //     }
        // }
        
    }
    #endregion
    
    #region Follow State

    [System.Serializable]
    public class Follow<T> : MoveAI<T> where T : AIPotatoFSM
    {
        
        public Follow(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            MoveToPosition();
        }

        protected override void MoveToPosition()
        {
            // component.NavMesh.destination = playerPosition;
            component.NavMesh.SetDestination(component.Player.transform.position);
        }
    }

    #endregion

    #region Move State
    // AI CONTROLLER STATES
    [System.Serializable]
    public class MoveAI<T>: AIBaseState<T> where T: AIPotatoFSM
    {
        protected T component;
        
        protected float moveTimer = 0.0f;
        protected float moveTime = 5.0f;
    
        protected float popOutTimer = 0.0f;
        protected float popOutTime = 3.0f;

        public MoveAI(T component) : base(component)
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

        protected void PopOut()
        {
            if (popOutTimer < popOutTime)
            {
                popOutTimer += Time.deltaTime;
            }
            else
            {
                popOutTimer = 0;
            }
        }
    
        protected virtual void PickRandomPosition()
        {
            float randX = Random.Range(-1.0f, 1.0f);
            float randY = Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX,0,randY);
            Vector3 currentPosXZ = new Vector3(component.transform.position.x,0,component.transform.position.z);
            component.seekPosition = currentPosXZ + (randomPos * component.seekRange);

            component.NavMesh.SetDestination(component.seekPosition);
        }
    
        protected virtual void MoveToPosition()
        {
            if (moveTimer <= moveTime)
            {
                moveTimer += Time.deltaTime;
                component.seekPosition.y = component.transform.position.y;
                // component.Heading = (component.seekPosition - component.transform.position).normalized;
            }
            //condition for completion 
            if (Vector3.Distance(component.transform.position, component.seekPosition) < 1.5f || moveTimer >= moveTime)
            {
                moveTimer = 0;  // reset the timer 
                MakeDecision(); // make new decision 
            }
        }
        
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            //draw the seek range 
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(component.transform.position, component.seekRange);
            //draw the target 
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(component.seekPosition, 0.2f);
            //draw a line to target
            Gizmos.color = Color.black;
            Gizmos.DrawLine(component.transform.position, component.seekPosition);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one );
        }
    }
    #endregion

    #region Idle State

    [System.Serializable]
    public class IdleAI<T>: AIBaseState<T> where T: AIPotatoFSM
    {
        protected T component;

        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;

        public IdleAI(T component) : base(component)
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
                // TriggerExit(PlantStates.Move);
                MakeDecision();
            }
            else
            {
                component.Heading = Vector3.zero;
                component.NavMesh.StopNavigation(); // stops the nav mesh destination and path
            }
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one);
        }
    }
    #endregion

    #region Feed State

    [System.Serializable]
    public class Feed<T>: AIBaseState<T> where T: AIPotatoFSM
    {
        //will transition into feed state from look state 
        
        protected T component;
        
        protected float eatTimer = 0.0f;
        protected float eatTime = 3.0f;

        public Feed(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            Eat();
        }

        //ACTIONS 
        protected virtual void Eat()
        {
            //condition for completion
            if (component.FeedTarget == null)
            {
                // component.eatingEffect.SetActive(false);
                MakeDecision();
            }
            else
            {
                Vector3 targetPosition = component.FeedTarget.transform.position;
                if (Vector3.Distance(component.transform.position, targetPosition) < component.FeedDistance)
                {
                    component.NavMesh.StopNavigation();
                    
                    //eatimer is for decreasing healthbar by tigs
                    eatTimer += Time.deltaTime;
                    if (eatTimer >= eatTime)
                    {
                        eatTimer = 0.0f;
                        if (component.FeedTarget.Health - 25.0f <= 0)
                        {
                            component.Hunger += 10.0f;
                            component.FeedTarget.Health -= 25.0f;
                            component.FeedTarget.Die();
                        }
                        else
                        {
                            component.Hunger += 10.0f;
                            component.FeedTarget.Health -= 25.0f;
                        }
                    }
                }
                else
                {
                    //set navmesh destintation to target 
                    component.NavMesh.SetDestination(component.FeedTarget.transform.position);
                }
                
            }
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one);
        }
    }

    #endregion
    
    #region Look State

    [System.Serializable]
    public class Look<T>: AIBaseState<T> where T: AIPotatoFSM
    {
        //will transition into look after moving or idling
        
        protected T component;
        
        protected float lookTimer = 0.0f;
        protected float lookTime = 3.0f;
        

        public Look(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            LookAround();
        }

        //ACTIONS 
        protected virtual void LookAround()
        {
            //check for surrounding AI
            //check for food 
            //check for whatever
            
            //todo generalize and maybe get the closest?
            //todo prevent it from eating itself
            Collider[] hitColliders = Physics.OverlapSphere(component.transform.position, component.seekRange);
            foreach (Collider hit in hitColliders)
            {
                if(hit.TryGetComponent(out AIPotatoFSM potato))
                {
                    component.FeedTarget = potato;
                    TriggerExit(PotatoStates.Eat);
                }
            }
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one);
        }
    }

    #endregion
    
    #region Dying State

    [System.Serializable]
    public class Dying<T>: AIBaseState<T> where T: AIPotatoFSM
    {
        //will transition into look after moving or idling
        
        protected T component;

        protected bool hasFinishedDying = false;

        public Dying(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
            
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            DyingAnimation();
            CheckDeath();
        }

        IEnumerator DyingAnim()
        {
            //emit particle and other stuff it needs
            yield return null;
        }

        protected void DyingAnimation()
        {
            //process for dying here
            
            //it has finished doing everything before dying
            hasFinishedDying = true;
        }

        protected void CheckDeath()
        {
            if (hasFinishedDying)
            {
                component.Die();
            }
        }

    }

    #endregion
    
    #region Picked Up State

    [System.Serializable]
    public class PickedUp<T>: AIBaseState<T> where T: AIPotatoFSM
    {
        //will transition into look after moving or idling
        
        protected T component;

        protected bool isPickedUp = false;
        
        public PickedUp(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
            isPickedUp = true;
        }

        //Everyframe while ur in this state
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

        }

        protected void PickedUpState()
        {
            //disbale physics
            //disable pathfinding
            //do nothing else 
        }
        

    }

    #endregion
}