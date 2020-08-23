/* This script is attached to the Customer prefab */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [HideInInspector] public PlaceCustomers pc;
    [HideInInspector] public int maxSaladItem;                          //value set from PlaceCustomer.cs when customer prefab is initialized
    [HideInInspector] public int pos;                                   //value set from PlaceCustomer.cs when customer prefab is initialized
    float waitTimePerVegetable;                                         //time in seconds the customer will wait per item in his salad order. Default : 20
    public List<Sprite> VegetableSprites;
    public GameObject CustomerTimer;                                    //timer HUD object
    [HideInInspector] public float timeToDeduct;                        //time to deduct in coroutine. Default is 0.1f
    [HideInInspector] public List<string> combination;
    [HideInInspector] public bool angryWithPlayer1, angryWithPlayer2;   //this is set from PlayerController if the player provides incorrect combination.
    int playerDeductScore;
    void Start()
    {
        angryWithPlayer1 = angryWithPlayer2 = false;
        waitTimePerVegetable = 20f;
        playerDeductScore = 10;
        int rand = Random.Range(2, maxSaladItem+1); //selecting number of vegetables the customer demands in the salad. Minimum =2 max = set in PlaceCustomer.cs
        combination = new List<string>(rand);
        for(int i=0;i<rand;i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
            int r = Random.Range(0, VegetableSprites.Count);
            gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = VegetableSprites[r];
            combination.Add(VegetableSprites[r].name);
            VegetableSprites.RemoveAt(r);
        }
        StartCoroutine(StartWaitTimer(rand));
    }

    IEnumerator StartWaitTimer(int noOfItems)
    {
        float waitTime = waitTimePerVegetable * noOfItems;
        float time = waitTime ;
        while (time > 0)
        {
            CustomerTimer.transform.GetChild(0).GetComponent<Image>().fillAmount = time / waitTime;
            yield return new WaitForSeconds(0.1f);
            time -= timeToDeduct;
        }
        DestroyCustomerDeductPoints();       
    }

    public void DestroyCustomer()
    {
        pc.AddCustomerIndex(pos);   //free up the position for new customer to instansiate.
        Destroy(gameObject);
    }

    /*If the customer is not served, then we deduct points before destroying the customer. We check if any of the players have provided incorrect combination and deduct the
     * points accordingly*/
    void DestroyCustomerDeductPoints()
    {
        GameObject Player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject Player2 = GameObject.FindGameObjectWithTag("Player2");
        if (angryWithPlayer1) Player1.GetComponent<PlayerController>().DeductScore(playerDeductScore * 2);
        else Player1.GetComponent<PlayerController>().DeductScore(playerDeductScore);

        if (angryWithPlayer2) Player2.GetComponent<PlayerController>().DeductScore(playerDeductScore * 2);
        else Player2.GetComponent<PlayerController>().DeductScore(playerDeductScore);
        Destroy(gameObject);

    }



}
