using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using GUI;
using System;

namespace CardTable
{

	/// <summary>
	/// Questa classa si occupa della gestione del gioco del solitario, del gameplay in particolare.
	/// Tutte le operazioni sono centralizzate, ogni movimento delle carte è gestito dal GamePlayManager.
	/// Il game manager si occupa anche di memorizzare le mosse e eventualmente ripristinarle.
	/// 
	/// Il tavolo da gioco si può suddividere in 4 elementi principali:
	/// Dealer: Il mazzo di carte da cui pescare le carte
	/// Waste: Lo scarto dove vengono raccolte le carte pescate dal mazzo che non è possibile giocare
	/// Tableau di Pile: la parte principale del gioco suddivisa in pile
	/// Foundations: Sono le carte da ordinare con lo stesso seme per completare il gioco
	/// 
	/// Questi quattro elementi possono comunicare tra di loro escluisivamente tramite il GamePlaymanager.
	/// 
	/// Il GamePlaymanager si occupa fondamentalmente di 4 operazioni:
	/// -PickCards - Prelevare le carte da una zona di gioco
	/// -IsValiPlay - Verificare che le carte prelevate siano compatibili con la zona di gioco di destinazione
	/// -AddCards - Aggiunfere le carte prelevate in un 'altra zona di gioco
	/// -OnPickComplete - Eseguire delle operazioni (implementate indipendentemente dalla zona di gioco) quando le carte sono state trasferite
	/// 
	/// Queste quattro operazioni sono garantite mediante l'implementazione dell'interfaccia IDropZone d parte di tutte le zone di gioco.
	/// 
	/// Il GamePlaymanager si occupa di fare le chiamate di questi metodi, le zone di gioco si comportano di conseguenza 
	/// grazie alla propria implementazione dell'interfaccia
	/// 
	/// </summary>
	public partial class GamePlayManager : MonoBehaviour {
		[SerializeField] private Foundations foundations;
		[SerializeField] private Dealer dealer;
		[SerializeField] private TableauController tableauController;
		[SerializeField] private Waste waste;
		[SerializeField] private Transform draggedCardsContainer; // Un contenitore temporaneo per le carte spostate. Serve a mettere in primo piano le carte
		[SerializeField] private Timer timer;

		private static GamePlayManager _instance;
		public static GamePlayManager _Instance { get { return _instance; } }

		public Action OnGameComlete;
		public Action OnPause;
		public Action OnReset;
		public Action OnResume;

		public enum DropZoneType { Dealer, Waste, Pile, Foundation }  // Per identificare le varie DropZone che implementano IDropZone

		public GameSettings CurrentGameSettings { get{ return currentGameSettings ;}}

		private  int points { get { return  GameManager._Instance.Points; } set{ GameManager._Instance.Points = value; }}
		private  int moves { get { return GameManager._Instance.Moves; }  set{GameManager._Instance.Moves = value; } }
		private int turnedCards = 0;

		private Pile[] piles;  // Le pile del Tableau dove vengono incolonnate le carte
		private APlay currentPlay = null;  // Struttura che memorizza i dettagli della mossa

		private Deck deck; // Il mazzo di carte

		private Card[] draggedCards; // Le carte che sono in movimento, che sono trascinate

		private OptionMenu optionMenu; // Menu opzioni. Riferimento per registrare l'evento di chiusura del menu e per visualizzare il messaggio di riavvio del gioco
		private GameSettings currentGameSettings; 
		private GameSettings  globalGameSettings { get { return GameManager._Instance.GameSettings; }}


		private bool gameComplete = false;

		void Awake()
		{
            if (GamePlayManager._Instance == null) {
                Debug.LogError("La scena iniziale è Start Menu. Aprire la scena Start Menu e premere Play");
                Application.Quit();
            }

			_instance = this;

			optionMenu = InterfaceManager._Instance.GetOptionMenu ();
			optionMenu.OnOptionMenuClosed += OnOptionMenuClose;
			InterfaceManager._Instance.OnMenuShow += PauseGame ;
			InterfaceManager._Instance.OnMenuHide += Resume;

			// Registro all'evento Settaggi cambiati
			GameManager._Instance.GameSettingsManager.OnSettingsChanged += OnGlobalSettingsChanged;

			currentGameSettings = GameManager._Instance.GameSettings;
		}




		IEnumerator Start()
		{
			// Attendo che le pile vengano generate dinamicamente e siano pronte
			yield return new WaitUntil (() => tableauController.ArePilesReady);
			piles = tableauController.GetComponentsInChildren<Pile> (); 

			deck = GetComponent<Deck>();

			NewGame();
		}


