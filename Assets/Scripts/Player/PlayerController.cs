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
        public float m_floorOffset;




        // private variables ------------------------
        private Rigidbody m_rb;                         // Instance of the rigidbody
        private Camera m_cam;                           // Instance of the main camera in the scene   
        private Vector3 m_moveDirection;                // Where the player is going

        private Vector3 m_gravityForce;                 // Gravity that is applied with raycasters
        private Vector3 m_raycastFloorPos;
        private Vector3 m_combinedRaycast;
        private Vector3 m_floorMovement;            



        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get the components
            m_rb = GetComponent<Rigidbody>();
            m_cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        private void Update()
        {
            // Make the player able to move
            CheckInput();
        }

        void FixedUpdate()
        {
            // Make the player move with physics
            m_rb.velocity = (m_moveDirection * m_movementSpeed) + m_gravityForce;

        }


        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Check user's input ------------------------------------------------------
        private void CheckInput()
        {
            m_moveDirection = Vector3.zero;

            // Hold the value of the player's axis
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            // Always be true with the camera position
            Vector3 correctedHorizontal = horizontalAxis * Camera.main.transform.forward;
            Vector3 correctedVertical = verticalAxis * Camera.main.transform.right;
            Vector3 combinedInput = correctedHorizontal + correctedVertical;

            // Normalize the correction (for constant diagonal movement)
            m_moveDirection = new Vector3(combinedInput.normalized.x, 0, combinedInput.normalized.z);

            // Look in the correct direction 
            // (make sure it stays in same direction when no inputs)
            if (m_moveDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(-m_moveDirection);
        }


        
        
        
    }
}
