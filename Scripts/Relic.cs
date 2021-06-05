using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Relic : MonoBehaviour
{
    [SerializeField] int relicToAdd = 1;
    [SerializeField] AudioClip winsfx;

    void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<GameSession>().AddToRelic(relicToAdd);
        Destroy(gameObject);
        FindObjectOfType<GameSession>().ProcessPlayerWin();
        AudioSource.PlayClipAtPoint(winsfx, Camera.main.transform.position);
    }
}
