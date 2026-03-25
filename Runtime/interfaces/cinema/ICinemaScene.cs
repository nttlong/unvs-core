using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using unvs.gameword;

namespace interfaces.cinema
{
    public interface ICinemaScene
    {
        Camera Main { get; }
        CinemachineBrain Brain { get; }
        CinemachineCamera VCam { get; }
        GlobalWorldBound WorldBound { get; }
        CinemachineFollow CinemaFollow { get; }
    }
}