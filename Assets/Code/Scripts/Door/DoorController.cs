using UnityEngine;
using DG.Tweening;

public class DoorController : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Rotation settings")]
    [SerializeField] private float openAngle = 110f;
    [SerializeField] private float animationDuration = 0.8f;

    [Header("Detection")]
    [SerializeField] private string customerLayerName = "Customers";

    [Header("Auto Close")]
    [SerializeField] private float playerOpenTime = 2f;

    private int openRequests = 0;
    private bool isOpen = false;
    private bool triggerHasOpened = false;

    private Tween leftTween;
    private Tween rightTween;
    private Tween playerCloseTween;

    private int customerLayer;
    private float currentOpenAngle;

    private void Awake()
    {
        customerLayer = LayerMask.NameToLayer(customerLayerName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != customerLayer)
            return;

        if(!StoreController.instance.GetIsOpen())
            return;


        if (triggerHasOpened)
            return;

        triggerHasOpened = true;

        Vector3 doorForward = transform.forward;
        Vector3 toOther = (other.transform.position - transform.position).normalized;

        float dot = Vector3.Dot(doorForward, toOther);
        currentOpenAngle = dot >= 0 ? -openAngle : openAngle;

        RequestOpen();

        playerCloseTween?.Kill();
        playerCloseTween = DOVirtual.DelayedCall(playerOpenTime, () =>
        {
            triggerHasOpened = false;
            RequestClose();
        });
    }

    public void OpenDoorFromPlayer()
    {
        currentOpenAngle = openAngle;

        openRequests = 0;
        isOpen = false;

        RequestOpen();

        playerCloseTween?.Kill();
        playerCloseTween = DOVirtual.DelayedCall(playerOpenTime, () =>
        {
            ForceClose();
        });
    }

    private void ForceClose()
    {
        openRequests = 0;
        isOpen = false;
        PlayCloseAnimation();
    }

    public void CloseDoor()
    {
        RequestClose();
    }

    private void RequestOpen()
    {
        openRequests++;

        if (isOpen)
            return;

        isOpen = true;
        PlayOpenAnimation();
    }

    private void RequestClose()
    {
        openRequests = Mathf.Max(0, openRequests - 1);

        if (openRequests > 0)
            return;

        isOpen = false;
        PlayCloseAnimation();
    }

    private void PlayOpenAnimation()
    {
        leftTween?.Kill();
        rightTween?.Kill();

        if (leftDoor != null)
        {
            leftTween = leftDoor.DOLocalRotate(
                new Vector3(0, currentOpenAngle, 0),
                animationDuration
            ).SetEase(Ease.OutCubic);
        }

        if (rightDoor != null)
        {
            rightTween = rightDoor.DOLocalRotate(
                new Vector3(0, -currentOpenAngle, 0),
                animationDuration
            ).SetEase(Ease.OutCubic);
        }
    }

    private void PlayCloseAnimation()
    {
        leftTween?.Kill();
        rightTween?.Kill();

        if (leftDoor != null)
        {
            leftTween = leftDoor.DOLocalRotate(
                Vector3.zero,
                animationDuration
            ).SetEase(Ease.OutCubic);
        }

        if (rightDoor != null)
        {
            rightTween = rightDoor.DOLocalRotate(
                Vector3.zero,
                animationDuration
            ).SetEase(Ease.OutCubic);
        }
    }
}
