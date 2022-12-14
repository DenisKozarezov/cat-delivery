using System;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;
using DG.Tweening;

namespace Core.UI
{
    [Binding]
    public class HealthView : MonoBehaviour
    {
        [SerializeField]
        private Transform _parentPanel;
        [SerializeField]
        private GameObject _heathPrefab;
        [SerializeField]
        private Texture2D _happyEmoji;
        [SerializeField]
        private Texture2D _sadEmoji;

        private int _currentIndex;
        private byte _maxHealth;
        private Tweener _vibeTweener;
        private Transform _lastHealth;

        private void VibeLastHealth(bool isVibing)
        {
            if (isVibing)
            {
                _vibeTweener = _lastHealth
                    .DOPunchScale(Vector3.one * 0.5f, duration: 1f, vibrato: 2, elasticity: 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(_lastHealth.gameObject);
            }
            else
            {
                _vibeTweener.Kill();
                _lastHealth.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear).SetLink(_lastHealth.gameObject);
            }
        }

        public void Init(byte maxHealth)
        {
            _maxHealth = maxHealth;
            _currentIndex = maxHealth;
            for (byte i = 0; i < _maxHealth; i++)
            {
                GameObject prefab = GameObject.Instantiate(_heathPrefab, _parentPanel);
                prefab.transform.SetAsFirstSibling();
            }
            _lastHealth = _parentPanel.GetChild(0);
        }
        public void Clear()
        {
            for (int i = 0; i < _parentPanel.childCount; i++)
            {
                GameObject.Destroy(_parentPanel.GetChild(i).gameObject);
            }
        }
        public void Reset()
        {
            VibeLastHealth(false);
        }
        public void AddHealth(byte value)
        {
            if (_currentIndex == _maxHealth) return;

            int newValue = Math.Min(_currentIndex + value, _maxHealth);
            for (int i = _currentIndex - 1; i < newValue; i++)
            {
                RawImage image = _parentPanel.GetChild(i).GetComponent<RawImage>();
                image.texture = _happyEmoji;
                image.color = Color.white;
            }
            _currentIndex = newValue;
        }
        public void RemoveHealth(byte value)
        {
            if (_currentIndex == 0) return;

            int newValue = Math.Max(_currentIndex - value, 0);
            for (int i = _currentIndex - 1; i >= newValue; i--)
            {
                RawImage image = _parentPanel.GetChild(i).GetComponent<RawImage>();
                image.texture = _sadEmoji;
                image.color = Color.grey;
            }
            _currentIndex = newValue;

            if (_currentIndex == 1) VibeLastHealth(true);
        }
    }
}