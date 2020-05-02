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
        public float m_movementSpeed = 10f;             // Movement speed of the player

        [Title("Walking Steps")]
        public float m_stepForce = 100f;                // Up force while moving (for steps)

        [Title("Main Camera Position (Read Only)", "Changes the player's inputs based on the camera's position")]
        [ReadOnly] public bool m_cameraIsNorth;         // Specify if the camera's position is North
        [ReadOnly] public bool m_cameraIsWest;          // Specify if the camera's position is West
        [ReadOnly] public bool m_cameraIsSouth;         // Specify if the camera's position is South
        [ReadOnly] public bool m_cameraIsEast;          // Specify if the camera's position is East


        // private variables ------------------------
        private Rigidbody m_rb;                         // Instance of the rigidbody
        private Vector3 m_direction = new Vector3(0, 0, 0); // Direction of the player's velocity 
        private CharacterController m_controller;       // Character controller linked to this object
        private bool m_isGrounded;                      // Check if the object is grounded or not


        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get the components
            m_rb = GetComponent<Rigidbody>();
            //m_controller = GetComponent<CharacterController>();

            // Is grounded on start
            m_isGrounded = true;
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        private void Update()
        {
            // Make the player able to move
            CheckInput();

            if (m_isGrounded && m_direction != Vector3.zero)
                PlayerSteps();
        }

        void FixedUpdate()
        {
            // Make the player move with physics
            m_rb.MovePosition(m_rb.position + m_direction * m_movementSpeed * Time.deltaTime);
        }


        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Check user's input ------------------------------------------------------
        private void CheckInput()
        {
            // Hold the value of the player's axis
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            // Input setup depending on the camera's position (point of view)
            if (m_cameraIsNorth)
                m_direction = new Vector3(-horizontalAxis, 0, -verticalAxis);

            if (m_cameraIsWest)
                m_direction = new Vector3(verticalAxis, 0, -horizontalAxis);

            if (m_cameraIsSouth)
                m_direction = new Vector3(horizontalAxis, 0, verticalAxis);

            if (m_cameraIsEast)
                m_direction = new Vector3(-verticalAxis, 0, horizontalAxis);

            // Look in the correct direction
            transform.rotation = Quaternion.LookRotation(m_direction);
        }


        // Update camera's position ------------------------------------------------
        public void UpdateCamera(int camIndex)
        {
            // If the camera is North (1), is West (2), is South (3)
            switch (camIndex)
            {
                case 1:
                    m_cameraIsNorth = true;
                    m_cameraIsWest = false;
                    m_cameraIsSouth = false;
                    m_cameraIsEast = false;
                    break;
                case 2:
                    m_cameraIsNorth = false;
                    m_cameraIsWest = true;
                    m_cameraIsSouth = false;
                    m_cameraIsEast = false;
                    break;
                case 3:
                    m_cameraIsNorth = false;
                    m_cameraIsWest = false;
                    m_cameraIsSouth = true;
                    m_cameraIsEast = false;
                    break;
                case 4:
                    m_cameraIsNorth = false;
                    m_cameraIsWest = false;
                    m_cameraIsSouth = false;
                    m_cameraIsEast = true;
                    break;
            }

            // Make a warning if camIndex is out of range
            if (camIndex < 1 || camIndex > 4)
                Debug.LogWarning("Impossible to determine the camera's position. Out of range.");
        }


        // Make the player jumpy ---------------------------------------------------
        private void PlayerSteps()
        {
            // Add a little jump
            m_rb.AddForce(Vector3.up * Mathf.Sqrt(m_stepForce * -0.6f * Physics.gravity.y), ForceMode.VelocityChange);

            // In air
            m_isGrounded = false;
        }


        // Detect when the player is mid air ---------------------------------------
        private void OnCollisionEnter(Collision col)
        {
            // Check if colliding with the ground
            if (col.gameObject.tag == "Ground")
                m_isGrounded = true;
        }
    }
}
