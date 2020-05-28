using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PotatoGame
{

    [System.Serializable]
    public class State
    {
        // protected T component;
        protected Dictionary<Enum, State> allStates;
        
        protected const string name = "State";
        protected bool hasFinished = false;
        protected bool hasExecutedStart = false;
        protected bool hasExecutedExit = false;

        protected Enum currentState;
        protected Enum nextState;

        // public State(T component)
        // {
        //     this.component = component;
        // }

        public Dictionary<Enum, State> AllStates
        {
            get => allStates;
            set => allStates = value;
        }

        public String Name => name;
        public bool HasFinished => hasFinished;
        public bool HasExecutedStart => hasExecutedStart;
        public bool HasExecutedExit => hasExecutedExit;
        public Enum NextState => nextState;

        // public MonoBehaviour Component
        // {
        //     get => component;
        //     set => component = value;
        // }

        public virtual void HandleInput(){}
        
        public void EnterState()
        {
            Reset();
            OnStateStart();   
        }

        public void ExitState()
        {
            OnStateExit();
        }

        public void TriggerExit(Enum T)
        {
            if (allStates.ContainsKey(T))
            {
                nextState = T;
                hasFinished = true;
                OnStateExit();
            }
        }

        #region Call Methods
        //When you first switch into this state
        public virtual void OnStateStart()
        {
            if(hasExecutedStart) return;
            hasExecutedStart = true;
        }

        //Everyframe while ur in this state
        public virtual void OnStateUpdate()
        {
            if (hasFinished) return;
        }
        
        //When you exit this state
        public virtual void OnStateExit()
        {
            if(hasExecutedExit) return;
            hasExecutedExit = true;
        }

        public virtual void Reset()
        {
            hasFinished = false;
            hasExecutedStart = false;
            hasExecutedExit = false;
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
        // protected T component;
        [SerializeField] protected Dictionary<Enum, State> stateDict = new Dictionary<Enum, State>();
        [SerializeField] protected State currentState = new State();
        [SerializeField] protected Enum currentStateKey;
        
        public Dictionary<Enum, State> StateDict => stateDict;
        public State Current { get { return currentState; } }
        public Enum CurrentStateKey { get { return currentStateKey; } }
        public void Add(Enum id, State state)	{ stateDict.Add(id, state); }
        public void Remove(Enum id) { stateDict.Remove(id); }
        public void Clear() { stateDict.Clear(); }

        public void Initialize(Enum id)
        {
            //assign the monobehaviour from parent to each ind. states
            foreach (KeyValuePair<Enum,State> state in stateDict)
            {
                // state.Value.Component = this.component;
                state.Value.AllStates = stateDict;
            }
            
            //assign current state
            State initState = stateDict[id];
            currentState = initState;
            currentStateKey = id;
            initState.EnterState();
        }

        public void CheckCompletion()
        {
            if (currentState.HasFinished && currentState.HasExecutedExit)
            {
                if(currentState.NextState != null)
                    ChangeState(currentState.NextState);
            }
        }

        public void ChangeState(Enum id)
        {
            currentState.ExitState();
            State next = stateDict[id];
            next.EnterState();
            currentState = next;
            currentStateKey = id;
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
            if(currentState != null)
                currentState.DrawGizmos();
        }
    }

}