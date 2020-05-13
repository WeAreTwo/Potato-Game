using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class ActionController : MonoBehaviour
{
    // public variables -------------------------
    public bool mCanInteract = true;               // Allow interaction with objects
    public GameObject mProximityObject;            // Target catched by a trigger
    public GameObject mActionIcon;                 // Action iconto be displayed when target's valid
    public float mTrowForce = 2.5f;                // Force when an object is trown after holding

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
            mProximityObject.transform.position = transform.position;
        }
    }


    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Check for inputs to trigger an action ---------------------------------------
    private void CheckInputs()
    {
        // Check if the action button is triggered ----------
        if (Input.GetAxisRaw("Action") != 0 && mProximityObject != null)
        {
            if (mCanInteract)
            {
                // Disable interaction 
                mCanInteract = !mCanInteract;

                // Scan for the correct type of object
                if (mProximityObject.tag == ProjectTags.DynamicObject)
                    Hold();

                if (mProximityObject.tag == ProjectTags.Potato)
                {
                    // Make sure the potato is not already planted
                    if (mProximityObject.GetComponent<PlantingController>() != null &&
                        !mProximityObject.GetComponent<PlantingController>().m_planted)
                        Hold();                    
                    
                    //Codrin Code for his potatoes
                    if (mProximityObject.GetComponent<Plant>() != null &&
                        !mProximityObject.GetComponent<Plant>().Planted)
                        Hold();
                }
            }

            // If player is holding an object, trow it
            if (_mHolding && _mReadyToTrow)
                Trow(false);
        }

        // When the action input is not triggered ----------
        if (Input.GetAxisRaw("Action") == 0 && mProximityObject != null)
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
            mProximityObject = col.gameObject;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // Empty out the proximity object
        if (col.gameObject.tag == ProjectTags.DynamicObject && !_mHolding ||
            col.gameObject.tag == ProjectTags.Potato && !_mHolding)
        {
            mProximityObject = null;
        }
    }


    // When holding a dynamic object -----------------------------------------------
    private void Hold()
    {
        // Set the object as a child
        mProximityObject.transform.SetParent(transform);

        // Get its rigid body and cancel its gravity
        Rigidbody objectRb = mProximityObject.GetComponent<Rigidbody>();
        objectRb.useGravity = false;

        // Disable object's collider
        foreach (Collider objectCollider in mProximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = true;


        // Set the same position as our player hands
        mProximityObject.transform.position = transform.position;

        // Bring back the trigger box as a collider
        _mBoxCol.isTrigger = false;

        // Currently holding
        _mHolding = true;

        // Check if it's a potato (activate the plant action)
        if (mProximityObject.tag == ProjectTags.Potato)
            _mReadyToPlant = true;
    }


    // Trowing a dynamic object ----------------------------------------------------
    private void Trow(bool plant)
    {
        // Reset object rigid body's property
        Rigidbody objectRb = mProximityObject.GetComponent<Rigidbody>();
        objectRb.useGravity = true;

        // Enable object's collider
        foreach(Collider objectCollider in mProximityObject.GetComponents<Collider>())
            objectCollider.isTrigger = false;

        // Apply a velocity to the object
        objectRb.velocity = transform.forward * mTrowForce;

        // Check if the object will be planted
        // Get the planting mechanic from the object activated
        if (mProximityObject.GetComponent<PlantingController>() != null && plant)
            mProximityObject.GetComponent<PlantingController>().m_planting = true;        
        
        
        // Codrin Code for his potatoes
        if (mProximityObject.GetComponent<Plant>() != null && plant)
            mProximityObject.GetComponent<Plant>().Planting = true;

        
        // Get rid of the object
        mProximityObject.transform.parent = null;
        mProximityObject = null;

        // Enable interaction
        mCanInteract = true;
        _mHolding = false;
        _mReadyToTrow = false;
        _mReadyToPlant = false;

        // Set the trigger back
        _mBoxCol.isTrigger = true;
    }


    
}
