using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    public interface IPlantable
    {
        void PlantObject();
    }
    
    public interface IHarvestable
    {
        void Harvest();
    }
    
    public interface IPickUp
    {
        void PickUp();
    }
}