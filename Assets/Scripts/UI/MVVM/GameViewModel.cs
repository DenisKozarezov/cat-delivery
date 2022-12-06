using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityWeld.Binding;
using Zenject;
using Core.Models;
using Core.Infrastructure.Signals.Cats;
using Core.Infrastructure.Signals.Game;
using Core.Audio;

namespace Core.UI
{
    [Binding]
    public class GameViewModel : MonoBehaviour, INotifyPropertyChanged, IPauseHandler
    {
        [field: SerializeField] private HealthView HealthVM { get; set; }
        [field: SerializeField] private GameOverViewModel GameOverVM { get; set; }

        private SignalBus _signalBus;
        private GameSettings _gameSettings;
        private CatsSettings _catsSettings;
        private GameSounds _gameSounds;
        private Countdown _countdown;
        private GameState _gameState = new GameState();
        private bool _enabled;

        [Binding]
        public int Score
        {
            get => _gameState.Score;
            set
            {
                if (_gameState.Score == value) return;

                _gameState.Score = value;
                OnPropertyChanged("Score");
            }
        }

        [Binding]
        public int Streak
        {
            get => _gameState.Streak;
            set
            {
                if (_gameState.Streak == value) return;

                _gameState.Streak = value;
                OnPropertyChanged("Streak");
            }
        }

        [Binding]
        public float CurrentTime
        {
            get => _gameState.CurrentTime;
            set
            {
                if (_gameState.CurrentTime == value) return;

                _gameState.CurrentTime = value;
                OnPropertyChanged("CurrentTime");
            }
        }

        [Binding]
        public int HighScore
        {
            get => _gameState.HighScore;
            set
            {
                if (_gameState.HighScore == value) return;

                _gameState.HighScore = value;
                OnPropertyChanged("HighScore");
            }
        }

        [Binding]
        public float BestTime
        {
            get => _gameState.BestTime;
            set
            {
                if (_gameState.BestTime == value) return;

                _gameState.BestTime = value;
                OnPropertyChanged("BestTime");
            }
        }

        public bool IsGameOver => _gameState.CurrentLifes == 0;
        public event PropertyChangedEventHandler PropertyChanged;

        [Inject]
        private void Construct(
            SignalBus signalBus, 
            GameSettings gameSettings, 
            GameSounds gameSounds, 
            CatsSettings catsSettings,
            Countdown countdown,
            IPauseProvider pauseProvider)
        {
            _gameSettings = gameSettings;
            _gameSounds = gameSounds;
            _signalBus = signalBus;
            _catsSettings = catsSettings;
            _countdown = countdown;
            pauseProvider.Register(this);
        }
        private void Awake()
        {
            HealthVM.Clear();
            HealthVM.Init(_gameSettings.Lifes);
            _gameState.CurrentLifes = _gameSettings.Lifes;

            _signalBus.Subscribe<CatFellSignal>(OnCatFellSignal);
            _signalBus.Subscribe<CatSavedSignal>(OnCatSavedSignal);
            _signalBus.Subscribe<CatKidnappedSignal>(OnCatKidnappedSignal);
            _signalBus.Subscribe<PlayerWeaponMissedSignal>(OnPlayerWeaponMissed);
        }
        private IEnumerator Start()
        {
            yield return _countdown.StartCountdown(_gameSettings.PreparationTime);
            _enabled = true;
            SoundManager.PlayMusic(_gameSounds.GameBackground.Clip, _gameSounds.GameBackground.Volume);
        }
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<CatFellSignal>(OnCatFellSignal);
            _signalBus.TryUnsubscribe<CatSavedSignal>(OnCatSavedSignal);
            _signalBus.TryUnsubscribe<CatKidnappedSignal>(OnCatKidnappedSignal);
            _signalBus.TryUnsubscribe<PlayerWeaponMissedSignal>(OnPlayerWeaponMissed);
        }
        private void Update()
        {
            if (!_enabled || IsGameOver) return;

            CurrentTime += Time.deltaTime;
        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void OnCatFellSignal()
        {
            Score += _catsSettings.FallingReward;
        }
        private void OnCatSavedSignal()
        {
            Score += _catsSettings.SavedReward;
            Streak++;
        }
        private void OnCatKidnappedSignal()
        {
            Score -= _catsSettings.KidnapPenalty;
            Streak = 0;
            _gameState.CurrentLifes--;
            HealthVM.RemoveHealth(1);

            if (IsGameOver) OnGameOverSignal();
        }
        private void OnPlayerWeaponMissed()
        {
            Streak = 0;
        }
        private void OnGameOverSignal()
        {
            _signalBus.Fire<GameOverSignal>();

            UpdateStatistics();

            _signalBus.TryUnsubscribe<CatFellSignal>(OnCatFellSignal);
            _signalBus.TryUnsubscribe<CatSavedSignal>(OnCatSavedSignal);
            _signalBus.TryUnsubscribe<CatKidnappedSignal>(OnCatKidnappedSignal);

            SoundManager.StopMusic();
            SoundManager.PlayOneShot(_gameSounds.GameOver.Clip, _gameSounds.GameOver.Volume);
            HealthVM.Reset();
            GameOverVM.Show();
        }

        private void UpdateStatistics()
        {
            _gameState.Deserialize();

            if (BestTime < CurrentTime) BestTime = CurrentTime;
            if (HighScore < Score) HighScore = Score;

            _gameState.Serialize();
        }

        void IPauseHandler.SetPaused(bool isPaused)
        {
            _enabled = !isPaused;
        }
    }
}