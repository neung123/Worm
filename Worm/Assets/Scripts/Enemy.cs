using DG.Tweening;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _minSpeedDuration;

    [SerializeField]
    private float _maxSpeedDuration;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public Action<Enemy> WhenCompletedSwim;
    public bool IsSwiming;

    private bool _isSpawnLeft;
    private Tween _tween;

    public void StartSwim(Vector3 startPosition, Vector3 endPosition, bool isSpawnLeft)
    {
        IsSwiming = true;
        gameObject.SetActive(true);

        transform.position = startPosition;

        _isSpawnLeft = isSpawnLeft;
        _spriteRenderer.flipX = !_isSpawnLeft;

        var duration = UnityEngine.Random.Range(_minSpeedDuration, _maxSpeedDuration);

        _tween = transform.DOMove(endPosition, duration).OnComplete(() => OnCompletedSwim());
    }

    public void StopSwim()
    {
        IsSwiming = false;
        gameObject.SetActive(false);
        _tween.Kill();
    }

    private void OnCompletedSwim()
    {
        IsSwiming = false;
        WhenCompletedSwim?.Invoke(this);
    }
}
