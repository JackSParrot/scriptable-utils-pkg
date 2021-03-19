using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Utils
{
    public abstract class ListSO<T> : ScriptableObject
    {
        public List<T> Elements = new List<T>();
    }
}