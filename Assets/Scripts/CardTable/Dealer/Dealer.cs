using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

namespace CardTable
{
	public class Dealer : MonoBehaviour
	{
		[SerializeField] protected PlaceHolder placeHolder;
		[SerializeField] private Waste waste;

		public bool IsEmpty{ get { return dealerDeck.Count == 0 ? true : false; } }
		public Waste Waste{ get { return waste; } }
		public PlaceHolder PlaceHolder{ get { return placeHolder; } }

		public LinkedList<Card> DealerDeck { get { return dealerDeck; } }

		private DealerDefaultDraw dealer;
		private LinkedList<Card> dealerDeck = new LinkedList<Card>();

		private Canvas canvasToRenderPlaceHolderOnTop;

		private void Awake()
		{
			GamePlayManager._Instance.OnReset += Reset;

			ChangeDealMode ();
		}
			

		/// <summary>
		/// Bypassa l'evento OnPointerDown. Cliccando sul dealer non viene scoperta la carta.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		public void EnableDealer( bool state )
		{
			if (dealer != null) {
				if (state)
					dealer.EnableDealer ();
				else
					dealer.DisableDealer ();
			}
		}


		/// <summary>
		/// Viene assegnato un nuovo mazzo di carte mischiato.
		/// Carica gli script per la modalità di gioco
		/// </summary>
		/// <param name="deck">Deck.</param>
		public void SetNewDeck(LinkedList<Card> deck)
		{
			dealerDeck = deck;
		}


		public void SetDealingMode(bool state, Transform toParent)
		{
			if (state && canvasToRenderPlaceHolderOnTop == null) {
				canvasToRenderPlaceHolderOnTop = placeHolder.gameObject.AddComponent<Canvas> ();
				canvasToRenderPlaceHolderOnTop.overrideSorting = true;
				canvasToRenderPlaceHolderOnTop.sortingOrder = 10;

			} else {
				Destroy (canvasToRenderPlaceHolderOnTop);
			}
		}


		/// <summary>
		///  Restituisce la prima carta estratta dal mazzo.
		/// Viene chiamato ripetutamente a inizio partita per distribuire le carte sul tavolo
		/// </summary>
		/// <returns>The A card.</returns>
		public Card DealACard()
		{
			if (dealerDeck.Count > 0) {
				
				Card card = dealerDeck.First.Value;
				dealerDeck.RemoveFirst ();
				card.EnableCard ();
				card.transform.position -= Vector3.up * CardExtensionMethods.GetScaledHeightOfCard () * 0.3f;

				return card;
			}
			return null;
		}



		/// <summary>
		/// Redeal, riprendo le carte di scarto
		/// </summary>
		public void ReDeal ()
		{
			placeHolder.SetBackFaceImage ();

			dealerDeck = waste.ReDeal ();

			foreach (Card card in dealerDeck) {
				card.Reset ();
				card.transform.SetParent (transform);
			}
		}


		/// <summary>
		/// Annullo la mossa, Redeal
		/// </summary>
		public void RestoreReDeal()
		{
			placeHolder.SetEmptyImage ();

			waste.RestoreReDeal (dealerDeck);
			dealerDeck = new LinkedList<Card> ();
		}


		/// <summary>
		/// Sceglie se caricare la classe base per il gioco in modalità default o
		/// la classe derivata per la modalità Draw 3
		/// </summary>
		private void ChangeDealMode()
		{

			if (dealer != null)
				Destroy (dealer);
			
			if (GamePlayManager._Instance.CurrentGameSettings.Draw3Mode) {
				dealer = gameObject.AddComponent<DealerDraw3Mode> ();
				dealer.Initialize (this);

			} 
			else{
				dealer = gameObject.AddComponent<DealerDefaultDraw> ();
				dealer.Initialize(this);

			}
		}


		private void Reset()
		{
			StopAllCoroutines ();

			placeHolder.SetBackFaceImage ();
			SetDealingMode (false, null);

			ChangeDealMode ();
		}


		private void OnDestroy()
		{
			GamePlayManager._Instance.OnReset -= Reset;
		}

	}
}