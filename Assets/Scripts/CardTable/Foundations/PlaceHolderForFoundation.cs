using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using UnityEngine.UI;

namespace CardTable
{
	public class PlaceHolderForFoundation : PlaceHolder {
		[SerializeField] private Image suitImage;

		override protected void Awake()
		{

		}

		// Called in Awake by SortedOfAKind class
		public void Initialize(Card.Suit suit)
		{
			InitializeOnAwake (suit);

			suitImage.overrideSprite = CardResources.GetSuitSprite (typeOfSuit);
		}

		public void EnableCard()
		{
			canvasGroup.EnableCanvasGroup ();
		}

		public void DisableCard()
		{
			canvasGroup.DisableCanvasGroup ();
		}
	}
}
