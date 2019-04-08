using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialPage : MonoBehaviour
{

    [SerializeField]
    UnityEngine.UI.Image her;
    [SerializeField]
    UnityEngine.UI.Text message;

    private void Awake()
    {
        GfxActive(false);
        HideMessage();
    }

    public void ShowMessage(string message)
    {
        this.message.gameObject.SetActive(true);
        this.message.text = message;
    }
    public void HideMessage()
    {
        message.gameObject.SetActive(false);
    }
    public void SetGfx(Sprite s)
    {
        her.sprite = s;
    }
    public void GfxActive(bool v)
    {
        her.gameObject.SetActive(v);
    }
}
