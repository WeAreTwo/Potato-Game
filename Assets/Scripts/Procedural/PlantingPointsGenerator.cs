using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantingPointsGenerator : MonoBehaviour
{
    // public variables -------------------------
    [Title("Sample parameters")]
    public float m_pointRadius;  // Radius of each point
    public Vector2 m_regionSize;  // Region size where to spawn points (grid)
    public int m_rejectionSamples = 30;  // Number of rejection per point allowed
    [Title("Valid Planting Points")] 
    public LayerMask m_groundLayer;  // Layer mask to collide with (ground)
    public LayerMask m_biomeLayer;  // Layer mask to collide with (biomes)
    [AssetsOnly] public GameObject m_plantingPointObject;  // Planting point prefab object
    [Title("Gizmos Settings")] 
    public bool m_displayGizmos = true;  // Display all Gizmos in the scene view
    [InfoBox("Drawing gizmos for this point generation is expensive in edit mode. Use them for debugging purpose only and disable them for better scene view experience.")]
    [ShowIf("m_displayGizmos")] public float m_displayRadius = 1f;  // Radius of the points sphere in gizmos
    [ShowIf("m_displayGizmos")] public float m_displayRaysHeight = 75f;  // Height of the lines (rays)
    [ShowIf("m_displayGizmos")][TabGroup("Points")] public bool m_drawGrid = true;  // Display points that generate a valid spot
    [ShowIf("m_displayGizmos")][TabGroup("Points")] public bool m_drawValidPoints = true;  // Display points that generate a valid spot
    [ShowIf("m_displayGizmos")][TabGroup("Points")] public bool m_drawInvalidPoints = true;  // Display points that do not generate a valid spot
    [ShowIf("m_displayGizmos")][TabGroup("Rays")] public bool m_drawValidRays = true;  // Display rays that end with a valid spot
    [ShowIf("m_displayGizmos")][TabGroup("Rays")] public bool m_drawInvalidRays = true;  // Display rays that end with no valid spot
    [ShowIf("m_displayGizmos")][TabGroup("Planting Points")] public bool m_drawPlantingPoints = true;  // Display rays that end with no valid spot
    [InfoBox("Only in play mode:")]
    [ShowIf("m_displayGizmos")][TabGroup("Planting Points")] public bool m_drawPointsBiome= true;  // Display rays that end with no valid spot
    

    // private variables ------------------------
    private bool _pointsGenerated = false;  // Check if the points have been generated in the scene (as objects)
    private List<Vector2> _points;  // Capture all generated points
    private Vector3[] _worldPoints;  // Points coordinate in 3d space
    private readonly List<Vector3> _hitPoints = new List<Vector3>();  // Ground point where rays hit
    private readonly List<int> _validPoints = new List<int>();  // Indexes of all valid generating points


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Search for all planting points
        SearchGroundPoints();
        
        // Then spawn every valid planting points
        SpawnPlantingPoints();
    }
    

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Map points on the  ground -----------------------------------------------
    private void SearchGroundPoints()
    {
        // Clear all planting spots
        _hitPoints.Clear();
        _validPoints.Clear();

        // Get points generated (poisson sampling)
        _points = PoissonDiscSampling.GeneratePoints(m_pointRadius, m_regionSize, m_rejectionSamples);
        
        // Create array in relation of point in world space
        _worldPoints = new Vector3[_points.Count];

        // Convert each points to a 3D environment
        for (int i = 0; i < _worldPoints.Length; i++)
        {
            // World points in relation to game object in world (centered grid)
            _worldPoints[i] = new Vector3(_points[i].x - m_regionSize.x / 2, transform.position.y, 
                _points[i].y - m_regionSize.y / 2);

            
            // Cast a ray down to the ground
            RaycastHit hit;
            if (!Physics.Raycast(_worldPoints[i], Vector3.down, out hit, Mathf.Infinity, m_groundLayer)) 
                continue;
            
            // Register the current point as a hit
            _hitPoints.Add(hit.point);
            _validPoints.Add(i);
        }
    }
    
    
    // Set the valid planting points -------------------------------------------
    private void SpawnPlantingPoints()
    {
        // Create a planting point for each hit point on the ground
        foreach (var point in _hitPoints)
        {
            // Create a candidate object based on the planting point prefab
            var candidate = m_plantingPointObject;
            var candidateController = candidate.GetComponent<PlantPointController>();
            
            // Cast origin point needs to be above the biomes
            var castOrigin = point;
            castOrigin.y += 50f;

            // Create a name for the point
            var pointName = string.Empty;

            // Cast a ray in search of a biome
            RaycastHit biomeHit;
            if (Physics.Raycast(castOrigin, Vector3.down, out biomeHit, Mathf.Infinity,m_biomeLayer))
            {
                // If a biome is found, capture it and send data to the candidate
                var activeBiome = biomeHit.transform.gameObject.GetComponent<BiomeController>();
                candidateController.m_pointIsInBiome = activeBiome.m_biomeName;
                candidateController.m_biomeType = activeBiome.m_biomeType;
                candidateController.m_pointColor = activeBiome.m_biomeColor;
                pointName = "Plant Point " + activeBiome.m_biomeName;
            }
            else
            {
                // If no biome is found, set default value to the candidate
                candidateController.m_pointIsInBiome = "Not in a biome";
                candidateController.m_biomeType = 0;
                candidateController.m_pointColor = Color.gray;
                pointName = "Plant Point Empty";
            }
            
            // Update the candidate general values
            candidate.transform.position = point;
            candidateController.m_gizmoRadius = m_displayRadius;
            
            // Instantiate the new planting point based on the candidate
            var newPoint = Instantiate(candidate, point, Quaternion.identity, transform);
            newPoint.name = pointName;
            
            // Points are generated
            _pointsGenerated = true;
        }
    }
    
    
    
    // Change when inspector changes -------------------------------------------
    private void OnValidate()
    {
        // Only if gizmos are enabled and objects not yet generated
        if (m_displayGizmos && !_pointsGenerated)
        {
            // Only cast virtual points, skip generating objects
            SearchGroundPoints();
        }
    }

    
    #region Gizmos
    // Draw Gizmos into the scene ----------------------------------------------
    private void OnDrawGizmos()
    {
        // Only if gizmos are enabled
        if (!m_displayGizmos) 
            return;
        
        // Get the virtual grid in the center of the game object
        if (m_drawGrid)
        {
            var gridCenter = transform.position;
            var gridSize = new Vector3(m_regionSize.x, 0, m_regionSize.y);

            // Draw the grid
            Gizmos.DrawWireCube(gridCenter, gridSize);
        }

        // Make sure points are generated
        if (_points == null) 
            return;
        
        // For each points (translated in world space)
        for (var i = 0; i < _worldPoints.Length; i++)
        {
            // Draw valid points
            if (_validPoints != null && _validPoints.Contains(i) && m_drawValidPoints)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(_worldPoints[i], m_displayRadius);

                // Draw the rays down that hit the ground
                if (m_drawValidRays)
                    Gizmos.DrawRay(_worldPoints[i], Vector3.down * m_displayRaysHeight);
            }
            else if (m_drawInvalidPoints && _validPoints != null && !_validPoints.Contains(i))
            {
                // Draw invalid points
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(_worldPoints[i], m_displayRadius);
                       
                // Draw the rays down to the abyss
                if (m_drawInvalidRays)
                    Gizmos.DrawRay(_worldPoints[i], Vector3.down * m_displayRaysHeight);
            }
        }

        // Draw planting points
        if (m_drawPlantingPoints)
        {
            foreach (var point in _hitPoints)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(point, m_displayRadius * 2f);
            }
        }
        
        // Draw the planting points with their biome and states (only work in play mode)
        foreach (Transform child in transform)
        {
            child.GetComponent<PlantPointController>().m_drawGizmos = m_drawPointsBiome;
        }
    }
    #endregion
}
