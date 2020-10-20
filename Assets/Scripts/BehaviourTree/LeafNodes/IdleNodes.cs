using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    //wait a certain amount of time 
    [System.Serializable]
    public class WaitFor : ActionNode<BehaviourTreeAITest>
    {
        [SerializeField] protected float waitTime = 5.0f;
        [SerializeField] protected float waitTimer = 0;
        
        public WaitFor(BehaviourTreeAITest context, float waitTime = 5.0f) : base(context)
        {
            this.context = context;
            this.waitTime = waitTime;
        }

        public override NodeState TickNode()
        {
            if (waitTimer >= waitTime)
            {
                this.nodeStatus = NodeState.SUCCESS;
                return NodeState.SUCCESS;
            }
            else
            {
                waitTimer += Time.deltaTime;
                this.nodeStatus = NodeState.RUNNING;
                return NodeState.RUNNING;
            }
        }

        public override void OnReset()
        {
            base.OnReset();
            waitTimer = 0;
        }
    }  
}