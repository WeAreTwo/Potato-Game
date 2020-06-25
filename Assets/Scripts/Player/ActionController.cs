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
        [Header("INTERACTIVE STATES")]
        [SerializeField] protected bool _holding; // Is an object in hand?
        [SerializeField] protected bool _interactStationary; // Is an object in hand?
        
        [Header("INTERACTABLE OBJECTS")]
        // public variables -------------------------
        public GameObject _pickedObject; // Target caught by a trigger
        public GameObject _proximityObject; // Target caught by a trigger
        public GameObject _proximityStationaryObject; // Target caught by a trigger
        public GameObject _planterObject;
        
        [Header("THROWING/RAYCASTING")]
        public float _throwForce = 2.5f; // Force when an object is trow after holding
        public float _raycastOffsetX = 2f; // Offset on the x axis for raycasts
        public float _raycastOffsetZ = -0.2f; // Offset on the z axis for raycasts

        
        // private variables ------------------------
        protected BoxCollider _interactionBoxCol; // Collider with the trigger
        protected Vector3 _rightOrigin; // Use for right hand raycast starting point
        protected Vector3 _leftOrigin; // Use for left hand raycast starting point

        protected Vector3 _leftDirectionToObject;
        protected Vector3 _rightDirectionToObject;
        protected IKController _ik;

        #region Call Methods

        protected void Start()
        {
            // Get components
            _ik = GameManager.Instance.ikController;
            _interactionBoxCol = GetComponent<BoxCollider>();
        }
        
        protected void Update()
        {
            CheckInputs(); // Always check for inputs
            HoldingState();
        }
        
        #endregion
        
        #region Collision Handling

        // Scan an object when colliding with it ---------------------------------------
        private void OnTriggerEnter(Collider col)
        {
            // if(!_mHolding)
                CheckForNearbyPickableObject(col);
        }
        
        private void OnTriggerExit(Collider col)
        {
            // if(!_mHolding)
                ResetProximityObject(col);
        }

        private void CheckForNearbyPickableObject(Collider col)
        {
            if (col.IsType<InteractableStationary>())
            {
                Debug.Log(col.gameObject.name + "is near");
                _interactStationary = true;
                _proximityStationaryObject = col.gameObject;
            }
                
            if(col.IsType<InteractableObject>() && !_holding && col.gameObject != _pickedObject)
                _proximityObject = col.gameObject; 
                           
        }

        private void ResetProximityObject(Collider col)
        {
            if (col.IsType<InteractableStationary>())
            {
                _interactStationary = false;
                _proximityStationaryObject = null;
            }
            if(col.IsType<InteractableObject>() && !_holding && col.gameObject != _pickedObject)
                _proximityObject = null;            
        }

        #endregion

        #region Input Handling/Action States

        // Check for inputs to trigger an action ---------------------------------------
        protected void CheckInputs()
        {
            DefaultActions();
            HoldingActions();
        }

        protected void HoldingActions()
        {
            if (_holding && _pickedObject)
            {
                //NOTE while holding the collider for pickup cant be triggered
                if (Input.GetMouseButtonDown(0) && _interactStationary)
                {
                    InsertIntoMachine();
                }
                // Check if the action button is triggered ----------
                else if (Input.GetAxisRaw("Action") != 0)
                {
                    Throw();
                }

                // If you can plant an object ----------------------
                if (Input.GetAxisRaw("Plant") != 0)
                    InstantPlant();
            }
        }        
        
        protected void DefaultActions()
        {
            if (!_holding && !_pickedObject)
            {
                // Check if the action button is triggered ----------
                if (Input.GetAxisRaw("Action") != 0 && _proximityObject != null)
                {
                    // Scan for the correct type of object
                    if (_proximityObject.TryGetComponent(out InteractableObject interactable))
                    {
                        interactable.PickUp();
                        Hold();
                    }
                    else if (_proximityObject.TryGetComponent(out Plant plant))
                    {
                        plant.Harvest(); //call interface method
                        Harvest();
                    }
                }
            }
        }
        
        #endregion

        #region Pick Up/ Holding/ Throwing

        protected void HoldingState()
        {
            // While holding
            if (_holding)
            {
                // Pick up an object
                if (_pickedObject.transform.position.y < transform.position.y)
                {
                    // Make the object move up to the current position
                    Vector3 currentPos = _pickedObject.transform.position;
                    currentPos = Vector3.MoveTowards(currentPos, transform.position, 4f * Time.deltaTime);
                    _pickedObject.transform.position = currentPos;
                }
                else
                {
                    // Keep the object stick on its original point and follow collisions
                    _pickedObject.transform.position = transform.position;
                }
            }
            
        }

        protected void InsertIntoMachine()
        {
            if (_proximityStationaryObject.TryGetComponent(out MachineBase machine))
            {
                ResetHandWeight();
                machine.InsertPlant(_pickedObject.GetComponent<Plant>());
                Debug.Log("inserted");
                ResetInteraction();
            }
        }
        
        protected void Hold()
        {
            Debug.Log("Hold");
            _pickedObject = _proximityObject;
            _proximityObject = null;
            
            _pickedObject.HoldObject(this.transform); //hold the object 

            SetHandTargets(); // Put hands on the object
            StartCoroutine(PickUp(0.3f, _pickedObject)); // Start to pick up
        }
        
        // Trowing a dynamic object ----------------------------------------------------
        protected void Throw()
        {
            ResetHandWeight();
            
            if (_pickedObject.TryGetComponent(out InteractableObject interactable))
                interactable.Throw(transform.forward, _throwForce);
            
            ResetInteraction();
        }

        
        // Wait before starting to hold and simulate pick up ---------------------------
        protected IEnumerator PickUp(float delay, GameObject pickUpObject)
        {
            // Wait the delay before starting to hold
            yield return new WaitForSeconds(delay);
            // Make sure the player did not target another object
            _holding = true;
            // m_proximityObject = pickUpObject;
            _pickedObject = pickUpObject;

        }

        // For harvesting potatoes when they are ready ---------------------------------
        protected void Harvest()
        {
            // Add count to inventory
            var inventoryController = GameManager.Instance.inventoryController;
            inventoryController.InventoryCount(1);
            
            // Destroy the object
            Destroy(_proximityObject);
        }

        //check planting area
        protected bool CanPlant(Vector3 center, float radius)
        {

            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            foreach (var col in hitColliders)
            {
                if (col.IsType<Plant>())
                    return false;
            }

            return true;
        }
        
        
        //Instant plant
        protected void InstantPlant()
        {
            if (_pickedObject.TryGetComponent(out Plant plant))
            {
                _pickedObject.layer = 0;
    
                    var layerMask = LayerMask.GetMask("Ground");
                    if (Physics.Raycast(_planterObject.transform.position, Vector3.down,
                        out RaycastHit plantingPosition, 10.0f, layerMask))
                    {
                        if (CanPlant(plantingPosition.point, plant.GrowthCharacteristics.growthRadius))
                        {
                            plant.PlantObject(plantingPosition.point);
                            ParticleController.Instance.EmitAt(plantingPosition.point);
                            ResetHandWeight();
                            ResetInteraction();
                        }
                        
                    }
                
            }
        }

        protected void ResetHandWeight()
        {
            _ik.ActivateWeight = false; //reset hand position
            _ik.m_leftHandTarget.parent = this.transform;
            _ik.m_rightHandTarget.parent = this.transform;

        }

        protected void ResetInteraction()
        {
            // Get rid of the object
            _pickedObject.transform.parent = null;
            _pickedObject = null;

            // Clear hold
            _holding = false;
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
        protected void SetHandTargets()
        {
            var objectPos = _pickedObject.transform.position;
            var layerMask = LayerMask.GetMask("InHand");
            _ik.ActivateWeight = true;

            // Capture current layer and change it
            _pickedObject.layer = LayerMask.NameToLayer("InHand");

            // Set origins of the raycasts + offsets
            Vector3 objectPositionOffset = objectPos - transform.position;

            _leftOrigin = transform.TransformPoint((Vector3.left * _raycastOffsetX) +
                                                    (Vector3.forward * _raycastOffsetZ) + objectPositionOffset);
            _rightOrigin = transform.TransformPoint((Vector3.right * _raycastOffsetX) +
                                                     (Vector3.forward * _raycastOffsetZ) + objectPositionOffset);
            
            // we will use the normalized direction towards the prox. obj instead of a fixed direction
            _leftDirectionToObject = (objectPos - _leftOrigin).normalized;
            _rightDirectionToObject = (objectPos - _rightOrigin).normalized;
            
            // For left side ----------
            //todo need to reset parent transform when its not holding 
            RaycastHit leftEdge;
            if (Physics.Raycast(_leftOrigin, _leftDirectionToObject, out leftEdge, _raycastOffsetX + 10.0f, layerMask))
            {
                if (_pickedObject)
                {
                    _ik.LeftHandTarget.parent = _pickedObject.transform;
                    _ik.LeftHandTarget.position = leftEdge.point;
                    _ik.LeftHandTarget.rotation = Quaternion.LookRotation(leftEdge.normal);
                }
            }
            
            // For right side ---------
            RaycastHit rightEdge;
            if (Physics.Raycast(_rightOrigin, _rightDirectionToObject, out rightEdge, _raycastOffsetX + 10.0f, layerMask))
            {
                if (_pickedObject)
                {
                    _ik.RightHandTarget.parent = _pickedObject.transform;
                    _ik.RightHandTarget.position = rightEdge.point;
                    _ik.RightHandTarget.rotation = Quaternion.LookRotation(rightEdge.normal);
                }
            }
        }
        #endregion
        
        #region Gizmos
        // Draw gizmos on play ---------------------------------------------------------
        protected void OnDrawGizmos()
        {
            //Codrin: POSITION OF THE HAND TARGETS 
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_leftOrigin, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_rightOrigin, 0.1f);
            Gizmos.DrawSphere(_planterObject.transform.position, 0.1f);

            Debug.DrawRay(_leftOrigin, _leftDirectionToObject * 1.5f, Color.magenta);
            Debug.DrawRay(_rightOrigin, _rightDirectionToObject * 1.5f, Color.green);

        }
        #endregion
    }
}
