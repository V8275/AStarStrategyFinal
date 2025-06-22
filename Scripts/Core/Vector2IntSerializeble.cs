using UnityEngine;

[System.Serializable]
public class Vector2IntSerializable
{
    public int x;
    public int y;

    public Vector2IntSerializable(Vector2Int vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }
}
