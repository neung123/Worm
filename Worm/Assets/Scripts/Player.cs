using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Main Game Setup")]
    [SerializeField]
    private Transform _holder;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private int _maxRotate;

    [SerializeField]
    private float _rotationAcceleration;

    [SerializeField]
    private float _maxRotationSpeed;

    // TODO: Implement start delay
    [Header("Pendulum Setup")]
    [SerializeField]
    private float _startDelayDuration;

    [SerializeField]
    private float _pendulemLerpIncrease;

    [SerializeField]
    private float _pendulemLerp = 0.5f;

    [SerializeField]
    private float _pendulemSpeedMultiplier;

    public Action WhenPlayerDead;
    public bool IsDead => _isDead;

    private const float _lerpThreshold = 0.3f;

    // Animation triggers
    private const string _animatorRightMoveTrigger = "Right";
    private const string _animatorLeftMoveTrigger = "Left";
    private const string _animatorIdleMoveTrigger = "Idle";

    private bool _isHoldPosition;
    private bool _isHoldingMovement;
    private Vector2 _movement;
    private Tween _tween;
    private bool _isPlaying;
    private float _localPendulemLerp;
    private bool _isDead;
    private float _currentRotationSpeed;

    private Coroutine _stopHoldRoutine;

    private void Start()
    {
        GameInputAction inputActions = CoreGame.Instance.GameInputAction;

        inputActions.Gameplay.Movement.performed += PerformMovement;
        inputActions.Gameplay.Movement.canceled += CancelMovement;

        inputActions.Gameplay.HoldAction.performed += PerformHold;
        inputActions.Gameplay.HoldAction.canceled += CancelHold;
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

        if (_isHoldingMovement)
        {
            if (_tween != null)
            {
                _tween.Kill();
            }

            // 1) figure out desired speed based on input (_movement.x should be -1..1 or scaled input)
            float targetSpeed = _movement.x * _maxRotationSpeed;

            // 2) accelerate currentRotationSpeed toward targetSpeed
            _currentRotationSpeed = Mathf.MoveTowards(
                _currentRotationSpeed,
                targetSpeed,
                _rotationAcceleration * Time.deltaTime
            );

            // 3) compute how much to rotate this frame
            float deltaZ = _currentRotationSpeed * Time.deltaTime;

            // 4) clamp to max allowable rotation
            var e = _holder.eulerAngles;
            float newZ = e.z + deltaZ;

            if (Mathf.Abs(NormalizeAngle(newZ)) > _maxRotate)
            {
                return;
            }

            // 5) apply
            _holder.rotation = Quaternion.Euler(e.x, e.y, newZ);
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

    private void PerformHold(InputAction.CallbackContext context)
    {
        _animator.SetTrigger(_animatorIdleMoveTrigger);

        _isHoldPosition = true;
        _isHoldingMovement = true;

        ResetPendulum();
        _movement = Vector2.zero;
        _currentRotationSpeed = 0;

        if (_stopHoldRoutine != null)
        {
            StopCoroutine(_stopHoldRoutine);
        }
    }

    private void CancelHold(InputAction.CallbackContext context)
    {
        if (_stopHoldRoutine != null)
        {
            StopCoroutine(_stopHoldRoutine);
        }

        _isHoldPosition = false;
        _isHoldingMovement = false;

        _movement = CoreGame.Instance.GameInputAction.Gameplay.Movement.ReadValue<Vector2>();

        if (_movement.magnitude > 0)
        {
            _isHoldingMovement = true;
            string animationTrigger = _movement.x >= 0 ? _animatorRightMoveTrigger : _animatorLeftMoveTrigger;
            _animator.SetTrigger(animationTrigger);
        }
        else
        {
            _stopHoldRoutine = StartCoroutine(StopHoldingRoutine());
        }
    }

    private void PerformMovement(InputAction.CallbackContext context)
    {
        if (_isHoldPosition)
        {
            return;
        }

        ResetPendulum();

        _currentRotationSpeed = 0;
        _isHoldingMovement = true;
        _movement = context.ReadValue<Vector2>();

        // Animation
        string animationTrigger = _movement.x >= 0 ? _animatorRightMoveTrigger : _animatorLeftMoveTrigger;
        _animator.SetTrigger(animationTrigger);

        if (_stopHoldRoutine != null)
        {
            StopCoroutine(_stopHoldRoutine);
        }
    }

    private void CancelMovement(InputAction.CallbackContext context)
    {
        if (_isHoldPosition)
        {
            return;
        }

        _movement = Vector2.zero;
        _currentRotationSpeed = 0;
        // Animation
        _animator.SetTrigger(_animatorIdleMoveTrigger);

        if (_stopHoldRoutine != null)
        {
            StopCoroutine(_stopHoldRoutine);
        }

        _stopHoldRoutine = StartCoroutine(StopHoldingRoutine());
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

        float dist = currentZ;

        float finalSpeed = _pendulemSpeedMultiplier * MathF.Abs(currentZ);

        _tween = _holder.DORotate(toRotation, finalSpeed)
            .SetEase(Ease.InOutSine)
            .SetSpeedBased(true)
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

    private IEnumerator StopHoldingRoutine()
    {
        yield return new WaitForSeconds(_startDelayDuration);

        _isHoldingMovement = false;
    }
}
