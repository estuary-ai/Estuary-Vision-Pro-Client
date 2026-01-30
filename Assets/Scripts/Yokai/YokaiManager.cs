using System.Collections;
using UnityEngine;

public class YokaiManager : MonoBehaviour
{
   public ApplicationReferences appRef;
   [SerializeField] private GameObject karakasa;
   [SerializeField] private GameObject zashiki;

   [Header ("Timings")]
   [SerializeField] private float karakasaSpawnDelay = 10f;
   [SerializeField] private float karakasaDuration = 10f;
   [SerializeField] private float beginMischiefDelay = 3f;
   [SerializeField] private float zashikiSpawnDelay = 10f;
   [SerializeField] private float zashikiDuration = 10f;

   [Header("Sounds")]
   [SerializeField] private AudioClip spawnSound;
   [SerializeField] private AudioClip despawnSound;

   private AudioSource audioSource;

   private void Start()
   {
      audioSource = GetComponent<AudioSource>();
      if (audioSource == null)
      {
         Debug.LogError("[YokaiManager]: AudioSource component not found.");
      }
      StartCoroutine(SpawnKarakasa(karakasaSpawnDelay));
   }

   public void SummonKarakasa()
   {
      StartCoroutine(SpawnKarakasa(1f));
   }

   public void SummonZashiki()
   {
      StartCoroutine(SpawnZashiki(1f));
   }

   /// <summary>
   /// Make the Karakasa appear at the given position.
   /// </summary>
   private void KarakasaAppearBehindUser()
   {
      karakasa.SetActive(true);
      // Set the position of the Karakasa to be behind the user
      Vector3 newPos = appRef.camTrans.position - appRef.camTrans.forward * 1f;
      newPos.y = appRef.camTrans.position.y;
      karakasa.transform.position = newPos;
   }

   private void ZashikiAppearNearUser()
   {
      zashiki.SetActive(true);
      // Set the position of the Zashiki to be near a sphere around the user
      Vector3 newPos = appRef.camTrans.position + Random.insideUnitSphere * 1f;
      newPos.y = appRef.camTrans.position.y + Random.Range(0f, 1f);
      zashiki.transform.position = newPos;
   }

   private IEnumerator SpawnKarakasa(float seconds)
   {
      yield return new WaitForSeconds(seconds-1);
      if(audioSource != null) audioSource.PlayOneShot(spawnSound);
      yield return new WaitForSeconds(1f);
      KarakasaAppearBehindUser();
      StartCoroutine(DespawnKarakasa(karakasaDuration));
   }

   private IEnumerator SpawnZashiki(float seconds)
   {
      yield return new WaitForSeconds(seconds-1);
      if(audioSource != null) audioSource.PlayOneShot(spawnSound);
      yield return new WaitForSeconds(1f);
      ZashikiAppearNearUser();
      StartCoroutine(DespawnZashiki(zashikiDuration));
   }

   private IEnumerator DespawnKarakasa(float seconds)
   {
      yield return new WaitForSeconds(seconds-1);
      if(audioSource != null) audioSource.PlayOneShot(despawnSound);
      yield return new WaitForSeconds(1f);
      karakasa.GetComponent<KarakasaController>().FadeOut();
      yield return new WaitForSeconds(beginMischiefDelay);
      appRef.mischiefManager.EnableMischiefs();
      StartCoroutine(SpawnZashiki(zashikiSpawnDelay));
   }

   private IEnumerator DespawnZashiki(float seconds)
   {
      yield return new WaitForSeconds(seconds-1);
      if(audioSource != null) audioSource.PlayOneShot(despawnSound);
      yield return new WaitForSeconds(1f);
      zashiki.SetActive(false);
   }
}
