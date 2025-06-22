using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Melee : MonoBehaviour, IUnit
{
    [SerializeField]
    private bool isPlayer = true;

    private InputManager inputManager;
    private GameObject UnitUI;

    public void Init(Vector2Int spawnPosition)
    {
        if (isPlayer)
        {
            inputManager = GetComponent<InputManager>();
            inputManager.Init(spawnPosition);
        }
    }

    public void WaitForMove(bool active)
    {
        inputManager.SetMove(active);
        FindFirstObjectByType<CinemachineCamera>().Follow = transform;
    }

    /// <summary>
    /// Calculate movement when clicked on map
    /// </summary>
    public bool UpdateCalculations()
    {
        if (inputManager != null)
        {
            return inputManager.Calculate();
        }
        else return true;
    }

    public void DestroyUnit()
    {
        Destroy(UnitUI);
        Destroy(gameObject);
    }

    public void SetUnitUI(GameObject UI)
    {
        UnitUI = UI;
    }

    public string GetUnitData()
    {
        string data = "";

        data += gameObject.name;

        return data;
    }

    public void SetName(string name)
    {
        gameObject.name = name;
    }

    public void DisableUI()
    {
        UnitUI.GetComponent<Button>().interactable = false;
    }

    public void EnableUI()
    {
        UnitUI.GetComponent<Button>().interactable = true;
    }
}
