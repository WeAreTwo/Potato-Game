using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoGame
{
    public static class DebuggingExtension
    {
        public static void D(string value)
        {
            Debug.Log(value);
        }
    }

}