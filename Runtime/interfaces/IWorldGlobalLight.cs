using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace unvs.interfaces
{
    public interface IWorldGlobalLight
    {
        IGlobalLightWapper GlobalLight { get; set; }

        void Off();
        void On();
    }
    public interface IGlobalLightWapper
    {
        void TunOff();
        GameObject GoOwner { get; }
        IScenePrefab Owner { get; }
        Light2D Light { get; }
        Vector2 Position { get; set; }
      
    }
}
