using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

            var props = new MaterialPropertyBlock();

            sr.GetPropertyBlock(props);

            if (_layer == VisualLayer.Background)
            {
                props.SetColor("_LayerTint", new Color(0.7f, 0.8f, 1f)); // soft blue
                props.SetFloat("_LayerBrightness", 0.3f);
            }
            else if (_layer == VisualLayer.Midground)
            {
                props.SetColor("_LayerTint", new Color(0.7f, 0.8f, 1f));
                props.SetFloat("_LayerBrightness", 0.6f);
            }
            else if (_layer == VisualLayer.Foreground)
            {
                props.SetColor("_LayerTint", Color.gray); // slight warm
                props.SetFloat("_LayerBrightness", 1f);
            }

            sr.SetPropertyBlock(props);
        }
    }
}