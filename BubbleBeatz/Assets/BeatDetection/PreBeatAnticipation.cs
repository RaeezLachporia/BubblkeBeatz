using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PreBeatAnticipation : MonoBehaviour
{
    public SpectrumAnalyzer analyzing;
    public Image pulseCue;
    public Color cueColor = Color.cyan;
    public float duration = 0.2f;

    private void Start()
    {
        if (analyzing != null)
        {
            analyzing.OnPrebeatWarning += FlashCue;
        }
    }

    private void FlashCue()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        pulseCue.color = cueColor;
        pulseCue.enabled = true;
        yield return new WaitForSeconds(duration);
        pulseCue.enabled = false;
    }
}
