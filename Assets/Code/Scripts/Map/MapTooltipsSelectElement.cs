using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapTooltipsSelectElement : MonoBehaviour
{
    [SerializeField] private Transform gridBtnElement;
    [SerializeField] private GameObject btnElement;
    private void Start() {
        HideMenu();
    }

    public void ShowMenu(ElementType type)
    {
        gameObject.SetActive(true);

        Init(type);
        
        StartCoroutine(RebuildNextFrame());
    }

    private void Init(ElementType type)
    {
        foreach (Transform child in gridBtnElement.transform) {
            Destroy(child.gameObject);
        }

        switch(type)
        {
            case ElementType.Door:
                for (int i = 0; i < StockInfoController.instance.allDoors.Count; i++)
                {

                    GameObject btn = Instantiate(btnElement, gridBtnElement);
                    btn.GetComponent<CustomElementBtn>().Init(StockInfoController.instance.allDoors[i].spriteElement, i);
                }
                break;
            default:
                break;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
        
    }

    private IEnumerator RebuildNextFrame()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
