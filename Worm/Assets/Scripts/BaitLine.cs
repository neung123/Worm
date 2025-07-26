using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitLine : MonoBehaviour
{
    [SerializeField]
    private Transform _topLocation;

    [SerializeField]
    private Transform _baitLocation;

    [SerializeField]
    private LineRenderer _baitLine;

    private void Start()
    {
        _baitLine.positionCount = 2;
        _baitLine.SetPosition(0, _topLocation.position);
        _baitLine.SetPosition(1, _baitLocation.position);
    }

    private void Update()
    {
        if (_baitLine.positionCount == 2)
        {
            _baitLine.SetPosition(0, _topLocation.position);
            _baitLine.SetPosition(1, _baitLocation.position);
        }
    }
}
