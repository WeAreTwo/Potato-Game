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
        [SerializeField] protected PotatoCharacteristics characteristics;

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
                case PlantState.Planted:
                    // this.transform.LookAt(this.transform.position + growingAxis);
                    break;
                default:
                    break;
            }
        }

        protected virtual void CreateMaterial()
        {
            potatoMat = new Material(Shader.Find(ProjectTags.BaseUnlit));
            this.gameObject.GetComponent<Renderer>().material = potatoMat;
        }

        protected virtual void SetPotatoVariety()
        {
            var potatoVariety = GameManager.Instance.varietyPool.PotatoVariety;
            characteristics = potatoVariety[Random.Range(0, potatoVariety.Count)].characteristics;
        }

        protected virtual void SetPotatoCharacteristics()
        {
            //SET LOOK DIRECTION
            this.transform.LookAt(this.transform.position + growingAxis);
            //SET THE TAG
            this.gameObject.tag = ProjectTags.Potato;
            
            //SET CHARACTERISTICS
            this.transform.localScale *= characteristics.size;
            growthCompletionTime = characteristics.growthTime;
            potatoMat.SetColor("_BaseColor", characteristics.color);
        }

    }

}
