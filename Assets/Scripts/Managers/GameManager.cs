using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    private Transform player;

    [SerializeField] private Checkpoint[] checkpoints;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPreFab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        checkpoints = FindObjectsOfType<Checkpoint>();
        player = PlayerManager.instance.player.transform;
    }

    public void restartScene()
    {
        SaveManager.instance.saveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void loadData(GameData _data)
    {
        loadCheckpoints(_data);
        loadLostCurrency(_data);
    }

    private void loadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.Id == pair.Key && pair.Value == true)
                    checkpoint.activateCheckpoint();
            }
        }

        foreach (Checkpoint checkpoint in checkpoints) //if fails check Checkpoint section mid video
        {
            if (_data.closestCheckpointId == checkpoint.Id)
                player.position = checkpoint.transform.position;
        }
    }

    private void loadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPreFab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    public void saveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if (findClosestCheckpoint() != null)
            _data.closestCheckpointId = findClosestCheckpoint().Id;
        
        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.Id, checkpoint.activationStatus);
        }
    }

    private Checkpoint findClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distance = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distance < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    public void pauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
