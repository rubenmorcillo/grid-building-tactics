using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    [Header ("Paneles")]
    //Paneles
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject panelBuildings;
    [SerializeField] GameObject panelIngame;
	[SerializeField] public GameObject panelMenu;
	[SerializeField] GameObject panelRecursos;
	[SerializeField] public GameObject panelEndGame;
	[SerializeField] public GameObject panelStart;
    [SerializeField] GameObject panelSurvivor;

    [Header("Otros txt")]
    //txt varios
    [SerializeField] Text txtInicio;
    [SerializeField] Text txtGanar;
    [SerializeField] Text txtPerder;

    [Header("Recursos")]
    //Recursos
    [SerializeField] TextMeshProUGUI txtMadera;
    [SerializeField] TextMeshProUGUI txtMetal;
    [SerializeField] TextMeshProUGUI txtTela;

    [Header("Construcción")]
    //construccion edificios
    [SerializeField] TextMeshProUGUI txtNombre;
    [SerializeField] TextMeshProUGUI txtCosteMadera;
    [SerializeField] TextMeshProUGUI txtCosteMetal;
    [SerializeField] TextMeshProUGUI txtDesc;
    [SerializeField] Button btnBuildConfirm;
    [SerializeField] Button btnBuildMode;
    [SerializeField] Button btnStreet;
    [SerializeField] Button btnHome;
    [SerializeField] Button btnSurvivor;

    [Header("Survivor")]
    //Survivor
    [SerializeField] Camera cameraSurvivor;
    [SerializeField] TextMeshProUGUI txtSurvivorNombre;
    [SerializeField] TextMeshProUGUI txtSurvivorVida;
    [SerializeField] TextMeshProUGUI txtSurvivorAtaque;

    public float panelDelayTime = 2f;
    public bool panelDelayed = false;

    [SerializeField] PlacedObjectTypeSO selectedBuild;

    List<TextMeshProUGUI> txtRecursos;
    List<Button> botones = new List<Button>();

    public static UIManager instance
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
            //Destroy(canvas);
            Destroy(this.gameObject);

        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        txtRecursos = new List<TextMeshProUGUI>();
        txtRecursos.Add(txtMetal);
        txtRecursos.Add(txtMadera);
        txtRecursos.Add(txtTela);
        ResourceManager.RecursoCambiadoEvent += ActualizarTextosRecursos;
        panelIngame.SetActive(false);
        panelRecursos.SetActive(false);
        panelBuildings.SetActive(true);
        panelEndGame.SetActive(false);
        DontDestroyOnLoad(canvas);
        foreach(Button boton in FindObjectsOfType<Button>())
		{
            botones.Add(boton);
           
		}
        ResetBuildInfoTxts();
        IniciarTextosRecursos();
        ActualizarTextosRecursos();
        //txtMadera.text = GameManager.instance.resourceManager.GetResourceByIndex(0)._name + ": " + GameManager.instance.resourceManager.GetResourceByIndex(0)._amount
        //    + " / " + GameManager.instance.resourceManager.GetResourceByIndex(0)._maxAmount;
        //txtMetal.text = GameManager.instance.resourceManager.GetResourceByIndex(1)._name + ": " + GameManager.instance.resourceManager.GetResourceByIndex(1)._amount
        //    + " / " + GameManager.instance.resourceManager.GetResourceByIndex(1)._maxAmount;
        //txtTela.text = GameManager.instance.resourceManager.GetResourceByIndex(2)._name + ": " + GameManager.instance.resourceManager.GetResourceByIndex(2)._amount
        //    + " / " + GameManager.instance.resourceManager.GetResourceByIndex(2)._maxAmount;

        panelBuildings.SetActive(false);
        //List<TextMeshProUGUI> txtRecursos = new List<TextMeshProUGUI>();
        //txtRecursos.Add(txtMetal);
        //txtRecursos.Add(txtMadera);
        //txtRecursos.Add(txtTela);

      
    }

    void IniciarTextosRecursos() {
        foreach (Resource recurso in GameManager.instance.resourceManager.GetResourceList())
        {
            foreach(TextMeshProUGUI txt in txtRecursos)
			{
                if (txt.gameObject.name.ToUpper().Contains(recurso._name.ToUpper()))
				{
                    txt.text = recurso._name + ": " + recurso._amount + " / " + recurso._maxAmount;
                }
            }
        }
       
    }
    void ActualizarTextosRecursos()
	{
        foreach (Resource recurso in GameManager.instance.resourceManager.GetResourceList())
        {

            //Debug.Log("Cambiando "+eventArgs.resourceName);
            foreach (TextMeshProUGUI txt in txtRecursos)
            {
                //Debug.Log("texto: " + txt.text.ToUpper() + " <-> "+ eventArgs.resourceName.ToUpper());
                if (txt.text.ToUpper().Contains(recurso._name.ToUpper()))
                {
                    //Debug.Log("actualizando " + txt);
                    Resource r = GameManager.instance.resourceManager.GetResourceByName(recurso._name);
                    txt.text = r._name.ToUpper() + ": " + r._amount + " / " + r._maxAmount;
                }
            }
        }
    }
	private void Update()
	{
        if (GameManager.instance.gameState == GameManager.GameState.BUILD)
		{
            ShowDetails();
        }
        if (GameManager.instance.gameState == GameManager.GameState.BASE || GameManager.instance.gameState == GameManager.GameState.BUILD)
		{
            ShowSurvivorDetails();
        }
    }

    public Camera CameraSurvivor
	{
		get
		{
            return cameraSurvivor;
		}
		set
		{
            cameraSurvivor = value;
		}
	}

    public void StartOff()
	{
        panelStart.SetActive(false);
	}
    public void BaseModeOff()
	{
        btnBuildMode.gameObject.SetActive(false);
        btnStreet.gameObject.SetActive(false);
        btnHome.gameObject.SetActive(false);
        panelBuildings.SetActive(false);
        panelIngame.SetActive(false);
    }
    public void BaseModeOn()
	{
        btnBuildMode.gameObject.SetActive(true);
        btnStreet.gameObject.SetActive(true);
        btnHome.gameObject.SetActive(false);
        panelIngame.SetActive(true);
    }

    public void ToggleExploreMode()
	{
        DeselectBuild();
        if (GameManager.instance.gameState == GameManager.GameState.BASE)
		{
            Debug.Log("base mode off");
            BaseModeOff();
		}
        else if (GameManager.instance.gameState == GameManager.GameState.EXPLORE)
        {
            Debug.Log("base mode on");
            BaseModeOn();
		}
       
	}

    private void MenuModeOn()
	{
        panelIngame.SetActive(false);
        panelRecursos.SetActive(false);
        panelEndGame.SetActive(false);
        panelMenu.SetActive(true);
    }

    private void MenuModeOff()
	{
        txtInicio.gameObject.SetActive(false);
        panelIngame.SetActive(true);
        panelRecursos.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void ToggleMenuMode()
	{
        if (GameManager.instance.gameState == GameManager.GameState.MENU_INICIO)
		{
            MenuModeOff();
		} 
        if (GameManager.instance.gameState == GameManager.GameState.END)
		{
            MenuModeOn();
		}
	}

    void ResetBuildInfoTxts()
	{
        txtNombre.text = "";
        txtCosteMadera.text = "";
        txtCosteMetal.text = "";
        txtDesc.text = "";
	}

 //   public void TriggerOnResourceChange(string resourceName)
	//{
 //       OnResourceChange?.Invoke(this, new OnResourceChangeArgs(resourceName));
 //   }

    public void ToggleBuildMode()
	{
        ActivarBotonesBuild();
        DeselectBuild();
        panelBuildings.SetActive(!panelBuildings.activeSelf);
        GameManager.instance.ToggleBuildMode();
	}

    public void ActiveBaseMode()
	{
        panelMenu.SetActive(false);
        panelRecursos.SetActive(true);
        panelIngame.SetActive(true);
        panelSurvivor = GameObject.Find("PanelSurvivor");
	}

    public void ToggleEnd(bool win)
	{
        panelBuildings.SetActive(false);
        panelIngame.SetActive(false);
        panelMenu.SetActive(false);
        panelRecursos.SetActive(false);
        panelEndGame.SetActive(true);
        if (win)
		{
            txtPerder.enabled = false;
		}
		else
		{
            txtGanar.enabled = false;
		}
	}
   

    private void ActivarBotonesBuild()
	{
        //Debug.Log("activando botones");
        foreach (Button btn in botones)
        {
            foreach (PlacedObjectTypeSO po in GridBuildingSystem3D.Instance.GetPlacedObjectTypeSOList())
            {
                //Debug.Log("comprobando build: " +po.name + " wc: "+po.woodCost + " - mc: "+po.metalCost);
                if (btn.name.Contains(po.name))
                {
                    //Debug.Log("mi metal: "+GameManager.instance.resourceManager.GetResourceByName("METAL")._amount + "\nmi madera: " + GameManager.instance.resourceManager.GetResourceByName("MADERA")._amount);
                    if (GameManager.instance.resourceManager.GetResourceByName("METAL")._amount >= po.metalCost && GameManager.instance.resourceManager.GetResourceByName("MADERA")._amount >= po.woodCost)
                    {
                        //Debug.Log("activando " + btn.name);
                        btn.enabled = true;
                    }
                }
            }
        }
    }

    public bool IsMouseOverUI()
	{
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void DeselectBuild()
	{
        btnBuildConfirm.interactable = false;
        btnBuildConfirm.GetComponentInChildren<Text>().enabled = false;
        GridBuildingSystem3D.Instance.DeselectObjectType();
        selectedBuild = null;
	}
    public void SelectBuild(PlacedObjectTypeSO build)
	{
        DeselectBuild();
        selectedBuild = build;
        if (GameManager.instance.resourceManager.GetResourceByName("madera")._amount >= selectedBuild.woodCost && GameManager.instance.resourceManager.GetResourceByName("metal")._amount >= selectedBuild.metalCost)
		{
            btnBuildConfirm.interactable = true;
            btnBuildConfirm.GetComponentInChildren<Text>().enabled = true;
		}
		else
		{
            btnBuildConfirm.interactable = false;
            btnBuildConfirm.GetComponentInChildren<Text>().enabled = false;
        }
	}

    public void ConfirmBuild()
	{
        GridBuildingSystem3D.Instance.SetPlacedObject(selectedBuild);
    }

    void ShowDetails()
	{
        if (selectedBuild != null)
		{
            txtNombre.text = selectedBuild.name;
            if (GameManager.instance.resourceManager.GetResourceByName("madera")._amount < selectedBuild.woodCost)
			{
                txtCosteMadera.color = Color.red;
			}
			else
			{
                txtCosteMadera.color = Color.white;
            }
            txtCosteMadera.text = "Madera: "+ selectedBuild.woodCost.ToString();
            if (GameManager.instance.resourceManager.GetResourceByName("metal")._amount < selectedBuild.metalCost)
            {
                txtCosteMetal.color = Color.red;
			}
			else
			{
                txtCosteMetal.color = Color.white;

            }
            txtCosteMetal.text = "Metal: "+ selectedBuild.metalCost.ToString();
            txtDesc.text = "Descripción:\n "+ selectedBuild.desc;
		}
		else
		{
            ResetBuildInfoTxts();
        }
	}

    public void EfectoDelay(GameObject panel, bool encender)
	{
        //Debug.Log("el panel " + panel.name + " va a estar " + encender);
        if (encender)
		{
			StartCoroutine(PanelFadeInDelay(panel));
        }
		else
		{
            StartCoroutine(PanelFadeOutDelay(panel));
        }
	}

    public void ToggleSurvivorView()
	{
		panelSurvivor.SetActive(!panelSurvivor.activeSelf);
	}

	private void ShowSurvivorDetails()
	{
        string litNombre = "Superviviente: ";
        string litAtaque = "Ataque: ";
        string litVida = "Vida: ";

        txtSurvivorNombre.text = litNombre + GameManager.instance.personajeActivo.name;
        txtSurvivorAtaque.text = litAtaque + GameManager.instance.personajeActivo.fuerza.ToString();
        txtSurvivorVida.text = litVida + GameManager.instance.personajeActivo.CurrentVida + "/" +GameManager.instance.personajeActivo.MaxVida;

	}


    IEnumerator PanelFadeOutDelay(GameObject panel)
    {
        panelDelayed = false;
        panel.GetComponent<Image>().CrossFadeColor(Color.black, panelDelayTime, false, true);
        yield return new WaitForSeconds(panelDelayTime);
        panelDelayed = true;
    }
    IEnumerator PanelFadeInDelay(GameObject panel)
    {
        panelDelayed = false;
        panel.GetComponent<Image>().CrossFadeAlpha(0, panelDelayTime, false);
        yield return new WaitForSeconds(panelDelayTime);
        if (panel == panelMenu)
		{
            txtInicio.gameObject.SetActive(true);
            GameManager.instance.esperando = false;
		}
        panelDelayed = true;
    }
}
