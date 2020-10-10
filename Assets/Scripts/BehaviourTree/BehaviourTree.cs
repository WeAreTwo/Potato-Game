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
        public abstract NodeState CheckNodeStatus();
    }

    [System.Serializable]
    public abstract class Composite : Node
    {
        [SerializeField] protected List<Node> childNodes = new List<Node>();

        public Composite(List<Node> childNodes)
        {
            this.childNodes = childNodes;
        }
        
        public override NodeState CheckNodeStatus()
        {
            
            bool anyChildRunning = false;
            
            //todo make it start from the running node of lookping through the first nodes again
            //if some node is still running then keep looping
            while (anyChildRunning)
            {
                anyChildRunning = false; //reset bool
                
                foreach (Node child in childNodes)
                {
                    switch (child.CheckNodeStatus())
                    {
                        case NodeState.SUCCESS:
                            continue;
                        case NodeState.FAILURE:
                            this.nodeStatus = NodeState.FAILURE;
                            return this.nodeStatus;
                        case NodeState.RUNNING:
                            anyChildRunning = true;
                            continue;
                    }
                }
            }

            this.nodeStatus = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return this.nodeStatus;
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

        //will prob not use delegate
        private LeafNodeDelegate leaf;
        
        public Leaf(LeafNodeDelegate leaf) {
            this.leaf = leaf;
        }

        //behaviour goes here
        public override NodeState CheckNodeStatus() {
            switch (leaf()) {
                case NodeState.SUCCESS:
                    this.nodeStatus = NodeState.SUCCESS;
                    return this.nodeStatus;
                case NodeState.FAILURE:
                    this.nodeStatus = NodeState.FAILURE;
                    return this.nodeStatus;
                case NodeState.RUNNING:
                    this.nodeStatus = NodeState.RUNNING;
                    return this.nodeStatus;
                default:
                    this.nodeStatus = NodeState.FAILURE;
                    return this.nodeStatus;
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