// EnemyAI.cs
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Melee, Ranged, Kamikaze, Boss }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Tipo de Inimigo")]
    public EnemyType type;

    [Header("Atributos")]
    public float maxHealth = 10f;
    public float currentHealth;
    public float attackDamage = 0.5f; // 0.5 barra de vida
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Refer�ncias")]
    private NavMeshAgent agent;
    private Transform player;
    private PlayerController playerController;

    [Header("Kamikaze")]
    public float explosionRadius = 5f;
    public float explosionDamage = 1f; // 1 barra de vida
    public GameObject explosionEffect;

    [Header("Ranged")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    public void Initialize(Transform playerTarget, PlayerController pController)
    {
        player = playerTarget;
        playerController = pController;

        // Ajustes baseados no tipo
        switch (type)
        {
            case EnemyType.Boss:
                maxHealth *= 1.10f; // 10% mais vida
                currentHealth = maxHealth;
                agent.speed *= 0.98f; // 2% mais lento
                transform.localScale *= 1.5f; // Torna o boss visualmente maior
                break;
            case EnemyType.Kamikaze:
                // 1.5% mais r�pido que o jogador
                agent.speed = pController.moveSpeed * 1.15f; // Aumentei para 15% para ser mais percept�vel
                attackRange = 2.5f; // Raio para iniciar a explos�o
                break;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true; // Para de se mover para atacar
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    void Attack()
    {
        transform.LookAt(player); // Vira para o jogador antes de atacar

        switch (type)
        {
            case EnemyType.Melee:
            case EnemyType.Boss:
                playerController.TakeDamage(attackDamage);
                break;
            case EnemyType.Ranged:
                // L�gica de ataque � dist�ncia
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                // Adicionar um script simples ao proj�til para mover e causar dano
                // ou usar Rigidbody.AddForce
                projectile.GetComponent<Rigidbody>().linearVelocity = (player.position - firePoint.position).normalized * projectileSpeed;
                Destroy(projectile, 5f); // Destroi o proj�til ap�s 5s
                break;
            case EnemyType.Kamikaze:
                Explode();
                break;
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Causa dano ao jogador se estiver no raio
        if (Vector3.Distance(transform.position, player.position) <= explosionRadius)
        {
            playerController.TakeDamage(explosionDamage);
        }

        // Causa dano a outros inimigos
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            EnemyAI otherEnemy = hit.GetComponent<EnemyAI>();
            if (otherEnemy != null && otherEnemy != this)
            {
                otherEnemy.TakeDamage(maxHealth); // Dano massivo para matar outros inimigos
            }
        }

        Die(false); // Morre sem notificar o HordeManager para evitar contagem dupla
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die(true);
        }
    }

    void Die(bool notifyManager)
    {
        if (notifyManager)
        {
            HordeManager.Instance.EnemyDefeated();
        }
        Destroy(gameObject);
    }
}