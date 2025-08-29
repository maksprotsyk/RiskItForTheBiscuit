using System.Collections.Generic;
using Items;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Manager for all pickup item tooltips
// * shows/hides tooltip when hovered over pickup element
// * sets up tooltip info
// * handles hover logic for mouse
namespace Managers
{
    public class TooltipManager : MonoBehaviour
    {
        GameObject tooltip;
        public TextMeshProUGUI TooltipTitleText;
        public TextMeshProUGUI TooltipDescText;
        public Image TooltipSprite;

        // Mouse hover logic variables
        private Canvas canvas;
        private LayerMask uiTargetLayer;
        private LayerMask sceneTargetLayer;
        private GraphicRaycaster raycaster;
        private GameObject lastHoveredObject;

        public void SetAndShowTooltip(ItemDefinition itemDescription)
        {
            if (!tooltip)
            {
                return;
            }

            tooltip.SetActive(true);
            tooltip.transform.SetAsLastSibling();

            // Set tooltip info about object
            if (TooltipTitleText)
            {
                TooltipTitleText.SetText(itemDescription.Name);
            }

            if (TooltipDescText)
            {
                TooltipDescText.SetText(itemDescription.Description);
            }

            if (TooltipSprite)
            {
                TooltipSprite.sprite = itemDescription.Icon;
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
            canvas = FindObjectOfType<Canvas>();
            raycaster = canvas.GetComponent<GraphicRaycaster>();
            uiTargetLayer = LayerMask.GetMask("Drag");
            sceneTargetLayer = LayerMask.GetMask("Drag");

            tooltip = GameObject.Find("ItemTooltip");
            if (!tooltip)
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

                if (!IsTooltipOnTop(tooltip))
                {
                    tooltip.transform.SetAsLastSibling();
                }
            }

            HandleMouseHover();
        }

        bool IsTooltipOnTop(GameObject tooltip)
        {
            Transform parent = tooltip.transform.parent;
            if (parent == null) return false;

            // last sibling index is always childCount - 1
            return tooltip.transform.GetSiblingIndex() == parent.childCount - 1;
        }

        void HandleMouseHover()
        {
            GameObject hoveredObject = null;
        
            // Check 2d scene objects
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            RaycastHit2D hit2D = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, sceneTargetLayer);

            if (hit2D.collider != null && hit2D.collider.gameObject.GetComponent<SpriteRenderer>().isVisible)
            {
                hoveredObject = hit2D.collider.gameObject;
            }
            else
            {
                // Check UI canvas objects
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerEventData, results);

                foreach (var result in results)
                {
                    if (((1 << result.gameObject.layer) & uiTargetLayer) != 0)
                    {
                        hoveredObject = result.gameObject;
                        break;
                    }
                }
            }

            // Trigger enter/exit events
            if (hoveredObject != lastHoveredObject)
            {
                if (lastHoveredObject != null)
                    OnMouseExitEvent(lastHoveredObject);

                if (hoveredObject != null)
                    OnMouseEnterEvent(hoveredObject);

                lastHoveredObject = hoveredObject;
            }
        }

        void OnMouseEnterEvent(GameObject objectEntered)
        {
            if(!objectEntered)
            {
                return;
            }

            //Debug.Log("Mouse entered: " + objectEntered.name);
            ItemTooltip tooltipComp = objectEntered.GetComponent<ItemTooltip>();
            if (tooltipComp)
            {
                tooltipComp.OnMouseEnterHandle();
            }
        }

        void OnMouseExitEvent(GameObject objectExited)
        {
            if (!objectExited)
            {
                return;
            }

            //Debug.Log("Mouse exited: " + objectExited.name);
            ItemTooltip tooltipComp = objectExited.GetComponent<ItemTooltip>();
            if (tooltipComp)
            {
                tooltipComp.OnMouseExitHandle();
            }
        }
    }
}
