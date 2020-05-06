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
        }

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
            growthCompletionTime = characteristics.growthTime;
            potatoMat.SetColor("_BaseColor", characteristics.color);
            potatoMat.SetFloat("_LightStepThreshold", 0.15f);
            potatoMat.SetFloat("_BlueNoiseMapScale", 4.0f);
            potatoMat.SetFloat("_DetailAmount", 0.35f);
            potatoMat.SetFloat("_DetailScale", 8.50f);
        }

    }

}
