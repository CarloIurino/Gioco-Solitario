using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using CardTable;

namespace Cards
{
	[RequireComponent (typeof(Card))]
	public class Draggable : MonoBehaviour, IPointerDownHandler , IPointerUpHandler, IDragHandler, IEndDragHandler
	{

		Transform lastParent = null;
		Card card;

		bool wasDragging;

		private Vector2 offset = new Vector2(0, CardExtensionMethods.GetScaledHeightOfCard()*0.3f);

		void Awake()
		{

			card = GetComponent<Card> ();
			lastParent = transform.parent;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			card.SetInteractable (false);
			transform.position = eventData.position - offset;
			GamePlayManager._Instance.PickCardsFromDropZone( card, card.GetCurrentDropZone() );
		}

		public void OnDrag(PointerEventData eventData)
		{
			wasDragging = true;
			transform.position = eventData.position - offset;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			wasDragging = false;

			if (!card.AssignedToZone) {
				card.AssignedToZone = true;
				GamePlayManager._Instance.RestoreCurrentPlay ();
			}

			StartCoroutine (WaitCardEndMovingToReactivate ());
		}

		private IEnumerator WaitCardEndMovingToReactivate()
		{
			

			yield return new WaitWhile (() => card.IsMoving);
			card.SetInteractable (true);

		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!wasDragging) {
				card.SetInteractable (true);
				if (!card.AssignedToZone) {
					card.AssignedToZone = true;
					GamePlayManager._Instance.RestoreCurrentPlay ();
				}
			}
		}




	}
}
