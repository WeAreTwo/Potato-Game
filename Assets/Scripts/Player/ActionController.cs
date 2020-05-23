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
        public bool m_canInteract = true; // Allow interaction with objects
        public GameObject m_proximityObject; // Target caught by a trigger
        public float _m_throwForce = 2.5f; // Force when an object is trow after holding
        public float m_raycastOffsetX = 2f; // Offset on the x axis for raycasts
        public float m_raycastOffsetZ = -0.2f; // Offset on the z axis for raycasts

        // private variables ------------------------
        private BoxCollider _mInteractionBoxCol; // Collider with the trigger
        private bool _mHolding; // Is an object in hand?
        private bool _mReadyToTrow; // If an object is ready to be dropped
        private bool _mReadyToPlant; // If the holding object is a potato, ready to plant
        private Vector3 _mRightOrigin; // Use for right hand raycast starting point
        private Vector3 _mLeftOrigin; // Use for left hand raycast starting point
        private int _mOriginalLayer; // Original physic layer applied to the proximity object
        
        protected Vector3 leftDirectionToObject;
        protected Vector3 rightDirectionToObject;

        [SerializeField] protected IKController _ik;

        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get components
            _ik = GameManager.Instance.playerController.gameObject.GetComponent<IKController>();
            _mInteractionBoxCol = GetComponent<BoxCollider>();
        }


        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
        {
            // Always check for inputs
            CheckInputs();

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


        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Check for inputs to trigger an action ---------------------------------------
        private void CheckInputs()
        {
            // Check if the action button is triggered ----------
            if (Input.GetAxisRaw("Action") != 0 && m_proximityObject != null)
            {
                if (m_canInteract)
                {
                    // Disable interaction 
                    m_canInteract = !m_canInteract;

                    // Scan for the correct type of object
                    if (m_proximityObject.IsType<IPickUp>())
                        Hold();
                    else if (m_proximityObject.IsType<IHarvestable>())
                        Harvest();
                        
                }

                // If player is holding an object, trow it
                if (_mHolding && _mReadyToTrow)
                    Throw(false);
            }

            // When the action input is not triggered ----------
            if (Input.GetAxisRaw("Action") == 0 && m_proximityObject != null)
            {
                // When holding an object, next ready a trow
                if (_mHolding)
                    _mReadyToTrow = true;
            }

            // If you can plant an object ----------------------
            if (Input.GetAxisRaw("Plant") != 0 && _mReadyToPlant)
            {
                Throw(true); // Plant a potato
            }

        }

        // Sccan an object when colliding with it --------------------------------------
        private void OnTriggerEnter(Collider col)
        {
            CheckForNearbyPickableObject(col);
        }
        
        private void OnTriggerExit(Collider col)
        {
            ResetProximityObject(col);
        }

        void CheckForNearbyPickableObject(Collider col)
        {
            if(col.GetComponent<IPickUp>() != null)
                m_proximityObject = col.gameObject;
        }

        void ResetProximityObject(Collider col)
        {
            // col.gameObject.IsType<>()
            if(col.GetComponent<IPickUp>() != null && !_mHolding)
                m_proximityObject = null;
        }


        // When holding a dynamic object -----------------------------------------------
        private void Hold()
        {
            /* PROCESS
             * 1 - SET PARENT
             * 2 - DISTABLED GRAVITY AND FREEZE ROTATION
             * 3 - SET HANDS
             * 4 - DISABLE COLLIDER
             * 5 - ENABLED INTERACTION COLLIDER
             * 6-  START PICK ANIM
             * 7 - IF POTATO, ITS READY TO PLANT 
             */
            
            
            m_proximityObject.HoldObject(this.transform); //hold the object 
            m_proximityObject.SetAllColliderTriggers(true);
            _mInteractionBoxCol.SetColliderTrigger(false); // Bring back the trigger box as a collider
            SetHandTargets(); // Put hands on the object
            StartCoroutine(PickUp(0.3f, m_proximityObject)); // Start to pick up


            // Check if it's a plantable obj
            if (m_proximityObject.IsType<IPlantable>())
                _mReadyToPlant = true;
            
        }
        
        void ResetHandWeight()
        {
            _ik.ActivateWeight = false; //reset hand position
            m_proximityObject.layer = _mOriginalLayer;
        }

        // Trowing a dynamic object ----------------------------------------------------
        private void Throw(bool plant)
        {
            ResetHandWeight();
            m_proximityObject.ThrowObject(transform.forward, _m_throwForce); //throws the object 
            m_proximityObject.SetAllColliderTriggers(false);
            
            if(plant) Plant();

            // Get rid of the object
            m_proximityObject.transform.parent = null;
            m_proximityObject = null;

            // Enable interaction
            m_canInteract = true;
            _mHolding = false;
            _mReadyToTrow = false;
            _mReadyToPlant = false;

            // Set the trigger back
            _mInteractionBoxCol.SetColliderTrigger(true);
        }

        void Plant()
        {
            //check if it can be planted
            if (m_proximityObject.IsType<IPlantable>())
                m_proximityObject.GetComponent<IPlantable>().Planting = true;
            
        }


        // Set targets in real time for the hands --------------------------------------
        private void SetHandTargets()
        {

            int layerMask = LayerMask.GetMask("InHand");
            _ik.ActivateWeight = true;

            // Capture current layer and change it
            _mOriginalLayer = m_proximityObject.layer;
            m_proximityObject.layer = LayerMask.NameToLayer("InHand");

            // Set origins of the raycasts + offsets
            Vector3 objectPositionOffset = m_proximityObject.transform.position - transform.position;

            _mLeftOrigin = transform.TransformPoint((Vector3.left * m_raycastOffsetX) +
                                                    (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            _mRightOrigin = transform.TransformPoint((Vector3.right * m_raycastOffsetX) +
                                                     (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);

            //TODO add check to make sure that both hands rays land on the same obj and not on separate ones 
            
            /* Thought process here 
             * 1- get direction towards object
             * 2- ray cast there
             * 3- set parent to obj and weights to IKController
             *
             * NOTE: Need to call this function only once 
             */
            
            //we will use the normalized direction towards the prox. obj instead of a fixed direction
            leftDirectionToObject = (m_proximityObject.transform.position - _mLeftOrigin).normalized;
            rightDirectionToObject = (m_proximityObject.transform.position - _mRightOrigin).normalized;
            
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

            m_canInteract = true;
        }


        // Draw gizmos on play ---------------------------------------------------------
        private void OnDrawGizmos()
        {
            //Codrin: POSITION OF THE HAND TARGETS 
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_mLeftOrigin, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_mRightOrigin, 0.1f);

            Debug.DrawRay(_mLeftOrigin, leftDirectionToObject * 1.5f, Color.magenta);
            Debug.DrawRay(_mRightOrigin, rightDirectionToObject * 1.5f, Color.green);

        }
    }
}
