using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : Personaje
{

    [SerializeField]
    private string nombre;
    [SerializeField]
    private bool armado = false;

    public string Nombre
	{
		get
		{
            return nombre;
		}
		set
		{
            nombre = value;
		}
	}

    public bool Armado
	{
		get
		{
            return armado;
		}
		set
		{
            armado = value;
		}
	}


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        alive = true;
        currentVida = maxVida;
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            if (currentVida <= 0)
            {
                Morir();
            }
        }
    }

    protected void Morir()
    {
        animator.SetBool("Alive", false);
        alive = false;
        GameManager.instance.gameState = GameManager.GameState.GAMEOVER_LOSE;
    }
}
