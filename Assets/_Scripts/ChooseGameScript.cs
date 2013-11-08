﻿//#define UNITY_PRO
using UnityEngine;
using System.Collections;

public class ChooseGameScript : Overlay
{
	#region MEMBERS
	public AudioClip audioPress;
	public GameObject camPrefab;
	public Texture2D hugeBackground;
	public float scrollingSpeed;
	public string[] sceneNames;
	public MovieTexture[] buttonTextures;

	// Entire background will be shifted by this value to create an illusion of "centering" the camera
	public float centerPivotOffset; 
	// This will be define how much background is shifted from center pivot. 
	private float currentPivotOffset;
	private Rect backgroundRect;
	private Rect leftScrollRegion, rightScrollRegion;
	private Camera _localCamera;
	private AudioSource _localAudioSource;
	
	#endregion
	
	public class GameButton
	{
		public Texture texture;
		public float horizontalOffset;
		public string startSceneName;
		
		private Rect movieRect; // Used for video clip
		private Rect backgroundRect; // Used for borders
		private float floatingHeight;
		
		public GameButton (float x, float y, float width, float height, 
			Texture texture, string startSceneName, float borderWidth)
		{
			this.movieRect = new Rect (x + width / 2f, y - height / 2f, width, height);
			this.texture = texture;
			this.startSceneName = startSceneName;
			
			// Create crude black border
			backgroundRect = new Rect(movieRect);
			backgroundRect.x -= borderWidth;
			backgroundRect.y -= borderWidth;
			backgroundRect.width += borderWidth * 2f;
			backgroundRect.height += borderWidth * 2f;
		}
		
		public Rect CalcRect ()
		{
			Rect __r = new Rect (movieRect);
			__r.x += horizontalOffset;
			__r.y -= floatingHeight;
			return __r;
		}
		
		public Rect CalcBGRect () {
			Rect __r = new Rect (backgroundRect);
			__r.x += horizontalOffset;
			__r.y -= floatingHeight;
			return __r;
		}
		
		public Rect CalcStaticRect() {
			Rect __r = new Rect (backgroundRect);
			__r.x += horizontalOffset;
			return __r;
		}
		
		public void FloatUp() {
			floatingHeight = Mathf.Lerp(floatingHeight, 20f, Time.deltaTime * 10f);
		}
		
		public void FloatBack() {
			floatingHeight = Mathf.Lerp(floatingHeight, 0f, Time.deltaTime * 5f);
		}
	}
	
	private GameButton[] gameButtons;
	
