using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class PoissonDiscSampling
{
    // Generate points on a 2d grid --------------------------------------------
    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSampleBeforeRejection = 30)
    {
        // Use pythagoras theorem to calculate the side of a grid cell
        // Since we know the radius of a disc should fit in a cell : s = r / Sqr(2)
        float cellSize = radius / Mathf.Sqrt(2);
        
        /*
         * -------------------------------------------------
         * This grid will tell for each cell, what the index of the point in the points list is,
         * that lies in that cell. ( 0 = no point in that cell, 1 = point in the cell)
         *
         * When we add a new point, we also add a new spawn point,
         * we would want to add future points around that spawn point 
         * -------------------------------------------------
         */
        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();
        
        // Listening to add a starting point as our spawn point (in the middle of the sample)
        spawnPoints.Add(sampleRegionSize / 2);
        
        // While the spawn points list is not empty
        while (spawnPoints.Count > 0)
        {
            // Pick a random spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];

            bool candidateAccepted = false;
            
            for (int i = 0; i < numSampleBeforeRejection; i++)
            {
                // Pick a random direction
                float angle = Random.value * Mathf.PI * 2;
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                
                // Get a candidate from the middle and new direction
                // Double the radius to make sure to be outside the middle point range
                Vector2 candidate = spawnCentre + direction * Random.Range(radius, 2 * radius);
                
                // Determine if we accept the candidate or not
                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    // If valid, we add the candidate to the points list and as a new spawn point
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    
                    // Record which cell it ends up in
                    grid[(int) (candidate.x / cellSize), (int) (candidate.y / cellSize)] = points.Count;
                    
                    // Candidate accepted
                    candidateAccepted = true;
                    break;
                }
            }
            
            // If no candidate was recorded
            if (!candidateAccepted)
            {
                // Get rid of the point
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        // Return the point list
        return points;
    }
    
    
    
    // Check if a candidate point is valid or not ------------------------------
    static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        // Does the candidate lies within the sample region?
        if (!(candidate.x >= 0) || !(candidate.x < sampleRegionSize.x) || 
            !(candidate.y >= 0) ||
            !(candidate.y < sampleRegionSize.y)) 
            return false;
        
        
        // In which cell the candidate does lie in? (to search surrounding cells)
        var cellX = (int) (candidate.x / cellSize);
        var cellY = (int) (candidate.y / cellSize);
            
        // Search the 5x5 block around the cell (2 on the left, and 2 on the right)
        // Min Max to make sure it doesn't go out of bounds of our grid
        var searchStartX = Mathf.Max(0,cellX - 2);
        var searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
        var searchStartY = Mathf.Max(0,cellY - 2);
        var searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

        for (var x = searchStartX; x <= searchEndX; x++)
        {
            for (var y = searchStartY; y <= searchEndY; y++)
            {
                // Catch a point index
                var pointIndex = grid[x, y] - 1;
                    
                // If the point index is equal to -1, that means, no point in that cell
                if (pointIndex == -1) 
                    continue;
                    
                // I use sqr for optimisation (less costly for magnitude)
                // That's why radius is squared in the check
                var sqrDistance = (candidate - points[pointIndex]).sqrMagnitude;
                        
                // Is it less than the radius?
                if (sqrDistance < radius * radius)
                {
                    // Candidate is too close to the point, not valid
                    return false;
                }
            }
        }
        // Candidate is valid
        return true;

        // Not valid
    }
}
