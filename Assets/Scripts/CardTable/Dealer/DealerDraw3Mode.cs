using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

namespace CardTable
{
	/// <summary>
	/// DealerDraw3Mode estende la classe base DealerDefaultDraw.
	/// Si differenzia dalla classe base per l'override del metodo PickCards.
	/// Preleva tre carte dal mazzo a differenza della classe base che ne preleva una per volta
	/// </summary>
	public class DealerDraw3Mode : DealerDefaultDraw {

		// Prendo tre carte dal dealer
		override public Card[] PickCards(Card c)
		{
			if ( dealerDeck.Count > 0 ){
				DisableDealer (); // Disabilito il dealer metre sposto le carte new "Waste". Sarà riabilitato al termine dello spostamento

				//
				// Scorro la linked list per tre nodi e salvo le carte nel vettore ritornato dal metodo
				Card[] cards = new Card[ Mathf.Clamp(dealerDeck.Count, 1, 3) ];

				LinkedListNode<Card> node = dealerDeck.First;
				for (int i = 0; i < cards.Length && node != null; i++) {
					cards [i] = node.Value;

					node = node.Next;
				}
				/////////////

				// Posiziono le carte sul dealer e le abilito
				foreach( Card card in cards){
					card.transform.position = placeHolder.transform.position;
					card.EnableCard ();
					dealerDeck.RemoveFirst ();
				}

				SetImageIfEmpty (); // Cambio immagine se il dealer è vuoto
					
				return cards;
			} 
			else {
				return null;
			}
		}

		override public void AddCard(Card[] cardsToAdd)
		{
			cardsToAdd.SetCurrentDropZone (this);

			StartCoroutine (AddRoutine (cardsToAdd));

		}

		private IEnumerator AddRoutine(Card[] cardsToAdd)
		{
			for( int i = cardsToAdd.Length-1; i >= 0; i--){
				dealerDeck.AddFirst (cardsToAdd[i]);

				cardsToAdd [i].transform.SetAsLastSibling ();

				StartCoroutine (MoveCard ());

				yield return new WaitForSeconds (0.05f);
			}
		}
	}
}
