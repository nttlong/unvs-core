using System;
using UnityEngine;
using unvs.components;
using unvs.components;

namespace unvs.types
{
    [Serializable]
    public abstract class UnvsEditableProperty
    {

    }
    [Serializable]
    public class UnvsProperty<T>: UnvsEditableProperty where T : UnvsBaseComponent
    {
       
        public T Owner;
    }
}