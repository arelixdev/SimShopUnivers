using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkerMenu : MonoBehaviour
{
    public static WorkerMenu instance;

    public WorkerController workerSelected;

    [SerializeField] private GameObject workerMenu;


    [SerializeField] private TMP_Dropdown shopDropDown;
    [SerializeField] private TMP_Dropdown typeWorkerDropDown;

    public GameObject GetWorkerMenu()
    {
        return workerMenu;
    }

    private void Awake()
    {
        instance = this;

        CloseMenu();
        ActualizeTypeWorkerDropDown();
    }

    private TypeWorkers typeWorker;

    private void ActualizeTypeWorkerDropDown()
    {
        Type enumType = typeWorker.GetType();
        List<TMP_Dropdown.OptionData> newOptions = new();

        for (int i = 0; i < Enum.GetNames(enumType).Length; i++)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(enumType, i)));
        }

        typeWorkerDropDown.ClearOptions();
        typeWorkerDropDown.AddOptions(newOptions);
    }

    private void ActualizeShopDropDown()
    {
        List<TMP_Dropdown.OptionData> newOptions = new();

        newOptions.Add(null);

        for (int i = 0; i < PanelShopMaster.instance.listShopCreated.Count; i++)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(PanelShopMaster.instance.listShopCreated[i].shopName));
        }

        shopDropDown.ClearOptions();
        shopDropDown.AddOptions(newOptions);
    }

    public void DropDownChanged()
    {
        Enum.TryParse(typeWorkerDropDown.options[typeWorkerDropDown.value].text,  out TypeWorkers workering);
        workerSelected.GetComponent<WorkerController>().DoAction(shopDropDown.options[shopDropDown.value].text, workering);
    }

    public void OpenMenu(WorkerController worker)
    {
        workerMenu.SetActive(true);
        ActualizeShopDropDown();
        workerSelected = worker;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseMenu()
    {
        workerMenu.SetActive(false);
        workerSelected = null;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
