using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class State
    {
        protected MonoBehaviour component;
        protected bool hasExecutedEnter = false;
        protected bool hasExecutedExit = false;

        public void HandleInput(){}
        
        public void EnterState()
        {
            OnStateEnter();   
        }

        public void ExitState()
        {
            OnStateExit();
        }

        #region Call Methods
        //When you first switch into this state
        public virtual void OnStateEnter()
        {
            hasExecutedEnter = true;
        }

        //Everyframe while ur in this state
        public virtual void OnStateUpdate() {}
        
        //When you exit this state
        public virtual void OnStateExit()
        {
            hasExecutedExit = true;
        }
        #endregion

        #region Collisions

        public virtual void OnCollisionEnter(Collision col) {}        
        public virtual void OnCollisionStay(Collision col) {}        
        public virtual void OnCollisionExit(Collision col) {}

        #endregion
    }
    
    
    public class StateMachine
    {
        protected Dictionary<string, State> stateDict = new Dictionary<string, State>();
        protected State current = new State();
	
        public State Current { get { return current; } }
	
        public void Add(string id, State state)	{ stateDict.Add(id, state); }
        public void Remove(string id) { stateDict.Remove(id); }
        public void Clear() { stateDict.Clear(); }

        public void Change(string id, params object[] args)
        {
            current.ExitState();
            State next = stateDict[id];
            next.EnterState();
            current = next;
        }

        public void Start()
        {
            current.OnStateEnter();
        }
	
        public void Update()
        {
            current.OnStateUpdate();
        }
	
        public void HandleInput()
        {
            current.HandleInput();
        }
        
        public void OnCollisionEnter(Collision col) 
        {
            current.OnCollisionEnter(col);
        }        
        public void OnCollisionStay(Collision col) 
        {
            current.OnCollisionStay(col);
        }        
        public void OnCollisionExit(Collision col) 
        {
            current.OnCollisionExit(col);
        }
    }

}