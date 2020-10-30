using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class FieldTypeController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_playerInField = false;  // Check if the player is in this field or not
    [Title("Field Type")] 
    public string m_fieldName = "ENTER FIELD NAME";  // Name of the field
    public bool m_potatoField;  // State if the field is for potatoes
    public bool m_tomatoField;  // State if the field is for tomatoes
    public bool m_onionField;  // State if the field is for onions
    [Space(10)][Title("Scene View")]
    public bool m_drawArea;  // Draw the area in scene view (for testing and prototyping level)
    [ShowIf("m_drawArea")] public Color m_areaColor;  // Color of this area
    [ShowIf("m_drawArea")] public SphereCollider[] m_sphere;  // All spheres collider to determine the area
    [ShowIf("m_drawArea")] public BoxCollider[] m_box;  // All boxes collider to determine the area

    
    // private variables ------------------------
    private GameObject _player;  // Instance of the player game object in the scene
    


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        _player = GameObject.FindWithTag("Player");
        

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
            Debug.Log("Player in " + m_fieldName + " field.");
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        // Check if colliding with the player
        if (col.CompareTag("Player"))
        {
            // Player out of the area
            m_playerInField = false;
            Debug.Log("Player no longer in " + m_fieldName + " field." );
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
            Gizmos.color = m_areaColor;
        
        // Draw all spheres collider
        if (m_sphere != null)
        {
            for (int i = 0; i < m_sphere.Length; i++)
            {
                // Get the position and radius of the collider
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireSphere(m_sphere[i].center, m_sphere[i].radius);
            }
        }
        
        // Draw all boxes collider
        if (m_box != null)
        {
            for (int i = 0; i < m_box.Length; i++)
            {
                // Get the position and size of the collider
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(m_box[i].center, m_box[i].size);
            }
        }
    }
}
