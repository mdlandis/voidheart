using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    public static void ResizeToSize(this SpriteRenderer spriteRenderer, Vector2 targetSize) {
        spriteRenderer.transform.localScale = new Vector3(targetSize.x / spriteRenderer.bounds.size.x, targetSize.y / spriteRenderer.bounds.size.y, 1.0f);
    }
}