		#region Game Management
		/// <summary>
		/// Inizia una nuova partita
		/// </summary>
		public void NewGame()
		{
			
			// Passo un nuovo mazzo di carte mischiato 
			dealer.SetNewDeck ( deck.GetNewDeck() );  

			StartGame ();
		}


		/// <summary>
		/// Inizia una nuova partita senza mischiare le carte
		/// </summary>
		public void TryAgain()
		{
			// Passo un nuovo mazzo di carte mischiato 
			dealer.SetNewDeck ( deck.GetSameDeck() );  

			StartGame ();
		}

		private void StartGame()
		{
			ResetGame ();

			if (currentGameSettings.EnableTimer)
				StartCoroutine (RemovePointsForTime ());
			else
				StopCoroutine (RemovePointsForTime ());

			// Distribuisco le carte sul tableau
			StartCoroutine(DealCards ());
		}


		private void ResetGame()
		{
			StopAllCoroutines ();

			currentGameSettings = globalGameSettings;

			draggedCards = null;
			currentPlay = null;

			gameComplete = false;
			turnedCards = 0;

			points = 0;
			moves = 0;

            // Cancella le mosse memorizzate
            plays.Clear();


			if (OnReset != null)
				OnReset ();

			deck.ResetCards ();
		}


		public void Resume()
		{
			dealer.EnableDealer (true);


			if (OnResume != null)
				OnResume ();
		}
			


		public  void PauseGame()
		{

			dealer.EnableDealer (false);

			if (OnPause != null)
				OnPause ();
		}




		#endregion


		/// <summary>
		/// Distribuisce le carte sul tableau
		/// </summary>
		private IEnumerator DealCards ()
		{
			dealer.SetDealingMode (true, draggedCardsContainer);
			foreach (Pile p in piles) {
				p.EnablePoseCardAnimation (false);
			}

			int cardsToCall = 1;

			for (int i = 0; i < GameDefinitions.NUMBER_OF_PILES; i++) {
				for (int j = 1; j <= cardsToCall; j++) {
					Card card = dealer.DealACard ();
					card.SetCardDragging ( draggedCardsContainer);
					piles [i].AddCard (new Card[]{card});

					yield return new WaitForSeconds (GameDefinitions.DEAL_CARDS_SPEED);
				}
				piles [i].ShowLastCard (); // Scopre l'ultima carta della pila

				cardsToCall++;
			}

			yield return new WaitForSeconds (0.05f);
			dealer.SetDealingMode (false, null);
			dealer.EnableDealer (true);

			foreach (Pile p in piles) {
				p.EnablePoseCardAnimation (true);
			}

		}



		/// <summary>
		/// Recupera le carte di scarto
		/// </summary>
		public void ReDeal()
		{
			RegisterPlay (true, null, null, false, 0);
			dealer.ReDeal ();

			if ( !currentGameSettings.Draw3Mode )
				points += GameDefinitions.REDEAL;
		}
			



		#region THE CORE
		/// <summary>
		/// Seleziona le carte e le prepara al trascinamento, dalla zona di gioco passata come parametro che implementa l'interfaccia IDropZone.
		/// Verranno prese una o più carte oltre a quella passata come parametro a seconda dell'implementazione della IDropZone
		/// </summary>
		/// <param name="selectedCard">La carta selezionata dal puntatore o in automatioco dalla IDropZone</param>
		/// <param name="dropZone">La zona di gioco da cui prendere le carte</param>
		public void PickCardsFromDropZone( Card selectedCard, IDropZone dropZone )
		{
			draggedCards = dropZone.PickCards (selectedCard);

			if (draggedCards != null) {
				foreach (Card card in draggedCards) {
					card.SetCardDragging (draggedCardsContainer); // Sposta le carte nel contenitore di trascinamento e assegna alle carte la zona IDropZone di provenienza

					card.SetCurrentDropZone (null);
				}
				// Registra la giocata in corso
				currentPlay = new APlay  (false, draggedCards, dropZone, dropZone.LastCard != null ? dropZone.LastCard.IsHidden : false, 0);
			}
		}

	
			

