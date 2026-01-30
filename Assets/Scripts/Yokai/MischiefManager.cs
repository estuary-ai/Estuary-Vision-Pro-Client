using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MischiefManager : MonoBehaviour
{
    [SerializeField] private ApplicationReferences appRef;
    [SerializeField] private List<GameObject> mischiefPrefabs;
    [SerializeField] private float timeBetweenMischiefs = 3f;

    private bool isMischiefEnabled;
    void Start()
    {
        StartCoroutine(SpawnMischiefCoroutine(timeBetweenMischiefs));
    }

    public void EnableMischiefs()
    {
        isMischiefEnabled = true;
    }

    private void SpawnMischief()
    {
        if (mischiefPrefabs.Count == 0 || !isMischiefEnabled) return;

        int randomIndex = Random.Range(0, mischiefPrefabs.Count);
        GameObject mischiefPrefab = mischiefPrefabs[randomIndex];
        // spawn position within 1m of the camera
        Vector3 spawnPosition = appRef.camTrans.position + Random.insideUnitSphere;
        spawnPosition.y = appRef.camTrans.position.y + Random.Range(0.5f, 1f);
        // random rotation
        Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        Instantiate(mischiefPrefab, spawnPosition, spawnRotation);
    }

    private IEnumerator SpawnMischiefCoroutine(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            SpawnMischief();
        }
    }
}
