using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField]
    private int _spawnDelayDuration;

    [SerializeField]
    private Vector2 _MaxSpawnPoint;

    [SerializeField]
    private List<Enemy> _easyEnemies;

    [SerializeField]
    private List<Enemy> _mediumEnemies;

    [SerializeField]
    private List<Enemy> _hardEnemies;

    private Dictionary<string, EnemyPool> _pools = new Dictionary<string, EnemyPool>();

    private float _elapsedSpawnTime;
    private List<Enemy> _currentEnemy;
    private EnemyEnum _enemyEnum;

    private void Awake()
    {
        _currentEnemy = new List<Enemy>();
    }

    private void Update()
    {
        ElapsedSpawnTime();
    }

    public void SetEnemyEnum(EnemyEnum enemyEnum)
    {
        _enemyEnum = enemyEnum;
    }

    private void ElapsedSpawnTime()
    {
        _elapsedSpawnTime += Time.deltaTime;

        if (_elapsedSpawnTime >= _spawnDelayDuration)
        {
            _elapsedSpawnTime = 0f;

            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        switch (_enemyEnum)
        {
            case EnemyEnum.Easy:
                SpawnFromList(_easyEnemies);
                break;
            case EnemyEnum.Medium:
                SpawnFromList(_mediumEnemies);
                break;
            case EnemyEnum.Hard:
                SpawnFromList(_hardEnemies);
                break;
        }
    }

    private void SpawnFromList(List<Enemy> enemyList)
    {
        if (enemyList.Count == 0) return;

        int index = Random.Range(0, enemyList.Count);
        Enemy selected = enemyList[index];
        Enemy spawnedEnemy;

        if (_pools.TryGetValue(selected.name, out var pool))
        {
            spawnedEnemy = pool.GetEnemy();
        }
        else
        {
            var newGameObj = new GameObject(selected.name);
            var newPool = newGameObj.AddComponent<EnemyPool>();
            newPool.transform.parent = transform;

            newPool.Init(selected);

            _pools.Add(selected.name, newPool);

            spawnedEnemy = newPool.GetEnemy();
        }

        spawnedEnemy.transform.position = GetSpawnPosition();

        _currentEnemy.Add(spawnedEnemy);
    }

    private Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }
}
