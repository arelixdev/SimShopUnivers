using UnityEngine;
using DG.Tweening;

public class DoorTriggerController : MonoBehaviour
{
    [Header("Rotation settings")]
    [SerializeField] private float openAngle = 110f;
    [SerializeField] private float animationDuration = 0.8f;
    [SerializeField] private float openTime = 2f;

    bool isOpen = false;
    Tween activeTween;

    Transform door; // pour être explicite si porte enfant
    void Awake()
    {
        door = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        OpenDoorFromDirection(other.transform);
    }

    public void OpenDoorFromDirection(Transform player)
    {
        if (isOpen) return;
        isOpen = true;

        activeTween?.Kill();

        // Détermine côté : dot entre forward de la porte et direction vers le joueur
        Vector3 toPlayer = (player.position - door.position).normalized;
        float dot = Vector3.Dot(door.forward, toPlayer);

        // Si le joueur arrive "devant" la porte => ouvrir dans un sens
        // S'il arrive "derrière" => ouvrir dans l'autre sens
        float targetAngle = (dot > 0) ? -openAngle : openAngle;

        activeTween = door.DORotate(new Vector3(0, targetAngle, 0), animationDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(openTime, CloseDoor);
            });
    }

    void CloseDoor()
    {
        activeTween?.Kill();
        activeTween = door.DORotate(Vector3.zero, animationDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => isOpen = false);
    }
}
