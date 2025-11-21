using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    public List<EnemyInWave> enemies = new List<EnemyInWave>();
    public int currWave;
    private int waveValue;

    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public Transform[] spawnLocation;
    public int spawnIndex;

    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    public List<GameObject> spawnedEnemies = new List<GameObject>();

    [Header("Scene Load Settings")]
    public float sceneLoadDelay = 3f;
    private bool loadingNextScene;

    private Coroutine loadSceneRoutine;

    // ---------- NOVO: Delay antes de iniciar spawns ----------
    [Header("Wave Start Delay")]
    public float startDelay = 5f;    // tempo antes do primeiro spawn
    private bool waveStarted = false;

    void Start()
    {
        StartCoroutine(StartWaveAfterDelay());
    }

    private IEnumerator StartWaveAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);

        waveStarted = true;
        GenerateWave();
    }

    void FixedUpdate()
    {
        if (!waveStarted) return;   // <-- TRAVA O SPAWN AT� PASSAR O DELAY

        if (spawnTimer <= 0)
        {
            // spawn an enemy
            if (enemiesToSpawn.Count > 0)
            {
                GameObject enemy = (GameObject)Instantiate(
                    enemiesToSpawn[0],
                    spawnLocation[spawnIndex].position,
                    Quaternion.identity
                );

                enemiesToSpawn.RemoveAt(0);
                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;

                if (spawnIndex + 1 <= spawnLocation.Length - 1)
                {
                    spawnIndex++;
                }
                else
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                waveTimer = 0; // if no enemies remain, end wave
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        // if wave finished and no spawned enemies remain
        if (!loadingNextScene && waveTimer <= 0 && spawnedEnemies.Count <= 0)
        {
            currWave++;
            GenerateWave();
        }
    }

    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count;
        waveTimer = waveDuration;
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();

        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }

        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }

        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }

        if (!loadingNextScene && spawnedEnemies.Count == 0 && enemiesToSpawn.Count == 0)
        {
            loadSceneRoutine = StartCoroutine(LoadSceneAfterDelay());
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        loadingNextScene = true;
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

[System.Serializable]
public class EnemyInWave
{
    public GameObject enemyPrefab;
    public int cost;
}
