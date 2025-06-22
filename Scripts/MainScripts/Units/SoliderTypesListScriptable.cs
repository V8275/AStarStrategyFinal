using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoliderTypesListScriptable", menuName = "Scriptable Objects/SoliderTypesListScriptable")]
public class SoliderTypesListScriptable : ScriptableObject
{
    public FractionUnits[] fractionUnits;
}

[Serializable]
public class UnitData
{
    public string Name;
    public GameObject UnitPrefab;
}

[Serializable]
public class FractionUnits
{
    public string Name;
    public UnitData[] unitDatas;
}