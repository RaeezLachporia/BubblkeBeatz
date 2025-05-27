using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PreBeatAnticipation : MonoBehaviour
{
    public SpectrumAnalyzer analyzing;
    public Image pulseCue;
    public Color cueColor = Color.cyan;
    public float duration = 0.2f;
    public TextMeshProUGUI cueText;
    private void Start()
    {
        if (analyzing != null)
        {
            analyzing.OnPrebeatWarning += FlashCue;
            analyzing.OnPrebeatWarning += ShowCue;
            cueText.gameObject.SetActive(false);
        }
    }
    public void ShowCue()
    {
        cueText.text = " Get Ready!";
        cueText.color = Color.cyan;
        cueText.gameObject.SetActive(true);
        Invoke(nameof(HideCue), 0.3f);
    }
    private void HideCue()
    {
        cueText.gameObject.SetActive(false);
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
