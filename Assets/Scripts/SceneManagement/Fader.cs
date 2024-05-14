using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.SceneManagement
{
    
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Coroutine activeCoroutine = null;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

        }

        public void FadeOutImmediate()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1;

            }
        }
        public IEnumerator FadeOut(float time)
        {

            return Fade(1, time);
        }

        public IEnumerator Fade(float target, float time)
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            activeCoroutine = StartCoroutine(FadeRoutine(target, time));
            yield return activeCoroutine;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            if (_canvasGroup == null)
            {
                yield return null;
            }
            while (!Mathf.Approximately(_canvasGroup.alpha , target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time) ;
                yield return null;
            }
        }
        
        public IEnumerator FadeIn(float time)
        {
            return Fade(0, time);
        }
        
        
    }

    
}
