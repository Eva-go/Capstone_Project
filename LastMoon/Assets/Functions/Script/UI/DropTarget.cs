using UnityEngine;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour
{
    public Sprite newSprite; // 변경할 이미지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            GetComponent<Image>().sprite = newSprite; // 이미지 변경
            collision.gameObject.GetComponent<DraggableObject>().ResetPosition(); // 원래 위치로 돌아가기
        }
    }
}