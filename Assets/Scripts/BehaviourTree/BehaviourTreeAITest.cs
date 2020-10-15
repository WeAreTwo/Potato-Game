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
        public GameObject destinationOne;
        public GameObject destinationTwo;
        public GameObject destinationThree;
        public GameObject destinationFour;
        
        [SerializeField] protected SequenceNode moveSequenceNode;
        [SerializeField] protected SequenceNode grabSword;
        
        [SerializeField] protected CheckForItem hasSwordCondition;
        [SerializeField] protected MoveToNode moveToOne;
        [SerializeField] protected MoveToNode moveToTwo;
        [SerializeField] protected MoveToNode moveToThree;
        [SerializeField] protected MoveToNode moveToFour;

        public NavMeshAgent navAgent;
        public bool hasPath;

        public bool hasSword = false;
        
        // Start is called before the first frame update
        void Start()
        {
            navAgent = this.GetComponent<NavMeshAgent>();
            
            //for each children, get component, set context (this)
            
            moveToOne = new MoveToNode(this, destinationOne.transform.position);
            moveToTwo = new MoveToNode(this, destinationTwo.transform.position);
            moveToThree = new MoveToNode(this, destinationThree.transform.position);
            moveToFour = new MoveToNode(this, destinationFour.transform.position);
            
            //initiation behaviour tree here
            grabSword = new SequenceNode("Grab Sword",
                new CheckForItem(this),
                new MoveToNode(this, destinationFour.transform.position)
            );
            
            moveSequenceNode = new SequenceNode("Move Sequence",
                    moveToOne,
                    moveToTwo,
                    moveToThree,
                    grabSword
                    );
        }

        // Update is called once per frame
        void Update()
        {
            hasPath = navAgent.hasPath;
            
            //call the behaviour tree tick
            moveSequenceNode.TickNode();



        }

        void OnDrawGizmos()
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
        }
    }

}
