/* Script to dynamically set the play area sprites - Stretch the background and place the borders */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayArea : MonoBehaviour
{
    public Transform Background,BorderLeft, BorderRight, BorderBottom, BorderTop;

    Vector2 scale, screenBounds;

    private void Awake()
    {
        scale = SetSpriteScale(Background.GetComponent<SpriteRenderer>());
        SetBorderSprites();

        Background.localScale = scale;
        BorderLeft.localScale = BorderRight.localScale = new Vector3(BorderLeft.localScale.x, scale.y, BorderLeft.localScale.z);
        BorderTop.localScale = BorderBottom.localScale = new Vector3(scale.x, BorderBottom.localScale.y, BorderBottom.localScale.z);

    }

    /* Function to return the scale for the sprite as Vector2  to fit the entire screen*/
    Vector2 SetSpriteScale(SpriteRenderer spriteRenderer)
    {
        float camHeight = Camera.main.orthographicSize * 2;
        float camWidth = Camera.main.aspect * camHeight;
        Vector2 cameraSize = new Vector2(camWidth, camHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        float scalex = transform.localScale.x;
        float scaley = transform.localScale.y;
        scalex *= cameraSize.x / spriteSize.x;
        scaley *= cameraSize.y / spriteSize.y;

        return new Vector2(scalex, scaley);

    }

    /* Function to place the border sprites to the edge of the screen*/
    void SetBorderSprites()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        BorderLeft.localPosition = new Vector3(-screenBounds.x + BorderLeft.localScale.x, BorderLeft.localPosition.y, BorderLeft.localPosition.z);
        BorderRight.localPosition = new Vector3(screenBounds.x - BorderLeft.localScale.x, BorderRight.localPosition.y, BorderRight.localPosition.z);
        BorderBottom.localPosition = new Vector3(BorderBottom.localPosition.x, -screenBounds.y + BorderBottom.localScale.y, BorderBottom.localPosition.z);
        BorderTop.localPosition = new Vector3(BorderTop.localPosition.x, 0.65f * screenBounds.y, BorderTop.localPosition.z);

    }
}
