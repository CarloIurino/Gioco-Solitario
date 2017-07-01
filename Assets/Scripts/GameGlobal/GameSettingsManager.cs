using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSettingsManager {

	public Action OnSettingsChanged;

	public enum Background { Green, Cyan, Yellow, LightYellow}

	public GameSettings GameSettings { get { return _gameSettings; } }
	public bool Draw3Mode { get { return _gameSettings.Draw3Mode; } }
	public bool EnableTimer { get { return _gameSettings.EnableTimer; } }
	public bool EnableHelps { get { return _gameSettings.EnableHelps; } }
	public bool EnableSounds { get { return _gameSettings.EnableSounds; } }
	public Background BackgroundColor { get { return _gameSettings.BackgroundColor; } }

	static private GameSettings _gameSettings = new GameSettings();

	public Color[] Colors;

	public GameSettingsManager()
	{
		Input.multiTouchEnabled = false;

		LoadSettings();
	}

	private void LoadSettings()
	{
		if (PlayerPrefs.HasKey ("Draw3Mode")) {
			_gameSettings.Draw3Mode = true;
		}

		if (PlayerPrefs.HasKey ("EnableTimer")) {
			_gameSettings.EnableTimer = true;
		}

		if (PlayerPrefs.HasKey ("EnableSounds")) {
			_gameSettings.EnableSounds = true;
		}

		if (PlayerPrefs.HasKey ("EnableHelps")) {
			_gameSettings.EnableHelps = true;
		}

		if (PlayerPrefs.HasKey ("BackgoundColor")) {
			_gameSettings.BackgroundColor = (Background)PlayerPrefs.GetInt ("BackgoundColor");
		}
		else
			_gameSettings.BackgroundColor = Background.Green;
	}


	#region Setters
	public void SetDraw3Mode(bool enable)
	{
		_gameSettings.Draw3Mode = enable;
		if (enable)
			PlayerPrefs.SetInt ("Draw3Mode", 1);
		else
			PlayerPrefs.DeleteKey ("Draw3Mode");

		AlertOnSettingsChanged ();
	}


	public void SetEnableTimer(bool enable)
	{
		_gameSettings.EnableTimer = enable;
		if (enable)
			PlayerPrefs.SetInt ("EnableTimer", 1);
		else
			PlayerPrefs.DeleteKey ("EnableTimer");

		AlertOnSettingsChanged ();
	}



	public void SetEnableHelps(bool enable)
	{
		_gameSettings.EnableHelps = enable;
		if (enable)
			PlayerPrefs.SetInt ("EnableHelps", 1);
		else
			PlayerPrefs.DeleteKey ("EnableHelps");

		AlertOnSettingsChanged ();
	}



	public void SetEnableSound(bool enable)
	{
		_gameSettings.EnableSounds = enable;
		if (enable)
			PlayerPrefs.SetInt ("EnableSounds", 1);
		else
			PlayerPrefs.DeleteKey ("EnableSounds");

		AlertOnSettingsChanged ();
	}



	public void SetBackgroundColor (Background color)
	{
		_gameSettings.BackgroundColor = color;
		PlayerPrefs.SetInt ("BackgoundColor", (int)BackgroundColor);

		AlertOnSettingsChanged ();
	}

	#endregion



	private void AlertOnSettingsChanged()
	{
		if (OnSettingsChanged != null)
			OnSettingsChanged ();
	}

}

public struct GameSettings
{
	public bool Draw3Mode;
	public bool EnableTimer;
	public bool EnableHelps;
	public bool EnableSounds;
	public GameSettingsManager.Background BackgroundColor;


	/// <summary>
	/// Confronta solo i settaggi che riguardano il gameplay: Draw3, Timer, Helps
	/// </summary>
	/// <returns><c>true</c>, se i settaggi sono uguali, <c>false</c> altrimenti.</returns>
	/// <param name="settings1">Settings1.</param>
	/// <param name="settings2">Settings2.</param>
	public static bool CompareGamePlaySettings (GameSettings settings1, GameSettings settings2)
	{
		if (settings1.Draw3Mode == settings2.Draw3Mode &&
			settings1.EnableTimer == settings2.EnableTimer &&
			settings1.EnableHelps == settings2.EnableHelps) {

			return true;
		} 
		else
			return false;
	}
}
