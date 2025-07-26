using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _startScreen;

    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private Button _retryButton;

    [SerializeField]
    private GameObject _winScreen;

    [SerializeField]
    private GameObject _loseSrceen;

    private void Awake()
    {
        _startScreen.SetActive(true);
        _startButton.gameObject.SetActive(true);

        _winScreen.SetActive(false);
        _loseSrceen.SetActive(false);
        _retryButton.gameObject.SetActive(false);

        _startButton.onClick.AddListener(OnStartButtonClicked);
        _retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    public void Win()
    {
        _winScreen.SetActive(true);
        _retryButton.gameObject.SetActive(true);
    }

    public void Lose()
    {
        _loseSrceen.SetActive(true);
        _retryButton.gameObject.SetActive(true);
    }

    private void OnStartButtonClicked()
    {
        _startScreen.SetActive(false);
        CoreGame.Instance.StartGame();
    }

    private void OnRetryButtonClicked()
    {
        _winScreen.SetActive(false);
        _loseSrceen.SetActive(false);
        _retryButton.gameObject.SetActive(false);

        CoreGame.Instance.StartGame();
    }
}
