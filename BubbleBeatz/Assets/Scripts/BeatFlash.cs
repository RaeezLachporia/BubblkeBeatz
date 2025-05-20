using UnityEngine;
using UnityEngine.UI;

public class BeatFlash : MonoBehaviour
{
    public SpectrumAnalyzer analyzer;
    public Image flashOverlay;

    [Header("Flash Settings")]
    public Color flashColor = Color.magenta;
    public float flashAlpha = 0.3f;
    public float pulseDuration = 0.2f;
    public float fadeSpeed = 2f;
    public bool randomizeColor = false;

    private float timer;
    private Color transparentColor;

    void Start()
    {
        transparentColor = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
        flashOverlay.color = transparentColor;
    }

    void Update()
    {
        if (analyzer != null && analyzer.isBassBeatDetected())
        {
            timer = pulseDuration;

            Color finalColor;
            if (randomizeColor)
            {
                Color randomColor = Color.HSVToRGB(Random.value, 0.7f, 1f);
                finalColor = new Color(randomColor.r, randomColor.g, randomColor.b, flashAlpha);
            }
            else
            {
                finalColor = new Color(flashColor.r, flashColor.g, flashColor.b, flashAlpha);
            }

            flashOverlay.color = finalColor;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            // Fade out to transparent
            flashOverlay.color = Color.Lerp(flashOverlay.color, transparentColor, Time.deltaTime * fadeSpeed);
        }
    }
}