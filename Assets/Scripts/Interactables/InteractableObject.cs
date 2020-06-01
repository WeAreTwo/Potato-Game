using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshCollider))]       
    public class InteractableObject : MonoBehaviour, IPickUp
    {
        [SerializeField] protected bool pickedUp;
        protected Rigidbody rb;

        public bool PickedUp { get => pickedUp; set => pickedUp = value; }
        public Rigidbody Rb { get => rb; set => rb = value; }

        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }
        
        public virtual void PickUp()
        {
            //nothing for now
            pickedUp = true;
            rb.DeActivatePhysics();
        }

        public virtual void Drop()
        {
            pickedUp = false;
            rb.ActivatePhysics();
        }

        public virtual void Throw(Vector3 direction, float force)
        {
            pickedUp = false;
            this.gameObject.layer = 0; // bring back the default physic layer
            this.gameObject.ThrowObject(direction, force);
        }
    }

}