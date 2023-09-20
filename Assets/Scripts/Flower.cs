using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Flower : MonoBehaviour
{
    public string sceneName;
    private Player _player;
    void Start()
    {
        _player = FindObjectOfType<Player>() ?? throw new ArgumentNullException(nameof(Player));
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject == _player.gameObject)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
