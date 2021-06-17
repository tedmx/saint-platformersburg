using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;

using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;

namespace Platformer.Mechanics
{
    public class PlayerCombat : MonoBehaviour
    {

        public Transform attackPoint;
        public float attackRange = 0.8f;
        public LayerMask enemyLayers;
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        internal Animator animator;

        public void FinishAttack()
        {
            animator.SetBool("attack", false);
        }
    
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0)) {

                animator = GetComponent<Animator>();
                animator.SetTrigger("attack");
                animator.SetBool("attack", true);

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

                foreach(Collider2D enemyCldr in hitEnemies)
                {
                    EnemyController enemy = enemyCldr.GetComponent<EnemyController>();
                    enemy.Die();
                    Debug.Log("We hit " + enemy.name);
                }

                Invoke("FinishAttack", 0.1f);
            }

            var playerGO = GameObject.FindWithTag("Player");
            PlayerController player = playerGO.GetComponent<PlayerController>();
            Debug.Log("auo");
            Debug.Log(playerGO.transform.position);
            Debug.Log(attackPoint.position);
            if (player.facingRight)
            {
                attackPoint.position = new Vector3(playerGO.transform.position.x + 0.5f, playerGO.transform.position.y - 0.02f);
            } else
            {
                attackPoint.position = new Vector3(playerGO.transform.position.x - 0.5f, playerGO.transform.position.y - 0.02f);
                //attackPoint.position = new Vector3(-0.5f, -0.02f);
            }
            // if (playerGO.transform.rotation)
        }

        void OnDrawGizmosSelected () {
            if (attackPoint == null) {
                return;
            }
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            
        }
    }
}
