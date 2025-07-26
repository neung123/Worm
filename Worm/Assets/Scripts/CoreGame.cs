using UnityEngine;

public class CoreGame : MonoBehaviour
{
    [field: Header("Reference")]
    [field: SerializeField]
    public Player Player { get; private set; }

    [field: SerializeField]
    public UIController UIController { get; private set; }

    [field: SerializeField]
    public EnemyController EnemyController { get; private set; }

    [Header("Gameplay Setup")]
    [Tooltip("The duration of the gameplay in seconds")]
    [SerializeField]
    private int _gameplayDuration;

    [SerializeField]
    private int _mediumStartTime;

    [SerializeField]
    private int _hardStartTime;

    public static CoreGame Instance;

    public float GameplayProgress => Mathf.Clamp01(_elapsedGameTime / _gameplayDuration);

    private float _elapsedGameTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        ElapsedGameTime();
    }

    private void ElapsedGameTime()
    {
        _elapsedGameTime += Time.deltaTime;

        if (_elapsedGameTime >= _gameplayDuration)
        {
            UIController.Win();
        }
        else
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        var enemyEnum = EnemyEnum.Easy;

        if (_elapsedGameTime >= _hardStartTime)
        {
            enemyEnum = EnemyEnum.Hard;
        }
        else if (_elapsedGameTime >= _mediumStartTime)
        {
            enemyEnum = EnemyEnum.Medium;
        }

        EnemyController.SetEnemyEnum(enemyEnum);
    }
}
