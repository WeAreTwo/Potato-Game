using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    
    [System.Serializable]
    public enum PlantStates
    {
        SEED,
        GROWN,
        AUTONOMOUS,
        IDLE,
        MOVE,
        MOVE_TO_BELL,
        EAT
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
        public float seekRange = 5.0f;
        public float seekForce = 5.0f;
        
        protected override void Start()
        {
            fsm = new StateMachine();
            fsm.Add(PlantStates.SEED, new SeedState<PotatoFSM>(this));
            fsm.Add(PlantStates.GROWN, new GrownState<PotatoFSM>(this));
            fsm.Add(PlantStates.AUTONOMOUS, new AutonomousState<PotatoFSM>(this));
            fsm.Add(PlantStates.IDLE, new Idle<PotatoFSM>(this));
            fsm.Add(PlantStates.MOVE, new Move<PotatoFSM>(this));
            fsm.Add(PlantStates.EAT, new Eat<PotatoFSM>(this));
            
            fsm.Initialize(initState);
        }
        
    }


}
