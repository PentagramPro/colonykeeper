using UnityEngine;
using System.Collections;

public class ParticleFxController : EffectController {

	public bool ContinuousMode = true;
	
	public ParticleSystem Particles;
	float timer = 0;

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

	public override void Spark(float time)
	{
		timer = time;
		if(!Particles.isPlaying)
			Particles.Play();
	}
	
	public override void Play()
	{
		Particles.Play();
	}
	
	public override void Stop()
	{
		Particles.Stop();
	}

}
