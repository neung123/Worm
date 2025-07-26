using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _winScreen;

    [SerializeField]
    private GameObject _loseSrceen;

    private void Awake()
    {
        _winScreen.SetActive(false);
        _loseSrceen.SetActive(false);
    }
    public void Win()
    {
        _winScreen.SetActive(true);
    }

    public void Lose()
    {
        _loseSrceen.SetActive(true);
    }
}
