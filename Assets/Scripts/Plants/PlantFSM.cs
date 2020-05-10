using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    public abstract class PlantFSM : MonoBehaviour
    {
        [SerializeField] protected GrowthParams growthParams;
        [SerializeField] protected StateMachine states;
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            states = new StateMachine(this);
            states.Add("Seed", new SeedState(growthParams));
            states.Add("Grown", new GrownState(growthParams));
            states.Add("Autonomous", new AutonomousState());
            
            states.Initialize("Seed");
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            states.Update();
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
            states.OnCollisionEnter(col);
        }

        protected virtual void OnCollisionStay(Collision col)
        {
            states.OnCollisionStay(col);
        }

        protected virtual void OnCollisionExit(Collision col)
        {
            states.OnCollisionExit(col);
        }

        protected virtual void OnDrawGizmos()
        {
            states.DrawGizmos();
        }
    }
}