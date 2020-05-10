using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class State
    {
        [SerializeField] protected string name;
        protected MonoBehaviour component;
        protected bool hasExecutedStart = false;
        protected bool hasExecutedExit = false;

        public void HandleInput(){}
        
        public void EnterState()
        {
            OnStateStart();   
        }

        public void ExitState()
        {
            OnStateExit();
        }

        #region Call Methods
        //When you first switch into this state
        public virtual void OnStateStart()
        {
            hasExecutedStart = true;
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

        #region Gizmos
        public virtual void DrawGizmos(){}
        #endregion
    }
    
    [System.Serializable]
    public class StateMachine
    {
        protected MonoBehaviour component;
        [SerializeField] protected Dictionary<string, State> stateDict = new Dictionary<string, State>();
        [SerializeField] protected State currentState = new State();
	
        public State Current { get { return currentState; } }
	
        public void Add(string id, State state)	{ stateDict.Add(id, state); }
        public void Remove(string id) { stateDict.Remove(id); }
        public void Clear() { stateDict.Clear(); }

        public void Initialize(string id)
        {
            State initState = stateDict[id];
            currentState = initState;
            initState.EnterState();
        }

        public void Change(string id, params object[] args)
        {
            currentState.ExitState();
            State next = stateDict[id];
            next.EnterState();
            currentState = next;
        }

        public void HandleInput()
        {
            currentState.HandleInput();
        }
        
        public void Start()
        {
            currentState.OnStateStart();
        }
	
        public void Update()
        {
            currentState.OnStateUpdate();
        }
	
        
        public void OnCollisionEnter(Collision col) 
        {
            currentState.OnCollisionEnter(col);
        }        
        public void OnCollisionStay(Collision col) 
        {
            currentState.OnCollisionStay(col);
        }        
        public void OnCollisionExit(Collision col) 
        {
            currentState.OnCollisionExit(col);
        }
    }

}