using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
   
    public class PotatoFSM : PlantFSM
    {
        protected override void Start()
        {
            states = new StateMachine(this);
            states.Add("Seed", new SeedState(growthParams));
            states.Add("Grown", new GrownState(growthParams));
            states.Add("Autonomous", new AutonomousState(growthParams));
            
            states.Initialize("Seed");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        
    }


}
