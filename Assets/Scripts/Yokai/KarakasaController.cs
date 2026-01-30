using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KarakasaController : MonoBehaviour
{
    public ApplicationReferences appRef;
    private NavMeshAgent navMeshAgent;
    private AudioSource[] audioSources;

    [SerializeField] private float followInterval = 0.5f;
    private float lastFollowTime = 0f;
    [SerializeField] private float speed = 1f;

    [Tooltip("Duration of for despawn transition")]
    [SerializeField] private float despawnDuration = 1.2f;
    public Renderer[] _rends;

    [Header("Dissolve Settings")]
    public string propName = "_DissolveAmount";
    public Material[] materials;

    void Awake()
    {
        _rends = GetComponentsInChildren<Renderer>();
        materials = new Material[_rends.Length];
        for (int i = 0; i < _rends.Length; i++)
            materials[i] = _rends[i].material = new Material(_rends[i].material);
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSources = GetComponentsInChildren<AudioSource>();
        if (navMeshAgent == null)
        {
            Debug.LogError("[KarakasaController]: NavMeshAgent component not found.");
        }
    }

    private void FixedUpdate()
    {
        if (navMeshAgent)
        {
            if (Time.time - lastFollowTime > followInterval)
            {
                lastFollowTime = Time.time;
                navMeshAgent.SetDestination(appRef.camTrans.position);
                StartCoroutine(TurnToCamera());
            }
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void Dissolve()
    {
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator TurnToCamera()
    {
        // turn around to face the camera
        float dur = 0.8f;
        Quaternion start = navMeshAgent.gameObject.transform.rotation;
        Quaternion end = Quaternion.LookRotation(new Vector3(appRef.camTrans.position.x, navMeshAgent.gameObject.transform.position.y, appRef.camTrans.position.z) - navMeshAgent.gameObject.transform.position);
        float rotationTime = 0f;
        while (rotationTime < dur)
        {
            navMeshAgent.gameObject.transform.rotation = Quaternion.Slerp(start, end, rotationTime / dur);
            yield return null;
            rotationTime += Time.fixedDeltaTime;
        }
    }

    IEnumerator DissolveRoutine()
    {
        float t = 0;
        while (t < despawnDuration)
        {
            t += Time.deltaTime;
            float v = t / despawnDuration;
            foreach (var m in materials) m.SetFloat(propName, v);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutRoutine()
    {
        // Create per‑instance material copies so you don’t edit shared assets
        foreach (var r in _rends) r.material = new Material(r.material);

        float t = 0;
        while (t < despawnDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - t / despawnDuration;
            foreach (var r in _rends)
            {
                foreach (var m in r.materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        var c = m.color;
                        m.color = new Color(c.r, c.g, c.b, alpha);
                    }

                    if (m.HasProperty("_BaseColor")) // URP Lit
                    {
                        var c = m.GetColor("_BaseColor");
                        m.SetColor("_BaseColor", new Color(c.r, c.g, c.b, alpha));
                        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        m.SetFloat("_Surface", 1); // Ensure it’s transparent
                    }
                }
            }
            // reduce audio volume
            foreach (var aud in audioSources)
            {
                aud.volume = Mathf.Lerp(1f, 0f, t / despawnDuration);
            }
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
