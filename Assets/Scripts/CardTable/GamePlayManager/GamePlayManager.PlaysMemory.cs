using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

namespace CardTable
{
	public partial class GamePlayManager {
		Stack<APlay> plays = new Stack<APlay>();



		/// <summary>
		/// Ripristina l'ultima giocata
		/// </summary>
		public void RestoreLastPlay()
		{
            if (plays.Count == 0)
                return;

			APlay aPlay = UnregisterPlay ();


			RestorePlay (aPlay, draggedCardsContainer);
			moves++;
		}


		/// <summary>
		/// Ripristina la giocata attuale quando non è valida
		/// </summary>
		public void RestoreCurrentPlay()
		{
			if (currentPlay != null) {
				RestorePlay (currentPlay, draggedCardsContainer);
			}
				
			currentPlay = null;
		}


		/// <summary>
		/// Memorizza una giocata che può essere ripristinata
		/// </summary>
		/// <returns>The play.</returns>
		/// <param name="reDeal">If set to <c>true</c> re deal.</param>
		/// <param name="draggedCards">Dragged cards.</param>
		/// <param name="dropZone">Drop zone.</param>
		/// <param name="previousCardWasHidden">If set to <c>true</c> previous card was hidden.</param>
		/// <param name="points">Points.</param>
		private APlay RegisterPlay(bool reDeal, Card[] draggedCards, IDropZone dropZone, bool previousCardWasHidden, int points)
		{
			APlay aPlay = new APlay (reDeal, draggedCards, dropZone, previousCardWasHidden, points);

			return RegisterPlay(aPlay);
		}


		private APlay RegisterPlay(APlay aPlay)
		{
			plays.Push (aPlay);

			return (APlay)plays.Peek ();
		}


		/// <summary>
		/// Rimuove dalla memoria una giocata
		/// </summary>
		/// <returns>The play.</returns>
		private APlay UnregisterPlay()
		{
			if (plays.Count > 0) {
				return (APlay)plays.Pop ();
			}
			else
				return null;
		}



	
		/// <summary>
		/// Ripristina la giocata passata come parametro
		/// </summary>
		/// <param name="aPlay">A play.</param>
		/// <param name="container">Container.</param>

		public  void RestorePlay(APlay aPlay, Transform container)
		{
			if (aPlay == null) 
				return;

			IDropZone currentPosition = null;

			if (aPlay.ReDeal) {
				dealer.RestoreReDeal ();
			}
			else {
				// Scandisce le carte della giocata da ripristinare
				for ( int i = aPlay.Cards.Length-1; i >= 0; i--) {
					Card card = aPlay.Cards [i];
					currentPosition = card.GetCurrentDropZone ();

					if (currentPosition != null)
						currentPosition.PickCards (card);

					card.SetCardDragging (container);
					card.transform.SetAsFirstSibling ();
				}

				// Se si tratta di una pila verifico se c'è una carta da coprire a causa del ripristino
				if (aPlay.LastCardPosition.DropZoneType == GamePlayManager.DropZoneType.Pile && aPlay.PreviousCardWasHidden) 
					((Pile)aPlay.LastCardPosition).HideLastCard ();

				// Riporto la carta nella posizione precedente
				aPlay.LastCardPosition.AddCard (aPlay.Cards);


				if ( currentPosition != null )
					currentPosition.OnPickCardsCompleted ();

				points -= aPlay.Points;
			}
		}


	}
		

	public class APlay
	{
		public bool ReDeal;
		public Card[] Cards;
		public IDropZone LastCardPosition;
		public bool PreviousCardWasHidden;
		public int Points;


		public APlay( bool reDeal, Card[] draggedCards, IDropZone dropZone, bool previousCardWasHidden, int points )
		{
			ReDeal = reDeal;
			Cards = draggedCards;
			LastCardPosition = dropZone;
			PreviousCardWasHidden = previousCardWasHidden;
			Points = points;
		}
	}
}
