using System;
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

    public bool IsPlaying { get; private set; }
    public bool IsDead { get; private set; }
    public float GameplayProgress => Mathf.Clamp01(_elapsedGameTime / _gameplayDuration);

    public event Action WhenGameStarted;

    public GameInputAction GameInputAction;

    private float _elapsedGameTime;

    private void Awake()
    {
        Instance = this;

        GameInputAction = new GameInputAction();
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        if (IsDead)
        {
            return;
        }

        ElapsedGameTime();
    }
    private void OnEnable()
    {
        GameInputAction?.Gameplay.Enable();
    }

    private void OnDisable()
    {
        GameInputAction.Gameplay.Disable();
    }

    public void StartGame()
    {
        IsPlaying = true;
        IsDead = false;
        
        _elapsedGameTime = 0;

        Player.WhenPlayerDead += OnPlayerDead;
        Player.ResetPosition();
        GameInputAction.Gameplay.Enable();
        EnemyController.ClearAllSpawnedEnemy();
        WhenGameStarted?.Invoke();
    }

    private void ElapsedGameTime()
    {
        _elapsedGameTime += Time.deltaTime;

        if (_elapsedGameTime >= _gameplayDuration)
        {
            UIController.Win();

            IsPlaying = false;

            GameInputAction.Gameplay.Disable();
            EnemyController.ClearAllSpawnedEnemy();
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

    private void OnPlayerDead()
    {
        Player.WhenPlayerDead -= OnPlayerDead;

        IsDead = true;
        IsPlaying = false;

        GameInputAction.Gameplay.Disable();
        EnemyController.ClearAllSpawnedEnemy();

        UIController.Lose();
    }
}
