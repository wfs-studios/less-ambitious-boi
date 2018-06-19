using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 50; 										//Number of columns in our game board.
		public int rows = 10;											//Number of rows in our game board.
		public Count wallCount = new Count (10, 30);					//Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 6);						//Lower and upper limit for our random number of food items per level.
		public GameObject exit;											//Prefab to spawn for exit.
		public GameObject floorTile;									//floor prefab.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] innocentTiles;								//Array of enemy prefabs.
        public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
        public GameObject[] outerWallTiles;								//Array of outer tile prefabs.
        public Sprite[] floorSprites;
        public int randomizer;
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
        private bool start = true;
		
		
		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 0; x < columns; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 0; y < rows; y++)
				{
					if(!(x == 0 && y == 0))
                       //At each index add a new Vector3 to our list with the x and y coordinates of that position.
					   gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;
			
            randomizer = (int)Random.Range(0, 3);
			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for(int x = -6; x < columns + 6; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -2; y < rows + 3; y++)
				{
                    GameObject toInstantiate = new GameObject();

                    floorTile.GetComponent<SpriteRenderer>().sprite = floorSprites[randomizer];
                    //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.


                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    if (x <= -1 || y <= -1 || x >= columns || y >= rows)
                        toInstantiate = outerWallTiles[randomizer];
                    else
                    {
                        toInstantiate = floorTile;
                        //floorMap.Add(new FloorMap(x, y, 0f, toInstantiate));
                    }

                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                    GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
					instance.transform.SetParent (boardHolder);
				}
			}
        }
		
		
		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomSpawn();
                randomizer = (int)Random.Range(0, 2);
                //Choose a random tile from tileArray and assign it to tileChoice
                GameObject tileChoice = tileArray[randomizer];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

        void SpawnBullet(GameObject[] tileArray, int minimum, int maximum)
        {
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            int objectCount = 13;
            Vector3 randomPosition;
            GameObject tileChoice;
            //Instantiate objects until the randomly chosen limit objectCount is reached
            for (int i = 0; i < objectCount; i++)
            {
                if (start)
                {
                    //Choose a random tile from tileArray and assign it to tileChoice
                    tileChoice = tileArray[0];
                    //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(51, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(7, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(15, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(25, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(30, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(33, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(40, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(42, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(45, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        randomPosition = new Vector3(48, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                }
                else
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        tileChoice = tileArray[0];
                        randomPosition = new Vector3(51, i, 0f);
                        Instantiate(tileChoice, randomPosition, Quaternion.identity);
                    }
                }
            }
        }

        Vector3 RandomSpawn()
        {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            Vector3 randomPosition;

            do
            {
                int randomIndex = Random.Range(0, gridPositions.Count);
                //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
                randomPosition = gridPositions[randomIndex];

                //Remove the entry at randomIndex from the list so that it can't be re-used.
                gridPositions.RemoveAt(randomIndex);
            } while ((randomPosition.x > 50) || (randomPosition.x == 7) || (randomPosition.x == 15) || (randomPosition.x == 25) || (randomPosition.x == 30) || (randomPosition.x == 33) || (randomPosition.x == 40) || (randomPosition.x == 42) || (randomPosition.x == 45) || (randomPosition.x == 48));
            //Return the randomly selected Vector3 position.
            return randomPosition;
        }

        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene (int level)
		{
			//Creates the outer walls and floor.
			BoardSetup ();
			
			//Reset our list of gridpositions.
			InitialiseList ();
			
			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

            //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
            //LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

            //Determine number of enemies based on current level number, based on a logarithmic progression
            //int innocentCount = 25;

            //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
            start = true;
            SpawnBullet(enemyTiles, 15, 20);
            start = false;
			
			//Instantiate the exit tile in the upper right hand corner of our game board
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}
}
