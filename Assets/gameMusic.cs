using UnityEngine;
using System.Collections;

public class gameMusic : MonoBehaviour
{

	void Awake()
	{
		Destroy(GameObject.Find("Music"));
		DontDestroyOnLoad(gameObject);
	}

}
