/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatSuite.Examples.Components {

	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

    [RequireComponent(typeof(Image))]
    public class CameraRecRecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [Header(@"Settings"), Range(5f, 60f), Tooltip(@"Maximum duration that button can be pressed.")]
        public float maxDuration = 15f;

        [Header(@"UI")]
        public Image countdown;

        [Header(@"Events")]
        public UnityEvent onTouchDown;
        public UnityEvent onTouchUp;
        public UnityEvent onTouchCapture;

        private Image button;
        private bool touch;

        private void Awake () => button = GetComponent<Image>();

        private void Start () => Reset();

        private void Reset () {
            button.fillAmount = 1f;
            countdown.fillAmount = 0.0f;
        }

        void IPointerDownHandler.OnPointerDown (PointerEventData eventData) => StartCoroutine(Countdown());

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData) => touch = false;

        private IEnumerator Countdown () {
            touch = true;
            // Wait for false touch
            yield return new WaitForSeconds(0.2f);
            if (!touch)
            {
                onTouchCapture?.Invoke();
                Debug.Log("State 1 ");
                yield break;
            }
            // Start recording
            onTouchDown?.Invoke();
            Debug.Log("State 2 ");
            // Animate the countdown
            var startTime = Time.time;
            while (touch) {
                var ratio = (Time.time - startTime) / maxDuration;
                Debug.Log("Time touch : " + ratio.ToString());
                touch = ratio <= 1f;
                countdown.fillAmount = ratio;
                button.fillAmount = 1.0f - ratio;
                yield return null;
            }
            Reset();
            onTouchUp?.Invoke();
        }
    }
}