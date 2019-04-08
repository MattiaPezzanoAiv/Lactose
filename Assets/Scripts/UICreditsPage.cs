using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsPage : MonoBehaviour, UIManager.IBackButtonHandler
{
    public RectTransform textPrefab;
    public RectTransform root;
    public float yOffset;
    public float movementSpeed, minScale, maxScale;

    public List<string> lines;

    private List<RectTransform> instantiatedTexts;

    public void OnBackButtonReleased()
    {
        UIManager.Instance.Show("UIMainPage");
    }

    private void OnEnable()
    {
        if (instantiatedTexts == null)
        {
            instantiatedTexts = new List<RectTransform>();

            int step = 0;
            foreach (var l in lines)
            {
                var line = Instantiate(textPrefab, root);
                line.anchoredPosition = new Vector2(0, -(step++ * line.sizeDelta.y + yOffset));
                line.GetComponent<Text>().text = l;
                instantiatedTexts.Add(line);
            }
            Destroy(textPrefab.gameObject);
        }
        else
        {
            ResetPositions();
        }
    }

    void ResetPositions()
    {
        int step = 0;
        foreach (var l in instantiatedTexts)
        {
            l.anchoredPosition = new Vector2(0, -(step++ * l.sizeDelta.y + yOffset));
            l.localScale = new Vector3(maxScale, maxScale, maxScale);
        }
    }

    private void Update()
    {
        float min = 0;
        float max = 1080;

        foreach (var l in instantiatedTexts)
        {
            l.anchoredPosition += Vector2.up * movementSpeed * Time.deltaTime;

            if (l.anchoredPosition.y < min)
                continue;

            float delta = l.anchoredPosition.y / (max - min);
            float newScale = Mathf.Lerp(maxScale, minScale, delta);
            l.localScale = new Vector3(newScale, newScale, newScale);

            
        }

        var last = instantiatedTexts[instantiatedTexts.Count - 1];
        if (last.anchoredPosition.y > max + last.sizeDelta.y)   //all off screen
        {
            ResetPositions();
            AchievmentsManager.Instance.Unlock<CreditsWatched>();
        }
    }
}
