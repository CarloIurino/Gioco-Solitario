using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GUI
{
	[RequireComponent(typeof(CanvasFade))]
	public class OptionMenu : MonoBehaviour, IPopMenu {
		[SerializeField] private Toggle draw3ModeToggle;
		[SerializeField] private Toggle enableTimeToggle;
		[SerializeField] private CanvasGroup resetMessage;

		public Action OnOptionMenuOpened;
		public Action OnOptionMenuClosed;

		private CanvasFade canvasFade;
		private GameSettingsManager gameSettings;

		private void Awake(){
			canvasFade = GetComponent<CanvasFade> ();
			gameSettings = GameManager._Instance.GameSettingsManager;

			resetMessage.DisableCanvasGroup ();
	
		}

		private void Start()
		{
			InitializeToggles ();
		}

		private void InitializeToggles()
		{
			draw3ModeToggle.isOn = gameSettings.Draw3Mode;
			enableTimeToggle.isOn = gameSettings.EnableTimer;
		}


		public void HidePopMenu()
		{
			InterfaceManager._Instance.HidePopMenu ();
		}


		#region Interface implementation
		public void Show()
		{
			canvasFade.Show ();

			if (OnOptionMenuOpened != null)
				OnOptionMenuOpened ();
		}

		public void Hide()
		{
			canvasFade.Hide ();

			if (OnOptionMenuClosed != null)
				OnOptionMenuClosed ();
		}
		#endregion


		public void ShowResetMessage()
		{
			resetMessage.EnableCanvasGroup ();
		}

		public void HideResetMessage()
		{
			resetMessage.DisableCanvasGroup ();
		}

		//
		// Metodo registrato all'evento OnMenuShow della classe Interface
		// Nasconde automaticamente il menu quando ne viene aperto un altro che implementa l'interfaccia IMenu
		//

		#region GameSettings setters
		public void SetDraw3Mode(bool enable)
		{
			gameSettings.SetDraw3Mode (enable);
		}

		public void SetEnableTimer(bool enable)
		{
			gameSettings.SetEnableTimer (enable);
		}

		public void SetEnableHelps(bool enable)
		{
			gameSettings.SetEnableHelps (enable);
		}

		public void SetEnableSounds(bool enable)
		{
			gameSettings.SetEnableSound (enable);
		}
		#endregion
	
	}
}
