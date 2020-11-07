using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    [RequireComponent(typeof(IKController))]
    public class ActionController : MonoBehaviour
    {
        
        // public variables -------------------------
        [Title("Interactive States")]
        [SerializeField] protected bool m_holding; // Is an object in hand?
        [SerializeField] protected bool m_interactStationary; // Is an object in hand?
        [Header("Interactable Objects")]
        public GameObject m_pickedObject; // Target caught by a trigger
        public GameObject m_proximityObject; // Target caught by a trigger
        public GameObject m_proximityStationaryObject; // Target caught by a trigger
        public GameObject m_planterObject;
        [Header("Physics")]
        public float m_throwForce = 2.5f; // Force when an object is trow after holding
        public float m_raycastOffsetX = 2f; // Offset on the x axis for raycasts
        public float m_raycastOffsetZ = -0.2f; // Offset on the z axis for raycasts
        
        // private variables ------------------------
        private bool _isPlayer;  // Determine if the parent is the player or not
        private BoxCollider _interactionBoxCol; // Collider with the trigger
        private Vector3 _rightOrigin; // Use for right hand raycast starting point
        private Vector3 _leftOrigin; // Use for left hand raycast starting point

        private Vector3 _leftDirectionToObject;
        private Vector3 _rightDirectionToObject;
        private IKController _ik;

        #region Call Methods

        
        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        protected void Start()
        {
            // Get components
            _ik = GameManager.Instance.ikController;
            _interactionBoxCol = GetComponent<BoxCollider>();
            
            // Is this action controller linked to the player?
            _isPlayer = gameObject.transform.parent.CompareTag("Player");
        }
        
        
        // ------------------------------------------
        // Update is called every frame
        // ------------------------------------------
        protected void Update()
        {
            CheckInputs(); // Always check for inputs
            HoldingState();
        }
        
        #endregion
        
        #region Collision Handling
        
        
        // ------------------------------------------
        // Methods
        // ------------------------------------------
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
                m_interactStationary = true;
                m_proximityStationaryObject = col.gameObject;
            }
                
            if(col.IsType<InteractableObject>() && !m_holding && col.gameObject != m_pickedObject)
                m_proximityObject = col.gameObject; 
                           
        }

        private void ResetProximityObject(Collider col)
        {
            if (col.IsType<InteractableStationary>())
            {
                m_interactStationary = false;
                m_proximityStationaryObject = null;
            }
            if(col.IsType<InteractableObject>() && !m_holding && col.gameObject != m_pickedObject)
                m_proximityObject = null;            
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
            if (m_holding && m_pickedObject)
            {
                //NOTE while holding the collider for pickup cant be triggered
                if (Input.GetMouseButtonDown(0) && m_interactStationary)
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
            if (!m_holding && !m_pickedObject)
            {
                
                // Check if the action button is triggered ----------
                // if (Input.GetAxisRaw("Action") != 0 && _proximityObject != null)
                // {
                //     // Scan for the correct type of object
                //     if (_proximityObject.TryGetComponent(out InteractableObject interactable))
                //     {
                //         interactable.PickUp();
                //         Hold();
                //     }
                //     else if (_proximityObject.TryGetComponent(out Plant plant))
                //     {
                //         plant.Harvest(); //call interface method
                //         Harvest();
                //     }
                // }                
                // Check if the action button is triggered ----------
                if (Input.GetMouseButtonDown(0) && m_proximityObject)
                {
                    // Scan for the correct type of object
                    if (m_proximityObject.TryGetComponent(out InteractableObject interactable))
                    {
                        interactable.PickUp();
                        Hold();
                    }
                }

                if (Input.GetMouseButtonDown(2) && m_proximityObject)
                {
                    if (m_proximityObject.TryGetComponent(out Plant plant))
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
            if (m_holding)
            {
                // Pick up an object
                if (m_pickedObject.transform.position.y < transform.position.y)
                {
                    // Make the object move up to the current position
                    Vector3 currentPos = m_pickedObject.transform.position;
                    currentPos = Vector3.MoveTowards(currentPos, transform.position, 4f * Time.deltaTime);
                    m_pickedObject.transform.position = currentPos;
                }
                else
                {
                    // Keep the object stick on its original point and follow collisions
                    m_pickedObject.transform.position = transform.position;
                }
            }
            
        }

        protected void InsertIntoMachine()
        {
            if (m_proximityStationaryObject.TryGetComponent(out MachineBase machine))
            {
                ResetHandWeight();
                // machine.InsertPlant(_pickedObject.GetComponent<Plant>());
                Debug.Log("inserted");
                ResetInteraction();
            }
        }
        
        protected void Hold()
        {
            Debug.Log("Hold");
            m_pickedObject = m_proximityObject;
            m_proximityObject = null;
            
            m_pickedObject.HoldObject(this.transform); //hold the object 

            SetHandTargets(); // Put hands on the object
            StartCoroutine(PickUp(0.3f, m_pickedObject)); // Start to pick up
        }
        
        // Trowing a dynamic object ----------------------------------------------------
        protected void Throw()
        {
            ResetHandWeight();
            
            if (m_pickedObject.TryGetComponent(out InteractableObject interactable))
                interactable.Throw(transform.forward, m_throwForce);
            
            ResetInteraction();
        }

        
        // Wait before starting to hold and simulate pick up ---------------------------
        protected IEnumerator PickUp(float delay, GameObject pickUpObject)
        {
            // Wait the delay before starting to hold
            yield return new WaitForSeconds(delay);
            // Make sure the player did not target another object
            m_holding = true;
            // m_proximityObject = pickUpObject;
            m_pickedObject = pickUpObject;

        }

        // For harvesting potatoes when they are ready ---------------------------------
        protected void Harvest()
        {
            //NOTE: Inventory commented out since its not being used now 
            // Add count to inventory
            // var inventoryController = GameManager.Instance.inventoryController;
            // inventoryController.InventoryCount(1);

            if (m_proximityObject.TryGetComponent(out Plant plant))
            {
                for (int i = 0; i < plant.GrowthSettings.harvestYield; i++)
                {
                    ParticleController.Instance.EmitAt(m_proximityObject.transform.position);
                    GameObject seed = Instantiate(m_proximityObject, m_proximityObject.transform.position + Vector3.up, Quaternion.identity) as GameObject;
                    seed.GetComponent<Plant>().HarvestInit();
                }
            }
            
            // Destroy the object
            Destroy(m_proximityObject);
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
            if (m_pickedObject.TryGetComponent(out Plant plant))
            {
                m_pickedObject.layer = 0;
    
                    var layerMask = LayerMask.GetMask("Ground");
                    if (Physics.Raycast(m_planterObject.transform.position, Vector3.down,
                        out RaycastHit plantingPosition, 10.0f, layerMask))
                    {
                        if (CanPlant(plantingPosition.point, plant.GrowthSettings.growthRadius))
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
            m_pickedObject.transform.parent = null;
            m_pickedObject = null;

            // Clear hold
            m_holding = false;
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
            var objectPos = m_pickedObject.transform.position;
            var layerMask = LayerMask.GetMask("InHand");
            _ik.ActivateWeight = true;

            // Capture current layer and change it
            m_pickedObject.layer = LayerMask.NameToLayer("InHand");

            // Set origins of the raycasts + offsets
            Vector3 objectPositionOffset = objectPos - transform.position;

            _leftOrigin = transform.TransformPoint((Vector3.left * m_raycastOffsetX) +
                                                    (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            _rightOrigin = transform.TransformPoint((Vector3.right * m_raycastOffsetX) +
                                                     (Vector3.forward * m_raycastOffsetZ) + objectPositionOffset);
            
            // we will use the normalized direction towards the prox. obj instead of a fixed direction
            _leftDirectionToObject = (objectPos - _leftOrigin).normalized;
            _rightDirectionToObject = (objectPos - _rightOrigin).normalized;
            
            // For left side ----------
            //todo need to reset parent transform when its not holding 
            RaycastHit leftEdge;
            if (Physics.Raycast(_leftOrigin, _leftDirectionToObject, out leftEdge, m_raycastOffsetX + 10.0f, layerMask))
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
            if (Physics.Raycast(_rightOrigin, _rightDirectionToObject, out rightEdge, m_raycastOffsetX + 10.0f, layerMask))
            {
                if (m_pickedObject)
                {
                    _ik.RightHandTarget.parent = m_pickedObject.transform;
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
            Gizmos.DrawSphere(m_planterObject.transform.position, 0.1f);

            Debug.DrawRay(_leftOrigin, _leftDirectionToObject * 1.5f, Color.magenta);
            Debug.DrawRay(_rightOrigin, _rightDirectionToObject * 1.5f, Color.green);

        }
        #endregion
    }
}
