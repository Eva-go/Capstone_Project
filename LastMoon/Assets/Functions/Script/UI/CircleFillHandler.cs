using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleFillHandler : MonoBehaviour
{
    public Image circleFillImage;
    public RectTransform handlerEdgeImage;
    public RectTransform fillHandler;
    public Text HP;
    public GameObject Hp;
    private float time;
    private float currentFillAmount;
    private bool isHpMax = false;

    // Start is called before the first frame update
    void Start()
    {
        Hp.SetActive(true);
        currentFillAmount = PlayerController.Hp / 100.0f;
        circleFillImage.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHpMax)
        {
            if (currentFillAmount != PlayerController.Hp / 100.0f)
            {
                StopAllCoroutines(); // 현재 실행 중인 코루틴이 있으면 중지
                StartCoroutine(SmoothFill(PlayerController.Hp));
            }
        }
        HP.text = PlayerController.Hp.ToString("F0");

        if (PlayerController.PreViewCam)
        {
            Hp.SetActive(false);
        }
    }

    IEnumerator SmoothFill(float targetHp)
    {
        float startFillAmount = currentFillAmount;
        float endFillAmount = targetHp / 100.0f;
        float duration = 0.5f; // 채워지는 시간 (0.5초)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentFillAmount = Mathf.Lerp(startFillAmount, endFillAmount, elapsed / duration);
            UpdateFillAmount(currentFillAmount);
            yield return null;
        }
        currentFillAmount = endFillAmount;
        UpdateFillAmount(currentFillAmount);
    }

    void UpdateFillAmount(float fillAmount)
    {
        circleFillImage.fillAmount = fillAmount;
        float angle = fillAmount * 360;
        fillHandler.localEulerAngles = new Vector3(0, 0, -angle);
        handlerEdgeImage.localEulerAngles = new Vector3(0, 0, angle);
    }
}