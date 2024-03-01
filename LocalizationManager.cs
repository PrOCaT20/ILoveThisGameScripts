using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour
{
    private const string FILENAME_PREFIX = "text_";
    private const string FILE_EXTENSION = ".json";
    public string FULL_NAME_TEXT_FILE;
    private string FULL_PATH_TEXT_FILE;

    public bool _isReady = false;
    private Dictionary<string, string> _localizedDictionary;

    private static GameObject _locInstance;
    private static LocalizationManager LocalizationManagerInstance;

    public static LocalizationManager Instance
    {
        get
        {
            if (LocalizationManagerInstance == null)
            {
                LocalizationManagerInstance = FindObjectOfType(typeof(LocalizationManager)) as LocalizationManager;
            }
            return LocalizationManagerInstance;
        }
    }

    private void Awake()
    {
        if (_locInstance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _locInstance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public IEnumerator Start()
    {
        FULL_NAME_TEXT_FILE = FILENAME_PREFIX + PlayerPrefs.GetString("Language").ToLower() + FILE_EXTENSION;
        FULL_PATH_TEXT_FILE = Path.Combine(Application.streamingAssetsPath, FULL_NAME_TEXT_FILE);

        yield return StartCoroutine(LoadJsonLanguageData());
        _isReady = true;
    }

    public bool IsReady => _isReady;

    public IEnumerator LoadJsonLanguageData()
    {
        if (File.Exists(FULL_PATH_TEXT_FILE))
        {
            string jsonText = File.ReadAllText(FULL_PATH_TEXT_FILE);
            LocalizationData data = JsonUtility.FromJson<LocalizationData>(jsonText);

            _localizedDictionary = new Dictionary<string, string>();
            foreach (LocalizationItems item in data.items)
            {
                _localizedDictionary[item.key] = item.value;
            }
        }
        else
        {
            Debug.LogError("Language file not found: " + FULL_PATH_TEXT_FILE);
        }

        yield return null;
    }

    public string GetTextForKey(string localizationKey)
    {
        if (_isReady && _localizedDictionary.ContainsKey(localizationKey))
        {
            return _localizedDictionary[localizationKey];
        }
        else
        {
            return "Key " + localizationKey + " not found";
        }
    }

    IEnumerator SwitchLanguageRuntime(string langChoose)
    {
        if (!_isReady)
        {
            yield break;
        }

        PlayerPrefs.SetString("Language", langChoose);
        FULL_NAME_TEXT_FILE = FILENAME_PREFIX + langChoose.ToLower() + FILE_EXTENSION;
        FULL_PATH_TEXT_FILE = Path.Combine(Application.streamingAssetsPath, FULL_NAME_TEXT_FILE);

        yield return LoadJsonLanguageData();

        LocalizedText[] arrayText = FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText text in arrayText)
        {
            text.Attributiontext();
        }
    }

    public void ChangeLanguage(string lang)
    {
        StartCoroutine(SwitchLanguageRuntime(lang));
    }
}
