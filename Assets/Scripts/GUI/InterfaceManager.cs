using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GUI
{

	/// <summary>
	/// Gestione centralizzata dei menu.
	/// Può essere attivo un solo menu a comparsa.
	/// un menu aperto si chiude automaticamente quando se ne apre un altro
	/// </summary>
	public class InterfaceManager : MonoBehaviour {

		#region Singleton Pattern
		private static InterfaceManager _instance;
		public static InterfaceManager _Instance { get { return _instance; } }

		void Awake ()
		{
			if (_instance == null) {
				_instance = this;
			} else {
				Destroy (gameObject);
				return;
			}

			DontDestroyOnLoad (gameObject);

		}
		#endregion

		[SerializeField] private OptionMenu optionMenu;

		public Action OnMenuShow;
		public Action OnMenuHide;

		private IPopMenu activeMenu;

		/// <summary>
		/// Mostra il menu che implementa l'interfacci IMenu passato come parametro.
		/// Consente di visualizzare un menu per volta.
		/// Quando si apre un nuovo menu, quello già aperto si chiude automaticamente
		/// </summary>
		/// <param name="menu">Menu.</param>
		public void ShowPopMenu(IPopMenu menu)
		{
			if (activeMenu != null)
				activeMenu.Hide ();

			activeMenu = menu;
			menu.Show ();

			if (OnMenuShow != null)
				OnMenuShow ();
		}

		public void HidePopMenu()
		{
			if (activeMenu != null)
				activeMenu.Hide ();

			activeMenu = null;

			if (OnMenuHide != null)
				OnMenuHide ();
		}



		/// <summary>
		/// Restituisce il menu opzioni
		/// </summary>
		/// <returns>The option menu.</returns>

		public OptionMenu GetOptionMenu()
		{
			return optionMenu;
		}
	}
}
