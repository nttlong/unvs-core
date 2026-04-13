using Cysharp.Threading.Tasks.Triggers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.ext;
#if UNITY_EDITOR
using unvs.shares.editor;
using unvs.sys;
#endif

namespace unvs.game2d.scenes.actors {
    
    public class UnvsPlayer : UnvsBaseComponent
    {
        private UnvsAnimStates motions;
        private UnvsActor actor;
        
        private InputAction actionMove;
        private Vector2 direction;
        public bool isMoving;
        private Vector2 target;

        private void Awake()
        {
           
            if (Application.isPlaying )
            {
                this.motions = this.GetComponent<UnvsAnimStates>();
                this.actor=this.GetComponent<UnvsActor>();
                actionMove = UnvsGlobalInput.Player["Move"];
                actionMove.started += ActionMoveStart_started;
                actionMove.canceled += ActionMove_canceled;
               
                
            }
        }
        private void OnDisable()
        {
            actionMove.started -= ActionMoveStart_started;
            actionMove.canceled -= ActionMove_canceled;
        }
        private void ActionMove_canceled(InputAction.CallbackContext obj)
        {
            this.direction = obj.ReadValue<Vector2>();
            this.motions.DirectionBy(direction);
            this.isMoving = false;
            this.motions.BaseMotion("idle");
        }

        private void ActionMoveStart_started(InputAction.CallbackContext obj)
        {
            this.motions.BaseMotion("walk");
            isMoving = true;
            this.isMoving = true;
            this.direction = obj.ReadValue<Vector2>();
            this.motions.DirectionBy(direction);
            this.target = new Vector2(this.transform.position.x + direction.x * 100000, 0);
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