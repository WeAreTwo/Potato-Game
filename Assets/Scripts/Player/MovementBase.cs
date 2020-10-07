using System;
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
        [Header("Movement Params")]
        protected float movementSpeed = 10f;             // Movement speed of the player
        protected float rotationSpeed = 15f;              // Movement speed for rotation
        [SerializeField] protected Vector3 heading = Vector3.zero;
        protected Vector3 movementDirection;
        protected Vector3 movementVelocity;                     // Velocity to apply to the player
        protected float movementStep;
        protected Quaternion lookAtRotation;              // Rotation that need to be look at
        
        
        [Header("Ground Params")]
        [SerializeField] protected float groundOffset = 0.2f;             // Where does the ground stands in relation to the player
        [SerializeField] protected float gravityForce = -9.81f;           // Gravity that is applied
        [SerializeField] protected LayerMask groundMask;                      // Ground layer (physics)
        protected bool isGrounded;                      // Check if the controller is in contact with the ground
        protected Transform groundCheck;                // Instance of the ground check position
        
        // COMPONENTS
        protected CharacterController charControllerComponent;       // Instance of the character controller
        protected Animator animComponent;                        // Instance of the animator linked to the player


        #region Properties
        public Vector3 Heading { get => heading; set => heading = value; }

        #endregion
        
        
        protected virtual void Awake()
        {
            animComponent = this.GetComponent<Animator>();
            // _mController = this.GetComponent<CharacterController>();
            // _mGroundCheck = transform.GetChild(0).transform; //Todo eliminate this dependancy (maybe using a vector3 inside script?)
        }

        protected virtual void Start(){}

        protected virtual void Update()
        {
            CheckInput();
            CheckAnim();
            Rotation();
        }

        protected virtual void CheckInput()
        {
            if (isGrounded && movementVelocity.y < 0)
                movementVelocity.y = 0f;

            // Step between each movement
            movementStep = movementSpeed * Time.deltaTime;
            
            // Take player's inputs
            heading.x = Input.GetAxis("Horizontal");
            heading.z = Input.GetAxis("Vertical");

            // Catch the inputs in a vector3
            // (make sure inputs makes sense with camera view)
            movementDirection = heading;
            movementDirection = Camera.main.transform.TransformDirection(movementDirection);
            movementDirection.y = 0f;
        }
        
        protected virtual void CheckGround()
        {
            // Check if the player is grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundOffset, groundMask, QueryTriggerInteraction.Ignore);

        }
        
        protected virtual void CheckAnim()
        {
            if (movementDirection != Vector3.zero)
            {
                // Update the animator
                animComponent.SetBool("walking", true);
            }
            else
            {
                // Return to iddle state
                animComponent.SetBool("walking", false);
            }
        }

        protected virtual void Rotation()
        {
            // When we record input, move the controller
            if (movementDirection != Vector3.zero)
            {
                movementDirection = Vector3.ClampMagnitude(movementDirection, 1);
                lookAtRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, rotationSpeed * Time.deltaTime);
            }
        }

        protected virtual void Movement()
        {
            // When we record input, move the controller
            if (movementDirection != Vector3.zero)
            {
                movementDirection = Vector3.ClampMagnitude(movementDirection, 1);
                charControllerComponent.Move(movementDirection * movementStep);
                lookAtRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, rotationSpeed * Time.deltaTime);
                
            }

            // Add gravity
            movementVelocity.y += gravityForce * Time.deltaTime;
            charControllerComponent.Move(movementVelocity * Time.deltaTime);
        }

        protected virtual void OnDrawGizmos()
        {
            if (groundCheck)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundOffset);
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + heading);
        }
    }

}
