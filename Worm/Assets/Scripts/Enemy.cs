using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _minSpeedDuration;

    [SerializeField]
    private float _maxSpeedDuration;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private bool _isSpawnLeft;
    private Tween _tween;

    public void StartSwim(Vector3 startPosition, Vector3 endPosition, bool isSpawnLeft)
    {
        transform.position = startPosition;

        _isSpawnLeft = isSpawnLeft;
        _spriteRenderer.flipX = !_isSpawnLeft;

        var duration = Random.Range(_minSpeedDuration, _maxSpeedDuration);

        _tween = transform.DOMove(endPosition, duration);
    }
}
