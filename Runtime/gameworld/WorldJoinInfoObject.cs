using System;
using System.Collections;
using UnityEngine;
using unvs.interfaces;
using unvs.shares;

namespace unvs.gameword
{


    public class WorldJoinInfoObject : MonoBehaviour, IWorldJoinInfoObject
    {
        [SerializeField]
        public WorldJoinInfo worldJoinInfo = new WorldJoinInfo();

        public WorldJoinInfo WorldJoinInfo => worldJoinInfo;
    }
}