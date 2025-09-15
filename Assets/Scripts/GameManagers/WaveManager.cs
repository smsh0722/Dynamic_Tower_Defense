using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnRate;
        public Transform spawnPoint;
    }

    public Wave[] waves;
    public float timeBetweenWaves = 5f;

    public static WaveManager Instance { get; private set; }

    private int leftWaveSpwanCount = 0;
    private List<GameObject> mActiveEnemies = new List<GameObject>();

    public bool IsWin()
    {
        if ( leftWaveSpwanCount > 0) 
            return false;
        mActiveEnemies.RemoveAll(e => e == null);

        return (mActiveEnemies.Count == 0);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        foreach (var wave in waves)
        {
            leftWaveSpwanCount = wave.count;
            yield return StartCoroutine(SpawnWave(wave));

            yield return new WaitUntil(() => leftWaveSpwanCount == 0);
        }
        
        yield return null;
    }
    IEnumerator SpawnWave( Wave wave )
    {
        while ( leftWaveSpwanCount > 0)
        {
            // Spawn
            GameObject enemy = Instantiate(wave.enemyPrefab, wave.spawnPoint.position, Quaternion.identity );
            mActiveEnemies.Add(enemy);

            leftWaveSpwanCount--;

            yield return new WaitForSeconds(Mathf.Max(0f, wave.spawnRate));
        }
    }
}