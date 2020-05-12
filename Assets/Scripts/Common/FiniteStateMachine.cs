using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [System.Serializable]
    public class State
    {
        protected const string name = "State";
        [SerializeField] protected MonoBehaviour component;

        protected bool hasFinished = false;
        protected bool hasExecutedStart = false;
        protected bool hasExecutedExit = false;

        protected string nextState;
        
        public String Name => name;
        public bool HasFinished => hasFinished;
        public bool HasExecutedStart => hasExecutedStart;
        public bool HasExecutedExit => hasExecutedExit;
        public string NextState => nextState;

        public MonoBehaviour Component
        {
            get => component;
            set => component = value;
        }

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
        public virtual void OnStateUpdate()
        {
            if (hasFinished)
                if(!hasExecutedExit)
                    OnStateExit();
        }
        
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

        public StateMachine(MonoBehaviour component)
        {
            this.component = component;
        }
        
        public void Initialize(string id)
        {
            //assign the monobehaviour from parent to each ind. states
            foreach (KeyValuePair<string,State> state in stateDict)
            {
                state.Value.Component = this.component;
            }
            
            //assign current state
            State initState = stateDict[id];
            currentState = initState;
            initState.EnterState();
        }

        public void CheckCompletion()
        {
            if (currentState.HasFinished && currentState.HasExecutedExit)
            {
                ChangeState(currentState.NextState);
            }
        }

        public void ChangeState(string id)
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
            CheckCompletion();
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

        public void DrawGizmos()
        {
            currentState.DrawGizmos();
        }
    }

}