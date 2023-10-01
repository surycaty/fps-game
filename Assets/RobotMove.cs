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

    public GameObject owner { get; set; }
    public float currentCharge { get; private set; }
    public Vector3 muzzleWorldVelocity { get; private set; }



    [Header("Shoot Parameters")]
    [Tooltip("The type of weapon wil affect how it shoots")]
    public WeaponShootType shootType;
    [Tooltip("The projectile prefab")]
    public ProjectileBase projectilePrefab;
    [Tooltip("Minimum duration between two shots")]
    public float delayBetweenShots = 1f;
    [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
    public float bulletSpreadAngle = 0f;
    [Tooltip("Amount of bullets per shot")]
    public int bulletsPerShot = 1;
    [Tooltip("Force that will push back the weapon after each shot")]
    [Range(0f, 2f)]
    public float recoilForce = 1;
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
    [Range(0f, 1f)]
    public float aimZoomRatio = 1f;
    [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
    public Vector3 aimOffset;

    [Header("Internal References")]
    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject weaponRoot;
    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform weaponMuzzle;

    public AISimples aISimples;

    public GameObject patrolPath;

    // Start is called before the first frame update
    void Start()
    {
        robotEnemy = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //player = GetComponent<Transform>();

        // Subscribe to damage & death actions
        health.onDie += OnDie;
        health.onDamaged += OnDamaged;

        owner = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ESTADO: " + aISimples._estadoAI);
        if(AISimples.estadoDaAI.seguindo.Equals(aISimples._estadoAI) && robotEnemy.isActiveAndEnabled)
            robotEnemy.SetDestination(player.position);
    }

    private void FixedUpdate()
    {
        if (health.currentHealth <= 0 )
        {
            health.Kill();
        }

        if (AISimples.estadoDaAI.seguindo.Equals(aISimples._estadoAI) && 
            robotEnemy.isActiveAndEnabled && robotEnemy.remainingDistance <= robotEnemy.stoppingDistance)
        {
            animator.SetBool("attack", true);
            animator.SetBool("walk", false);

            delayBetweenShots += Time.deltaTime;

            if (delayBetweenShots > 0){
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
                GetComponentInChildren<AudioSource>().Play();
                //weaponRoot.transform.LookAt(player.transform);
                ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position, 
                                                        Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(this);

                delayBetweenShots -= 1;
            }
        } 
        else
        {
            if(AISimples.estadoDaAI.parado.Equals(aISimples._estadoAI))
            {
                patrolPath.SetActive(true);
            } else
            {
                patrolPath.SetActive(false);
            }
            animator.SetBool("attack", false);
            animator.SetBool("walk", true);
        }
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = bulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);

        return spreadWorldDirection;
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
        animator.SetTrigger("OnDie");
        // this will call the OnDestroy function
        Destroy(gameObject, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
