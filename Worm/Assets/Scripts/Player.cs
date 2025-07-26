using DG.Tweening;
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
    private float _duration;

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

    public GameInputAction _gameInputAction;

    private const float _lerpThreshold = 0.3f;

    private bool _isHolding;
    private Vector2 _movement;
    private Tween _tween;
    private bool _isPlaying;
    private float _localPendulemLerp;

    private void Start()
    {
        _gameInputAction = new GameInputAction();
        _gameInputAction.Gameplay.Enable();
        _gameInputAction.Gameplay.Movement.performed += PerformMovement;
        _gameInputAction.Gameplay.Movement.canceled += CancelMovement;
    }

    private void Update()
    {
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

            _tween = _holder.DORotate(ToRotation, _duration);
        }
        else
        {
            StartPendulum();
        }
    }
    private void OnEnable()
    {
        _gameInputAction?.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _gameInputAction.Gameplay.Disable();
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
