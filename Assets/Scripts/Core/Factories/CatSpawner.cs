using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Core.Cats;
using Core.Models;
using Core.Infrastructure.Signals.Game;
using Core.Infrastructure.Signals.Cats;

namespace Core
{
    public class CatSpawner : IInitializable, ITickable, ILateDisposable
    {
        private SignalBus _signalBus;
        private CatView.Factory _catFactory;
        private CatsSettings _settings;
        private float _timer;
        private float _spawnTime;
        private bool _enabled = true;

        private LinkedList<CatView> _cats = new LinkedList<CatView>();

        [Inject]
        private void Construct(SignalBus signalBus, CatView.Factory catFactory, CatsSettings settings)
        {
            _signalBus = signalBus;
            _catFactory = catFactory;
            _settings = settings;
        }

        void IInitializable.Initialize()
        {
            _spawnTime = Random.Range(_settings.SpawnInterval.x, _settings.SpawnInterval.y);
            _signalBus.Subscribe<GameOverSignal>(OnGameOverSignal);
        }
        void ITickable.Tick()
        {
            if (_enabled && Time.realtimeSinceStartup - _timer >= _spawnTime)
            {
                SpawnCat();
                _timer = Time.realtimeSinceStartup;
                _spawnTime = Random.Range(_settings.SpawnInterval.x, _settings.SpawnInterval.y);
            }
        }
        void ILateDisposable.LateDispose()
        {
            _signalBus.TryUnsubscribe<GameOverSignal>(OnGameOverSignal);
        }
        private void SpawnCat()
        {
            float width = _settings.CatsSpawnWidth / 2f;
            Vector2 spawnPosition = new Vector2(Random.Range(-width, width), 5.85f);
            CatView cat = _catFactory.Create();
            cat.transform.position = spawnPosition;

            cat.Disposed += OnCatDisposed;
            cat.Saved += OnCatSaved;
            cat.Kidnapped += OnCatKidnapped;

            cat.SetInteractable(true);
            cat.SetDirection(Vector2.down, _settings.CatsFallingSpeed);

            int rand = Random.Range(0, _settings.Skins.Length);
            cat.SetSkin(_settings.Skins[rand]);

            _cats.AddLast(cat);
        }

        private void OnCatKidnapped(CatView cat, Vector2 direction)
        {
            cat.SetInteractable(false);
            cat.SetDirection(direction, _settings.CatsKidnapSpeed);
            cat.Dispose();
            _signalBus.Fire(new CatKidnappedSignal { KidnappedCat = cat });
        }
        private void OnCatSaved(CatView cat)
        {
            cat.SetInteractable(false);
            cat.SetDirection(Vector2.down, _settings.CatsSaveSpeed);
            _signalBus.Fire(new CatSavedSignal { SavedCat = cat });
        }
        private void OnCatDisposed(CatView cat)
        {
            cat.Disposed -= OnCatDisposed;
            cat.Saved -= OnCatSaved;
            cat.Kidnapped -= OnCatKidnapped;
            _cats.Remove(cat);
        }
        private void OnGameOverSignal()
        {
            _enabled = false;
            while (_cats.Count > 0)
            {
                _cats.First.Value.Dispose();
            }
        } 
    }
}