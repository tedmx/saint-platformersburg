using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;
        public bool isSample;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        Vector2 move;
        bool isDying = false;
        Vector3 deathPos;

        public GameObject bloodSplatPrefab;

        public Bounds Bounds => _collider.bounds;
        internal Animator animator;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public void DestroyGObj()
        {
            Destroy(gameObject);
        }

        public void Die()
        {
            deathPos = transform.position;
            Schedule<EnemyDeath>().enemy = this;
            isDying = true;
            Invoke("DestroyGObj", 1.5f);

            _collider.enabled = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = 0.0f;
            rb.gravityScale = 0.0f;

            var bloodSplashGO = Instantiate(bloodSplatPrefab, new Vector3(deathPos.x, deathPos.y, 0), Quaternion.identity);
            ParticleSystem bloodSplash = bloodSplashGO.GetComponent<ParticleSystem>();
            bloodSplash.Play();

            animator.SetTrigger("death");
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDying)
            {
                return;
            }
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (isSample || isDying)
            {
                return;
            }

            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
            else
            {
                var player = GameObject.FindWithTag("Player");

                Vector3 toPlayerVector = Vector3.right;
                if (player.transform.position.x < transform.position.x)
                {
                    toPlayerVector = Vector3.left;
                }

                transform.position += toPlayerVector * Time.deltaTime;
            }
        }

    }
}