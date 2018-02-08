﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Sub : MonoBehaviour
{
    public PlayerCam Cam;

    /// <summary>
    /// 0-indexed
    /// </summary>
    public int Player;

    public string Input
    {
        get { return "P" + (Player + 1); }
    }

    public Transform Transform
    {
        get { return transform; }
    }

    public bool IsDestroyed { get; private set; }

    public FormattableText BlownUpText;

    public GameObject SpawnPoint;

    public GameObject ExplosionPrefab;

    public int Lives;

    public float RespawnDelay = 3;

    public TorpedoTargetable Targetable { get; set; }

    public Taggable Taggable { get; private set; }

    public SubBody Body { get; private set; }

    public Image PingReadyIcon;

    public GamePadState InputState { get; private set; }

    public int Kills { get; private set; }

    public RectTransform KillCounter;

    public GameObject KillPrefab;

    public float AimDistance = 30;

    public RectTransform AimReticule;

    public Camera SubCamera { get; private set; }

    public void Awake()
    {
        Taggable = GetComponent<Taggable>();
        Targetable = GetComponent<TorpedoTargetable>();

        Targetable.OnHitByTorpedo += BlowUp;
        Targetable.OnHitByMine += BlowUp;
        Body = GetComponentInChildren<SubBody>();

        SubCamera = Cam.GetComponent<Camera>();
    }

    private void OnGameStarted()
    {
        Kills = 0;
        foreach (Transform child in KillCounter)
        {
            Destroy(child.gameObject);
        }
    }

    public void GotKill()
    {
        Kills++;

        Instantiate(KillPrefab, KillCounter);
    }

    private void BlowUp(Mine mine)
    {
        BlowUp(mine.Parent.Player);
    }

    private void BlowUp(Torpedo torpedo)
    {
        BlowUp(torpedo.Parent.Player);
    }

    private void BlowUp(int playerCaused)
    {
        if (IsDestroyed)
            return;

        Instantiate(ExplosionPrefab, transform.position, transform.rotation);

        GetComponentInChildren<SubBody>(true).gameObject.SetActive(false);
        IsDestroyed = true;

        BlownUpText.gameObject.SetActive(true);
        BlownUpText.Format(playerCaused + 1);

        if (playerCaused != Player)
            SubManager.Instance.GetSub(playerCaused).GotKill();

// explosion of ship is done in the mine or torpedo
//        SoundManager.PlaySound("explosion_far1");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnDelay);
        IsDestroyed = false;
        transform.position = SpawnPoint.transform.position;
        transform.rotation = SpawnPoint.transform.rotation;
        GetComponentInChildren<SubBody>(true).gameObject.SetActive(true);
        BlownUpText.gameObject.SetActive(false);
    }

    public void Start()
    {
        GameplayManager.Instance.GameStarted += OnGameStarted;
        SubManager.Instance.Subs.Add(this);
        BlownUpText.gameObject.SetActive(false);

        transform.position = SpawnPoint.transform.position;
        transform.rotation = SpawnPoint.transform.rotation;
    }

    public void Update()
    {
        InputState = GamePad.GetState((PlayerIndex)Player);

        AimReticule.anchoredPosition = Helpers.Instance.CameraSpaceToMultiplyerSpace(
            Helpers.Instance.WorldPointToScreenSpace(transform.position + (transform.forward * AimDistance), SubCamera));
    }

    public void OnDestroy()
    {
        SubManager.Instance.Subs.Remove(this);
    }

    public void OnCollisionEnter(Collision collision)
    {
        var otherSub = collision.gameObject.GetComponentInParent<Sub>();
        if (otherSub != null)
        {
            BlowUp(otherSub.Player);
        }

        if (collision.gameObject.CompareTag("Ceiling"))
        {
            GetComponent<SubController>().HitCeiling();
        }
    }
}