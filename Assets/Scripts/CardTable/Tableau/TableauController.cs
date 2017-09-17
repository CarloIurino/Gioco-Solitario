using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;

namespace CardTable
{
	public class TableauController : MonoBehaviour {
		[SerializeField] Pile pile0;


		public int CardsToTurn { get {return cardsToTurn;} }
		public bool ArePilesReady { get { return readyPilesCount == GameDefinitions.NUMBER_OF_PILES; } }

		private Pile[] piles = new Pile[GameDefinitions.NUMBER_OF_PILES];
		private int readyPilesCount = 0;
		private int cardsToTurn;

		void Awake()
		{
			piles [0] = pile0;

			for (int i = 1; i < GameDefinitions.NUMBER_OF_PILES; i++) {
				piles[i] = (Pile) Instantiate (pile0, transform, false);
				piles[i].name = "Pile" + i;
			}

			for (int i = 1; i <= GameDefinitions.NUMBER_OF_PILES - 1; i++)
				cardsToTurn += i;
		}


			
		public void AddReadyPile()
		{
			readyPilesCount++;
		}


		/// <summary>
		/// Scansiona le pile una per volta cercando una carta da inserire nel Foundation
		/// </summary>
		public void  Autocomplete(IDropZone destination)
		{
			StartCoroutine ("AutoCompleteRoutine", destination);

		}

		/// <summary>
		/// Ferma la routine di autocompletamento
		/// </summary>
		public void StopAutoComplete()
		{
			StopCoroutine ("AutoCompleteRoutine");
		}

		private IEnumerator AutoCompleteRoutine(IDropZone destination)
		{
			// Eseguo al massimo per 5 secondi
			float delta = 0;

			for (int i = 0; delta < 10f; i++) {
				Pile pile = piles [i % GameDefinitions.NUMBER_OF_PILES];

				if (pile.LastCard != null) {
					GamePlayManager._Instance.PickCardsFromDropZone (pile.LastCard, pile);

					if (GamePlayManager._Instance.DropCardsToDropZone (destination))
						yield return new WaitForSeconds (0.05f);
				}
				yield return null;
				delta += Time.deltaTime;
			}

		}

	}
}
