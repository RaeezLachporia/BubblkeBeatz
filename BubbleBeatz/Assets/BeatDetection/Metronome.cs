using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Metronome : MonoBehaviour
{
    public SpectrumAnalyzer spectrumanalyzer;
    public Image metronomeBar;

    public float pulseDuration = 0.15f;
    public Color normalColor = new Color(1f, 1f, 1f, 0.2f);
    public Color beatColor = new Color(0f, 1f, 1f, 1f);
    private float timer = 0f;


    public void Start()
    {
        if (metronomeBar==null)
        {
            metronomeBar = GetComponent<Image>();
            metronomeBar.color = normalColor;
        }
    }
    public void Update()
    {
        if (spectrumanalyzer.isBassBeatDetected())
        {
            timer = pulseDuration;
            metronomeBar.color = beatColor;
            metronomeBar.rectTransform.localScale = new Vector3(1f, 1.5f, 1f);
        }
        if (timer>0f)
        {
            timer -= Time.deltaTime;

        }
        else
        {
            metronomeBar.color = Color.Lerp(metronomeBar.color, normalColor, Time.deltaTime * 10f);
            metronomeBar.rectTransform.localScale = Vector3.Lerp(metronomeBar.rectTransform.localScale, Vector3.one, Time.deltaTime * 10f);
        }
    }
}
