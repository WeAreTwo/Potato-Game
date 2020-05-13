using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    
    [System.Serializable]
    public enum PlantStates
    {
        Seed,
        Grown,
        Autonomous,
        Idle,
        Move,
        Eat
    }
   
    public class PotatoFSM : PlantFSM
    {
        [Header("POTATO PARTS")] 
        public GameObject potatoEyes;
        public GameObject eatingEffect;
        
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
