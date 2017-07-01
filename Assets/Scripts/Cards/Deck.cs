using UnityEngine;
using System.Collections.Generic;

namespace Cards
{
	/// <summary>
	/// Il componente che rappresenta il mazzo di carte.
	/// Si occupa di inizializzare, gestire le carte e restituire un nuovo mazzo mischiato.
	/// Le carte sono memorizzate in un vettore per velocizzare il mescolamento.
	/// Quando viene richiesto un nuovo mazzo di carte viene restituita una linkedList con riferimento alle carte nell'array.
	/// Le carte non vengono mai distrutte. Questo evita rallentamenti dovuti all'inizializzazione e distruzione di carte e problemi di garbage collector, 
	/// soprattutto quando viene terminata una partita. Se così non fosse sarebbero distrutte e ricreate carte in un breve lasso di tempo, 
	/// mentre vengono eseguite contemporaneamente animazioni, calcolo dei punteggi, menu e mescolamento delle carte.
	/// Non c'è un sistema di pooling centralizzato ma ogni componente del gioco si occupa di abilitare o disabilitare visivamente le carte.
	/// </summary>
	public class Deck : MonoBehaviour
	{
		[SerializeField] private Card cardPrefab;
		[SerializeField] private Transform gameObjectToParent; // Parent al Dealer

		private Card[] cards = new Card[GameDefinitions.DECK_SIZE]; // Array contenente le carte

		private CardResources resorces; // Classe che gestisce le risorse delle carte, come le immagini

		private void Awake ()
		{
			resorces = new CardResources (); // Carico le immagini delle carte
			InitzializeCards ();
		}


		/// <summary>
		/// Inizializza le carte ed assegna un id.
		/// </summary>
		void InitzializeCards()
		{
			int cardsOfASuit = GameDefinitions.DECK_SIZE / 4;

			for (int i = 0; i < GameDefinitions.DECK_SIZE; i++) {
				cards[i] = (Card) Instantiate ( cardPrefab, gameObjectToParent);
				cards [i].InitializeCard (i, (Card.Suit)(i / cardsOfASuit), (i % cardsOfASuit + 1));
				cards [i].name = i.ToString ();
			}
		}
			
		/// <summary>
		/// Restituisce una Linked List delle carte mescolate
		/// </summary>
		/// <returns>The new deck.</returns>
		public LinkedList <Card> GetNewDeck()
		{
			Shuffle ();

			LinkedList<Card> deck = new LinkedList<Card> ();

			foreach (Card card in cards)
				deck.AddFirst (card);

			return deck;
		}


		/// <summary>
		/// Restituisce una Linked List delle carte senza rimescolarle
		/// </summary>
		/// <returns>The new deck.</returns>
		public LinkedList <Card> GetSameDeck()
		{
			LinkedList<Card> deck = new LinkedList<Card> ();

			foreach (Card card in cards)
				deck.AddFirst (card);

			return deck;
		}



		/// <summary>
		/// Disabilita visivamente le carte e le riposiziona sul dealer
		/// </summary>
		public  void ResetCards()
		{
			foreach (Card card in cards) {
				card.transform.SetParent (gameObjectToParent);
				card.transform.position = gameObjectToParent.position;
				card.Reset ();
			}
		}


		/// <summary>
		/// Mescola le carte utilizzando l'algoritmo Fischer-Yates Shuffle
		/// </summary>
		private void Shuffle( )
		{
			System.Random random = new System.Random();

			for( int i = 0; i < cards.Length; i ++ ) {
				int j = random.Next( i, cards.Length);
				Card temp = cards[ i ];
				cards[ i ] = cards[ j ];
				cards[ j ] = temp;
			}
		}

	}
}
