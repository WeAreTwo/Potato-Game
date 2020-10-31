using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PotatoGame
{

    [System.Serializable]
    public class PassiveAgentTree : BehaviourTree<AIController>
    {
        [SerializeField] protected RepeaterNode repeatRoot;
        
        [SerializeField] protected SelectorNode rootSelector;
        [SerializeField] protected SequenceNode followSequence;
        
        [SerializeField] protected SequenceNode checkPlayerSequence;
        [SerializeField] protected WaitForNode waitBeforeCheckingPlayer;
        [SerializeField] protected CheckForPlayerNode checkForPlayerNodeNearby;
        [SerializeField] protected FollowNode followPlayer;

        [SerializeField] protected SequenceNode moveToRandomPositionSequence;
        [SerializeField] protected WaitForNode waitBeforeMoving;
        [SerializeField] protected MoveToRandomNode moveToRandomPosition;

        [SerializeField] protected MoveToNode returnToTetherPosition;


        public PassiveAgentTree(AIController context) : base(context)
        {
            this.context = context;
        }

        public override void Initialize()
        {
            waitBeforeCheckingPlayer = new WaitForNode(context, 1.0f);
            checkForPlayerNodeNearby = new CheckForPlayerNode(context);
            
            checkPlayerSequence = new SequenceNode(
                    "Check Player Sequence",
                    waitBeforeCheckingPlayer,
                    checkForPlayerNodeNearby
            );
            
            followPlayer = new FollowNode(context);

            //follows the player branch
            followSequence = new SequenceNode(
                "Follow Player",
                checkPlayerSequence,
                followPlayer
            );
            
            //move to random position branch
            waitBeforeMoving = new WaitForNode(context, 3.5f);
            moveToRandomPosition = new MoveToRandomNode(context);
            moveToRandomPositionSequence = new SequenceNode(
                "Move Random Sequence",
                waitBeforeMoving,
                moveToRandomPosition
            );

            //return to tether branch
            returnToTetherPosition = new MoveToNode(context, context.destinationOne.transform.position);
            

            //selector branch
            rootSelector = new SelectorNode(
                    "Follow Selector",
                    followSequence,
                    moveToRandomPositionSequence,
                    returnToTetherPosition
                    
            );
            
            //root node 
            repeatRoot = new RepeaterNode(rootSelector);
        }

        public override void Run()
        {
            repeatRoot.TickNode();
        }

        public override void Debug()
        {
            base.Debug();
            
            repeatRoot.OnDebug("Root ###");
        }

        public override void DrawGizmos()
        {
            if(repeatRoot != null) repeatRoot.DrawGizmos();
        }
    }

}
