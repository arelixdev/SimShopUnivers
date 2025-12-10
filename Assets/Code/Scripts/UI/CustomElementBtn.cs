using UnityEngine;
using UnityEngine.UI;

public class CustomElementBtn : MonoBehaviour
{
    [SerializeField] private Image spriteIcn;
    [SerializeField] private int numberValueDoor;

    public void Init(Sprite img, int value)
    {
        spriteIcn.sprite = img;
        numberValueDoor = value;
    }

    public void ClickOnElementBtn()
    {
        MapTooltipsPanel.instance.CreateElement(numberValueDoor);
    }
}
