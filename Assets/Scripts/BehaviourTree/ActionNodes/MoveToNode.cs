using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
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

}