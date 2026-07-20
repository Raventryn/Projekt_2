using TMPro;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class KeybindTips : MonoBehaviour
{
    [SerializeField] GameObject scannerTipTextContainer;
    [SerializeField] GameObject inventoryTipTextContainer;

    bool isScannerTipSown;
    bool isInventoryTipShown;

    void OnEnable()
    {
        GameEventsManager.instance.uiEvents.onShowScannerTooltip += ToggleScannerText;
        GameEventsManager.instance.uiEvents.onShowInventoryTooltip += ToggleInventoryText;
    }

    void OnDisable()
    {
        GameEventsManager.instance.uiEvents.onShowScannerTooltip -= ToggleScannerText;
        GameEventsManager.instance.uiEvents.onShowInventoryTooltip -= ToggleInventoryText;
    }

    void Start()
    {
        scannerTipTextContainer.SetActive(false);
        inventoryTipTextContainer.SetActive(false);
    }

    void ToggleScannerText(bool toggle)
    {

        if (!isScannerTipSown)
        {
            scannerTipTextContainer.SetActive(toggle);
            isScannerTipSown = true;
        }
        else if(scannerTipTextContainer.activeSelf)
        {
            scannerTipTextContainer.SetActive(false);
            CheckTipCompletion();
        }   
    }

    void ToggleInventoryText(bool toggle)
    {

        if (!isInventoryTipShown)
        {
            inventoryTipTextContainer.SetActive(toggle);
            isInventoryTipShown = true;
        }
        else if(inventoryTipTextContainer.activeSelf)
        {
            inventoryTipTextContainer.SetActive(false);
            CheckTipCompletion();
        }  
    }

    void CheckTipCompletion()
    {
        if(isScannerTipSown && isInventoryTipShown)
        {
            this.enabled = false;
        }
    }
}
