using Unity.Mathematics;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem particles;
    public AudioSource audioSource;

    private bool _exploded;
    
    public void Start()
    {
        Explode();
    }

    public void Explode()
    {
        if (_exploded) return;

        var particleDuration = particles.main.duration;
        var audioDuration = audioSource.clip.length;
        particles.Play();
        audioSource.Play();

        _exploded = true;
        
        Invoke(nameof(StopAudio),audioDuration);
        Invoke(nameof(StopParticles),particleDuration);
        Invoke(nameof(Kill),math.max(particleDuration,audioDuration));
    }

    private void StopAudio()
    {
        audioSource.Stop();
    }

    private void StopParticles()
    {
        particles.Stop();
    }

    public void Kill()
    {
        _exploded = false;
        Destroy(gameObject); //todo object pool
    }
}
