using System;
using UnityEngine;
using unvs.actor.player;
using unvs.game2d.actors;

namespace unvs.controllers
{
    [Serializable]
    public class ActorController : unvs.types.UnvsEditableProperty
    {
        public MonoBehaviour owner;
        public UnvsPlayer Player;
    }
}