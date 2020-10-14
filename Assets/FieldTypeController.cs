﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class FieldTypeController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_drawArea;  // Draw the area in scene view (for testing and prototyping level)
    public bool m_playerInField = false;  // Check if the player is in this field or not
    [Title("Triggers")] 
    public SphereCollider[] m_sphere;  // All spheres collider to determine the area
    public BoxCollider[] m_box;  // All boxes collider to determine the area

    
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
    // Get the triggers in action ----------------------------------------------
    private void OnTriggerStay(Collider col)
    {
        // Check if colliding with the player
        if (col.CompareTag("Player"))
        {
            // Player in the area
            m_playerInField = true;
            Debug.Log("Player in field area A");
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        // Check if colliding with the player
        if (col.CompareTag("Player"))
        {
            // Player out of the area
            m_playerInField = false;
            Debug.Log("Player no longer in field area A");
        }
    }

    
    
    
    
    // Interactive gizmos in scene view ----------------------------------------
    private void OnDrawGizmos()
    {
        // Enable or disable gizmos
        if (!m_drawArea)
            return;
        
        // Select a color depending on the player state
        if (m_playerInField)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.magenta;
        

        // Draw all spheres collider
        if (m_sphere != null)
        {
            for (int i = 0; i < m_sphere.Length; i++)
            {
                // Get the position and radius of the collider
                var pos = transform.position + m_sphere[i].center;
                Gizmos.DrawWireSphere(pos, m_sphere[i].radius);
            }
        }
    }
}
