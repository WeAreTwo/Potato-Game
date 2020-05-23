using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{

    public static class GenericExtension 
    {
        public static bool IsType<T>(this GameObject comparison) where T : class
        {
            if (comparison.GetComponent<T>() != null)
                return true;
            else
                return false;
        }        

    }

}