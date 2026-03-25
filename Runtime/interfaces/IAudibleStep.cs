using System.Collections;
using UnityEngine;

namespace unvs.interfaces
{
    public interface IAudibleStep
    {
        AudioClip StepSound { get; }
        float Volume { get; }
    }
}