using UnityEngine;

public class CoreGame : MonoBehaviour
{
    [field: Header("Reference")]
    [field: SerializeField]
    public Player Player { get; private set; }

    [field: SerializeField]
    public UIController UIController { get; private set; }

    [Header("Gameplay Setup")]
    [Tooltip("The duration of the gameplay in seconds")]
    [SerializeField]
    private int _gameplayDuration;

    public static CoreGame Instance;

    public float GameplayProgress => Mathf.Clamp01(_elapsedGameTime / _gameplayDuration);

    private float _elapsedGameTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _elapsedGameTime += Time.deltaTime;

        if (_elapsedGameTime >= _gameplayDuration)
        {
            UIController.Win();
        }
    }
}
