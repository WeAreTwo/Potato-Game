using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MovementBase
    {
        [SerializeField] protected NavSettings navAgentSettings;
        
        protected NavMeshAgent navAgent;
        protected StateMachine fsm;
        protected PlantStates initState = PlantStates.Idle;
        
        protected Vector3 mouseTest = Vector3.zero;
        
        //navmesh velocity difference (for animation)
        protected float velocityTimer = 0;
        protected float velocityRecordDelay = 0.1f;
        protected Vector3 lastFrameVelocity;
        protected float speedDifference;
        protected float speedAnimThreshold = 0.009f;

        #region Properties
        public NavSettings NavAgentSettings { get => navAgentSettings; set => navAgentSettings = value; }
        public NavMeshAgent NavMesh { get => navAgent; set => navAgent = value; }
        public StateMachine Fsm { get => fsm; set => fsm = value; }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            navAgent = this.GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            CheckLastFrameVelocity();
        }

        //Adding to list
        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected void OnValidate()
        {
            if (navAgent)
            {
                navAgent.SetNavSetting(navAgentSettings);
            }
            else
            {
                navAgent = this.GetComponent<NavMeshAgent>();
                navAgent.SetNavSetting(navAgentSettings);
            }
        }

        protected virtual void CheckLastFrameVelocity()
        {
            velocityTimer += Time.deltaTime;
            if (velocityTimer > velocityRecordDelay)
            {
                velocityTimer = 0;
                speedDifference = Vector3.Distance(navAgent.velocity, lastFrameVelocity);
                // Debug.Log(speedDifference);
                // Debug.Log("Velocity Timer!");
            }
            lastFrameVelocity = navAgent.velocity;
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

        protected override void CheckAnim()
        {
            // if (navAgent.hasPath == false)
            if (speedDifference > speedAnimThreshold)
            {
                _mAnim.SetBool("walking", true);
            }
            else
            {
                _mAnim.SetBool("walking", false);
            }
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
    

}
