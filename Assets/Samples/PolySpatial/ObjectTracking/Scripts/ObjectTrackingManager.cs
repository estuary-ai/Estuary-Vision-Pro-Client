using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#else
using ARTrackedObjectManager = UnityEngine.Object;
using XRReferenceObjectLibrary = UnityEngine.Object;
#endif

namespace PolySpatial.Samples
{
    public class ObjectTrackingManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Object manager on the AR Session Origin")]
        ARTrackedObjectManager m_ObjectManager;

        [SerializeField]
        [Tooltip("Reference Object Library")]
        XRReferenceObjectLibrary m_ObjectLibrary;

        [SerializeField]
        [Tooltip("Prefabs to spawn")]
        GameObject[] m_PrefabsToSpawn;

        readonly Dictionary<Guid, GameObject> m_PrefabsToSpawnDictionary = new();
        readonly Dictionary<Guid, GameObject> m_SpawnedPrefabs = new();

        public Dictionary<Guid, GameObject> spawnedPrefabs => m_SpawnedPrefabs;

#if UNITY_INCLUDE_ARFOUNDATION
        void Awake()
        {
            var count = m_PrefabsToSpawn.Length;
            var objectCount = m_ObjectLibrary.count;
            if (count > objectCount)
                Debug.LogWarning($"Number of prefabs ({count}) exceeds the number of objects in the reference library ({objectCount})");

            count = Math.Min(count, objectCount);
            for (var i = 0; i < count; i++)
            {
                var guid = m_ObjectLibrary[i].guid;
                m_PrefabsToSpawnDictionary[guid] = m_PrefabsToSpawn[i];
            }
        }

        void OnEnable()
        {
            m_ObjectManager.trackablesChanged.AddListener(ObjectManagerOnTrackedObjectsChanged);
        }

        void OnDisable()
        {
            m_ObjectManager.trackablesChanged.RemoveListener(ObjectManagerOnTrackedObjectsChanged);
        }

        void ObjectManagerOnTrackedObjectsChanged(ARTrackablesChangedEventArgs<ARTrackedObject> objectTrackables)
        {
            // added, spawn prefab
            foreach (var obj in objectTrackables.added)
            {
                var guid = obj.referenceObject.guid;
                if (m_PrefabsToSpawnDictionary.TryGetValue(guid, out var prefab))
                {
                    var objectTransform = obj.transform;
                    var spawnedPrefab = Instantiate(prefab, objectTransform.position, objectTransform.rotation);
                    m_SpawnedPrefabs[guid] = spawnedPrefab;
                }
            }

            // updated, set prefab position and rotation
            foreach (var obj in objectTrackables.updated)
            {
                var guid = obj.referenceObject.guid;

                // If the object is tracking, update its position and show its visuals
                var isTracking = obj.trackingState == TrackingState.Tracking;
                if (isTracking && m_SpawnedPrefabs.TryGetValue(guid, out var spawnedPrefab))
                {
                    var spawnedPrefabTransform = spawnedPrefab.transform;
                    var objectTransform = obj.transform;
                    spawnedPrefabTransform.SetPositionAndRotation(objectTransform.position, objectTransform.rotation);
                }

                // Remove the object if we get limited tracking, Apple won't send a removed event, so this is the only signal we will get.
                var limitedTRacking = obj.trackingState == TrackingState.Limited;
                if (limitedTRacking)
                {
                    if (m_SpawnedPrefabs.TryGetValue(guid, out spawnedPrefab))
                    {
                        var spawnedPrefabGo = spawnedPrefab.gameObject;
                        Destroy(spawnedPrefabGo);
                        m_SpawnedPrefabs.Remove(guid);
                    }
                }
            }

            // removed, destroy spawned instance
            foreach (var obj in objectTrackables.removed)
            {
                var guid = obj.Value.referenceObject.guid;
                if (m_SpawnedPrefabs.TryGetValue(guid, out var spawnedPrefab))
                {
                    Destroy(spawnedPrefab);
                    m_SpawnedPrefabs.Remove(guid);
                }
            }
        }
#endif
    }
}

