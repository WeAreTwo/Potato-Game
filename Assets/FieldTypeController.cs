using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldTypeController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_playerInField = false;  // Check if the player is in this field or not



    // private variables ------------------------
    private GameObject _mPlayer;  // Instance of the player game object in the scene



    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        _mPlayer = GameObject.FindWithTag("Player");

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
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            m_playerInField = true;
            Debug.Log("Player in field area A");
        }
    }
    
    
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            m_playerInField = false;
            Debug.Log("Player no longer in field area A");
        }
    }
}
