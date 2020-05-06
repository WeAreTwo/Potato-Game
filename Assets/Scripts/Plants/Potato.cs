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

    public class Potato : Plant
    {
        //MEMBERS
        [Header("CHARACTERISTICS")]
        [SerializeField] protected PotatoCharacteristics characteristics;

        [Header("CHARACTERISTICS")] 
        [SerializeField] protected bool poppedOut;
        
        protected Material potatoMat;

        public PotatoCharacteristics Characteristics
        {
            get => characteristics;
            set => characteristics = value;
        }

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
            PopOutOfTheGround();
        }
        
        #region Autonomy

        protected virtual void PopOutOfTheGround()
        {
            if (harvestable && !poppedOut)
            {
                // pop out of the ground 
                this.transform.position += new Vector3(0, growthRadius, 0);
                this.transform.rotation = Random.rotation;
                
                // Activate gravity and defreeze all
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;

                poppedOut = true;
                plantStatus = PlantState.Uprooted;
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
            this.transform.LookAt(this.transform.position + growingAxis);
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
            growthCompletionTime = characteristics.growthTime + growthDeviance;
            
            
            potatoMat.SetColor("_BaseColor", characteristics.color);
            potatoMat.SetFloat("_LightStepThreshold", 0.15f);
            potatoMat.SetFloat("_BlueNoiseMapScale", 4.0f);
            potatoMat.SetFloat("_DetailAmount", 0.35f);
            potatoMat.SetFloat("_DetailScale", 8.50f);
        }
        
        #endregion

    }

}
