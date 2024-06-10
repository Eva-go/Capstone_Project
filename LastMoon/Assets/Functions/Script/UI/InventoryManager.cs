using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    public GameObject scrollViewContent; // Scroll View Content
    public List<GameObject> nodeFields; // List of object_fild GameObjects
    private GameObject node;

    public string[] nodeNames = { "Dirt", "Concrete", "wood", "Sand", "Planks", "Scrap" };

    // Update is called once per frame
    private void Start()
    {
        node = GameObject.Find("CanvasController");
    }
    void Update()
    {
        UpdateRanking();
    }
    public void UpdateRanking()
    {
        // 각 object_fild에서 점수를 추출하여 튜플 리스트를 만듦
        List<(GameObject, int)> objectScores = new List<(GameObject, int)>();
        foreach (GameObject obj in nodeFields)
        {
            int count=0;
            Text textComponent = obj.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                int score;
                if (int.TryParse(textComponent.text, out score))
                {
                    objectScores.Add((obj, score));
                }
            }
            foreach (string searchString in nodeNames)
            {
                if(obj.name.Contains(searchString))
                {
                    count += 1;
                    textComponent.text = count.ToString();
                }
            }
            
        }

        // 점수에 따라 오브젝트 정렬 (내림차순)
        objectScores.Sort((x, y) => y.Item2.CompareTo(x.Item2));

        // 정렬된 오브젝트를 Scroll View에 다시 배치
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject); // 기존 자식 오브젝트 삭제
        }

        foreach ((GameObject obj, int score) in objectScores)
        {
            obj.transform.SetParent(scrollViewContent.transform, false);
        }
    }
}
