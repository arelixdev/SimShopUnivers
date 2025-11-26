using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Mop")]
public class MopAction : RadialAction
{
    public GameObject mopPrefab;
    public override void Execute()
    {
        Debug.Log("TAKE MOP");
    }
}
