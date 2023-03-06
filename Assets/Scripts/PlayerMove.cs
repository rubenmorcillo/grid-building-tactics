using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{
    public NavMeshAgent agent;
    public Interactuable target;
    public Zombie targetEnemy;
    public Vector3 realTargetPoint;
    public Animator animator;
    AudioSource audio;
    //public bool weapon;
    public Survivor player;
    
    private Coroutine lastCoroutine;

    private IClickable _previousClickable;

    public enum PlayerState { RUN, COLLECTING, ATTACKING, IDDLE }
    public PlayerState playerState = PlayerState.IDDLE;

    public void SetCurrentAgent(NavMeshAgent agent)
	{
        this.agent = agent;
        player = agent.GetComponent<Survivor>();
        GameManager.instance.SetPersonajeActivo(player);
        
        animator = agent.GetComponent<Animator>();
    }

	private void Start()
    {
        SetCurrentAgent(GetComponent<NavMeshAgent>());
        audio = GetComponent<AudioSource>();
	}
    
    void Update()
    {
        
        if (!player.alive)
		{
            return;
		}
        if (GameManager.instance.gameState == GameManager.GameState.EXPLORE)
		{
            CheckZonaSegura();
		}
        //animator.SetBool("Weapon", weapon);
        if (GameManager.instance.gameState != GameManager.GameState.BUILD)
        {
            if (player != null && player.damaged)
			{
                if (lastCoroutine != null)
				{
                    StopCoroutine(lastCoroutine);
                }
               
                StartCoroutine(DamagedDelay());
                return;
			}
            if (targetEnemy != null)
			{
                if (!targetEnemy.alive)
				{
                    targetEnemy = null;
				}
				else
				{
                    GoThere(targetEnemy.transform.position);
                }
			}
            //OnHover Scope
            OnHoverScope();
            MouseInput();

        }

        if (ArrivedToTarget())
        {
            animator.SetBool("Run", false);
            if (playerState == PlayerState.RUN)
            {
                playerState = PlayerState.IDDLE;
            }

            if (target != null && playerState == PlayerState.IDDLE)
            {
                if (!target.looted)
                {
                    playerState = PlayerState.COLLECTING;
                    lastCoroutine = StartCoroutine(InteractWhitTarget(target));
                }

                target = null;
            }
            else if (targetEnemy != null && playerState == PlayerState.IDDLE)
            {
                if (targetEnemy.alive)
				{
                    playerState = PlayerState.ATTACKING;
                    StartCoroutine(AttackDelay());
				}
				else
				{
                    targetEnemy = null;
				}
            }
        }
        else
        {
            if (!agent.isStopped)
			{
                animator.SetBool("Run", true);
            }
        }
    }
    void CheckZonaSegura()
	{
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit)){
            //Debug.Log("encima de "+hit.transform.name);
            if (hit.transform.name.Equals("ZonaSegura"))
			{
                GameManager.instance.GoBackHome();
            }
            if (hit.transform.name.Equals("ActivadorCalle"))
			{
                StreetManager.instance.Empezar();
			}
        }
	}

    void MouseInput()
	{
        if (Input.GetMouseButtonDown(1))
        {
            if (lastCoroutine != null)
			{
                StopCoroutine(lastCoroutine); //no está funcionando
            }
           
			ResetAnimator(); //Resetear todo para que se mueva
            animator.Play("Running");
            target = null;
            targetEnemy = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Suelo"))
				{
                    playerState = PlayerState.RUN;
                    agent.Resume();
                    //agent.GetComponent<Animator>().SetBool("Run", true);
                    GoThere(hit.point);
                }
                if (hit.transform.CompareTag("Interactuable"))
                {
                    agent.Resume();

                    target = hit.transform.GetComponent<Interactuable>();
                    //encontrar punto más cercano de la malla
                    //hit.transform.GetComponent<BoxCollider>().ClosestPoint(agent.transform.position);
                    Vector3[] boundPositions;
                    GoThere(hit.transform.GetComponent<BoxCollider>().ClosestPoint(agent.transform.position));

                }
                if (hit.transform.CompareTag("Enemigo"))
                {
                    targetEnemy = hit.transform.GetComponent<Zombie>();
                    agent.Resume();
                    //agent.GetComponent<Animator>().SetBool("Run", true);
                    GoThere(hit.point);
                }

            }
        }
    }

    void ResetAnimator()
	{
        animator.SetBool("Cook", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Heal", false);
        animator.SetBool("Craft", false);
        animator.SetBool("Build", false);
        //animator.SetBool("Run", false);
	}

    void GoThere(Vector3 point)
	{
        realTargetPoint = point;
        //Debug.Log("Voy a " + point);
        //playerState = PlayerState.RUN;
        agent.SetDestination(point);
    }

    bool ArrivedToTarget()
	{
        bool arrived = false;

        if (target != null)
		{
            Vector3 dif = realTargetPoint - agent.transform.position;
            if (Mathf.Abs(dif.x) < agent.stoppingDistance && Mathf.Abs(dif.z ) < agent.stoppingDistance)
            {
                arrived = true;
            }
		}
		else if (targetEnemy != null)
        {
            Vector3 dif = targetEnemy.transform.position - agent.transform.position;
            //Debug.Log("remaining :" + agent.remainingDistance);
            if (Mathf.Abs(dif.x) < agent.stoppingDistance && Mathf.Abs(dif.z) < agent.stoppingDistance)
            {
                arrived = true;
            }
        }
		else
		{
            arrived = agent.remainingDistance <= agent.stoppingDistance;
        }
        
        
        return arrived;
    }

    void Collect(Interactuable interactuable)
	{
        //Debug.Log("Collecting " + interactuable.name);
        interactuable.GiveLoot();
        //agent.SetDestination(interactuable.gameObject.transform.position); //debería buscar el punto más cercano del collider del objeto
	}

    public IEnumerator InteractWhitTarget(Interactuable target)
	{
        animator.SetBool("Cook", true);
		yield return new WaitForSeconds(target.interactTime);
        Collect(target);
        animator.SetBool("Cook", false);
        playerState = PlayerState.IDDLE;
	}

    IEnumerator AttackDelay()
	{
        playerState = PlayerState.ATTACKING;
        animator.Play("Standing Melee Punch");
        if (audio != null)
        {
            audio.Play();
        }
        targetEnemy.SufrirDamage(player.fuerza);
        yield return new WaitForSeconds(1);
        if (playerState == PlayerState.ATTACKING)
        {
            playerState = PlayerState.IDDLE;
        }
    }

    IEnumerator DamagedDelay()
    {
        //animator.Play("Iddle"); //poner la animacion de sufrir daño
        yield return new WaitForSeconds(0.5f);
        player.damaged = false;
    }

    void OnHoverScope()
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            IClickable clickable = hit.transform.GetComponent<IClickable>();
            if (clickable != _previousClickable)
            {
                if (_previousClickable != null)
                {
                    _previousClickable.OnHoverExit();
                }
                if (clickable != null)
                {
                    clickable.OnHoverEnter();
                }
                _previousClickable = clickable;
            }
        }
    }

}
