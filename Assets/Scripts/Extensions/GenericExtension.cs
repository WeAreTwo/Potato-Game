using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PotatoGame
{

    public static class GenericExtension 
    {
        //COMPONENT EXTENSION METHODS 
        public static bool IsType<T>(this GameObject comparison) where T : class
        {
            return comparison.TryGetComponent(out T component);
        }        
        
        public static bool IsType<T>(this Collider comparison) where T : class
        {
            return comparison.TryGetComponent(out T component);
        }
        
        //RIGIDBODY EXTENSION METHODS
        public static void ActivatePhysics(this GameObject obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
                rb.gameObject.GetComponent<Collider>().isTrigger = false;
            }
        }
        
        public static void DeActivatePhysics(this GameObject obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;               
            }
        }
                
        public static void ActivatePhysics(this Rigidbody rb)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.gameObject.GetComponent<Collider>().isTrigger = false;
        }
        
        public static void DeActivatePhysics(this Rigidbody rb)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;               
        }
        
        public static void ActivatePlantingPhysics(this Rigidbody rb)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;               
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
        
        //AI will hold it over head
        public static void HoldObjectAI(this GameObject obj, Transform parent)
        {
            obj.transform.SetParent(parent);
            obj.transform.position = new Vector3(
                parent.transform.position.x, 
                obj.transform.position.y + 2.0f,
                parent.transform.position.z
            );
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.DeActivatePhysics();
            }
            obj.SetAllColliderTriggers(true);
        }
        
        //NAV MESH EXTENSION METHODS
        public static void SetNavSetting(this NavMeshAgent agent, NavSettings settings)
        {
            agent.baseOffset = settings.baseOffset;
            agent.speed = settings.speed;
            agent.angularSpeed = settings.angularSpeed;
            agent.acceleration = settings.acceleration;
            agent.stoppingDistance = settings.stoppingDistance;
            agent.autoBraking = settings.autoBraking;
            agent.radius = settings.radius;
            agent.height = settings.height;
            agent.avoidancePriority = settings.priority;
        }

        public static void StopNavigation(this NavMeshAgent agent)
        {
            agent.Stop();
            agent.ResetPath();
        }
    }
}