using System;
using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Vector2Int startLocation;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float moveDelay = 0.5f;
    [SerializeField] private float yPlayerOffset = 0.5f;
    [SerializeField] private string GroundTag = "Ground";
    [SerializeField] private int Iterations = 5000;
    [SerializeField] private AnimationManager animationManager;

    private GridManager gridManager;
    private Vector2Int? targetLocation;
    private int[,] grid;
    private Coroutine calculate;
    private bool move = false;

    public void Init(Vector2Int startloc)
    {
        gridManager = FindAnyObjectByType<GridManager>();
        if (!playerObject) playerObject = gameObject;
        startLocation = startloc;
        playerObject.transform.position = new Vector3(startLocation.x, yPlayerOffset, startLocation.y);
        grid = gridManager.GetGrid();
    }

    public bool Calculate()
    {
        if (Input.GetMouseButtonDown(0) && !move)
        {
            try
            {
                move = true;
                if (calculate != null)
                    StopCoroutine(calculate);
                targetLocation = GetMouseGridPosition();

                if (targetLocation != null && CheckPathExists(startLocation, (Vector2Int)targetLocation))
                {
                    calculate = StartCoroutine(MoveAlongPath(startLocation, (Vector2Int)targetLocation));
                        return true;
                }
                else
                {
                    move = false;
                    return false;
                }
            }
            catch (Exception)
            {
                if (calculate != null)
                    StopCoroutine(calculate);
                return false;
            }
        }
        return false;
    }

    private bool CheckPathExists(Vector2Int startCoord, Vector2Int endCoord)
    {
        GridAStar.Node start = new GridAStar.Node(startCoord.x, startCoord.y);
        GridAStar.Node end = new GridAStar.Node(endCoord.x, endCoord.y);
        var boolGrid = GridAStar.ConvertIntArrayToBoolArray(grid);
        var path = GridAStar.FindPath(boolGrid, start, end, Iterations);
        return path != null;
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - playerObject.transform.position;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            playerObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private IEnumerator MoveAlongPath(Vector2Int startCoord, Vector2Int endCoord)
    {
        if (animationManager)
        {
            animationManager.AnimationIdle(false);
            animationManager.AnimationRun(true);
        }

        GridAStar.Node start = new GridAStar.Node(startCoord.x, startCoord.y);
        GridAStar.Node end = new GridAStar.Node(endCoord.x, endCoord.y);

        var endGroundTile = GameObject.Find($"Cell_{endCoord.x}_{endCoord.y}");

        var fx = endGroundTile.GetComponent<SelectFXScript>();
        if (fx) fx.Select();

        var boolGrid = GridAStar.ConvertIntArrayToBoolArray(grid);
        var path = GridAStar.FindPath(boolGrid, start, end, Iterations);

        if (path != null)
        {
            foreach (var node in path)
            {
                Vector3 targetPosition = new Vector3(node.X, playerObject.transform.position.y, node.Y);
                RotateTowardsTarget(targetPosition);

                while (Vector3.Distance(playerObject.transform.position, targetPosition) > 0.1f)
                {
                    playerObject.transform.position = 
                        Vector3.MoveTowards(playerObject.transform.position, targetPosition, Time.deltaTime * 5f);
                    yield return null;
                }

                yield return new WaitForSeconds(moveDelay);
            }

            if (fx) fx.DeSelect();

            move = false;
            startLocation = (Vector2Int)targetLocation;

            if (animationManager)
            {
                animationManager.AnimationIdle(true);
                animationManager.AnimationRun(false);
            }

            Vector2IntSerializable serializableVector = new Vector2IntSerializable(startLocation);
            string json = JsonUtility.ToJson(serializableVector);
            PlayerPrefs.SetString("Save", json);
        }
        else
        {
            move = false;
            if (fx) fx.Select(false);
            
            Debug.Log("Path not found!");
        }
    }

    private Vector2Int? GetMouseGridPosition()
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
            if (hit.collider.gameObject.CompareTag(GroundTag))
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
            }
            //Debug.Log($"Hit: {hit.collider.name}");
            else
            {
                move = false;
                throw new ArgumentException("This cant be target");
            }
        }
        return null;
    }

    /// <summary>
    /// Set, if can move character by player (false - stop, true - move)
    /// </summary>
    /// <param name="setting"></param>
    public void SetMove(bool setting)
    {
        move = setting;
    }
}