	#region UNITY_METHODS
	public override void Awake ()
	{
		base.Awake ();
		Object o = FindObjectOfType (typeof(Camera));
		if (o == null) {
			GameObject c = (GameObject)Instantiate (camPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
			_localCamera = c.camera;
		} else {
			_localCamera = (Camera)o;
		}
		
		// Initialize background
		backgroundRect = new Rect (0f, 0f, hugeBackground.width, hugeBackground.height);

		// Initialize mouse scroll regions
		float __regionWidth = Screen.width / 4f;
		leftScrollRegion = new Rect (0f, 0f, __regionWidth, Screen.height);
		rightScrollRegion = new Rect (Screen.width - __regionWidth, 0f, __regionWidth, Screen.height);	
	}
	
	void Start ()
	{
		FadeIn ();
		
		/* 
		 * Positions are randomly generated and evenly spread across window.
		 * Will change later.
		 */ 
		float __buttonWidth = Screen.width / 8f;
		float __buttonHeight = Screen.height / 8f;
		float __startX = -Screen.width / 2f - __buttonWidth / 2f;
		float __periodX = __buttonWidth * 1.365f;
		Vector2[] __buttonPositions = new Vector2[10];
		for (int i = 0; i < __buttonPositions.Length; i++) {
			__buttonPositions [i].x = __startX + (i + 1) * __periodX;
			__buttonPositions [i].y = (Screen.height / 2f) + Random.Range (-1f, 1f) * Screen.height / 4f;
		}
		
		// Construct game buttons
		gameButtons = new GameButton[10];
		for (int i = 0; i < gameButtons.Length; i++) {
			gameButtons [i] = new GameButton (
				__buttonPositions [i].x, // Position x
				__buttonPositions [i].y, // Position y
				__buttonWidth, // Size width
				__buttonHeight, // Size height
				buttonTextures [i], // Movie Texture
				sceneNames [i],  // Scene name
				10f // Border width
			);			
		}

		// Sound system
		_localAudioSource = GetComponent<AudioSource> ();
		_localAudioSource.clip = audioPress;
		_localAudioSource.volume = 0.5f;
		
		// This hack kills any extra music objects
		GameObject t = GameObject.Find ("MusicMemory(Clone)");
		if (t != null)
			Destroy (t);
		
		#if UNITY_PRO
		// Set movie textures to loop and start playing them
		for (int i = 0; i < buttonTextures.Length; i++) {
			((MovieTexture)buttonTextures[i]).loop = true;
			((MovieTexture)buttonTextures[i]).Play();
		}
		#endif
	}
	
	void Update ()
	{
		// Mouse scrolling
		Vector2 __mouse = InputManager.MouseScreenToGUI ();
		if (leftScrollRegion.Contains (__mouse)) {
			float __force = 1f - __mouse.x / leftScrollRegion.width;
			currentPivotOffset += Time.deltaTime * scrollingSpeed * __force;
		}
		
		if (rightScrollRegion.Contains (__mouse)) {
			float __force = 1f - (Screen.width-__mouse.x) / rightScrollRegion.width;
			currentPivotOffset -= Time.deltaTime * scrollingSpeed * __force;
			Debug.Log(__force);
		}
		
		currentPivotOffset = Mathf.Clamp (currentPivotOffset, 
			-Mathf.Abs(centerPivotOffset), Mathf.Abs(centerPivotOffset));
		
		for (int i = 0; i < gameButtons.Length; i++) {
			gameButtons [i].horizontalOffset = currentPivotOffset;
			// If hovering, float button
			if (gameButtons [i].CalcStaticRect().Contains(__mouse)) {
				gameButtons[i].FloatUp();
			} else {
				gameButtons[i].FloatBack();
			}
		}
		
		// Mouse input
		if (Input.GetMouseButtonDown (0)) {
			_HandleMouseClick ();
		}
	}
	
	// Update is called once per frame
	void OnGUI ()
	{
		// Draw background
		backgroundRect = new Rect (centerPivotOffset + currentPivotOffset, 0f, hugeBackground.width, hugeBackground.height);
		GUI.DrawTexture (backgroundRect, hugeBackground);
		
		#if UNITY_EDITOR
		// Draw mouse scroll regions
		GUI.Box(leftScrollRegion, "left");
		GUI.Box(rightScrollRegion, "right");
		#endif

		// Draw buttons
		for (int i = 0; i < gameButtons.Length; i++) {
			GUI.Box(gameButtons[i].CalcBGRect(), "");
			
			#if UNITY_PRO
			GUI.DrawTexture(gameButtons[i].CalcRect(), gameButtons[i].texture);
			#else
			GUI.Box (gameButtons [i].CalcRect (), gameButtons [i].startSceneName);
			#endif
		}
	}
	#endregion
	
	#region METHODS
	IEnumerator _FadeOutAndLoad (string scene)
	{
		AudioSource source = _localCamera.audio;
		LoadLevel (scene);
		while (source.volume > 0.2f || _localAudioSource.isPlaying) {
			source.volume -= Time.deltaTime * 0.2f;
			yield return null;
		}
	}
	
	private void _HandleMouseClick ()
	{
		Vector2 __mousePos = InputManager.MouseScreenToGUI ();
		for (int i = 0; i < gameButtons.Length; i++) {
			if (gameButtons [i].CalcStaticRect ().Contains (__mousePos) &&
				gameButtons [i].startSceneName != "") {
				_localAudioSource.Play ();
				StartCoroutine (_FadeOutAndLoad (gameButtons [i].startSceneName));
			}
		}
	}
	#endregion
}
