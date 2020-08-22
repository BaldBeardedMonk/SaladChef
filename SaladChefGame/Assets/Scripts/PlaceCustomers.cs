using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This script handles placement of the customers dynamically based on the screen width and the maximum number of vegetable combination each customer can demand
  maxSaladItem : can be set from inspector
  maxCustomerPossible: is calculated based on the width of the screen/space required for individual customer
  customerPositions: is Vector2 which holds individual positions where customer can be placed. Again dynamically calculated based on screen size and max customer possible
  customerOccupiedIndex : is a list of integers which holds all available position to spawn the customer. When customer is spawned the position is removed from the list
                          and when customer is destroyed the position is added back into the list.*/

public class PlaceCustomers : MonoBehaviour
{
    public GameObject BackgroundSprite,Customer;
    [SerializeField]
    [Range(2,6)]
    public int maxSaladItem =3;                     //maximum number of combinations that a customer can order. Default is 3. Maximum is 6. To add more make changes to customer prefab
    int maxCustomersPossible;                       //maximum number of customers that can fit the screen. Also depends on the maximum combination a customer can have.
    int startPositionX, startPositionY;
    Vector2 [] customerPositions;
    [HideInInspector] public List<int> customerOccupiedIndex;   //the values in the list will correspond to free index positions
    
    float customerSpawnTime;                        //time in seconds before a new customer is spawned.

    void Start()
    {
        startPositionX = (int)Mathf.Round(BackgroundSprite.transform.localScale.x);
        startPositionY = (int)Mathf.Round(BackgroundSprite.transform.localScale.y);
        maxCustomersPossible = ((startPositionX*2)/ maxSaladItem) +1;
        customerOccupiedIndex = new List<int>(maxCustomersPossible);
        customerPositions = new Vector2[maxCustomersPossible];
        customerSpawnTime = 50f;

        for(int i=0; i<maxCustomersPossible; i++)
        {
            customerPositions[i] = new Vector2(-startPositionX + (i * maxSaladItem), startPositionY);
            customerOccupiedIndex.Add(i);       //initially the values will index of be all the free positions available.
        }

        /*We start by placing two customers. One at the left of the screen and one at the right of the screen. After that there will be a new customer spawned every 50 seconds
         * or when a customer is served or his timer runs out */  
        int r = Random.Range(0, customerOccupiedIndex.Count / 2);
        int pos = customerOccupiedIndex[r];
        PlaceCustomer(customerPositions[pos], pos);
        customerOccupiedIndex.Remove(pos);     //remove the index value from the list as this position is already occupied by a customer  
        r = Random.Range(customerOccupiedIndex.Count / 2, customerOccupiedIndex.Count);
        pos = customerOccupiedIndex[r];
        PlaceCustomer(customerPositions[pos], pos);
        customerOccupiedIndex.Remove(pos);      //remove the index value from the list as this position is already occupied by a customer
        InvokeRepeating("CallPlaceCustomer", customerSpawnTime, customerSpawnTime);

    }

    void PlaceCustomer(Vector2 customerPosition,int pos)
    {
        GameObject C = Instantiate(Customer, customerPosition, Quaternion.identity) as GameObject;
        C.GetComponent<CustomerController>().maxSaladItem = maxSaladItem;
        C.GetComponent<CustomerController>().pos = pos;
        C.GetComponent<CustomerController>().pc = gameObject.GetComponent<PlaceCustomers>();
    }

    public void AddCustomerIndex(int pos)
    {
        customerOccupiedIndex.Add(pos);
        Invoke("CallPlaceCustomer", 5f);
        
    }

    void CallPlaceCustomer()
    {
        if (customerOccupiedIndex.Count>0)
        {
            int r = Random.Range(0, customerOccupiedIndex.Count);
            int pos = customerOccupiedIndex[r];
            PlaceCustomer(customerPositions[pos], pos);
            customerOccupiedIndex.Remove(pos);  //remove the index value from the list as this position is already occupied by a customer
        }
    }


}
