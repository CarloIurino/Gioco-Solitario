using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardTable;

namespace GUI
{
	public class BottomMenu : MonoBehaviour {

		[SerializeField] private PauseMenu pauseMenu;

		private OptionMenu optionMenu;

		void Awake()
		{
			optionMenu = InterfaceManager._Instance.GetOptionMenu ();
		}

		public void StartMenu()
		{
			GameManager._Instance.StartMenu ();
		}

		public void ShowOptionMenu()
		{
			InterfaceManager._Instance.ShowPopMenu (optionMenu);
		}

		public void ShowPauseMenu()
		{
			InterfaceManager._Instance.ShowPopMenu (pauseMenu);
			GamePlayManager._Instance.PauseGame ();
		}

		public void UndoPlay()
		{
			GamePlayManager._Instance.RestoreLastPlay ();
		}
	}
}
