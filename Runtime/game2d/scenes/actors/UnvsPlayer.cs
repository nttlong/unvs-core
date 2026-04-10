using Cysharp.Threading.Tasks.Triggers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
using unvs.ext;
#if UNITY_EDITOR
using unvs.shares.editor;
using unvs.sys;
#endif

namespace unvs.game2d.scenes.actors {
    public class UnvsPlayer : UnvsBaseComponent
    {
        private AnimMap motions;
        private UnvsActor actor;
        private Vector2 direction;
        private bool isMoving;
        private Vector2 target;

        private void Awake()
        {
           
            if (Application.isPlaying )
            {
                this.motions = this.GetComponent<AnimMap>();
                this.actor=this.GetComponent<UnvsActor>();
              
                UnvsGlobalInput.Player["Move"].started += c =>
                {
                    this.motions.Motion("walk");
                    isMoving = true;
                    this.isMoving = true;
                    this.direction = c.ReadValue<Vector2>();
                    this.motions.DirectionBy(direction);
                    this.target = new Vector2(this.transform.position.x + direction.x * 100000, 0);
                };
                UnvsGlobalInput.Player["Move"].canceled += c =>
                {
                    this.direction = c.ReadValue<Vector2>();
                    this.motions.DirectionBy(direction);
                    this.isMoving = false;
                    this.motions.Motion("idle");

                };
                
            }
        }
        private void Update()
        {
            if (isMoving)
            {
                this.transform.MoveStep(this.target, this.actor.WalkSpeed, out var dir, this.direction.x);
            }
        }
    }
}