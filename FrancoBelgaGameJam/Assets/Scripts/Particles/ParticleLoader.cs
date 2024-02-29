using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLoader : MonoBehaviour
{
    [SerializeField] private ParticleSelfDestruct HitParticle;
    [SerializeField] private ParticleSelfDestruct DeathParticle;
    [SerializeField] private ParticleSelfDestruct GetHitParticle;
    [SerializeField] private ParticleSelfDestruct TaserParticle;
    [SerializeField] private ParticleSelfDestruct EnergyCollectParticle;

    void Awake()
    {
        var particle = new ParticleSelfDestruct[]{HitParticle};
        new ManagerDeParticle(particle);
        ParticleNames.Hit = HitParticle.name;
        ParticleNames.Death = DeathParticle.name;
        ParticleNames.Taser = TaserParticle.name;
        ParticleNames.EnergyCollect = EnergyCollectParticle.name;
        ParticleNames.GetHit = GetHitParticle.name;
        Destroy(this.gameObject);
    }
    
}
