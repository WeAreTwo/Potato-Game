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
    public GameObject m_actionIcon;                 // Action icon to be displayed when target's valid
    public float m_trowForce = 2.5f;                // Force when an object is trow after holding

    // private variables ------------------------
    private BoxCollider _mBoxCol;                   // Collider with the trigger
    private bool _mHolding;                         // Is an object in hand?
    private bool _mReadyToTrow;                     // If an object is ready to be dropped
    private bool _mReadyToPlant;                    // If the holding object is a potato, ready to plant


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
            // Keep the object stick on its original point and follow collisions
            m_proximityObject.transform.position = transform.position;
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
                    if (m_proximityObject.GetComponent<PlantFSM>() != null &&
                        !m_proximityObject.GetComponent<PlantFSM>().Planted)
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

        // Get its rigid body and cancel its gravity
        Rigidbody objectRb = m_proximityObject.GetComponent<Rigidbody>();
        objectRb.useGravity = false;

        // Disable object's collider
        foreach (Collider objectCollider in m_proximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = true;


        // Set the same position as our player hands
        m_proximityObject.transform.position = transform.position;

        // Bring back the trigger box as a collider
        _mBoxCol.isTrigger = false;

        // Currently holding
        _mHolding = true;

        // Check if it's a potato (activate the plant action)
        if (m_proximityObject.tag == ProjectTags.Potato)
            _mReadyToPlant = true;
    }


    // Trowing a dynamic object ----------------------------------------------------
    private void Trow(bool plant)
    {
        // Reset object rigid body's property
        Rigidbody objectRb = m_proximityObject.GetComponent<Rigidbody>();
        objectRb.useGravity = true;

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
        if (m_proximityObject.GetComponent<PlantFSM>() != null && plant)
            m_proximityObject.GetComponent<PlantFSM>().Planting = true;


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
}
