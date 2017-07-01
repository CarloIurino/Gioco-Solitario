using Cards;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

namespace CardTable
{
	public class DealerDefaultDraw : MonoBehaviour, IDropZone, IPointerDownHandler {
		
		protected Dealer dealerLoader;  // Il componente che si occupa di caricare questa classe per la modalità di gioco corrispondente
		protected LinkedList<Card> dealerDeck { get { return dealerLoader.DealerDeck; } }  // Il mazzo di carte usato durante il gioco
		protected PlaceHolder placeHolder{ get { return dealerLoader.PlaceHolder; } }

		private GamePlayManager.DropZoneType dropZoneType = GamePlayManager.DropZoneType.Dealer;   // Setto il tipo di IDropZone
		private Waste waste { get { return dealerLoader.Waste; } }
		private bool isDealerEnabled;  // Attiva o disattiva il dealer bypassando l'evento OnpointerDown

	

		/// Chiamato dal loader (Dealer) per l'inizzializzazione appena viene caricato
		public void Initialize(Dealer loader)
		{
			dealerLoader = loader;
			waste.OnAddCardsAnimationCompleted += EnableDealer;  // Registro all'evento per riabilitare il dealer quando vengono posizionate tutte le carte nello scarto (Waste)
		}
			

		public  void EnableDealer()
		{
			isDealerEnabled = true;
		}

		public void DisableDealer()
		{
			isDealerEnabled = false;
		}


		#region IDropZone Implementation

		public Card LastCard{ get { return null; }}

		public GamePlayManager.DropZoneType DropZoneType { get { return dropZoneType; } }


		// Seleziono la prima carta dal dealer e la abilito
		virtual public Card[] PickCards(Card c)
		{
			if ( dealerDeck.Count > 0 ){
				DisableDealer ();

				Card card = dealerDeck.First.Value;
				dealerDeck.RemoveFirst ();

				// Sposto la carta in posizione del dealer e la abilito
				card.transform.position = placeHolder.transform.position;
				card.EnableCard ();

				SetImageIfEmpty (); // Quando il dealer è vuoto cambio immagine

				return new Card[]{card};
			} 
			else {
				return null;
			}
		}


		// Quando viene cliccato sul dealer, incarico il GamePlayManager di spostare la carta verso il Wast
		public void OnPointerDown(PointerEventData eventData)
		{
			// Solo se il dealer è attivo
			if (isDealerEnabled) 
			{
				if (dealerDeck.Count == 0 && (waste.Size == 0 || waste.Size == 1)){
					return;
				}
				else if (dealerDeck.Count == 0 && waste.Size > 1) {  // Se il dealer è vuoto recupero le carte di scarto, se esistono
					GamePlayManager._Instance.ReDeal ();
				} else {
					// Prendo la carta e la sposto nel "Waste"
					GamePlayManager._Instance.PickCardsFromDropZone (null, this);
					GamePlayManager._Instance.DropCardsToDropZone (waste);
				}
			}
		}


		// Aggiungo una carta al dealer. Azione possibile solo quando si ripristina una mossa
		virtual public void AddCard (Card[] cardsToAdd)
		{
			cardsToAdd.SetCurrentDropZone (this);
			dealerDeck.AddFirst (cardsToAdd[0]);
			StartCoroutine (MoveCard ());
		}
			

		public bool OnPickCardsCompleted()
		{
			return false;
		}


		// Non è possibile spostare carte nel dealer. Ritorna sempre falso
		// Vengono aggiunte carte al dealer solo quando si ripristina una mossa ma in quel caso non viene fatto il controllo, la giocata era sicuramente valida
		public bool IsValidPlay(Card[] cards, IDropZone dropZone)
		{
			return false;
		}


		/// <summary>
		/// Resetta il dealer
		/// </summary>
		public void Reset()
		{
			isDealerEnabled = false;
		}

		#endregion


		protected IEnumerator MoveCard()
		{
			Card card = dealerDeck.First.Value;

			card.IsMoving = false;
			card.HideCard ();

			yield return new WaitForSeconds (0.1f);


            yield return StartCoroutine(card.TranslateCardToPosition (placeHolder.transform.position, GameDefinitions.TIME_TO_TRASLATE));

            card.transform.SetParent(transform);


            card.DisableCard ();

			SetImageIfEmpty ();
		}


		// Se il dealer è vuoto setto l'immagine corrispondente
		protected void SetImageIfEmpty()
		{
			if (dealerDeck.Count == 0) {
				placeHolder.SetEmptyImage ();
			} else
				placeHolder.SetBackFaceImage ();
		}


		private void OnDestroy()
		{
			waste.OnAddCardsAnimationCompleted -= EnableDealer;
		}

	}
}
