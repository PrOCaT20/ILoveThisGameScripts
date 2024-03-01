using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string _localizationKey;

    Text _textComponent;

    IEnumerator Start()
    {
        while (!LocalizationManager.Instance._isReady)
        {
            yield return null;
        }
        Attributiontext();
    }


    public void Attributiontext()
    {
        if(_textComponent == null)
        {
            _textComponent = gameObject.GetComponent<Text>();
        }
        try
        {
            _textComponent.text = LocalizationManager.Instance.GetTextForKey(_localizationKey);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
