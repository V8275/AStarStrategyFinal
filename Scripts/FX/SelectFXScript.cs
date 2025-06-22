using Cysharp.Threading.Tasks;
using UnityEngine;

public class SelectFXScript : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] 
    private Color CheckedColor = Color.white;
    [SerializeField, ColorUsage(true, true)]
    private Color UreachableColor = Color.white;
    [SerializeField, ColorUsage(true, true)]
    private Color StandardColor = Color.black;

    [SerializeField]
    private int TimeToDeselect = 5000;
    [SerializeField]
    private int TimeToCrashDeselect = 1000;

    public async void Select(bool reach = true)
    {
        var time = TimeToDeselect;
        if (reach) SetColor(CheckedColor);
        else 
        { 
            SetColor(UreachableColor);
            time = TimeToCrashDeselect;
        }

        await UniTask.Delay(time);

        DeSelect();
    }

    public void DeSelect()
    {
        SetColor(StandardColor);
    }

    private void SetColor(Color col)
    {
        Material mat = GetComponent<Renderer>().material;
        mat.SetColor("_EmitColor", col);
    }
}
