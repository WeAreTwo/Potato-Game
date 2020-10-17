﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
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
        protected int ticks = 0; //how many times it got updates in the update function
        
        protected bool onEnterCalled = false;
        protected bool onExitCalled = false;
        protected bool onOompleteCalled = false;
        
        public NodeState nodeStatus = NodeState.RUNNING;

        //constructor
        public Node() {}
        
        //called once when entered
        public virtual void OnEnterNode() 
        {
            onEnterCalled = true;
        }
        
        //main node function called in update
        //to determine succes,  fail, running
        public abstract NodeState TickNode();


        //called when exiting
        public virtual void OnExitNode() 
        {
            onExitCalled = true;
        }

        public virtual void OnReset()
        {
            ticks = 0;
        }
        
        public virtual void DrawGizmos() {}
    }

    //node with more than 1 node
    [System.Serializable]
    public abstract class CompositeNode : Node
    {
        [SerializeField] protected string compositeName;
        [SerializeField] protected List<Node> childNodes;

        public CompositeNode(string compositeName, params Node[] childNodes)
        {
            this.childNodes = childNodes.ToList();
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
    
    //leafs are responsible for changing the ai script so we need context
    [System.Serializable]
    public abstract class LeafNode<T> : Node where T : MonoBehaviour
    {
        protected T context;
        public LeafNode(T context)
        {
            this.context = context;
        }
    }

    [System.Serializable]
    public class SequenceNode : CompositeNode
    {
        [SerializeField] protected int currentNodeIndex = 0;

        public SequenceNode(string compositeName, params Node[] childNodes) : base(compositeName, childNodes)
        {
            this.childNodes = childNodes.ToList();
        }
        
        public override NodeState TickNode()
        {

            NodeState currentNodeState = childNodes[currentNodeIndex].TickNode();
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

    //selector is like an OR , it will return success if any children returns success
    //will not stop after a failure
    //usefull for describing a preferred set of behaviours
    [System.Serializable]
    public class SelectorNode : CompositeNode
    {
        [SerializeField] protected int currentNodeIndex = 0;

        public SelectorNode(string compositeName, params Node[] childNodes) : base(compositeName, childNodes)
        {
            this.childNodes = childNodes.ToList();
        }
        
        //will stop at the first instance of success, fail will increment
        public override NodeState TickNode()
        {

            NodeState currentNodeState = childNodes[currentNodeIndex].TickNode();
            switch(currentNodeState)
            {
                //will stop processing children the moment we return success
                case NodeState.SUCCESS:
                    return NodeState.SUCCESS;
                    break;
                case NodeState.RUNNING:
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    if (currentNodeIndex < childNodes.Count - 1) currentNodeIndex++;
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
    
    //will repeate node X number of times or forevere
    [System.Serializable]
    public class RepeaterNode : DecoratorNode
    {
        public bool repeatForever; //if the node will loop forever
        public int repeatTimes = 1; //if it wont repeat forever, for how many times
        
        public RepeaterNode(Node childNode, bool repeatForever = true, int repeatTimes = 1) : base(childNode)
        {
            this.repeatForever = repeatForever;
            this.repeatTimes = repeatTimes;
        }
        
        public override NodeState TickNode()
        {
            NodeState childNodeState;
            
            //if its forever cycle these nodes
            if (repeatForever)
            {
                childNodeState = childNode.TickNode();
                switch(childNodeState)
                {
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                        break;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                        break;
                    case NodeState.FAILURE:
                        return NodeState.FAILURE; 
                        break;
                }
            }
            
            //if not keep check for how many times it processed
            else if (ticks < repeatTimes)
            {
                childNodeState = childNode.TickNode();
                switch(childNodeState)
                {
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                        break;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                        break;
                    case NodeState.FAILURE:
                        return NodeState.FAILURE; 
                        break;
                }
            }

            return NodeState.SUCCESS;  //default return success
        }
    }

    //node with only 1 child, checks for condition
    [System.Serializable]
    public abstract class ConditionNode<T> : LeafNode<T> where T : MonoBehaviour
    {
        public ConditionNode(T context) : base(context)
        {
            this.context = context;
        }

        public override NodeState TickNode()
        {
            this.nodeStatus = CheckCondition();

            switch (this.nodeStatus)
            {
                case NodeState.SUCCESS:
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
    public abstract class ActionNode<T> : LeafNode<T> where T : MonoBehaviour
    {
        protected T context;
        
        public ActionNode(T context) : base(context)
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