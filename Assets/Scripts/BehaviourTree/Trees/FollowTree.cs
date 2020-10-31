using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PotatoGame
{

    [System.Serializable]
    public class FollowTree : BehaviourTree<AIController>
    {
        [SerializeField] protected RepeaterNode repeatRoot;
        
        [SerializeField] protected SelectorNode followSelectorNode;
        [SerializeField] protected SequenceNode followSequenceNode;
        [SerializeField] protected SequenceNode testSequenceNode;
        
        [SerializeField] protected SequenceNode checkPlayerSequence;
        [SerializeField] protected WaitFor waitBeforeChecking;
        [SerializeField] protected CheckForPlayer checkForPlayer;

        [SerializeField] protected RepeaterNode moveCertainAmount;
        [SerializeField] protected SequenceNode moveToRandomPositionSequence;
        [SerializeField] protected WaitFor waitBeforeMoving;
        [SerializeField] protected PickRandomPosition pickRandomPosition;

        [SerializeField] protected MoveToNode returnToTether;
        
        [SerializeField] protected Follow followPlayer;
        
        public FollowTree(AIController context) : base(context)
        {
            this.context = context;
        }

        public override void Initialize()
        {
            waitBeforeChecking = new WaitFor(context, 1.0f);
            checkForPlayer = new CheckForPlayer(context);
            
            checkPlayerSequence = new SequenceNode(
                    "Check Player Sequence",
                    waitBeforeChecking,
                    checkForPlayer
            );
            
            followPlayer = new Follow(context);

            //follows the player branch
            followSequenceNode = new SequenceNode(
                "Follow Player",
                checkPlayerSequence,
                followPlayer
            );
            
            //move to random position branch
            waitBeforeMoving = new WaitFor(context, 3.5f);
            pickRandomPosition = new PickRandomPosition(context);
            moveToRandomPositionSequence = new SequenceNode(
                "Move Random Sequence",
                waitBeforeMoving,
                pickRandomPosition
            );
            
            moveCertainAmount = new RepeaterNode(
                moveToRandomPositionSequence, true, 3
                );

            //return to tether branch
            returnToTether = new MoveToNode(context, context.destinationOne.transform.position);
            
            //test 
            testSequenceNode = new SequenceNode(
                    "Test repeater",
                    returnToTether,
                    moveCertainAmount
                );

            //selector branch
            followSelectorNode = new SelectorNode(
                    "Follow Selector",
                    returnToTether,
                    moveCertainAmount
                    
            );
            
            //root node 
            repeatRoot = new RepeaterNode(testSequenceNode);
        }

        public override void Run()
        {
            repeatRoot.TickNode();
        }

        public override void DrawGizmos()
        {
            if(repeatRoot != null) repeatRoot.DrawGizmos();
        }
    }

}
