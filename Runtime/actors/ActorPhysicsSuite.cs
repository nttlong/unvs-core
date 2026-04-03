using UnityEngine;
using unvs.interfaces;

namespace unvs.actors
{
    /// <summary>
    /// Abstract base class for Actor Physics Components.
    /// Bundles Rigidbody2D and Collider2D requirements and handles core "plumbing" (wiring).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public abstract class ActorPhysicsSuite : MonoBehaviour
    {
        protected Rigidbody2D rb;
        protected CapsuleCollider2D coll;
        protected IActorObject owner;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<CapsuleCollider2D>();
            owner = GetComponent<IActorObject>();

            // Ensure core physics settings are applied
            if (rb != null)
            {
                rb.freezeRotation = true;
            }
        }

        /// <summary>
        /// Gets the Rigidbody2D associated with this actor.
        /// </summary>
        public Rigidbody2D Body => rb;

        /// <summary>
        /// Gets the CapsuleCollider2D associated with this actor.
        /// </summary>
        public CapsuleCollider2D Collider => coll;
    }
}
