﻿using UnityEngine;

public class MiniBullet : BulletClass
{

    public ParticleSystem Effect;

    public void Start()
    {
        HitEffect = Effect;
        damage = 5;
        StartCoroutine(SetLifetime(Random.Range(0.2f, 0.5f)));
    }

    public override void collide()
    {
        Instantiate(HitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
