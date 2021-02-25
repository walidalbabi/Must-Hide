using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FootPrints : MonoBehaviour
{

    [SerializeField]
    private bool footPrints;

    public float LifeTime;
    float startTime;
    SpriteRenderer sprite;
    SpriteMask mask;
    Color color;
    bool dying;
    // Start is called before the first frame update
    void Start()
    {
        mask = GetComponent<SpriteMask>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite)
            color = sprite.color;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > LifeTime && !dying)
        {
            dying = true;
            StartCoroutine(Fade());
        }
    }
    IEnumerator Fade()
    {
        float value = 1;
        while (value > 0)
        {
            value -= Time.deltaTime * 0.05f *10f;
            if (sprite)
                sprite.color = new Color(color.r, color.g, color.b, value);

            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

}
