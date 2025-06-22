using UnityEngine;

public interface IUnit
{
    public void Init(Vector2Int spawnPosition);
    public void WaitForMove(bool active);
    public bool UpdateCalculations();
    public void DestroyUnit();
    public void SetUnitUI(GameObject UI);
    public string GetUnitData();
    public void SetName(string name);
    public void DisableUI();
    public void EnableUI();
}
