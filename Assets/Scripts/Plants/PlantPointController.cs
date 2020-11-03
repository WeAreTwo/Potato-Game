using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantPointController : MonoBehaviour
{
    // public variables -------------------------
    public bool m_occupied;  // Determine if the point is occupied or not
    public bool m_playerInRange;  // Is the player in range to plant?
    [Title("Plant type")] 
    public string m_pointIsInBiome;  // What type of biome does this point lives in?
    public bool m_drawGizmos;  // Draw gizmos of this point
    
    [HideInInspector] public int m_biomeType;  // Index number of the biome type
    [HideInInspector] public Color m_pointColor;  // Point's color relative to the type of biome
    [HideInInspector] public float m_gizmoRadius = 1f; // Radius of the point
    
    // private variables ------------------------
    
    

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Get the triggers in action ----------------------------------------------
    private void OnTriggerStay(Collider col)
    {
        // Check if colliding with the player
        if (!col.CompareTag("Player")) 
            return;
        
        // Player in the area
        m_playerInRange = true;
        Debug.Log("Player can plant a " + m_pointIsInBiome + " plant.");
    }
    
    private void OnTriggerExit(Collider col)
    {
        // Check if colliding with the player
        if (!col.CompareTag("Player")) 
            return;
        
        // Player out of the area
        m_playerInRange = false;
        Debug.Log("Player no longer in " + m_pointIsInBiome + " biome." );
    }

    
    #region Gizmos
    // Draw Gizmos into the scene ----------------------------------------------
    private void OnDrawGizmos()
    {
        if (!m_drawGizmos) 
            return;
        
        Gizmos.color = m_pointColor;
        Gizmos.DrawSphere(transform.position, m_gizmoRadius);
    }
    #endregion
    
}
