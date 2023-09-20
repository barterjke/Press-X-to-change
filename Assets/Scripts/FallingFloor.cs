using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFloor : MonoBehaviour
{
    public float delay;
    
    private BoxCollider2D _collider;
    private Player _player;
    private Animator _animator;
    void Start()
    {
        _animator = GetComponentInChildren<Animator>() ?? throw new ArgumentNullException(nameof(Animator));
        _player = FindObjectOfType<Player>() ?? throw new ArgumentNullException(nameof(Player));
        _collider = GetComponent<BoxCollider2D>() ?? throw new ArgumentNullException(nameof(BoxCollider2D));
        _player.OnGrounded += MaybeDestroy;
    }
    
    IEnumerator DestroyItself()
    {
        yield return new WaitForSeconds(delay);
        _animator.SetTrigger("fall");
        _collider.enabled = false;
    }

    private void MaybeDestroy(Collision2D collider)
    {
        if (collider.gameObject == gameObject)
        {
            _animator.SetTrigger("shake");
            StartCoroutine(DestroyItself());
        }
    }

    private void OnDisable()
    {
        _player.OnGrounded -= MaybeDestroy;
    }
}
