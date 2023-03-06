using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour
{
    public int maxEnemigos;
    public List<Zombie> listaEnemigos = new List<Zombie>();

    public bool activado = false;
    public bool modoCaos = false;

    [SerializeField] float tiempoInicioFase;
    [SerializeField] float tiempoDesatarCaos = 60;


    private static StreetManager _instance;
    public static StreetManager instance
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

    // Start is called before the first frame update
    void Start()
    {
        foreach(Zombie z in FindObjectsOfType<Zombie>())
		{
            listaEnemigos.Add(z);
		}
      
    }

    public void Empezar()
	{
        tiempoInicioFase = Time.realtimeSinceStartup;
        activado = true;
	}

	public void Update()
	{
        if (!activado)
		{
            return;
		}
		if (Time.realtimeSinceStartup - tiempoInicioFase > tiempoDesatarCaos)
		{
            if (!modoCaos)
			{
                modoCaos = true;
                maxEnemigos = 60;
			}
		}
	}
}
