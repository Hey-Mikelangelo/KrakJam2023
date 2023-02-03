using System;
using UnityEngine;

namespace ScenesLoading
{
    public class ProgressView<T> : IProgress<T>
    {
        public virtual event System.Action OnProgressChanged;
        public T ProgresValue { get; protected set; }
        public virtual void Report(T value)
        {
            ProgresValue = value;
            OnProgressChanged?.Invoke();
        }
    }

}



