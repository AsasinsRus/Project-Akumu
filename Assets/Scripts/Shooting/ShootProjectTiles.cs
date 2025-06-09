using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShootProjectTiles : MonoBehaviour
{
    [SerializeField] public Transform bullet;
    [SerializeField] private Transform raycast;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float destroyBulletDelay;

    [ContextMenu(nameof(Shoot))]

    public void Shoot()
    {
        Transform createdBullet = Instantiate(bullet, raycast.position, raycast.rotation);

        muzzleFlash.GetComponentInChildren<VisualEffect>().Play();
        PlayPewSound();

        createdBullet.GetComponent<Bullet>().Setup(raycast.forward);

        Destroy(createdBullet.gameObject, destroyBulletDelay);
    }

    private void PlayPewSound()
    {
        var AISoundsManager = GetComponentInChildren<AISoundsManager>();

        AISoundsManager.audioSource.clip = AISoundsManager.pew;
        AISoundsManager.audioSource.Play();
    }
}
