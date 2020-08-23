/* Script to dynamically place the vegetables on the shelfs
 * Set the vegetable sprites size in the inspector - should be between 2-10
 * Drag the individual vegetable sprites in the inspector */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceVegetables : MonoBehaviour
{
    public GameObject Vegetable;
    public GameObject LeftBorder, RightBorder;
    [Tooltip("Value between 2 to 10 ")]
    public Sprite[] VegetableSprites;
    Sprite tempSprite;
    float distance;

    void Start()
    {
        distance = VegetableSprites[0].bounds.size.y * 2f / 10f;    // Distance between vegetables placed on the shelf - calculated based on the sprite size and the local scale
        Shuffle();                                                  // This creates variation in the game where the order of vegetables on the shelf changes in every play
        PlaceVegetableOnShelf(LeftBorder,0, VegetableSprites.Length / 2);
        PlaceVegetableOnShelf(RightBorder, VegetableSprites.Length / 2, VegetableSprites.Length);

    }

    void Shuffle()                                                  // Function to shuffle the array of vegetable sprites
    {
        for(int i=0;i<VegetableSprites.Length;i++)
        {
            int rand = Random.Range(0, VegetableSprites.Length);
            tempSprite = VegetableSprites[rand];
            VegetableSprites[rand] = VegetableSprites[i];
            VegetableSprites[i] = tempSprite;
        }
    }

    void PlaceVegetableOnShelf(GameObject Border,int start, int end)
    {
        int counter = 0;
        for (int i = start; i < end; i++)
        {
            GameObject G = Instantiate(Vegetable) as GameObject;
            G.GetComponent<SpriteRenderer>().sprite = VegetableSprites[i];
            G.transform.parent = Border.transform;
            G.transform.localPosition = new Vector3(0f, 0.4f-(counter++*distance),G.transform.localPosition.z);
        }
    }

}
