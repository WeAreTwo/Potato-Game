using System.Collections;
using System.Collections.Generic;
using PotatoGame;
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
    private void OnTriggerEnter(Collider col)
    {
        // Check if colliding with the player or AI
        if (!col.CompareTag("Player") && !col.CompareTag("AI")) 
            return;
        
        // Capture the action controller and add this point to their list
        var action = col.gameObject.GetComponentInChildren<ActionController>();
        action.m_proximityPoints.Add(this.gameObject);
    }
    
    private void OnTriggerExit(Collider col)
    {
        // Check if colliding with the player or AI
        if (!col.CompareTag("Player") && !col.CompareTag("AI")) 
            return;

        // Capture the action controller and remove this point from their list
        var action = col.gameObject.GetComponentInChildren<ActionController>();
        action.m_proximityPoints.Remove(this.gameObject);
    }
    
    
    // Planting an object at this point ----------------------------------------
    public void Planted()
    {
        // This point is now occupied
        m_occupied = true;
    }
    
    // Harvesting and object planted a this point ------------------------------
    public void Harvested()
    {
        // This point is now available
        m_occupied = false;
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
