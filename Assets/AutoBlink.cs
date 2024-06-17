using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBlink : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    private int blinkIndexL;
    private int blinkIndexR;
    private int squintIndexL;
    private int squintIndexR;
    private int doubleEyelidUpIndexL;
    private int doubleEyelidUpIndexR;
    private int cheekRaiseIndexL;
    private int cheekRaiseIndexR;

    [Header("Properties")]
    public float intervalRandomizer     = 1.2f;  //randomize the blink interval
    public float blinkInterval          = 5.0f;  //time until the next blink
    public float blinkEyeCloseDuration  = 0.09f; //how long the eye stays closed during blinking
    public float openingRandomizer      = 0.08f; //randomize the blink interval
    public float blinkOpeningSeconds    = 0.2f;  //the speed of eye opening in the animation
    public float blinkClosingSeconds    = 0.1f;  //the speed of eye closing in the animation

    public Coroutine blinkCoroutine;

    private void Awake()
    {
        blinkIndexL = GetBlendshapeIndex("Eye_Blink_L");
        blinkIndexR = GetBlendshapeIndex("Eye_Blink_R");
        squintIndexL = GetBlendshapeIndex("Eye_Squint_L");
        squintIndexR = GetBlendshapeIndex("Eye_Squint_R");
        doubleEyelidUpIndexL = GetBlendshapeIndex("double_eyelid_up_L");
        doubleEyelidUpIndexR = GetBlendshapeIndex("double_eyelid_up_R");
        cheekRaiseIndexL = GetBlendshapeIndex("Cheek_Raise_L");
        cheekRaiseIndexR = GetBlendshapeIndex("Cheek_Raise_R");
    }

    private int GetBlendshapeIndex(string blendshapeName)
    {
        Mesh mesh = skinnedMesh.sharedMesh;
        return mesh.GetBlendShapeIndex(blendshapeName);;
    }

    private IEnumerator BlinkRoutine()
    {
        //This is an infinite loop coroutine
        while (true)
        {
            //Wait until we need to blink
            // slightly randomize the blink interval
            var tempInterval = blinkInterval + Random.Range(0, intervalRandomizer);
            Debug.Log("tempInterval: " + tempInterval);
            yield return new WaitForSeconds(tempInterval);

            //Close eyes
            var value = 0f;
            var closeSpeed = 1.0f / blinkClosingSeconds;
            while (value < 1)
            {
                skinnedMesh.SetBlendShapeWeight(blinkIndexL, value * 100);
                skinnedMesh.SetBlendShapeWeight(blinkIndexR, value * 100);
                skinnedMesh.SetBlendShapeWeight(squintIndexL, value * 20);
                skinnedMesh.SetBlendShapeWeight(squintIndexR, value * 20);
                skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexL, value * 60);
                skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexR, value * 60);
                skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexL, value * 10);
                skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexR, value * 10);
                value += Time.deltaTime * closeSpeed;
                yield return null;
            }
            skinnedMesh.SetBlendShapeWeight(blinkIndexL, 100);
            skinnedMesh.SetBlendShapeWeight(blinkIndexR, 100);
            skinnedMesh.SetBlendShapeWeight(squintIndexL, 20);
            skinnedMesh.SetBlendShapeWeight(squintIndexR, 20);
            skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexL, 60);
            skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexR, 60);
            skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexL, 10);
            skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexR, 10);

            //Wait to open our eyes
            yield return new WaitForSeconds(blinkEyeCloseDuration);

            //Open eyes
            value = 1f;
            var tempOpening =  blinkOpeningSeconds + Random.Range(0, openingRandomizer);
            Debug.Log("temp opening: " + tempOpening);
            var openSpeed = 1.0f / tempOpening;
            while (value > 0)
            {
                skinnedMesh.SetBlendShapeWeight(blinkIndexL, value * 100);
                skinnedMesh.SetBlendShapeWeight(blinkIndexR, value * 100);
                skinnedMesh.SetBlendShapeWeight(squintIndexL, value * 20);
                skinnedMesh.SetBlendShapeWeight(squintIndexR, value * 20);
                skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexL, value * 60);
                skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexR, value * 60);
                skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexL, value * 10);
                skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexR, value * 10);
                value -= Time.deltaTime * openSpeed;
                yield return null;
            }
            skinnedMesh.SetBlendShapeWeight(blinkIndexL, 0);
            skinnedMesh.SetBlendShapeWeight(blinkIndexR, 0);
            skinnedMesh.SetBlendShapeWeight(squintIndexL, 0);
            skinnedMesh.SetBlendShapeWeight(squintIndexR, 0);
            skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexL, 0);
            skinnedMesh.SetBlendShapeWeight(doubleEyelidUpIndexR, 0);
            skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexL, 0);
            skinnedMesh.SetBlendShapeWeight(cheekRaiseIndexR, 0);
        }
    }

    private void OnEnable()
    {
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }
}