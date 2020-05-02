using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class GrowthParameters
    {
        
    }
    
    //BASE CLASS FOR ALL LIVING THINGS THAT GROW
    public abstract class Plant : MonoBehaviour
    {
        //GROWTH PARAMS
        [SerializeField] protected float growthRadius = 1.0f;  
        [SerializeField] protected float growthPace = 1.001f;                    
    
        [SerializeField] protected float growthTime = 0.0f;           //growth counter   
        [SerializeField] protected float growthStartTime;             //time from when it was planted and growing
        [SerializeField] protected float growthCompletionTime = 10.0f;      //time where it finished growing

        [SerializeField] protected bool isGrowing;
        
        //NEIGHBOUGRING PLANTS
        [SerializeField] protected List<Plant> neighbouringPlants = new List<Plant>();
        
        //PROPERTIES
        public float GrowthRadius { get => growthRadius; }

        protected virtual void Awake(){}
        protected virtual void Start(){}
        
        protected virtual void Update()
        {
            Grow();
            UpdateGrowthRadius();
        }

        protected virtual void Grow()
        {
            if (growthTime < growthCompletionTime && !IsCollidingNeighbouringPlants())
            {
                growthTime += Time.deltaTime;
                isGrowing = true;
            }
            else
            {
                isGrowing = false;
            }
        }

        protected virtual void UpdateGrowthRadius()
        {
            if (isGrowing)
            {
                this.transform.localScale *= growthPace;
                growthRadius *= growthPace;
            }
        }

        protected virtual bool IsCollidingNeighbouringPlants()
        {
            var allPlants = GameManager.Instance.plantsController.Plants;
            if (allPlants != null)
            {
                foreach (var plant in allPlants)
                {
                    if (this == plant) continue; //ignore self by skipping it 
                        
                    if(Vector3.Distance(this.transform.position, plant.transform.position) < this.growthRadius + plant.growthRadius)
                        return true;
                }
            }

            return false;
        }

        protected virtual void OnEnable()
        {
            growthStartTime = Time.time;
        }
        
        protected virtual void OnDisable(){}

        protected virtual void OnDrawGizmos()
        {
            if(isGrowing) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, growthRadius);
        }
    }

}
