using UnityEngine;
using Cards;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	#region Singleton Pattern

	// Anche se il gioco in questione è composto da una sola scena, preferisco implementare le classi più
	// importanti come Singleton che persistono per tutta l'esecuzione del gioco.
	// Questo per eventuali espansioni future del gioco

	private static GameManager _instance;
	public static GameManager _Instance { get { return _instance; } }

	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
		} else {
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (gameObject);

		InitOnAwake ();
	}
	#endregion



	#region Fields, Initialization
	public Action<int> OnPointsUpdate;
	public Action<int> OnMovesUpdate;


	public int Points{ 
		get 
		{ 
			return points; 
		} 
		set 
		{
			points = Mathf.Clamp (value, 0, int.MaxValue);

			if (OnPointsUpdate != null)
				OnPointsUpdate (points);
		}
	}
	public int Moves{ 
		get 
		{ 
			return moves; 
		} 
		set
		{
			moves =  value;

			if (OnMovesUpdate != null)
				OnMovesUpdate (moves);
		}
	}

	public GameSettingsManager GameSettingsManager{ get { return gameSettingsManager; } }
	public GameSettings GameSettings{ get { return gameSettingsManager.GameSettings; } }

	private GameSettingsManager gameSettingsManager;

	private int points;
	private int moves;

	private void InitOnAwake()
	{
		gameSettingsManager = new GameSettingsManager ();

		points = 0;
		moves = 0;

		// Non è stato implementato un metodo per gestire e salvare i punteggi
	}

    #endregion

    /// <summary>
    /// Inizia la partita
    /// </summary>
    public void StartGame()
	{
		SceneManager.LoadScene (1);
	}


    /// <summary>
    /// Ritorna al menu iniziale
    /// </summary>
	public void StartMenu()
	{
		SceneManager.LoadScene (0);
	}


    /// <summary>
    /// Chiude l'applicazione
    /// </summary>
	public void CloseApplication()
	{
		// Fai qualcosa prima di chiudere

		Application.Quit ();
	}

		
    /// <summary>
    /// Ripristina i punteggi
    /// </summary>
	public void ResetScore()
	{
		points = 0;
		moves = 0;
	}


}
