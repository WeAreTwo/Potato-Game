using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    //PUBLIC CONSTANT REGISTRY FOR TAGS 
    //Use these variable so that when we modify a string, it affects all our scripts
    //Helpful as the project gets bigger 
    public static class ProjectTags 
    {
        public const string Player = "Player";
        public const string Camera = "MainCamera";
        public const string DynamicObject = "DynamicObject";
        public const string Ground = "Ground";
        public const string Potato = "Potato";
    }

}