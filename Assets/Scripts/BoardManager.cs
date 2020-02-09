﻿using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using System.Linq;

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
		
		
		public Count columns = new Count(5, 15); 										//Number of columns in our game board.
		public Count rows = new Count(5, 8);											//Number of rows in our game board.
		public Count wallCount = new Count (5, 9);						//Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
		public GameObject exit;											//Prefab to spawn for exit.
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] enemyTiles;									//Array of enemy prefabs.
		public GameObject[] outerWallTiles;								//Array of outer tile prefabs.
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
		private List<Vector3> playerPositions = new List<Vector3>();    //A list of possible locations to spawn players.
		private List<Vector3> playerPositionsTaken = new List<Vector3>();//A list of taken locations to spawn players.
		private Vector3 defaultPositionPlayer;							//First element of the above list.
		private int currentColumns;
		private int currentRows;

		private int columnOffset;
		private int rowOffset;

		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();

			//Loop through x axis (columns).
			for (int x = 1; x < currentColumns - 1; ++x)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < currentRows - 1; ++y)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x - columnOffset, y - rowOffset, 0f));
				}
			}
		}

		
		void InitialisePlayerPositions()
		{
			//Clear our list playerPositions and playerPositionsTaken.
			playerPositions.Clear();
			playerPositionsTaken.Clear();

			// Add bottom line to spawn positions.

			//Loop through x axis (columns).
			for (int x = 0; x < currentColumns; ++x)
			{
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				playerPositions.Add(new Vector3(x - columnOffset, -rowOffset, 0f));
			}

			// Add left column to spawn positions.

			//Loop through x axis (columns).
			for (int y = 0; y < currentRows; ++y)
			{
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				playerPositions.Add(new Vector3(-columnOffset, y - rowOffset, 0f));
			}

			// Remove duplicates (1 at least, the bottom left position)
			playerPositions = playerPositions.Distinct().ToList();

			// Set first element to keep in case our list ends up empty
			defaultPositionPlayer = playerPositions[0];
		}


		//Return the next available position to spawn a player.
		public Vector3 NextPositionPlayer(int index)
		{
			// Failsafe.
			if (playerPositions.Count == playerPositionsTaken.Count)
			{
				return defaultPositionPlayer;
			}

			Vector3 nextPosition;

			// Try and spawn at the requested position.
			if (!playerPositionsTaken.Contains(playerPositions[index]))
			{
				// If available spawn there
				nextPosition = playerPositions[index];
			}
			else
			{
				// Else spawn in the first slot available
				int i = 0;
				while (playerPositionsTaken.Contains(playerPositions[i])) i++;
				nextPosition = playerPositions[i];
			}

			//Remove said position from list of available positions.
			playerPositionsTaken.Add(nextPosition);

			//Return the selected Vector3 position.
			return nextPosition;
		}


		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;

			currentColumns = Random.Range(columns.minimum, columns.maximum);
			currentRows = Random.Range(rows.minimum, rows.maximum);

			columnOffset = currentColumns / 2;
			rowOffset = currentRows / 2;

			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for (int x = -1; x < currentColumns + 1; ++x)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < currentRows + 1; ++y)
				{
					GameObject toInstantiate = null;
					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if (x == -1 || x == currentColumns || y == -1 || y == currentRows)
					{
						toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
					}
					else
					{
						//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
						toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
					}
					
					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance = Instantiate (toInstantiate, new Vector3 (x - columnOffset, y - rowOffset, 0f), Quaternion.identity) as GameObject;
					
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

			if (objectCount > gridPositions.Count) return;
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
		
		
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			//Creates the outer walls and floor.
			BoardSetup ();
			
			//Reset our list of gridpositions.
			InitialiseList ();

			//Reset our player spawn positions.
			InitialisePlayerPositions();

			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			
			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			
			//Determine number of enemies based on current level number, based on a logarithmic progression
			int enemyCount = (int)Mathf.Log(level, 2f);
			
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
			//Instantiate the exit tile in the upper right hand corner of our game board
			Instantiate (exit, new Vector3 (currentColumns - 1 - columnOffset, currentRows - 1 - rowOffset, 0f), Quaternion.identity);
		}
	}
}
