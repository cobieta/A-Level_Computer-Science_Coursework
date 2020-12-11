using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFader : MonoBehaviour {

	private Animator anim;

	//Initializes the animation component of the fader game object.
	void Start () {
		anim = GetComponent<Animator>();
	}

	// Calls the animator to trigger the fade in sequence of animations.
	public void FadeIn () {
		anim.SetTrigger("FadeInTrigger");
	}

	// Calls the animator to trigger the fade out sequence of animations.
	public void FadeOut () {
		anim.SetTrigger("FadeOutTrigger");
	}
}
