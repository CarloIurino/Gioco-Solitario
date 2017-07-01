using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
	public static  class CardExtensionMethods
	{

        public static void SetSizeOfCard(this RectTransform recttransform) {



            recttransform.sizeDelta = new Vector2( GameDefinitions.CARD_WIDTH, GameDefinitions.CARD_HEIGHT);
        }


        public static float GetHeightOfCard() {

            return GameDefinitions.CARD_HEIGHT;
        }


        public static float GetWidthOfCard() {

            return GameDefinitions.CARD_WIDTH;
        }

        public static float GetScaledHeightOfCard() {

            float factor = Screen.width / GameDefinitions.SCREEN_REFERENCE_WIDTH;

            return GameDefinitions.CARD_HEIGHT*factor;
        }


        public static float GetScaledWidthOfCard() {

            float factor = Screen.width / GameDefinitions.SCREEN_REFERENCE_WIDTH;


            return GameDefinitions.CARD_WIDTH*factor;
        }
    }
}
