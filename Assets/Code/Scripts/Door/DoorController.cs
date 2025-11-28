using UnityEngine;
using DG.Tweening;

public class DoorController : MonoBehaviour
{
    [Header("Rotation settings")]
    [SerializeField] private float openAngle = -110f;
    [SerializeField] private float animationDuration = 0.8f;
    [SerializeField] private float openTime = 2f;

    bool isOpen = false;
    Tween activeTween;

    public void OpenDoor()
    {
        if (isOpen) return; // empêche de relancer si déjà ouverte
        isOpen = true;

        // Stop une anim en cours si jamais
        activeTween?.Kill();

        // Ouvre
        activeTween = transform.DORotate(new Vector3(0, openAngle, 0), animationDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // Après X secondes, refermer
                DOVirtual.DelayedCall(openTime, CloseDoor);
            });
    }

    void CloseDoor()
    {
        // Fermer
        activeTween?.Kill();
        activeTween = transform.DORotate(Vector3.zero, animationDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => isOpen = false);
    }
}
