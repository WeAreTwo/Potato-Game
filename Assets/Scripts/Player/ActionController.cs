using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class ActionController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_canInteract = true;               // Allow interaction with objects
    public GameObject m_proximityObject;            // Target caught by a trigger
    public float m_trowForce = 2.5f;                // Force when an object is trow after holding
    public float m_raycastOffsetX = 2f;             // Offset on the x axis for raycasts
    public float m_raycastOffsetZ = -0.2f;          // Offset on the z axis for raycasts

    // private variables ------------------------
    private BoxCollider _mBoxCol;                   // Collider with the trigger
    private bool _mHolding;                         // Is an object in hand?
    private bool _mReadyToTrow;                     // If an object is ready to be dropped
    private bool _mReadyToPlant;                    // If the holding object is a potato, ready to plant
    private Vector3 _mRightOrigin;                  // Use for right hand raycast starting point
    private Vector3 _mLeftOrigin;                   // Use for left hand raycast starting point
    private int _mOriginalLayer;                    // Original physic layer applied to the proximity object


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        _mBoxCol = GetComponent<BoxCollider>();
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
                if (m_proximityObject.tag == ProjectTags.DynamicObject)
                    Hold();

                if (m_proximityObject.tag == ProjectTags.Potato)
                {
                    // Make sure the potato is not already planted
                    if (m_proximityObject.GetComponent<PlantingController>() != null &&
                        !m_proximityObject.GetComponent<PlantingController>().m_planted)
                        Hold();                    
                    
                    //Codrin Code for his potatoes
                    if (m_proximityObject.GetComponent<Plant>() != null &&
                        !m_proximityObject.GetComponent<Plant>().Planted)
                        Hold();
                }
            }

            // If player is holding an object, trow it
            if (_mHolding && _mReadyToTrow)
                Trow(false);
        }

        // When the action input is not triggered ----------
        if (Input.GetAxisRaw("Action") == 0 && m_proximityObject != null)
        {
            // When holding an object, next ready a trow
            if (_mHolding)
                _mReadyToTrow = true;
        }

        // If you can plant an object ----------------------
        if (Input.GetAxisRaw("Plant") !=0 && _mReadyToPlant)
        {
            // Plant a potato
            Trow(true);
        }
         
    }


    // Sccan an object when colliding with it --------------------------------------
    private void OnTriggerEnter(Collider col)
    {
        // Check if the object can be grabbed
        if (col.gameObject.tag == ProjectTags.DynamicObject ||
            col.gameObject.tag == ProjectTags.Potato)
        { 
            m_proximityObject = col.gameObject;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // Empty out the proximity object
        if (col.gameObject.tag == ProjectTags.DynamicObject && !_mHolding ||
            col.gameObject.tag == ProjectTags.Potato && !_mHolding)
        {
            m_proximityObject = null;
        }
    }


    // When holding a dynamic object -----------------------------------------------
    private void Hold()
    {
        // Set the object as a child
        m_proximityObject.transform.SetParent(transform);

        // Get its rigid body and cancel its gravity and freeze rotation
        Rigidbody objectRb = m_proximityObject.GetComponent<Rigidbody>();
        objectRb.useGravity = false;
        objectRb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Put hands on the object
        SetHandTargets();

        // Disable object's collider
        foreach (Collider objectCollider in m_proximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = true;

        // Bring back the trigger box as a collider
        _mBoxCol.isTrigger = false;

        // Start to pick up
        StartCoroutine(PickUp(0.3f, m_proximityObject));

        // Check if it's a potato (activate the plant action)
        if (m_proximityObject.tag == ProjectTags.Potato)
            _mReadyToPlant = true;
    }


    // Trowing a dynamic object ----------------------------------------------------
    private void Trow(bool plant)
    {
        // Reset object rigid body's property and unfreeze rotation
        Rigidbody objectRb = m_proximityObject.GetComponent<Rigidbody>();
        objectRb.useGravity = true;
        objectRb.constraints = RigidbodyConstraints.None;
        
        // Reset hand positions
        HandTargetPosition handTargets = m_proximityObject.GetComponent<HandTargetPosition>();
        handTargets.m_activateWeight = false;
        m_proximityObject.layer = _mOriginalLayer;
        
        // Enable object's collider
        foreach (Collider objectCollider in m_proximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = false;

        // Apply a velocity to the object
        objectRb.velocity = transform.forward * m_trowForce;

        // Check if the object will be planted
        // Get the planting mechanic from the object activated
        if (m_proximityObject.GetComponent<PlantingController>() != null && plant)
            m_proximityObject.GetComponent<PlantingController>().m_planting = true;        
        
        
        //Codrin Code for his potatoes
        if (m_proximityObject.GetComponent<Plant>() != null && plant)
            m_proximityObject.GetComponent<Plant>().Planting = true;


        // Get rid of the object
        m_proximityObject.transform.parent = null;
        m_proximityObject = null;

        // Enable interaction
        m_canInteract = true;
        _mHolding = false;
        _mReadyToTrow = false;
        _mReadyToPlant = false;

        // Set the trigger back
        _mBoxCol.isTrigger = true;
    }
    
    
    // Set targets in real time for the hands --------------------------------------
    private void SetHandTargets()
    {
        RaycastHit leftEdge;
        RaycastHit rightEdge;

        int layerMask = LayerMask.GetMask("InHand");
        
        HandTargetPosition handTargets = m_proximityObject.GetComponent<HandTargetPosition>();
        handTargets.m_activateWeight = true;
        
        // Capture current layer and change it
        _mOriginalLayer = m_proximityObject.layer;
        m_proximityObject.layer = LayerMask.NameToLayer("InHand");

        // Set origins of the raycasts + offsets
        Vector3 objectRelativePosition = m_proximityObject.transform.position - transform.position;
        objectRelativePosition.Normalize();
        
        _mRightOrigin = transform.TransformPoint((Vector3.right * m_raycastOffsetX) + (Vector3.forward * m_raycastOffsetZ) + objectRelativePosition);
        _mLeftOrigin = transform.TransformPoint((Vector3.left * m_raycastOffsetX) + (Vector3.forward * m_raycastOffsetZ) + objectRelativePosition);

        // For right side ---------
        if (Physics.Raycast(_mRightOrigin, transform.TransformDirection(Vector3.left), out rightEdge, m_raycastOffsetX, layerMask))
        {
            // Put the right hand at the edge hit
            handTargets.m_rightHandTarget.position = rightEdge.point;
            handTargets.m_rightHandTarget.rotation = Quaternion.LookRotation(rightEdge.normal);
            Debug.Log("Hit Right");
        }
        else
            Debug.Log("Nope Right");
        

        // For left side ----------
        if (Physics.Raycast(_mLeftOrigin, transform.TransformDirection(Vector3.right), out leftEdge, m_raycastOffsetX, layerMask))
        {
            // Put the left hand at the edge hit
            handTargets.m_leftHandTarget.position = leftEdge.point; 
            handTargets.m_leftHandTarget.rotation = Quaternion.LookRotation(leftEdge.normal);
            Debug.Log("Hit Left");
        }
        else
            Debug.Log("Nope Left");
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

    
    // Draw gizmos on play ---------------------------------------------------------
    private void OnDrawGizmos()
    {
        //Codrin: POSITION OF THE HAND TARGETS 
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_mLeftOrigin, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_mRightOrigin, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_mLeftOrigin, _mRightOrigin);
    }
}
