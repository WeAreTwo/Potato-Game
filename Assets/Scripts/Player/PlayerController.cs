using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
        public float m_gravityForce = -9.81f;           // Gravity that is applied with raycasters 
        public LayerMask m_ground;                      // Ground layer (physics)




        // private variables ------------------------
        private CharacterController m_controller;       // Instance of the character controller
        private Transform m_groundCheck;                // Instance of the ground check position
        private Vector3 m_velocity;                     // Velocity to apply to the player
        private bool m_isGrounded;                      // Check if the controller is in contact with the ground
        private Quaternion m_lookRotation;              // Rotation that need to be look at




        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get the components
            m_controller = GetComponent<CharacterController>();
            m_groundCheck = transform.GetChild(0).transform;
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
            m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_groundOffset, m_ground, QueryTriggerInteraction.Ignore);

            if (m_isGrounded && m_velocity.y < 0)
                m_velocity.y = 0f;

            // Step between each movement
            float step = m_movementSpeed * Time.deltaTime;
            
            // Take player's inputs
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            // Catch the inputs in a vector3
            Vector3 move = new Vector3(horizontalAxis, 0, verticalAxis);

            // When we record input, move the controller
            if (move != Vector3.zero)
            {
                m_controller.Move(new Vector3(horizontalAxis, 0, verticalAxis) * step);
                m_lookRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Lerp(transform.rotation, m_lookRotation, m_rotationSpeed * Time.deltaTime);
            }

            // Add gravity
            m_velocity.y += m_gravityForce * Time.deltaTime;
            m_controller.Move(m_velocity * Time.deltaTime);
        }
    }
}
