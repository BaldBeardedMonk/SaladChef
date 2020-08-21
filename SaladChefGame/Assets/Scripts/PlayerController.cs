using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/* Script to control player movement and actions using Unity's new input system */

public class PlayerController : MonoBehaviour
{
    public GameObject ChoppingBoard, Plate;
    public GameObject PlayerChoppingMeter,FinalSalad;

    Vector2 input_movement;
    bool isChopping = false;                                    // true when chopping is taking place
    string combination;                                         // the combination on the chopboard
    Sprite pickedVegetableSprite,putDownVegetableSprite;
    int maxPickUpAllowed;                                       // maximum vegetables player can pick up at once
    int maxPutDownAllowed;                                      // maximum vegetables that can be placed on the chopping board
    int pickedUpCount;                                          // values can be 0-maxPickUpAllowed for number of items the player has currently picked up.
    int putDownCount;                                           // values can be 0-maxPutDownAllowed for the number of items placed on the chopping board.
    int putDownCase;                                            // values can be 1-4 : 1= chopping board, 2= plate, 3 = trash 4= customer
    int pickUpCase;                                             // values can be 1-3 : 1= vegetable, 2= salad, 3 =plate
    bool isSaladPickedUp = false;

    [SerializeField]
    float moveSpeed = 5f;
    float chopTime  = 5f;


    private void Start()
    {
        maxPutDownAllowed = ChoppingBoard.transform.childCount;
        for(int i=0;i<gameObject.transform.childCount;i++)
        {
            //since player prefab also has other children apart from pickup hud elements - in future one can just modify the prefab to allow players to pickup more vegetables at once
            if (gameObject.transform.GetChild(i).tag == "PlayerPickUpHud") maxPickUpAllowed++;
        }
            
    }

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
        /* Possible pickup cases : 1 - Vegetable 2 - Salad 3 - Plate */
        switch (pickUpCase)
        {
            case 1:
                CheckVegetablePickup();
                break;
            case 2:
                CheckPickUpSalad();
                break;
            default:
                break;

        }

    }

    private void OnPutDown()
    {
        /* Possible putdown cases : 1 - Chopping board 2 - Plate 3 - Trash 4- Customer */
        switch (putDownCase)
        {
            case 1:
                CheckChoppingBoardPutDown();
                break;
            case 2:
                CheckPlatePutDown();
                break;
            case 3:
                CheckTrashBoxPutDown();
                break;
            default:
                break;

        }
    }

    /*Function to check if vegetable can be picked up by the player and perform the necessary action */
    void CheckVegetablePickup()
    {
        if(!IsPlayerPickUpFull() && !isSaladPickedUp)
        {
            transform.GetChild(pickedUpCount).GetComponent<SpriteRenderer>().sprite = pickedVegetableSprite;
            pickedVegetableSprite = null;
            pickedUpCount++;
        }
    }

    /*Function to check if salad can be picked up by the player and perform the necessary action */
    void CheckPickUpSalad()
    {
        if(IsPlayerPickUpEmpty() && !isChopping && !isSaladPickedUp)
        {
            isSaladPickedUp = true;
            putDownCount = 0;
            FinalSalad.SetActive(true);
            for (int i = 0; i < ChoppingBoard.transform.childCount; i++)
            {
                FinalSalad.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = ChoppingBoard.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                ChoppingBoard.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }

    /*Function to check if vegetable can be put down on the chopping board by the player and perform the necessary action */
    void CheckChoppingBoardPutDown()
    {
        if(!IsPlayerPickUpEmpty() && !isChopping && !IsChoppingBoardFull())
        {
            combination += putDownVegetableSprite.name;
            ChoppingBoard.transform.GetChild(putDownCount).GetComponent<SpriteRenderer>().sprite = putDownVegetableSprite;
            putDownVegetableSprite = null;
            putDownCount++;
            pickedUpCount--;
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            for (int i = 0; i < pickedUpCount; i++)     //rearraning the pickedup sprite on player HUD after the 1st one is kept on the chopping board.
            {
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite;
                gameObject.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite = null;
            }
            StartCoroutine(StartChopping());
        }
    }

    /*Function to check if vegetable can be put down on the plate by the player and perform the necessary action */
    void CheckPlatePutDown()
    {
        Debug.Log("Inside Plate PutDown");
        if(!IsPlayerPickUpEmpty() && !IsPlateFull())
        {
            Plate.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = putDownVegetableSprite;
            pickedUpCount--;
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            for (int i = 0; i < pickedUpCount; i++)     //rearraning the pickedup sprite on player HUD after the 1st one is kept on the chopping board.
            {
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite;
                gameObject.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }

    /*Function to check if salad can be thrown in trash by the player and perform the necessary action */
    void CheckTrashBoxPutDown()
    {
        if(isSaladPickedUp)
        {
            combination = "";
            isSaladPickedUp = false;
            for (int i = 0; i < ChoppingBoard.transform.childCount; i++)
            {
                FinalSalad.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
            }
            FinalSalad.SetActive(false);
        }
    }

    /* Function that returns if plate is full or not - useful to check if vegetable can be placed on the plate */
    bool IsPlateFull()
    {
        for(int i=0;i<Plate.transform.childCount;i++)
        {
            if (Plate.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite == null) return false;
        }
        return true;
    }

    /* Function that returns if chopping board is full or not - useful to check if vegetable can be placed on the chopping board */
    bool IsChoppingBoardFull()
    {
        for (int i = 0; i < ChoppingBoard.transform.childCount; i++)
        {
            if (ChoppingBoard.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite == null) return false;
        }
        return true;
    }

    /* Function that returns if player pick up capacity is full or not - useful to check if vegetable can be picked up by the player */
    bool IsPlayerPickUpFull()
    {
        if (pickedUpCount < maxPickUpAllowed) return false;
        else return true;
    }

    /* Function that returns if player pick up is empty or not - useful to check if player has a vegetable before putting it down */
    bool IsPlayerPickUpEmpty()
    {
        if (pickedUpCount == 0) return true;
        else return false;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        #region Pickup trigger stay logic
        if (other.tag=="Vegetables")
        {
            pickUpCase = 1;
            pickedVegetableSprite = other.GetComponent<SpriteRenderer>().sprite;
        }
        if (other.tag==ChoppingBoard.tag)
        {
            pickUpCase = 2;
        }
        #endregion

        #region PutDown trigger stay logic
        if(other.tag==ChoppingBoard.tag)
        {

            putDownCase = 1;
            putDownVegetableSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;        
        }
        if(other.tag==Plate.tag)
        {
            putDownCase = 2;
            putDownVegetableSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        if(other.tag=="TrashBox")
        {
            putDownCase = 3;
        }
        #endregion
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        pickUpCase = 0;
        putDownCase = 0;
       
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
