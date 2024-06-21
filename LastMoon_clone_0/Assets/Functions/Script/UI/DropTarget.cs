using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropTarget: MonoBehaviour
{
    //�̹��� �浹ó�� �ڵ�
    
    public GameObject canvas;

    private AddNode poiAddNode;

    private GraphicRaycaster raycaster;
    private PointerEventData pointer;
    private EventSystem eventSystem;

    private bool nodeTarget = false;

    private string nodeCountck;

    private Collider2D collisionName1;
    private Collider2D collisionName2;

    private void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        poiAddNode = transform.GetComponentInParent<AddNode>();
    }

    private void Update()
    {
        if (nodeCountck == "node1"&& Input.GetMouseButtonUp(0)&&nodeTarget)
        {
            Debug.Log("���1");
            poiAddNode.addNode1(collisionName1);
        }
        else if (nodeCountck == "node2"&& Input.GetMouseButtonUp(0) && nodeTarget)
        {
            Debug.Log("���");
            poiAddNode.addNode2(collisionName2);
        }
        
    }

    private void Ray(Collider2D collision)
    {
        pointer = new PointerEventData(eventSystem);
        pointer.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointer, results);
        GameObject hit = results[0].gameObject;
        if (hit.name == "node1")
        {
            nodeCountck = hit.name;
            collisionName1 = collision;
        }
        else if (hit.name == "node2")
        {
            nodeCountck = hit.name;
            collisionName2= collision;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        nodeTarget = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            Ray(collision);
            nodeTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        nodeTarget = false;
    }
}