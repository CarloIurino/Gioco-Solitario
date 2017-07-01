using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GUI
{
	public class StartMenu : MonoBehaviour {

		private OptionMenu optionMenu;

		void Awake()
		{
			optionMenu = InterfaceManager._Instance.GetOptionMenu ();
		}

		public void StartGame()
		{
			GameManager._Instance.StartGame ();
		}

		public void ShowOptionMenu()
		{
			InterfaceManager._Instance.ShowPopMenu (optionMenu);
		}

		public void CloseApplication()
		{
			GameManager._Instance.CloseApplication ();
		}
	}
}
