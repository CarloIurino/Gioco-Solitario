using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
	[RequireComponent (typeof(Card))]
	public class FollowPrevCard : MonoBehaviour {
		Card card;
		Transform target;

		void Awake()
		{
			enabled = false;
			card = GetComponent<Card> ();
		}

		public void FollowCard(Transform prevCard)
		{
			target = prevCard;
			enabled = true;
		}

		public void StopFollow()
		{
			enabled = false;
			target = null;
		}

		void Update()
		{
			if (target != null)
				transform.position = target.position + Vector3.down * CardExtensionMethods.GetScaledHeightOfCard()*GameDefinitions.RELATIVE_SPACE_DRAGGING_CARDS;
			else
				enabled = false;
		}
	}
}
