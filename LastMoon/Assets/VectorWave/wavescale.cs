using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class wavescale : MonoBehaviour
{
    private float _maxScaleY = 2;
    private float _scaleSpeed = 1f;
    private float _timerInterval = 12f;

    private Vector3 initScale;
    private bool scalingUP = false;

    public PhotonView pv;
    public Vector3 localScale;
    public float maxScaleY
    {
        get { return _maxScaleY; }
        set { _maxScaleY = value; }
    }

    public float scaleSpeed
    {
        get { return _scaleSpeed; }
        set { _scaleSpeed = value; }
    }

    public float timerInterval
    {
        get { return _timerInterval; }
        set { _timerInterval = value; }
    }
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
        StartCoroutine(ScaleOverTime());
        Debug.Log(maxScaleY);
        Debug.Log(scaleSpeed);
        Debug.Log(timerInterval);
    }
    public void Update()
    {
        _maxScaleY = _maxScaleY / 2;
        Debug.Log(transform.localScale);

    }
   
    IEnumerator ScaleOverTime()
    {
        while (true)
        {
            while (true)
            {
                float timer = 0f; // Ÿ�̸� �ʱ�ȭ

                while (timer < timerInterval)
                {
                    // ������ ����Ͽ� �ڿ������� ũ�� ����
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

    }
}
