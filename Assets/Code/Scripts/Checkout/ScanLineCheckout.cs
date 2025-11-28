using System;
using TMPro;
using UnityEngine;

public class ScanLineCheckout : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI quantityTxt;
    [SerializeField] private TextMeshProUGUI valueTxt;

    internal void UpdateLine(StockInfoSO info)
    {
        nameTxt.text = info.name;
        valueTxt.text = info.currentPrice.ToString("F2") + " €";
    }
}
