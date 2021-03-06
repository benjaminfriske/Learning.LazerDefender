﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header(header:"Player")]
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float padding = 1f;
    [SerializeField] float health = 200;

    [Header(header: "Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 1f;
    [SerializeField] float fireRate = .1f;
    [SerializeField] [Range(0, 1)]  float laserVolume = 0.5f;
    [SerializeField] AudioClip laserSfx;

    [Header(header: "Explosion")]
    [SerializeField] AudioClip explosionSfx;
    [SerializeField] [Range(0,1)] float explosionVolume = 0.5f;

    private Coroutine fireContinuousy;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    void Start()
    {
        SetUpMoveBoundaries();
    }

    

    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            fireContinuousy = StartCoroutine(FireContinuously());
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(fireContinuousy);
        }
    }

    IEnumerator FireContinuously()
    {
        while(true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(laserSfx, Camera.main.transform.position, laserVolume);
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if(damageDealer != null)
        {
            ProcessHit(damageDealer);
        }
        
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamageAmount();
        damageDealer.Hit();

        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(explosionSfx, Camera.main.transform.position, explosionVolume);
            Destroy(gameObject);
        }
    }
}
