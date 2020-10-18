﻿using UnityEngine;

public class Bullet : BulletClass
{

    public ParticleSystem Effect;

    public void Start()
    {
        HitEffect = Effect;
        damage = 1;
        StartCoroutine(SetLifetime(1));
    }

    public override void collide()
    {
        Instantiate(HitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
