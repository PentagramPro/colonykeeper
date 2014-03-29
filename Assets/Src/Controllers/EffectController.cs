using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {
	
	public bool ContinuousMode = true;

	public ParticleSystem Particles;
	float timer = 0;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(ContinuousMode)
		{
			if(timer>0)
			{
				timer-=Time.smoothDeltaTime;
				if(timer<=0)
					Particles.Stop();
			}
		}
		else
		{
			if(!Particles.IsAlive())
				Destroy(gameObject);
		}

	}

	public void Spark(float time)
	{
		timer = time;
		if(!Particles.isPlaying)
			Particles.Play();
	}

	public void Play()
	{
		Particles.Play();
	}

	public void Stop()
	{
		Particles.Stop();
	}
}
