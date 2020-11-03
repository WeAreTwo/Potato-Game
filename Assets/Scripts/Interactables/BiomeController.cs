using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class BiomeController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_playerInField = false;  // Check if the player is in this field or not
    [Title("Biome Type")]
    [ValueDropdown("BiomeTypes")] 
    public int m_biomeType = 0;
    [ReadOnly] public Color m_biomeColor;
    [Space(10)][Title("Scene View")]
    public bool m_drawArea;  // Draw the area in scene view (for testing and prototyping level)
    [ShowIf("m_drawArea")] public SphereCollider[] m_sphere;  // All spheres collider to determine the area
    [ShowIf("m_drawArea")] public BoxCollider[] m_box;  // All boxes collider to determine the area

    [HideInInspector] public string m_biomeName;  // Name of the biome
    
    // private variables ------------------------
    private GameObject _player;  // Instance of the player game object in the scene



    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        _player = GameObject.FindWithTag("Player");
        
        // Get correct biome name
        GetBiomeParameters();
    }


    // All the available biome types -------------------------------------------
    private IEnumerable BiomeTypes = new ValueDropdownList<int>()
    {
        {"None", 0},
        {"Type A", 1},
        {"Type B", 2},
        {"Type C", 3},
    };
    
    
    // Get the correct name for the biome ---------------------------------------
    private void GetBiomeParameters()
    {
        // Select the correct name
        switch(m_biomeType)
        { 
            case 0: 
                m_biomeName = "None";
                m_biomeColor = Color.clear;
                break;
            case 1:
                m_biomeName = "Type A";
                m_biomeColor = new Color(1, 0.3f, 0, 1);
                break;
            case 2:
                m_biomeName = "Type B";
                m_biomeColor = new Color(0.95f, 0.3f, 0.7f, 1);
                break;
            case 3:
                m_biomeName = "Type C";
                m_biomeColor = new Color(0.3f, 0.5f, 0.95f, 1);
                break;
        }
    }
    
    
    // Function to load when modifying the inspector ---------------------------
    private void OnValidate()
    {
        // get the selected biome parameter
        GetBiomeParameters();
    }
    

    #region Gizmos
    
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
            Gizmos.color = m_biomeColor;
        
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
    #endregion
}
