using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.actor.skills;
using unvs.game2d.actors;

namespace unvs.actors.controls {
    
    public class SampleControl : UnvsPlayer
    {
        private UnvsActor actor;
        //private InputAction Look;
        //private InputAction interact;
        //private InputAction move;
        //private Collider2D _coll;
        //private Vector2 targetMoving;
        private MapAction interact;
        private MapAction look;
        private MapAction move;
        private MapAction jump;
        private MapAction sprint;
        private bool _isSprint;

        private UnvsActor _actor;
        //private bool _isJumping;
        private MapAction crouch;
        private bool _isCrouch;
        private float _speedBeforeCrouch = -1f;
        private int count;
        private bool _isMove;
        private RuntimeAnimatorController lastAnim;

        public override MapAction OnMapConrrol(string name)
        {
            //_actor = GetComponent<UnvsActor>();
            if (name == "interact") return this.interact;
            if (name == "look") return this.look;
            if (name == "move") return this.move;
            if (name == "jump") return this.jump;
            if (name == "sprint") return this.sprint;
            if (name == "crouch") return this.crouch;
            return null;
        }





       

       

        public override void InitRuntime()
        {

            this.actor = this.GetComponent<UnvsActor>();

        
            this.move = new MapAction();
            jump = new MapAction();
            this.sprint = new MapAction();
            crouch = new MapAction();
          
            this.move.performed += Move_performed;
            this.move.canceled += Move_canceled;

           
            this.sprint.performed += Sprint_performed;
            this.sprint.canceled += Sprint_canceled;
            jump.started += Jump_performed;
            jump.canceled += Jump_canceled;
            crouch.performed += Crouch_performed;
            crouch.canceled += Crouch_canceled;
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