using UnityEngine;
using System.Collections;

public class EffectWatcher : MonoBehaviour
{
    private NcDuplicator ncduplicator;
    private IResetAnimation[] ncResetlist;
    private NcDelayActive[] ncdelayactives;
    private ParticleSystem[] particlesystems;
    private Animation[] animations;

    //private CreateForTime[] createForTimes;

    private NcDuplicator item;

    private bool bInit = false;
    // Use this for initialization
    void Start()
    {
        if (bInit)
            return;

        bInit = true;
        item = gameObject.GetComponentInChildren<NcDuplicator>();
        if (item != null)
        {
            LogSystem.LogWarning(gameObject.name, " NcDuplicator cannot be replayed.");
            return;
        }

        ncduplicator = gameObject.GetComponentInChildren<NcDuplicator>();
        ncResetlist = gameObject.GetComponentsInChildren<IResetAnimation>(true);
        ncdelayactives = gameObject.GetComponentsInChildren<NcDelayActive>(true);
        particlesystems = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        animations = gameObject.GetComponentsInChildren<Animation>(true);
        //createForTimes = gameObject.GetComponentsInChildren<CreateForTime>(true);
    }

    public void ResetEffect()
    {
        if (!bInit)
        {
            Start();
        }
        if (item != null)
        {
            LogSystem.LogWarning(gameObject.name, " NcDuplicator cannot be replayed.");
            return;
        }

        for (int i = 0; i < ncdelayactives.Length; i++)
        {
            if (ncdelayactives[i] != null)
                ncdelayactives[i].ResetAnimation();
        }

        for (int i = 0; i < ncResetlist.Length; i++)
        {
            if (ncResetlist[i] != null)
                ncResetlist[i].ResetAnimation();
        }

        /*for (int x = 0; x < createForTimes.Length; x++)
        {
            if (createForTimes[x] != null)
                createForTimes[x].ResetAnimation();
        }*/

        for (int i = 0; i < particlesystems.Length; i++)
        {
            ParticleSystem pSystem = particlesystems[i];
            if (pSystem != null)
            {
                pSystem.Stop();
                pSystem.Clear();
                pSystem.time = 0;
                pSystem.Play();
            }
        }

        for (int i = 0; i < animations.Length; i++)
        {
            Animation animation = animations[i];
            if (animation == null)
                continue;
            foreach (AnimationState anim in animation)
            {
                anim.time = 0;
            }
            animation.Play();
        }

    }

    public void OnSpawned()
    {

    }

    void OnDestroy()
    {
        item = null;
        ncduplicator = null;
        ncResetlist = null;
        ncdelayactives = null;
        particlesystems = null;
        animations = null;
    }
}
