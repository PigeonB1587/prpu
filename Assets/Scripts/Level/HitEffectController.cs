using UnityEngine;
using UnityEngine.Pool;

namespace PigeonB1587.prpu
{
    public class HitEffectController : MonoBehaviour
    {
        public ObjectPool<GameObject> perfectEffectsPool,
            goodEffectsPool,
            badEffectsPool;
        public GameObject perfectEffectPrefab,
            goodEffectPrefab,
            badEffectPrefab;

        public void Start()
        {
            SetPool();
        }

        private void SetPool()
        {
            perfectEffectsPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(perfectEffectPrefab, transform).GetComponent<GameObject>(),
                actionOnGet: (tap) =>
                {
                    tap.gameObject.SetActive(true);
                },
                actionOnRelease: (tap) => tap.gameObject.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap.gameObject),
                defaultCapacity: 20,
                maxSize: 10000
            );
            goodEffectsPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(goodEffectPrefab, transform).GetComponent<GameObject>(),
                actionOnGet: (tap) =>
                {
                    tap.gameObject.SetActive(true);
                },
                actionOnRelease: (tap) => tap.gameObject.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap.gameObject),
                defaultCapacity: 20,
                maxSize: 10000
            );
            badEffectsPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(badEffectPrefab, transform).GetComponent<GameObject>(),
                actionOnGet: (tap) =>
                {
                    tap.gameObject.SetActive(true);
                },
                actionOnRelease: (tap) => tap.gameObject.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap.gameObject),
                defaultCapacity: 10,
                maxSize: 1000
            );
        }
        public void ShowHitEffect(HitEffectType type, Vector3 position)
        {
            GameObject effect = null;
            switch (type)
            {
                case HitEffectType.Perfect:
                    effect = perfectEffectsPool.Get();
                    break;
                case HitEffectType.Good:
                    effect = goodEffectsPool.Get();
                    break;
                case HitEffectType.Bad:
                    effect = badEffectsPool.Get();
                    break;
                case HitEffectType.Miss:
                    return;
            }
            effect.transform.position = position;
            StartCoroutine(ReleaseEffectAfterDelay(effect, 1f));
        }

        private System.Collections.IEnumerator ReleaseEffectAfterDelay(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (effect.CompareTag("PerfectEffect"))
            {
                perfectEffectsPool.Release(effect);
            }
            else if (effect.CompareTag("GoodEffect"))
            {
                goodEffectsPool.Release(effect);
            }
            else if (effect.CompareTag("BadEffect"))
            {
                badEffectsPool.Release(effect);
            }
        }
    }

    public enum HitEffectType { Perfect, Good, Bad, Miss }
}
