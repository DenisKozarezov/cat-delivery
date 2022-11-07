using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Zenject;

namespace Core.Models
{
    [Serializable]
    public class GameSettings
    {
        public ushort GameTime;
        public ushort SavedReward;
        public ushort KidnapPenalty;
    }

    [Serializable]
    public class CatsSettings
    {
        public SpriteLibraryAsset[] Skins;
        [Range(0f, 10f)]
        public float CatsFallingSpeed;
        [Range(0f, 10f)]
        public float CatsKidnapSpeed;
        [Range(0f, 10f)]
        public float CatsSaveSpeed;
        [Editor.MinMaxSlider(0f, 10f, width: 45f)]
        public Vector2 SpawnInterval;
    }

    [Serializable]
    public class PlayerSettings
    {
        [Min(0f)]
        public float ReloadTime;
        [Range(0f, 10f)]
        public float MovementSpeed;
        [Range(0f, 50f)]
        public float JumpForce;
        [Range(0f, 50f)]
        public float NormalGravityScale;
        [Range(0f, 50f)]
        public float FallingGravityScale;
    }

    [Serializable]
    public class EnemySettings
    {
        [Range(0f, 10f)]
        public float MovementSpeed;
        [Editor.MinMaxSlider(0f, 10f, width: 45f)]
        public Vector2 AttackCooldownInterval;
    }

    [Serializable]
    public class WeaponsSettings
    {
        public BulletGunConfig BulletGunConfig;
        public LaserGunConfig LaserGunConfig;
    }

    [CreateAssetMenu(fileName = "Game Settings", menuName = "Installers/Game Settings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        [SerializeField]
        private GameSettings _gameSettings;
        [SerializeField]
        private PlayerSettings _playerSettings;
        [SerializeField]
        private CatsSettings _catsSettings;
        [SerializeField]
        private EnemySettings _enemySettings;
        [SerializeField]
        private WeaponsSettings _weaponsSettings;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.Bind<ILogger>().To<StandaloneLogger>().AsCached();

            Container.Bind<GameSettings>().FromInstance(_gameSettings).IfNotBound();
            Container.Bind<PlayerSettings>().FromInstance(_playerSettings).IfNotBound();
            Container.Bind<CatsSettings>().FromInstance(_catsSettings).IfNotBound();
            Container.Bind<EnemySettings>().FromInstance(_enemySettings).IfNotBound();
            Container.Bind<WeaponsSettings>().FromInstance(_weaponsSettings).IfNotBound();
        }
    }
}
