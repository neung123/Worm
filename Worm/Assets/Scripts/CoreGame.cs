using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreGame : MonoBehaviour
{
    [field: SerializeField]
    public Player Player { get; private set; }

    public static CoreGame Instance;

    private void Awake()
    {
        Instance = this;
    }
}
