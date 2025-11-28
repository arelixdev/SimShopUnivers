using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialSliceUI : Image, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Geometry")]
    public float innerRadius;
    public float outerRadius;
    public float startAngle;
    public float endAngle;
    public float gapAngle;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    public System.Action onClick;

    protected override void Awake()
    {
        base.Awake();
        color = normalColor;
    }

    // --- HOVER ---
    public void OnPointerEnter(PointerEventData e)
    {
        color = hoverColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        color = normalColor;
    }

    // --- CLICK ---
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    // --- RAYCAST FIX ---
    public override bool Raycast(Vector2 screenPos, Camera cam)
    {
        RectTransform rt = rectTransform;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPos, cam, out local);

        float dist = local.magnitude;

        // Rayon
        if (dist < innerRadius || dist > outerRadius)
            return false;

        // Angle
        float angle = Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        float start = startAngle + gapAngle * 0.5f;
        float end = endAngle - gapAngle * 0.5f;

        // Normalisation si end < start
        if (end < start) end += 360;

        if (angle < start || angle > end)
            return false;

        return true; // <-- Raycast valide
    }

    // --- DRAWING ---
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        int segments = 40;

        float adjustedStart = startAngle + gapAngle * 0.5f;
        float adjustedEnd = endAngle - gapAngle * 0.5f;

        float angleStep = (adjustedEnd - adjustedStart) / segments;

        for (int i = 0; i < segments; i++)
        {
            float a0 = Mathf.Deg2Rad * (adjustedStart + angleStep * i);
            float a1 = Mathf.Deg2Rad * (adjustedStart + angleStep * (i + 1));

            Vector2 p0 = new Vector2(Mathf.Cos(a0), Mathf.Sin(a0));
            Vector2 p1 = new Vector2(Mathf.Cos(a1), Mathf.Sin(a1));

            UIVertex v1 = UIVertex.simpleVert; v1.color = color; v1.position = p0 * innerRadius;
            UIVertex v2 = UIVertex.simpleVert; v2.color = color; v2.position = p1 * innerRadius;
            UIVertex v3 = UIVertex.simpleVert; v3.color = color; v3.position = p1 * outerRadius;
            UIVertex v4 = UIVertex.simpleVert; v4.color = color; v4.position = p0 * outerRadius;

            vh.AddUIVertexQuad(new[] { v1, v2, v3, v4 });
        }
    }
}