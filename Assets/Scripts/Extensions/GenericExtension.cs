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
        
        public static bool IsType<T>(this Collider comparison) where T : class
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

        public static void SetColliderTrigger(this Collider col, bool value)
        {
            col.isTrigger = value;
        }
        
        public static void SetAllColliderTriggers(this GameObject obj, bool value)
        {
            foreach (Collider objectCollider in obj.GetComponents<Collider>())
                objectCollider.isTrigger = value;
        }

        public static void ThrowObject(this Rigidbody rb, Vector3 direction, float force)
        {
            rb.velocity = direction * force;
        } 
        
        public static void ThrowObject(this GameObject obj, Vector3 direction, float force)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.ActivatePhysics();
                rb.ThrowObject(direction, force);
            }
            
            obj.SetAllColliderTriggers(false);
        }
        
        public static void HoldObject(this GameObject obj, Transform parent)
        {
            obj.transform.SetParent(parent);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.DeActivatePhysics();
            }
            obj.SetAllColliderTriggers(true);
        }
    }
}