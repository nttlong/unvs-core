using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
namespace unvs.actors.actions
{
    public class ActorsControllers : MonoBehaviour
    {
        public Action<Vector2> OnMoving { get; set; }
        public Action OnStop { get; set; }

        public Vector2 Direction { get; set; }

        public Action<Vector2> OnSprint { get; set; }
        public bool IsMoving { get; set; }
        public float Speed { get; set; }
        public Action<GameObject> OnInteract { get; set; }
        public bool IsSprint { get; set; }
        public bool IsInteracting { get; private set; }

        public Vector2 CalculateDiection(Vector2 v)
        {

            unvs.ext.Vector2dExtesion.CalculateDiection(v);
            return v;
        }
        /// <summary>
        /// Call when controls.Player.Move.started
        /// <code>
        /// controls.Player.Move.started += ctx =>
        ///{
        ///    
        ///    base.ControlsPlayerMoveStart(controls.Player.Move.ReadValue());
        ///};
        /// </code>
        /// </summary>
        /// <param name="vector2"></param>
        public void ControlsPlayerMoveStart(Vector2 vector2)
        {
            Direction = CalculateDiection(vector2);
            IsMoving = true;
            if (IsSprint) OnSprint?.Invoke(Direction);
            else OnMoving?.Invoke(Direction);
        }
        /// <summary>
        /// Call when contrller input Move canceled
        /// <code>
        /// controls.Player.Move.canceled += ctx =>
        ///{
        ///     
        ///     base.ControlsPlayerMoveCanceled();
        ///
        /// };
        /// </code>
        /// </summary>
        public void ControlsPlayerMoveCanceled()
        {
            IsMoving = false;
            OnStop?.Invoke();
        }
        /// <summary>
        /// <code>
        /// controls.Player.Sprint.started += ctx =>
        ///{
        ///    
        ///    base.ControlsPlayerSprintStarted();
        ///};
        /// </code>
        /// </summary>
        public void ControlsPlayerSprintStarted()
        {
            IsSprint = true;
            if (IsMoving) OnSprint?.Invoke(Direction);
        }
        /// <summary>
        /// <code>
        /// controls.Player.Sprint.canceled += ctx =>
        ///{
        ///    base.ControlsPlayerSprintCanceled();
        ///};
        /// </code>
        /// </summary>
        public void ControlsPlayerSprintCanceled()
        {
            IsSprint = false;
            if (IsMoving) OnMoving?.Invoke(Direction);
        }
        /// <summary>
        /// <code>
        /// controls.Player.Interact.started += ctx =>
        /// {
        ///    base.ControlsPlayerInteractStarted(controls.Player.Look.ReadValue(), ctx.control.device is Mouse);
        ///
        /// };
        /// </code>
        /// </summary>
        /// <param name="vector2ScreenCoordinate"></param>
        /// <param name="isUsingMouse"></param>
        public void ControlsPlayerInteractStarted(Vector2 vector2ScreenCoordinate, bool isUsingMouse)
        {
            var actor = GetComponent<IActorObject>();
            if (actor == null) return;
            var interactable = actor.Interactable;
            GameObject scanResult = null;
            if (isUsingMouse)
            {
                /*
                 * If is mouse using use 
                 */
                var input = vector2ScreenCoordinate.ToWorld();
                //If is mouse using use 
                scanResult = interactable.GetObject(input);

                if (scanResult == null)
                {
                    var actorPos = actor.Coll.bounds.center;
                    Direction = new Vector2(actorPos.x > input.x ? -1 : 1, 0);
                    IsMoving = true;
                    if (IsSprint)
                    {
                        OnSprint?.Invoke(Direction);
                    }
                    OnMoving?.Invoke(Direction);
                    return;
                }

            }
            else
            {
                scanResult = interactable.ScanObject();
            }
            if (scanResult != null)
            {
                IsInteracting = true;
                IsMoving = false;
                IsSprint = false;
                OnInteract?.Invoke(scanResult);
                return;
            }
        }
        /// <summary>
        /// <code>
        /// controls.Player.Interact.canceled += ctx =>
        ///{
        ///    
        ///    base.ControlsPlayerInteractCanceled(ctx.control.device is Mouse);
        /// };
        /// </code>
        /// </summary>
        /// <param name="IsUsingMouse"></param>
        public void ControlsPlayerInteractCanceled(bool IsUsingMouse)
        {
            if (IsUsingMouse)
            {
                if (IsInteracting)
                {
                    IsInteracting = false;
                    return;
                }

            }
            IsMoving = false;
            IsSprint = false;
            OnStop?.Invoke();
        }
    }
}