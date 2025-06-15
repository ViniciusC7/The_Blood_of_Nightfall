// HordeManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordeManager : MonoBehaviour
{
    public static HordeManager Instance;

    [Header("Referências")]
    public List<GameObject> enemyPrefabs; // 0: Melee, 1: Ranged, 2: Kamikaze
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    private Transform playerTransform;
    private PlayerController playerController;

    [Header("Estado do Jogo")]
    public int currentRound = 0;
    private int enemiesRemaining;
    private bool isSpawning = false;
    private bool bossSpawned = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = playerTransform.GetComponent<PlayerController>();
        StartCoroutine(StartNextRoundWithDelay(3f));
    }

    IEnumerator StartNextRoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextRound();
    }

    void StartNextRound()
    {
        currentRound++;
        bossSpawned = false;
        HUDManager.Instance.UpdateRoundUI(currentRound);

        // Lógica de Upgrade
        if (currentRound > 1 && (currentRound - 1) % 5 == 0)
        {
            UpgradeManager.Instance.OfferUpgrades();
            // O jogo vai pausar e o próximo round começará quando o upgrade for escolhido
            return;
        }

        int enemiesToSpawn = 10 + (currentRound - 1) * 10;
        enemiesRemaining = enemiesToSpawn;

        StartCoroutine(SpawnWave(enemiesToSpawn));
    }

    IEnumerator SpawnWave(int count)
    {
        isSpawning = true;
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f); // Intervalo entre spawns
        }
        isSpawning = false;
    }

    void SpawnEnemy()
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject newEnemy = Instantiate(randomEnemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        newEnemy.GetComponent<EnemyAI>().Initialize(playerTransform, playerController);
    }

    void SpawnBoss()
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject boss = Instantiate(bossPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        boss.GetComponent<EnemyAI>().Initialize(playerTransform, playerController);
        enemiesRemaining = 1;
        bossSpawned = true;
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0 && !isSpawning)
        {
            // Se for um round de boss e o boss ainda não apareceu
            if (currentRound % 5 == 0 && !bossSpawned)
            {
                Debug.Log("Todos os inimigos normais derrotados. Spawning Boss!");
                SpawnBoss();
            }
            else // Se não for round de boss ou o boss já foi derrotado
            {
                Debug.Log("Round " + currentRound + " completo!");
                StartCoroutine(StartNextRoundWithDelay(5f)); // Pausa de 5s entre rounds
            }
        }
    }

    // Chamado pelo UpgradeManager após o jogador escolher um upgrade
    public void ResumeAfterUpgrade()
    {
        int enemiesToSpawn = 10 + (currentRound - 1) * 10;
        enemiesRemaining = enemiesToSpawn;
        StartCoroutine(SpawnWave(enemiesToSpawn));
    }
}