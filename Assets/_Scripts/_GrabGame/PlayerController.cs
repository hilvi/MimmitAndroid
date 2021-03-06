﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	
	public float speed = 5;
	private Animator2D _anim;
	Vector3 _pos;
	float __screenPos;
	
	// Use this for initialization
	void Start ()
	{
		_anim = GetComponent<Animator2D> ();
		_anim.PlayAnimation ("left");
		_pos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_pos.x = Camera.main.ScreenToWorldPoint (Input.mousePosition).x;

		transform.position = Vector2.Lerp (transform.position, _pos, Time.deltaTime * speed);

		float distance = transform.position.x - _pos.x;
		if(Mathf.Abs(distance) < 2f) {
			if(Manager.GetCharacter() == Character.Fox)
				_anim.StopAnimation();
			else
				_anim.PlayAnimation("idle");
		}
		else if (distance > 0)
			_anim.PlayAnimation ("left");
		else
			_anim.PlayAnimation ("right");
	}
}
