﻿using UnityEngine;

namespace Core.Models
{
    [CreateAssetMenu(menuName = "Configuration/Weapons/Bullet Gun")]
    public class BulletGunConfig : WeaponConfig
    {
        [field: SerializeField, Min(0f)] public float BulletForce { get; private set; }
        [field: SerializeField, Min(0f)] public float BulletLifetime { get; private set; }
    }
}
