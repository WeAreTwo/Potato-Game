using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantingPointsGenerator : MonoBehaviour
{
    // public variables -------------------------
    [Title("Points parameters")]
    public float m_pointRadius;  // Radius of each point
    public Vector2 m_regionSize;  // Region size where to spawn points (grid)
    public int m_rejectionSamples = 30;  // Number of rejection per point allowed
    [Title("Valid Planting Spot")] 
    public LayerMask m_groundLayer;  // Layer mask to collide with (ground)
    [ReadOnly] public List<Vector3> m_plantingSpots;  // Valid planting spot
    [Title("Gizmos Settings")] 
    public bool m_displayGizmos = true;  // Display all Gizmos in the scene view
    [ShowIf("m_displayGizmos")] public float m_displayRadius = 1f;  // Radius of the points sphere in gizmos
    [ShowIf("m_displayGizmos")] public float m_displayRaysHeight = 75f;  // Height of the lines (rays)
    [ShowIf("m_displayGizmos")] public bool m_drawValidPoints = true;  // Display points that generate a valid spot
    [ShowIf("m_displayGizmos")] public bool m_drawInvalidPoints = true;  // Display points that do not generate a valid spot
    [ShowIf("m_displayGizmos")] public bool m_drawValidRays = true;  // Display rays that end with a valid spot
    [ShowIf("m_displayGizmos")] public bool m_drawInvalidRays = true;  // Display rays that end with no valid spot
    

    // private variables ------------------------
    private List<Vector2> _points;  // Capture all generated points
    private Vector3[] _worldPoints;  // Points coordinate in 3d space
    private List<int> _validPoints = new List<int>();  // Indexes of all valid generating points


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    
    // Change when inspector changes -------------------------------------------
    private void OnValidate()
    {
        // Clear all planting spots
        m_plantingSpots.Clear();
        _validPoints.Clear();

        // Get points generated (poisson sampling)
        _points = PoissonDiscSampling.GeneratePoints(m_pointRadius, m_regionSize, m_rejectionSamples);
        
        // Create array in relation of point in world space
        _worldPoints = new Vector3[_points.Count];

        // Convert each points to a 3D environment
        for (int i = 0; i < _worldPoints.Length; i++)
        {
            _worldPoints[i] = new Vector3(_points[i].x - m_regionSize.x / 2, transform.position.y, 
                _points[i].y - m_regionSize.y / 2);

            
            RaycastHit hit;
            if (Physics.Raycast(_worldPoints[i], Vector3.down * 75f, out hit, Mathf.Infinity, m_groundLayer))
            {
                m_plantingSpots.Add(hit.point);
                _validPoints.Add(i);
            }
        }
    }
    
    
    // Draw Gizmos into the scene ----------------------------------------------
    private void OnDrawGizmos()
    {
        if (m_displayGizmos)
        {
            Vector3 gridCenter = transform.position;
            Vector3 gridSize = new Vector3(m_regionSize.x, 0, m_regionSize.y);

            // Draw the grid
            Gizmos.DrawWireCube(gridCenter, gridSize);

            // Make sure points are generated
            if (_points != null)
            {
                for (int i = 0; i < _worldPoints.Length; i++)
                {
                    // Draw valid points
                    if (_validPoints != null && _validPoints.Contains(i) && m_drawValidPoints)
                    {
                        // Capture the valid planting spot
                        Gizmos.color = Color.white;
                        Gizmos.DrawSphere(_worldPoints[i], m_displayRadius);

                        if (m_drawValidRays)
                            Gizmos.DrawRay(_worldPoints[i], Vector3.down * m_displayRaysHeight);
                    }
                    else if (m_drawInvalidPoints && _validPoints != null && !_validPoints.Contains(i))
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawSphere(_worldPoints[i], m_displayRadius);
                        
                        if (m_drawInvalidRays)
                            Gizmos.DrawRay(_worldPoints[i], Vector3.down * m_displayRaysHeight);
                    }
                }

                for (int i = 0; i < m_plantingSpots.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(m_plantingSpots[i], m_displayRadius);

                }
            }
        }
    }
}
