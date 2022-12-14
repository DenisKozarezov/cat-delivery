using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Core.UI
{
    public enum FadeMode { On, Off }

    [RequireComponent(typeof(RawImage))]
    public class Vignette : MonoBehaviour
    {
        private RawImage _vignette;

        private void Awake()
        {
            _vignette = GetComponent<RawImage>();
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator Fade(FadeMode mode, float duration)
        {
            float alpha = mode == FadeMode.On ? 0f : 1f;
            _vignette.color = _vignette.color.WithAlpha(1f - alpha);
            yield return _vignette.DOFade(alpha, duration)
                .SetEase(Ease.Linear)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    if (mode == FadeMode.On) Destroy(gameObject);
                });
        }
    }
}