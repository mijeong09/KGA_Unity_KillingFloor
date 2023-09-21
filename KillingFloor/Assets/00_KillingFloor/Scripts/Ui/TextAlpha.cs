using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextAlpha : MonoBehaviour
{
    private TMP_Text text;

    private Color colorUp = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    private Color colorDown = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    float timeElapsed = 0.0f;

    void Start()
    {
        text = GetComponent<TMP_Text>();

        StartCoroutine(AlphaUp());
    }

    private IEnumerator AlphaUp()
    {
        if (SceneManager.GetActiveScene().name == "MainLoad")
        {
            timeElapsed = 0.0f;

            while (timeElapsed < 2.0f)
            {
                timeElapsed += Time.deltaTime;

                float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 2.0f), 2);

                text.color = Color.Lerp(colorDown, colorUp, time);

                yield return null;
            }

            StartCoroutine(AlphaDown());
        }
    }

    private IEnumerator AlphaDown()
    {
        if (SceneManager.GetActiveScene().name == "MainLoad")
        {
            timeElapsed = 0.0f;

            while (timeElapsed < 2.0f)
            {
                timeElapsed += Time.deltaTime;

                float time = Mathf.Clamp01(timeElapsed / 2.0f);

                text.color = Color.Lerp(colorUp, colorDown, time * time);

                yield return null;
            }

            StartCoroutine(AlphaUp());
        }
    }
}