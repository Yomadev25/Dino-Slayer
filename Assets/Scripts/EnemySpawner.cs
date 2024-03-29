using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private int maximumEnemy;
    [SerializeField]
    private GameObject _spawnFx;

    bool isStart;

    private void Awake()
    {
        GameManager.onUpdateState += CheckGameState;
    }

    private void OnDestroy()
    {
        GameManager.onUpdateState -= CheckGameState;
    }

    private void CheckGameState(GameManager.State state)
    {
        if (state == GameManager.State.GAMEPLAY)
        {
            isStart = true;
        }
        else
        {
            isStart = false;
        }
    }

    private void Update()
    {
        if (isStart)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length < maximumEnemy)
        {
            Vector3 spawnPos = Vector3.zero;
            spawnPos.x = Random.Range(-9f, 9f);
            spawnPos.y = Random.Range(-5f, 5f);

            Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)], spawnPos, Quaternion.identity);
            Destroy(Instantiate(_spawnFx, spawnPos, Quaternion.identity), 1f);
        }
    }
}
