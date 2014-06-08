using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour {

	Vector3 velocity = new Vector3(0,0.1f,0);
	float alpha = 1.0f;
	float duration = 1.5f;

	static GameObject prefab;
	// Use this for initialization
	void Start () {
		alpha = 1;

	}
	
	// Update is called once per frame
	void Update () {
		if (alpha>0){
			transform.position += velocity*Time.deltaTime;
			alpha -= Time.deltaTime/duration;
			Color c = guiText.material.color;
			guiText.material.color = new Color(c.r,c.g,c.b,alpha);
		} else {
			Destroy(gameObject); // text vanished - destroy itself
		}
	}

	public static void SpawnText(string text, Vector3 pos)
	{

		if(prefab==null)
		{
			prefab = Resources.Load<GameObject>("Prefabs/Helpers/FloaterPrefab");
			
			if(prefab==null)
				throw new UnityException("Cannot find prefab for floating text!");
		}
		Transform obj = ((GameObject)GameObject.Instantiate(prefab,pos,Quaternion.identity))
			.GetComponent<Transform>();

		obj.guiText.text = text;
	}
}
