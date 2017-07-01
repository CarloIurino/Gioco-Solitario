using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using UnityEngine.EventSystems;
using System;


namespace CardTable
{
	/// <summary>
	/// Classe che gestisce una singola pila di carte del gioco solitario.
	/// Implementa l'interfaccia IDrawZone.
	/// </summary>
	public class Pile : MonoBehaviour, IDropZone, IDropHandler 
	{
		[SerializeField] Transform pivot; // Punto di ancoraggio della prima carta

		public int TurnedCards{ get { return turnedCards; } }

		private static TableauController _columnsGenerator;  // Controller che inizializza le pile
		private Stack<Card> cards = new Stack<Card>(); // La pila che memorizza la posizione delle carte simulando la situazione reale del gioco
		private Vector3 anchorPosition;  // Il punto di ancoraggio per le carte. varia ogni volta che una carta viene aggiunta/rimossa

		private float facingUpCardOffset;
		private float facingDownCardOffset;

		private  int movingCardsCount = 0; // Tiene traccia del numero di carte che stanno eseguendo l'animazione. Serve a tenere disattivata la pila fin quando tutte le carte sono al posto
		private bool fastAnimation;

		private int turnedCards = 0;

		void Awake()
		{
			if (_columnsGenerator == null)
				_columnsGenerator = GetComponentInParent<TableauController> ();

			GamePlayManager._Instance.OnReset += Reset;
		}


		IEnumerator Start()
		{
			// Attendo un frame per consentire al componente HorizontalLayout di fare il resize delle colonne prima di leggere le posizioni dei Pivot
			yield return new WaitForEndOfFrame ();
			anchorPosition = pivot.position;

			// Notifico che la pila è inizializzata e pronta
			_columnsGenerator.AddReadyPile ();

			facingUpCardOffset = CardExtensionMethods.GetScaledHeightOfCard() * GameDefinitions.RELATIVE_SPACE_FACING_UP_CARDS;
			facingDownCardOffset = CardExtensionMethods.GetScaledHeightOfCard() * GameDefinitions.RELATIVE_SPACE_FACING_DOWN_CARDS;

            Debug.Log(facingDownCardOffset);
		}



		#region IDropZone Implementation

		private GamePlayManager.DropZoneType dropZoneType = GamePlayManager.DropZoneType.Pile; 
		public GamePlayManager.DropZoneType DropZoneType { get { return dropZoneType; } }

		public Card LastCard 
		{ 
			get { 
				if (cards.Count > 0)
					return (Card)cards.Peek();
				else
					return null;
			} 
		}


		/// <summary>
		/// Abilita o disabilita l'animazione di posa della carta. Utile a disabilitare l'animazione durante la distribuzione delle carte
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		public void EnablePoseCardAnimation(bool state)
		{
			fastAnimation = !state; 
		}


		// Seleziono la carta della pila passata come parametro e tutte le carte collegate ad essa dal quel punto fino alla fine della pila
		public Card[] PickCards (Card mainCard)
		{
			Stack<Card> selectedCards = new Stack<Card> ();

			Card tmpCard;

			do {
				tmpCard = RemoveCard ();  // Rimuove la carta dallo stack e riposiziona il punto di ancoraggio
				tmpCard.SetInteractable(false);
				tmpCard.StopAllCoroutines();

				if (!tmpCard.Equals (mainCard)) {
					tmpCard.FollowCard( (Card)cards.Peek() );
				}

				selectedCards.Push (tmpCard);
			} while (cards.Count > 0 && !tmpCard.Equals(mainCard));


			return selectedCards.ToArray();
		}


		// Aggiungo la pila di carte passata come parametro alla pila corrente
		public void AddCard(Card[] cardsToAdd)
		{
			cardsToAdd.SetCurrentDropZone (this);

			// Calcola l'offset tra una carta e l'altra a seconda che la carta precedente sia coperta o scoperta
			if ( LastCard != null ){
				if ( LastCard.IsHidden ) {
					anchorPosition.y -= facingDownCardOffset;
				} 
				else {
					anchorPosition.y -= facingUpCardOffset;
				}
			}

			// Posiziono la prima carta della serie
			cards.Push (cardsToAdd[0]);
			cardsToAdd[0].StopFollowCard ();

			// Animazione traslazione
			StartCoroutine (MoveCardToPosition (cardsToAdd[0], anchorPosition));

			// Aggiungo (se esistono) una ad una le carte calcolando l'offset per il posizionamento
			for( int i = 1; i < cardsToAdd.Length; i++ ) {
				
				anchorPosition.y -= facingUpCardOffset; 
				cards.Push (cardsToAdd[i]);
				cardsToAdd[i].StopFollowCard ();

				// Animazione traslazione
				StartCoroutine (MoveCardToPosition (cardsToAdd[i], anchorPosition));

			}
		}
			


