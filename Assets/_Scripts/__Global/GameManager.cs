using UnityEngine;
using System.Collections;

public enum GameState { Pregame, Running, Paused, Won, Lost };
public class GameManager : Overlay {
	#region MEMBERS
	public bool isLastLevel = true;
	public int currentLevel = 1;
	public string gameName;
	
	public static GameState gameState;
	public static GameState prevGameState;
	#endregion
	
	#region UNITY_METHODS
	public virtual void Start () 
	{
		SetGameState(GameState.Pregame); //reset the game state set by previous game, TODO  why do we need static gameState?
		Time.timeScale = 1;
		
		Camera[] cams = (Camera[])FindObjectsOfType(typeof(Camera));
		foreach(Camera c in cams)
		{
			if(c.gameObject.CompareTag("SoundCam"))
			{
				Destroy (c.gameObject);
			}
		}
	}
	#endregion
	
	#region METHODS
	public void SetGameState(GameState s) 
	{
		GameManager.gameState = s;
	}
	
	public GameState GetGameState() 
	{
		return GameManager.gameState;
	}
	
	public void PauseGame() 
	{
		GameManager.prevGameState = GetGameState();
		GameManager.gameState = GameState.Paused;
		Time.timeScale = 0;
	}
	
	public void UnpauseGame() 
	{
		GameManager.gameState = GameManager.prevGameState;
		Time.timeScale = 1;
	}
	
	public void ResumeGame() 
	{
		GameManager.gameState = GameState.Running;
		Time.timeScale = 1;
	}
	
	public void RestartGame() 
	{
		//Reset global time scale
		Time.timeScale = 1;
		LoadLevel(Application.loadedLevelName);
	}
	
	public bool IsGameRunning() 
	{
		if (GetGameState() == GameState.Running) return true;
		return false;
	}
	
	public void GoToNextLevel() 
	{
		//Reset global time scale
		Time.timeScale = 1;
		if (!isLastLevel){
			int i = currentLevel + 1;
			LoadLevel(gameName + i.ToString());
		}else
			LoadLevel("WinScene");	

	}
	
	public void EndGame() 
	{
		PauseGame();
		SetGameState(GameState.Won);
		Time.timeScale = 0;
	}
	#endregion
}
