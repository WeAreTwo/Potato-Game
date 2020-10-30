using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PotatoGame
{

    [System.Serializable]
    public class FollowTree : BehaviourTree<BehaviourTreeAITest>
    {
        [SerializeField] protected RepeaterNode repeatRoot;
        
        [SerializeField] protected SelectorNode followSelectorNode;
        [SerializeField] protected SequenceNode followSequenceNode;
        
        [SerializeField] protected SequenceNode checkPlayerSequence;
        [SerializeField] protected WaitFor waitBeforeChecking;
        [SerializeField] protected CheckForPlayer checkForPlayer;
        
        [SerializeField] protected PickRandomPosition pickRandomPosition;
        
        [SerializeField] protected Follow followPlayer;
        
        public FollowTree(BehaviourTreeAITest context) : base(context)
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
            
            pickRandomPosition = new PickRandomPosition(context);
            
            followSequenceNode = new SequenceNode(
                "Follow Player",
                checkPlayerSequence,
                followPlayer
            );
            
            // followSelectorNode = new SelectorNode(
            //         "Follow Selector",
            //         followSequenceNode,
            //         new SequenceNode(
            //                 "Check Player While Moving",
            //                 new InverterNode(
            //                         new CheckForPlayer(context)
            //                     ),
            //                 new MoveToNode(context, context.destinationOne.transform.position)
            //             )
            // );            
            followSelectorNode = new SelectorNode(
                    "Follow Selector",
                    followSequenceNode,
                    pickRandomPosition,
                    new MoveToNode(context, context.destinationOne.transform.position)
            );
            
            repeatRoot = new RepeaterNode(followSelectorNode);
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
