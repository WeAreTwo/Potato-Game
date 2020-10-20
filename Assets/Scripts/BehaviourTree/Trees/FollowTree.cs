using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PotatoGame
{

    [System.Serializable]
    public class FollowTree : BehaviourTree<BehaviourTreeAITest>
    {
        [SerializeField] protected SequenceNode followSequenceNode;
        [SerializeField] protected CheckForPlayer checkForPlayer;
        [SerializeField] protected Follow followPlayer;
        
        public FollowTree(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }

        public override void Initialize()
        {
            followSequenceNode = new SequenceNode(
                "Follow Player",
                checkForPlayer,
                followPlayer
            );
        }

        public override void Run()
        {
            followSequenceNode.TickNode();
        }
    }

}
