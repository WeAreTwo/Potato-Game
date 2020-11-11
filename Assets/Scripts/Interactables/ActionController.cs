using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
        public List<PlantPointController> m_proximityPoints;  // Holds all near by possible planting points
        public GameObject m_closestPoint;  // The closest available planting point

        [Title("Physics")] 
        public float m_trowForce = 2.5f;  // Force applied to a trowed object 
        public float m_raycastOffsetX = 2f;  // Offset on the x axis for raycasts
        public float m_raycastOffsetZ = -0.2f;  // Offset on the z axis for raycasts
        [Space(20)] 
        public bool m_drawGizmos;  // Draw gizmos related to all action controllers
        
        
        // private variables ------------------------
        private BoxCollider _pickUpBox;  // Instance of the pickup box
        private Transform _planter;  // By where the entity will send a seed
        private Rigidbody _pickedRB;  // RB of the picked object
        private Collider _pickedCollider;  // Colliders of the picked object
        private float _closestPointDistance;  // The distance between closest point and planter
        private Vector3 _rightCastOrigin = Vector3.zero;  // Used for right hand raycast starting point
        private Vector3 _leftCastOrigin = Vector3.zero;  // Used for left hand raycast starting point
        private Vector3 _leftDirectionToObject;  // Contextual direction from the left object
        private Vector3 _rightDirectionToObject;  // Contextual direction from the right object
        private IKController _ik;  // Instance of the ik controller
        

        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {
            // Get Components
            _planter = gameObject.transform.GetChild(0);
            _ik = GameManager.Instance.ikController;
            _pickUpBox = GetComponent<BoxCollider>();
        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
        {
            CheckNearestPlantingPoint();
            
            // Hold a picked up object
            if (m_holding)
                Hold();
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
            if(col.CompareTag("DynamicObject") && !m_holding && col.gameObject != m_pickedObject)
                m_proximityObject = col.gameObject;
        }

        private void ResetProximityObject(Collider col)
        {
            if(col.CompareTag("DynamicObject") && !m_holding && col.gameObject != m_pickedObject)
                m_proximityObject = null;            
        }
        
        #endregion
        
        #region Actions
        
        // Pick up an object -----------------------------------------------------------
        public void PickUp()
        {
            if (!m_proximityObject)
                return;
            
            // Get the proximity object as the picked object
            m_pickedObject = m_proximityObject;
            m_proximityObject = null;
            
            // Make the new picked object as a child
            m_pickedObject.transform.SetParent(transform); 
            
            // Capture picked object physics data and deactivate them
            SetPickedObject();
            PhysicsSwitch(false, _pickedRB, _pickedCollider);
            
            // Put hands on the object & start pick up process
            SetHandTargets();
            StartCoroutine(PickUpProcess(0.3f, m_pickedObject));
        }


        // Trow a hold object ----------------------------------------------------------
        public void Trow()
        {
            // Reset the ik
            ResetHandWeight();

            // Bring the picked object on the default layer
            m_pickedObject.layer = 0;
            
            // Trow the object and reactivate object's physics
            var direction = transform.forward;
            
            PhysicsSwitch(true, _pickedRB, _pickedCollider);
            _pickedRB.velocity = direction * m_trowForce;
            
            // Ready for next action
            ResetInteraction();
        }
        
        
        // Plant an object in a point --------------------------------------------------
        public void Plant()
        {
            if (!m_closestPoint)
                return;
            
            var plantPos = m_closestPoint.transform.position;

            // Bring the picked object on the default layer 
            m_pickedObject.layer = 0;
            PhysicsSwitch(false, _pickedRB, _pickedCollider);
            
            // Plant the object in ground with small animation
            ParticleController.Instance.EmitAt(plantPos);
            m_pickedObject.transform.position = plantPos;
                    
            ResetHandWeight();
            ResetInteraction();
        }
        
        
        // Wait before starting to hold and simulate pick up ---------------------------
        private IEnumerator PickUpProcess(float delay, GameObject pickUpObject)
        {
            // Wait the delay before starting to hold
            yield return new WaitForSeconds(delay);
            // Make sure the player did not target another object
            m_holding = true;
            // m_proximityObject = pickUpObject;
            m_pickedObject = pickUpObject;
        }
        
        
        // Hold a picked up object -----------------------------------------------------
        private void Hold()
        {
            // Hold an object
            if (m_pickedObject.transform.position.y < transform.position.y)
            {
                // Make the object move up to the current position
                var currentPos = m_pickedObject.transform.position;
                currentPos = Vector3.MoveTowards(currentPos, transform.position, 4f * Time.deltaTime);
                m_pickedObject.transform.position = currentPos;
            }
            else
            {
                // Keep the object stick on its original point and follow collisions
                m_pickedObject.transform.position = transform.position;
            }
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

        #region Planting Points Availability

        // Check nearest planting points from the list -----------------------------
        private void CheckNearestPlantingPoint()
        {
            // Make sure the list is not empty
            if (m_proximityPoints == null)
                return;
            
            // Calculate the closest available point
            foreach (var point in m_proximityPoints)
            {
                if (point.m_occupied) 
                    continue;
                
                // Capture the distance between the planter and a valid point
                var distance = Vector3.Distance(_planter.transform.position, point.transform.position);
                    
                // Compare with other point distance to get the smallest amount
                if (!(distance <= _closestPointDistance) && _closestPointDistance != 0.0f) 
                    continue;
                
                // The best candidate is captured
                _closestPointDistance = distance;
                m_closestPoint = point.gameObject;
            }
            
            // Reset the closest point distance for next roll
            _closestPointDistance = 0.0f;
        }

        #endregion

        # region Physics

        // Activate or deactivate physics on a rb --------------------------------------
        private void PhysicsSwitch(bool state, Rigidbody rb, Collider col)
        {
            // Change gravity and transform collider behaviour 
            rb.useGravity = state;
            col.isTrigger = !state;
            _pickUpBox.isTrigger = state;

            // Change the constraints
            rb.constraints = state ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
        }
        
        
        // Activate or deactivate physics on a rb --------------------------------------
        // Get picked object physics components ----------------------------------------
        private void SetPickedObject()
        {
            if (!m_pickedObject)
                return;
            
            // Get the rigidbody and colliders
            _pickedRB = m_pickedObject.GetComponent<Rigidbody>();
            _pickedCollider = m_pickedObject.GetComponent<Collider>();
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


        
        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!m_drawGizmos)
                return;
            
            foreach (var point in m_proximityPoints)
            {
                Gizmos.color = point.m_occupied ? Color.red : point.m_pointColor;
                Gizmos.DrawSphere(point.transform.position, point.m_gizmoRadius);
            }

            if (m_closestPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_closestPoint.transform.position, 0.3f);
            }
        }

        #endregion
    }
}
