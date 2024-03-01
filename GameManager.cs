using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private Grid[] gridMaps; // Mapa
    public SpriteRenderer[] OtherBorders; // Imatges del mapa o altes parts
    private SpriteRenderer PlayerRenderer; // Renderer del player 
    private GameObject Door; // la porta
    private Text[] ColorText; // TOT el text del nivell
    private TextMeshPro[] ColorTextTMP;  // TOT el text del nivell
    public Image[] ImageColor; // altres imatges no existents al mapa
    private GameObject[] SoundEmitter; // objectes que treuen so
    public GameObject tipManager; // Manager dels tips
    private GameObject tipButton; // Butto per obrir tip
    public float tipSeconds = 3f; // espera de segons avans de mostrar el tip
    public bool allowReload = true; // Es permet reiniciar el nivell (s'utilita poc)
    private Text levelText; // el text que mostra a quin nivell estem
    private static GameManager _instance; // GameManager en si
    public AudioSource[] otherSoundsCotroll; // altres objectes que treuen so no existents al mapa

    private Sprite OpenDoorSprite, CloseDoorSprite; // imatges de la porta oberta i tancada
    private AudioClip OpenDoorSound, CloseDoorSound; // sons d'obrir i tancar la porta

    public static GameManager Instance { get { if (_instance == null) { _instance = FindObjectOfType<GameManager>(); } return _instance; } } // Possibilitat d'acces al GameManager desde altres scripts, com el Jugador

    void Awake() // El primer frame fes aixo una vegada:
    {
    //Posar a quin nivell estem mirant el nom de la escena
        levelText = GameObject.Find("LevelInt").GetComponent<Text>();
           if (levelText != null)
               levelText.text = LocalizationManager.Instance.GetTextForKey("level") + " " + SceneManager.GetActiveScene().name;

    //inicialitzacio del TipManager
        tipManager.SetActive(true);
        tipButton = GameObject.Find("TipButton");

    //inicialitzacio del propi tip
        if (PlayerPrefs.GetInt("lvlTip" + SceneManager.GetActiveScene().name) == 0)
        {
            tipButton.SetActive(false);
            StartCoroutine(SetTip());
        } else if(tipButton != null) { tipButton.SetActive(true);}


        SoundEmitter = GameObject.FindGameObjectsWithTag("SoundEmitter"); // troba tots els objectes que emetin so i...
        foreach (GameObject soundEmitter in SoundEmitter) soundEmitter.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundValue"); // posals'hi el volumen guardat a la memoria

        if(otherSoundsCotroll != null) foreach (AudioSource sound in otherSoundsCotroll) sound.volume = PlayerPrefs.GetFloat("SoundValue"); // el mateix

        ColorText = FindObjectsOfType<Text>(); // trobar textos
        ColorTextTMP = FindObjectsOfType<TextMeshPro>(); // Trobar textos
        OpenDoorSprite = Resources.Load<Sprite>("Sprites/OpenDoor"); // cargar imatges de la porta desde arxius de joc
        CloseDoorSprite = Resources.Load<Sprite>("Sprites/CloseDoor");
        Door = GameObject.Find("Door"); // trobar l'objecte de la porta
        OpenDoorSound = Resources.Load<AudioClip>("OpenDoorSound");  // cargar sons de la porta desde arxius de joc
        CloseDoorSound = Resources.Load<AudioClip>("CloseDoorSound");
        PlayerRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>(); // trobar renderer del jugador

        if (PlayerPrefs.HasKey("HexBackgroundColor")) // aplicar color al fondo
            Camera.main.backgroundColor = HexToColor(PlayerPrefs.GetString("HexBackgroundColor"));

        gridMaps = FindObjectsOfType<Grid>(); // trobar mapa

        if (PlayerPrefs.HasKey("HexColor"))// trobar HEX CODE del color triat
            UpdateColorAll(HexToColor(PlayerPrefs.GetString("HexColor")));

        if (PlayerRenderer != null) PlayerRenderer.color = HexToColor(PlayerPrefs.GetString("HexColor")); // aplicar color al jugador
    }

    void UpdateColorAll(Color color) => UpdateColorBorders(color); // inicialitzacio de colors

    public void OpenTip()
    {
        Time.timeScale = 0f;
        tipManager.SetActive(true);
    }

    void UpdateColorBorders(Color color) // metode molt gran que simplement agafa TOTS els objectes i els hi posa el color
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

    private void Update() { if (Input.GetKeyDown(KeyCode.R) && allowReload) ReloadLevel(); } // si es prem el boto R, reinicia el nivell

    IEnumerator SetTip() // temps que es tarda perque el tip sigui disponible
    {
        yield return new WaitForSeconds(tipSeconds);
        tipButton.SetActive(true);
        PlayerPrefs.SetInt("lvlTip" + SceneManager.GetActiveScene().name, 1);
    }

    public void ReloadLevel() // reinici nivell
    {
        PlayerPrefs.SetInt("ReloadsCount", PlayerPrefs.GetInt("ReloadsCount") + 1); // sumar al comptador de reinicis una unitat
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // cargar la mateixa escena que es troba ara el USER
    }

    private Color HexToColor(string hex) // convertir HEX COLOR CODE
    {
        Color color = Color.black;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public Color GetBorderColor() // metode accesible desde altres scripts per aconseguir el color guardat a la memoria
    {
        Color colorItNeeds = HexToColor(PlayerPrefs.GetString("HexColor"));
        return colorItNeeds;
    }

    public Color GetBgColor() // metode accesible desde altres scripts per aconseguir el color del fons guardat a la memoria
    {
        Color colorItNeeds = HexToColor(PlayerPrefs.GetString("HexBackgroundColor"));
        return colorItNeeds;
    }

//metodes per obrir i tancar la porta
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
