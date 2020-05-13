using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    [System.Serializable]
    public enum PlantPhase
    {
        Seed,
        Grown, 
        Sentient
    }

    [System.Serializable]
    public enum PlantState
    {
        Uprooted,       //above ground
        Planted,        //in the ground
        Autonomous      //deus ex machina
    }
    
    [System.Serializable]
    public class GrowthParams
    {
        //PLANTED/GROWTH PARAMS
        [Header("GROWTH")]
        public Vector3 growingAxis = Vector3.up;
        public float growthRadius = 1.0f;  
        public float growthPace = 1.0005f;                    
    
        public float growthTime = 0.0f;           //growth counter   
        public float growthStartTime;             //time from when it was planted and growing
        public float growthCompletionTime = 6.0f;      //time where it finished growing
        
        //PRIVATE MEMBERS 
        public float harvestTime = 0.0f; 
        public float harvestPeriod = 15.0f; //second (amount of time before it before you cant harvest it anymore)
        
        
    }

    
    //BASE CLASS FOR ALL LIVING THINGS THAT GROW
    [RequireComponent(typeof(Rigidbody))]        //automatically add rb
    [RequireComponent(typeof(MeshCollider))]    //automatically add meshcollider        
    public abstract class Plant : MonoBehaviour
    {
        #region Members

        [Header("COMMON")] 
        [SerializeField] protected float health = 100.0f;
        
        //PLANT STATE
        [Header("STATES")]
        [SerializeField] protected PlantPhase plantPhase = PlantPhase.Seed;
        [SerializeField] protected PlantState plantStatus = PlantState.Uprooted;
        
        [SerializeField] protected bool planting;
        [SerializeField] protected bool planted;

        [SerializeField] protected bool growing = true;
        [SerializeField] protected bool growthCompleted;
        
        [SerializeField] protected bool harvestable;
        [SerializeField] protected bool harvestPeriodCompleted;
        
        //PLANTING PARAMS
        [Header("PLANTING")]
        [SerializeField] protected float plantingDepth;
        [SerializeField] protected Vector2 plantingDepthRange = new Vector2(0.3f, 0.45f);
        
        //GROWTH PARAMS
        [SerializeField] protected GrowthParams growthParams;
        
        //NEIGHBOUGRING PLANTS
        protected List<Plant> neighbouringPlants = new List<Plant>();
        
        //COMPONENTS 
        protected Rigidbody rb;
        
        //PROPERTIES (this makes private properties accessible to other scripts like an API)
        //NOTES: you can remove the set function to make it read only
        public PlantState PlantStatus { get => plantStatus; set => plantStatus = value; }
        public bool Growing { get => growing; }
        public bool Planting { get => planting; set => planting = value; }
        public bool Planted { get => planted; }

        public float GrowthRadius { get => growthParams.growthRadius; }

        public float Health { get => health; set => health = value; }

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
            Die();
            switch (plantStatus)
            {
                case PlantState.Uprooted:
                    //Do nothing
                    break;
                case PlantState.Planted:
                    PlantedSettings();
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
            if(GameManager.Instance != null) GameManager.Instance.plantsController.Plants.Add(this);
        }

        protected virtual void OnDisable()
        {
            if(GameManager.Instance != null) GameManager.Instance.plantsController.Plants.Remove(this);
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
                    OnPlantCollision(col);
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

        #region Common

        protected virtual void Die()
        {
            if (health <= 0)
            {
                Destroy(this.gameObject);
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

        protected virtual void PlantedSettings()
        {
            // Deactivate gravity and freeze all
            if(rb.useGravity == true) rb.useGravity = false;
            if(rb.constraints != RigidbodyConstraints.FreezeAll) rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        protected virtual void Grow()
        {
            //growth period 
            if (growthParams.growthTime <= growthParams.growthCompletionTime && !growthCompleted)
            {
                growthParams.growthTime += Time.deltaTime;
                growing = true;
            }
            else if(growthParams.growthTime >= growthParams.growthCompletionTime)
            {
                growthCompleted = true;
                growing = false;
            }

            //harvest period
            if (growthParams.harvestTime <= growthParams.harvestPeriod && growthCompleted && !harvestPeriodCompleted)
            {
                growthParams.harvestTime += Time.deltaTime;
                harvestable = true;
            }
            else if(growthParams.harvestTime >= growthParams.harvestPeriod)
            {
                harvestPeriodCompleted = true;
                harvestable = false;
            }
        }

        protected virtual void UpdateGrowthRadius()
        {
            if (growing)
            {
                this.transform.localScale *= growthParams.growthPace;
                growthParams.growthRadius *= growthParams.growthPace;
            }
        }

        protected virtual void SetGrowthAxis()
        {
            growthParams.growingAxis.x = Random.Range(-0.50f, 0.50f);
            growthParams.growingAxis.z = Random.Range(-0.50f, 0.50f);
        }

        protected virtual void GrowAlongAxis()
        {
            this.transform.position += growthParams.growingAxis * 0.001f;
        }

        protected virtual void OnPlantCollision(Collision col)
        {
            var plantComponent = col.gameObject.GetComponent<Plant>();
            if (plantComponent != null && plantComponent.plantStatus == PlantState.Planted)
            {
                growing = false;
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
                        
                    if(Vector3.Distance(this.transform.position, plant.transform.position) < this.growthParams.growthRadius + plant.growthParams.growthRadius)
                        return true;
                }
            }

            return false;
        }
        #endregion
        
        #region Gizmos

        protected virtual void UprootedGizmos() { }
        protected virtual void PlantedGizmos()
        {
            //GROWTH RADIUS
            if(growing) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, growthParams.growthRadius);
            
            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position, this.transform.position + growthParams.growingAxis * 3.0f);
            
            //HARVESTABLES
            if (harvestable)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(this.transform.position + Vector3.up * growthParams.growthRadius, 0.2f);
            }
            
        }
        protected virtual void AutonomousGizmos() { }
        
        protected virtual void OnDrawGizmos()
        {
            switch (PlantStatus)
            {
                case PlantState.Uprooted:
                    UprootedGizmos();
                    break;
                case PlantState.Planted:
                    PlantedGizmos();
                    break;
                case PlantState.Autonomous:
                    AutonomousGizmos();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

}
