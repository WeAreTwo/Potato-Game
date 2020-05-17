using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{ 
    
    [System.Serializable]
    public class GrowthParams
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
        
        public float growthTime = 0.0f;           //growth counter   
        public float growthStartTime;             //time from when it was planted and growing
        public float growthCompletionTime = 12.0f;      //time where it finished growing
        
        //HARVEST PARAMS 
        [Header("HARVEST")]
        public float harvestTime = 0.0f; 
        public float harvestPeriod = 15.0f; //second (amount of time before it before you cant harvest it anymore)
        public int harvestYield = 2; //how many seeds your gonna get out of this 
        

    }
    
    [RequireComponent(typeof(Rigidbody))]        //automatically add rb
    [RequireComponent(typeof(MeshCollider))]    //automatically add meshcollider        
    public abstract class PlantFSM : MonoBehaviour
    {
        [Header("HEALTH")] 
        [SerializeField] protected float health = 100.0f;
        
        [SerializeField] protected GrowthParams growthParams;
        [SerializeField] protected StateMachine fsm;

        [SerializeField] protected bool planting;
        [SerializeField] protected bool planted;

        //Components
        protected Rigidbody rb;

        public Rigidbody Rb { get => rb; set => rb = value; }
        public float Health { get => health; set => health = value; }
        public bool Planting { get => planting; set => planting = value; }
        public bool Planted { get => planted; set => planted = value; }
        public GrowthParams GrowthParams { get => growthParams; set => growthParams = value; }
        public StateMachine FSM => fsm;
        
        // Start is called before the first frame update
        protected void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            fsm = new StateMachine();
            fsm.Add("Seed", new SeedState<PlantFSM>(this));
            fsm.Add("Grown", new GrownState<PlantFSM>(this));

            fsm.Initialize("Seed");
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            fsm.Update();
        }

        protected virtual void OnEnable()
        {
            if(GameManager.Instance != null) GameManager.Instance.plantsController.Plants.Add(this);
        }

        protected virtual void OnDisable()
        {
            if(GameManager.Instance != null) GameManager.Instance.plantsController.Plants.Remove(this);
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
            fsm.OnCollisionEnter(col);
        }

        protected virtual void OnCollisionStay(Collision col)
        {
            fsm.OnCollisionStay(col);
        }

        protected virtual void OnCollisionExit(Collision col)
        {
            fsm.OnCollisionExit(col);
        }

        protected virtual void OnDrawGizmos()
        {
            fsm.DrawGizmos();
        }
        
        public virtual void Kill()
        {
            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}