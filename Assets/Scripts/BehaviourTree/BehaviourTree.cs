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
        public NodeState nodeState = NodeState.RUNNING;

        //constructor
        public Node()
        {
            
        }
        
        //to determine succes,  fail, running
        public abstract NodeState Evaluate();
    }

    [System.Serializable]
    public abstract class Composite : Node
    {
        [SerializeField] protected List<Node> childNodes = new List<Node>();

        public Composite(List<Node> childNodes)
        {
            this.childNodes = childNodes;
        }
        
        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;
            foreach (Node child in childNodes)
            {
                switch (child.Evaluate())
                {
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILURE:
                        this.nodeState = NodeState.FAILURE;
                        return this.nodeState;
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        continue;
                }
            }

            this.nodeState = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return this.nodeState;
        }
    }
    
    //node with only 1 child
    [System.Serializable]
    public abstract class Decorator : Node
    {
        
    }
    
    //end node at the very end, behaviour is here
    [System.Serializable]
    public abstract class Leaf : Node
    {
        public delegate NodeState LeafNodeDelegate();

        /* The delegate that is called to evaluate this node */
        private LeafNodeDelegate leaf;

        /* Because this node contains no logic itself,
         * the logic must be passed in in the form of 
         * a delegate. As the signature states, the action
         * needs to return a NodeState enum */
        public Leaf(LeafNodeDelegate leaf) {
            this.leaf = leaf;
        }

        /* Evaluates the node using the passed in delegate and 
         * reports the resulting state as appropriate */
        public override NodeState Evaluate() {
            switch (leaf()) {
                case NodeState.SUCCESS:
                    this.nodeState = NodeState.SUCCESS;
                    return this.nodeState;
                case NodeState.FAILURE:
                    this.nodeState = NodeState.FAILURE;
                    return this.nodeState;
                case NodeState.RUNNING:
                    this.nodeState = NodeState.RUNNING;
                    return this.nodeState;
                default:
                    this.nodeState = NodeState.FAILURE;
                    return this.nodeState;
            }
        }
    }
    
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