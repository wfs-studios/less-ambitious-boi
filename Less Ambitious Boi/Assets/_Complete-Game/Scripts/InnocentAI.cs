using UnityEngine;
using System.Collections;

namespace Completed
{
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class InnocentAI : MovingObject
    {
        public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
        public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
        public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.

        public int xRand;
        public int yRand;
        public int randomizer;
        public int xy;
        public int wallDamage = 1;
        public string script;

        private Transform target;                           //Transform to attempt to move toward each turn.
        private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.


        //Start overrides the virtual Start function of the base class.
        protected override void Start()
        {
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
            GameManager.instance.AddInnocentsToList(this);

            Randomize();

            //Call the start function of our base class MovingObject.
            base.Start();
        }


        public void Randomize()
        {
            randomizer = Random.Range(0, 3);
            xy = Random.Range(0, 2);
            if (randomizer % 3 == 0)
            {
                xRand = 0;
                yRand = 0;
            }
            if (randomizer % 2 == 1)
            {
                xRand = -1;
                yRand = -1;
            }
            else
            {
                xRand = 1;
                yRand = 1;
            }
        }

        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        //See comments in MovingObject for more on how base AttemptMove function works.
        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            //Hit will store whatever our linecast hits when Move is called.
            RaycastHit2D hit;

            //Set canMove to true if Move was successful, false if failed.
            bool canMove = Move(xDir, yDir, out hit);
            //Check if nothing was hit by linecast
            if (hit.transform == null)
            {
                //If nothing was hit, return and don't execute further code.
                return;
            }
            //Get a component reference to the component of type T attached to the object that was hit
            T hitComponent = hit.transform.GetComponent<T>();
            Wall ball = null;
            if (hitComponent == null)
            {
                ball = hit.transform.GetComponent<Wall>();
            }
            //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
            if (!canMove)
            {
                if (hitComponent != null)
                    OnCantMove(hitComponent);

                if (ball != null)
                    OnCantMove(ball);
            }
        }

        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveInnocent()
        {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            //If the difference in positions is approximately zero (Epsilon) do the following:
            if (xy == 1)

                //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                yDir = yRand;

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            else
                //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
                xDir = xRand;

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            Randomize();
            AttemptMove<Player>(xDir, yDir);
        }


        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        protected override void OnCantMove<T>(T component)
        {
            if (component is Player)
            {
                //Declare hitPlayer and set it to equal the encountered component.
                Player hitPlayer = component as Player;
            }

            if (component is Wall)
            {
                //Set hitWall to equal the component passed in as a parameter.
                Wall hitWall = component as Wall;

                //Call the DamageWall function of the Wall we are hitting.
                hitWall.DamageWall(wallDamage);
            }

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
        }
    }
}
