using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
   
    public class PotatoFSM : PlantFSM
    {
        [Header("POTATO PARTS")] 
        [SerializeField] protected GameObject potatoEyes;
        [SerializeField] protected GameObject eatingEffect;
        
        protected override void Start()
        {
            fsm = new StateMachine();
            fsm.Add("Seed", new SeedState<PotatoFSM>(this));
            fsm.Add("Grown", new GrownState<PotatoFSM>(this));
            fsm.Add("Autonomous", new AutonomousState<PotatoFSM>(this));
            fsm.Add("Idle", new Idle<PotatoFSM>(this));
            fsm.Add("Move", new Move<PotatoFSM>(this));
            fsm.Add("Eat", new Eat<PotatoFSM>(this));
            
            fsm.Initialize("Seed");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        
    }


}
