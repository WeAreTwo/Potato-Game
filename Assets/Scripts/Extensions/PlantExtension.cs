using System;
using UnityEngine;

namespace PotatoGame
{
    public static class PlantExtension
    {
        public static void HarvestInit(this Plant plant)
        {
            plant.gameObject.ActivatePhysics();
            plant.Health = 100;
            plant.PickedUp = false;
            plant.Planted = false;
            plant.InitState = PlantStates.Seed;
        }
    }
}