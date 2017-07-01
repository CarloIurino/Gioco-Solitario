using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using System;
using UnityEngine.EventSystems;

namespace CardTable
{
	public class Waste : MonoBehaviour, IDropZone 
	{
		[SerializeField] private Transform placeHolderAsPivot;

		public Action OnAddCardsAnimationCompleted;

		public int Size { get { return cards.Count; } }   // Le carte di scarto


		private Vector2[] pivots = new Vector2[3];  	// I punti di ancoraggio delle carte visibili
		private GamePlayManager.DropZoneType dropZoneType = GamePlayManager.DropZoneType.Waste; 	 // Setto il tipo di questa IDropZone
		private LinkedList<Card> cards = new LinkedList<Card>(); 		// Tutte le carte di scarto


		private void Awake()
		{
			GamePlayManager._Instance.OnReset += Reset;

		}


		private void SetPivots()
		{
			// Setto i punti di ancoraggio delle tre carte visibili dello scarto
			float offset = CardExtensionMethods.GetScaledWidthOfCard () * 0.4f;
			pivots [2] = transform.position;
			pivots [1] = new Vector2 (pivots [2].x - offset, pivots [2].y);
			pivots[0] = new Vector2 (pivots [2].x - 2*offset, pivots [2].y);
		}


		// Viene cancellata la lista delle carte di scarto. La lista viene passata come valore di ritorno
		public LinkedList<Card> ReDeal()
		{
			LinkedList<Card> newDeck = cards;
			cards = new LinkedList<Card> ();

			return newDeck;
		}


		/// <summary>
		/// Annullo la mossa del ReDeal riportando le carte nello scarto (Waste)
		/// </summary>
		/// <param name="cards">Cards.</param>
		public void RestoreReDeal(LinkedList<Card> cards)
		{
			this.cards = cards;

			foreach (Card card in cards) {
				card.transform.SetParent (transform);
				card.SetCurrentDropZone (this);
				card.ShowCard ();
			}

			StartCoroutine (MoveCards ());
		}


		/// <summary>
		/// Ripristino lo scarto
		/// </summary>
		public void Reset()
		{
			StopAllCoroutines ();
			cards.Clear ();
		}
			


		#region IDropZone Implementation

		// La carta che è possibile prelevare
		public Card LastCard{ get { return cards.Count > 0 ? cards.Last.Value : null; }}
		public GamePlayManager.DropZoneType DropZoneType { get { return dropZoneType; } }

		// Prende la prima carta dallo scarto
		public Card[] PickCards (Card c){ 
			Card card = cards.Last.Value;
			cards.RemoveLast ();

			return new Card[] { card };
		}


		// Aggiunge le carte allo scarto
		public void AddCard (Card[] cardsToAdd)
		{
			cardsToAdd.SetCurrentDropZone (this);

			if (cards.Count > 0) {
				cards.Last.Value.SetInteractable (false);
			}

			StartCoroutine(AddOneAtTime (cardsToAdd));
		}
			

		// Quando viene presa la prima carta, quelle precedenti si riposizionano
		public bool OnPickCardsCompleted(){
			StartCoroutine(MoveCards ());

			return false;
		}


		// Controllo se la giocata è valida
		public bool IsValidPlay(Card[] cards, IDropZone dropZone)
		{
			if (dropZone.DropZoneType == GamePlayManager.DropZoneType.Dealer) {
				return true;
			}
			else
				return false;
		}
			
		#endregion



		// Nella modalità Draw3 le carte vengono inserite una alla volta secondo un tempo di attesa tra una e l'altra.
		IEnumerator AddOneAtTime(Card[] cardsToAdd)
		{
			foreach (Card card in cardsToAdd) {
				cards.AddLast (card);
				card.SetInteractable (false);
				StartCoroutine (MoveCards ());

				yield return new WaitForSeconds (0.2f);
			}

			yield return new WaitWhile (() => LastCard.IsMoving);

			if (OnAddCardsAnimationCompleted != null)
				OnAddCardsAnimationCompleted ();
		}


		// Riposiziona le tra carte visibili quando ne viene aggiunta o prelevata una
		private IEnumerator MoveCards()
		{
			if (cards.Count > 0) {

				Stack<Card> visibleCards = new Stack<Card> ();

				LinkedListNode<Card> node = cards.Last;

				for (int i = 0; i < 3 && node != null; i++) {
					visibleCards.Push (node.Value);
					node = node.Previous;
				}


				for (int i = 0; i < 3 && visibleCards.Count > 0; i++) {
					Card card = (Card)visibleCards.Pop ();

					StartCoroutine (SetInPosition (card, pivots [i]));
				}
					

				yield return new WaitWhile (() => LastCard.IsMoving);

				cards.Last.Value.SetInteractable (true);
				if (node != null) {
					node.Value.DisableCard ();
				}
			}

		}
			

		// Imposta la carta come posizionata e visibile tra le tre consentite dal "Waste"
		private IEnumerator SetInPosition(Card card, Vector3 position)
		{
			card.EnableCard ();
			card.SetInteractable (false);
			card.IsMoving = true;
			yield return StartCoroutine(card.TranslateCardToPosition (position, GameDefinitions.TIME_TO_TRASLATE/2));
			card.transform.SetParent (transform);
			card.ShowCard ();
			card.IsMoving = false;
		}


		// Reimposto i pivot quando viene fatto un resize
		private void Update()
		{
			if (transform.hasChanged) {
				transform.hasChanged = false;
				SetPivots ();
			}
		}
			

		private void OnDestroy()
		{
			GamePlayManager._Instance.OnReset -= Reset;
		}
	}
}
