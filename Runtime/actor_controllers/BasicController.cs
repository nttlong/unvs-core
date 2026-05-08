using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.actor.player;
using unvs.actor.skills;
using unvs.ext;
using unvs.ext.physical2d;
using unvs.game2d.actors;
using unvs.game2d.objects;
using unvs.game2d.scenes;
using unvs.shares;
using UNVS.Core.Actors.Skills;
using static UnityEditor.PlayerSettings;

namespace unvs.controllers
{

    public class BasicController : UnvsPlayer
    {
        protected UnvsActor actor;


        protected MapAction look;
        protected MapAction move;
        protected MapAction jump;
        protected MapAction sprint;
        protected MapAction interact;
        private bool _isSprint;


        //private bool _isJumping;
        protected MapAction crouch;

        private bool _isMove;
        private MapAction inventory;

        public override MapAction OnMapConrrol(string name)
        {
            //_actor = GetComponent<UnvsActor>();
            //if (name == "interact") return this.interact;
            if (name == "look") return this.look;
            if (name == "move") return this.move;
            if (name == "jump") return this.jump;
            if (name == "sprint") return this.sprint;
            if (name == "crouch") return this.crouch;
            if (name == "interact") return this.interact;
            if(name== "inventory") return this.inventory;
            return null;
        }

        public override void InitRuntime()
        {

            this.actor = this.GetComponent<UnvsActor>();


            this.move = new MapAction();
            jump = new MapAction();
            this.sprint = new MapAction();
            crouch = new MapAction();
            this.look = new MapAction();
            this.interact = new MapAction();
            this.inventory =new MapAction();
            this.move.performed += Move_performed;
            this.move.canceled += Move_canceled;


            this.sprint.performed += Sprint_performed;
            this.sprint.canceled += Sprint_canceled;
            jump.started += Jump_performed;
            jump.canceled += Jump_canceled;
            crouch.performed += Crouch_performed;
            crouch.canceled += Crouch_canceled;
            interact.started += Interact_started;
           
            this.inventory.started += Inventory_started;

        }

        private void Inventory_started(InputAction.CallbackContext obj)
        {
            var inventorySkill = actor.Skills.Get<ActorUnvsInventorySkill>();
            if (inventorySkill !=null)
            {
                inventorySkill.ToggleInventoryPanel();
            }
        }

        

        private void Interact_started(InputAction.CallbackContext obj)
        {
            if (obj.control.device is Mouse) return;
            var go = actor.scanerBound.ScanObject(0, 0, Constants.Layers.INTERACT_OBJECT);
            if (go == null) return;
            UnvsInteractObject ret = go.GetComponent<UnvsInteractObject>();
            if (ret == null) return;
            this.actor.CurrentSkill.Direction = new Vector2(this.actor.coll.bounds.center.GetDirectionTo(ret.GetPosition()), 0);
            ret.ExecuteAsync(this.actor, actor.RefreshToken()).ContinueWith(p =>
            {

            }).Forget();
        }

        private void Crouch_canceled(InputAction.CallbackContext obj)
        {
            var com = actor.GetComponent<CompositeCollider2D>();

            var cruchSkill = actor.Skills.Get<ActorUnvsCrouchSkill>();


            actor.Skills.Get<ActorUnvsCrouchSkill>().StopAsync(actor.RefreshToken().Token).ContinueWith(ret =>
            {
                if (!ret) return;
                var defautlSkill = actor.Skills.Get<ActorDefaultSkill>();
                defautlSkill.CurrentSpeed = _isMove ? defautlSkill.MoveSpeed : 0;
                defautlSkill.Direction = actor.CurrentSkill.Direction;
                actor.CurrentSkill = defautlSkill;


            }).Forget();

        }

        private void Crouch_performed(InputAction.CallbackContext obj)
        {

            var com = actor.GetComponent<CompositeCollider2D>();
            var cruchSkill = actor.Skills.Get<ActorUnvsCrouchSkill>();
            if (cruchSkill == null) return;
            if (actor.CurrentSkill == cruchSkill) return;
            if (cruchSkill.IsHitTopGround())
            {

                return;
            }



            actor.Skills.Get<ActorUnvsCrouchSkill>().StartAysnc().ContinueWith(ret =>
            {
                if (!ret) return;
                cruchSkill.CurrentSpeed = _isMove ? cruchSkill.MoveSpeed : 0;
                cruchSkill.Direction = actor.CurrentSkill.Direction;
                actor.CurrentSkill = cruchSkill;
                cruchSkill.Resume();
                //actor.SayText($"Crouch_performed,pos={com.bounds.size}");
            }).Forget();


        }

        private void Jump_canceled(InputAction.CallbackContext obj)
        {

        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            //// this.ControlDisable();
            //// actor.CurrentSkill.IsLocked = true;
            //actor.Skills.Get<ActorJump>().OnPerform(() =>
            //{
            //    //  this.ControlEnable();
            //    actor.CurrentSkill.Resume();
            //});


        }

        private void Sprint_performed(InputAction.CallbackContext obj)
        {

            _isSprint = true;
            var skill = actor.CurrentSkill.Cast<ActorDefaultSkill>();
            if (skill != null)
            {
                if (skill.CurrentSpeed > 0)
                {
                    skill.CurrentSpeed = skill.SprintSpeed;
                }
            }


        }

        private void Sprint_canceled(InputAction.CallbackContext obj)
        {

            _isSprint = false;
            var skill = actor.CurrentSkill.Cast<ActorDefaultSkill>();
            if (skill != null)
            {
                if (_isMove) skill.CurrentSpeed = skill.MoveSpeed;
                else skill.CurrentSpeed = 0;
            }

        }



        private void Move_canceled(InputAction.CallbackContext obj)
        {
            _isMove = false;

            var skill = actor.CurrentSkill.Cast<ActorDefaultSkill>();
            if (skill != null)
            {
                skill.CurrentSpeed = 0;
            }


        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            if (actor.CurrentSkill == null) return;
            _isMove = true;
            var skill = actor.CurrentSkill.Cast<ActorDefaultSkill>();

            skill.Direction = obj.ReadValue<Vector2>();
            if (_isSprint)
            {
                skill.CurrentSpeed = skill.SprintSpeed;
            }
            else
            {
                skill.CurrentSpeed = skill.MoveSpeed;
            }



        }








    }
}