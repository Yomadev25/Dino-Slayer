using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour, IOnEventCallback
{
    public string id;
    public Color color;
    public bool isReady;
    public int score;

    [Header("Movement Setting")]
    [SerializeField]
    private float _speed;

    [Header("Shooting Setting")]
    [SerializeField]
    private Transform _gunHandler;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float _fireRate;
    private float _lastShooted;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Rigidbody2D _rigidBody;
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private AudioSource _hitSfx;
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private GameObject _hitFx;

    float horizontal;
    float vertical;
    float gunHorizontal;
    float gunVertical;

    bool knockBack;

    public static event Action onScoreUpdate;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        _spriteRenderer.color = color;
        _nameText.text = id;
    }

    private void Update()
    {
        if (horizontal > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (horizontal < 0)
        {
            _spriteRenderer.flipX = true;
        }

        GunHandler();
        AnimationHandler();
    }

    private void FixedUpdate()
    {
        if (knockBack) return;
        _rigidBody.velocity = new Vector2(horizontal * _speed, vertical * _speed);
    }

    private void GunHandler()
    {
        Vector2 moveVector = (Vector2.up * gunVertical - Vector2.left * gunHorizontal);
        if (gunHorizontal != 0 || gunVertical != 0)
        {
            _gunHandler.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);

            if (Time.time > _lastShooted)
            {
                Bullet bullet = Instantiate(_bulletPrefab, _gunHandler.position, _gunHandler.rotation).GetComponent<Bullet>();
                bullet.owner = id;
                _lastShooted = Time.time + _fireRate;
            }
        }
    }

    private void AnimationHandler()
    {
        _anim.SetBool("isWalk", horizontal != 0 || vertical != 0);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == PlayerController.MOVE_CHARACTER)
        {
            object[] data = (object[])photonEvent.CustomData;
            if ((string)data[0] == id)
            {
                horizontal = (float)data[1];
                vertical = (float)data[2];
            }
        }
        else if (eventCode == PlayerController.ATTACK)
        {
            object[] data = (object[])photonEvent.CustomData;
            if ((string)data[0] == id)
            {
                gunHorizontal = (float)data[1];
                gunVertical = (float)data[2];
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (collision.TryGetComponent(out Bullet bullet))
            {
                if (bullet.owner == id) return;

                Vector2 direction = (transform.position - bullet.transform.position).normalized;
                StopAllCoroutines();
                StartCoroutine(Knockback(direction));

                Destroy(collision.gameObject);
            }
        }
    }

    IEnumerator Knockback(Vector2 direction)
    {
        knockBack = true;
        _hitSfx.Play();
        Destroy(Instantiate(_hitFx, transform.position, Quaternion.identity), 1f);
        _rigidBody.AddForce(direction * 5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        knockBack = false;
    }

    public void AddScore(int score)
    {
        this.score += score;
        onScoreUpdate?.Invoke();
    }
}
