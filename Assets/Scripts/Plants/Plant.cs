using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [SerializeField] protected Vector3 growingAxis = Vector3.up;
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

        protected virtual void Start()
        {
            SetGrowthAxis();
        }
        
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
                GrowAlongAxis();
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

        protected virtual void SetGrowthAxis()
        {
            growingAxis.x = Random.Range(-0.50f, 0.50f);
            growingAxis.z = Random.Range(-0.50f, 0.50f);
        }

        protected virtual void GrowAlongAxis()
        {
            this.transform.position += growingAxis * 0.001f;
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
            GameManager.Instance.plantsController.Plants.Add(this);
        }

        protected virtual void OnDisable()
        {
            //GameManager.Instance.plantsController.Plants.Remove(this);
        }

        protected virtual void OnDrawGizmos()
        {
            //GROWTH RADIUS
            if(isGrowing) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, growthRadius);
            
            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position, this.transform.position + growingAxis * 3.0f);
        }
    }

}
