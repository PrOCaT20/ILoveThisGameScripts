using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Text ResolutionText;
    private int actualResolutionIndex = 0;
    private Resolution[] possibleResolutions;

    public GameObject graphicsOptions, audioOptions, generalOptions;
    public Text optionText;
    private int optionIndex = 0;
    private string[] options;

    public Text languageText;
    private int languageIndex = 0;
    private string[] languageOptions = { "English", "Русский", "Català", "Українська", "Español" };

    public Slider musicSlider;
    public Slider soundSlider;
    public AudioSource MusicAudioSource, SoundAudioSource;

    void Start()
    {
        options = new string[3];
        StartCoroutine(WaitForLocIsReady()); 
        musicSlider.value = PlayerPrefs.GetFloat("MusicValue");
        soundSlider.value = PlayerPrefs.GetFloat("SoundValue");
        MusicAudioSource.volume = PlayerPrefs.GetFloat("MusicValue");
        SoundAudioSource.volume = PlayerPrefs.GetFloat("SoundValue");
        UpdateOptionText();
        possibleResolutions = new Resolution[]
        {
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 1600, height = 900 }
        };

        ChangeResolution(0);
    }
    IEnumerator WaitForLocIsReady() // metode: avans d'utilitzar el LocalizationManager, espera que estigui actiu i preparat per funcionar
    {
        yield return StartCoroutine(LocalizationManager.Instance.Start());
        options[0] = LocalizationManager.Instance.GetTextForKey("settingsGeneral");
        options[1] = LocalizationManager.Instance.GetTextForKey("settingsGraphics");  //atribuïm text dels diferents temes d'ajustament quan LocManager esta isReady
        options[2] = LocalizationManager.Instance.GetTextForKey("settingsAudio");
        UpdateOptionText();
    }

    void UpdateOptionText()
    {
        optionText.text = options[optionIndex];
        generalOptions.SetActive(optionIndex == 0);
        graphicsOptions.SetActive(optionIndex == 1);
        audioOptions.SetActive(optionIndex == 2);
    }

    public void ClickRightArrow() // management de les fletxes per mirar opcions
    {
        optionIndex = (optionIndex + 1) % options.Length;
        UpdateOptionText();
    }

    public void ClickLeftArrow()
    {
        optionIndex = (optionIndex - 1 + options.Length) % options.Length;
        UpdateOptionText();
    }

    public void ClickRightArrowLan()
    {
        languageIndex = (languageIndex + 1) % languageOptions.Length;
        languageText.text = languageOptions[languageIndex];
    }

    public void ClickLeftArrowLan()
    {
        languageIndex = (languageIndex - 1 + languageOptions.Length) % languageOptions.Length;
        languageText.text = languageOptions[languageIndex];
    }

    public void ApplyLanguage() // boto per aplicar el llenguatge escollit depenent del index de la opcio possible
    {
        switch (languageIndex)
        {
            case 0: LocalizationManager.Instance.ChangeLanguage("EN"); break;
            case 1: LocalizationManager.Instance.ChangeLanguage("RU"); break;
            case 2: LocalizationManager.Instance.ChangeLanguage("CA"); break;
            case 3: LocalizationManager.Instance.ChangeLanguage("UA"); break;
            case 4: LocalizationManager.Instance.ChangeLanguage("ES"); break;
            default: LocalizationManager.Instance.ChangeLanguage("EN"); break;
        }
        SceneManager.LoadScene("Menu");
    }

    public void ChangeResolution(int increment)  // boto per aplicar la resolucio de la pantalla
    {
        actualResolutionIndex += increment;

        if (actualResolutionIndex < 0)
        {
            actualResolutionIndex = possibleResolutions.Length - 1;
        }
        else if (actualResolutionIndex >= possibleResolutions.Length)
        {
            actualResolutionIndex = 0;
        }

        Resolution novaResolucio = possibleResolutions[actualResolutionIndex];
        Screen.SetResolution(novaResolucio.width, novaResolucio.height, Screen.fullScreen);
        ResolutionText.text = possibleResolutions[actualResolutionIndex].width + "x" + possibleResolutions[actualResolutionIndex].height;
    }

    public void ChangeMusicVolume()  // quan el value del slider de musica es canvia, ajusta el "volume" de la musica amb el value del slider && Guarda el value a la memoria (PlayerPrefs.SetX)
    {
        PlayerPrefs.SetFloat("MusicValue", musicSlider.value);
        MusicAudioSource.volume = musicSlider.value;
    }

    public void ChangeSoundVolume() // quan el value del slider del so es canvia, ajusta el "volume" del so amb el value del slider && Guarda el value a la memoria (PlayerPrefs.SetX)
    {
        PlayerPrefs.SetFloat("SoundValue", soundSlider.value);
        SoundAudioSource.volume = soundSlider.value;
    }
}
