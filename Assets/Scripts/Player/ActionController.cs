using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PotatoGame
{
    [RequireComponent(typeof(IKController))]
    public class ActionController : MonoBehaviour
    {
        // public variables -------------------------
        public GameObject m_proximityObject; // Target caught by a trigger
        public GameObject m_planterObject;
        public float m_throwForce = 2.5f; // Force when an object is trow after holding
        public float m_raycastOffsetX = 2f; // Offset on the x axis for raycasts
        public float m_raycastOffsetZ = -0.2f; // Offset on the z axis for raycasts

        // private variables ------------------------
        private BoxCollider _mInteractionBoxCol; // Collider with the trigger
        private bool _mHolding; // Is an object in hand?
        private Vector3 _mRightOrigin; // Use for right hand raycast starting point
        private Vector3 _mLeftOrigin; // Use for left hand raycast starting point

        private Vector3 leftDirectionToObject;
        private Vector3 rightDirectionToObject;
        private IKController _ik;

        
        void Start()
        {
            // Get components
            _ik = GameManager.Instance.ikController;
            _mInteractionBoxCol = GetComponent<BoxCollider>();
        }
        
        void Update()
        {
            CheckInputs(); // Always check for inputs
            HoldingState();
        }
        
        #region Collision Handling

        // Scan an object when colliding with it ---------------------------------------
        private void OnTriggerEnter(Collider col)
        {
            CheckForNearbyPickableObject(col);
        }
        
        private void OnTriggerExit(Collider col)
        {
            ResetProximityObject(col);
        }

        private void CheckForNearbyPickableObject(Collider col)
        {
            if(col.IsType<InteractableObject>() && !_mHolding)
                m_proximityObject = col.gameObject;
        }

        private void ResetProximityObject(Collider col)
        {
            // col.gameObject.IsType<>()
            if(col.IsType<InteractableObject>() && !_mHolding)
                m_proximityObject = null;
        }

        #endregion

        #region Input Handling/Action States

        // Check for inputs to trigger an action ---------------------------------------
        private void CheckInputs()
        {
            DefaultActions();
            HoldingActions();
        }

        void HoldingActions()
        {
            // Check if the action button is triggered ----------
            if (Input.GetAxisRaw("Action") != 0 && m_proximityObject != null)
            {
                // If player is holding an object, trow it
                if (_mHolding)
                    Throw();
            }
            
            // If you can plant an object ----------------------
            if (Input.GetAxisRaw("Plant") != 0 && _mHolding)
                InstantPlant();
        }        
        
        void DefaultActions()
        {
            // Check if the action button is triggered ----------
            if (Input.GetAxisRaw("Action") != 0 && m_proximityObject != null)
            {
                if (!_mHolding)
                {
                    // Scan for the correct type of object
                    if (m_proximityObject.TryGetComponent(out InteractableObject interactable))
                    {
                        interactable.PickUp();
                        Hold();
                    }
                    else if (m_proximityObject.TryGetComponent(out Plant plant))
                    {
                        plant.Harvest(); //call interface method
                        Harvest();
                    }
                }
            }
        }
        
        #endregion

        #region Pick Up/ Holding/ Throwing

        private void HoldingState()
        {
            // While holding
            if (_mHolding)
            {
                // Pick up an object
                if (m_proximityObject.transform.position.y < transform.position.y)
                {
                    // Make the object move up to the current position
                    Vector3 currentPos = m_proximityObject.transform.position;
                    currentPos = Vector3.MoveTowards(currentPos, transform.position, 4f * Time.deltaTime);
                    m_proximityObject.transform.position = currentPos;
                }
                else
                {
                    // Keep the object stick on its original point and follow collisions
                    m_proximityObject.transform.position = transform.position;
                }
            }
            
        }
        
        private void Hold()
        {
            m_proximityObject.HoldObject(this.transform); //hold the object 
            _mInteractionBoxCol.SetColliderTrigger(false); // Bring back the trigger box as a collider
            SetHandTargets(); // Put hands on the object
            StartCoroutine(PickUp(0.3f, m_proximityObject)); // Start to pick up
        }
        
        // Trowing a dynamic object ----------------------------------------------------
        private void Throw()
        {
            ResetHandWeight();
            
            if (m_proximityObject.TryGetComponent(out InteractableObject interactable))
                interactable.Throw(transform.forward, m_throwForce);
            
            ResetInteraction();
        }

        
        // Wait before starting to hold and simulate pick up ---------------------------
        private IEnumerator PickUp(float delay, GameObject pickUpObject)
        {
            // Wait the delay before starting to hold
            yield return new WaitForSeconds(delay);

            // Make sure the player did not target another object
            m_proximityObject = pickUpObject;
            _mHolding = true;
        }
        
        
        // For harvesting potatoes when they are ready ---------------------------------
        private void Harvest()
        {
            // Add count to inventory
            var inventoryController = GameManager.Instance.inventoryController;
            inventoryController.InventoryCount(1);
            
            // Destroy the object
            Destroy(m_proximityObject);
        }

        // //physics plant
        // private void Plant()
        // {
        //     ResetHandWeight();
        //     m_proximityObject.layer = 0; // bring back the default physic layer
        //     m_proximityObject.ThrowObject(transform.forward, m_throwForce); //throws the object 
        //     
        //     //check if it can be planted
        //     if (m_proximityObject.TryGetComponent(out Plant plant))
        //     {
        //         plant.Planting = true;
        //         plant.PickedUp = false;
        //     }
        //     
        //     ResetInteraction();
        // }
        //
        //Instant plant
        private void InstantPlant()
        {
            if (m_proximityObject.TryGetComponent(out Plant plant))
            {
                ResetHandWeight();
                m_proximityObject.layer = 0;
    
                    var layerMask = LayerMask.GetMask("Ground");
                    if (Physics.Raycast(m_planterObject.transform.position, Vector3.down, out RaycastHit plantingPosition, 10.0f, layerMask))
                        plant.PlantObject(plantingPosition.point);
                
                ResetInteraction();
            }
        }

        private void ResetHandWeight()
        {
            _ik.ActivateWeight = false; //reset hand position
        }

        private void ResetInteraction()
        {
            // Get rid of the object
            m_proximityObject.transform.parent = null;
            m_proximityObject = null;

            // Clear hold
            _mHolding = false;

            // Set the trigger back
            _mInteractionBoxCol.SetColliderTrigger(true); //TODO this wont work with potatoes and rigidbody movement
        }
        #endregion

        #region Raycasting
        // Set targets in real time for the hands --------------------------------------
        /* Thought process here 
         * 1- get direction towards object
         * 2- ray cast there
         * 3- set parent to obj and weights to IKController
         *
         * NOTE: Need to call this function only once 
         */
        private void SetHandTargets()
        {
            var objectPos = m_proximityObject.transform.position;
            var layerMask = LayerMask.GetMask("InHand");
            _ik.ActivateWeight = true;

            // Capture current layer and change it
            m_proximityObject.layer = LayerMask.NameToLayer("InHand");

            // Set origins of the raycasts + offsets
            Vector3 objectPositionOffset = objectPos - transform.position;

            _mLeftOrigin = transform.TransformPoint((Vector3.left * m_raycastOffsetX) +
                                                    (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            _mRightOrigin = transform.TransformPoint((Vector3.right * m_raycastOffsetX) +
                                                     (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            
            // we will use the normalized direction towards the prox. obj instead of a fixed direction
            leftDirectionToObject = (objectPos - _mLeftOrigin).normalized;
            rightDirectionToObject = (objectPos - _mRightOrigin).normalized;
            
            // For left side ----------
            RaycastHit leftEdge;
            if (Physics.Raycast(_mLeftOrigin, leftDirectionToObject, out leftEdge, m_raycastOffsetX + 10.0f, layerMask))
            {
                _ik.LeftHandTarget.parent = m_proximityObject.transform;
                _ik.LeftHandTarget.position = leftEdge.point;
                _ik.LeftHandTarget.rotation = Quaternion.LookRotation(leftEdge.normal);
            }
            
            // For right side ---------
            RaycastHit rightEdge;
            if (Physics.Raycast(_mRightOrigin, rightDirectionToObject, out rightEdge, m_raycastOffsetX + 10.0f, layerMask))
            {
                _ik.RightHandTarget.parent = m_proximityObject.transform;
                _ik.RightHandTarget.position = rightEdge.point;
                _ik.RightHandTarget.rotation = Quaternion.LookRotation(rightEdge.normal);
            }
        }
        #endregion
        
        #region Gizmos
        // Draw gizmos on play ---------------------------------------------------------
        private void OnDrawGizmos()
        {
            //Codrin: POSITION OF THE HAND TARGETS 
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_mLeftOrigin, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_mRightOrigin, 0.1f);
            Gizmos.DrawSphere(m_planterObject.transform.position, 0.1f);

            Debug.DrawRay(_mLeftOrigin, leftDirectionToObject * 1.5f, Color.magenta);
            Debug.DrawRay(_mRightOrigin, rightDirectionToObject * 1.5f, Color.green);

        }
        #endregion
    }
}
