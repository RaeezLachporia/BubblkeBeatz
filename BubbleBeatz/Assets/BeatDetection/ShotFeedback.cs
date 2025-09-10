using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShotFeedback : MonoBehaviour
{
    public SpectrumAnalyzer spectrumAnny;
    public TextMeshProUGUI feedbackText;
    //public Transform playerTransform;
    //public Vector3 offSet = new Vector3(0, 2, 0);
    public float perfectThreshHold = 0.1f;
    public float lateThreshHold = 0.25f;

    private float lastBeatTime;
    public void Start()
    {
        feedbackText.text = " ";
    }
    private void Update()
    {
        //transform.position = playerTransform.position + offSet;

    }

    public void ShowShootTimingFeedback()
    {
        lastBeatTime = spectrumAnny.GetLastBeatTime();
        float timeDiff = Mathf.Abs(Time.time - lastBeatTime);
        if (timeDiff <= perfectThreshHold)
        {
            feedbackText.text = "ON BEAT!";
            feedbackText.color = Color.green;
        }
        else if (Time.time<lastBeatTime)
        {
            feedbackText.text = "Early";
            feedbackText.color = Color.yellow;
        }
        else
        {
            feedbackText.text = "Late!";
            feedbackText.color = Color.red;
        }
        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), 0.5f);
    }

    private void ClearFeedback()
    {
        feedbackText.text = " ";
    }
}
