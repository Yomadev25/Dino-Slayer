using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefabs;

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

        }
    }
}
