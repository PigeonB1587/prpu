using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PigeonB1587.prpu
{
    public class HitEffectController : MonoBehaviour
    {
        public AudioClip click, drag, flick;
        public List<(AudioClip audio, int noteIndex, int judgeLineIndex)> customHitSound = new();
        public ObjectPool<GameObject> perfectEffectsPool,
            goodEffectsPool,
            badEffectsPool;
        public GameObject perfectEffectPrefab,
            goodEffectPrefab,
            badEffectPrefab;
        public float hitFxScale = 1f;


        public Color perfectHitFxDefaultColor, perfectHitFxParticleDefaultColor;
        public Color goodHitFxDefaultColor, goodHitFxParticleDefaultColor;

        private Dictionary<int, List<(AudioClip audio, int noteIndex)>> _customSoundGroups;
        private Dictionary<GameObject, AudioSource> _audioSourceCache = new Dictionary<GameObject, AudioSource>();

        // bushiwoxiede
        public void Start() => SetPool();

        public async UniTask LoadCustomClip()
        {
            foreach (var item in GameInformation.Instance.levelStartInfo.noteAssets)
            {
                Debug.Log($"Load custom hit sound: AddressableKey:{item.hitSoundAddressableKey}\njudgeLineIndex={item.judgeLineIndex}, noteIndex={item.noteIndex}");
                AudioClip clip = await GameInformation.Instance.LoadAddressableAsset<AudioClip>(item.hitSoundAddressableKey);
                customHitSound.Add((clip, item.noteIndex, item.judgeLineIndex));
            }
            customHitSound.Sort((a, b) => a.judgeLineIndex.CompareTo(b.judgeLineIndex));
            _customSoundGroups = new Dictionary<int, List<(AudioClip, int)>>();
            foreach (var sound in customHitSound)
            {
                if (!_customSoundGroups.ContainsKey(sound.judgeLineIndex))
                {
                    _customSoundGroups[sound.judgeLineIndex] = new List<(AudioClip, int)>();
                }
                _customSoundGroups[sound.judgeLineIndex].Add((sound.audio, sound.noteIndex));
            }

            await UniTask.CompletedTask;
            return;
        }

        private void SetPool()
        {
            perfectEffectsPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var go = Instantiate(perfectEffectPrefab, transform);
                    var audioSource = go.GetComponent<AudioSource>();
                    _audioSourceCache[go] = audioSource;
                    return go;
                },
                actionOnGet: (tap) => tap.SetActive(true),
                actionOnRelease: (tap) => tap.SetActive(false),
                actionOnDestroy: (tap) =>
                {
                    _audioSourceCache.Remove(tap);
                    Destroy(tap);
                },
                defaultCapacity: 40,
                maxSize: 10000
            );
            goodEffectsPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var go = Instantiate(goodEffectPrefab, transform);
                    var audioSource = go.GetComponent<AudioSource>();
                    _audioSourceCache[go] = audioSource;
                    return go;
                },
                actionOnGet: (tap) => tap.SetActive(true),
                actionOnRelease: (tap) => tap.SetActive(false),
                actionOnDestroy: (tap) =>
                {
                    _audioSourceCache.Remove(tap);
                    Destroy(tap);
                },
                defaultCapacity: 30,
                maxSize: 10000
            );
            badEffectsPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var go = Instantiate(badEffectPrefab, transform);
                    var audioSource = go.GetComponent<AudioSource>();
                    _audioSourceCache[go] = audioSource;
                    return go;
                },
                actionOnGet: (tap) => tap.SetActive(true),
                actionOnRelease: (tap) => tap.SetActive(false),
                actionOnDestroy: (tap) =>
                {
                    _audioSourceCache.Remove(tap);
                    Destroy(tap);
                },
                defaultCapacity: 11,
                maxSize: 1000
            );
        }

        public void GetHitFx(HitType type, Vector3 position, int noteType, Transform line = null, int lineIndex = -1, int noteIndex = -1, Color? hitFxColor = null)
        {
            var gameInfo = GameInformation.Instance;
            if (!gameInfo.isHitFXEnabled)
            {
                return;
            }

            if (type == HitType.Miss)
            {
                return;
            }

            GameObject effect = null;
            ObjectPool<GameObject> targetPool = null;
            SpriteRenderer spriteRenderer = null;
            ParticleSystem ps = null;
            ParticleSystem.MainModule main = default;

            var size = hitFxScale * gameInfo.noteScale * gameInfo.screenRadioScale;

            switch (type)
            {
                case HitType.Perfect:
                    targetPool = perfectEffectsPool;
                    effect = targetPool.Get();
                    effect.TryGetComponent(out spriteRenderer);
                    ps = effect.GetComponentInChildren<ParticleSystem>();
                    main = ps.main;

                    spriteRenderer.color = perfectHitFxDefaultColor;
                    main.startColor = perfectHitFxParticleDefaultColor;

                    if (hitFxColor.HasValue)
                    {
                        var colorValue = hitFxColor.Value;
                        spriteRenderer.color = new Color(colorValue.r, colorValue.g, colorValue.b, perfectHitFxDefaultColor.a);
                        main.startColor = new Color(colorValue.r, colorValue.g, colorValue.b, perfectHitFxParticleDefaultColor.a);
                    }
                    break;

                case HitType.Good:
                    targetPool = goodEffectsPool;
                    effect = targetPool.Get();
                    effect.TryGetComponent(out spriteRenderer);
                    ps = effect.GetComponentInChildren<ParticleSystem>();
                    main = ps.main;

                    spriteRenderer.color = perfectHitFxDefaultColor;
                    main.startColor = perfectHitFxParticleDefaultColor;

                    if (hitFxColor.HasValue)
                    {
                        var colorValue = hitFxColor.Value;
                        spriteRenderer.color = new Color(colorValue.r, colorValue.g, colorValue.b, goodHitFxDefaultColor.a);
                        main.startColor = new Color(colorValue.r, colorValue.g, colorValue.b, goodHitFxParticleDefaultColor.a);
                    }
                    break;

                case HitType.Bad:
                    targetPool = badEffectsPool;
                    effect = targetPool.Get();
                    break;
            }

            effect.transform.localScale = new Vector3(size, size, size);
            effect.transform.SetParent(line != null ? line : transform);
            effect.transform.position = position;

            if (_audioSourceCache.TryGetValue(effect, out AudioSource audioSource))
            {
                PlayHitSound(audioSource, noteType, lineIndex, noteIndex);
            }

            StartCoroutine(ReturnToPoolAfterDelay(effect, targetPool, 1f));
        }

        private void PlayHitSound(AudioSource source, int noteType, int lineIndex, int noteIndex)
        {
            if (_customSoundGroups != null && lineIndex >= 0 &&
                _customSoundGroups.ContainsKey(lineIndex))
            {
                AudioClip customClip = FindCustomHitSound(lineIndex, noteIndex);
                if (customClip != null)
                {
                    source.PlayOneShot(customClip, GameInformation.Instance.hitFXVolume);
                    return;
                }
            }

            AudioClip defaultClip = noteType switch
            {
                1 => click,
                2 => drag,
                4 => flick,
                _ => null
            };
            if (defaultClip != null)
            {
                source.PlayOneShot(defaultClip, GameInformation.Instance.hitFXVolume);
            }
        }

        private AudioClip FindCustomHitSound(int lineIndex, int noteIndex)
        {
            if (_customSoundGroups.TryGetValue(lineIndex, out var lineSounds))
            {
                foreach (var sound in lineSounds)
                {
                    if (sound.noteIndex == noteIndex)
                    {
                        return sound.audio;
                    }
                }
            }
            return null;
        }

        private IEnumerator ReturnToPoolAfterDelay(GameObject obj, ObjectPool<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Release(obj);
        }
    }

    public enum HitType { Perfect, Good, Bad, Miss }
}