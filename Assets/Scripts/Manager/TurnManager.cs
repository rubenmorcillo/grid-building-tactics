using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TurnManager
{
    //public static List<TestTacticsMove> unidadesCombate = new List<TestTacticsMove>();
    //public static Queue<TestTacticsMove> unidadesTurno = new Queue<TestTacticsMove>();


    //public void Update()
    //{
    //    if (unidadesTurno.Count == 0)
    //    {
    //        IniciarColaTurnos();
    //    }
    //}

    //public static void AddUnit(TestTacticsMove unidad)
    //{
    //    unidadesCombate.Add(unidad);
    //}


    //public static void IniciarColaTurnos()
    //{
    //    if (GameManager.instance.mostrarDebug) Debug.Log("TM: iniciando cola turnos...");
    //    IOrderedEnumerable<TestTacticsMove> orderedEnumerables = unidadesCombate.Where(u => u.datosUnidad.estoyVivo).OrderByDescending(u => u.datosUnidad.iniciativa);
    //    for (int i = 0; i < orderedEnumerables.Count(); i++)
    //    {
    //        if (GameManager.instance.mostrarDebug) Debug.Log("TM: metiendo en cola a " + orderedEnumerables.ElementAt(i).datosUnidad.tipo.nombre + " cuya iniciativa es -> " + orderedEnumerables.ElementAt(i).datosUnidad.iniciativa);
    //        unidadesTurno.Enqueue(orderedEnumerables.ElementAt(i));
    //    }

    //    StartTurn();
    //}

    //public static void StartTurn()
    //{
    //    if (unidadesTurno.Count > 0)
    //    {
    //        if (GameManager.instance.mostrarDebug) Debug.Log("TM: pickeando de la cola a " + unidadesTurno.First().datosUnidad.tipo.nombre);
    //        TestTacticsMove unidad = unidadesTurno.Peek();
    //        unidad.BeginTurn();
    //        GameManager.instance.combateManager.SetUnidadActiva(unidad);
    //    }
    //}

    //public static void EndTurn()
    //{

    //    TestTacticsMove unit = unidadesTurno.Dequeue();
    //    if (GameManager.instance.mostrarDebug) Debug.Log("TM: sacando de la cola a " + unit + " porque ha terminado su turno");
    //    unit.EndTurn();

    //    if (unidadesTurno.Count > 0)
    //    {
    //        StartTurn();
    //    }
    //    else
    //    {
    //        IniciarColaTurnos();
    //    }
    //}
}
