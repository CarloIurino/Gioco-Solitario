using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Regola automaticamente la risoluzione di riferimento del canvas scaler secondo la classe gameDefinitions
/// </summary>

[ExecuteInEditMode]
class CanvasSettings : MonoBehaviour {
	#if UNITY_EDITOR
	CanvasScaler canvasScaler;
	Vector2 reference;

	void OnEnable()
	{
		reference = new Vector2 (GameDefinitions.SCREEN_REFERENCE_WIDTH, GameDefinitions.SCREEN_REFERENCE_HEIGHT);
		canvasScaler = GetComponent<CanvasScaler> ();

		canvasScaler.referenceResolution = reference;
	}

	void Update()
	{
		if (canvasScaler.referenceResolution != reference){
			canvasScaler.referenceResolution = reference;
			Debug.LogWarning ("Reference resolution is driven by GameDefinitions values.");
		}
	}
	#endif
}
