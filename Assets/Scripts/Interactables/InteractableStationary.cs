using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [RequireComponent(typeof(MeshCollider))]       
    [RequireComponent(typeof(Rigidbody))]       
    public abstract class InteractableStationary : MonoBehaviour
    {
        protected Collider col;
        protected Rigidbody rb;
        protected virtual void Awake()
        {
            col = this.GetComponent<MeshCollider>();
            rb = this.GetComponent<Rigidbody>();
            // rb.isKinematic = false;
            rb.DeActivatePhysics();
        }

        // public abstract void Interact();
        public abstract void Interact();


        // private void OnTriggerEnter(Collider other)
        // {
        //     if(other.IsType<ActionController>())
        //         Debug.Log("Action is near");            
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     if(other.IsType<ActionController>())
        //         Debug.Log("Action is exit");
        // }
    }

}