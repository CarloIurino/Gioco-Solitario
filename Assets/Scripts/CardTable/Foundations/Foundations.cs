using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using UnityEngine.EventSystems;

namespace CardTable
{
	public class Foundations: MonoBehaviour, IDropZone, IDropHandler {
		[SerializeField] Foundation foundationPrefab;

		public bool IsFull
		{
			get{
				foreach (Foundation foundation in foundations.Values) {
					if ( !foundation.IsFull  )
						return false;
				}

				return true;
			}
		}

		private GamePlayManager.DropZoneType dropZoneType = GamePlayManager.DropZoneType.Foundation;  // Setto il tipo di IDropZone
		private Dictionary<Card.Suit, Foundation> foundations = new Dictionary<Card.Suit, Foundation>(4);  // Dizionario contenente i 4 "Foundation" per un accesso rapido tramite chiave (Suit)


		void Awake()
		{
			for (int i = 0; i < 4; i++) {
				Foundation foundation = (Foundation)Instantiate (foundationPrefab, transform);
				foundation.GetComponent<RectTransform> ().SetSizeOfCard (); // Ridimensiono dinamicamente secondo le dimensioni della carta da gioco
				foundation.Set ((Card.Suit)i); // Setto il tipo(Suit) del "Foundation"

				foundations.Add (foundation.TypeOfSuit, foundation);  // Aggiungo al dizionario
			}

			GamePlayManager._Instance.OnReset += Reset;
		}


		#region IDropZone Implementation
		public Card LastCard{ get{ return null;}}
		public GamePlayManager.DropZoneType DropZoneType { get { return dropZoneType; } }


		// La carta selezionata dal puntatore viene rimossa dal foundation
		public Card[] PickCards (Card card)
		{
			return new Card[]{ foundations [card.TypeOfSuit].RemoveCard () };
		}


		// Aggiungo la carta al foundation con lo stesso seme
		public void AddCard (Card[] cardsToAdd)
		{
			cardsToAdd.SetCurrentDropZone (this);

			foundations [cardsToAdd[0].TypeOfSuit].PushCard (cardsToAdd[0]); 
		}



		public bool OnPickCardsCompleted(){ return false; }


		// Controllo che la carta selezionata dal puntatore può essere aggiunta ad uno dei foundation
		public bool IsValidPlay(Card[] cards, IDropZone dropZone)
		{
			if (cards.Length != 1)
				return false;
			
			if (foundations [cards[0].TypeOfSuit].LastCardValue == cards[0].Value - 1)
				return true;
			else
				return false;
		}

		#endregion


		// Quando viene rilasciata una carta sui "Foundation"  interpello il gamePlayManager che si occuperà delle operazioni di spostamento
		public void OnDrop(PointerEventData eventData)
		{
			GamePlayManager._Instance.DropCardsToDropZone ( this);
		}


		private void Reset()
		{
			foreach (Foundation foundation in foundations.Values)
				foundation.Reset ();
		}


		private void OnDestroy()
		{
			GamePlayManager._Instance.OnReset -= Reset;
		}
	}
}
