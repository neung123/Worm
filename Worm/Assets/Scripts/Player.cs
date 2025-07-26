using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform _holder;

    [SerializeField]
    private int _maxRotate;

    [SerializeField]
    private float _duration;

    public GameInputAction _gameInputAction;

    private bool _isHolding;
    private Vector2 _movement;

    private Tween _tween;

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
        _isHolding = true;
        _movement = context.ReadValue<Vector2>();
    }

    private void CancelMovement(InputAction.CallbackContext context)
    {
        _isHolding = false;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
