using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Broom")]
public class BroomAction : RadialAction
{
    public override void Execute()
    {
        Debug.Log("TAKE BROOM");
    }
}
