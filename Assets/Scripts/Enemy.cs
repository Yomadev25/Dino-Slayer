using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _hp;
    [SerializeField]
    private int _score;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private AudioSource _hitSfx;

    Vector2 destination;

    private void Start()
    {
        RandomDestination();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, destination) > 1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, _speed * Time.deltaTime);
        }
        else
        {
            RandomDestination();            
        }
    }

    private void RandomDestination()
    {
        destination.x = Random.Range(-9f, 9f);
        destination.y = Random.Range(-5f, 5f);

        Vector2 dir = (destination - (Vector2)transform.position).normalized;

        if (dir.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (dir.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
    }

    public void TakeDamage(Character dealer)
    {
        _hp--;
        StartCoroutine(TakeDamageCoroutine());

        if (_hp <= 0)
        {
            dealer.AddScore(_score);
            Destroy(gameObject);
        }
    }

    IEnumerator TakeDamageCoroutine()
    {
        _hitSfx.Play();
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (collision.TryGetComponent(out Bullet bullet))
            {
                Character[] characters = FindObjectsOfType<Character>();
                Character character = characters.First(x => x.id == bullet.owner);
                TakeDamage(character);
                Destroy(collision.gameObject);
            }
        }
    }
}
