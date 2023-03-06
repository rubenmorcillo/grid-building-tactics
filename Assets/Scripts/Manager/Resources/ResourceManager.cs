using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] List<Resource> resources;
    public delegate void ValueChange();
    public static event ValueChange RecursoCambiadoEvent;
    public event EventHandler<OnResourceValueChangeArgs> OnValueChange;
    public event EventHandler<OnResourceValueChangeArgs> OnMaxStorageChange;

    private int initialWoodAmount = 150;
    private int initialMetalAmount = 150;
    private int initialWoodMax = 200;
    private int initialMetalMax = 200;

    public class OnResourceValueChangeArgs
	{
        public int amount;
        public string resourceName;
        public OnResourceValueChangeArgs(int amount, string resourceName)
		{
            this.amount = amount;
            this.resourceName = resourceName;
		}
	}

	//private static ResourceManager _instance;

	//public static ResourceManager instance
	//{
	//	get
	//	{
	//		return _instance;
	//	}
	//}

	//private void Awake()
	//{
	//	if (_instance != null && _instance != this)
	//	{
	//		Destroy(this.gameObject);
	//	}

	//	_instance = this;
	//	DontDestroyOnLoad(this.gameObject);
	//}
    public void cambiarRecursos( OnResourceValueChangeArgs eventArgs)
	{
        //Debug.Log("Cambiando " + eventArgs.amount + " de " + eventArgs.resourceName);
        foreach (Resource r in resources)
        {
            if (r._name.ToUpperInvariant().Equals(eventArgs.resourceName.ToUpperInvariant()))
            {
                r._amount += eventArgs.amount;
                if (r._amount > r._maxAmount)
                {
                    r._amount = r._maxAmount;
                }
            }
        }
        RecursoCambiadoEvent();
    }

    public void ChangeMaxStorage(OnResourceValueChangeArgs recurso)
    {
        foreach (Resource r in resources)
        {
            if (r._name.ToUpperInvariant().Equals(recurso.resourceName.ToUpperInvariant()))
            {
                r._maxAmount += recurso.amount;
            }
        }
        RecursoCambiadoEvent();
    }

    void Start()
    {
        if (resources == null)
		{
            resources = new List<Resource>();
            GenerateInitialResources();
		}

   //     OnValueChange += (object sender, OnResourceValueChangeArgs eventArgs) => {
   //         //Debug.Log("Cambiando " + eventArgs.amount + " de " + eventArgs.resourceName);
   //         foreach (Resource r in resources)
			//{
   //             if (r._name.ToUpperInvariant().Equals(eventArgs.resourceName.ToUpperInvariant()))
			//	{
   //                 r._amount += eventArgs.amount;
   //                 if (r._amount > r._maxAmount)
			//		{
   //                     r._amount = r._maxAmount;
			//		}
   //             }
   //         }
   //         GameManager.instance.uiManager.TriggerOnResourceChange( eventArgs.resourceName);
   //     };

        //OnMaxStorageChange += (object sender, OnResourceValueChangeArgs eventArgs) =>
        //{
        //    foreach (Resource r in resources)
        //    {
        //        if (r._name.ToUpperInvariant().Equals(eventArgs.resourceName.ToUpperInvariant()))
        //        {
        //            r._maxAmount += eventArgs.amount;
        //        }
        //    }
        //    GameManager.instance.uiManager.TriggerOnResourceChange(eventArgs.resourceName);
        //};
    }

    public void ResetManager()
	{
        resources.Clear();
        GenerateInitialResources();
        //GameManager.instance.uiManager.TriggerOnResourceChange("madera");
        //GameManager.instance.uiManager.TriggerOnResourceChange("metal");
    }

    private void GenerateInitialResources()
    {
        Resource madera = new Resource(initialWoodAmount, "madera", initialWoodMax);
        Resource metal = new Resource(initialMetalAmount, "metal", initialMetalMax);
        //Resource tela = new Resource(0, "tela");
        AddResource(madera);
        AddResource(metal);
        //AddResource(tela);

    }

   
    public void AddResource(Resource r)
	{
        if (!resources.Contains(r)) { 
            resources.Add(r);
        }
    }

    public int CountResources()
	{
        return resources.Count;
	}

    public Resource GetResourceByIndex(int i)
	{
        return resources[i];
	}

    public Resource GetResourceByName(String name)
	{
        Resource r = null;
        foreach(Resource re in resources)
		{
            if (re._name.ToUpperInvariant().Equals(name.ToUpperInvariant()))
			{
                r = re;
			}
		}
        return r;
	}

    public List<Resource> GetResourceList()
	{
        return resources;
	}

    
}
