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
        public float mMovementSpeed = 10f;             // Movement speed of the player
        public float mRotationSpeed = 5f;              // Movement speed for rotation
        [Title("Physics")]
        public float mGroundOffset = 0.2f;             // Where does the ground stands in relation to the player
        public float mGravityForce = -9.81f;           // Gravity that is applied
        public LayerMask mGround;                      // Ground layer (physics)
        
        // private variables ------------------------
        private CharacterController _mController;       // Instance of the character controller
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
            _mIsGrounded = Physics.CheckSphere(_mGroundCheck.position, mGroundOffset, mGround, QueryTriggerInteraction.Ignore);

            if (_mIsGrounded && _mVelocity.y < 0)
                _mVelocity.y = 0f;

            // Step between each movement
            var step = mMovementSpeed * Time.deltaTime;
            
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
                transform.rotation = Quaternion.Lerp(transform.rotation, _mLookRotation, mRotationSpeed * Time.deltaTime);
            }

            // Add gravity
            _mVelocity.y += mGravityForce * Time.deltaTime;
            _mController.Move(_mVelocity * Time.deltaTime);
        }
    }
}
