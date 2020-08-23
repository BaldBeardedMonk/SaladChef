# SaladChef
Programmer version of Salad Chef simulation in Unity; using Unity's new input control system.



# Features
1. Local multiplayer (2 player) using Unity's new input control system. Both players can play using keyboard
2. Easily extensible - Follow the instructions in the scripts to modify parameters such as
    * No of vegetables spawned
    * No of vegetables the player can pick up at once
    * Maximum number of vegetables allowed on the chopping board at once
    * Maximum combination the customer can order
    * Player speed, chopping time, customer wait time, scoring system
    
# Scripts
* WelcomeScript.cs - It is the first script to be called and shows the welcome panel which gives details of the controls.

* CustomerController.cs - Contains the code specific to the individual customer i.e his salad combination, his wait time, and if he is angry or not.

* GameController.cs - Script to check game over code and display the final panel along with player scores and the winner.

* PlaceCustomer.cs - Contains the available positions for customers to spawn. Spawns the customer and sets the corresponding parameters for each of them. 

* PlaceVegetables.cs - Places the vegetables on the shelf for players to pick up. Currently supports min 2 and maximum 10 vegetables can be placed.

* PlayerController.cs - The most important script where the player movements and actions are coded. Player interactions(pickup/putdown) with various elements, player time and player score are coded in this script.

* SePlayArea.cs - Dynamically stretch the play area to the size of the screen. All other placements of objects and prefabs and calculated based on the play area.

# Controls
* Player 1 : Movement - WASD.  Pickup - Tab.   PutDown - LeftShift.
* Player 2 : Movement - Arrow. Pickup - Enter. PutDown - RightShift.

# TODO
1. Implement the PowerUps - The trigger logic for PowerUp pickups will be coded in the script attached to the powerup. It will check if the player is allowed to pickup the power and then modify the required parameter(Player speed, player time or player score) based on the powerup.

2. Implement HighScore : If the player score is in top 10, ask for the player name after the game is over. Store it in a local file and read from it whenever required.

3. Another fun enhancement can be to enable the players to collide with each other, this can lead to a new silly/fun dimension in the game where they can stop each other from moving around.
