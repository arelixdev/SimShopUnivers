using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    [SerializeField] private GameObject paintObj;
    [SerializeField] private bool hasPaint;
    public Material brushPaintMat;

    public void AddPaintOnBrush(Material paintMat)
    {
        paintObj.SetActive(true);
        brushPaintMat = paintMat;
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
