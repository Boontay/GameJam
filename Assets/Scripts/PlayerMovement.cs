using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {
    public float jumpVelocity;
    public Vector2 velocity;
    private Vector2 runVelocity;
    public GameObject playerObject;
    private GameObject playerObjectSize;
    private RectTransform playerObjectRectangle;
    private float width, height;
    public float gravity;
    public LayerMask floorMask;
    public LayerMask wallMask;
    private bool walking, movingLeft, movingRight, jumping;

    public enum PlayerState {
        jumping,
        idle,
        walking,
        running
    }

    private PlayerState playerState = PlayerState.idle;
    public bool grounded = false;
    private bool running = false;

    void Start () {
        runVelocity = GetRunVelocity(velocity);
        playerObjectSize = (GameObject)Instantiate(playerObject);
        playerObjectRectangle = (RectTransform)playerObjectSize.transform;
        width = playerObjectRectangle.rect.width;
        height = playerObjectRectangle.rect.height;

        Fall();
	}

	void Update () {
        CheckPlayerInput();
        UpdatePlayerPosition();
        // UpdatePlayerAnimationStates();
	}

    /*
    void UpdatePlayerAnimationStates() {
        if (grounded && !walking) {
            GetComponent<Animator>().SetBool("isJumping", false);   // isJumping is an animation.
            GetComponent<Animator>().SetBool("isRunning", true);   // isRunning is an animation.
            GetComponent<Animator>().SetBool("isWalking", false);    // isWalking is an animation.
        }

        if (grounded && running) {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
            GetComponent<Animator>().SetBool("isWalking", false);
        }

        if (grounded && walking) {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
            GetComponent<Animator>().SetBool("isWalking", true);
        }

        if (playerState == PlayerState.jumping) {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
            GetComponent<Animator>().SetBool("isWalking", false);
        }
    }
    */

    Vector2 GetRunVelocity(Vector2 velocity) {
        return new Vector2(velocity.x * 1.5f, velocity.y * 1.5f);
    }

    void UpdatePlayerPosition() {
        Vector3 position = transform.localPosition;
        Vector3 scale = transform.localScale;

        if (walking) {
            if (movingLeft) {
                position.x -= velocity.x * Time.deltaTime;
                scale.x = -1;
            } else if (movingRight) {
                position.x += velocity.x * Time.deltaTime;
                scale.x = 1;
            }

            position = CheckWallRays(position, scale.x);
            //position = CheckFloorRays(position);
            //position = CheckCeilingRays(position);
        }

        if (running) {
            if (movingLeft) {
                position.x -= runVelocity.x * Time.deltaTime;
            } else {
                position.y += runVelocity.x * Time.deltaTime;
            }

            position = CheckWallRays(position, scale.x);
            position = CheckFloorRays(position);
            position = CheckCeilingRays(position);
        }

        if (jumping && playerState != PlayerState.jumping) {
            playerState = PlayerState.jumping;
            velocity = new Vector2(velocity.x, jumpVelocity);
        }
        if (playerState == PlayerState.jumping) {
            position.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }

        if (running && playerState != PlayerState.running) {
            playerState = PlayerState.running;
            velocity = new Vector2(velocity.x, runVelocity.y);
        }

        transform.localPosition = position;
        transform.localScale = scale;
    }

    void CheckPlayerInput() {
        bool leftInput = Input.GetKey(KeyCode.LeftArrow);
        bool rightInput = Input.GetKey(KeyCode.RightArrow);
        bool jumpInput = Input.GetKey(KeyCode.UpArrow);
        bool runInput = Input.GetKey(KeyCode.Space);
        bool restartScene = Input.GetKey(KeyCode.R);

        walking = leftInput || rightInput;
        movingLeft = leftInput && !rightInput;
        movingRight = !leftInput && rightInput;
        running = runInput;
        jumping = jumpInput;

        if (restartScene) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void Fall() {
        velocity.y = 0;
        playerState = PlayerState.jumping;
        grounded = false;
        running = false;
    }

    Vector3 CheckWallRays(Vector3 position, float direction) {
        Vector2 originTop = new Vector2(position.x + (direction * 0.4f), position.y - 0.2f + (height / 2f));
        Vector2 originMiddle = new Vector2(position.x + (direction * 0.4f), position.y);
        Vector2 originBottom = new Vector2(position.x + (direction * 0.4f), position.y - 0.2f + (height / 2f));

        RaycastHit2D wallTop = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        if (wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null) {
            position.x -= velocity.x * Time.deltaTime * direction;
        }

        return position;
    }

    Vector3 CheckFloorRays(Vector3 position) {
        Vector2 originLeft = new Vector2(position.x - (width / 2f) + 0.2f, position.y - (height / 2f));
        Vector2 originMiddle = new Vector2(position.x, position.y - (height / 2));
        Vector2 originRight = new Vector2(position.x + (width / 2f) -0.2f, position.y - (height / 2f));

        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if (floorLeft.collider != null || floorMiddle.collider != null || floorRight.collider != null) {
            RaycastHit2D hitRay = floorMiddle;

            if (floorLeft) {
                hitRay = floorLeft;
            } else if (floorMiddle) {
                hitRay = floorRight;
            }

            playerState = PlayerState.idle;
            grounded = true;
            velocity.y = 0;
            position.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1;
        } else {
            if (playerState != PlayerState.jumping) {
                Fall();
            }
        }

        return position;
    }

    Vector3 CheckCeilingRays(Vector3 position) {
        Vector2 originLeft = new Vector2(position.x - (width / 2f) + 0.2f, position.y + (height / 2f));
        Vector2 originRight = new Vector2(position.x + (width / 2f) - 0.2f, position.y + (height / 2f));
        Vector2 originMiddle = new Vector2(position.x, position.y + (height / 2f));

        RaycastHit2D ceilingLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilingMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilingRight = Physics2D.Raycast(originRight, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        if (ceilingLeft.collider != null || ceilingMiddle.collider != null || ceilingRight.collider != null) {
            RaycastHit2D hitRay = ceilingMiddle;

            if (ceilingLeft) {
                hitRay = ceilingLeft;
            } else if (ceilingRight) {
                hitRay = ceilingRight;
            }

            position.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - (height / 2);
            Fall();
        }

        return position;
    }
}