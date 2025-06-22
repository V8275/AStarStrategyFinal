using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorLayersScriptable", menuName = "Scriptable Objects/GeneratorLayersScriptable")]
public class GeneratorLayersScriptable : ScriptableObject
{
    public Noise[] noiseLayers;
}

[Serializable]
public class Noise
{
    public GameObject[] prefabs;
    public float[] thresholds;
    public float NoiseScale = 0.1f;
}