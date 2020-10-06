﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;

namespace PotatoGame
{
    //todo delete all character controller related variables if navmesh based movement is way to go
    [RequireComponent(typeof(Animator))]                //for animation
    public class MovementBase : MonoBehaviour
    {
        
        // public variables -------------------------
        [Title("Controls")]
        [SerializeField] protected float m_movementSpeed = 10f;             // Movement speed of the player
        [SerializeField] protected float m_rotationSpeed = 5f;              // Movement speed for rotation
        [Title("Physics")]
        [SerializeField] protected float m_groundOffset = 0.2f;             // Where does the ground stands in relation to the player
        [SerializeField] protected float m_gravityForce = -9.81f;           // Gravity that is applied
        [SerializeField] protected LayerMask m_ground;                      // Ground layer (physics)
        
        // private variables ------------------------
        protected CharacterController _mController;       // Instance of the character controller
        protected Animator _mAnim;                        // Instance of the animator linked to the player
        protected Transform _mGroundCheck;                // Instance of the ground check position
        [SerializeField] protected Vector3 _mHeading = Vector3.zero;
        protected Vector3 _movementDirection;
        protected Vector3 _mVelocity;                     // Velocity to apply to the player
        protected float _movementStep;
        protected bool _mIsGrounded;                      // Check if the controller is in contact with the ground
        protected Quaternion _mLookRotation;              // Rotation that need to be look at


        #region Properties
        public Vector3 Heading { get => _mHeading; set => _mHeading = value; }

        #endregion
        
        
        protected virtual void Awake()
        {
            _mAnim = this.GetComponent<Animator>();
            // _mController = this.GetComponent<CharacterController>();
            // _mGroundCheck = transform.GetChild(0).transform; //Todo eliminate this dependancy (maybe using a vector3 inside script?)
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            CheckInput();
            // CheckGround();
            CheckAnim();
            // Movement();
            NavMeshMovement();
        }

        protected virtual void CheckInput()
        {
            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            _movementStep = m_movementSpeed * Time.deltaTime;
            
            // Take player's inputs
            _mHeading.x = Input.GetAxis("Horizontal");
            _mHeading.z = Input.GetAxis("Vertical");

            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            _movementDirection = _mHeading;
            _movementDirection = Camera.main.transform.TransformDirection(_movementDirection);
            _movementDirection.y = 0f;
        }
        
        protected virtual void NavMeshMovement()
        {
            //refL https://www.youtube.com/watch?v=bH33Qvhvl40 Ciro from unity
            float inputMagnitude = _mHeading.sqrMagnitude;
            if (inputMagnitude >= .01f)
            {
                Vector3 newPosition = transform.position + _movementDirection * (m_movementSpeed * Time.deltaTime);
                NavMeshHit hit;
                bool isValid = NavMesh.SamplePosition(newPosition, out hit, .3f, NavMesh.AllAreas);
                if (isValid)
                {
                    if ((transform.position - hit.position).magnitude >= .02f)
                    {
                        transform.position = hit.position;
                    }
                    else
                    {
                        //movement stopped this frame
                    }
                }
                else
                {
                    //no input from player
                }
                
                _movementDirection = Vector3.ClampMagnitude(_movementDirection, 1);
                _mLookRotation = Quaternion.LookRotation(_movementDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, _mLookRotation, m_rotationSpeed * Time.deltaTime);
            }
        }

        protected virtual void CheckGround()
        {
            // Check if the player is grounded
            _mIsGrounded = Physics.CheckSphere(_mGroundCheck.position, m_groundOffset, m_ground, QueryTriggerInteraction.Ignore);

        }
        
        protected virtual void CheckAnim()
        {
            if (_movementDirection != Vector3.zero)
            {
                // Update the animator
                _mAnim.SetBool("walking", true);
            }
            else
            {
                // Return to iddle state
                _mAnim.SetBool("walking", false);
            }
        }

        protected virtual void Movement()
        {
            // When we record input, move the controller
            if (_movementDirection != Vector3.zero)
            {
                _movementDirection = Vector3.ClampMagnitude(_movementDirection, 1);
                _mController.Move(_movementDirection * _movementStep);
                _mLookRotation = Quaternion.LookRotation(_movementDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, _mLookRotation, m_rotationSpeed * Time.deltaTime);
                
            }

            // Add gravity
            _mVelocity.y += m_gravityForce * Time.deltaTime;
            _mController.Move(_mVelocity * Time.deltaTime);
        }

        protected virtual void OnDrawGizmos()
        {
            if (_mGroundCheck)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_mGroundCheck.position, m_groundOffset);
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + _mHeading);
        }
    }

}
