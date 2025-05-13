using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour
{
    public int spectrumSize = 512;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    public float[] spectrum;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spectrum = new float[spectrumSize];
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);
        float max = 0f;
        int maxIndex = 0;
        for (int i = 0; i < spectrumSize; i++)
        {
            if (spectrum[i] >max)
            {
                max = spectrum[i];
                maxIndex = i;
            }
        }

        float freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrumSize;
        Debug.Log("Peak frequency: " + freq + "Hz");
    }
    public bool IsBeatDetected()
    {
        float bassEnergy = 0f;
        for (int i = 0; i < 20; i++)
        {
            bassEnergy += spectrum[i];
        }
        return bassEnergy > 0.1f;
    }
}
