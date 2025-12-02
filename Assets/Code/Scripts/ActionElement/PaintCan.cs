using UnityEngine;
using UnityEngine.UI;

public class PaintCan : MonoBehaviour
{
    [SerializeField] private Slider paintSlider;
    public int numberUsage = 5;
    public Material matPaint;

    void Start()
    {
        paintSlider.maxValue = numberUsage;
        paintSlider.value = numberUsage;
    }

    public bool UsePaintCan()
    {
        if( numberUsage > 0)
        {
            numberUsage--;
            paintSlider.value = numberUsage;
            return true;
        } else
        {
            return false;
        }
    }
}
