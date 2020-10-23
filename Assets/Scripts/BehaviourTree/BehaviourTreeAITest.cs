using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PotatoGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BehaviourTreeAITest : MonoBehaviour
    {
        [Header("Context Parameters")]
        public NavMeshAgent navAgent;
        public bool hasPath;
        public bool hasSword = false;
        
        public float seekingRange = 1.5f;
        public GameObject seekTarget;
        public Vector3 seekPosition;
        
        public GameObject pickUpObject;
        public float pickUpRange = 2.5f;
        
        public GameObject destinationOne;
        public GameObject destinationTwo;
        public GameObject destinationThree;
        public GameObject destinationFour;

        [Header("Behaviour Trees")]
        [SerializeField] protected PickItemTree pickUpTree;
        [SerializeField] protected MoveTree moveTree;
        [SerializeField] protected FollowTree followTree;
        
        // Start is called before the first frame update
        protected void Start()
        {
            navAgent = this.GetComponent<NavMeshAgent>();
            
            //picking and dropping behaviour 
            pickUpTree = new PickItemTree(this); //pass the context first
            pickUpTree.Initialize(); //Initialize Tree
            
            moveTree = new MoveTree(this); //pass the context first
            moveTree.Initialize(); //Initialize Tree
            
            followTree = new FollowTree(this); //pass the context first
            followTree.Initialize(); //Initialize Tree
            
            
        }

        // Update is called once per frame
        protected void Update()
        {
            hasPath = navAgent.hasPath;
            
            //call the behaviour tree tick
            pickUpTree.Run(); //update the tree
            // moveTree.Run();
            // followTree.Run();
        }

        protected void OnDrawGizmos()
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
            
            //item pickup range 
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, pickUpRange);
        }
    }

}
