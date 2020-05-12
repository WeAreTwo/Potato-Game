﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
   
    public class PotatoFSM : PlantFSM
    {
        protected override void Start()
        {
            fsm = new StateMachine(this);
            fsm.Add("Seed", new SeedState(growthParams));
            fsm.Add("Grown", new GrownState(growthParams));
            fsm.Add("Autonomous", new AutonomousState(growthParams));
            
            fsm.Initialize("Seed");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        
    }


}
