using UnityEngine;

namespace Cards
{
	public class CardResources
	{
		private static Sprite [] _cardsNumbersSprites;
		private static Sprite [] _cardSuitsSprites;
		private static Sprite _frontCard;
		private static Sprite _backCard;
		private static Sprite _placeHolder;


		public CardResources()
		{
			_cardsNumbersSprites = new Sprite[GameDefinitions.DECK_SIZE/4];

			for (int i = 0; i < _cardsNumbersSprites.Length; i++) {
				_cardsNumbersSprites [i] = Resources.Load<Sprite> ("CardImages/CardNumbers/" + (i+1) + "_Sprite");

				if (_cardsNumbersSprites [i] == null)
					Debug.LogError ("Image \"CardImages/CardNumbers/" + (i+1) + "_Sprite\" not found");
			}

			_cardSuitsSprites = Resources.LoadAll<Sprite> ("CardImages/CardSuits");
			if ( _cardSuitsSprites == null || _cardSuitsSprites.Length <= 3 )
				Debug.LogError ("A card suit Image not found, be sure all card suits images are in the folder \"Resources/CardImages/CardSuits\"");

			_frontCard = Resources.Load<Sprite> ("CardImages/FrontCard_Sprite");
			_backCard = Resources.Load<Sprite> ("CardImages/BackCard_Sprite");
			_placeHolder = Resources.Load<Sprite> ("CardImages/PlaceHolder_Sprite");

			if ( _frontCard == null || _backCard == null || _placeHolder == null )
				Debug.LogError ("A card Image not found, be sure all card images are in the folder \"Resources/CardImages\"");
		}

		public static Sprite GetSpriteOfValue( int v )
		{
			return _cardsNumbersSprites [v-1];
		}


		public static Sprite GetSuitSprite( Card.Suit suit )
		{
			return _cardSuitsSprites [(int)suit];
		}

		public static Sprite GetFrontCardSprite()
		{
			return _frontCard;
		}

		public static Sprite GetBackCardSprite()
		{
			return _backCard;
		}

		public static Sprite GetPlaceHolderSprite()
		{
			return _placeHolder;
		}

	}
}
