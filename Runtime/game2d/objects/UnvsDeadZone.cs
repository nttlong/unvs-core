using System;
using System.Linq;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.game2d.scenes.actors;
using unvs.shares;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsDeadZone:UnvsBaseComponent
    {
        private void Awake()
        {
            if(Application.isPlaying)
            {

                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null) return;
            if (collision.gameObject.tag == Constants.Tags.ACTOR)
            {
                var actor = collision.gameObject.GetComponent<UnvsActor>();
                if (actor != null)
                {
                    UnvsApp.Instance.RaiseResart(this.GetNearestCheckPoint());
                }
            }
        }

        private CheckPintInfo GetNearestCheckPoint()
        {
            var scene = this.GetComponentInParent<UnvsScene>();
            if (scene == null) return new CheckPintInfo();

            // Get all checkpoints in this scene
            var checkPoints = scene.GetComponentsInChildren<UnvsCheckPoint>();

            if (checkPoints == null || checkPoints.Length == 0) return new CheckPintInfo();

            float currentX = this.transform.position.x;

            // Use LINQ to find the one with the minimum X distance
            var nearest = checkPoints
                .OrderBy(cp => Mathf.Abs(cp.transform.position.x - currentX))
                .FirstOrDefault();

            return new CheckPintInfo
            {
                checkPointName = nearest.name,
                scenePath = scene.name
            };
        }
    }
#if UNITY_EDITOR
    public partial class UnvsDeadZone : UnvsBaseComponent
    {
        private void OnValidate()
        {
            if (GetComponent<SpriteRenderer>().sprite == null)
                GetComponent<SpriteRenderer>().sprite = Commons.LoadAsset<Sprite>("Packages/com.unvs.core/Runtime/Sprites/Square.png");
            GetComponent<SpriteRenderer>().color = new Color(255,0,0,0.2f);
            GetComponent<Collider2D>().isTrigger = true;
          
        }
    }
#endif
}