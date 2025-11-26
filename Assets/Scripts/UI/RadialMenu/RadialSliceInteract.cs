using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialSliceInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RadialSliceUI slice;
    public int index;

    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 1f, 0.5f);

    public System.Action<int> onClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        slice.color = hoverColor;
        slice.SetVerticesDirty(); // force redraw
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slice.color = normalColor;
        slice.SetVerticesDirty();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(index);
    }
}