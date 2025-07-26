using System.Collections.Generic;
using UnityEngine;

public class DepthSimulationController : MonoBehaviour
{
    [SerializeField]
    private List<DepthSimulatable> _simulateObjects;

    private void Start()
    {
        CoreGame.Instance.WhenGameStarted += OnGameStarted;
    }

    private void Update()
    {
        if (!CoreGame.Instance.IsPlaying)
        {
            return;
        }

        float normalizedDepth = CoreGame.Instance.GameplayProgress;

        foreach (var simulatable in _simulateObjects)
        {
            simulatable.SimulateDepth(Time.deltaTime, normalizedDepth);
        }
    }

    private void OnGameStarted()
    {
        foreach (var simulatable in _simulateObjects)
        {
            simulatable.ResetDepth();
        }
    }
}
