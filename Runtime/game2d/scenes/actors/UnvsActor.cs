using Cysharp.Threading.Tasks.Triggers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.ext;

namespace unvs.game2d.scenes.actors
{
    public class UnvsActor : UnvsBaseComponent
    {
        public InputAction inputAction;
        private bool isMoving;
        private Vector2 direction;

        private void Awake()
        {
            if (Application.isPlaying)
            {
               
                UnvsGlobalInput.Player["Move"].started += UnvsActor_started;
                UnvsGlobalInput.Player["Move"].canceled += c =>
                {
                    this.isMoving = false;
                };
                UnvsCinema.Instance.vcam.Watch(transform);
            }
        }

        private void UnvsActor_started(InputAction.CallbackContext obj)
        {
            this.isMoving = true;
            this.direction = obj.ReadValue<Vector2>();
        }

        private void Instance_PlayerActions(ActionSender obj)
        {
            
            if(obj.Name == "Interact" && obj.Start)
            {

                this.transform.position = (Vector3) obj.Pos;
            }
        }
        private void Update()
        {
            if(isMoving)
            {
                this.transform.MoveStep(this.direction,3f,out var dir, this.direction.x);
            }
        }

#if UNITY_EDITOR
        [UnvsButton]
        public void Generate()
        {
            
        }
#endif
    }
}
