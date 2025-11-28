using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Remove")]
public class RemoveAction : RadialAction
{
   public override void Execute(RadialActionContext context)
    {
        if(context.player != null)
        {
            context.player.GetComponent<PlayerController>().RemoveTools();
        }
    } 
}
