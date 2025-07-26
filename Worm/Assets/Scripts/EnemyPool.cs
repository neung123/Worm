using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private Enemy _enemyPrefab;
    private Queue<Enemy> _pool = new Queue<Enemy>();

    public void Init(Enemy enemy)
    {
        _enemyPrefab = enemy;
    }

    public Enemy GetEnemy()
    {
        if (_pool.Count == 0)
        {
            Enemy newEnemy = Instantiate(_enemyPrefab);
            newEnemy.gameObject.SetActive(false);
            _pool.Enqueue(newEnemy);
        }

        Enemy enemyToUse = _pool.Dequeue();
        enemyToUse.gameObject.SetActive(true);
        return enemyToUse;
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemy.transform.parent = transform;
        enemy.gameObject.SetActive(false);
        _pool.Enqueue(enemy);
    }
}
