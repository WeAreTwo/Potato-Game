using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PotatoGame
{
    public class InventoryController : MonoBehaviour
    {
        // public variables -------------------------
        public int m_numberOfPotato;                   // Number of potatoes held by the player
        public int m_inventoryCapacity;                // If there's a max capacity


        // private variables ------------------------


        // ------------------------------------------
        // Start is called before update
        // ------------------------------------------
        void Start()
        {

        }

        // ------------------------------------------
        // Update is called once per frame
        // ------------------------------------------
        void Update()
        {

        }

        // ------------------------------------------
        // Methods
        // ------------------------------------------
        // Rectify the current inventory -----------------------------------------------
        public void InventoryCount(int newPotato)
        {
            // Add/Subtract the request number to inventory
            m_numberOfPotato += newPotato;
        }
    }
}
