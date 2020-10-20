using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class CheckForItem : ConditionNode<BehaviourTreeAITest>
    {
        public CheckForItem(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            if (context.hasSword)
            {
                this.nodeStatus = NodeState.SUCCESS;
                return NodeState.SUCCESS;
            }
            else
            {
                this.nodeStatus = NodeState.FAILURE;
                return NodeState.FAILURE;
            }
        }
    }

    [System.Serializable]
    public class CheckForPlayer : ConditionNode<BehaviourTreeAITest>
    {
        public CheckForPlayer(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            Debug.Log(nodeStatus);

            if (context.seekTarget != null)
            {
                this.nodeStatus = NodeState.SUCCESS;
                return NodeState.SUCCESS;
            }
            else
            {
                Collider[] hitColliders = Physics.OverlapSphere(context.transform.position, context.seekingRange);
                foreach (var col in hitColliders)
                {
                    if (col.TryGetComponent(out PlayerController component))
                    {
                        context.seekTarget = component.gameObject;
                        this.nodeStatus = NodeState.SUCCESS;
                        return NodeState.SUCCESS;
                    }
                }

                //if no player is found, return fail
                this.nodeStatus = NodeState.FAILURE;
                return NodeState.FAILURE;
            }

            this.nodeStatus = NodeState.RUNNING;
            return NodeState.RUNNING;
        }
    }

}
