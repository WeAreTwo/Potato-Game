using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [RequireComponent(typeof(MeshCollider))]       
    public abstract class InteractableStationary : MonoBehaviour
    {
        protected Collider col;
        protected virtual void Awake()
        {
            col = this.GetComponent<MeshCollider>();
        }

        // public abstract void Interact();
        public abstract void Interact(params object[] args);
    }

}