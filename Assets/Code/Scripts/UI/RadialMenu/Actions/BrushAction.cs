using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Brush")]
public class BrushAction : RadialAction
{
    public GameObject brushPrefab;
    public override void Execute(RadialActionContext context)
    {
        Debug.Log("TAKE BRUSH");
        if(context.brushHand != null)
        {
            context.player.GetComponent<PlayerController>().RemoveTools();
            Transform brush = Instantiate(brushPrefab, context.brushHand).transform;
            context.player.GetComponent<PlayerController>().SetBrushObj(brush);
        }
    }
}
