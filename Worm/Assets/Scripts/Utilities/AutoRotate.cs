using UnityEngine;
using DG.Tweening;

public class AutoRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Axis to rotate around")]
    [SerializeField] 
    private Vector3 _rotationAxis = Vector3.forward;

    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] 
    private float _rotationSpeed = 180f;

    [Tooltip("Loop type of the tween")]
    [SerializeField]
    private LoopType _loopType = LoopType.Restart;

    [Tooltip("Easing type of the tween")]
    [SerializeField] 
    private Ease _easeType = Ease.Linear;

    private Tween _rotateTween;

    private void OnEnable()
    {
        float duration = 360f / Mathf.Max(_rotationSpeed, 0.01f); // Avoid division by zero

        _rotateTween = transform.DORotate(_rotationAxis * 360f, duration, RotateMode.FastBeyond360)
            .SetEase(_easeType)
            .SetLoops(-1, _loopType)
            .SetRelative();
    }

    private void OnDisable()
    {
        _rotateTween?.Kill();
        _rotateTween = null;
    }
}
