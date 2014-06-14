using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour {

	public delegate void FloatingTextDelegate();
	public event FloatingTextDelegate OnDestroyed;

	Vector3 velocity = new Vector3(0,0.01f,0);
	float alpha = 1.0f;
	public float Duration = 3.5f;

	public bool Glide = true;


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
			if(OnDestroyed!=null)
				OnDestroyed();
			Destroy(gameObject); // text vanished - destroy itself
		}
	}

	public void RefreshText(string text)
	{
		alpha=1;
		guiText.text=text;
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
