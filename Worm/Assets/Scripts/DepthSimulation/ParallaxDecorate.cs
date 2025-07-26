using System.Collections.Generic;
using UnityEngine;

public class ParallaxDecoration : MonoBehaviour
{
    [SerializeField]
    private List<SpriteRenderer> decorationPrefabs = new();

    [Tooltip("Used by spawner to decide where to place object horizontally")]
    [SerializeField] 
    private SpawnAnchor spawnAnchor = SpawnAnchor.Anywhere;

    [SerializeField]
    private float _speedMultiplier = 1f;
    public float SpeedMultiplier => _speedMultiplier;

    [SerializeField] 
    private VisualLayer _layer = VisualLayer.Midground;
    public VisualLayer Layer => _layer;

    public SpawnAnchor Anchor => spawnAnchor;

    public void Setup(float scale, string sortingLayer)
    {
        transform.localScale = Vector3.one * scale;

        foreach (var sr in decorationPrefabs)
        {
            sr.sortingLayerName = sortingLayer;
        }
    }
}