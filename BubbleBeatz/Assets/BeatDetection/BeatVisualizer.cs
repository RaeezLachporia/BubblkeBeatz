using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatVisualizer : MonoBehaviour
{
    public SpectrumAnalyzer analyzer;
    public Image pulseImage;
    public float pulseScale = 1.2f;
    public float pulseDuration = 0.2f;
    private Vector3 originalScale;
    private float timer;

    private void Start()
    {
        originalScale = pulseImage.rectTransform.localScale;
    }

    private void Update()
    {
        if (analyzer.IsBeatDetected())
        {
            timer = pulseDuration;
            pulseImage.rectTransform.localScale = originalScale * pulseScale;

        }
        if (timer >0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            pulseImage.rectTransform.localScale = Vector3.Lerp(pulseImage.rectTransform.localScale, originalScale, Time.deltaTime * 10f);
        }
    }
}
