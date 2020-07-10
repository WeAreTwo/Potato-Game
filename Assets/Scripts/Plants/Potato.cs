using System;
using UnityEngine;

namespace PotatoGame
{
    
    [System.Serializable]
    public class GrowthSettings
    {
        //SEED PARAM
        [Header("SEED")] 
        public float seedSize = 0.25f;
        
        //PLANTED/GROWTH PARAMS
        [Header("GROWTH")]
        public Vector3 growingAxis = Vector3.up;
        public float growthRadius = 1.0f;  
        public float growthPace = 1.0005f;
        public float growthSize = 1.0f;
        public float growthTime = 0.0f;
        public float growthCompletionTime = 12.0f;      //time where it finished growing
        
        //HARVEST PARAMS 
        [Header("HARVEST")]
        public float harvestTime = 0.0f; 
        public float harvestPeriod = 15.0f; //second (amount of time before it before you cant harvest it anymore)
        public int harvestYield = 2; //how many seeds your gonna get out of this 

        [Header("VISUAL")] 
        [SerializeField] protected Color currentColor;
        public Color seedColor = Color.grey;
        public Color grownColor = Color.yellow;
        
        
        public Vector3 ShiftSize()
        {
            float dt = growthTime/growthCompletionTime;
            return Vector3.Lerp(Vector3.one * seedSize, Vector3.one * growthSize, dt );
        }
        
        public Color ShiftColor()
        {
            float dt = growthTime/growthCompletionTime;
            currentColor = Color.Lerp(seedColor, grownColor, dt);
            return currentColor;
        }
    }
    
    public class Potato : Plant
    {
        protected const string name = "Seed";

        [SerializeField] protected GameObject autonomousForm;

        [SerializeField] protected bool growing;
        [SerializeField] protected bool growthCompleted;

        protected Material mat;
        
        //CONSTRUCTOR
        public Potato()
        {
        }

        //CALL METHODS 
        public virtual void Start()
        {
            // component.transform.localScale *= component.GrowthParams.seedSize;
            mat = GetComponent<Renderer>().material;
        }

        public virtual void Update()
        {
            this.transform.localScale = growthSettings.ShiftSize();
            mat.SetColor("_BaseColor", growthSettings.ShiftColor());
            if (Planted)
            {
                // PlantedSettings();
                Grow();
                UpdateGrowthRadius();
            }
            else
            {
                // UprootedSettings();
            }
        }
        
        public virtual void OnCollisionEnter(Collision col)
        {
            var plantComponent = col.gameObject.GetComponent<PlantFSM>();
            if (plantComponent != null)
            {
                growthCompleted = true;
            }
        }
        
        public override void PlantObject()
        {
            this.gameObject.DeActivatePhysics();
        }


        protected virtual void Grow()
        {
            
            //growth period 
            if (growthSettings.growthTime <= growthSettings.growthCompletionTime && !growthCompleted)
            {
                growthSettings.growthTime += Time.deltaTime;
                growing = true;
            }
            else if (growthSettings.growthTime >= growthSettings.growthCompletionTime)
            {
                growthCompleted = true;
                growing = false;
                
                //Todo : This is where we poof into an autonomous potato 
                // TriggerExit(PlantStates.Grown);
                TransformToAutonomous();
                
            }
        }

        protected virtual void TransformToAutonomous()
        {
            //particles
            ParticleController.Instance.EmitAt(this.transform.position);
            GameObject obj = Instantiate(autonomousForm, this.transform.position + Vector3.up, Quaternion.identity);
            
            Destroy(this.gameObject);
        }

        protected virtual void UpdateGrowthRadius()
        {
            if (growing && !growthCompleted)
            {
                float dt = growthSettings.growthTime / growthSettings.growthCompletionTime;
                // this.transform.localScale = Vector3.Lerp(
                //     Vector3.one * GrowthParams.seedSize,
                //     Vector3.one * GrowthParams.growthSize,
                //     dt
                //     );
                //
                // this.transform.localScale *= GrowthParams.growthPace;
                growthSettings.growthRadius *= growthSettings.growthPace;
            }
        }

        protected virtual void SetGrowthAxis()
        {
            growthSettings.growingAxis.x = UnityEngine.Random.Range(-0.50f, 0.50f);
            growthSettings.growingAxis.z = UnityEngine.Random.Range(-0.50f, 0.50f);
        }

        protected virtual void GrowAlongAxis()
        {
            this.transform.position += growthSettings.growingAxis * 0.001f;
        }

        //GIZMOS
        public virtual void OnDrawGizmos()
        {
            //GROWTH RADIUS
            if (!growthCompleted) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, growthSettings.growthRadius);

            //GROWTH AXIS 
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + growthSettings.growingAxis * 3.0f);

        }
    }
}