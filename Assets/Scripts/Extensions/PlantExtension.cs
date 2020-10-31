using System;
using UnityEngine;

namespace PotatoGame
{
    public static class PlantExtension
    {
        public static void HarvestInit(this PlantFSM plantFsm)
        {
            plantFsm.gameObject.ActivatePhysics();
            plantFsm.Health = 100;
            plantFsm.PickedUp = false;
            plantFsm.Planted = false;
            plantFsm.InitState = PlantStates.SEED;
        }
        public static void HarvestInit(this Plant plantFsm)
        {
            plantFsm.gameObject.ActivatePhysics();
            plantFsm.Health = 100;
            plantFsm.PickedUp = false;
            plantFsm.Planted = false;
            plantFsm.InitState = PlantStates.SEED;
        }
    }
}