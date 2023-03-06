using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;

public class Personaje : MonoBehaviour
{
    [SerializeField] protected int range;
    public int Range
	{
		get
		{
            return range;
		}
		set
		{
            range = value;
		}
	}
    [SerializeField] protected int maxVida;
    public int MaxVida
	{
		get
		{
            return maxVida;
		}
		set
		{
            maxVida = value;
		}
	}
    [SerializeField] protected int currentVida;
    public int CurrentVida
	{
		get
		{
            return currentVida;
		}
	}
    [SerializeField] public int fuerza;
    [SerializeField] public bool alive;
    public Coroutine lastCoroutine;
    public bool attacking = false;
    public bool damaged;
    public float damagedTime = 0;

    [SerializeField] protected Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        alive = true;
        currentVida = maxVida;
    }

	private void Update()
	{
      
	}

	
    protected bool IsTargetInRange(Vector3 position)
    {
        return true;
    }

    public void SufrirDamage(int dmg)
	{
        attacking = false;
        if (lastCoroutine != null)
		{
            StopCoroutine(lastCoroutine);
        }
        damaged = true;
        StartCoroutine(DamagedDelay());
        UtilsClass.CreateWorldTextPopup(""+(-dmg), transform.position);
        currentVida -= dmg;
	}

    IEnumerator DamagedDelay()
    {
        animator.Play("Idle");
        yield return new WaitForSeconds(damagedTime);
        damaged = false;
    }

	private void OnGUI()
	{
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        int offset = 40;
        GUI.Box(new Rect(pos.x - offset/1.5f, Screen.height - (pos.y + offset * 1.5f), 60, 20), currentVida + "/" + maxVida);
    }
}
