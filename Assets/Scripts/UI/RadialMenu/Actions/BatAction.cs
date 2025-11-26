using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Bat")]
public class BatAction : RadialAction
{
    public override void Execute(RadialActionContext context)
    {
        Debug.Log("TAKE BAT");
    }
}
