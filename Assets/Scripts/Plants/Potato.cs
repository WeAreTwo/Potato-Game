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


    public class Potato : Plant
    {
        [Header("POTATO PARTS")] 
        public GameObject potatoEyes;
        public GameObject eatingEffect;
        
        [Header("AUTONOMOUS AGENT")] 
        public Plant victim;
        public float seekRange = 5.0f;
        public float seekForce = 5.0f;
        
        protected override void Start()
        {
            fsm = new StateMachine();
            fsm.Add("Seed", new SeedState<Potato>(this));
            fsm.Add("Grown", new GrownState<Potato>(this));
            fsm.Add("Autonomous", new AutonomousState<Potato>(this));
            fsm.Add("Idle", new Idle<Potato>(this));
            fsm.Add("Move", new Move<Potato>(this));
            fsm.Add("Eat", new Eat<Potato>(this));
            
            fsm.Initialize("Seed");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        
    }


}
