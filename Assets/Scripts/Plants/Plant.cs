﻿using System;
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
    public abstract class Plant : MonoBehaviour, IPickUp, IPlantable
    {
        //Finite State Machine
        protected StateMachine fsm;
        
        [SerializeField] protected bool planting;
        [SerializeField] protected bool planted;
        [SerializeField] protected bool pickedUp;
        [SerializeField] protected Vector2 plantingDepthRange; // Range for the depth of the potato when planted
        
        [Header("HEALTH")] 
        [SerializeField] protected float health = 100.0f;
        [SerializeField] protected GrowthParams growthParams;


        //Components
        protected Rigidbody rb;

        //Properties
        public Rigidbody Rb { get => rb; set => rb = value; }
        public float Health { get => health; set => health = value; }
        public bool Planting { get => planting; set => planting = value; }
        public bool Planted { get => planted; set => planted = value; }
        public bool PickedUp { get => pickedUp; set => pickedUp = value; }
        public GrowthParams GrowthParams { get => growthParams; set => growthParams = value; }
        public StateMachine FSM => fsm;
        
        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            fsm = new StateMachine();
            fsm.Add(PlantStates.Seed.ToString(), new SeedState<Plant>(this));
            fsm.Add(PlantStates.Grown.ToString(), new GrownState<Plant>(this));

            fsm.Initialize(PlantStates.Seed.ToString());
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if(!pickedUp) fsm.Update();
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
            // Plant when in contact with the ground
            if (col.gameObject.tag == ProjectTags.Ground && planting)
                PlantObject();
            
            if(!pickedUp) fsm.OnCollisionEnter(col);
        }

        protected virtual void OnCollisionStay(Collision col)
        {
            if(!pickedUp) fsm.OnCollisionStay(col);
        }

        protected virtual void OnCollisionExit(Collision col)
        {
            if(!pickedUp) fsm.OnCollisionExit(col);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if(fsm != null) fsm.DrawGizmos();
        }
        
        public virtual void Kill()
        {
            //when the health is below 0
            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        public virtual void PlantObject()
        {
            // Pick a random depth number
            var depth = UnityEngine.Random.Range(plantingDepthRange.x, plantingDepthRange.y);

            // Deactivate gravity and freeze all
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Deactivate the colliders
            foreach (Collider objectCollider in GetComponents<Collider>())
                objectCollider.isTrigger = true;

            // Get the potato in the ground
            Vector3 currentPos = transform.position;
            currentPos.y -= depth;
            transform.position = currentPos;

            // The potato is now planted!
            planting = false;
            planted = true;
        }

        public virtual void PickUp()
        {
            //Put pick up code here 
        }

        void PlantedConfig()
        {
            
        }
        
        
    }
}