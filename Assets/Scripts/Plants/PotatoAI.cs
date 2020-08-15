using System.Collections;
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
        Eat
    }

    public class PotatoAI : AIController
    {
        [SerializeField] protected GameObject player;
        [SerializeField] protected float hunger = 100.0f;
        [SerializeField] protected float hungerDecreaseFactor = 3.0f;
        [SerializeField] protected Vector3 bellPosition;

        #region Properties
        public GameObject Player { get => player; set => player = value; }
        public float Hunger { get => hunger; set => hunger = value; }
        public float HungerDecreaseFactor { get => hungerDecreaseFactor; set => hungerDecreaseFactor = value; }
        public Vector3 BellPosition { get => bellPosition; set => bellPosition = value; }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            player = GameObject.FindWithTag(ProjectTags.Player);
            bellPosition = GameObject.FindWithTag(ProjectTags.Bell).transform.position;
        }

        protected override void Start()
        {
            fsm = new StateMachine();
            fsm.Add(PotatoStates.Idle, new IdleAI<PotatoAI>(this));
            fsm.Add(PotatoStates.Move, new MoveAI<PotatoAI>(this));
            fsm.Add(PotatoStates.MoveToBell, new MoveToBell<PotatoAI>(this));
            fsm.Add(PotatoStates.Follow, new Follow<PotatoAI>(this));
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
            if(GameManager.Instance && GameManager.Instance.plantsController)
                GameManager.Instance.plantsController.AutonomousPotatoes.Add(this);
        }

        protected virtual void OnDisable()
        {
            if(GameManager.Instance && GameManager.Instance.plantsController)
                GameManager.Instance.plantsController.AutonomousPotatoes.Remove(this);
        }
        
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

        protected override void CheckAnim()
        {
            if (navAgent.isStopped)
            {
                _mAnim.SetBool("walking", false);
            }
            else
            {
                _mAnim.SetBool("walking", true);
            }
        }

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

    //THIS IS GOING TO BE THE BRAIN OF THE DECISION MAKING 
    [System.Serializable]
    public class PotatoBaseState<T> : State where T : PotatoAI
    {
        protected T component;

        public PotatoBaseState(T component)
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

    [System.Serializable]
    public class MoveToBell<T> : MoveAI<T> where T : PotatoAI
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
    
    [System.Serializable]
    public class Follow<T> : MoveAI<T> where T : PotatoAI
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

    
    // AI CONTROLLER STATES
    [System.Serializable]
    public class MoveAI<T>: PotatoBaseState<T> where T: PotatoAI
    {
        // protected T component;
    
        protected Vector3 seekPosition;
        protected float seekRange = 10.0f;
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
            seekPosition = currentPosXZ + (randomPos * seekRange);
        }
    
        protected virtual void MoveToPosition()
        {
            if (moveTimer <= moveTime)
            {
                moveTimer += Time.deltaTime;
                seekPosition.y = component.transform.position.y;
                component.Heading = (seekPosition - component.transform.position).normalized;
            }
            //condition for completion 
            if (Vector3.Distance(component.transform.position, seekPosition) < 1.5f || moveTimer >= moveTime)
            {
                moveTimer = 0;  // reset the timer 
                MakeDecision(); // make new decision 
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
            Gizmos.DrawWireCube(component.transform.position, Vector3.one );
        }
    }

    [System.Serializable]
    public class IdleAI<T>: PotatoBaseState<T> where T: PotatoAI
    {
        // protected T component;

        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;

        protected float popOutTimer = 0.0f;
        protected float popOutTime = 3.0f;

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
                TriggerExit(PlantStates.Move);
            }
            else
            {
                component.Heading = Vector3.zero;
            }
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(component.transform.position, Vector3.one);
        }

    }
}