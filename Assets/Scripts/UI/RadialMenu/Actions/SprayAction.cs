using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Spray")]
public class SprayAction : RadialAction
{
    public override void Execute(RadialActionContext context)
    {
        Debug.Log("TAKE SPRAY");
    }
}
