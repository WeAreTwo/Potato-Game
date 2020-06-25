using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    public interface IPlantable
    {
        bool Planted { get; set; }
        void PlantObject();
    }
    
    public interface IHarvestable
    {
        void Harvest();
    }
    
    public interface IPickUp
    {
        bool PickedUp { get; set; }
        void PickUp();
    }
}