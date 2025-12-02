using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Brush")]
public class BrushAction : RadialAction
{
    public GameObject brushPrefab;
    public override void Execute(RadialActionContext context)
    {
        if(context.brushHand != null)
        {
            context.player.GetComponent<PlayerController>().RemoveTools();
            Transform brush = Instantiate(brushPrefab, context.brushHand).transform;
            context.player.GetComponent<PlayerController>().SetBrushObj(brush);
        }
    }
}
