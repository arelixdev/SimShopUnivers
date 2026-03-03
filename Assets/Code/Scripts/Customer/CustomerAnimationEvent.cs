using UnityEngine;

public class CustomerAnimationEvent : MonoBehaviour
{
    public void PickObj()
    {
        GetComponentInParent<CustomerController>().GrabTargetStock();
    }

    public void GoNextAction()
    {
        GetComponentInParent<CustomerController>().EndTakeProduct();
    }
}
