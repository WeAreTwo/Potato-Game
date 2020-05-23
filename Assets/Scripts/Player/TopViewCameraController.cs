using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PotatoGame
{
    public class TopViewCameraController : MonoBehaviour
    {
        // public variables -------------------------
        [ProgressBar("m_zoomMin", "_mZoomMax", Height = 20)]
        public float m_currentZoom; // Current player's zoom
        public float m_zoomSpeed = 300f; // Speed when zooming in/out
        public float m_rotateSpeed = 25f; // Rotation speed of the camera around the target

        [Space(10)] [Title("Target and Position")]
        public Transform m_target; // The target that should be the subject to follow

        public float m_smoothSpeed = 0.125f; // Smooth float for following movement
        public Vector3 offset; // Position of the camera around the target


        // private variables ------------------------
        private Camera _mCam; // Instance of the camera component
        private float _mZoomMin; // Minimum zoom of the camera
        private float _mZoomMax; // Maximum zoom of the camera


        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get components
            _mCam = GetComponent<Camera>();

            // Set zoom range and get the current zoom
            _mZoomMin = 10f;
            _mZoomMax = 35f;
            m_currentZoom = _mCam.fieldOfView;
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
        {
            // Always listen to player's inputs
            Zoom();
        }

        void LateUpdate()
        {
            // Look at the player
            RotateCam();
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
            var step = m_zoomSpeed * Time.deltaTime;

            // Get the zoom directly from the mouse scroll input
            m_currentZoom += Input.GetAxis("Mouse ScrollWheel") * -step;

            // Safe net it
            if (m_currentZoom < _mZoomMin)
                m_currentZoom = _mZoomMin;
            else if (m_currentZoom > _mZoomMax)
                m_currentZoom = _mZoomMax;

            // Smooth zoom
            var smoothZoom = Mathf.Lerp(_mCam.fieldOfView, m_currentZoom, m_smoothSpeed);

            // Update current zoom
            _mCam.fieldOfView = smoothZoom;
        }

        
        // Rotate the camera in a direction ----------------------------------------
        private void RotateCam()
        {
            // Get the rotation axis
            var rotationAxis = Input.GetAxis("Rotate");
            var step = rotationAxis * m_rotateSpeed;

            // Rotate around the target while keeping the offset
            offset = Quaternion.AngleAxis(step, Vector3.up) * offset;
        }
    }
}
