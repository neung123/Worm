using UnityEngine;

public class ParallaxDecorate : DepthSimulatable
{
    [Tooltip("Amount of parallax movement relative to normalized depth (0–1)")]
    public float parallaxFactor = 1f;

    [Tooltip("Max vertical scroll range for this layer")]
    public float maxOffset = 10f;

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    public override void SimulateDepth(float deltaTime, float normalizeDepth)
    {
        base.SimulateDepth(deltaTime, normalizeDepth);

        float offsetY = normalizeDepth * maxOffset * parallaxFactor;
        transform.position = _initialPosition + Vector3.up * offsetY;
    }
}
