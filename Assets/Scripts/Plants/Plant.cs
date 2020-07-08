using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{ 
    
    [System.Serializable]
    public class GrowthCharacteristics
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
        public float growthCompletionTime = 12.0f;      //time where it finished growing
        
        //HARVEST PARAMS 
        [Header("HARVEST")]
        public float harvestTime = 0.0f; 
        public float harvestPeriod = 15.0f; //second (amount of time before it before you cant harvest it anymore)
        public int harvestYield = 2; //how many seeds your gonna get out of this 

        [Header("VISUAL")] 
        [SerializeField] protected Color currentColor;
        public Color seedColor;
        public Color grownColor;


        public void Update()
        {
            
        }
        
        public void ShiftSize(ref Component component)
        {
            float dt = 1;
            // component.transform.localPosition = Vector3.Lerp();
        }
        
        public void ShiftColor()
        {
            float dt = growthTime/growthCompletionTime;
            currentColor = Color.Lerp(seedColor, grownColor, dt);
        }


    }
    
    public abstract class Plant : InteractableObject, IPlantable
    {
        //Finite State Machine
        [Header("FSM PARAMS")]
        protected StateMachine fsm;
        [SerializeField] protected PlantStates initState = PlantStates.Seed;

        [Header("GENERAL")]
        [SerializeField] protected float health = 100.0f;
        [SerializeField] protected bool planted;
        
        [Header("GROWTH CHARACTERISTICS")]
        [SerializeField] protected GrowthCharacteristics growthCharacteristics;

        //Properties
        public StateMachine FSM => fsm;
        public PlantStates InitState { get => initState; set => initState = value; }
        public float Health { get => health; set => health = value; }
        public bool Planted { get => planted; set => planted = value; }
        public GrowthCharacteristics GrowthCharacteristics { get => growthCharacteristics; set => growthCharacteristics = value; }

        protected virtual void Start()
        {
            fsm = new StateMachine();
            fsm.Add(PlantStates.Seed, new SeedState<Plant>(this));
            fsm.Add(PlantStates.Grown, new GrownState<Plant>(this));

            fsm.Initialize(initState);
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

        public override void PickUp()
        {
            base.PickUp();
            planted = false;
        }

        public void PlantObject()
        {
            throw new NotImplementedException();
        }

        public virtual void PlantObject(Vector3 plantingPosition)
        {
            this.gameObject.DeActivatePhysics();
            this.transform.position = plantingPosition;

            // The potato is now planted!
            planted = true;
            pickedUp = false;
        }

        public virtual void Harvest()
        {
            
        }
    }
}