		// A termine di uno spostamento, se l'ultima carta della pila è copera, la scopro
		public bool OnPickCardsCompleted()
		{
			return ShowLastCard ();
		}


		// Scopro l'ultima carta della pila se questa è coperta
		public bool ShowLastCard()
		{
			if (LastCard != null && LastCard.IsHidden) {
				LastCard.ShowCard ();
				LastCard.SetInteractable (true);

				return true;
			}

			return false;
		}


		// Nascondo l'ultima carta della pila
		public bool HideLastCard()
		{
			if (LastCard != null && !LastCard.IsHidden) {
				LastCard.HideCard ();
				LastCard.SetInteractable (false);

				return true;
			}

			return false;
		}


		// Controlla se la mossa è valida secondo le regole del gioco
		public bool IsValidPlay(Card[] draggedCards, IDropZone dropZone)
		{
			Card card = draggedCards[0];

			if (dropZone.Equals (this))
				return false;

			if (cards.Count == 0) {
				if (card.Value == GameDefinitions.DECK_SIZE / 4)
					return true;
				else
					return false;
			} 
			else if (LastCard.IsRed != card.IsRed && LastCard.Value == card.Value + 1) {
				return true;
			} 
			else
				return false;

		}
			
		#endregion
			


		/// <summary>
		/// Implementazione dell'interfaccia IDropHandler.
		/// Quando una carta è rilasciata nella zona di Drop, viene fatta una chiamata al GamePlaymanager
		/// che si occuperà di verificare che la giocata si validà e in caso positivo sposterà le carte
		/// </summary>
		public void OnDrop(PointerEventData eventData)
		{
			GamePlayManager._Instance.DropCardsToDropZone ( this);
		}


		/// <summary>
		/// Rimuovo dallo stack la carta selezionata
		/// </summary>
		/// <returns>The card.</returns>
		private Card RemoveCard()
		{
			if (cards.Count > 0) {
				Card card = (Card)cards.Pop();
				anchorPosition = LastCard != null ? LastCard.transform.position : pivot.position;

				return card;
			} 
			else
				return null;
		}


		// Animazione che porta la carta in posizione
		private IEnumerator MoveCardToPosition(Card card, Vector3 targetPosition)
		{
			MakeCardsNOTDraggable (); // Disattivo tutte le carte della pila durante l'animazione
			movingCardsCount++;

			card.IsMoving = true;

            
            if (fastAnimation) {
				yield return StartCoroutine (card.TranslateCardToPosition (targetPosition, GameDefinitions.TIME_TO_TRASLATE));
			} else {
				yield return StartCoroutine (card.TranslateCardToPosition (targetPosition - Vector3.up * CardExtensionMethods.GetScaledHeightOfCard () * 0.2f, GameDefinitions.TIME_TO_TRASLATE_TO_PILES));
				yield return StartCoroutine (card.TranslateCardToPosition(targetPosition, GameDefinitions.TIME_TO_TRASLATE*2f));
			}

            card.transform.SetParent(transform);

            card.IsMoving = false;

			movingCardsCount--;
			if ( movingCardsCount == 0)
				MakeCardsDraggable ();  // Riattivo solo se tutte le carte della pila sono posizionate
		}



		private void MakeCardsDraggable()
		{
			foreach (Card card in cards) {
				if (!card.IsHidden) {
					card.SetInteractable (true);
				}
			}
		}


		private void MakeCardsNOTDraggable()
		{
			foreach (Card card in cards) 
				card.SetInteractable (false);
		}



		private void Reset()
		{
			StopAllCoroutines ();

			fastAnimation = false;
			movingCardsCount = 0;
			anchorPosition = pivot.position;
			cards.Clear ();
		}
			

		void OnDestroy()
		{
			GamePlayManager._Instance.OnReset -= Reset;
		}

	}
}
