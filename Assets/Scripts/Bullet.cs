using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string owner;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb.AddForce(transform.up * _speed, ForceMode2D.Impulse);
        Destroy(gameObject, 1f);
    }
}
