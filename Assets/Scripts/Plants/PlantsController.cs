using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class PlantsController : MonoBehaviour
    {
        //PLANTS LIST
        [SerializeField] protected List<PlantFSM> plants = new List<PlantFSM>();

        public List<PlantFSM> Plants
        {
            get => plants;
            set => plants = value;
        }

        protected void Awake()
        {
            plants.Clear();
            var scenePlants = FindObjectsOfType(typeof(PlantFSM));
            foreach (var p in scenePlants)
            {
                plants.Add((PlantFSM)p);
            }
        }
        
        /* TO DO:
         * ADD EVENT SYSTEM ONPLANTADD()
         * 
         */
    }

}