using UnityEngine;
using System.Collections;

public class ShieldGeneratorController : BaseManagedController, IValueModifier {

	public HullController Hull;
	public float Strength = 70;
	public float Delay = 5;
	public Transform ShieldModel;

	float counter = 0;
	bool turnedOn=false;

	Transform lastAttacker = null;
	// Use this for initialization
	void Start () {
	
		if(Hull==null)
			throw new UnityException("Hull must not be null");

		ShieldModel.renderer.enabled = false;
		Hull.OnUnderAttack+=OnUnderAttack;
	}
	
	// Update is called once per frame
	void Update () {
		if(counter>0)
		{
			counter-=Time.smoothDeltaTime;
			if(counter<=0)
			{
				counter=0;
				lastAttacker = null;
				TurnOff();
			}
		}
	}

	void OnUnderAttack(Transform attacker)
	{
		if(lastAttacker!=attacker && !turnedOn)
			TurnOn();
		lastAttacker = attacker;
		counter = Delay;
	}

	void TurnOn()
	{
		turnedOn = true;
		ShieldModel.renderer.enabled = true;
	}

	void TurnOff()
	{
		turnedOn = false;
		ShieldModel.renderer.enabled = false;
	}

	#region IValueModifier implementation

	public void Modify (ref int val)
	{
		val-=(int)Strength;
		if(val<0)
			val = 0;
	}

	#endregion
}
