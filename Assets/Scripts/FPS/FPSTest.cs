using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSTest : MonoBehaviour
{

    private Text fpsText;
    private float cooldown = 0.2f;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        fpsText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > cooldown)
        {
            timer = 0;

            var dt = Time.deltaTime;
            float rate = 1.0f / dt;

            fpsText.text = "FPS: " + rate.ToString("0");
        }
    }
}
