using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Potato
{
    public class TopViewCameraController : MonoBehaviour
    {
        // public variables -------------------------
        [ProgressBar("m_zoomMin", "m_zoomMax", Height = 20)]
        public float m_currentZoom;                     // Current player's zoom
        public float m_zoomSpeed = 300f;                // Speed when zooming in/out

        [Space(10)] [Title("Target and Position")]
        public Transform m_target;                      // The target that should be the subject to follow

        public float m_smoothSpeed = 0.125f;            // Smooth float for following movement
        public Vector3 offset;                          // Position of the camera arround the target


        // private variables ------------------------
        private GameObject m_player;                    // Instance of the player object
        private Camera m_cam;                           // Instance of the camera component
        private float m_zoomMin;                        // Minimum zoom of the camera
        private float m_zoomMax;                        // Maximum zoom of the camera


        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get components
            m_player = GameObject.FindGameObjectWithTag("Player");
            m_cam = GetComponent<Camera>();

            // Set zoom range and get the current zoom
            m_zoomMin = 10f;
            m_zoomMax = 35f;
            m_currentZoom = m_cam.fieldOfView;

            // Set the initial Position
            SetPosition(3, offset);
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
        {
            // Always listen to player's inputs
            Zoom();
            RotateCam();
        }

        void FixedUpdate()
        {
            // Look at the player
            FollowTarget();
        }


        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Follow a target (the player) --------------------------------------------
        private void FollowTarget()
        {
            // Instance of the desired position of the camera and the smooth(lerp) position
            Vector3 desiredPosition = m_target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_smoothSpeed);

            // Change the current position of the camera
            transform.position = smoothedPosition;

            // Make sure the camera is always looking at the target
            transform.LookAt(m_target);
        }


        // Zoom in and out ---------------------------------------------------------
        private void Zoom()
        {
            float step = m_zoomSpeed * Time.deltaTime;

            // Get the zoom directly from the mouse scroll input
            m_currentZoom += Input.GetAxis("Mouse ScrollWheel") * -step;

            // Safe net it
            if (m_currentZoom < m_zoomMin)
                m_currentZoom = m_zoomMin;
            else if (m_currentZoom > m_zoomMax)
                m_currentZoom = m_zoomMax;

            // Smooth zoom
            float smoothZoom = Mathf.Lerp(m_cam.fieldOfView, m_currentZoom, m_smoothSpeed);

            // Update current zoom
            m_cam.fieldOfView = smoothZoom;
        }

        
        // Rotate the camera in a direction ----------------------------------------
        private void RotateCam()
        {
            // Get the rotation axis
            float rotationAxis = Input.GetAxis("Rotate");
            float step = rotationAxis * 300f;

            Debug.Log(rotationAxis);
            // Rotate around the target
            transform.RotateAround(m_target.position, Vector3.up, step * Time.deltaTime);
        }


        // Set the camera on a new position ----------------------------------------
        private void SetPosition(int side, Vector3 nextPosition)
        {
            // Tell the player from which side the camera is standing (1 to 4 : N, W, S, E)
            var playerController = GameManager.Instance.playerController;
            playerController.UpdateCamera(side);

            // Move the camera to the next position it should be at
            offset = nextPosition;
        }
    }
}
