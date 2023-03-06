using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CursorManager : MonoBehaviour
{
	private static CursorManager _instance;
	[SerializeField] private Texture2D[] cursorTextureArray;
	//[SerializeField] private int frameCount;
	//[SerializeField] private float frameRate;

	//private int currentFrame;
	//private float frameTimer;
	public enum CursorType { DEFAULT, LOOK, ATTACK }
	public static CursorManager instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}

		_instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		Cursor.SetCursor(cursorTextureArray[0], new Vector2(0, 0), CursorMode.Auto);
	}

	public void SetCursor(CursorType newType)
	{
		Texture2D actualTextureType;
		switch (newType)
		{
			case CursorType.DEFAULT:
				actualTextureType = cursorTextureArray[0];
				break;
			case CursorType.LOOK:
				actualTextureType = cursorTextureArray[1];
				break;
			case CursorType.ATTACK:
				actualTextureType = cursorTextureArray[2];
				break;
			default:
				actualTextureType = cursorTextureArray[0];
			return;
		}
		Cursor.SetCursor(actualTextureType, new Vector2(0, 0), CursorMode.Auto);
	}


}
