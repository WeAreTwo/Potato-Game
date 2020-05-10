using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PotatoGame
{
    [System.Serializable]
    public class PotatoCharacteristics
    {
        public Color color = Color.yellow;
        public float size = 1.0f;
        public float growthTime = 10.0f;
        public float longevity = 20.0f; 
        
    }

    [System.Serializable]
    public enum PotatoStates
    {
        Moving,
        Eating,
        Idling
    }

    public class Potato : Plant
    {
        //MEMBERS
        [Header("POTATO PARTS")] 
        [SerializeField] protected GameObject potatoEyes;
        [SerializeField] protected GameObject eatingEffect;
        
        [Header("CHARACTERISTICS")]
        [SerializeField] protected PotatoCharacteristics characteristics;

        [Header("AUTONOMOUS AGENT")] 
        [SerializeField] protected bool poppedOut;
        [SerializeField] protected Plant victim;
        [SerializeField] protected Vector3 seekPosition;
        [SerializeField] protected float seekRange = 5.0f;
        [SerializeField] protected float seekForce = 5.0f;
        
        
        [Header("STATE MACHINE")] 
        [SerializeField] protected PotatoStates stateMachine;
        [SerializeField] protected int killCount = 0; //how many potatoes it ate

        //PRIVATE MEMBERS

        //TIMERS
        protected float idleTimer = 0.0f;
        protected float idleTime = 5.0f;
        protected float moveTimer = 0.0f;
        protected float moveTime = 5.0f;
        protected float eatTimer = 0.0f;
        protected float eatTime = 3.0f;
        
        protected Material potatoMat;

        public PotatoCharacteristics Characteristics { get => characteristics; set => characteristics = value; }

        //potato params
        protected override void Awake()
        {
            base.Awake();
            SetPotatoOrientation();
        }

        protected override void Start()
        {
            base.Start();
            CreateMaterial();
            SetPotatoVariety();
            SetPotatoCharacteristics();
        }

        protected override void Update()
        {
            base.Update();
            switch (plantStatus)
            {
                case PlantState.Uprooted:
                    break;
                case PlantState.Planted:
                    PopOutOfTheGround();
                    break;
                case PlantState.Autonomous:
                    FiniteStateMachine();
                    break;
                default:
                    break;
            }
        }
        
        #region Autonomy/State Machine

        protected virtual void PopOutOfTheGround()
        {
            if (harvestPeriodCompleted && !poppedOut)
            {
                // pop out of the ground 
                this.transform.position += new Vector3(0, growthParams.growthRadius, 0);
                this.transform.rotation = Random.rotation;
                
                // Activate gravity and defreeze all
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;

                poppedOut = true;
                
                PickRandomPosition();
                plantStatus = PlantState.Autonomous;
                potatoEyes.SetActive(true);
            }
        }

        //BEHAVIOUR TREE
        protected virtual void FiniteStateMachine()
        {
            switch (stateMachine)
            {
                case PotatoStates.Idling:
                    Idle();
                    break;
                case PotatoStates.Moving:
                    MoveToPosition();
                    break;
                case PotatoStates.Eating:
                    EatPotato();
                    break;
                default:
                    break;
            }
        }

        protected virtual bool CheckLineOfSight(Vector3 target)
        {
            //check to see if the victim is in the line of sight 
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position,target, out hit, seekRange))
            {
                return true;
            }
            return false;
        }
        
        protected virtual void PickRandomPosition()
        {
            float randX = Random.Range(-1.0f, 1.0f);
            float randY = Random.Range(-1.0f, 1.0f);
            Vector3 randomPos = new Vector3(randX,0,randY);
            Vector3 currentPosXZ = new Vector3(this.transform.position.x,0,this.transform.position.z);
            seekPosition = currentPosXZ + (randomPos * seekRange);
        }

        //ACTIONS 
        protected virtual void Idle()
        {
            idleTimer += Time.deltaTime;
            //condition for completion
            if (idleTimer >= idleTime)
            {
                idleTimer = 0;
                MakeDecision();
            }
        }
        
        protected virtual void MoveToPosition()
        {
            if (moveTimer <= moveTime)
            {
                moveTimer += Time.deltaTime;
                seekPosition.y = this.transform.position.y;
                Vector3 force = (seekPosition - this.transform.position).normalized;
                rb.AddForce(force * seekForce);            
            
            }
            //condition for completion 
            if (Vector3.Distance(this.transform.position, seekPosition) < 1.5f * growthParams.growthRadius || moveTimer >= moveTime)
            {
                moveTimer = 0;  // reset the timer 
                MakeDecision(); // make new decision 
            }
        }

        protected virtual void EatPotato()
        {
            //condition for completion
            if (victim == null)
            {
                eatingEffect.SetActive(false);
                MakeDecision();
            }
            else
            {
                Vector3 targetPosition = victim.transform.position;
                if (Vector3.Distance(this.transform.position, targetPosition) < 2.5f * growthParams.growthRadius)
                {
                    eatTimer += Time.deltaTime;
                    if (eatTimer >= eatTime)
                    {
                        eatTimer = 0.0f;
                        if (victim.Health - 25.0f <= 0)
                        {
                            killCount++;
                            victim.Health -= 25.0f;
                        }
                        else
                        {
                            victim.Health -= 25.0f;
                        }
                    }
                }
                else
                {
                    Vector3 force = (targetPosition - this.transform.position).normalized;
                    rb.AddForce(force * seekForce);
                }
                
            }
        }

        //DECISION MAKING 
        protected virtual void MakeDecision()
        {
            //Check is there are other potatoes nearby
            var allPlants = GameManager.Instance.plantsController.Plants;
            if (allPlants != null)
            {
                foreach (var plant in allPlants)
                {
                    if (this == plant) continue; //ignore self by skipping it 

                    if (Vector3.Distance(this.transform.position, plant.transform.position) < this.growthParams.growthRadius + plant.GrowthRadius)
                        victim = plant;
                }
            }

            float roll = Random.value;   //roll
            if (victim != null)
            {
                eatingEffect.SetActive(true);
                stateMachine = PotatoStates.Eating;
            }
            else if (roll < 0.45f)
            {
                PickRandomPosition(); //move to new position
                stateMachine = PotatoStates.Moving;
            }
            else
            {
                stateMachine = PotatoStates.Idling; //idle in the same spot
            }
        }
        
        #endregion
        
        #region Material/Characteristics

        protected virtual void CreateMaterial()
        {
            potatoMat = new Material(Shader.Find(ProjectTags.BaseUnlit));
            this.gameObject.GetComponent<Renderer>().material = potatoMat;
        }

        protected virtual void SetPotatoOrientation()
        {
            //SET LOOK DIRECTION
            this.transform.LookAt(this.transform.position + growthParams.growingAxis);
        }

        protected virtual void SetPotatoVariety()
        {
            if (GameManager.Instance.varietyPool == null) return;
            var potatoVariety = GameManager.Instance.varietyPool.PotatoVariety;
            characteristics = potatoVariety[Random.Range(0, potatoVariety.Count)].characteristics;
        }

        protected virtual void SetPotatoCharacteristics()
        {
            //SET THE TAG
            this.gameObject.tag = ProjectTags.Potato;
            
            //SET CHARACTERISTICS
            this.transform.localScale *= characteristics.size;
            float growthDeviance = Random.Range(characteristics.growthTime - 3.5f, characteristics.growthTime + 3.5f);
            growthParams.growthCompletionTime = characteristics.growthTime + growthDeviance;
            
            
            potatoMat.SetColor("_BaseColor", characteristics.color);
            potatoMat.SetFloat("_LightStepThreshold", 0.15f);
            potatoMat.SetFloat("_BlueNoiseMapScale", 4.0f);
            potatoMat.SetFloat("_DetailAmount", 0.35f);
            potatoMat.SetFloat("_DetailScale", 8.50f);
        }
        
        #endregion

        #region Gizmos

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            switch (PlantStatus)
            {
                case PlantState.Autonomous:

                    switch (stateMachine)
                    {
                        case PotatoStates.Idling:
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(this.transform.position, Vector3.one * growthParams.growthRadius);
                            break;
                        case PotatoStates.Moving:
                            //draw the seek range 
                            Gizmos.color = Color.black;
                            Gizmos.DrawWireSphere(this.transform.position, seekRange);
                            //draw the target 
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireSphere(seekPosition, 0.2f);
                            //draw a line to target
                            Gizmos.color = Color.black;
                            Gizmos.DrawLine(this.transform.position, seekPosition);                           
                            
                            Gizmos.color = Color.magenta;
                            Gizmos.DrawWireCube(this.transform.position, Vector3.one * growthParams.growthRadius);
                            break;
                        case PotatoStates.Eating:
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireCube(this.transform.position, Vector3.one * growthParams.growthRadius);

                            if (victim != null)
                            {
                                //draw a line to target
                                Gizmos.color = Color.black;
                                Gizmos.DrawLine(this.transform.position, victim.transform.position);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion
    }

}
