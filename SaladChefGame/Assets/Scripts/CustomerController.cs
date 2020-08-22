using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [HideInInspector] public PlaceCustomers pc;
    [HideInInspector] public int maxSaladItem;      //value set from PlaceCustomer.cs when customer prefab is initialized
    [HideInInspector] public int pos;               //value set from PlaceCustomer.cs when customer prefab is initialized
    float waitTimePerVegetable;                     //time in seconds the customer will wait per item in his salad order. Default : 20
    public List<Sprite> VegetableSprites;
    public GameObject CustomerTimer;                //timer HUD object
    [HideInInspector] public float timeToDeduct;    //time to deduct in coroutine. Default is 0.1f
    void Start()
    {
        waitTimePerVegetable = 20f;
        timeToDeduct = 0.1f;
        int rand = Random.Range(2, maxSaladItem+1); //selecting number of vegetables the customer demands in the salad. Minimum =2 max = set in PlaceCustomer.cs
        for(int i=0;i<rand;i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
            int r = Random.Range(0, VegetableSprites.Count);
            gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = VegetableSprites[r];
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
            yield return new WaitForSeconds(timeToDeduct);
            time -= timeToDeduct;
        }
        pc.AddCustomerIndex(pos);   //free up the position for new customer to instansiate.
        Destroy(gameObject);
    }


}
