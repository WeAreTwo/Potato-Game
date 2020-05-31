using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshCollider))]       
    public class InteractableObject : MonoBehaviour, IPickUp
    {
        protected bool pickedUp;
        protected Rigidbody rb;

        public bool PickedUp { get => pickedUp; set => pickedUp = value; }
        public Rigidbody Rb { get => rb; set => rb = value; }

        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }
        
        public void PickUp()
        {
            //nothing for now
        }
    }

}