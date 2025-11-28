using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Broom")]
public class BroomAction : RadialAction
{
    public GameObject broomPrefab;
    public override void Execute(RadialActionContext context)
    {
        if(context.broomHand != null)
        {
            context.player.GetComponent<PlayerController>().RemoveTools();
            Transform broom = Instantiate(broomPrefab, context.broomHand).transform;
            context.player.GetComponent<PlayerController>().SetBroomObj(broom);
        }
    }
}
