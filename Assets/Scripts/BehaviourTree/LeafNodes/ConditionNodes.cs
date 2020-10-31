using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    //Checks for item (used for testing)
    [System.Serializable]
    public class CheckForItem : ConditionNode<AIController>
    {
        public CheckForItem(AIController context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            if (true)
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
    
    //Checks for item nearby the agent
    [System.Serializable]
    public class CheckNearbyItem : ConditionNode<AIController>
    {
        public CheckNearbyItem(AIController context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {

            if (context.pickUpObject != null)
            {
                this.nodeStatus = NodeState.SUCCESS;
                return NodeState.SUCCESS;
            }
            else
            {
                Collider[] hitColliders = Physics.OverlapSphere(context.transform.position, context.pickUpRange);
                foreach (var col in hitColliders)
                {
                    if (col.TryGetComponent(out InteractableObject component))
                    {
                        context.pickUpObject = component.gameObject;
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

    [System.Serializable]
    public class CheckForPlayerNode : ConditionNode<AIController>
    {
        public CheckForPlayerNode(AIController context) : base(context)
        {
            this.context = context;
        }

        //Note; returning succes when target is not equal to null not working
        public override NodeState TickNode()
        {
            // Debug.Log(nodeStatus);

            /*
             * ifnequal to null
             *     checkforplayer()
             * else
             */
            
            // if (context.seekTarget != null)
            // {
            //     // this.nodeStatus = NodeState.SUCCESS;
            //     // return NodeState.SUCCESS;
            // }
            // else
            // {
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
            // }

            // this.nodeStatus = NodeState.RUNNING;
            // return NodeState.RUNNING;
        }
    }

}
