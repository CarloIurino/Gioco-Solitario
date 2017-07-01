using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
	[RequireComponent ( typeof(Card) )]
	public class CardFlip : MonoBehaviour {

		[SerializeField] Image cardImage;
		[SerializeField] CanvasGroup frontFaceImagesCanvasGroup;

		private bool showingBackFace = true;
		private bool isFlipping { get { return transform.rotation != Quaternion.identity; } }

		Card card;

		void Awake()
		{
			frontFaceImagesCanvasGroup.DisableCanvasGroup ();
			card = GetComponent<Card> ();
			showingBackFace = true;
		}


		public void Flip()
		{
			if (isFlipping && card.IsHidden == showingBackFace) {
				StopAllCoroutines ();
				StartCoroutine (Rotate (0));
			}
			else if (card.IsHidden != showingBackFace){
				StopAllCoroutines ();
				StartCoroutine (FlipRoutine (card.IsHidden));
			}
			
		}

		public void ShowRightFace()
		{
			if (card.IsHidden)
				EnableBackImages ();
			else
				EnableFrontImages ();
		}

		IEnumerator FlipRoutine (bool hide)
		{
			yield return new WaitWhile (() => card.IsMoving);

			float target = 0;
			yield return  StartCoroutine( Rotate (target) );

			if (!hide)
				EnableFrontImages ();
			else
				EnableBackImages ();

			target = 1;
			yield return  StartCoroutine( Rotate (target) );
				
		}



		/// <summary>
		/// Simula la rotazione della carta ma si intervene "scala" dell'asse x
		/// </summary>
		/// <param name="target">Target.</param>
		IEnumerator Rotate( float target)
		{
			while (transform.lossyScale.x != target) {
				transform.localScale = new Vector3( Mathf.MoveTowards (transform.localScale.x, target, GameDefinitions.FLIP_VELOCITY * Time.deltaTime), 1, 1);
				yield return null;
			}
		}

		private void EnableFrontImages()
		{
			showingBackFace = false;
			cardImage.overrideSprite = CardResources.GetFrontCardSprite();
			frontFaceImagesCanvasGroup.EnableCanvasGroup ();
		}

		private void EnableBackImages()
		{
			showingBackFace = true;
			cardImage.overrideSprite = CardResources.GetBackCardSprite();
			frontFaceImagesCanvasGroup.DisableCanvasGroup ();
		}
	}
}
