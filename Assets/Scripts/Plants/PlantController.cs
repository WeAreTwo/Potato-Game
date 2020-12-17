using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantController : MonoBehaviour
{
    // public variables -------------------------
    
    public float m_growingTime = 5f;  // Time for the plant to grow out
    public float m_overgrowingDelay = 5f;  // Delay for the plant to be overgrown
    [ValueDropdown("PlantTypes")] 
    public int m_plantType = 0;

    [Title("States")]
    [ReadOnly] public bool m_planted = false;  // Main state of the plant  
    [ReadOnly] public bool m_grown = false;  // Fully grown state
    [ReadOnly] public bool m_overgrown;  // Overgrown, becoming alive
    [ReadOnly] public int m_biome;  // In which biome does this currently is
    [Title("Prototyping")] 
    public Material m_grownMaterial;  // Material when seed plant is grown
    public Material m_overgrownMaterial;  // Material when the plant is overgrown
    
    
    // private variables ------------------------
    private MeshRenderer m_renderer;  // Instance of the renderer
    private float _growTimer = 0.0f;  // Timer used when growing the plant
    private float _overgrowTimer = 0.0f;  // Timer used to determine when the plant is overgrown


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Get components
        m_renderer = GetComponent<MeshRenderer>();
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        // When the plant is planted
        if (m_planted && !m_grown)
            Grow();
        
        // When the plant is grown
        if (m_grown && !m_overgrown)
            Overgrowing();
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Allow the plant to grow -------------------------------------------------
    private void Grow()
    {
        // Make sure the plant can grow where it's been planted
        if (m_biome != m_plantType) 
            return;
        
        // Growing time
        if (_growTimer >= m_growingTime)
        {
            // Plant is now grown and ready to be harvested
            m_grown = true;
            m_renderer.material = m_grownMaterial;
        }
        else
            _growTimer += Time.deltaTime;
    }
    
    
    // Determine when the pant is overgrown ------------------------------------
    private void Overgrowing()
    {
        // Growing time
        if (_overgrowTimer >= m_overgrowingDelay)
        {
            // Plant is now grown and ready to be harvested
            m_overgrown = true;
            m_renderer.material = m_overgrownMaterial;
        }
        else
            _overgrowTimer += Time.deltaTime;
    }
    
    // All the available plant types -------------------------------------------
    private IEnumerable PlantTypes = new ValueDropdownList<int>()
    {
        {"None", 0},
        {"Type A", 1},
        {"Type B", 2},
        {"Type C", 3},
    };
}
