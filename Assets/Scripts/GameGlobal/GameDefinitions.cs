using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public  class GameDefinitions 
{
	public const int DECK_SIZE = 52;  // Should be a multiple of 4
	public const int  NUMBER_OF_PILES = 7;

	public const float CARD_ASPECT_RATIO = 1.5f;

    public const int CARD_WIDTH = 75;
    public const int CARD_HEIGHT = 110;


	public const float SCREEN_REFERENCE_WIDTH = 1600;
	public const float SCREEN_REFERENCE_HEIGHT = 900;

	public  const float TIME_TO_TRASLATE = 0.2f;
	public  const float TIME_TO_TRASLATE_TO_PILES = 0.1f;
	public  const float FLIP_VELOCITY = 12f;

	public  const float RELATIVE_SPACE_FACING_DOWN_CARDS = 0.18f;
	public const float RELATIVE_SPACE_FACING_UP_CARDS = 0.4f;
	public const float RELATIVE_SPACE_DRAGGING_CARDS = 0.6f;

	public const float DEAL_CARDS_SPEED = 0.07f;

	#region Point Rules
	public const int POINTS_CARD_TO_FOUNDATION = 10;
	public const int POINTS_CARD_FROM_FOUNDATION = -15;
	public const int POINTS_TURN_CARD = 5;
	public const int POINTS_CARD_FROM_STOCK = 5;
	public const int REDEAL = -100;
	#endregion


	#if UNITY_EDITOR
	void Update()
	{
		if (DECK_SIZE % 4 != 0)
			Debug.LogError ("NUMBER_OF_CARDS shuold be a multiple of 4");
		if ( DECK_SIZE > 52 )
			Debug.LogError ("NUMBER_OF_CARDS: use max 52 cards");
	}
	#endif
}
