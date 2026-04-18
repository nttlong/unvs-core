using Codice.CM.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using unvs.actions;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.game2d.scenes.actors;
using unvs.shares;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace unvs.actor.skills
{
    public class SkillSelectorAttribute : PropertyAttribute { }
    [Serializable]
    public class ActorBaseSkill // Chuyển thành abstract
    {


        public string name;
        [HideInInspector] public MonoBehaviour Owner;

        // Hàm này gọi sau khi gán Owner để Skill chuẩn bị (nếu cần)
        public virtual void OnBind() { }
        
    }
    
    public class ActorSpeaker: ActorBaseSkill
    {
        
        public ActorSpeaker()
        {
            name = "speak";
        }
        public void SayText(string text)
        {
            var coll= Owner.GetComponent<Collider2D>();
            var pos = new Vector2(coll.bounds.center.x, coll.bounds.max.y + 2);
            UnvsActirDialogue.Instance.Show(pos, text);
        }
       
    }
    public enum SkillSpeddEnum
    {
        Idle, walk, sprint
    }
    public abstract class ActionBaseSkill : ActorBaseSkill
    {
        
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
        public UnvsAnimStates motions;
        public float CurrentSpeed;
        

       

       
        

        public abstract void OnPerform();
       
    }
    public class ActorDefaultSkill : ActionBaseSkill
    {
       
        public float MoveSpeed;
        public float SprintSpeed;
        private Collider2D coll;
        public UnvsActor actor;
        private CalculateSlopeDirectionResull _slopDirectionResult;
      
        public Vector2 target;
       

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
        

       

        public override void OnBind()
        {
            base.OnBind();
            motions = Owner.GetComponent<UnvsAnimStates>();
            coll= Owner.GetComponent<Collider2D>();
            actor= Owner.GetComponent<UnvsActor>();
        }
        public override void OnPerform()
        {
            if (coll == null) return;

            ref var r = ref this._slopDirectionResult;
            coll.CalculateSlopDirection(ref r, Direction.x);

            this.MoveStep(Owner.transform, r.slopeDir);
        }
        public void MoveStep(Transform transform, Vector2 slopeDirection)
        {
            if (Application.isPlaying)
            {
                
                if (CurrentSpeed>0)
                {
                    
                    if (slopeDirection == Vector2.zero)
                        transform.MoveStep(new Vector2(Direction.x*1000,0), base.CurrentSpeed, out var dir, Direction.x);
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
    }
}