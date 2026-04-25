
using UnityEngine.InputSystem;
using unvs.actor.player;
using unvs.actor.skills;
using unvs.game2d.actors;
using unvs.ext;
using UnityEngine;
using unvs.game2d.objects;
using Cysharp.Threading.Tasks;
namespace unvs.controllers
{
    public class BasicMouseController : BasicController{
        protected bool IsInInteraction;

        protected MapAction interact => this.NewMapAction("interact", action =>
        {
            action.started += ctx =>
            {
                if(ctx.control.device is Mouse)
                {
                    if (IsInInteraction) actor.RefreshToken();
                    //this.actor.RefreshToken();
                    var pos = this.look.ReadValue<Vector2>().ToWorld();
                    this.actor.CurrentSkill.Direction = new Vector2(this.actor.coll.bounds.center.GetDirectionTo(pos),0);
                    if (pos.ToScreen().GetHitCollider<UnvsInteractObject>() is var obj && obj != null)
                    {
                        IsInInteraction=true;
                        obj.ExecuteAsync(this.actor, actor.RefreshToken()).ContinueWith(p =>
                        {
                            IsInInteraction=false;
                        }).Forget();
                    }
                    else
                    {
                        if (this.actor.CurrentSkill is ActorMouseInteractSkill mouseSkill)
                        {
                            mouseSkill.CurrentSpeed = mouseSkill.MoveSpeed;

                        }
                        else if (this.actor.CurrentSkill is ActorDefaultSkill defaultSkill)
                        {
                            defaultSkill.CurrentSpeed = defaultSkill.MoveSpeed;

                        }
                    }
                    
                   
                }
                
            };
            action.canceled += ctx =>
            {
                if (ctx.control.device is Mouse)
                {
                    //this.actor.RefreshToken();
                    var pos = this.look.ReadValue<Vector2>().ToWorld();
                    this.actor.CurrentSkill.Direction = new Vector2(this.actor.coll.bounds.center.GetDirectionTo(pos), 0);
                    if (this.actor.CurrentSkill is ActorMouseInteractSkill mouseSkill)
                    {
                        mouseSkill.CurrentSpeed = 0;
                    }
                    else if (this.actor.CurrentSkill is ActorDefaultSkill defaultSkill)
                    {
                        defaultSkill.CurrentSpeed = 0;
                    }

                }
            };
            
        });

       
    }
}