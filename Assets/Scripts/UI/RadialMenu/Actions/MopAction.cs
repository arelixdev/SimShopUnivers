using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Mop")]
public class MopAction : RadialAction
{
    public GameObject mopPrefab;
    public override void Execute(RadialActionContext context)
    {
        Debug.Log("TAKE MOP");
        if(context.mopHand != null)
        {
            Transform mop = Instantiate(mopPrefab, context.mopHand).transform;
            context.player.GetComponent<PlayerController>().SetMopObj(mop);
        }
    }
}
