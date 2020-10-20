using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PotatoGame
{
    [System.Serializable]
    public class AITest : BehaviourTree<BehaviourTreeAITest>
    {

        [SerializeField] protected RepeaterNode root;
        
        [SerializeField] protected SequenceNode moveSequenceNode;
        [SerializeField] protected SequenceNode grabSword;
        
        [SerializeField] protected CheckForItem hasSwordCondition;
        [SerializeField] protected MoveToNode moveToOne;
        [SerializeField] protected MoveToNode moveToTwo;
        [SerializeField] protected MoveToNode moveToThree;
        [SerializeField] protected MoveToNode moveToFour;
        
        public AITest(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }
        
        public override void Initialize()
        {
            //for each children, get component, set context (this)
            
            moveToOne = new MoveToNode(context, context.destinationOne.transform.position);
            moveToTwo = new MoveToNode(context, context.destinationTwo.transform.position);
            moveToThree = new MoveToNode(context, context.destinationThree.transform.position);
            moveToFour = new MoveToNode(context, context.destinationFour.transform.position);
            
            //initiation behaviour tree here
            grabSword = new SequenceNode("Grab Sword",
                new CheckForItem(context),
                new MoveToNode(context, context.destinationFour.transform.position)
            );
            
            moveSequenceNode = new SequenceNode("Move Sequence",
                moveToOne,
                moveToTwo,
                moveToThree
                // grabSword
            );
            
            root = new RepeaterNode(
                moveSequenceNode, false, 3
            );
        }

        public override void Run()
        {
            root.TickNode();
        }
    }
    
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class BehaviourTreeAITest : MonoBehaviour
    {
        public NavMeshAgent navAgent;
        public bool hasPath;
        public bool hasSword = false;
        
        public float seekingRange = 1.5f;
        public GameObject seekTarget;
        
        public GameObject pickUpObject;
        public float pickUpRange = 2.5f;
        
        public GameObject destinationOne;
        public GameObject destinationTwo;
        public GameObject destinationThree;
        public GameObject destinationFour;

        [SerializeField] protected RepeaterNode repeatMoveSequence;
        
        [SerializeField] protected SequenceNode pickUpSequence;
        [SerializeField] protected CheckNearbyItem checkNearbyItem;
        [SerializeField] protected PickUpItem pickUpItem;
        [SerializeField] protected WaitFor waitFor;
        [SerializeField] protected DropItem dropItem;
        
        
        [SerializeField] protected SequenceNode followSequenceNode;
        [SerializeField] protected SequenceNode moveSequenceNode;
        [SerializeField] protected SelectorNode moveSelectorNode;
        [SerializeField] protected SequenceNode grabSword;
        
        [SerializeField] protected CheckForItem hasSwordCondition;
        [SerializeField] protected MoveToNode moveToOne;
        [SerializeField] protected MoveToNode moveToTwo;
        [SerializeField] protected MoveToNode moveToThree;
        [SerializeField] protected MoveToNode moveToFour;
        
        
        [SerializeField] protected ReturnNode returnFail;


        
        // Start is called before the first frame update
        void Start()
        {
            navAgent = this.GetComponent<NavMeshAgent>();
            
            //for each children, get component, set context (this)
            
            moveToOne = new MoveToNode(this, destinationOne.transform.position);
            moveToTwo = new MoveToNode(this, destinationTwo.transform.position);
            moveToThree = new MoveToNode(this, destinationThree.transform.position);
            moveToFour = new MoveToNode(this, destinationFour.transform.position);
            returnFail = new ReturnNode(this, NodeState.FAILURE);

            hasSwordCondition = new CheckForItem(this);
            
            //initiation behaviour tree here
            grabSword = new SequenceNode("Grab Sword",
                hasSwordCondition,
                moveToFour
            );
            
            moveSequenceNode = new SequenceNode("Move Sequence",
                    moveToOne,
                    moveToTwo,
                    moveToThree
                    // grabSword
                    );
                        
            followSequenceNode = new SequenceNode("Follow Player",
                    new CheckForPlayer(this),
                    new Follow(this)
            );
            
            // moveSelectorNode = new SelectorNode("Move Selector",
            //     grabSword,
            //     new ReturnNode(this, NodeState.FAILURE),
            //     new ReturnNode(this, NodeState.FAILURE),
            //         moveToThree
            //         );
            
            //will complete move sequence before it checks again for player to follow
            moveSelectorNode = new SelectorNode("Follow Selector",
    followSequenceNode,
                        moveToTwo
                    );
            
            checkNearbyItem = new CheckNearbyItem(this);
            pickUpItem = new PickUpItem(this);
            waitFor = new WaitFor(this);
            dropItem = new DropItem(this);
            pickUpSequence = new SequenceNode(
                "Pick Up Sequence",
                checkNearbyItem,
                // new WaitFor(this, 2.0f),
                pickUpItem,
                waitFor,
                dropItem
            
            );
            
            repeatMoveSequence = new RepeaterNode(
                pickUpSequence, false, 1
                );
        }

        // Update is called once per frame
        void Update()
        {
            hasPath = navAgent.hasPath;
            
            //call the behaviour tree tick
            repeatMoveSequence.TickNode();



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
            
            //seeking range
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.transform.position, seekingRange);
            
            //item pickup range 
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, pickUpRange);
        }
    }

}
