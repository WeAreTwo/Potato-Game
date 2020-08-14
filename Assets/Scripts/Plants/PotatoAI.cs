using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class PotatoAI : AIController
    {
        [SerializeField] protected Vector3 bellPosition;

        #region Properties
        public Vector3 BellPosition { get => bellPosition; set => bellPosition = value; }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            bellPosition = GameObject.FindWithTag(ProjectTags.Bell).transform.position;
        }

        protected override void Start()
        {
            // base.Start();
            fsm = new StateMachine();
            fsm.Add(PlantStates.Idle, new IdleAI<PotatoAI>(this));
            fsm.Add(PlantStates.Move, new MoveAI<PotatoAI>(this));
            fsm.Add(PlantStates.MoveToBell, new MoveToBell<PotatoAI>(this));
            fsm.Initialize(PlantStates.Idle);
        }


        protected override void Update()
        {
            base.Update();
        }
        
        //Adding to list
        protected virtual void OnEnable()
        {
            if(GameManager.Instance && GameManager.Instance.plantsController)
                GameManager.Instance.plantsController.AutonomousPotatoes.Add(this);
        }

        protected virtual void OnDisable()
        {
            if(GameManager.Instance && GameManager.Instance.plantsController)
                GameManager.Instance.plantsController.AutonomousPotatoes.Remove(this);
        }
    }

    [System.Serializable]
    public class MoveToBell<T> : MoveAI<T> where T : PotatoAI
    {
        protected T component;
        
        public MoveToBell(T component) : base(component)
        {
            this.component = component;
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            MoveToPosition();
        }

        protected override void MoveToPosition()
        {
            // component.BellPosition.y = component.transform.position.y;
            component.Heading = (component.BellPosition - component.transform.position).normalized;
            
            //condition for completion 
            if (Vector3.Distance(component.transform.position, component.BellPosition) < 1.5f)
            {
                MakeDecision(); // make new decision 
            }
            
        }

        protected override void MakeDecision()
        {
            TriggerExit(PlantStates.Idle);
        }
    }

}