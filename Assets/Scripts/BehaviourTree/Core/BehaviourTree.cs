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

        public virtual void Debug(){}
        public virtual void DrawGizmos(){}
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
        public string nodeName;
        
        [SerializeField] protected int ticks = 0; //how many times it got updates in the FailSafe function
        [SerializeField] protected int failThreshold = 1000;

        protected bool onEnterCalled = false;
        protected bool onExitCalled = false;
        
        public NodeState nodeStatus = NodeState.RUNNING;
        
        //constructor
        public Node()
        {
            // automatically assign name based on class name
            string nodeName = this.GetType().ToString();
            nodeName = nodeName.Replace("PotatoGame.", "");
            this.nodeName = nodeName;
        }
        
        //called once when entered (use in ticknode function)
        public virtual void OnEnterNode() 
        {
            if(onEnterCalled) return;
            onEnterCalled = true;
        }
        
        //main node function called in update
        //to determine succes,  fail, running
        public abstract NodeState TickNode();
        
        //for when it gets stuck, it will start counting tick (update calls) and reset once its past the threshold
        public virtual bool TriggerFailSafe()
        {
            ticks++;
            if (ticks > failThreshold)
                return true;
            else
                return false;
        }
        
        //called when exiting
        public virtual void OnExitNode()
        {
            if (onExitCalled) return; //return if its already been called 
            onExitCalled = true;
        }

        public virtual void OnReset()
        {
            onEnterCalled = false;        //RESET ALL THE CALL FUNCTIONS 
            onExitCalled = false;
            nodeStatus = NodeState.RUNNING;
            ticks = 0;
        }

        public virtual void OnDebug(string link)
        {
        }

        public virtual void OnCurrentNodePath() {}
        
        public virtual void DrawGizmos() {}
    }

    //node with more than 1 node
    [System.Serializable]
    public abstract class CompositeNode : Node
    {
        [SerializeField] protected string compositeName;
        [SerializeField] protected List<Node> childNodes;
        [SerializeField] protected int currentNodeIndex = 0;

        public CompositeNode(string compositeName, params Node[] childNodes)
        {
            this.compositeName = compositeName;
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

        public override void OnDebug(string link)
        {
            base.OnDebug(link);

            // foreach (Node child in childNodes)
            // {
            //     child.OnDebug($"{link} {nodeName} >> ");
            // }

            childNodes[currentNodeIndex].OnDebug($"{link} {nodeName} >> ");
            
        }
        
        
        public override void DrawGizmos()
        {
            base.DrawGizmos();

            foreach (Node child in childNodes)
            {
                if(child != null) child.DrawGizmos();
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
        
        public override void OnDebug(string link)
        {
            base.OnDebug(link);
            
            childNode.OnDebug($"{link} {nodeName} ** ");
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if(childNode != null) childNode.DrawGizmos();
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
        
        public override void OnDebug(string link)
        {
            base.OnDebug(link);
            
            Debug.Log($"{link} {nodeName} :: ");
        }
    }

    //in order for the sequence node to return success, ALL child nodes need to return success
    //if one of them doesnt return sucess , the sequence return failure 
    [System.Serializable]
    public class SequenceNode : CompositeNode
    {
        
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
        
        public SelectorNode(string compositeName, params Node[] childNodes) : base(compositeName, childNodes)
        {
            this.childNodes = childNodes.ToList();
        }
        
        //will stop at the first instance of success, fail will increment
        public override NodeState TickNode()
        {
            // if (this.nodeStatus == NodeState.FAILURE) return NodeState.FAILURE; //if its already fail, return fail right away
            
            NodeState currentNodeState = childNodes[currentNodeIndex].TickNode();
            // foreach (Node node in childNodes)
            // {
                // NodeState currentNodeState = node.TickNode();
                switch (currentNodeState)
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
                        
                        // if (childNodes.IndexOf(node) == childNodes.Count - 1)
                        // {
                        //     this.nodeStatus = NodeState.FAILURE;
                        //     return NodeState.FAILURE;
                        // }
                        // continue;
                        break;
                // }
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
    
    //Will invert the result from the child node
    //e.g if child node is SUCCESS, it will invert to FAILURE vice versa
    [System.Serializable]
    public class InverterNode : DecoratorNode
    {
        public InverterNode(Node childNode) : base(childNode)
        {
        }
        
        public override NodeState TickNode()
        {
            NodeState childNodeState;

            childNodeState = childNode.TickNode();
            switch (childNodeState)
            {
                case NodeState.SUCCESS:
                    this.nodeStatus = NodeState.FAILURE;
                    return NodeState.FAILURE;
                    break;
                
                case NodeState.RUNNING:
                    this.nodeStatus = NodeState.RUNNING;
                    return NodeState.RUNNING;
                    break;
                
                case NodeState.FAILURE:
                    this.nodeStatus = NodeState.SUCCESS;
                    return NodeState.SUCCESS;
                    break;
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
            
            //NOTE: Since it repeats forever, IT WILL NOT RETURN SUCCESS OR FAILURE
            if (repeatForever)
            {
                childNodeState = childNode.TickNode();
                switch(childNodeState)
                {
                    case NodeState.SUCCESS:
                        currentCycle++;
                        childNode.OnReset();
                        // this.nodeStatus = NodeState.SUCCESS;
                        // return NodeState.SUCCESS;
                        break;
                    case NodeState.RUNNING:
                        this.nodeStatus = NodeState.RUNNING;
                        return NodeState.RUNNING;
                        break;
                    case NodeState.FAILURE:
                        currentCycle++;
                        childNode.OnReset();
                        // this.nodeStatus = NodeState.FAILURE;
                        // return NodeState.FAILURE; 
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
                        if (currentCycle + 1 == repeatCycles)
                        {
                            childNode.OnReset();
                            this.nodeStatus = NodeState.SUCCESS;
                            return NodeState.SUCCESS;
                        }
                        else
                        {
                            childNode.OnReset();
                            currentCycle++;
                        }
                        break;
                    case NodeState.RUNNING:
                        this.nodeStatus = NodeState.RUNNING;
                        return NodeState.RUNNING;
                        break;
                    case NodeState.FAILURE:
                        if (currentCycle + 1 == repeatCycles)
                        {
                            childNode.OnReset();
                            this.nodeStatus = NodeState.FAILURE;
                            return NodeState.FAILURE;
                        }
                        else
                        {
                            currentCycle++;
                            childNode.OnReset();
                        }
                        break;
                }
            }

            this.nodeStatus = NodeState.RUNNING;
            return NodeState.RUNNING;  //default return success
        }

        public override void OnReset()
        {
            base.OnReset();
            currentCycle = 0;
        }
    }    
    
    //will reapeat until child node return failure
    [System.Serializable]
    public class RepeatUntilFailNode : DecoratorNode
    {
        [SerializeField] protected int currentCycle = 0;
        
        public RepeatUntilFailNode(Node childNode) : base(childNode)
        {
        }

        public override NodeState TickNode()
        {
            NodeState childNodeState;

            childNodeState = childNode.TickNode();
            switch (childNodeState)
            {
                case NodeState.SUCCESS:
                    currentCycle++;
                    childNode.OnReset();
                    break;
                case NodeState.RUNNING:
                    this.nodeStatus = NodeState.RUNNING;
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    currentCycle++;
                    childNode.OnReset();
                    this.nodeStatus = NodeState.FAILURE;
                    return NodeState.FAILURE;
                    break;
            }

            this.nodeStatus = NodeState.RUNNING;
            return NodeState.RUNNING; //default return success
        }

        public override void OnReset()
        {
            base.OnReset();
            currentCycle = 0;
        }
    }    
    
    //will reapeat until child node return success
    [System.Serializable]
    public class RepeatUntilSuccess : DecoratorNode
    {
        [SerializeField] protected int currentCycle = 0;
        
        public RepeatUntilSuccess(Node childNode) : base(childNode)
        {
        }

        public override NodeState TickNode()
        {
            NodeState childNodeState;

            childNodeState = childNode.TickNode();
            switch (childNodeState)
            {
                case NodeState.SUCCESS:
                    currentCycle++;
                    childNode.OnReset();
                    this.nodeStatus = NodeState.SUCCESS;
                    return NodeState.SUCCESS;
                    break;
                case NodeState.RUNNING:
                    this.nodeStatus = NodeState.RUNNING;
                    return NodeState.RUNNING;
                    break;
                case NodeState.FAILURE:
                    currentCycle++;
                    childNode.OnReset();
                    break;
            }

            this.nodeStatus = NodeState.RUNNING;
            return NodeState.RUNNING; //default return success
        }

        public override void OnReset()
        {
            base.OnReset();
            currentCycle = 0;
        }
    }

    /*
     * NOTE:
     *     THERE ARE 3 TYPES OF LEAF NODE
     *         CONDITION NODE FOR CHECKING CONDITIONS
     *         ACTION NODE FOR BEHAVIOUR
     *         DEBUG NODE FOR DEBUGGING 
     */
    
    //node with only 1 child, checks for condition
    [System.Serializable]
    public abstract class ConditionNode<T> : LeafNode<T> where T : MonoBehaviour
    {
        public ConditionNode(T context) : base(context)
        {
            this.context = context;
        }
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

    [System.Serializable]
    public abstract class DebugNode<T> : LeafNode<T> where T : MonoBehaviour
    {
        public DebugNode(T context) : base(context)
        {
            this.context = context;
        }
    }

}