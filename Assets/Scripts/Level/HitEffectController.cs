using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace PigeonB1587.prpu
{
    public class HitEffectController : MonoBehaviour
    {
        public AudioClip click, drag, flick;
        public ObjectPool<GameObject> perfectEffectsPool,
            goodEffectsPool,
            badEffectsPool;
        public GameObject perfectEffectPrefab,
            goodEffectPrefab,
            badEffectPrefab;
        public float hitFxScale = 1f;

        public void Start() => SetPool();
        private void SetPool()
        {
            perfectEffectsPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(perfectEffectPrefab, transform),
                actionOnGet: (tap) =>
                {
                    tap.SetActive(true);
                },
                actionOnRelease: (tap) => tap.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap),
                defaultCapacity: 20,
                maxSize: 10000
            );
            goodEffectsPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(goodEffectPrefab, transform),
                actionOnGet: (tap) => tap.SetActive(true),
                actionOnRelease: (tap) => tap.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap),
                defaultCapacity: 20,
                maxSize: 10000
            );
            badEffectsPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(badEffectPrefab, transform),
                actionOnGet: (tap) => tap.SetActive(true),
                actionOnRelease: (tap) => tap.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap),
                defaultCapacity: 10,
                maxSize: 1000
            );
        }
        public void GetHitFx(HitEffectType type, Vector3 position, int noteType, Transform line = null)
        {
            if (!GameInformation.Instance.isHitFXEnabled)
            {
                return;
            }
            GameObject effect = null;
            ObjectPool<GameObject> targetPool = null;

            switch (type)
            {
                case HitEffectType.Perfect:
                    effect = perfectEffectsPool.Get();
                    targetPool = perfectEffectsPool;
                    break;
                case HitEffectType.Good:
                    effect = goodEffectsPool.Get();
                    targetPool = goodEffectsPool;
                    break;
                case HitEffectType.Bad:
                    effect = badEffectsPool.Get();
                    targetPool = badEffectsPool;
                    break;
                case HitEffectType.Miss:
                    return;
            }
            var size = hitFxScale * GameInformation.Instance.noteScale;
            effect.transform.localScale = new Vector3(size, size, size);
            if (line != null)
            {
                effect.transform.SetParent(line);
            }
            else
            {
                effect.transform.SetParent(transform);
            }
            effect.transform.position = position;
            PlayAudio(noteType, effect.GetComponent<AudioSource>());
            StartCoroutine(ReturnToPoolAfterDelay(effect, targetPool, 1f));
        }

        private void PlayAudio(int type, AudioSource source)
        {
            // Unity Audio
            switch (type)
            {
                case 1:
                    source.PlayOneShot(click, GameInformation.Instance.hitFXVolume);
                    break;
                case 2:
                    source.PlayOneShot(drag, GameInformation.Instance.hitFXVolume);
                    break;
                case 4:
                    source.PlayOneShot(flick, GameInformation.Instance.hitFXVolume);
                    break;
            }
        }

        private IEnumerator ReturnToPoolAfterDelay(GameObject obj, ObjectPool<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Release(obj);
        }
    }

    public enum HitEffectType { Perfect, Good, Bad, Miss }
}
