using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public abstract class BehaviourTree<T> where T : MonoBehaviour
    {
        protected T context;

        public BehaviourTree(T context)
        {
            this.context = context;
        }
        
        public abstract void Initialize();
        public abstract void Run();
    }
    
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
        [SerializeField] protected int ticks = 0; //how many times it got updates in the update function
        
        protected bool onEnterCalled = false;
        protected bool onExitCalled = false;
        protected bool onCompleteCalled = false;
        
        public NodeState nodeStatus = NodeState.RUNNING;
        
        //todo gotta update the ticks , maybe subcribe method OnTickNode()?

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
            nodeStatus = NodeState.RUNNING;
            ticks = 0;
        }

        public virtual void OnDebug()
        {
            Debug.Log("im called");
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
        
        //reset every child node
        public override void OnReset()
        {
            base.OnReset();

            foreach (Node child in childNodes)
            {
                child.OnReset();
            }
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
        
        //reset child node
        public override void OnReset()
        {
            base.OnReset();
            childNode.OnReset();
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

    //in order for the sequence node to return success, ALL child nodes need to return success
    //if one of them doesnt return sucess , the sequence return failure 
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
            //if it failed once, terminate sequence 
            if (this.nodeStatus == NodeState.FAILURE) return NodeState.FAILURE;

            //else continue sequence
            NodeState currentNodeState = childNodes[currentNodeIndex].TickNode();
            switch(currentNodeState)
            {
                case NodeState.SUCCESS:
                    if (currentNodeIndex < childNodes.Count - 1) currentNodeIndex++;
                    else if (currentNodeIndex == childNodes.Count - 1)
                    {
                        this.nodeStatus = NodeState.SUCCESS;
                        return NodeState.SUCCESS;
                    }
                    break;
                case NodeState.RUNNING:
                    this.nodeStatus = NodeState.RUNNING;
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    this.nodeStatus = NodeState.FAILURE;
                    return NodeState.FAILURE; //need to do something when it fails otherwise it repeats
                    break;
            }

            this.nodeStatus = NodeState.RUNNING; //default is running status
            return NodeState.RUNNING;
        }
        
        //reset the index
        public override void OnReset()
        {
            base.OnReset();
            currentNodeIndex = 0;
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
            // if (this.nodeStatus == NodeState.FAILURE) return NodeState.FAILURE; //if its already fail, return fail right away
            
            NodeState currentNodeState = childNodes[currentNodeIndex].TickNode();
            switch(currentNodeState)
            {
                //will stop processing children the moment we return success
                case NodeState.SUCCESS:
                    this.nodeStatus = NodeState.SUCCESS;
                    return NodeState.SUCCESS;
                    break;
                case NodeState.RUNNING:
                    this.nodeStatus = NodeState.RUNNING;
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    if (currentNodeIndex < childNodes.Count - 1) currentNodeIndex++;
                    else if (currentNodeIndex == childNodes.Count - 1)
                    {
                        this.nodeStatus = NodeState.FAILURE;
                        return NodeState.FAILURE;
                    }
                    break;
            }
            
            this.nodeStatus = NodeState.RUNNING;
            return NodeState.RUNNING;
        }
        
        //reset the index
        public override void OnReset()
        {
            base.OnReset();
            currentNodeIndex = 0;
        }
    }
    
    //will repeate node X number of times or forevere
    [System.Serializable]
    public class RepeaterNode : DecoratorNode
    {
        public bool repeatForever; //if the node will loop forever
        [SerializeField] protected int currentCycle = 0;
        public int repeatCycles = 1; //if it wont repeat forever, for how many times
        
        public RepeaterNode(Node childNode, bool repeatForever = true, int repeatCycles = 1) : base(childNode)
        {
            this.repeatForever = repeatForever;
            this.repeatCycles = repeatCycles;
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
                        currentCycle++;
                        childNode.OnReset();
                        return NodeState.SUCCESS;
                        break;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                        break;
                    case NodeState.FAILURE:
                        currentCycle++;
                        childNode.OnReset();
                        return NodeState.FAILURE; 
                        break;
                }
            }
            
            //if not keep check for how many times it processed
            else if (currentCycle < repeatCycles)
            {
                childNodeState = childNode.TickNode();
                switch(childNodeState)
                {
                    case NodeState.SUCCESS:
                        currentCycle++;
                        childNode.OnReset();
                        return NodeState.SUCCESS;
                        break;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                        break;
                    case NodeState.FAILURE:
                        currentCycle++;
                        childNode.OnReset();
                        return NodeState.FAILURE; 
                        break;
                }
            }

            return NodeState.SUCCESS;  //default return success
        }

        public override void OnReset()
        {
            base.OnReset();
            currentCycle = 0;
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

        // public override NodeState TickNode()
        // {
        //     // this.nodeStatus = CheckCondition();
        //
        //     switch (this.nodeStatus)
        //     {
        //         case NodeState.SUCCESS:
        //             return NodeState.SUCCESS;
        //             break;
        //         case NodeState.RUNNING:
        //             return NodeState.RUNNING;
        //             break;
        //         case NodeState.FAILURE:
        //             return NodeState.FAILURE; //need to do something when it fails otherwise it repeats
        //             break;
        //     }
        //
        //     return NodeState.RUNNING;
        //     // return this.nodeStatus;
        // }

        // public abstract NodeState CheckCondition();
    }
    
    //end node at the very end, behaviour is here
    [System.Serializable]
    public abstract class ActionNode<T> : LeafNode<T> where T : MonoBehaviour
    {
        public ActionNode(T context) : base(context)
        {
            this.context = context;
        }
    }
    

    // [System.Serializable]
    // public class DebugLeaf : Leaf
    // {
    //     
    // }

}