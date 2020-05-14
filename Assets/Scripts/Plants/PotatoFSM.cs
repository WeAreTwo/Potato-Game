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

    [System.Serializable]
    public class PotatoCharacteristics
    {
        public Color color = Color.yellow;
        public float size = 1.0f;
        public float growthTime = 10.0f;
        public float longevity = 20.0f;
    }


    public class PotatoFSM : PlantFSM
    {
        [Header("POTATO PARTS")] 
        public GameObject potatoEyes;
        public GameObject eatingEffect;
        
        [Header("AUTONOMOUS AGENT")] 
        public PlantFSM victim;
        public float seekForce = 5.0f;
        
        protected override void Start()
        {
            fsm = new StateMachine();
            fsm.Add("Seed", new SeedState<PotatoFSM>(this));
            fsm.Add("Grown", new GrownState<PotatoFSM>(this));
            fsm.Add("Autonomous", new AutonomousState<PotatoFSM>(this));
            fsm.Add("Idle", new Idle<PotatoFSM>(this));
            fsm.Add("Move", new Move<PotatoFSM>(this));
            fsm.Add("Eat", new Eat<PotatoFSM>(this));
            
            fsm.Initialize("Autonomous");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        
    }


}
