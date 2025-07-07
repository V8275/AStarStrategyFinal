using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string DefaultSoliderType;
    [SerializeField]
    private Vector2Int DefaultSoliderLocation;

    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private Transform buttonTransform;

    [SerializeField]
    private Fractions[] TeamCount;
    [SerializeField]
    bool a;
    [SerializeField]
    int FrameRate = 60;

    private TeamUnit selectedUnit = null;
    private SoliderFactory _productFactory;
    private bool _isWaitingForClick = false;
    private int createdObjectIndex = 0;
    private TeamController teamController;

    public TeamController TeamController
    {
        get { return teamController; }
    }

    public TeamUnit SelectedUnit
    {
        get { return selectedUnit; }
    }

    private void Start()
    {
        Application.targetFrameRate = FrameRate;

        _productFactory = GetComponent<SoliderFactory>();

        teamController = new TeamController(TeamCount);
        teamController.Init();

        CreateUnit(DefaultSoliderType, DefaultSoliderLocation);
    }

    /// <summary>
    /// Update Calculation for units in current team
    /// </summary>
    private void Update()
    {
        foreach (var unit in teamController.GetCurrentTeamUnits())
        {
            if (unit.moveable)
            {
                bool move = unit.unit.UpdateCalculations();
                unit.moveable = !move;
            }
        }
    }

    public void NextTeamTurn()
    {
        teamController.NextTeamTurn();
    }

    private void CreateUnit(string name, Vector2Int spawnPosition)
    {
        var team = teamController.GetCurrentTeamUnits();

        if (team.Count > 0)
            foreach (var unit in team) unit.unit.WaitForMove(true);

        IUnit newUnit = _productFactory.CreateProduct(name, teamController.GetCurrentTeamFraction());
        newUnit.Init(spawnPosition);
        newUnit.WaitForMove(false);
        newUnit.SetName(name + $"_{createdObjectIndex}");

        TeamUnit newTeamUnit = new TeamUnit(newUnit, true);
        selectedUnit = newTeamUnit;
        teamController.AddUnitInCurrentTeam(newTeamUnit);
        var button = Instantiate(buttonPrefab, buttonTransform);
        button.GetComponent<Button>().onClick.AddListener(() => SelectUnit(newTeamUnit, teamController.GetCurrentTeamUnits()));
        button.GetComponentInChildren<TMP_Text>().text = name + $"_{createdObjectIndex}";
        newUnit.SetUnitUI(button);
        createdObjectIndex++;
    }

    public void AddUnit(string name)
    {
        AddUnitToScene(name).Forget();
    }

    public void SelectUnit(TeamUnit unit, List<TeamUnit> selectedUn)
    {
        foreach (var item in selectedUn)
        {
            item.unit.WaitForMove(true);
        }
        unit.unit.WaitForMove(false);
        print(unit.moveable);
        selectedUnit = unit;
    }

    public void RemoveUnit()
    {
        teamController.RemoveUnitInTeam(selectedUnit);
        selectedUnit.unit.DestroyUnit();
        selectedUnit = null;
        selectedUnit = teamController.GetCurrentTeamUnits().Last();
        selectedUnit.unit.WaitForMove(false);
    }

    public void RemoveUnit(TeamUnit DestructedUnit)
    {
        Team team = teamController.GetTeamByUnit(DestructedUnit);
        teamController.RemoveUnitInTeam(team, DestructedUnit);
        DestructedUnit.unit.DestroyUnit();
    }

    private async UniTaskVoid AddUnitToScene(string name)
    {
        if (_isWaitingForClick) return;
        _isWaitingForClick = true;

        try
        {
            var spawnLocation = await WaitForTileSelection();
            if (spawnLocation.HasValue)
            {
                Debug.Log("Created!");
                Debug.Log(spawnLocation);
                CreateUnit(name, (Vector2Int)spawnLocation);
            }
        }
        finally
        {
            _isWaitingForClick = false;
        }
    }

    private async UniTask<Vector2Int?> WaitForTileSelection()
    {
        Debug.Log("ќжидание выбора клетки...");

        Vector2Int? result = null;
        await UniTask.WaitUntil(() =>
        {
            if (Input.GetMouseButtonDown(1))
            {
                result = SelectSpawnPoint();
                return result.HasValue;
            }
            return false;
        });

        return result;
    }

    private Vector2Int? SelectSpawnPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return null;
        }

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2);
        int layerMask = ~LayerMask.GetMask("UI");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                var groundTile = hit.collider.gameObject;

                string name = groundTile.name;

                Match match = Regex.Match(name, @"Cell_(\d+)_(\d+)");

                if (match.Success &&
                    int.TryParse(match.Groups[1].Value, out int x) &&
                    int.TryParse(match.Groups[2].Value, out int y))
                {
                    return new Vector2Int(x, y);
                }
                else return null;
            }
            else return null;
        }
        else return null;
    }
}
