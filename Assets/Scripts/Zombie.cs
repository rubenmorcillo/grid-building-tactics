using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Personaje, IClickable
{
    [SerializeField] Personaje personajeObjetivo;

	NavMeshAgent agent;
	AudioSource audio;

	private void Start()
	{
		damaged = false;
		damagedTime = 0.25f;
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		audio = GetComponent<AudioSource>();
		alive = true;
		currentVida = maxVida;
		personajeObjetivo = GameObject.Find("Player").GetComponent<Personaje>();
	}

	void Update()
    {
        if (currentVida <= 0)
		{
			Morir();
			return;
		}
		if (StreetManager.instance.activado)
		{
			if (damaged)
			{
				return;
			}

			if (!attacking)
			{
				agent.SetDestination(personajeObjetivo.transform.position);
			}

			if (agent.remainingDistance != 0 && ArrivedToTarget())
			{
				animator.SetBool("Run", false);
				if (!attacking)
				{
					if (audio != null && !audio.isPlaying)
					{
						audio.Play();
					}
					animator.Play("Bash");
					attacking = true;
					lastCoroutine = StartCoroutine(AttackDelay());
				}
			}
			else
			{
				animator.SetBool("Run", true);
			}
		}
		
	}

	bool ArrivedToTarget()
	{
		bool arrived = false;

		if (personajeObjetivo != null)
		{
			Vector3 dif = personajeObjetivo.transform.position - agent.transform.position;
			//Debug.Log("remaining :" + agent.remainingDistance);
			if (Mathf.Abs(dif.x) < agent.stoppingDistance && Mathf.Abs(dif.z) < agent.stoppingDistance)
			{
				arrived = true;
			}
		}


		return arrived;
	}

	IEnumerator AttackDelay()
	{
		yield return new WaitForSeconds(1f);
		personajeObjetivo.SufrirDamage(fuerza);
		attacking = false;
	}

	IEnumerator MorirDelay()
	{
		yield return new WaitForSeconds(10);
		Destroy(gameObject);
	}

	new public void Morir()
	{
		animator.Play("Zombie Dying");
		animator.SetBool("Alive", false);
		alive = false;
		StreetManager.instance.listaEnemigos.Remove(this);
		StartCoroutine(MorirDelay());
		//desvanecerse;
	}

	public void OnHoverEnter()
	{
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			CursorManager.instance.SetCursor(CursorManager.CursorType.ATTACK);
			r.material.SetColor("_OutlineColor", new Color32(174, 12, 34, 190));
			r.material.SetFloat("_Outline", 0.2f);
		}
	}

	public void OnHoverExit()
	{
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			CursorManager.instance.SetCursor(CursorManager.CursorType.DEFAULT);
			r.material.SetColor("_OutlineColor", new Color32(0, 0, 0, 190));
			r.material.SetFloat("_Outline", 0f);
		}
	}

}
