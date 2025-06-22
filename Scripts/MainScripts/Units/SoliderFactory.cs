using System;
using UnityEngine;

public class SoliderFactory : MonoBehaviour
{
    [SerializeField] private SoliderTypesListScriptable SoliderTypesList;

    public IUnit CreateProduct(string type, int fractionIdx)
    {
        foreach (var unitData in SoliderTypesList.fractionUnits[fractionIdx].unitDatas)
        {
            if (unitData.Name.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                return Instantiate(unitData.UnitPrefab, Vector3.zero, Quaternion.identity).GetComponent<IUnit>();
            }
        }
        throw new ArgumentException("Неверный тип Unit");
    }
}