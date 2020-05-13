using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [RequireComponent(typeof(Rigidbody))]        //automatically add rb
    [RequireComponent(typeof(MeshCollider))]    //automatically add meshcollider        
    public abstract class PlantFSM : MonoBehaviour
    {
        [Header("HEALTH")] [SerializeField] protected float health = 100.0f;
        
        [SerializeField] protected GrowthParams growthParams;
        [SerializeField] protected StateMachine fsm;

        //Components
        protected Rigidbody rb;

        public Rigidbody Rb
        {
            get => rb;
            set => rb = value;
        }

        public GrowthParams GrowthParams => growthParams;
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
        
        protected virtual void Die()
        {
            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}