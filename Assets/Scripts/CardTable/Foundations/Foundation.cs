using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

namespace CardTable
{
	public class Foundation : MonoBehaviour {
		public Card.Suit TypeOfSuit { get { return placeHolder.TypeOfSuit; } }  // Ritorna il seme (Suit) del "Foundation"

		public int LastCardValue 
		{ 
			get
			{ 
				ICard card = (ICard)cards.Peek ();
				if (card != null)
					return card.Value;
				else
					return 0;
			}
		}  // Ritorna il valore dell'ultima carta per un confronto


		public bool IsFull 
		{
			get
			{
				if ( LastCardValue == GameDefinitions.DECK_SIZE/4 )
					return true;
				return
					false;
			}

		}


		// Proprietà che ritorna l'ultima carta del foundation. Quando è vuoto ritorna il placeHolder con valore 0.
		// Questo permette di semplificare il controllo di validità di una mossa.
		private ICard LastCard{ get { return (ICard)cards.Peek (); } }


		private PlaceHolderForFoundation placeHolder; // PlaceHolder usato come carta fittizia e per ridimensionare il foundation

		private Stack cards = new Stack();  // Le carte presenti nel foundation


		public void Set( Card.Suit typeOfSuit )
		{
			GetComponent<RectTransform> ().SetSizeOfCard ();

			placeHolder = GetComponentInChildren<PlaceHolderForFoundation> ();
			placeHolder.Initialize (typeOfSuit);

			cards.Push ( placeHolder );
		}

		public void PushCard(Card card)
		{
			StartCoroutine (MoveCardToPosition (card));
		}

		public Card RemoveCard()
		{
			Card card = (Card) cards.Pop ();
			LastCard.EnableCard ();

			return card;
		}


		public void Reset()
		{
			StopAllCoroutines ();

			cards.Clear ();
			cards.Push (placeHolder);
			placeHolder.EnableCard ();
		}


		// Animazione di spostamento della carta verso il foundation
		private IEnumerator MoveCardToPosition(Card card)
		{
			card.IsMoving = true;
			yield return StartCoroutine( card.TranslateCardToPosition (transform.position, GameDefinitions.TIME_TO_TRASLATE) );

			card.transform.SetParent (transform);
			card.IsMoving = false;
			LastCard.DisableCard ();
			cards.Push (card);
		}


	}
}
