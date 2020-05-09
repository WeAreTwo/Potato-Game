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
            // If not on ground, increase gravity force
            if (FloorRaycasts(0, 0, 0.6f) == Vector3.zero)
                m_gravityForce += Vector3.up * Physics.gravity.y * Time.fixedDeltaTime;

            // Make the player move with physics
            m_rb.velocity = (m_moveDirection * m_movementSpeed) + m_gravityForce;

            // Find Y pos with raycasts
            m_floorMovement = new Vector3(m_rb.position.x, FindFloor().y + )



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


        private Vector3 FindFloor()
        {
            // Width of raycasts under the character
            float raycastWidth = 0.25f;
            int floorAverage = 1;

            m_combinedRaycast = FloorRaycasts(0, 0, 1.6f);
            floorAverage += (getFloorAverage(raycastWidth, 0) + getFloorAverage(-raycastWidth, 0) + getFloorAverage(0, raycastWidth) + getFloorAverage(0, -raycastWidth));

            return m_combinedRaycast / floorAverage;
        }


        private int getFloorAverage(float offsetX, float offsetZ)
        {
            if (FloorRaycasts(offsetX, offsetZ, 1.6ff) != Vector3.zero)
            {
                m_combinedRaycast += FloorRaycasts(offsetX, offsetZ, 1.6f);
                return 1;
            }
            else
                return 0;
        }


        // Detect when the player is mid air ---------------------------------------
        private Vector3 FloorRaycasts(float offsetX, float offsetZ, float raycastLenght)
        {
            RaycastHit hit;

            m_raycastFloorPos = transform.TransformPoint(0 + offsetX, 0 + 0.5f, 0 + offsetZ);

            // Print the ray lines in the editor
            Debug.DrawRay(m_raycastFloorPos, Vector3.down, Color.magenta);

            //
            if (Physics.Raycast(m_raycastFloorPos, -Vector3.up, out hit, raycastLenght))
                return hit.point;
            else
                return Vector3.zero;
        }
        
        
    }
}
