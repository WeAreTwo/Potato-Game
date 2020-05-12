using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    public abstract class PlantFSM : MonoBehaviour
    {
        [Header("HEALTH")] [SerializeField] protected float health = 100.0f;
        
        [SerializeField] protected GrowthParams growthParams;
        [SerializeField] protected StateMachine<PlantStates> fsm;

        public StateMachine<PlantStates> FSM => fsm;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            fsm = new StateMachine<PlantStates>(this);
            fsm.Add("Seed", new SeedState(growthParams));
            fsm.Add("Grown", new GrownState(growthParams));
            
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