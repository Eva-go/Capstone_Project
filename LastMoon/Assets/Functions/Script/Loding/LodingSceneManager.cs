using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LoadingSceneManager : MonoBehaviourPun
{
    public static string nextScene;
    [SerializeField] Image progressBar;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LoadSceneAsMaster());
        }
        else
        {
            StartCoroutine(LoadSceneAsClient());
        }
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadSceneAsMaster()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                float progress = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                progressBar.fillAmount = progress;
                photonView.RPC("UpdateProgressBar", RpcTarget.Others, progress); // Ŭ���̾�Ʈ�鿡�� ���� ��Ȳ ����
                if (progressBar.fillAmount >= op.progress) { timer = 0f; }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                photonView.RPC("UpdateProgressBar", RpcTarget.Others, 1f); // �ε� �Ϸ� �� 1�� ����ȭ
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    IEnumerator LoadSceneAsClient()
    {
        while (progressBar.fillAmount < 1f)
        {
            yield return null; // ������ Ŭ���̾�Ʈ�κ��� ���� ��Ȳ�� ��ٸ�
        }

        // �ε��� �Ϸ�Ǹ� �� Ȱ��ȭ
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = true;
    }

    [PunRPC]
    void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;
    }
}