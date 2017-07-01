using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardTable;

namespace GUI
{
	[RequireComponent (typeof(CanvasFade))]
	public class PauseMenu : MonoBehaviour, IPopMenu {
		CanvasFade canvasFade;

		private void Awake()
		{
			canvasFade = GetComponent<CanvasFade> ();
		}

		public void HidePopMenu()
		{
			InterfaceManager._Instance.HidePopMenu ();

		}

		public void Show()
		{
			canvasFade.Show ();
		}

		public void Hide()
		{
			canvasFade.Hide ();
		}

	
		#region Buttons Actions
		public void TryAgain()
		{
			GamePlayManager._Instance.TryAgain ();
			HidePopMenu ();
		}

		public void NewGame()
		{
			GamePlayManager._Instance.NewGame ();
			HidePopMenu ();
		}

		public void Resume()
		{
			GamePlayManager._Instance.Resume ();
			HidePopMenu ();
		}
		#endregion
	}
}
