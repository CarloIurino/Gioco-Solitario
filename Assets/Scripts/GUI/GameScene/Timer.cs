using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardTable;

namespace GUI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class Timer : MonoBehaviour {
		[SerializeField]private Text timerText;

		public float Time{ get { return timer;}}

		private CanvasGroup canvasGroup;
		private float timer = 0;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup> ();
			GamePlayManager._Instance.OnPause += Pause;
			GamePlayManager._Instance.OnResume += Resume;
			GamePlayManager._Instance.OnReset += Reset;

			ChekIfEnable ();
		}

		private void ChekIfEnable()
		{
			if (GameManager._Instance.GameSettings.EnableTimer) {
				Show ();
			} else
				Hide();
		}

		private void Show()
		{
			timer = 0;
			enabled = true;

			canvasGroup.EnableCanvasGroup ();
		}

		private void Hide()
		{
			canvasGroup.DisableCanvasGroup ();
			enabled = false;
		}

		private void Update()
		{
			timer += UnityEngine.Time.deltaTime;

			SetText ();
		}

		private void SetText(){
			timerText.text = (timer / 60 ).ToString("00")  + ":" + (timer%60).ToString("00");
		}


		private void Pause()
		{
			enabled = false;
		}

		private void Resume()
		{
			enabled = true;
		}

		private void Reset()
		{
			timer = 0;
			SetText ();

			ChekIfEnable ();
		}

		void OnDestroy()
		{
			GamePlayManager._Instance.OnPause -= Pause;
			GamePlayManager._Instance.OnResume -= Resume;
			GamePlayManager._Instance.OnReset -= Reset;
		}
	}
}
