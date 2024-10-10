﻿using UnityEngine;
using System.Collections;

public class TankBase : MonoBehaviour
{
    public float Speed = 10.0f;
    public float RotSpeed = 20.0f;

    protected Genome genome;
	protected NeuralNetwork brain;
    protected IMinable nearMine;
    protected IMinable goodMine;
    protected IMinable badMine;
    protected float[] inputs;

    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    public void SetNearestMine(IMinable mine)
    {
        nearMine = mine;
    }

    public void SetGoodNearestMine(IMinable mine)
    {
        goodMine = mine;
    }

    public void SetBadNearestMine(IMinable mine)
    {
        badMine = mine;
    }

    protected bool IsGoodMine(IMinable mine)
    {
        return goodMine == mine;
    }

    protected Vector3 GetDirToMine(IMinable mine)
    {
        return (mine.GetPosition() - this.transform.position).normalized;
    }
    
    protected bool IsCloseToMine(IMinable mine)
    {
        return (this.transform.position - nearMine.GetPosition()).sqrMagnitude <= 2.0f;
    }

    protected void SetForces(float leftForce, float rightForce, float dt)
    {
        Vector3 pos = this.transform.position;
        float rotFactor = Mathf.Clamp((rightForce - leftForce), -1.0f, 1.0f);
        this.transform.rotation *= Quaternion.AngleAxis(rotFactor * RotSpeed * dt, Vector3.up);
        pos += this.transform.forward * Mathf.Abs(rightForce + leftForce) * 0.5f * Speed * dt;
        this.transform.position = pos;
    }

	public void Think(float dt) 
	{
        OnThink(dt);

        if(IsCloseToMine(nearMine))
        {
            OnTakeMine(nearMine);
            PopulationManager.Instance.RelocateMine(nearMine);
        }
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(IMinable mine)
    {
    }

    protected virtual void OnReset()
    {

    }
}

public interface IMinable
{
    public Vector3 GetPosition();
    public bool IsGoodMine();
    public void SetMine(bool value);
    void SetPosition(Vector3 getRandomPos);
    GameObject GetGameObject();
}
