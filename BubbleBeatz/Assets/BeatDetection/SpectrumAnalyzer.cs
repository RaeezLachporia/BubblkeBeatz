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
    public float bassMaxFrequency = 40f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spectrum = new float[spectrumSize];
        historyBuffer = new float[43];
        bassBandCount = Mathf.Max(3,GetBassBandLimit(bassMaxFrequency));
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);
        float bassEnergy = 0f;
        float maxVal = 0f;
        for (int i = 0; i < bassBandCount; i++)
        {
            if (spectrum[i]>0.005f)
            {
                bassEnergy += spectrum[i];
                maxVal = Mathf.Max(maxVal, spectrum[i]);
            }
            
        }
        float averageEnergy = 0f;
        foreach (float value in historyBuffer)
        {
            averageEnergy += value;
        }
        averageEnergy /= historyBuffer.Length;
        historyBuffer[historyIndex] = bassEnergy;
        historyIndex = (historyIndex + 1) % historyBuffer.Length;
        bool spike = maxVal > 0.01f;
        bool beatDetected = bassEnergy > averageEnergy * sensitivity && Time.time - lastBeatTime > beatCooldown;
        if (beatDetected)
        {
            isBeat = true;
            lastBeatTime = Time.time;

        }
        else
        {
            isBeat = false;
        }
        

       
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
        bool beatDetected = bassEnergy > averageEnergy * sensitivity;
        float currentTime = Time.time;
        bool inBeatWindow = currentTime - lastBeatTime < leeway;

        return beatDetected && inBeatWindow;
    }
    public int GetBassBandLimit(float maxFreq = 100f)
    {
        float sampleRate = AudioSettings.outputSampleRate;
        float freqPerBand = sampleRate / 2f / spectrumSize;
        return Mathf.FloorToInt(maxFreq/freqPerBand);
    }
}
