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

    [System.Serializable]
    public enum PlantState
    {
        Uprooted,   //above ground
        Planted,    //in the ground
        Autonomous  //deus ex machina
    }
    
    //BASE CLASS FOR ALL LIVING THINGS THAT GROW
    [RequireComponent(typeof(Rigidbody))]        //automatically add rb
    [RequireComponent(typeof(MeshCollider))]    //automatically add meshcollider        
    public abstract class Plant : MonoBehaviour
    {
        #region Members
        //PLANT STATE
        [SerializeField] protected PlantState plantStatus = PlantState.Uprooted;
        [SerializeField] protected bool growing;
        [SerializeField] protected bool planting;
        [SerializeField] protected bool planted;
        
        //PLANTING PARAMS
        [SerializeField] protected float plantingDepth;
        [SerializeField] protected Vector2 plantingDepthRange = new Vector2(0.3f, 0.45f);
        
        //PLANTED/GROWTH PARAMS
        [SerializeField] protected Vector3 growingAxis = Vector3.up;
        [SerializeField] protected float growthRadius = 1.0f;  
        [SerializeField] protected float growthPace = 1.0005f;                    
    
        [SerializeField] protected float growthTime = 0.0f;           //growth counter   
        [SerializeField] protected float growthStartTime;             //time from when it was planted and growing
        [SerializeField] protected float growthCompletionTime = 6.0f;      //time where it finished growing
        
        //AUTONOMOUS PARAMS
        
        //NEIGHBOUGRING PLANTS
        [SerializeField] protected List<Plant> neighbouringPlants = new List<Plant>();
        
        //COMPONENTS 
        protected Rigidbody rb;
        
        //PROPERTIES (this makes private properties accessible to other scripts like an API)
        //NOTES: you can remove the set function to make it read only
        public PlantState PlantStatus { get => plantStatus; set => plantStatus = value; }
        public bool Growing { get => growing; }
        public bool Planting { get => planting; set => planting = value; }
        public bool Planted { get => planted; }

        public float GrowthRadius { get => growthRadius; }
        #endregion

        #region Call Methods
        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            switch (plantStatus)
            {
                case PlantState.Uprooted:
                    //Do nothing
                    break;
                case PlantState.Planted:
                    SetGrowthAxis();
                    break;
                case PlantState.Autonomous:
                    //Do nothing
                    break;
                default:
                    //Do nothing
                    break;
            }
        }
        
        protected virtual void Update()
        {
            switch (plantStatus)
            {
                case PlantState.Uprooted:
                    //Do nothing
                    break;
                case PlantState.Planted:
                    Grow();
                    UpdateGrowthRadius();
                    break;
                case PlantState.Autonomous:
                    //Do nothing
                    break;
                default:
                    //Do nothing
                    break;
            }
        }
        
        protected virtual void OnEnable()
        {
            growthStartTime = Time.time; //Note: need to refactor this line 
            if(GameManager.Instance.plantsController != null) GameManager.Instance.plantsController.Plants.Add(this);
        }

        protected virtual void OnDisable()
        {
            ///if(GameManager.Instance.plantsController != null) GameManager.Instance.plantsController.Plants.Remove(this);
        }
        #endregion
        
        #region Collisions
        protected virtual void OnCollisionEnter(Collision col)
        {
            switch (plantStatus)
            {
                case PlantState.Uprooted:
                    CheckGroundAndPlant(col);
                    break;
                case PlantState.Planted:
                    //Do nothing
                    break;
                case PlantState.Autonomous:
                    //Do nothing
                    break;
                default:
                    //Do nothing
                    break;
            }
        }
        
        #endregion

        #region Uprooted State

        // Check the first collision with the ground -------------------------------
        protected virtual void CheckGroundAndPlant(Collision col)
        {
            // PlantObject when in contact with the ground
            if (col.gameObject.tag == ProjectTags.Ground && planting)
                PlantObject();
        }
        
        protected virtual void PlantObject()
        {
            // Pick a random depth number
            plantingDepth = Random.Range(plantingDepthRange.x, plantingDepthRange.y);

            // Deactivate gravity and freeze all
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Deactivate the colliders
            foreach (Collider objectCollider in GetComponents<Collider>())
                objectCollider.isTrigger = true;

            // Get the potato in the ground
            Vector3 currentPos = transform.position;
            currentPos.y -= plantingDepth;
            transform.position = currentPos;

            // The potato is now planted!
            planting = false;
            planted = true;
            
            //Change the state of potato
            plantStatus = PlantState.Planted;
        }
        
        #endregion

        #region Planted State
        protected virtual void Grow()
        {
            if (growthTime < growthCompletionTime && !IsCollidingNeighbouringPlants())
            {
                growthTime += Time.deltaTime;
                GrowAlongAxis();
                growing = true;
            }
            else
            {
                growing = false;
            }
        }

        protected virtual void UpdateGrowthRadius()
        {
            if (growing)
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
        #endregion
        
        #region Gizmos
        protected virtual void OnDrawGizmos()
        {
            //GROWTH RADIUS
            if(growing) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, growthRadius);
            
            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position, this.transform.position + growingAxis * 3.0f);
        }
        #endregion
    }

}
