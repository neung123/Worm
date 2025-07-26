using System.Collections.Generic;
using UnityEngine;

public class DepthSimulationController : MonoBehaviour
{
    [SerializeField]
    private List<DepthSimulatable> _simulateObjects;

    [SerializeField]
    private float _mockedDuration = 10f;

    private float _mockedDepth;

    private void Awake()
    {
        _mockedDepth = 0;
    }

    private void Update()
    {
        _mockedDepth += Time.deltaTime;
        float normalizedDepth = _mockedDepth / _mockedDuration;
        normalizedDepth = Mathf.Clamp01(normalizedDepth);

        foreach (var simulatable in _simulateObjects)
        {
            simulatable.SimulateDepth(Time.deltaTime, normalizedDepth);
        }
    }
}
