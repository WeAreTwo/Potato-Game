using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public abstract class Plant : InteractableObject, IPlantable
    {
        //Finite State Machine
        [SerializeField] protected PlantStates initState = PlantStates.SEED;

        [Header("GENERAL")]
        [SerializeField] protected float health = 100.0f;
        [SerializeField] protected bool planted;
        
        [Header("GROWTH CHARACTERISTICS")]
        [SerializeField] protected GrowthSettings growthSettings;

        //Properties
        public PlantStates InitState { get => initState; set => initState = value; }
        public float Health { get => health; set => health = value; }
        public bool Planted { get => planted; set => planted = value; }
        public GrowthSettings GrowthSettings { get => growthSettings; set => growthSettings = value; }

        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        protected virtual void OnEnable()
        {
            // if(GameManager.Instance != null) GameManager.Instance.plantsController.Plants.Add(this);
        }

        protected virtual void OnDisable()
        {
            // if(GameManager.Instance != null) GameManager.Instance.plantsController.Plants.Remove(this);
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
        }

        protected virtual void OnCollisionStay(Collision col)
        {
        }

        protected virtual void OnCollisionExit(Collision col)
        {
        }

        protected virtual void OnDrawGizmosSelected()
        {
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

        public virtual void PlantObject()
        {
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