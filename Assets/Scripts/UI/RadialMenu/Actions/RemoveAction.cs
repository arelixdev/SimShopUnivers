using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Remove")]
public class RemoveAction : RadialAction
{
   public override void Execute(RadialActionContext context)
    {
        Debug.Log("REMOVE TOOL");
    } 
}
