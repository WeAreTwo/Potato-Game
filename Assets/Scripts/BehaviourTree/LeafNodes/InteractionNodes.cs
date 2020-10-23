using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    [System.Serializable]
    public class PickUpItem : ConditionNode<BehaviourTreeAITest>
    {
        public PickUpItem(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            if (context.pickUpObject == null)
            {
                this.nodeStatus = NodeState.FAILURE;
                return NodeState.FAILURE;
            }
            
            if (Vector3.Distance(context.transform.position, context.pickUpObject.transform.position) < context.pickUpRange)
            {
                if (context.pickUpObject.TryGetComponent(out InteractableObject interactable))
                {
                    interactable.PickUp();
                    context.pickUpObject.HoldObjectAI(context.transform);
                    this.nodeStatus = NodeState.SUCCESS;
                    return NodeState.SUCCESS;
                }
                else
                {
                    this.nodeStatus = NodeState.FAILURE;
                    return NodeState.FAILURE;
                }
            }
            else
            {
                this.nodeStatus = NodeState.FAILURE;
                return NodeState.FAILURE;
            }
        }
    }       
    
    [System.Serializable]
    public class DropItem : ConditionNode<BehaviourTreeAITest>
    {

        public DropItem(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            if (context.pickUpObject && context.pickUpObject.TryGetComponent(out InteractableObject interactable))
            {
                interactable.Throw(context.transform.forward, 5.0f);
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
}