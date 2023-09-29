using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RobotMove : MonoBehaviour
{

    private NavMeshAgent robotEnemy;
    public Transform player;

    private Animator animator;

    public Health health;

    public UnityAction onDamaged;

    // Start is called before the first frame update
    void Start()
    {
        robotEnemy = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //player = GetComponent<Transform>();

        // Subscribe to damage & death actions
        health.onDie += OnDie;
        health.onDamaged += OnDamaged;
    }

    // Update is called once per frame
    void Update()
    {
        robotEnemy.SetDestination(player.position);
    }

    private void FixedUpdate()
    {
        if (health.currentHealth <= 0 )
        {
            health.Kill();
        }

        if (robotEnemy.remainingDistance <= robotEnemy.stoppingDistance)
        {
            animator.SetBool("attack", true);
            animator.SetBool("walk", false);
        } 
        else
        {
            animator.SetBool("attack", false);
            animator.SetBool("walk", true);
        }
    }

    void OnDamaged(float damage, GameObject damageSource)
    {
        // test if the damage source is the player
        if (damageSource && damageSource.GetComponent<PlayerCharacterController>())
        {
            // pursue the player
            //m_DetectionModule.OnDamaged(damageSource);

            if (onDamaged != null)
            {
                onDamaged.Invoke();
            }
            //m_LastTimeDamaged = Time.time;

            // play the damage tick sound
            //if (damageTick && !m_WasDamagedThisFrame)
            //    AudioUtility.CreateSFX(damageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

            //m_WasDamagedThisFrame = true;
        }
    }

    void OnDie()
    {
        /*// spawn a particle system when dying
        var vfx = Instantiate(deathVFX, deathVFXSpawnPoint.position, Quaternion.identity);
        Destroy(vfx, 5f);

        // tells the game flow manager to handle the enemy destuction
        m_EnemyManager.UnregisterEnemy(this);

        // loot an object
        if (TryDropItem())
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }*/
        animator.SetTrigger("OnDamaged");
        // this will call the OnDestroy function
        Destroy(gameObject, 2);
    }
}
