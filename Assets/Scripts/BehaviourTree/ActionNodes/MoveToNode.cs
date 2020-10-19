using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    //using interface to pass context , so we can remove constructor 
    [System.Serializable]
    public class MoveToNode : ActionNode<BehaviourTreeAITest>
    {

        [SerializeField] protected Vector3 destination;
        [SerializeField] protected float remainingDist;
        
        public MoveToNode(BehaviourTreeAITest context, Vector3 destination) : base(context)
        {
            this.context = context;
            this.destination = destination;
        }


        public override NodeState TickNode()
        {
            // Debug.Log("on move node");
            remainingDist = context.navAgent.remainingDistance;
            
            //if it doesnt have a path, set one
            if (!context.navAgent.hasPath)
            {
                context.navAgent.SetDestination(this.destination);
            }
            
            //as long as it nost stopped , keep running
            // if (context.navAgent.remainingDistance < 0.05f)
            if (Vector3.Distance(context.transform.position, this.destination) < 0.15f)
            {
                OnReset();
                this.nodeStatus = NodeState.SUCCESS;
                return NodeState.SUCCESS;
            }
            else
            {
                this.nodeStatus = NodeState.RUNNING;
                return NodeState.RUNNING;
            }
        }

        public override void OnReset()
        {
            context.navAgent.StopNavigation();
        }
    }
    
    [System.Serializable]
    public class Follow : ActionNode<BehaviourTreeAITest>
    {

        // [SerializeField] protected GameObject followTarget;
        [SerializeField] protected float followDistance;
        
        public Follow(BehaviourTreeAITest context, float followDistance = 10.0f) : base(context)
        {
            this.context = context;
            this.followDistance = followDistance;
        }


        public override NodeState TickNode()
        {
            //if the target ceizes to exist, then its fails
            if (context.seekTarget == null)
            {
                this.nodeStatus = NodeState.FAILURE;
                return NodeState.FAILURE;
            }
            
            //if it doesnt have a path, set one
            // if (!context.navAgent.hasPath)
            // {
                context.navAgent.SetDestination(context.seekTarget.transform.position);
            // }
            
            //if ur outside the followw distance, consider this succesfull
            if (Vector3.Distance(context.transform.position, context.seekTarget.transform.position) > followDistance)
            {
                context.navAgent.StopNavigation();
                this.nodeStatus = NodeState.FAILURE;
                return NodeState.FAILURE;
            }
            else
            {
                this.nodeStatus = NodeState.RUNNING;
                return NodeState.RUNNING;
            }
                return NodeState.RUNNING;
        }

        public override void OnReset()
        {
            context.navAgent.StopNavigation();
        }
    }
    
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