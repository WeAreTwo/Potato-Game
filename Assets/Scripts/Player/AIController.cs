using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    public class AIController : MovementBase
    {
        protected StateMachine fsm;
        protected PlantStates initState = PlantStates.Idle;
        
        protected Vector3 mouseTest = Vector3.zero;

        #region Properties
        public StateMachine Fsm { get => fsm; set => fsm = value; }
        #endregion

        protected override void Start()
        {
            base.Start();
            fsm = new StateMachine();
            fsm.Add(PlantStates.Idle, new IdleAI<AIController>(this));
            fsm.Add(PlantStates.Move, new MoveAI<AIController>(this));
            
            fsm.Initialize(initState);
        }

        protected override void Update()
        {
            base.Update();
            fsm.Update();
            // CheckInput();    
            // MoveToMousePosition();
        }

        //Adding to list
        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        // Check user's input ------------------------------------------------------
        protected override void CheckInput()
        {
            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            _movementStep = m_movementSpeed * Time.deltaTime;
            
            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            // float distToDest = Vector3.Distance(mouseTest, this.transform.position);
            // if (distToDest < 1.0f)
            // {
            //     _mHeading = Vector3.zero;
            // }
            // else
            // {
            //     _mHeading = (mouseTest - this.transform.position).normalized;
            // }

            // _mHeading.x = Mathf.PerlinNoise(Time.deltaTime, transform.position.z) * 2.0f - 1.0f;
            // _mHeading.z = Mathf.PerlinNoise(transform.position.x, Time.deltaTime) * 2.0f - 1.0f;
            
            // _mHeading.y = 0;
            _mHeading *= m_movementSpeed;
            _mHeading.x = Mathf.Clamp(_mHeading.x, -1.0f, 1.0f);
            _mHeading.z = Mathf.Clamp(_mHeading.z, -1.0f, 1.0f);
            
            _movementDirection = _mHeading;
            // _movementDirection = Camera.main.transform.TransformDirection(_movementDirection);
            _movementDirection.y = 0f;
        }

        protected virtual void MoveToMousePosition()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 touchPos = Input.mousePosition;
                Transform camTrans = Camera.main.transform;
                float dist = Vector3.Dot(this.transform.position - camTrans.position, camTrans.forward);
                touchPos.z = dist;
                mouseTest = Camera.main.ScreenToWorldPoint(touchPos);
                mouseTest.y = 0;
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            // Gizmos.color = Color.red;
            // Gizmos.DrawWireSphere(mouseTest, 0.5f);
        }
    }
    
    // AI CONTROLLER STATES
    [System.Serializable]
    public class IdleAI<T>: State where T: AIController
    {
        protected T component;

        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;
        
        protected float popOutTimer = 0.0f;
        protected float popOutTime = 3.0f;

        public IdleAI(T component)
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
    
    [System.Serializable]
    public class MoveAI<T>: State where T: AIController
    {
        protected T component;
        
        protected Vector3 seekPosition;
        protected float seekRange = 10.0f;
        protected float moveTimer = 0.0f;
        protected float moveTime = 5.0f;
        
        protected float popOutTimer = 0.0f;
        protected float popOutTime = 3.0f;

        public MoveAI(T component)
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

        protected virtual void MakeDecision()
        {
            //if there is a victim, go to eating mode 
            float roll = Random.value;
            if (roll < 0.45f)
                TriggerExit(PlantStates.Idle);
            else
                PickRandomPosition();

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


    // [System.Serializable]
    // public class MoveToBell<T> : MoveAI<T> where T: AIController
    // {
    //     protected T component;
    //     public MoveToBell(T component)
    //     {
    //         this.component = component;
    //     }
    // }

}
