using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private Grid[] gridMaps;
    public SpriteRenderer[] OtherBorders;
    private SpriteRenderer PlayerRenderer;
    private GameObject Door;
    private Text[] ColorText;
    private TextMeshPro[] ColorTextTMP;
    public Image[] ImageColor;
    private GameObject[] SoundEmitter;
    public GameObject tipManager;
    private GameObject tipButton;
    public float tipSeconds = 3f;
    public bool allowReload = true;
    private Text levelText;
    private static GameManager _instance;
    public AudioSource[] otherSoundsCotroll;

    private Sprite OpenDoorSprite, CloseDoorSprite;
    private AudioClip OpenDoorSound, CloseDoorSound;

    public static GameManager Instance { get { if (_instance == null) { _instance = FindObjectOfType<GameManager>(); } return _instance; } }

    void Awake()
    {
        levelText = GameObject.Find("LevelInt").GetComponent<Text>();
           if (levelText != null)
               levelText.text = LocalizationManager.Instance.GetTextForKey("level") + " " + SceneManager.GetActiveScene().name;

        tipManager.SetActive(true);
        tipButton = GameObject.Find("TipButton");

        if (PlayerPrefs.GetInt("lvlTip" + SceneManager.GetActiveScene().name) == 0)
        {
            tipButton.SetActive(false);
            StartCoroutine(SetTip());
        } else if(tipButton != null) { tipButton.SetActive(true);}

        SoundEmitter = GameObject.FindGameObjectsWithTag("SoundEmitter");
        foreach (GameObject soundEmitter in SoundEmitter) soundEmitter.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundValue");

        if(otherSoundsCotroll != null) foreach (AudioSource sound in otherSoundsCotroll) sound.volume = PlayerPrefs.GetFloat("SoundValue");

        ColorText = FindObjectsOfType<Text>();
        ColorTextTMP = FindObjectsOfType<TextMeshPro>();
        OpenDoorSprite = Resources.Load<Sprite>("Sprites/OpenDoor");
        CloseDoorSprite = Resources.Load<Sprite>("Sprites/CloseDoor");
        Door = GameObject.Find("Door");
        if(Door != null) Door.GetComponent<SpriteRenderer>().color = GetBorderColor();
        OpenDoorSound = Resources.Load<AudioClip>("OpenDoorSound");
        CloseDoorSound = Resources.Load<AudioClip>("CloseDoorSound");
        PlayerRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>();

        if (PlayerPrefs.HasKey("HexBackgroundColor"))
            Camera.main.backgroundColor = HexToColor(PlayerPrefs.GetString("HexBackgroundColor"));

        gridMaps = FindObjectsOfType<Grid>();

        if (PlayerPrefs.HasKey("HexColor"))
            UpdateColorAll(HexToColor(PlayerPrefs.GetString("HexColor")));

        if (PlayerRenderer != null) PlayerRenderer.color = HexToColor(PlayerPrefs.GetString("HexColor"));
    }

    void UpdateColorAll(Color color) => UpdateColorBorders(color);

    public void OpenTip()
    {
        Time.timeScale = 0f;
        tipManager.SetActive(true);
    }

    void UpdateColorBorders(Color color)
    {
        void UpdateColor(Graphic[] graphics)
        {
            foreach (var graphic in graphics)
            {
                if (graphic != null && !graphic.CompareTag("Ignore"))
                {
                    if (graphic is TextMeshPro)
                    {
                        TextMeshPro textMeshProUGUI = (TextMeshPro)graphic;
                        textMeshProUGUI.faceColor = color;
                        textMeshProUGUI.color = color;
                    }
                    else
                    {
                        graphic.material.color = Color.white;
                        graphic.color = color;
                        if (graphic is Text) tipManager.SetActive(false);
                    }
                }
            }
        }

        UpdateColor(ColorText);
        UpdateColor(ColorTextTMP);

        foreach (var renderer in OtherBorders)
        {
            if (renderer != null)
            {
                renderer.material.color = Color.white;
                renderer.color = color;
            }
        }

        UpdateColor(ImageColor);

        foreach (var grid in gridMaps)
        {
            foreach (var tilemap in grid.GetComponentsInChildren<Tilemap>())
                tilemap.color = color;
        }
    }

    private void Update() { if (Input.GetKeyDown(KeyCode.R) && allowReload) ReloadLevel(); }

    IEnumerator SetTip()
    {
        yield return new WaitForSeconds(tipSeconds);
        tipButton.SetActive(true);
        PlayerPrefs.SetInt("lvlTip" + SceneManager.GetActiveScene().name, 1);
    }

    public void ReloadLevel()
    {
        PlayerPrefs.SetInt("ReloadsCount", PlayerPrefs.GetInt("ReloadsCount") + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private Color HexToColor(string hex)
    {
        Color color = Color.black;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public Color GetBorderColor()
    {
        Color colorItNeeds = HexToColor(PlayerPrefs.GetString("HexColor"));
        return colorItNeeds;
    }

    public Color GetBgColor()
    {
        Color colorItNeeds = HexToColor(PlayerPrefs.GetString("HexBackgroundColor"));
        return colorItNeeds;
    }

    public void OpenDoor() => HandleDoor(true, OpenDoorSprite, OpenDoorSound);
    public void CloseDoor() => HandleDoor(false, CloseDoorSprite, CloseDoorSound);

    private void HandleDoor(bool open, Sprite sprite, AudioClip sound)
    {
        var doorSprite = Door.GetComponent<SpriteRenderer>();
        var doorCollider = Door.GetComponent<BoxCollider2D>();
        var audioSource = Door.GetComponent<AudioSource>();

        doorSprite.sprite = sprite;
        doorCollider.enabled = !open;
        audioSource.clip = sound;
        audioSource.Play();
    }
}