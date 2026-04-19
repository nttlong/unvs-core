
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using unvs.actions;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.game2d.scenes.actors;
using unvs.shares;


namespace unvs.actor.skills
{
    public class SkillSelectorAttribute : PropertyAttribute { }
    [Serializable]
    public abstract class AbstractActorBaseSkill // Chuyển thành abstract
    {


        public string name;
        [HideInInspector] public MonoBehaviour Owner;
        public AbstractActionBaseSkill PreviousSkill;
        public bool IsLocked { get; set; }
        public AbstractActorBaseSkill Disable()
        {
            IsLocked = false;
            return this;
        }
        public AbstractActorBaseSkill Enable()
        {
            IsLocked = true;
            return this;
        }
        // Hàm này gọi sau khi gán Owner để Skill chuẩn bị (nếu cần)
        public virtual void OnBind() { }
        public abstract void OnPerform(Action OnCompleted = null);
        public virtual void OnUpdate()
        {
            if (!this.IsLocked)
            {
                OnPerform();
            }
        }
        public virtual AbstractActorBaseSkill Resume() { IsLocked = false; return this; }
        public virtual async UniTask<bool> StartAysnc(CancellationToken tk = default)
        {
            await UniTask.Yield();
            return true;
        }
        public virtual async UniTask<bool> StopAsync(CancellationToken tk=default)
        {
            await UniTask.Yield();
            return true;
        }
        public T Cast<T>() where T : AbstractActorBaseSkill
        {
            return this as T;
        }

    }

    public class ActorSpeaker : AbstractActorBaseSkill
    {

        public ActorSpeaker()
        {
            name = "speak";
        }

        public override void OnPerform(Action OnCompleted = null)
        {
            throw new NotImplementedException();
        }



        public void SayText(string text)
        {
            var coll = Owner.GetComponent<Collider2D>();
            var pos = new Vector2(coll.bounds.center.x, coll.bounds.max.y + 2);
            UnvsActirDialogue.Instance.Show(pos, text);
        }

    }
    public enum SkillSpeddEnum
    {
        Idle, walk, sprint
    }
    public abstract class AbstractActionBaseSkill : AbstractActorBaseSkill
    {
        public UnvsAnimStates motions { get; set; }
        public float CurrentSpeed { get; set; }

        private Vector2 _lastDir;
        private SkillSpeddEnum _status;
        private Vector2 _dir;
        public SkillSpeddEnum LastStatus { get; internal set; }
        public virtual SkillSpeddEnum Status
        {
            get => _status;
            set
            {
                if (LastStatus != value)
                {
                    OnChangeStatus(value);
                    _status = value;
                    LastStatus = value;
                }
            }
        }
        public override void OnBind()
        {
            base.OnBind();
            InitSkill();
        }

        public abstract void InitSkill();

        public virtual Vector2 Direction
        {
            get => _dir;
            set
            {
                if (_lastDir != value)
                {
                    _dir = value;
                    OnChangeDirection(value);
                    _lastDir = value;
                }
            }
        }

        public abstract void OnChangeDirection(Vector2 value);
        public abstract void OnChangeStatus(SkillSpeddEnum value);


    }
    public class ActorDefaultSkill : AbstractActionBaseSkill
    {
        
        public float MoveSpeed;
        public float SprintSpeed;
        protected Collider2D coll;
        private UnvsActor actor;
        protected CalculateSlopeDirectionResull _slopDirectionResult;


        protected float _lastSpeed;



        public override void OnChangeStatus(SkillSpeddEnum value)
        {
            actor.RefreshToken();
            switch (value)
            {
                case SkillSpeddEnum.walk:
                    motions.BaseMotion("walk");
                    CurrentSpeed = MoveSpeed;
                    break;
                case SkillSpeddEnum.sprint:
                    motions.BaseMotion("sprint");
                    CurrentSpeed = MoveSpeed;
                    break;
                case SkillSpeddEnum.Idle:
                    motions.BaseMotion("Idle");
                    CurrentSpeed = 0;
                    break;
            }
        }



        public override AbstractActorBaseSkill Resume()
        {
            base.Resume();
            _lastSpeed = -1;
            return this;
        }

        public override void OnPerform(Action OnCompleted = null)
        {
            if (coll == null) return;
            if (_lastSpeed != CurrentSpeed)
            {
                
                if (CurrentSpeed <= 0)
                {
                    motions.BaseMotion("Idle");
                }
                if (CurrentSpeed == this.MoveSpeed)
                {
                    motions.BaseMotion("Walk");
                }
                if (CurrentSpeed == this.SprintSpeed)
                {
                    motions.BaseMotion("Sprint");
                }
                _lastSpeed = CurrentSpeed;
            }
            ref var r = ref this._slopDirectionResult;
            coll.CalculateSlopDirection(ref r, Direction.x);

            this.MoveStep(Owner.transform, r.slopeDir);
        }
        public void MoveStep(Transform transform, Vector2 slopeDirection)
        {

            if (Application.isPlaying)
            {

                if (CurrentSpeed > 0)
                {

                    if (slopeDirection == Vector2.zero)
                        //  transform.MoveStep(new Vector2(Direction.x*1000,0), base.CurrentSpeed, out var dir, Direction.x);
                        transform.position += (Vector3)Direction * base.CurrentSpeed * Time.deltaTime;
                    else
                        //transform.MoveStep(this.target, speed, out var dir, direction2.x);
                        transform.position += (Vector3)slopeDirection * base.CurrentSpeed * Time.deltaTime;
                }
            }

        }

        public override void OnChangeDirection(Vector2 value)
        {
            actor.DirectionBy(value.x);
        }

        public override void InitSkill()
        {
            motions = Owner.GetComponent<UnvsAnimStates>();
            coll = Owner.GetComponent<Collider2D>();
            actor = Owner.GetComponent<UnvsActor>();
        }


    }
}