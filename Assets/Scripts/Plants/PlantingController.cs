﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PotatoGame
{

    public class PlantingController : InteractableObject, IPlantable, IHarvestable
    {
        // public variables -------------------------
        [Title("Planting States")] 
        public bool m_planting; // When ready to be planted in the ground
        public bool m_planted; // Is the potato currently planted
        public Vector2 m_depthRange; // Range for the depth of the potato when planted
        private float m_depth; // How deep will the potato be planted

        public bool Planting { get => m_planting; set => m_planting = value; }
        public bool Planted { get => m_planted; set => m_planted = value; }
        
        // Check the first collision with the ground -------------------------------
        private void OnCollisionEnter(Collision col)
        {
            // Plant when in contact with the ground
            if (col.gameObject.tag == ProjectTags.Ground && m_planting)
                Plant();
        }

        // Plant the potato --------------------------------------------------------
        private void Plant()
        {
            // Pick a random depth number
            m_depth = Random.Range(m_depthRange.x, m_depthRange.y);

            // Deactivate gravity and freeze all
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Deactivate the colliders
            foreach (Collider objectCollider in GetComponents<Collider>())
                objectCollider.isTrigger = true;

            // Get the potato in the ground
            Vector3 currentPos = transform.position;
            currentPos.y -= m_depth;
            transform.position = currentPos;

            // The potato is now planted!
            m_planting = false;
            m_planted = true;
        }

        public void PlantObject()
        {
            
        }
        
        public void Harvest()
        {
            
        }
    }
}
