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
                photonView.RPC("UpdateProgressBar", RpcTarget.Others, progress); // 클라이언트들에게 진행 상황 전송
                if (progressBar.fillAmount >= op.progress) { timer = 0f; }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                photonView.RPC("UpdateProgressBar", RpcTarget.Others, 1f); // 로딩 완료 시 1로 동기화
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
            yield return null; // 마스터 클라이언트로부터 진행 상황을 기다림
        }

        // 로딩이 완료되면 씬 활성화
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = true;
    }

    [PunRPC]
    void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;
    }
}