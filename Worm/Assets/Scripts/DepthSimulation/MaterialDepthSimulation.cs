using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDepthSimulation : DepthSimulatable
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private static readonly int _depthValueProperty = Shader.PropertyToID("_DepthValue");

    private Material _targetMaterial;

    private void Awake()
    {
        _targetMaterial = _spriteRenderer.material;
    }

    public override void SimulateDepth(float deltaTime, float normalizeDepth)
    {
        base.SimulateDepth(deltaTime, normalizeDepth);

        _targetMaterial.SetFloat(_depthValueProperty, normalizeDepth);
    }

    public override void ResetDepth()
    {
        base.ResetDepth();

        _targetMaterial.SetFloat(_depthValueProperty, 0f);
    }
}
