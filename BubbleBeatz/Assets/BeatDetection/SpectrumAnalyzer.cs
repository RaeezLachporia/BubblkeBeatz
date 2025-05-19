using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour
{
    public int spectrumSize = 1024;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    public float[] spectrum;
    private AudioSource audioSource;
    private float lastBeatTime=-999f;
    private float beatLeeway = 0.15f;
    private bool isBeat;
    public float sensitivity = 1.5f;
    public float beatCooldown = 0.15f;
    public int bassBandCount = 20;
    private float[] historyBuffer;
    public int historyIndex = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spectrum = new float[spectrumSize];
        historyBuffer = new float[43];
        bassBandCount = Mathf.Max(5,GetBassBandLimit(100f));
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);
        float bassEnergy = 0f;
        for (int i = 0; i < bassBandCount; i++)
        {
            
                bassEnergy += spectrum[i];
            
        }
        float averageEnergy = 0f;
        foreach (float value in historyBuffer)
        {
            averageEnergy += value;
        }
        averageEnergy /= historyBuffer.Length;
        historyBuffer[historyIndex] = bassEnergy;
        historyIndex = (historyIndex + 1) % historyBuffer.Length;
        if (bassEnergy > averageEnergy * sensitivity && Time.time - lastBeatTime > beatCooldown)
        {
            isBeat = true;
            lastBeatTime = Time.time;

        }
        else
        {
            isBeat = false;
        }
        if (isBeat)
        {
            //Debug.Log($"Beat detected at {Time.time:F2}");
        }

        //float freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrumSize;
        //Debug.Log("Peak frequency: " + freq + "Hz");
    }
    public bool IsBeatDetected()
    {
        return isBeat;
    }
    public bool isBassBeatDetected(float leeway =0.15f)
    {
        float bassEnergy = 0f;
        for (int i = 0; i < bassBandCount; i++)
        {
            bassEnergy += spectrum[i];
        }
        float averageEnergy = 0f;
        foreach (float value in historyBuffer)
        {
            averageEnergy += value;
        }
        averageEnergy /= historyBuffer.Length;
        float currentTime = Time.time;
        bool beatRecentlyDetected = currentTime - lastBeatTime <= leeway;
        bool isBassPeak = bassEnergy > averageEnergy * sensitivity;
        return beatRecentlyDetected && isBassPeak;
       
    }
    public int GetBassBandLimit(float maxFreq = 100f)
    {
        float sampleRate = AudioSettings.outputSampleRate;
        float freqPerBand = sampleRate / 2f / spectrumSize;
        return Mathf.FloorToInt(maxFreq/freqPerBand);
    }
}
