using UnityEngine;

public class NeedInteractionPoint : MonoBehaviour
{
    public NeedType needType;
    public Transform standPoint;
    public float needAmountValue;

    public bool weHaveQueue;
    public bool isOccuped;

    public void ChangeOccupation(bool occ)
    {
        isOccuped = occ;
    }

    



}
