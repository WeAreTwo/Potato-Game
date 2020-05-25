using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class InteractableObject : MonoBehaviour, IPickUp
    {
        protected bool pickedUp;

        public bool PickedUp { get => pickedUp; set => pickedUp = value; }

        public void PickUp()
        {
            //nothing for now
        }
    }

}