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
        public float m_movementSpeed = 10f; // Movement speed of the player

        [Title("Main Camera Position (Read Only)", "Changes the player's inputs based on the camera's position")]
        [ReadOnly]
        public bool m_cameraIsNorth; // Specify if the camera's position is North

        [ReadOnly] public bool m_cameraIsWest; // Specify if the camera's position is West
        [ReadOnly] public bool m_cameraIsSouth; // Specify if the camera's position is South
        [ReadOnly] public bool m_cameraIsEast; // Specify if the camera's position is East


        // private variables ------------------------
        private Rigidbody m_rb; // Instance of the rigidbody
        private Vector3 m_direction = new Vector3(0, 0, 0); // Direction of the player's velocity 


        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get the components
            m_rb = GetComponent<Rigidbody>();
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
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

            // Make the player move by changing its velocity
            m_rb.velocity = m_direction * m_movementSpeed;
        }


        // Update camera's position ------------------------------------------------
        public void UpdateCamera(int camIndex)
        {
            // If the camera is at North (1)
            if (camIndex == 1)
            {
                m_cameraIsNorth = true;
                m_cameraIsWest = false;
                m_cameraIsSouth = false;
                m_cameraIsEast = false;
            }

            // If the camera is at West (2)
            if (camIndex == 2)
            {
                m_cameraIsNorth = false;
                m_cameraIsWest = true;
                m_cameraIsSouth = false;
                m_cameraIsEast = false;
            }

            // If the camera is at South (3)
            if (camIndex == 3)
            {
                m_cameraIsNorth = false;
                m_cameraIsWest = false;
                m_cameraIsSouth = true;
                m_cameraIsEast = false;
            }

            // If the camera is at East (4)
            if (camIndex == 4)
            {
                m_cameraIsNorth = false;
                m_cameraIsWest = false;
                m_cameraIsSouth = false;
                m_cameraIsEast = true;
            }

            // Make a warning if camIndex is out of range
            if (camIndex < 1 || camIndex > 4)
                Debug.LogWarning("Impossible to determine the camera's position. Out of range.");
        }
    }
}
