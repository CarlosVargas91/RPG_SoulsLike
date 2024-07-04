using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScript fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject congratsText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skilTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWIndow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        switchTo(skilTreeUI); //Need this to assign events on skilltreeslot on time
        fadeScreen.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        //switchTo(null); //No menu at beggining
        switchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            switchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.B))
            switchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.K))
            switchWithKeyTo(skilTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            switchWithKeyTo(optionsUI);
    }

    public void switchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScript>() != null;

            if (!fadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            AudioManager.instance.playSFX(7, null);
            _menu.SetActive(true);
        }

        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.pauseGame(false);
            else
                GameManager.instance.pauseGame(true);
        }
    }

    public void switchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            checkForInGameUI();
            return;
        }

        switchTo(_menu);
    }

    private void checkForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScript>() == null)
                return;
        }

        switchTo(inGameUI);
    }

    public void switchOnEndScreen()
    {
        fadeScreen.fadeOut();
        StartCoroutine(endScreenCoRoutine());

    }

    public void SwitchOnFinalScreen()
    {
        fadeScreen.fadeOut();
        congratsText.SetActive(true);
        restartButton.SetActive(true);
    }

    IEnumerator endScreenCoRoutine()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void restartGameButton() => GameManager.instance.restartScene();

    public void exitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }

    public void loadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parameter == pair.Key)
                    item.loadSlider(pair.Value);
            }
        }
    }

    public void saveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parameter, item.slider.value);
        }
    }
}
