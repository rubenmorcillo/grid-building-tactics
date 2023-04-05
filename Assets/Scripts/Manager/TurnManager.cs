using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TurnManager
{
	public static List<TacticUnity> unidadesCombate = new List<TacticUnity>();
	public static Queue<TacticUnity> unidadesTurno = new Queue<TacticUnity>();


	public void Update()
	{
		if (unidadesTurno.Count == 0)
		{
			IniciarColaTurnos();
		}
	}

	public void AddUnit(TacticUnity unidad)
	{
		unidadesCombate.Add(unidad);
	}


	public static void IniciarColaTurnos()
	{
		//if (GameManager.instance.mostrarDebug) Debug.Log("TM: iniciando cola turnos...");
		IOrderedEnumerable<TacticUnity> orderedEnumerables = unidadesCombate.Where(u => u.combatData.isAlive).OrderByDescending(u => u.combatData.initiative);
		Debug.Log("tenemos un total de unidades de " + unidadesCombate.Count() +" de los cuales " + orderedEnumerables.Count()+" personajes vivos");
		for (int i = 0; i < orderedEnumerables.Count(); i++)
		{
			//if (GameManager.instance.mostrarDebug) Debug.Log("TM: metiendo en cola a " + orderedEnumerables.ElementAt(i).datosUnidad.tipo.nombre + " cuya iniciativa es -> " + orderedEnumerables.ElementAt(i).datosUnidad.iniciativa);
			unidadesTurno.Enqueue(orderedEnumerables.ElementAt(i));
		}

		StartTurn();
	}

	public static void StartTurn()
	{
		if (unidadesTurno.Count > 0)
		{

			//if (GameManager.instance.mostrarDebug) Debug.Log("TM: pickeando de la cola a " + unidadesTurno.First().datosUnidad.tipo.nombre);
			TacticUnity unidad = unidadesTurno.Peek();
			Debug.Log("empezando turno de "+unidad.name);
			unidad.BeginTurn();
			CombateManager.instance.GetPathfinding().FindSelectableNodes();
			//pathRenderer.CreateSelectableNodesMesh(pathfinding.GetSelectableNodesVerts(), meshMaterial);
			//pathRenderer.DrawShape(pathfinding.GetSelectableNodesVertsInWorld());

			CombateManager.instance.GetPathfinding().DebugDrawSelectables();
			CombateManager.instance.GetPathRenderer().CreateSelectableNodesMesh();
			CombateManager.instance.SetCurrentUnity(unidad);
		}
	}

	public static void EndTurn()
	{

		TacticUnity unit = unidadesTurno.Dequeue();
		//if (GameManager.instance.mostrarDebug) Debug.Log("TM: sacando de la cola a " + unit + " porque ha terminado su turno");
		unit.EndTurn();

		if (unidadesTurno.Count > 0)
		{
			StartTurn();
		}
		else
		{
			IniciarColaTurnos();
		}
	}
}
