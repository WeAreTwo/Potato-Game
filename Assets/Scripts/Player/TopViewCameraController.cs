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
        [ProgressBar("m_zoomMin", "m_zoomMax", Height = 20)]
        public float mCurrentZoom;                     // Current player's zoom
        public float mZoomSpeed = 300f;                // Speed when zooming in/out

        [Space(10)] [Title("Target and Position")]
        public Transform mTarget;                      // The target that should be the subject to follow

        public float mSmoothSpeed = 0.125f;            // Smooth float for following movement
        public Vector3 offset;                          // Position of the camera around the target


        // private variables ------------------------
        private GameObject _mPlayer;                    // Instance of the player object
        private Camera _mCam;                           // Instance of the camera component
        private float _mZoomMin;                        // Minimum zoom of the camera
        private float _mZoomMax;                        // Maximum zoom of the camera


        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get components
            _mPlayer = GameObject.FindGameObjectWithTag("Player");
            _mCam = GetComponent<Camera>();

            // Set zoom range and get the current zoom
            _mZoomMin = 10f;
            _mZoomMax = 35f;
            mCurrentZoom = _mCam.fieldOfView;
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

        void LateUpdate()
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
            Vector3 desiredPosition = mTarget.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, mSmoothSpeed);

            // Change the current position of the camera
            transform.position = smoothedPosition;

            // Make sure the camera is always looking at the target
            transform.LookAt(mTarget);
        }


        // Zoom in and out ---------------------------------------------------------
        private void Zoom()
        {
            var step = mZoomSpeed * Time.deltaTime;

            // Get the zoom directly from the mouse scroll input
            mCurrentZoom += Input.GetAxis("Mouse ScrollWheel") * -step;

            // Safe net it
            if (mCurrentZoom < _mZoomMin)
                mCurrentZoom = _mZoomMin;
            else if (mCurrentZoom > _mZoomMax)
                mCurrentZoom = _mZoomMax;

            // Smooth zoom
            var smoothZoom = Mathf.Lerp(_mCam.fieldOfView, mCurrentZoom, mSmoothSpeed);

            // Update current zoom
            _mCam.fieldOfView = smoothZoom;
        }

        
        // Rotate the camera in a direction ----------------------------------------
        private void RotateCam()
        {
            // Get the rotation axis
            var rotationAxis = Input.GetAxis("Rotate");
            var step = rotationAxis * 300f;

            // Rotate around the target
            transform.RotateAround(mTarget.position, Vector3.up, step * Time.deltaTime);
        }
    }
}
