using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.game2d.scenes.actors;
using unvs.shares;

namespace unvs.game2d.objects {
    /// <summary>
    /// This is the liquid object emulate.
    /// </summary>

    [RequireComponent(typeof(BoxCollider2D))]
    public class UnvsLiquid : UnvsBaseComponent {
        List<Collider2D> colliders=new List<Collider2D>();
        private bool _hasHitPlayerFooter;
        private UnvsActorSwimmable _swimSkill;

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.collider.gameObject.tag == Constants.Tags.PLAYER_HEADER)
            {
                _swimSkill = collision.collider.GetComponentInParent<UnvsActorSwimmable>(true);
                if (_swimSkill != null)
                {
                    _swimSkill.ActiveSkill();
                }

            }
        }
        //private void OnCollisionExit2D(Collision2D collision)
        //{
        //    if (collision.collider.gameObject.tag == Constants.Tags.PLAYER_HEADER)
        //    {
        //        _swimSkill = collision.collider.GetComponentInParent<UnvsActorSwimmable>();
        //        if (_swimSkill != null)
        //        {
        //            _swimSkill.DeactiveSkill();
        //        }
        //    }
        //}
        //private void OnCollisionExit2D(Collision2D collision)
        //{
        //    if (collision.collider.gameObject.tag == Constants.Tags.PLAYER_HEADER)
        //    {
        //        var swimSkill = collision.collider.GetComponentInParent<UnvsActorSwimmable>();
        //        if (swimSkill != null)
        //        {
        //            if (swimSkill.GetComponent<UnvsActorPhysical>() != null)
        //            {
        //                if (swimSkill.GetComponent<UnvsActorPhysical>().IsHitDownFloor())
        //                {
        //                    swimSkill.DeactiveSkill();
        //                    if (UnvsApp.Instance.currentActor != null)
        //                    {
        //                        UnvsApp.Instance.currentActor.SayText("I'm not in water");
        //                    }
        //                }
        //            }

        //        }

        //    }
        //}
        private void Awake()
        {
            if (Application.isPlaying)
            {
                GetComponent<BoxCollider2D>().excludeLayers=LayerMask.GetMask(Constants.Layers.PLAYER_FOOTER);
            }
            this.SetMeOnLayer(Constants.Layers.WORLD_LIQUID);
        }
        
        //private void FixedUpdate()
        //{
        //    if (colliders.Count > 0)
        //    {
        //        var coll=this.GetComponent<Collider2D>();
        //        var c= colliders.Count(p=>p.bounds.max.y+10f< coll.bounds.max.y);
        //        if(c == colliders.Count)
        //        {

        //            for (var i = 0; i < colliders.Count; i++)
        //            {

        //                var collider = colliders[i];
        //                collider.isTrigger = false;
        //                colliders.RemoveAt(i);
        //            }
        //        }

        //    }
        //}
    }
}