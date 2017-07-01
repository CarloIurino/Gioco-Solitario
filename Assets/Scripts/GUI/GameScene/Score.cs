using UnityEngine;
using UnityEngine.UI;
using CardTable;

namespace GUI
{
	public class Score : MonoBehaviour {
		[SerializeField] private Text points;
		[SerializeField] private Text moves;

		private void Awake()
		{
			GameManager._Instance.OnPointsUpdate += UpdatePointsText;
			GameManager._Instance.OnMovesUpdate += UpdateMovesText;

			ResetScore ();
		}


		private void UpdatePointsText(int points)
		{
			this.points.text = points.ToString();
		}

		void UpdateMovesText(int moves)
		{
			this.moves.text = moves.ToString ();;
		}

		private void ResetScore()
		{
			points.text = 0.ToString ();;
			moves.text = 0.ToString ();;
		}


		private void OnDestroy()
		{
			GameManager._Instance.OnPointsUpdate -= UpdatePointsText;
			GameManager._Instance.OnMovesUpdate -= UpdateMovesText;
		}
	}
}
