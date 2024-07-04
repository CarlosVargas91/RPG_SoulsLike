using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName; // name of file
    [SerializeField] private bool encryptData;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHanlder dataHandler;

    [ContextMenu("Delete saved file")]
    public void deleteSavedData()
    {
        dataHandler = new FileDataHanlder(Application.persistentDataPath, fileName, encryptData);
        dataHandler.deleteData();
    }
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }


    private void Start()
    {
        dataHandler = new FileDataHanlder(Application.persistentDataPath, fileName, encryptData);
        saveManagers = findAllSaveManagers();

        loadGame();
    }
    public void newGame()
    {
        gameData = new GameData();
    }

    public void loadGame()
    {
        gameData = dataHandler.load();

        if (this.gameData == null)
        {
            Debug.Log("No save data found");
            newGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.loadData(gameData);
        }
    }

    public void saveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.saveData(ref gameData);
            Debug.Log("Saving" + saveManager);
        }

        dataHandler.save(gameData);
    }

    private void OnApplicationQuit()
    {
        saveGame();
    }

    //Search monoBehavior objects and filter for the ones implemented on ISaveManager
    private List<ISaveManager> findAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool hasSaveData()
    {
        if (dataHandler.load() != null)
            return true;

        return false;
    }
}
