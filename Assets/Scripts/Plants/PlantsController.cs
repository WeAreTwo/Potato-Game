using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class PlantsController : MonoBehaviour
    {
        //PLANTS LIST
        [SerializeField] protected List<Plant> plants = new List<Plant>();

        protected void Awake()
        {
            plants.Clear();
            var scenePlants = FindObjectsOfType(typeof(Plant));
            foreach (var p in scenePlants)
            {
                plants.Add((Plant)p);
            }
        }
    }

}