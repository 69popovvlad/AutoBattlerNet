using UnityEngine;

namespace Client.Services.Pool
{
    public abstract class PoolMonoBehaviour<T> : Pool<T> where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;

        [SerializeField, Tooltip("optional parent")]
        private Transform _parentForPooled;

        [SerializeField] private bool _deactivateOnReturn = true;

        protected override void Awake()
        {
            if (_prefab == null)
            {
                Debug.LogError($"[{nameof(PoolMonoBehaviour<T>)}] Prefab is not assigned for {typeof(T).Name}");
            }

            base.Awake();
        }


        /// Instantiate prefab
        protected override T CreateInstance()
        {
            if (_prefab == null)
            {
                return null;
            }

            var go = Instantiate(_prefab, _parentForPooled);
            if (_deactivateOnReturn)
            {
                go.gameObject.SetActive(true); // created items are active when rented
            }

            return go;
        }

        protected override void OnReturnedToPool(T item)
        {
            base.OnReturnedToPool(item);
            if (item == null)
            {
                return;
            }

            if (_deactivateOnReturn)
            {
                item.gameObject.SetActive(false);
            }

            // move to parent
            if (_parentForPooled != null)
            {
                item.transform.SetParent(_parentForPooled, false);
            }
        }

        protected override void OnDestroyReturned(T item)
        {
            if (item == null)
            {
                return;
            }

            Destroy(item.gameObject);
        }
    }
}