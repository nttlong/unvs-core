using System.Collections;
using UnityEngine;
using unvs.interfaces;

namespace unvs.actors
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharatorCamWacher : MonoBehaviour, ICamWacher
    {
        private BoxCollider2D coll;

        public BoxCollider2D Coll => coll;
        private void Awake()
        {
            coll=GetComponent<BoxCollider2D>();
            coll.isTrigger=true;
        }
    }
}