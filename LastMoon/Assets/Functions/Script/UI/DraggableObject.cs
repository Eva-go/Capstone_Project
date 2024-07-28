using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform startParent;
    private CanvasGroup canvasGroup;

    public Text Text_count;
    public int count;
    private void Awake()
    {
        Text_count.text = "0";
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogWarning("CanvasGroup 컴포넌트가 " + gameObject.name + " 오브젝트에 없습니다. 드래그 기능에 문제가 발생할 수 있습니다.");
        }
        count = int.Parse(Text_count.text);
    }

    private void Update()
    {
        count = int.Parse(Text_count.text);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(count>0)
        {
            transform.position = Input.mousePosition;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.SetParent(startParent);
    }

}