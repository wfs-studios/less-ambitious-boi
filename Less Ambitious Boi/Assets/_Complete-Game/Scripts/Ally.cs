using UnityEngine;
using System.Collections;

namespace Completed
{
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class Ally : MovingObject
    {
        public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
        public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
        public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.

        public int xRand;
        public int yRand;
        public int randomizer;
        public int xy;
        public int wallDamage = 1;
        public int hp = 1;
        public string script;

        private Transform target;                           //Transform to attempt to move toward each turn.
        private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.


        //Start overrides the virtual Start function of the base class.
        protected override void Start()
        {
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
            GameManager.instance.AddAlliesToList(this);

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
            else if (randomizer % 2 == 1)
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

        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveAlly()
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



        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        //See comments in MovingObject for more on how base AttemptMove function works.
        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            //Hit will store whatever our linecast hits when Move is called.
            RaycastHit2D hit;

            //Set canMove to true if Move was successful, false if failed.
            bool canMove = MoveyBoi(xDir, yDir, out hit);
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


        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        protected override void OnCantMove<T>(T component)
        {
            if (component is Player)
            {
                //Declare hitPlayer and set it to equal the encountered component.
                Player hitPlayer = component as Player;
                hp -= 1;
            }

            else if (component is Wall)
            {
                //Set hitWall to equal the component passed in as a parameter.
                Wall hitWall = component as Wall;

                //Call the DamageWall function of the Wall we are hitting.
                hitWall.DamageWall(wallDamage);
            }

            /*else if (component is Ally)
            {
                Ally hitAlly = component as Ally;

                hitAlly.DamageAlly(playerDamage);
            }*/

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
            if (hp <= 0)
                Destroy(gameObject);
        }

        bool MoveyBoi(int xDir, int yDir, out RaycastHit2D hit)
        {
            //Store start position to move from, based on objects current transform position.
            Vector2 start = transform.position;

            // Calculate end position based on the direction parameters passed in when calling Move.
            Vector2 end = start + new Vector2(xDir, yDir);

            //Disable the boxCollider so that linecast doesn't hit this object's own collider.
            boxCollider.enabled = false;

            //Cast a line from start point to end point checking collision on blockingLayer.
            hit = Physics2D.Linecast(start, end, blockingLayer);

            //Re-enable boxCollider after linecast
            boxCollider.enabled = true;

            //Check if anything was hit
            if (hit.transform == null)
            {
                //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
                StartCoroutine(SmoothMovemen(end));

                //Return true to say that Move was successful
                return true;
            }

            //If something was hit, return false, Move was unsuccesful.
            return false;
        }


        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        protected IEnumerator SmoothMovemen(Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
                /*for (int x = 0; x < 400; x++)
                {
                    if (newPosition.x == BoardManager.floorMap[x].x && newPosition.y == BoardManager.floorMap[x].y && (Player.turn % 2 == 1))
                    {
                        break;
                    }
                }*/
                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPosition);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
        }

    }
}
