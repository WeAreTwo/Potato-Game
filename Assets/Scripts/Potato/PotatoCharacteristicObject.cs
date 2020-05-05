using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    [CreateAssetMenu(fileName = "PotatoCharacteristic_NAME", menuName = "ScriptableObjects/PotatoCharacteristic",
        order = 1)]
    public class PotatoCharacteristicObject : ScriptableObject
    {
        public PotatoCharacteristics characteristics;
    }

}
