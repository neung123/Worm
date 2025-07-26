using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    private float _destroyDelay = 2f;

    private float _elapsedTime = 0f;

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _destroyDelay)
        {
            Destroy(gameObject);
        }
    }
}
