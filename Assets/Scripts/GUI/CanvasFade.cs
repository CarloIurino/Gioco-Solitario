using UnityEngine;


namespace GUI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasFade : MonoBehaviour 
	{
		[SerializeField] private bool visibleAtStart;
		public float TIME = 0.5f;
		CanvasGroup cg;
		float start;
		float end;
		float delta;

		private bool isVisible;

		void Awake () 
		{
			cg = GetComponent<CanvasGroup> ();
			enabled = false;

			if (visibleAtStart) {
				cg.EnableCanvasGroup ();

				isVisible = true;
			} else {
				cg.DisableCanvasGroup ();

				isVisible = false;
			}

		}


		void Update () 
		{
			if (delta/TIME > 1) 
				enabled = false;

			delta += Time.unscaledDeltaTime;
			cg.alpha = Mathf.Lerp (start, end, delta/TIME);
		}

		public void Show()
		{
			if (isVisible)
				return;


			isVisible = true;
			cg.interactable = true;
			cg.blocksRaycasts = true;
			delta = 0;
			start = 0;
			end = 1;
			enabled = true;
		}

		public void Hide() 
		{
			if (!isVisible)
				return;

			isVisible = false;
			cg.interactable = false;
			cg.blocksRaycasts = false;
			delta = 0;
			start = 1;
			end = 0;
			enabled = true;
		}

	}
}
