using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;
using CardTable;

public static class ExtensionMethods {
	
	public static void EnableCanvasGroup(this CanvasGroup canvasGroup)
	{
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	public static void DisableCanvasGroup(this CanvasGroup canvasGroup)
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	public static void DoInteractable ( this CanvasGroup canvasGroup )
	{
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public static void DoNotInteractable ( this CanvasGroup canvasGroup )
	{
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}
		


//	public static IEnumerator TranslateTO (this Transform transform, Vector3 target, float time)
//	{
//		float delta = 0;
//
//		Vector3 source = transform.position;
//
//		while (delta < time) {
//			transform.position = Vector3.Lerp (source, target, delta / time);
//			yield return null;
//			delta += Time.deltaTime;
//		}
//
//		transform.position = target;
//	}

	public static void SetCardDragging(this Card card, Transform container)
	{
		if (card != null) {
			card.transform.SetParent (container);
			card.transform.SetAsLastSibling ();
			card.IsMoving = true;
			card.AssignedToZone = false;
		}
	}

	public static void SetCurrentDropZone( this Card[] cards, IDropZone dropZone)
	{
		foreach (Card c in cards)
			c.SetCurrentDropZone (dropZone);
	}

}
