using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Player _player;
    public Animator anim;

    private void OnValidate()
    {
        _player = FindObjectOfType<Player>();
        print(_player);
    }

    void Start()
    {
        _player = FindObjectOfType<Player>() ?? throw new ArgumentNullException(nameof(Player));
        _player.OnGrounded += MaybeKill;
    }
    
    private void MaybeKill(Collision2D collider)
    {
        if (collider.gameObject == gameObject)
        {
            _player.Die();
        }
    }

    private void OnDisable()
    {
        _player.OnGrounded -= MaybeKill;
    }
}