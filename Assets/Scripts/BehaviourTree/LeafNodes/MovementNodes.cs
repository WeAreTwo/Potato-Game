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
    public class PickRandomPosition : ActionNode<BehaviourTreeAITest>
    {
        public PickRandomPosition(BehaviourTreeAITest context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            
            float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
            float randY = UnityEngine.Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX, 0, randY);
            Vector3 currentPosXZ = new Vector3(context.transform.position.x, 0, context.transform.position.z);
            context.seekPosition = currentPosXZ + (randomPos * context.seekingRange);

            this.nodeStatus = NodeState.SUCCESS;
            return NodeState.SUCCESS;
        }
    }

}