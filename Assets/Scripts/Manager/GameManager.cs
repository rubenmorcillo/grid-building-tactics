using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ResourceManager))]
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public ResourceManager resourceManager;
    public UIManager uiManager;
	public SoundManager soundManager;
	public Survivor personajeActivo;
	


	public bool esperando = true;

    [SerializeField] public List<PlacedObject_Done> placedObjectInBase = new List<PlacedObject_Done>();

    [SerializeField] 
    public enum GameState {MENU_INICIO, BASE, BUILD,DESTROY, EXPLORE, GAMEOVER_LOSE, GAMEOVER_WIN, END}
    public GameState gameState;
    public static GameManager instance
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

        uiManager = GetComponent<UIManager>();
        resourceManager = GetComponent<ResourceManager>();
        soundManager = GetComponentInChildren<SoundManager>();
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
	// Start is called before the first frame update
	void Start()
    {
		uiManager.EfectoDelay(uiManager.panelStart, false);
		StartCoroutine(SceneDelay("MenuInicioScene", uiManager.panelDelayTime));
		//SceneManager.LoadScene("MenuInicioScene");
		//gameState = GameState.BASE;
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

	private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
	{
		if (scene.name.Equals("SampleScene"))
		{
			GridBuildingSystem3D.Instance.Init();
		}
		
		if (scene.name.Equals("MenuInicioScene"))
		{
			uiManager.StartOff();
			uiManager.EfectoDelay(uiManager.panelMenu, true);
		}
		if (scene.name.Equals("EndGameScene"))
		{
			GetComponentInChildren<CursorManager>().SetCursor(CursorManager.CursorType.DEFAULT);
		}
		soundManager.ChangeMusic();

	}

	void Update()
    {
		if (!esperando)
		{
			if (gameState == GameState.MENU_INICIO)
			{
				if (Input.GetKeyDown(KeyCode.Return))
				{
					GoBackHome();
				}
			}
		}
		

		if (gameState == GameState.GAMEOVER_LOSE)
		{
			
			SceneManager.LoadScene("EndGameScene");
			gameState = GameState.END;
			uiManager.ToggleEnd(false);
		}
		if(gameState == GameState.GAMEOVER_WIN)
		{
			SceneManager.LoadScene("EndGameScene");
			gameState = GameState.END;
			uiManager.ToggleEnd(true);
		}
		if (gameState == GameState.END)
		{
			
			if (Input.GetKeyDown(KeyCode.Return))
			{
				MainMenu();
			}
		}
	}
	IEnumerator SceneDelay(string sceneName, float t)
	{
		yield return new WaitForSeconds(t);
		SceneManager.LoadScene(sceneName);
		
	}
	
    public void ToggleBuildMode()
	{
        if (gameState != GameState.BUILD)
		{
            gameState = GameState.BUILD;
		}
		else
		{
            gameState = GameState.BASE;
		}
    }
    public void ToggleDestroyMode()
	{
        if (gameState != GameState.DESTROY)
		{
            gameState = GameState.DESTROY;
		}
		else
		{
            gameState = GameState.BASE;
		}
	}

    public void ChangeState(GameState nuevoEstado)
	{
        gameState = nuevoEstado;
	}

    public void GoToStreet()
	{
        gameState = GameState.EXPLORE;
        //uiManager.ToggleExploreMode();
		uiManager.BaseModeOff();

		//UIManager.instance.enabled = false;
		SceneManager.LoadScene("StreetScene");
	}

    public void GoBackHome()
	{
		if (gameState == GameState.MENU_INICIO)
		{
			uiManager.ToggleMenuMode();
			//GetComponentInChildren<GridBuildingSystem3D>().Init();
		}
		else
		{
			uiManager.BaseModeOn();
		}

		gameState = GameState.BASE;
		

		//UIManager.instance.enabled = false;
		SceneManager.LoadScene("SampleScene");
	}

	void MainMenu()
	{
		////reiniciar las vainas de los managers
		//resourceManager -> valores iniciales
		uiManager.ToggleMenuMode();
		resourceManager.ResetManager();
		
		GetComponentInChildren<GridBuildingSystem3D>().ResetManager();
		gameState = GameState.MENU_INICIO;
		SceneManager.LoadScene("MenuInicioScene");
	}
	public void SetPersonajeActivo(Survivor survivor)
	{
		uiManager.CameraSurvivor = survivor.GetComponentInChildren<Camera>();
		personajeActivo = survivor;
	}


}
