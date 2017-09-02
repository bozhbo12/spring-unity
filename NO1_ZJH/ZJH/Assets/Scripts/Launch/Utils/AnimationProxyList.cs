using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationProxyList : MonoBehaviour
{
    public List<AnimationProxy> _animations = null;
    public void clearList()
    {
        if (_animations != null && _animations.Count > 0)
        {
            for (int i = 0; i < _animations.Count; i++)
            {
                if (_animations[i] == null)
                    continue;

                _animations[i].ClearAnimationClips();
            }
        }
    }

    void OnDestroy()
    {
        clearList();
        _animations.Clear();
        _animations = null;
    }
}