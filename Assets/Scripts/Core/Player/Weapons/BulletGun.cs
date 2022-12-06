﻿using System;
using UnityEngine;
using Core.Cats;
using Core.Audio;
using Zenject;

namespace Core.Weapons
{
    public class BulletGun : IWeapon
    {
        private readonly BulletGunModel _model;
        private Bullet.Factory _bulletFactory;
        private Bullet.ExplosionFactory _explosionFactory;

        public float ReloadTime { get => _model.ReloadTime; set => _model.ReloadTime = value; }
        public event Action Missed;
        public event Action<CatView> Hit;

        [Inject]
        private void Construct(Bullet.Factory bulletFactory, Bullet.ExplosionFactory explosionFactory)
        {
            _bulletFactory = bulletFactory;
            _explosionFactory = explosionFactory;
        }

        public BulletGun(BulletGunModel model)
        {
            _model = model;
        }

        public bool TryShoot()
        {
            if (!_model.Cooldown.IsOver) return false;

            Bullet bullet = _bulletFactory.Create
            (
                _model.FirePoint.position,
                _model.FirePoint.rotation,
                _model.BulletGunConfig.BulletForce,
                _model.BulletGunConfig.BulletLifetime
            );

            var clip = _model.BulletGunConfig.ShootSounds.Random();
            SoundManager.PlayOneShot(clip.Clip, clip.Volume);
            
            bullet.LifetimeElapsed += bullet.Dispose;
            bullet.Hit += OnBulletHit;
            bullet.Disposed += OnDisposed;

            _model.Cooldown.Run(_model.ReloadTime);
            return true;
        }

        private void CreateExplosion(Vector2 position)
        {
            int rand = UnityEngine.Random.Range(0, _model.BulletGunConfig.Explosions.Length);
            GameObject prefab = _model.BulletGunConfig.Explosions[rand];
            GameObject explosion = _explosionFactory.Create(prefab, position);
            GameObject.Destroy(explosion, 0.8f);
        }

        private void OnDisposed(Bullet bullet)
        {
            CreateExplosion(bullet.ContactPoint);
            bullet.LifetimeElapsed -= bullet.Dispose;
            bullet.Hit -= OnBulletHit;
            bullet.Disposed -= OnDisposed;
            if (!bullet.HitAnyTarget) Missed?.Invoke();
        }
        private void OnBulletHit(Bullet bullet, CatView target)
        {
            bullet.Dispose();
            Hit?.Invoke(target);
        }
    }
}
