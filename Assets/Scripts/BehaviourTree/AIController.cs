using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PotatoGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MovementBase
    {
        [Header("Context Parameters")]
        public float health = 100.0f;
        public NavMeshAgent navAgent;
        public bool hasPath;
        
        [Header("Seeking Parameters")]
        public float seekingRange = 6.5f;
        public GameObject seekTarget;
        public Vector3 seekPosition;
        
        [Header("Pickup Parameters")]
        public GameObject pickUpObject;
        public float pickUpRange = 1.5f;
        
        //these places could prob be in something like a biome manager?
        [Header("Destinations")]
        public GameObject destinationOne;
        public GameObject destinationTwo;
        public GameObject destinationThree;
        public GameObject destinationFour;

        //ANIMATION
        //navmesh velocity difference (to see how much the AI moved)
        protected float velocityTimer = 0;
        protected float velocityRecordDelay = 0.1f;
        protected Vector3 lastFrameVelocity;
        protected float speedDifference;
        protected float speedAnimThreshold = 0.009f;
        
        
        protected override void Awake()
        {
            base.Awake();
            navAgent = this.GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            CheckHealth();
            hasPath = navAgent.hasPath;
            
        }
        
        protected virtual void CheckHealth()
        {
            //if health depletes, die
            if(health <= 0.0f)
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (destinationOne && destinationTwo && destinationThree && destinationFour)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(destinationOne.transform.position, 1.0f);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(destinationTwo.transform.position, 1.0f);
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(destinationThree.transform.position, 1.0f);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(destinationFour.transform.position, 1.0f);
            }
            
            //seeking range
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.transform.position, seekingRange);
            
            //seeking target
            if (seekTarget)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(this.transform.position, seekTarget.transform.position);
            }

            //item pickup range 
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, pickUpRange);
        }
    }

}
