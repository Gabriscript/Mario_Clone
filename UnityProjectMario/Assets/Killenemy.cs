using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killenemy : MonoBehaviour
{
    [SerializeField] private AudioSource enemyDie;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Enemy") { Destroy(collision.gameObject);
            enemyDie.Play();
            FindObjectOfType<PlayerMover>().StompJump();
        
        }
    }
}
