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
    
    private bool _isSwiming;
    private bool _isSpawnLeft;
    private Tween _tween;

    public void StartSwim(Vector3 startPosition, Vector3 endPosition, bool isSpawnLeft)
    {
        _isSwiming = true;
        gameObject.SetActive(true);

        transform.position = startPosition;

        _isSpawnLeft = isSpawnLeft;

        var x = _isSpawnLeft ? transform.localScale.x : -transform.localScale.x;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);

        var duration = UnityEngine.Random.Range(_minSpeedDuration, _maxSpeedDuration);

        _tween = transform.DOMove(endPosition, duration).OnComplete(() => OnCompletedSwim());
    }

    public void StopSwim()
    {
        _isSwiming = false;
        gameObject.SetActive(false);
        _tween.Kill();

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.z), transform.localScale.y, transform.localScale.z);
    }

    private void OnCompletedSwim()
    {
        _isSwiming = false;
        WhenCompletedSwim?.Invoke(this);
    }
}
