using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CardTable;

namespace Cards
{
	[RequireComponent(typeof(CanvasGroup))]
	public class Card:  MonoBehaviour, ICard {
		[SerializeField] private CanvasGroup cardCanvasGroup;
		[SerializeField] private Image valueImage;
		[SerializeField] private Image smallSuitImage;
		[SerializeField] private Image largeSuitImage;

		public enum Suit { Hearts, Diamonds, Clubs, Spades}

		public int ID { get { return id; } }
		public Card.Suit TypeOfSuit{ get { return typeOfSuit; } }
		public int Value { get { return value; } }


		public bool AssignedToZone{ get; set; }
		public bool IsMoving{ get; set; }
		public bool IsRed { get { return typeOfSuit == Suit.Hearts || typeOfSuit == Suit.Diamonds; } }
		public bool IsHidden { get { return isHidden; } }

		private int id;
		private Card.Suit typeOfSuit;
		private int value;

		private bool isHidden = true;

		private IDropZone dropZone;

		private FollowPrevCard follower;
		private CardFlip cardFlip;
		private Draggable draggable;
	

		// Called in Awake by Deck
		public void InitializeCard( int id, Card.Suit suit, int value )
		{
			RectTransform rectTransform = GetComponent<RectTransform> ();
            rectTransform.SetSizeOfCard();

            draggable = GetComponent<Draggable> ();
			follower = GetComponent<FollowPrevCard> ();
			cardFlip = GetComponent<CardFlip> ();

			this.id = id;
			this.typeOfSuit = suit;
			this.value = value;

			IsMoving = false;

			LoadImages ();

			Reset ();
		}
			
		public void LoadImages()
		{
			valueImage.overrideSprite = CardResources.GetSpriteOfValue (value);
			smallSuitImage.overrideSprite = CardResources.GetSuitSprite (typeOfSuit);
			largeSuitImage.overrideSprite = smallSuitImage.overrideSprite;

			if (!IsRed)
				valueImage.color = Color.black;
		}


		public void SetCurrentDropZone( IDropZone dropZone )
		{
			this.dropZone = dropZone;
		}

		public IDropZone GetCurrentDropZone ()
		{
			return dropZone;
		}
			
		public void EnableCard()
		{
			cardCanvasGroup.EnableCanvasGroup ();

			cardFlip.ShowRightFace (); // Mette scopre o copre la carta in base alla variabile isHidden. 

			if (IsHidden) 
				cardCanvasGroup.DoNotInteractable ();
			else
				cardCanvasGroup.DoInteractable ();
		}

		public void DisableCard()
		{
			cardCanvasGroup.DisableCanvasGroup ();
		}

		public void SetInteractable ( bool interactable )
		{
			if (interactable) {
				cardCanvasGroup.DoInteractable ();
			}
			else {
				cardCanvasGroup.DoNotInteractable ();
			}
		}


		public void ShowCard()
		{
			isHidden = false;
			cardFlip.Flip ();

		}

		public void HideCard()
		{
			isHidden = true;
			cardFlip.Flip ();
		}


		public IEnumerator TranslateCardToPosition (Vector3 target, float time)
		{
			float delta = 0;

            Vector3 source = transform.position;

			while (delta < time) {
				transform.position = Vector3.Lerp (source, target, delta / time);

                yield return new WaitForEndOfFrame();
				delta += Time.deltaTime;
			}

            transform.position = target;
		}
			

		public void FollowCard(Card cardToFollow)
		{
			follower.FollowCard (cardToFollow.transform);
		}

		public void StopFollowCard()
		{
			follower.StopFollow ();
		}

		public void Reset()
		{
			StopAllCoroutines ();

			AssignedToZone = false;
			IsMoving = false;

			dropZone = null;

			StopFollowCard ();
			HideCard ();
			DisableCard ();
		}

		public string ToString()
		{			
			return "Card is " + value + " of " + TypeOfSuit;
		}
	}
}
