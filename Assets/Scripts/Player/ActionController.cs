using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Sirenix.OdinInspector;

public class ActionController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_canInteract = true;               // Allow interaction with objects
    public GameObject m_proximityObject;            // Target catched by a trigger
    public GameObject m_actionIcon;                 // Action iconto be displayed when target's valid
    public float m_trowForce = 2.5f;                // Force when an object is trown after holding

    // private variables ------------------------
    private BoxCollider m_boxCol;                   // Collider with the trigger
    private bool m_holding = false;                 // Is an object in hand?
    private bool m_readyToTrow = false;             // If an object is ready to be dropped
    private bool m_readyToPlant = false;            // If the holding object is a potato, ready to plant


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        m_boxCol = GetComponent<BoxCollider>();
        
    }


    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        // Always check for inputs
        CheckInputs();

        // While holding
        if (m_holding)
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
        if (Input.GetAxisRaw("Action") != 0  
            && m_proximityObject != null)
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
            if (m_holding && m_readyToTrow)
                Trow(false);
        }

        // When the action input is not triggered ----------
        if (Input.GetAxisRaw("Action") == 0
            && m_proximityObject != null)
        {
            // When holding an object, next ready a trow
            if (m_holding)
                m_readyToTrow = true;
        }

        // If you can plant an object ----------------------
        if (Input.GetAxisRaw("Plant") !=0 && m_readyToPlant)
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
        if (col.gameObject.tag == ProjectTags.DynamicObject && !m_holding ||
            col.gameObject.tag == ProjectTags.Potato && !m_holding)
        {
            m_proximityObject = null;
        }
    }


    // When holding a dynamic object -----------------------------------------------
    private void Hold()
    {
        // Set the object as a child
        m_proximityObject.transform.SetParent(transform);

        // Get its rigidbody and cancel its gravity
        Rigidbody objectRB = m_proximityObject.GetComponent<Rigidbody>();
        objectRB.useGravity = false;

        // Disable object's colliders
        foreach (Collider objectCollider in m_proximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = true;


        // Set the same position as our player hands
        m_proximityObject.transform.position = transform.position;

        // Bring back the trigger box as a collider
        m_boxCol.isTrigger = false;

        // Currently holding
        m_holding = true;

        // Check if it's a potato (activate the plant action)
        if (m_proximityObject.tag == ProjectTags.Potato)
            m_readyToPlant = true;
    }


    // Trowing a dynamic object ----------------------------------------------------
    private void Trow(bool plant)
    {
        // Reset object rigidbody's property
        Rigidbody objectRB = m_proximityObject.GetComponent<Rigidbody>();
        objectRB.useGravity = true;

        // Enable object's colliders
        foreach(Collider objectCollider in m_proximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = false;

        // Apply a velocity to the object
        objectRB.velocity = transform.forward * m_trowForce;

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
        m_holding = false;
        m_readyToTrow = false;
        m_readyToPlant = false;

        // Set the trigger back
        m_boxCol.isTrigger = true;
    }


    
}
