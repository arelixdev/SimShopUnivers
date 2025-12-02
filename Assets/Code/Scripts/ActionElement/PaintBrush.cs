using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    [SerializeField] private GameObject paintObj;
    [SerializeField] private bool hasPaint;

    public void AddPaintOnBrush()
    {
        paintObj.SetActive(true);
        hasPaint = true;
    }

    public void RemovePaintOnBrush()
    {
        paintObj.SetActive(false);
        hasPaint = false;
    }

    public bool HasPaint()
    {
        return hasPaint;
    }
}
