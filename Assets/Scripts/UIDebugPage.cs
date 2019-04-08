using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if M_DEBUG
public class UIDebugPage : MonoBehaviour
{
    [SerializeField]
    RectTransform body;
    [SerializeField]
    InputField ptsField;
    [SerializeField]
    Text ptsPerObject;


    [Header("Logs")]
    public UILogItem logItemPrefab;
    public ScrollRect scroller;
    public Text logDescription;

    private void Awake()
    {
        Application.logMessageReceived += (condition, stackTrace, logType) =>
        {
            var inst = Instantiate(logItemPrefab, scroller.content);
            inst.Setup(condition, stackTrace, logType);
            inst.gameObject.SetActive(true);

            inst.btn.onClick.AddListener(() => 
            {
                logDescription.text = condition + "\n" + stackTrace;
            });
        };

        body.gameObject.SetActive(false);
    }

    public void ToggleBody()
    {
        body.gameObject.SetActive(!body.gameObject.activeSelf);

        if (GameManager.Instance != null)
            ptsPerObject.text = "Pts per obj " + GameManager.Instance.ptsPerObject.ToString();
    }
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SetScorePerObject()
    {
        var amount = int.Parse(ptsField.text);
        GameManager.Instance.ptsPerObject = amount;
        ptsPerObject.text = "Pts per obj " + GameManager.Instance.ptsPerObject.ToString();
    }


}
#endif
