using UnityEngine;
using Zenject;
using Core.Weapons;
using Core.Models;
using Core.Audio;
using Core.Cats;
using Core.Infrastructure.Signals.Game;

namespace Core
{
    public class LaserSpawner : IInitializable, ITickable, ILateDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly Laser.Factory _factory;
        private readonly LaserGunConfig _laserConfig;
        private readonly Vector2 _attackCooldownInterval;
        private readonly float _spawnHeight;
        private readonly LevelBounds _levelBounds;
        private readonly CameraView _camera;

        private Vector2 _position;
        private float _timer;
        private float _spawnTime;
        private bool _enabled = true;

        public LaserSpawner(
            SignalBus signalBus, 
            Laser.Factory factory, 
            EnemySettings enemySettings, 
            WeaponsSettings weaponSettings, 
            LevelBounds levelBounds,
            CameraView camera)
        {
            _signalBus = signalBus;
            _factory = factory;
            _attackCooldownInterval = enemySettings.AttackCooldownInterval;
            _spawnHeight = enemySettings.LaserSpawnHeight;
            _laserConfig = weaponSettings.LaserGunConfig;
            _levelBounds = levelBounds;
            _camera = camera;
        }

        private Vector2 GetRandomPosition(bool isLeft)
        {
            int sign = isLeft ? -1 : 1;
            float y = Random.Range(-_spawnHeight, _spawnHeight);
            return new Vector2(sign * _levelBounds.Size.x / 2f, y);
        }
        private Quaternion GetRandomRotation(bool isLeft)
        {
            float startAngle = isLeft ? 0f : 180f;
            float randomAngle = Random.Range(-35f, 35f);
            return Quaternion.AngleAxis(startAngle - randomAngle, Vector3.forward);
        }
        private void SpawnLaser(bool isLeft)
        {
            _position = GetRandomPosition(isLeft);

            Laser laser = _factory.Create
            (
                _position,
                GetRandomRotation(isLeft),
                _laserConfig.PreparationTime,
                _laserConfig.LaserLifetime
            );

            var clip = _laserConfig.LaserSounds.Random();
            SoundManager.PlayOneShot(clip.Clip, clip.Volume);

            laser.LifetimeElapsed += OnLifetimeElapsed;
            laser.Hit += OnHit;
            laser.Prepared += OnLaserPrepared;
        }

        private void OnLifetimeElapsed(Laser laser)
        {
            laser.LifetimeElapsed -= OnLifetimeElapsed;
            laser.Hit -= OnHit;
            laser.Prepared -= OnLaserPrepared;
            laser.Dispose();
        }
        private void OnHit(CatView target)
        {
            Vector2 direction = _position - (Vector2)target.transform.position;
            target.Kidnap(direction.normalized);       
        }
        private void OnLaserPrepared()
        {
            if (_laserConfig.CameraShake)
            {
                _camera.Shake(_laserConfig.ShakeDuration, _laserConfig.ShakeFrequence);
            }
        }
        private void OnGameOverSignal()
        {
            _enabled = false;
        }

        void IInitializable.Initialize()
        {
            _spawnTime = Random.Range(_attackCooldownInterval.x, _attackCooldownInterval.y);
            _signalBus.Subscribe<GameOverSignal>(OnGameOverSignal);
        }
        void ILateDisposable.LateDispose()
        {
            _signalBus.TryUnsubscribe<GameOverSignal>(OnGameOverSignal);
        }
        void ITickable.Tick()
        {
            if (_enabled && Time.realtimeSinceStartup - _timer >= _spawnTime)
            {
                bool isLeft = Random.Range(0f, 1f) > 0.5f ? true : false;

                SpawnLaser(isLeft);
                _timer = Time.realtimeSinceStartup;
                _spawnTime = Random.Range(_attackCooldownInterval.x, _attackCooldownInterval.y);
            }
        }
    }
}