using System;
using System.Collections;
using System.Collections.Generic;
using PotatoGame;
using UnityEngine;
using Object = UnityEngine.Object;


public interface IWalkable
{
    void Walk(float speed);
}

public class Animal : MonoBehaviour, IWalkable
{
    protected bool something;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        
    }

    public virtual void Eat()
    {
        CheckHealth();
    }

    public virtual bool CheckHealth()
    {
        //if healt <= 0, die()
        return true;
    }

    public void Walk(float speed)
    {
        
    }
}

[System.Serializable]
public class Wolf : Animal
{
    public override void Eat()
    {
        base.Eat();
        
        //specific behaviour
    }
}

[System.Serializable]
public class Bear : Animal
{
    public override void Eat()
    {

    }
}

[System.Serializable]
public class AnimalController : Singleton<AnimalController>
{
    public List<Animal> animals = new List<Animal>();

    private void Start()
    {
        animals.Add( new Bear());
        animals.Add( new Wolf());
    }

    private void Update()
    {
        // this.GetComponent<IWalkable>();
        // IWalkable[] obj = FindObjectsOfType<IWalkable>();
        
        foreach (var animal in animals)
        {
            animal.Eat();
            animal.Walk(1.0f);
        }
    }
}
