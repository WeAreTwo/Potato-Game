using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    /*
     * Notes:
     * -Each node will return either succes, failure or running
     * -If any childrennode is still running, it (parent node) willl continue running
     * 
     */
    
    [System.Serializable]
    public enum NodeState
    {
        SUCCESS,
        FAILURE,
        RUNNING
    }

    [System.Serializable]
    public abstract class Node
    {
        protected bool onOtartCalled = false;
        protected bool onOxitCalled = false;
        protected bool onOompleteCalled = false;
        
        public NodeState nodeStatus = NodeState.RUNNING;

        //constructor
        public Node()
        {
            
        }
        
        //called once when entered
        public virtual void OnStart() 
        {
            onOtartCalled = true;
        }

        //called everyframe
        public virtual void OnUpdate()
        {
            
        }
        
        //called when exiting
        public virtual void OnExit() 
        {
            onOxitCalled = true;
        }
        
        //called when completed
        public virtual void OnComplete() 
        {
            onOompleteCalled = true;
        }
        
        public virtual void OnReset() {}

        //to determine succes,  fail, running
        public abstract NodeState TickNode();
    }

    [System.Serializable]
    public class SequenceNode : Node
    {
        [SerializeField] protected int currentNodeIndex = 0;
        [SerializeField] protected List<Node> childNodes;

        public SequenceNode(List<Node> childNodes)
        {
            this.childNodes = childNodes;
        }
        
        public override NodeState TickNode()
        {

            NodeState currentNodeState = childNodes[currentNodeIndex].TickNode();
            Debug.Log(currentNodeState);
            switch(currentNodeState)
            {
                case NodeState.SUCCESS:
                    if (currentNodeIndex < childNodes.Count - 1) currentNodeIndex++;
                    break;
                case NodeState.RUNNING:
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    return NodeState.FAILURE; //need to do something when it fails otherwise it repeats
                    break;
            }

            if (currentNodeIndex == childNodes.Count - 1)
            {
                this.nodeStatus = NodeState.SUCCESS;
                return NodeState.SUCCESS;
            }

            this.nodeStatus = NodeState.RUNNING;
            return NodeState.RUNNING;
        }
    }
    
    //node with only 1 child
    [System.Serializable]
    public abstract class DecoratorNode : Node
    {
        [SerializeField] protected Node childNode;

        public DecoratorNode(Node childNode)
        {
            this.childNode = childNode;
        }
    }
    
    //node with only 1 child, checks for condition
    [System.Serializable]
    public abstract class ConditionNode<T> : DecoratorNode where T : MonoBehaviour
    {
        protected T context;
        
        public ConditionNode(Node childNode, T context) : base(childNode)
        {
            this.childNode = childNode;
            this.context = context;
        }

        public override NodeState TickNode()
        {
            this.nodeStatus = CheckCondition();

            switch (this.nodeStatus)
            {
                case NodeState.SUCCESS:
                    childNode.TickNode();
                    return NodeState.SUCCESS;
                    break;
                case NodeState.RUNNING:
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    return NodeState.FAILURE; //need to do something when it fails otherwise it repeats
                    break;
            }

            return NodeState.RUNNING;
            // return this.nodeStatus;
        }

        public abstract NodeState CheckCondition();
    }
    
    //end node at the very end, behaviour is here
    [System.Serializable]
    public abstract class ActionNode<T> : Node where T : MonoBehaviour
    {
        protected T context;
        
        public ActionNode(T context)
        {
            this.context = context;
        }

        //behaviour goes here
        public override NodeState TickNode()
        {
            return this.nodeStatus;
        }
        
        // switch () {
        //     case NodeState.SUCCESS:
        //     this.nodeStatus = NodeState.SUCCESS;
        //     return this.nodeStatus;
        //     case NodeState.FAILURE:
        //     this.nodeStatus = NodeState.FAILURE;
        //     return this.nodeStatus;
        //     case NodeState.RUNNING:
        //     this.nodeStatus = NodeState.RUNNING;
        //     return this.nodeStatus;
        //     default:
        //     this.nodeStatus = NodeState.FAILURE;
        //     return this.nodeStatus;
        // }
    }
    

    // [System.Serializable]
    // public class DebugLeaf : Leaf
    // {
    //     
    // }
    
    public class BehaviourTree : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}