		/// <summary>
		/// Sposta le carte selezionate "draggedCards" da una zona di gioco a un'altra solo se la giocata è valida
		/// </summary>
		/// <param name="fromDropZone"> Zona di gioco sorgente </param>
		/// <param name="toDropZone">Zona di gioco destinazione </param>
		public bool DropCardsToDropZone(IDropZone toDropZone)
		{
			bool itWasAGoodPlay = false;

			IDropZone fromGameZone = draggedCards [0].GetCurrentDropZone ();

			// Controllo che la giocata sia valida
			if (currentPlay != null && toDropZone.IsValidPlay (draggedCards, currentPlay.LastCardPosition)) 
			{

				foreach (Card card in draggedCards) {
					card.AssignedToZone = true; // Le carte sono state assegnate a una zona di gioco, ma fisicamente devono ancora spostarsi mediante l'animazione
				}

				toDropZone.AddCard (draggedCards);  // Le carte vengono passate alla IDropZone che si occuprà del posizionamento mediante animazione

				currentPlay.Points = PointsForPlay (currentPlay.LastCardPosition, toDropZone);
				points += currentPlay.Points;
				moves++;


				// Eseguire dopo l'assegnazione dei punti
				// Operazioni da svolgere dopo aver prelevato una carta da una zona di gioco
				// Nel caso venga scoperta una carta nel tableau, viene restituito True
				bool newCardTurned = currentPlay.LastCardPosition.OnPickCardsCompleted ();  // La zona da cui è stata presa la carta, ad operazione ultimata, esegue delle operazioni che cambiano in base all'implementazione (IDropZone). Es. la pila scopre la carta
				
				if (newCardTurned)
					turnedCards++;
				
				RegisterPlay (currentPlay);

				CheckIfAutoComplete ();

				itWasAGoodPlay = true;
			} 
			else if ( currentPlay != null ){
				RestoreCurrentPlay ();  // Ripristino quando la mossa non è valida

				itWasAGoodPlay = false;
			}

			draggedCards = null;
			currentPlay = null;

			return itWasAGoodPlay;
		}



		/// <summary>
		/// Restituisce il punteggio della giocata
		/// </summary>
		/// <returns>The for play.</returns>
		/// <param name="fromDropZone">From drop zone.</param>
		/// <param name="toDropZone">To drop zone.</param>
		private int PointsForPlay(IDropZone fromDropZone, IDropZone toDropZone)
		{
			if (toDropZone.DropZoneType == DropZoneType.Foundation) 
				return GameDefinitions.POINTS_CARD_TO_FOUNDATION;


			if (fromDropZone.DropZoneType == DropZoneType.Foundation && toDropZone.DropZoneType == DropZoneType.Pile) {
				return GameDefinitions.POINTS_CARD_FROM_FOUNDATION;
			}
			else if (fromDropZone.DropZoneType == DropZoneType.Waste && toDropZone.DropZoneType == DropZoneType.Pile) {
				return GameDefinitions.POINTS_CARD_FROM_STOCK;
			}
				
			if (fromDropZone.DropZoneType == DropZoneType.Pile) {
				if ( fromDropZone.LastCard != null && fromDropZone.LastCard.IsHidden )  // Eseguire prima di chiamare IDragZone.ShowLastCard
					return GameDefinitions.POINTS_TURN_CARD;
			}

			return 0;
		
		}


		/// <summary>
		/// Controlla se tutte le carte sono in gioco e scoperte per completare automaticamente il gioco
		/// </summary>
		private void CheckIfAutoComplete()
		{
			if (gameComplete)
				return;

			if (turnedCards == tableauController.CardsToTurn && dealer.IsEmpty && waste.Size == 0) {
				gameComplete = true;
				StartCoroutine (AutoCompleteGame());

			}
		}

		private IEnumerator AutoCompleteGame()
		{
			yield return new WaitForSeconds (0.5f);
			tableauController.Autocomplete (foundations);
			yield return new WaitUntil ( () => foundations.IsFull);
			tableauController.StopAutoComplete ();
		}

		#endregion





		private bool ChekIfCurrentSettingsMatchGlobaSettings()
		{
			if (GameSettings.CompareGamePlaySettings (globalGameSettings, currentGameSettings)) {
				optionMenu.HideResetMessage ();
				return true;
			} else {
				optionMenu.ShowResetMessage ();
				return false;
			}
		}


		private IEnumerator RemovePointsForTime()
		{
			float delta = 0;

			while (true) {
				if (delta > 10) {
					points -= 2;
					delta = 0;
				}
				delta += Time.deltaTime;
				yield return null;
			}
		}



		#region Callbacks

		private void OnOptionMenuClose()
		{
			if (!ChekIfCurrentSettingsMatchGlobaSettings()) {
				TryAgain ();
			}
		}


		private void OnOptionMenuOpened()
		{
			ChekIfCurrentSettingsMatchGlobaSettings ();
		}

		private void OnGlobalSettingsChanged()
		{
			ChekIfCurrentSettingsMatchGlobaSettings ();
		}
	
		#endregion
			

		void OnDestroy()
		{
			InterfaceManager._Instance.OnMenuShow -= PauseGame ;
			InterfaceManager._Instance.OnMenuHide -= Resume;
			GameManager._Instance.GameSettingsManager.OnSettingsChanged -= OnGlobalSettingsChanged;
            optionMenu.OnOptionMenuClosed -= OnOptionMenuClose;
        }
    }


}
