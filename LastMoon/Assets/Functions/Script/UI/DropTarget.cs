using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropTarget: MonoBehaviour
{
    //이미지 충돌처리 코드
    
    public GameObject canvas;

    private AddNode poiAddNode;

    private GraphicRaycaster raycaster;
    private PointerEventData pointer;
    private EventSystem eventSystem;

    private bool nodeTarget = false;

    private string nodeCountck;
    private string []collisionNamae;

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
        if (Input.GetMouseButtonUp(0) && nodeTarget)
        {
            if (nodeCountck == "node1")
            {
                poiAddNode.addNode1(collisionName1);
            }
            else if (nodeCountck == "node2")
            {
                poiAddNode.addNode2(collisionName2);
            }
            else
                nodeCountck = "null";
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            Debug.Log("노드 충돌");
            Ray(collision);
            nodeTarget = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        nodeTarget = false;
    }
}