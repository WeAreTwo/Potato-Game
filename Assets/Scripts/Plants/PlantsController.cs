using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class PlantsController : MonoBehaviour
    {
        //PLANTS LIST
        [SerializeField] protected List<PotatoAI> autonomousPotatoes = new List<PotatoAI>();
        [SerializeField] protected List<PlantFSM> plants = new List<PlantFSM>();

        #region Properties
        public List<PotatoAI> AutonomousPotatoes { get => autonomousPotatoes; set => autonomousPotatoes = value; }
        public List<PlantFSM> Plants { get => plants; set => plants = value; }
        #endregion

        protected void Awake()
        {
            plants.Clear();
            var scenePlants = FindObjectsOfType(typeof(PlantFSM));
            foreach (var p in scenePlants)
            {
                plants.Add((PlantFSM)p);
            }
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                foreach (var potato in autonomousPotatoes)
                {
                    potato.Fsm.Current.TriggerExit(PotatoStates.Look);
                }
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                foreach (var potato in autonomousPotatoes)
                {
                    potato.Fsm.Current.TriggerExit(PotatoStates.MoveToBell);
                }
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                foreach (var potato in autonomousPotatoes)
                {
                    potato.Fsm.Current.TriggerExit(PotatoStates.Idle);
                }
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                foreach (var potato in autonomousPotatoes)
                {
                    potato.Fsm.Current.TriggerExit(PotatoStates.Follow);
                }
            }

        }

        /* TO DO:
         * ADD EVENT SYSTEM ONPLANTADD()
         * 
         */
    }

}