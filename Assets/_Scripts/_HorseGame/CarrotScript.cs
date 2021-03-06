﻿using UnityEngine;
using System.Collections;

public class CarrotScript : MonoBehaviour
{
	#region MEMBERS
	public HorseCharacterController _controller;
	private Renderer _renderer;
	private Transform _particle;
	private string player = "Player";
	public float powerUpSpeed = 2f;
	#endregion
	
	#region UNITY_METHODS
	void Start ()
	{
		_controller = GameObject.FindGameObjectWithTag ("Player").GetComponent<HorseCharacterController> ();
		_renderer = GetComponentInChildren<Renderer> ();
		_particle = transform.Find ("Particle");
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == player) 
		{
			StartCoroutine (DoubleSpeed());
			_renderer.enabled = false;
			_particle.gameObject.SetActive (false);
			_controller.particle.gameObject.SetActive (true);
		}
	}
	#endregion
	
	#region METHODS
	public IEnumerator DoubleSpeed ()
	{
		_controller.SetSpeed (_controller.GetSpeed () + powerUpSpeed);
		while (_controller.GetSpeed() > _controller.runningSpeed) 
		{
			_controller.SetSpeed (_controller.GetSpeed () - Time.deltaTime);
			yield return null;
		}
		_controller.SetSpeed (_controller.runningSpeed);
		_controller.particle.gameObject.SetActive (false);
	}
	#endregion
}
