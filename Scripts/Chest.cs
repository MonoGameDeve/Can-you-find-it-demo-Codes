using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    Animator myChestAnimation;
    BoxCollider2D frame;

    [SerializeField] int treasureToAdd = 10;
    [SerializeField] AudioClip treasureChestOpensfx;

    private void Start()
    {
        myChestAnimation = GetComponent<Animator>();
        frame = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        myChestAnimation.SetTrigger("OpenChest");
        FindObjectOfType<GameSession>().AddToTreasure(treasureToAdd);
        AudioSource.PlayClipAtPoint(treasureChestOpensfx, Camera.main.transform.position);
        Destroy(frame);
    }
}
