using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PigeonB1587.prpu
{
    public class NotePool : MonoBehaviour
    {
        public ObjectPool<TapController> tapPool;
        public ObjectPool<DragController> dragPool;

        public GameObject tapPrefab, dragPrefab;

        public int defaultSize, maxSize;
        public void Start()
        {
            tapPool = new ObjectPool<TapController>(
                createFunc: () => Instantiate(tapPrefab).GetComponent<TapController>(),
                actionOnGet: (tap) =>
                {
                    tap.gameObject.SetActive(true);
                    tap.isJudge = false;
                },
                actionOnRelease: (tap) =>
                {
                    tap.gameObject.transform.parent = null;
                    tap.gameObject.SetActive(false);
                },
                actionOnDestroy: (tap) => Destroy(tap.gameObject),
                defaultCapacity: defaultSize,
                maxSize: maxSize
            );

            dragPool = new ObjectPool<DragController>(
                createFunc: () => Instantiate(dragPrefab).GetComponent<DragController>(),
                actionOnGet: (drag) =>
                {
                    drag.gameObject.SetActive(true);
                    drag.isJudge = false;
                },
                actionOnRelease: (drag) =>
                {
                    drag.gameObject.transform.parent = null;
                    drag.gameObject.SetActive(false);
                },
                actionOnDestroy: (drag) => Destroy(drag.gameObject),
                defaultCapacity: defaultSize,
                maxSize: maxSize
            );
        }

        public TapController GetTap(Transform parent)
        {
            var tap = tapPool.Get();
            tap.transform.SetParent(parent);
            return tap;
        }

        public DragController GetDrag(Transform parent)
        {
            var drag = dragPool.Get();
            drag.transform.SetParent(parent);
            return drag;
        }
    }
}
