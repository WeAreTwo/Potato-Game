using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public static class GenericExtension 
    {
        //COMPONENT EXTENSION METHODS 
        public static bool IsType<T>(this GameObject comparison) where T : class
        {
            if (comparison.GetComponent<T>() != null)
                return true;
            else
                return false;
        }

        //RIGIDBODY EXTENSION METHODS
        public static void ActivatePhysics(this Rigidbody rb)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
        
        public static void DeActivatePhysics(this Rigidbody rb)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;               
        }

        public static void ThrowObject(this Rigidbody rb, Vector3 direction, float force)
        {
            rb.velocity = direction * force;
        }
    }
}