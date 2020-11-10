using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotatoGame;
using RootMotion.Demos;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PotatoGame
{
    public class ActionController : MonoBehaviour
    {
        // public variables -------------------------
        [Title("Interactive States")] 
        public bool m_holding;  // Is the entity holding an object?

        [Title("Interactable Objects")] 
        public GameObject m_pickedObject;  // Object currently hold by the entity
        public GameObject m_proximityObject;  // Target caught by the trigger

        [Title("Planting Points")] 
        public List<GameObject> m_proximityPoints;  // Holds all near by possible planting points
        public GameObject m_closestPoint;  // The closest available planting point

        [Title("Physics")] 
        public float m_trowForce = 2.5f;  // Force applied to a trowed object 
        public float m_raycastOffsetX = 2f;  // Offset on the x axis for raycasts
        public float m_raycastOffsetZ = -0.2f;  // Offset on the z axis for raycasts


        // private variables ------------------------
        private bool _isPlayer;  // Determine if the entity is controlled by the player
        private Transform _planter;  // By where the entity will send a seed
        private BoxCollider _interactionBox;  // Collider with a trigger
        private Vector3 _rightCastOrigin = Vector3.zero;  // Used for right hand raycast starting point
        private Vector3 _leftCastOrigin = Vector3.zero;  // Used for left hand raycast starting point
        private Vector3 _leftDirectionToObject;
        private Vector3 _rightDirectionToObject;
        private IKController _ik;

        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get Components
            _interactionBox = GetComponent<BoxCollider>();
            _planter = gameObject.transform.GetChild(0);
            _ik = GameManager.Instance.ikController;

            // Is this action controller triggered bvy the player?
            _isPlayer = gameObject.transform.parent.CompareTag("Player");
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
        {

        }

        // ------------------------------------------
        // Methods
        // ------------------------------------------

        #region Pickable Object Collision
        
        // Scan an object when colliding with it ---------------------------------------
        private void OnTriggerEnter(Collider col)
        {
            CheckForNearbyPickableObject(col);
        }
        
        private void OnTriggerExit(Collider col)
        {
            ResetProximityObject(col);
        }
        
        
        // Check or reset the nearest pickable object ----------------------------------
        private void CheckForNearbyPickableObject(Collider col)
        {
            if(col.IsType<InteractableObject>() && !m_holding && col.gameObject != m_pickedObject)
                m_proximityObject = col.gameObject;
        }

        private void ResetProximityObject(Collider col)
        {
            if(col.IsType<InteractableObject>() && !m_holding && col.gameObject != m_pickedObject)
                m_proximityObject = null;            
        }
        
        #endregion
        
        #region Actions
        
        // Hold an object --------------------------------------------------------------
        public void Hold()
        {
            if (m_proximityObject == null)
                return;
            
            // Get the proximity object as the picked object
            m_pickedObject = m_proximityObject;
            m_proximityObject = null;
            
            // Make the new picked object as a child
            m_pickedObject.transform.SetParent(transform);
            
            // Deactivate physics on the picked object
            PhysicsSwitch(false, m_pickedObject.GetComponent<Rigidbody>());
            
            // Start the pick up process
            StartCoroutine(PickUp(0.3f, m_pickedObject));
        }


        // Trow a hold object ----------------------------------------------------------
        public void Trow()
        {
            // Reset the ik
            ResetHandWeight();

            // Bring the picked object on the default layer
            m_pickedObject.layer = 0;
            
            // Trow the object and reactivate object's physics
            var rb = m_pickedObject.GetComponent<Rigidbody>();
            var direction = transform.forward;
            
            PhysicsSwitch(true, rb);
            rb.velocity = direction * m_trowForce;
            
            // Ready for next action
            ResetInteraction();
        }

        
        // Wait before starting to hold and simulate pick up ---------------------------
        private IEnumerator PickUp(float delay, GameObject pickUpObject)
        {
            // Wait the delay before starting to hold
            yield return new WaitForSeconds(delay);
            // Make sure the player did not target another object
            m_holding = true;
            // m_proximityObject = pickUpObject;
            m_pickedObject = pickUpObject;
        }
        
        
        // Reset the actions -----------------------------------------------------------
        private void ResetInteraction()
        {
            // Get rid of the object
            m_pickedObject.transform.parent = null;
            m_pickedObject = null;

            // Clear hold
            m_holding = false;
        }
        
        #endregion

        # region Physics

        // Activate or deactivate physics on a rb --------------------------------------
        private void PhysicsSwitch(bool state, Rigidbody rb)
        {
            // Change gravity and transform collider behaviour 
            rb.useGravity = state;
            rb.gameObject.GetComponent<Collider>().isTrigger = state;

            // Change the constraints
            rb.constraints = state ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
        }
        
        #endregion

        #region IKs Raycasting
        // Set targets in real time for the hands --------------------------------------
        private void SetHandTargets()
        {
            var objectPos = m_pickedObject.transform.position;
            var layerMask = LayerMask.GetMask("InHand");
            _ik.ActivateWeight = true;

            // Capture current layer and change it
            m_pickedObject.layer = LayerMask.NameToLayer("InHand");

            // Set origins of the raycasts + offsets
            Vector3 objectPositionOffset = objectPos - transform.position;

            _leftCastOrigin = transform.TransformPoint((Vector3.left * m_raycastOffsetX) +
                                                        (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            _rightCastOrigin = transform.TransformPoint((Vector3.right * m_raycastOffsetX) +
                                                        (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            
            // we will use the normalized direction towards the prox. obj instead of a fixed direction
            _leftDirectionToObject = (objectPos - _leftCastOrigin).normalized;
            _rightDirectionToObject = (objectPos - _rightCastOrigin).normalized;
            
            // For left side ----------
            //todo need to reset parent transform when its not holding 
            RaycastHit leftEdge;
            if (Physics.Raycast(_leftCastOrigin, _leftDirectionToObject, out leftEdge, m_raycastOffsetX + 10.0f, layerMask))
            {
                if (m_pickedObject)
                {
                    _ik.LeftHandTarget.parent = m_pickedObject.transform;
                    _ik.LeftHandTarget.position = leftEdge.point;
                    _ik.LeftHandTarget.rotation = Quaternion.LookRotation(leftEdge.normal);
                }
            }
            
            // For right side ---------
            RaycastHit rightEdge;
            if (Physics.Raycast(_rightCastOrigin, _rightDirectionToObject, out rightEdge, m_raycastOffsetX + 10.0f, layerMask))
            {
                if (m_pickedObject)
                {
                    _ik.RightHandTarget.parent = m_pickedObject.transform;
                    _ik.RightHandTarget.position = rightEdge.point;
                    _ik.RightHandTarget.rotation = Quaternion.LookRotation(rightEdge.normal);
                }
            }
        }
        
        
        // Reset iks to original state -------------------------------------------------
        private void ResetHandWeight()
        {
            _ik.ActivateWeight = false;

            var newTrans = this.transform;
            
            //reset hand position
            _ik.m_leftHandTarget.parent = newTrans;
            _ik.m_rightHandTarget.parent = newTrans;
        }

        #endregion
    }
}
