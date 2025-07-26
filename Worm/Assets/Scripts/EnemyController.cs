using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField]
    private int _spawnDelayDuration;

    [SerializeField]
    private Vector2 _minLeftSpawnPoint;

    [SerializeField]
    private Vector2 _maxLeftSpawnPoint;

    [SerializeField]
    private Vector2 _minRightSpawnPoint;

    [SerializeField]
    private Vector2 _maxRightSpawnPoint;

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
        if (!CoreGame.Instance.IsPlaying)
        {
            return;
        }

        if (CoreGame.Instance.IsDead)
        {
            return;
        }

        ElapsedSpawnTime();
    }

    public void SetEnemyEnum(EnemyEnum enemyEnum)
    {
        _enemyEnum = enemyEnum;
    }

    public void ClearAllSpawnedEnemy()
    {
        foreach (Enemy enemy in _currentEnemy)
        {
            if (_pools.TryGetValue(name, out var pool))
            {
                pool.ReturnEnemy(enemy);
            }
        }
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

        spawnedEnemy.transform.parent = transform;

        _currentEnemy.Add(spawnedEnemy);

        // Swim Logic
        bool spawnLeft = Random.value < 0.5f;

        spawnedEnemy.StartSwim(GetRandomPosition(spawnLeft), GetRandomPosition(!spawnLeft), spawnLeft);
        spawnedEnemy.WhenCompletedSwim += OnEnemyCompletedSwim;
    }

    private Vector3 GetRandomPosition(bool spawnLeft)
    {

        Vector2 minPoint = spawnLeft ? _minLeftSpawnPoint : _minRightSpawnPoint;
        Vector2 maxPoint = spawnLeft ? _maxLeftSpawnPoint : _maxRightSpawnPoint;

        float x = Random.Range(minPoint.x, maxPoint.x);
        float y = Random.Range(minPoint.y, maxPoint.y);

        return new Vector3(x, y, 0f);
    }

    private void OnEnemyCompletedSwim(Enemy enemy)
    {
        enemy.WhenCompletedSwim -= OnEnemyCompletedSwim;

        enemy.StopSwim();

        string name = enemy.name.Replace("(Clone)", "");

        if (_pools.TryGetValue(name, out var pool))
        {
            pool.ReturnEnemy(enemy);
        }
    }
}
