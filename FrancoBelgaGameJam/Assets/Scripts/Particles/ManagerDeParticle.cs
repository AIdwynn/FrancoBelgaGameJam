using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerDeParticle
{
    public static ManagerDeParticle Instance;
    public Dictionary<string, ParticleSelfDestruct> ParticleDictionary;

    public ManagerDeParticle(ParticleSelfDestruct[] particles)
    {
        Instance = this;
        ParticleDictionary = new Dictionary<string, ParticleSelfDestruct>();
        foreach (var particle in particles)
        {
            var name = particle.name;
            ParticleDictionary.Add(name, particle);
        }
    }

    public static ParticleSelfDestruct PlayParticleByName(string name)
    {
        var original = Instance.ParticleDictionary[name];
        var newOne = GameObject.Instantiate(original);
        newOne.StartCountDown();
        return newOne;
    }

    public static ParticleSelfDestruct PlayParticleByName(string name, Vector3 position)
    {
        var original = Instance.ParticleDictionary[name];
        var newOne = GameObject.Instantiate(original, position, Quaternion.identity);
        newOne.StartCountDown();
        return newOne;
    }
    
    public static ParticleSelfDestruct PlayParticleByName(string name, Vector3 position, Vector3 rotation)
    {
        var original = Instance.ParticleDictionary[name];
        var newOne = GameObject.Instantiate(original, position, Quaternion.identity);
        newOne.StartCountDown();
        return newOne;
    }
}

public static class ParticleNames
{
    public static string Hit = "";
    public static string Death = "";
    public static string GetHit = "";
    public static string Taser = "";
    public static string EnergyCollect = "";
    public static string Stun = "";
}
    
