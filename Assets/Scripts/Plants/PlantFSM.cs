using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public abstract class PlantFSM : InteractableObject, IPlantable
    {
        //Finite State Machine
        [Header("FSM PARAMS")]
        protected StateMachine fsm;
        [SerializeField] protected PlantStates initState = PlantStates.SEED;

        [Header("GENERAL")]
        [SerializeField] protected float health = 100.0f;
        [SerializeField] protected bool planted;
        
        [Header("GROWTH CHARACTERISTICS")]
        [SerializeField] protected GrowthSettings growthSettings;

        //Properties
        public StateMachine FSM => fsm;
        public PlantStates InitState { get => initState; set => initState = value; }
        public float Health { get => health; set => health = value; }
        public bool Planted { get => planted; set => planted = value; }
        public GrowthSettings GrowthSettings { get => growthSettings; set => growthSettings = value; }

        protected virtual void Start()
        {
            fsm = new StateMachine();
            fsm.Add(PlantStates.SEED, new SeedState<PlantFSM>(this));
            fsm.Add(PlantStates.GROWN, new GrownState<PlantFSM>(this));

            fsm.Initialize(initState);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if(!pickedUp) fsm.Update();
        }

        protected virtual void OnEnable()
        {
            if(GameManager.Instance != null) GameManager.Instance.plantsControllerFsm.Plants.Add(this);
        }

        protected virtual void OnDisable()
        {
            if(GameManager.Instance != null) GameManager.Instance.plantsControllerFsm.Plants.Remove(this);
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