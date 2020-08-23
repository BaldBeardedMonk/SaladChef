/* Script to control player movement and actions using Unity's new input system */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject ChoppingBoard, Plate;
    public GameObject PlayerChoppingMeter,FinalSalad;
    GameObject CustomerGameObject;
    public GameController gameController;

    Vector2 input_movement;
    bool isChopping = false;                                    // true when chopping is taking place
    List<string> combination = new List<string>();              // the combination on the chopboard
    List<string> customerCombination = new List<string>();      // the combination ordered by the particular customer
    Sprite pickedVegetableSprite,putDownVegetableSprite;
    int maxPickUpAllowed;                                       // maximum vegetables player can pick up at once
    int maxPutDownAllowed;                                      // maximum vegetables that can be placed on the chopping board
    int pickedUpCount;                                          // values can be 0-maxPickUpAllowed for number of items the player has currently picked up.
    int putDownCount;                                           // values can be 0-maxPutDownAllowed for the number of items placed on the chopping board.
    int putDownCase;                                            // values can be 1-4 : 1= chopping board, 2= plate, 3 = trash 4= customer
    int pickUpCase;                                             // values can be 1-3 : 1= vegetable, 2= salad, 3 =plate (tentative)
    bool isSaladPickedUp = false;
    float playerTime;
    int playerScore;                                           
    public Text playerTimeText, playerScoreText;
    int playerGiveScore;

    [SerializeField]
    float moveSpeed;
    float chopTime  = 5f;


    private void Start()
    {
        playerTime = gameController.gameTime;
        playerTimeText.text = "Time:" + playerTime.ToString("F0");
        playerGiveScore = 20;
        StartCoroutine(Timer());
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
            case 4:
                CheckCustomerPutDown();
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
            combination.Add(putDownVegetableSprite.name);
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
            ClearPlayerSalad();
            DeductScore(5);
        }
    }

    /*Function to check if correct salad is given to the customer*/
    void CheckCustomerPutDown()
    {
        if(isSaladPickedUp)
        {
            combination.Sort();customerCombination.Sort();      //sorting both the lists for comparison
            if (combination.Count==customerCombination.Count)
            {
                for (int i = 0; i < combination.Count; i++)
                {
                    if (combination[i] != customerCombination[i])
                    {
                        Debug.Log("Incorrect combination supplied");
                        break;
                    }
                    GiveScore();
                    Debug.Log("Correct combination supplied");
                    ClearPlayerSalad();
                    CustomerGameObject.GetComponent<CustomerController>().DestroyCustomer();
                }
            }
            else
            {
                if (gameObject.tag == "Player1") CustomerGameObject.GetComponent<CustomerController>().angryWithPlayer1 = true;
                if (gameObject.tag == "Player2") CustomerGameObject.GetComponent<CustomerController>().angryWithPlayer2 = true;
                CustomerGameObject.GetComponent<CustomerController>().timeToDeduct = CustomerGameObject.GetComponent<CustomerController>().timeToDeduct*2;
                ClearPlayerSalad();
                Debug.Log("Incorrect combination supplied bc");
            }
        }
    }

    void ClearPlayerSalad()
    {
        combination.Clear();
        isSaladPickedUp = false;
        for (int i = 0; i < ChoppingBoard.transform.childCount; i++)
        {
            FinalSalad.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        }
        FinalSalad.SetActive(false);
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


    /*TriggerStay function will contain all the different cases when a player presses pickup or putdown, depending on the trigger in contact, corresponding function is called*/
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
        if(other.tag=="Customer")
        {
            putDownCase = 4;
            customerCombination = other.GetComponent<CustomerController>().combination;
            CustomerGameObject = other.gameObject;
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

    /*Individual timer for each player */
    IEnumerator Timer()
    {
        while(playerTime>0)
        {
            yield return new WaitForSeconds(1f);
            playerTime--;
            playerTimeText.text = "Time:" + playerTime.ToString("F0");
        }
        /* Code to check which player's time is over and change the corresponding variable to true in Gamecontroller.cs*/
        if(gameObject.tag=="Player1")
        {
            gameController.player1TimeUp = true;
            gameController.player1Score = playerScore;
        }
        if(gameObject.tag=="Player2")
        {
            gameController.player2TimeUp = true;
            gameController.player2Score = playerScore;
        }
        gameController.CheckGameOver();
    }

    /* This is called from within this script, the triggering point is when the player provides correct combination*/
    void GiveScore()
    {
        playerScore += combination.Count * playerGiveScore;
        playerScoreText.text = "Score:" + playerScore.ToString();

    }

    /*This is always called from the customer script, because the triggering point is when the customer leaves without being served*/
    public void DeductScore(int score)
    {
        playerScore -= score;
        playerScoreText.text = "Score:" + playerScore.ToString();
    }

}
