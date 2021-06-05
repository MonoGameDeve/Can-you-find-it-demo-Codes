using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int treasure = 0;
    [SerializeField] int relic = 0;
    [SerializeField] float ShowDeathDelay = 2f;
    [SerializeField] float delayWin = 1f;
    [SerializeField] Text treasureText;
    [SerializeField] Text relicText;

    private void Awake()
    {
        var numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions < 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        treasureText.text = treasure.ToString();
        relicText.text = relic.ToString();
    }

    public void AddToRelic(int RelicToAdd)
    {
        relic += RelicToAdd;
        relicText.text = relic.ToString();
    }

    public void AddToTreasure(int TreasureToAdd)
    {
        treasure += TreasureToAdd;
        treasureText.text = treasure.ToString();
    }

    public void ProcessPlayerDeath()
    {
        StartCoroutine(ShowDeath());
        //SceneManager.LoadScene("You Died");
    }

    IEnumerator ShowDeath()
    {
        yield return new WaitForSecondsRealtime(ShowDeathDelay);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("You Died");
    }

    public void ProcessPlayerWin()
    {
        StartCoroutine(DelayWin());
    }

    IEnumerator DelayWin()
    {
        yield return new WaitForSecondsRealtime(delayWin);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("You Win");
    }
}
