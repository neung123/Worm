using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum SpawnAnchor
{
    Left = 0,
    Right = 1,
    Anywhere = 2
}

public enum VisualLayer
{
    Background,
    Midground,
    Foreground
}

[Serializable]
public struct LayerSettings
{
    public VisualLayer layer;
    public float scaleRangeMin;
    public float scaleRangeMax;
    public string sortingLayer;
    public float spawnCooldownMin;
    public float spawnCooldownMax;
}

[Serializable]
public struct AnchorSpawnWeight
{
    public SpawnAnchor anchor;
    public float weight;
}

public class ParallaxLayerController : DepthSimulatable
{
    [Header("Prefabs to Spawn")]
    [SerializeField]
    private List<ParallaxDecoration> prefabList;

    [Header("X Spawn Bounds")]
    [SerializeField]
    private float leftBoundMin = -10f;

    [SerializeField]
    private float leftBoundMax = -10f;

    [SerializeField]
    private float rightBoundMin = 10f;

    [SerializeField]
    private float rightBoundMax = 10f;

    [Header("Y Scroll")]
    [SerializeField]
    private float baseScrollSpeed = 1f;

    [SerializeField]
    private float bottomY = -5f;

    [SerializeField]
    private float topY = 10f;

    [Header("Layer Settings")]
    [SerializeField]
    private List<LayerSettings> layerSettingsList;

    [Header("Anchor Spawn Weights")]
    [SerializeField]
    private List<AnchorSpawnWeight> anchorWeights;

    [Header("Seabed Settings")]
    [SerializeField]
    private ParallaxDecoration seabedPrefab;

    [SerializeField, Range(0f, 1f)]
    private float stopSpawnRatio = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float seabedSpawnDepthThreshold = 0.9f;

    private class SpawnTimer
    {
        public VisualLayer Layer { get; }
        public float Cooldown { get; private set; }

        public SpawnTimer(VisualLayer layer, float cooldown)
        {
            Layer = layer;
            Cooldown = cooldown;
        }

        public void SetCooldown(float cooldown)
        {
            Cooldown = cooldown;
        }

        public void Update(float deltaTime)
        {
            Cooldown -= deltaTime;
        }
    }

    private readonly List<ParallaxDecoration> _activeObjects = new List<ParallaxDecoration>();
    private Dictionary<VisualLayer, LayerSettings> _layerSettingMap;
    private Dictionary<SpawnAnchor, float> _anchorWeightMap;
    private Dictionary<VisualLayer, SpawnTimer> _layerSpawnTimers;

    private bool _seabedSpawned = false;

    private void Awake()
    {
        _layerSettingMap = new();
        _anchorWeightMap = new();
        _layerSpawnTimers = new();

        foreach (var config in layerSettingsList)
        {
            _layerSettingMap[config.layer] = config;

            _layerSpawnTimers[config.layer] = new SpawnTimer(config.layer, GetRandomCooldown(config));
        }

        foreach (var anchor in anchorWeights)
        {
            _anchorWeightMap[anchor.anchor] = anchor.weight;
        }
    }

    public override void SimulateDepth(float deltaTime, float normalizeDepth)
    {
        base.SimulateDepth(deltaTime, normalizeDepth);

        if (normalizeDepth >= seabedSpawnDepthThreshold)
        {
            if (!_seabedSpawned)
            {
                SpawnSeabed();
                _seabedSpawned = true;
            }
        }

        if (normalizeDepth < stopSpawnRatio)
        {
            foreach (var layer in _layerSpawnTimers.Keys)
            {
                _layerSpawnTimers[layer].Update(deltaTime);

                if (_layerSpawnTimers[layer].Cooldown <= 0f)
                {
                    SpawnDecorationForLayer(layer);
                    _layerSpawnTimers[layer].SetCooldown(GetRandomCooldown(_layerSettingMap[layer]));
                }
            }
        }


        for (int i = _activeObjects.Count - 1; i >= 0; i--)
        {
            ParallaxDecoration deco = _activeObjects[i];
            float speed = baseScrollSpeed * deco.SpeedMultiplier * deco.transform.localScale.x;
            deco.transform.position += Vector3.up * speed * deltaTime;

            if (deco.transform.position.y > topY)
            {
                _activeObjects.RemoveAt(i);
                Destroy(deco.gameObject);
            }
        }
    }

    public override void ResetDepth()
    {
        base.ResetDepth();

        foreach (var decoration in _activeObjects)
        {
            Destroy(decoration.gameObject);
        }

        _activeObjects.Clear();
    }

    private float GetRandomCooldown(LayerSettings layer)
    {
        return Random.Range(layer.spawnCooldownMin, layer.spawnCooldownMax);
    }

    private void SpawnDecorationForLayer(VisualLayer targetLayer)
    {
        List<ParallaxDecoration> validPrefabs = new();

        foreach (var prefab in prefabList)
        {
            if (prefab.Layer == targetLayer)
            {
                validPrefabs.Add(prefab);
            }
        }

        if (validPrefabs.Count == 0)
        {
            return;
        }

        ParallaxDecoration selectedPrefab = GetWeightedPrefab(validPrefabs);
        SpawnDecorationInstance(selectedPrefab);
    }

    private ParallaxDecoration GetWeightedPrefab(List<ParallaxDecoration> candidates)
    {
        List<(ParallaxDecoration prefab, float weight)> weighted = new();

        foreach (var prefab in candidates)
        {
            float anchorWeight = _anchorWeightMap.TryGetValue(prefab.Anchor, out var weight) ? weight : 1f;
            weighted.Add((prefab, anchorWeight));
        }

        float total = 0f;
        foreach (var entry in weighted) total += entry.weight;

        float r = Random.Range(0, total);
        float sum = 0f;

        foreach (var entry in weighted)
        {
            sum += entry.weight;
            if (r <= sum)
                return entry.prefab;
        }

        return weighted[^1].prefab;
    }

    private void SpawnDecorationInstance(ParallaxDecoration prefab)
    {
        if (!_layerSettingMap.TryGetValue(prefab.Layer, out var layerConfig))
        {
            Debug.LogWarning($"No config defined for layer {prefab.Layer}");
            return;
        }

        float scale = Random.Range(layerConfig.scaleRangeMin, layerConfig.scaleRangeMax);

        float spawnX = prefab.Anchor switch
        {
            SpawnAnchor.Left => Random.Range(leftBoundMin, leftBoundMax),
            SpawnAnchor.Right => Random.Range(rightBoundMin, rightBoundMax),
            _ => Random.Range(leftBoundMax, rightBoundMax),
        };

        Vector3 spawnPos = new(spawnX, bottomY, 0f);
        ParallaxDecoration instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

        instance.Setup(scale, layerConfig.sortingLayer);
        _activeObjects.Add(instance);
    }

    private void SpawnSeabed()
    {
        if (seabedPrefab == null)
        {
            Debug.LogWarning("[ParallaxLayerController] Seabed prefab not assigned.");
            return;
        }

        Debug.Log("[ParallaxLayerController] Spawning seabed...");
        Vector3 spawnPos = new Vector3(0f, bottomY, 0f);

        var ground = Instantiate(seabedPrefab, spawnPos, Quaternion.identity, transform);
        _activeObjects.Add(ground);
    }
}
