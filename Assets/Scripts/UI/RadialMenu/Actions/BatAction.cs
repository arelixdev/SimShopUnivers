using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Bat")]
public class BatAction : RadialAction
{
    public override void Execute()
    {
        Debug.Log("TAKE BAT");
    }
}
