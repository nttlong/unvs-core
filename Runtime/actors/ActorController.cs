using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.actors.actions;
using unvs.ext;
using unvs.gameobjects;
using unvs.gameword;
using unvs.interfaces;
using unvs.shares;
using unvs.sys;
using unvs.ui;




namespace unvs.actors {
    public class ActorController : MonoBehaviour , IActorController
    {
        private IActorObject actor;
        private InputActions inputPlayer;

        public bool IsInteracting { get; internal set; }
        public bool IsMoving { get; internal set; }
        public float Speed { get; internal set; }
        public Vector2 Direction { get; internal set; }
        public Action OnStop { get; internal set; }
        public Action<Vector2> OnSprint { get; internal set; }
        public Action<GameObject> OnInteract { get; internal set; }
        public Action<Vector2> OnMoving { get; internal set; }

        public IActorObject Actor => actor;

        private void Awake()
        {
            if(Application.isPlaying)
            {
                actor = GetComponent<IActorObject>();
                if (GlobalApplication.GlobalInput != null)
                {
                    inputPlayer = GlobalApplication.GlobalInput.Player;
                    SetupInput();
                }
                  
            }
        }
       

        private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            IsMoving = false;
            Speed = 0;
            Actor.Motion.Idle();
        }

        private void SetupInput()
        {
            // Hide the system cursor
           
            inputPlayer.Move.started += Move_started;
            inputPlayer.Move.canceled += Move_canceled;
            inputPlayer.Interact.started += Interact_started;
            inputPlayer.Interact.canceled += Interact_canceled;
        }

        

        public void RemoveInputs()
        {

            inputPlayer.Move.started -= Move_started;
            inputPlayer.Move.canceled -= Move_canceled;
            inputPlayer.Interact.started -= Interact_started;
            inputPlayer.Interact.canceled -= Interact_canceled;

        }
        private void Move_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            IsMoving= true;
            Speed= this.Actor.Movable.WalkSpeed;
            Direction= inputPlayer.Move.ReadValue<Vector2>();
            this.Actor.Motion.Flip(Direction.x);
            this.Actor.Motion.Walk();
            SingleScene.Instance.CursorOff();
        }
        private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            var isMouse = obj.control.device is Mouse;
            if (isMouse)
            {
                SingleScene.Instance.CursorOn();
                var pos = inputPlayer.Look.ReadValue<Vector2>().ToWorld();
                Direction = (pos - actor.Physical.GetPosition()).CalculateDiection();
                this.Actor.Motion.Flip(Direction.x);
                var objInteract= actor.Interactable.GetObject(pos);
                if (objInteract != null)
                {
                    
                    DoInteract(objInteract);
                    return;
                } else
                {
                    actor.Cts?.Cancel();
                    Direction = (pos - actor.Physical.GetPosition()).CalculateDiection();

                    IsMoving = true;
                    Speed = this.Actor.Movable.WalkSpeed;

                    
                    this.Actor.Motion.Walk();
                }
                    
            } else
            {
                var objInteract=actor.Interactable.ScanObject();
                if(objInteract != null)
                {
                    DoInteract(objInteract);
                }
            }
        }

        private void DoInteract(GameObject objInteract)
        {
            IsMoving = false;
            Speed = 0;
            actor.Cts = actor.Cts.Refresh();
            objInteract.GetComponent<IInteractableObject>()?.ExecAsync(this, actor.Cts).Forget();
            return;
        }

        private void Interact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            IsMoving = false;
            Actor.Motion.Idle();
            Speed = 0;
            var pos= inputPlayer.Look.ReadValue<Vector2>().ToWorld();
            
        }
        private void Update()
        {
           
         
            if (!IsMoving) return;
           
            transform.MoveContinuous(Direction, Speed);
            
        }
        private void OnDestroy()
        {
            RemoveInputs();
        }
    }
}