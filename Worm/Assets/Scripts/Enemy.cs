using System.Net.NetworkInformation;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _minSpeed;

    [SerializeField]
    private float _maxSpeed;

    private float _timer;

    private void Update()
    {
        // TODO: make it actually swim
    }

    public void StartSwim(Vector3 startPosition)
    {

    }
}
