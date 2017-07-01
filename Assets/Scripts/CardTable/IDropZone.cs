using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using System;

namespace CardTable
{

	public interface IDropZone 
	{
		GamePlayManager.DropZoneType DropZoneType{ get; }
		Card LastCard{ get;}


		/// <summary>
		/// Preleva le carte dalla zona di gioco selezionata
		/// </summary>
		/// <returns>Array di carte</returns>
		/// <param name="card">La carta selezionata dal puntatore </param>
		Card[] PickCards (Card card);

		/// <summary>
		/// Aggiunge le carte prelevate nella zona di gioco selezionata
		/// </summary>
		/// <param name="card">Carte da aggiungere</param>
		void AddCard (Card[] cards);


		//		Card RemoveCard (Card card);

		/// <summary>
		/// Verifica che la giocata sia valida
		/// </summary>
		/// <returns><c>true</c> Se la giocata è valida ritorna True</returns>
		/// <param name="cards">Array di carte da spostare</param>
		bool IsValidPlay(Card[] cards, IDropZone dropZone);

		bool OnPickCardsCompleted ();
	}
}
