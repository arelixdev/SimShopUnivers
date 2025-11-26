using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Brush")]
public class BrushAction : RadialAction
{
    public override void Execute()
    {
        Debug.Log("TAKE BRUSH");
    }
}
