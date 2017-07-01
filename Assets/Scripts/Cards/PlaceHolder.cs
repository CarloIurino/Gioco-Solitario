using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;

namespace CardTable
{
	[RequireComponent (typeof(CanvasGroup))]
	public class PlaceHolder : MonoBehaviour, ICard {
		[SerializeField] private Sprite emptyImage;
		[SerializeField] private Sprite backFaceImage;

		private Image placeHolderimage;

		protected Card.Suit typeOfSuit;	
		protected int value = 0;

		public Card.Suit TypeOfSuit{ get { return typeOfSuit; } }
		public int Value { get { return value; }}

		protected CanvasGroup canvasGroup;

		virtual protected  void Awake()
		{
			InitializeOnAwake (Card.Suit.Hearts);
		}

		// Called in Awake by SortedOfAKind class that override Awake
		protected void InitializeOnAwake(Card.Suit suit)
		{
			placeHolderimage = GetComponent<Image> ();

			typeOfSuit = suit;

			RectTransform rectTransform = GetComponent<RectTransform> ();
			rectTransform.SetSizeOfCard ();

			canvasGroup = GetComponent<CanvasGroup> ();
		}

		public void EnableCard()
		{
			canvasGroup.EnableCanvasGroup ();
		}

		public void DisableCard()
		{
			canvasGroup.DisableCanvasGroup ();
		}

		public void SetEmptyImage()
		{
			placeHolderimage.overrideSprite = emptyImage;
		}

		public void SetBackFaceImage()
		{
			placeHolderimage.overrideSprite = backFaceImage;
		}
	}
}