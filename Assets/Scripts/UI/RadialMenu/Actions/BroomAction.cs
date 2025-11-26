using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Broom")]
public class BroomAction : RadialAction
{
    public override void Execute(RadialActionContext context)
    {
        Debug.Log("TAKE BROOM");
    }
}
