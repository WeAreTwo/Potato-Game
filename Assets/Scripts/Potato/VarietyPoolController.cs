using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public class VarietyPoolController : MonoBehaviour
    {
        [SerializeField] protected List<PotatoCharacteristicObject> potatoVariety = new List<PotatoCharacteristicObject>();

        public List<PotatoCharacteristicObject> PotatoVariety
        {
            get => potatoVariety;
        }
    }

}