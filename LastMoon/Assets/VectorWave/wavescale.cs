using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wavescale : MonoBehaviour
{
    public float maxScaleY = 80;
    public float scaleSpeed = 1f;
    public float timerInterval = 3f;

    private Vector3 initScale;
    private bool scalingUP = false;

    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
        StartCoroutine(ScaleOverTime());
    }

    IEnumerator ScaleOverTime()
    {
        while (true)
        {
            while (true)
            {
                float timer = 0f; // 타이머 초기화

                while (timer < timerInterval)
                {
                    // 보간을 사용하여 자연스러운 크기 변경
                    float progress = timer / timerInterval;
                    float newYScale;
                    if (scalingUP)
                        newYScale = Mathf.Lerp(initScale.y, maxScaleY, Mathf.SmoothStep(0f, 1f, progress));
                    else
                        newYScale = Mathf.Lerp(maxScaleY, initScale.y, Mathf.SmoothStep(0f, 1f, progress));

                    transform.localScale = new Vector3(initScale.x, newYScale, initScale.z);

                    timer += Time.deltaTime;
                    yield return null;
                }

                scalingUP = !scalingUP;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (scalingUP)
            {
                float newYScale = Mathf.MoveTowards(transform.localScale.y, maxScaleY, scaleSpeed * Time.deltaTime);
                transform.localScale = new Vector3(initScale.x, newYScale, initScale.z);
            }
            else
            {
                float newYScale = Mathf.MoveTowards(transform.localScale.y, initScale.y, scaleSpeed * Time.deltaTime);
                transform.localScale = new Vector3(initScale.x, newYScale, initScale.z);
            }
        }
    }
}
