using UnityEngine;

[CreateAssetMenu(menuName = "Radial/Actions/Scan")]
public class ScanAction : RadialAction
{
    public override void Execute()
    {
        Debug.Log("TAKE SCAN");
    }
}
