using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

// I usually namespace to avoid conflict and make it modular
namespace PotatoGame
{
    public class PlayerController : MonoBehaviour
    {
        // public variables -------------------------
        [Title("Controls")]
        public float m_movementSpeed = 10f;             // Movement speed of the player
        public float m_rotationSpeed = 5f;              // Movement speed for rotation
        [Title("Physics")]
        public float m_groundOffset = 0.2f;             // Where does the ground stands in relation to the player
        public float m_gravityForce = -9.81f;           // Gravity that is applied
        public LayerMask m_ground;                      // Ground layer (physics)
        
        // private variables ------------------------
        private CharacterController _mController;       // Instance of the character controller
        private Animator _mAnim;                        // Instance of the animator linked to the player
        private Transform _mGroundCheck;                // Instance of the ground check position
        private Vector3 _mVelocity;                     // Velocity to apply to the player
        private bool _mIsGrounded;                      // Check if the controller is in contact with the ground
        private Quaternion _mLookRotation;              // Rotation that need to be look at




        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get the components
            _mController = GetComponent<CharacterController>();
            _mAnim = GetComponent<Animator>();
            _mGroundCheck = transform.GetChild(0).transform;
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        private void Update()
        {
            // Make the player able to move
            CheckInput();
        }


        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Check user's input ------------------------------------------------------
        private void CheckInput()
        {
            // Check if the player is grounded
            _mIsGrounded = Physics.CheckSphere(_mGroundCheck.position, m_groundOffset, m_ground, QueryTriggerInteraction.Ignore);

            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            var step = m_movementSpeed * Time.deltaTime;
            
            // Take player's inputs
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");

            // Catch the inputs in a vector3
            var move = new Vector3(horizontalAxis, 0, verticalAxis);

            // When we record input, move the controller
            if (move != Vector3.zero)
            {
                _mController.Move(new Vector3(horizontalAxis, 0, verticalAxis) * step);
                _mLookRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Lerp(transform.rotation, _mLookRotation, m_rotationSpeed * Time.deltaTime);
                
                // Update the animator
                _mAnim.SetBool("walking", true);
            }
            else
            {
                // Return to iddle state
                _mAnim.SetBool("walking", false);
            }
            
            

            // Add gravity
            _mVelocity.y += m_gravityForce * Time.deltaTime;
            _mController.Move(_mVelocity * Time.deltaTime);
        }
    }
}
