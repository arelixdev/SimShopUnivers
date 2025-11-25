using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadialMenuUI : MonoBehaviour
{
    [Header("Slices")]
    public int itemCount = 6;
    public float innerRadius = 80f;
    public float outerRadius = 150f;

    [Header("Gap Between Slices")]
    public float gapAngle = 2f;

    public bool clockwise = true;

    [Header("Icons")]
    public List<Sprite> icons;
    public Vector2 iconSize = new Vector2(60, 60);

    [Header("Slice Color")]
    public Color sliceColor = Color.white;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        Destroy(transform.GetChild(i).gameObject);

        float anglePerSlice = 360f / itemCount;

        for (int i = 0; i < itemCount; i++)
        {
            float start = i * anglePerSlice;
            float end = (i + 1) * anglePerSlice;




            // ----- 1. CREATE SLICE -----
            GameObject sliceObj = new GameObject("Slice " + i);
            sliceObj.transform.SetParent(transform, false);

            RadialSliceUI slice = sliceObj.AddComponent<RadialSliceUI>();
            slice.innerRadius = innerRadius;
            slice.outerRadius = outerRadius;
            slice.startAngle = start;
            slice.endAngle = end;
            slice.gapAngle = gapAngle;
            slice.color = sliceColor;

            // ----- ADD INTERACTION SCRIPT -----
            RadialSliceInteract interact = sliceObj.AddComponent<RadialSliceInteract>();
            interact.slice = slice;
            interact.index = i;
            interact.normalColor = sliceColor;
            interact.hoverColor = new Color(sliceColor.r, sliceColor.g, sliceColor.b, 0.6f);

            // event click
            interact.onClick = (id) =>
            {
            Debug.Log("Slice clicked: " + id);
            };

            RectTransform rt = sliceObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // ----- 2. CREATE ICON -----
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(sliceObj.transform, false);

            Image iconImg = iconObj.GetComponent<Image>();
            RectTransform iconRT = iconObj.GetComponent<RectTransform>();

            if (icons != null && i < icons.Count)
                iconImg.sprite = icons[i];

            iconRT.sizeDelta = iconSize;

            // ANGLE DU CENTRE DU SLICE
            float midAngle = (start + end) * 0.5f;
            float rad = Mathf.Deg2Rad * midAngle;

            // POSITION CENTRÉE SUR LE SLICE
            float iconDistance = (innerRadius + outerRadius) * 0.5f;

            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * iconDistance,
                Mathf.Sin(rad) * iconDistance
            );

            iconRT.anchoredPosition = pos;
        }
        UIController.instance.CloseWheelToolMenu();
    }
}