using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public PotatoCharacteristics Characteristics
        {
            get => characteristics;
            set => characteristics = value;
        }

        //potato params
        protected override void Awake()
        {
            base.Awake();
            this.gameObject.tag = ProjectTags.Potato;
        }

        protected override void Start()
        {
            base.Start();
            this.transform.LookAt(this.transform.position + growingAxis);
            
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

    }

}
