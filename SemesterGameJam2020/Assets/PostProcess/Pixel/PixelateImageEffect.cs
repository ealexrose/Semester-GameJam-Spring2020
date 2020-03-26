﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelateImageEffect : MonoBehaviour
{

    public Material effectmaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRenderImage(RenderTexture src,RenderTexture dst )
    {
        Graphics.Blit(src, dst, effectmaterial);
    }
}
