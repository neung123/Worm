using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Main Game Setup")]
    [SerializeField]
    private Transform _holder;

    [SerializeField]
    private int _maxRotate;

    [SerializeField]
    private float _tweenDuration;

    // TODO: Implement start delay
    [Header("Pendulum Setup")]
    [SerializeField]
    private float _startDelayDuration;

    [SerializeField]
    private float _pendulemLerpIncrease;

    [SerializeField]
    private float _pendulemLerp = 0.5f;

    [SerializeField]
    private float _pendulemDuration;

    public Action WhenPlayerDead;
    public bool IsDead => _isDead;

    private const float _lerpThreshold = 0.3f;

    private bool _isHolding;
    private Vector2 _movement;
    private Tween _tween;
    private bool _isPlaying;
    private float _localPendulemLerp;
    private bool _isDead;

    private void Start()
    {
        GameInputAction inputActions = CoreGame.Instance.GameInputAction;

        inputActions.Gameplay.Movement.performed += PerformMovement;
        inputActions.Gameplay.Movement.canceled += CancelMovement;
    }

    private void Update()
    {
        if (!CoreGame.Instance.IsPlaying)
        {
            return;
        }

        if (CoreGame.Instance.IsDead)
        {
            return;
        }

        if (_isHolding)
        {
            if (_tween != null)
            {
                _tween.Kill();
            }

            var originalRotation = _holder.eulerAngles;
            var ToRotation = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z + _movement.x);

            if (Mathf.Abs(NormalizeAngle(ToRotation.z)) > _maxRotate)
            {
                return;
            }

            _tween = _holder.DORotate(ToRotation, _tweenDuration);
        }
        else
        {
            StartPendulum();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            _isDead = true;
            WhenPlayerDead?.Invoke();
        }
    }

    public void ResetPosition()
    {
        _holder.eulerAngles = Vector3.zero;

        _isPlaying = false;
        _isDead = false;
        _tween.Kill();
    }

    private void PerformMovement(InputAction.CallbackContext context)
    {
        ResetPendulum();

        _isHolding = true;
        _movement = context.ReadValue<Vector2>();
    }

    private void CancelMovement(InputAction.CallbackContext context)
    {
        _isHolding = false;
    }

    private void StartPendulum()
    {
        if (_isPlaying) return;

        if (_tween != null)
        {
            _tween.Kill();
        }

        var originalRotation = _holder.eulerAngles;
        float currentZ = NormalizeAngle(_holder.eulerAngles.z);

        if (Mathf.Abs(currentZ) <= _lerpThreshold)
        {
            transform.rotation = Quaternion.identity;
            ResetPendulum();
            return;
        }

        _localPendulemLerp -= _pendulemLerpIncrease;

        var LerpZAngle = Mathf.LerpAngle(currentZ, -currentZ, _localPendulemLerp);
        var toRotation = new Vector3(originalRotation.x, originalRotation.y, LerpZAngle);

        _tween = _holder.DORotate(toRotation, _pendulemDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => CompletedPendulum());

        _isPlaying = true;
    }

    private void CompletedPendulum()
    {
        _isPlaying = false;

        StartPendulum();
    }

    private void ResetPendulum()
    {
        _isPlaying = false;
        _localPendulemLerp = _pendulemLerp;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
