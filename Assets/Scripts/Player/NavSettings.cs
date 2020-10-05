using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace PotatoGame
{
    [CreateAssetMenu(fileName = "NavSettings_AITYPE", menuName = "ScriptableObjects/NavSetting", order = 1)]
    public class NavSettings : ScriptableObject
    {
        [Header("Steering Settings")]
        public float baseOffset = 0.05f;
        public float speed = 3.5f;
        public float angularSpeed = 120.0f;
        public float acceleration = 8.0f;
        public float stoppingDistance = 1.0f;
        public bool autoBraking = true;
        
        [Header("Obstacle Settings")]
        public float radius = 0.5f;
        public float height = 2.0f;
        public int priority = 50;
        
    }

}