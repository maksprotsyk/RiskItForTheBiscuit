using Characters.Player;
using Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    GameObject tooltip;
    public TextMeshProUGUI tooltipTitleText;

    public void SetAndShowTooltip(ItemDefinition itemDescription)
    {
        if (!tooltip)
        {
            return;
        }

        tooltip.SetActive(true);
        tooltip.transform.SetAsLastSibling();

        // Set tooltip info about object
        if (tooltipTitleText != null)
        {
            tooltipTitleText.SetText(itemDescription.Kind.ToString());
        }
    }

    public void HideTooltip()
    {
        if (tooltip)
        {
            tooltip.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tooltip = GameObject.Find("ItemTooltip");
        if (tooltip == null)
        {
            Debug.Log("Cannot find Tooltip object");
        }
        else
        {
            tooltip.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tooltip)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }


}
