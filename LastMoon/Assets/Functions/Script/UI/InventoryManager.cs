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
        // �� object_fild���� ������ �����Ͽ� Ʃ�� ����Ʈ�� ����
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

        // ������ ���� ������Ʈ ���� (��������)
        objectScores.Sort((x, y) => y.Item2.CompareTo(x.Item2));

        // ���ĵ� ������Ʈ�� Scroll View�� �ٽ� ��ġ
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject); // ���� �ڽ� ������Ʈ ����
        }

        foreach ((GameObject obj, int score) in objectScores)
        {
            obj.transform.SetParent(scrollViewContent.transform, false);
        }
    }
}
