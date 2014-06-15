using UnityEngine;
using System.Collections.Generic;

public class FloatingTextController : MonoBehaviour {



	Vector3 velocity = new Vector3(0,0.01f,0);
	float alpha = 1.0f;
	public float Duration = 3.5f;

	public bool Glide = true;
	public static Dictionary<Component, FloatingTextController> lastingTexts 
		= new Dictionary<Component, FloatingTextController>();


	static GameObject prefab;
	// Use this for initialization
	void Start () {
		alpha = 1;

	}
	
	// Update is called once per frame
	void Update () {
		if (alpha>0){
			if(Glide)
				transform.position += velocity*Time.deltaTime;
			alpha -= Time.deltaTime/Duration;

			Color c = guiText.material.color;
			guiText.material.color = new Color(c.r,c.g,c.b,alpha);
		} else {
			Component owner = null;
			foreach(Component o in lastingTexts.Keys)
			{
				if(lastingTexts[o]==this)
				{
					owner=o;
					break;
				}
			}
			if(owner!=null)
				lastingTexts.Remove(owner);
			Destroy(gameObject); // text vanished - destroy itself
		}
	}

	public static void LastingText(Component owner, Vector3 pos, string line)
	{
		FloatingTextController ftc = null;
		if(!lastingTexts.ContainsKey(owner))
		{
			ftc = SpawnText(line,pos,false,1.5f);
			lastingTexts.Add(owner,ftc);
		}
		else
		{
			ftc = lastingTexts[owner];
			ftc.alpha = 1;
			ftc.guiText.text=line;
		}


	}

	public static void ResetText(Component owner)
	{
		lastingTexts.Remove(owner);

	}

	public static FloatingTextController SpawnText(string text, Vector3 pos)
	{
		return SpawnText(text,pos,true,3.5f);
	}

	public static FloatingTextController SpawnText(string text, Vector3 pos, bool glide, float duration)
	{

		if(prefab==null)
		{
			prefab = Resources.Load<GameObject>("Prefabs/Helpers/FloaterPrefab");
			
			if(prefab==null)
				throw new UnityException("Cannot find prefab for floating text!");
		}

		Vector3 locPos = Camera.main.WorldToScreenPoint(pos);
		locPos = new Vector3(locPos.x/Camera.main.pixelWidth, locPos.y/Camera.main.pixelHeight, locPos.z);

		FloatingTextController obj = ((GameObject)GameObject.Instantiate(prefab,locPos,Quaternion.identity))
			.GetComponent<FloatingTextController>();

		obj.guiText.alignment = TextAlignment.Center;
		obj.guiText.text = text;
		obj.Glide=glide;
		obj.Duration = duration;


		return obj;
	}
}
