﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantingController : MonoBehaviour
{
    // public variables -------------------------
    [Title ("Planting States")]
    [ReadOnly] public bool m_planting;              // When ready to be planted in the ground
    [ReadOnly] public bool m_planted;               // Is the potato currently planted

    [Space]

    public Vector2 m_depthRange;                    // Range for the depth of the potato when planted

    // private variables ------------------------
    private Rigidbody m_rb;                         // Instance of the rigidbody
    private float m_depth;                            // How deep will the potato be planted


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        m_rb = GetComponent<Rigidbody>();

        
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
    // Check the first collision with the ground -------------------------------
    private void OnCollisionEnter(Collision col)
    {
        // Plant when in contact with the ground
        if (col.gameObject.tag == "Ground" && m_planting)
            Plant();
    }
    

    // Plant the potato --------------------------------------------------------
    private void Plant()
    {
        // Pick a random depth number
        m_depth = Random.Range(m_depthRange.x, m_depthRange.y);

        // Deactivate gravity and freeze all
        m_rb.useGravity = false;
        m_rb.constraints = RigidbodyConstraints.FreezeAll;

        // Deactivate the colliders
        foreach (Collider objectCollider in GetComponents<Collider>())
            objectCollider.isTrigger = true;

        // Get the potato in the ground
        Vector3 currentPos = transform.position;
        currentPos.y -= m_depth;
        transform.position = currentPos;

        // The potato is now planted!
        m_planting = false;
        m_planted = true;
    }
}
