using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/* Script to control player movement and actions using Unity's new input system */

public class PlayerController : MonoBehaviour
{
    public GameObject ChoppingBoard, Plate;
    public GameObject PlayerChoppingMeter;

    Vector2 input_movement;
    bool pickUpAllowed = false;                                 // true when player trigger is overlapping vegetable/chopping board/plate
    bool putDownAllowed = false;                                // true when player trigger is overlapping chopping board/plate/trash/customer
    bool isChopping = false;                                    // true when chopping is taking place
    string combination;                                         // the combination on the chopboard
    Sprite pickedVegetableSprite,putDownVegetableSprite;
    const int maxPickUpAllowed = 2;                             // maximum vegetables player can pick up at once *IMP:if you change this please make corresponding changes to the player prefab children
    const int maxPutDownAllowed = 6;                            // maximum vegetables that can be placed on the chopping board *IMP:if you change this please make corresponding changes to the choppingboard prefab children
    int pickedUpCount;                                          // values can be 0-maxPickUpAllowed for number of items the player has currently picked up.
    int putDownCount;                                           // values can be 0-maxPutDownAllowed for the number of items placed on the chopping board.

    [SerializeField]
    float moveSpeed = 5f;
    float chopTime  = 5f;


    private void Update()
    {
        if(!isChopping) Move();
    }

    void Move()
    {
        Vector2 movement = new Vector2(input_movement.x, input_movement.y)*moveSpeed*Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnMove(InputValue inputValue)                  //Function to capture input from the new input system
    {
        input_movement = inputValue.Get<Vector2>();
    }

    private void OnPickUp()
    {
        
        if(pickUpAllowed && !isChopping && pickedUpCount<maxPickUpAllowed)  
        {
            transform.GetChild(pickedUpCount).GetComponent<SpriteRenderer>().sprite = pickedVegetableSprite;
            pickedVegetableSprite = null;
            if(pickedUpCount<maxPickUpAllowed)  pickedUpCount++;
        }
    }

    private void OnPutDown()
    {
        if (putDownAllowed && !isChopping && putDownCount < maxPutDownAllowed && pickedUpCount > 0)
        {
            
            ChoppingBoard.transform.GetChild(putDownCount).GetComponent<SpriteRenderer>().sprite = putDownVegetableSprite;
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = putDownVegetableSprite = null;
            if(putDownCount<maxPutDownAllowed)  putDownCount++;
            if (pickedUpCount > 0) pickedUpCount--;
            for(int i = 0;i<pickedUpCount;i++) //rearraning the pickedup sprite on player HUD after the 1st one is kept on the chopping board.
            {
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite;
                gameObject.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite = null;
            }
            StartCoroutine(StartChopping());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        #region Pickup trigger stay logic
        if (other.tag=="Vegetables")
        {
            pickUpAllowed = true;
            pickedVegetableSprite = other.GetComponent<SpriteRenderer>().sprite;
        }

        #endregion

        #region PutDown trigger stay logic

        if(other.tag==ChoppingBoard.tag)
        {
         
            putDownAllowed = true;
            putDownVegetableSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
          
        }

        #endregion
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        #region Pickup trigger exit logic
        if (other.tag == "Vegetables")
        {
            pickUpAllowed = false;
        }
        #endregion

        #region PutDown trigger exit logic
        if (other.tag == ChoppingBoard.tag)
        {
            putDownAllowed = false;
        }
        #endregion

    }

    IEnumerator StartChopping()
    {
        PlayerChoppingMeter.SetActive(true);
        float time = chopTime;
        isChopping = true;
        while(time>0)
        {
            PlayerChoppingMeter.transform.GetChild(0).GetComponent<Image>().fillAmount = 1 - time / chopTime;
            yield return new WaitForSeconds(0.1f);
            time-=0.1f;
        }
        isChopping = false;
        PlayerChoppingMeter.SetActive(false);
    }
}
