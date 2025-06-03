using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Ebac.Core.Singleton;

public class PlayerController : Singleton<PlayerController>
{
    //publics
    [Header("Lerp")]
    public Transform target;
    public float lerpSpeed = 1f;

    public float speed = 1f;

    public string tagToCheckEnemy = "Enemy";
    public string tagToCheckEndLine = "EndLine";

    public bool invencible = false;

    public GameObject endScreen;

    [Header("TextMeshPro")]
    public TextMeshPro uiTextPowerUp;

    [Header("CoinSetup")]
    public GameObject coinCollector;

    [Header("Aniamtion")]
    public AnimatorManager animatorManager;

    [SerializeField] private BounceHelper _bounceHelper;

    //privates
    private bool _canRun = false;
    private Vector3 _pos;
    private float _currentSpeed;
    private Vector3 _startPosition;
    private float _baseSpeedToAnimation = 7f;

    private void Start()
    {
        _startPosition = transform.position;
        ResetSpeed();
    }

    private void Update()
    {
        if (!_canRun) return;

        _pos = target.position;
        _pos.y = transform.position.y;
        _pos.z = transform.position.z;

        _pos.x = Mathf.Clamp(_pos.x, -4.5f, 4.5f);

        transform.position = Vector3.Lerp(transform.position, _pos, lerpSpeed * Time.deltaTime);
        transform.Translate(transform.forward * Time.deltaTime * _currentSpeed);
    }

    public void Bounce()
    {
        if (_bounceHelper != null)
        {
            _bounceHelper.Bounce();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == tagToCheckEnemy)
        {
            if (!invencible) EndGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == tagToCheckEndLine)
        {
            EndGame();
        }

        if (other.transform.tag == tagToCheckEnemy)
        {
            if (!invencible)
            {
                EndGame(AnimatorManager.AnimationType.DEAD);
                MoveBack(transform);
            }
        }
    }

    private void MoveBack(Transform t)
    {
        t.DOMoveZ(-1f, .3f).SetRelative();
    }

    private void EndGame(AnimatorManager.AnimationType animationType = AnimatorManager.AnimationType.IDLE)
    {
        _canRun = false;
        endScreen.SetActive(true);
        animatorManager.Play(animationType);
    }

    public void StartToRun()
    {
        _canRun = true;
        animatorManager.Play(AnimatorManager.AnimationType.RUN, _currentSpeed / _baseSpeedToAnimation);
    }

    #region POWER UPS

    public void SetPowerUpText(string s)
    {
        uiTextPowerUp.text = s;
    }

    public void PowerUpSpeedUp(float f)
    {
        _currentSpeed = f;
        _baseSpeedToAnimation = 3.5f;
    }

    public void ResetSpeed()
    {
        _currentSpeed = speed;
    }

    public void SetInvencible(bool b = true)
    {
        invencible = b;
    }

    public void ChangeHeight(float amount, float duration, float animationDuration, Ease ease)
    {
        transform.DOMoveY(_startPosition.y + amount, animationDuration).SetEase(ease);
        Invoke(nameof(ResetHeight), duration);
    }

    public void ResetHeight()
    {
        transform.DOMoveY(_startPosition.y, .1f);
    }

    public void ChangeCoinCollectorSize(float amount)
    {
        coinCollector.transform.localScale = Vector3.one * amount;
    }

    #endregion
}
