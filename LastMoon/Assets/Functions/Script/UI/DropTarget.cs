using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropTarget : MonoBehaviour
{
    public GameObject canvas;
    private GraphicRaycaster raycaster;
    private PointerEventData pointer;
    private EventSystem eventSystem;

    public Sprite nodeAlpha;
    public Text[] nodeCounts;
    public Sprite[] nodeImage;
    private string[] nodeName = { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap", "Alpha" };
    private int nodeAdd = 0;
    private bool nodeTarget = false;

    private DraggableObject nodeCount;
    private void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonUp(0)&& nodeTarget)
        {
            gameObject.GetComponent<Image>().sprite = nodeImage[nodeAdd];
        }
    }

    private void Ray(Collider2D collision)
    {
        pointer = new PointerEventData(eventSystem);
        pointer.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointer, results);
        GameObject hit = results[0].gameObject;

        if (hit.tag == "MixNode")
        {
            for (int i = 0; i < nodeName.Length; i++)
            {
                if (collision.name == nodeName[i])
                {
                    nodeAdd = i;
                    break;
                }

            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            Ray(collision);
            nodeTarget = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        nodeTarget = false;
    }

    public void poi_Exit()
    {
        nodeAdd = 7;
        gameObject.GetComponent<Image>().sprite = nodeAlpha;
    }

}