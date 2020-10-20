using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class ReturnNode : ConditionNode<BehaviourTreeAITest>
    {
        [SerializeField] protected NodeState returnType;
        
        public ReturnNode(BehaviourTreeAITest context, NodeState returnType) : base(context)
        {
            this.context = context;
            this.returnType = returnType;
        }

        public override NodeState TickNode()
        {
            this.nodeStatus = returnType;
            Debug.Log(this.nodeStatus);
            return returnType;
        }
        
    }
}