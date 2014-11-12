using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour {
	private float h = 0.0f;
	private float v = 0.0f;

	private float fCrossFadeTime = 0.3f;

	private Transform tr;

	public float moveSpeed = 10.0f;
	public float rotSpeed = 1000000.0f;

    public int hp = 100;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

	[System.Serializable]
	public class Anim{
		public AnimationClip idle;
		public AnimationClip runForward;
		public AnimationClip runBackward;
		public AnimationClip runRight;
		public AnimationClip runLeft;
	}

	public Anim anim;

	public Animation _animation;

    private GameMgr _gameMgr;

	// Use this for initialization
	void Start () {
		tr = GetComponent <Transform>();

        _gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();

		Init_Animation ();
	}
	
	// Update is called once per frame
	void Update () {
		h = Input.GetAxis ("Horizontal");
		v = Input.GetAxis ("Vertical");

		//Debug.Log ("H = " + h.ToString ());
		//Debug.Log ("V = " + v.ToString ());

		Update_Move ();
		Update_Rotate ();
		Update_Animation ();
	}

	private void Init_Animation(){
		_animation = GetComponentInChildren<Animation> ();
		_animation.clip = anim.idle;
		_animation.Play ();
	}

	private void Update_Move(){
		Vector3 moveDir = ( Vector3.forward* v ) + (Vector3.right * h);
		tr.Translate( moveDir * Time.deltaTime * moveSpeed, Space.Self );
	}
	private void Update_Rotate()
    {
        if (Input.GetMouseButton(1) )
        {
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));
        }
	}
	private void Update_Animation(){
		if (v >= 0.1f) {
			_animation.CrossFade (anim.runForward.name, fCrossFadeTime);
		} else if (v < 0.0f) {
			_animation.CrossFade (anim.runBackward.name, fCrossFadeTime);
		} else if (h >= 0.1f) {
			_animation.CrossFade (anim.runRight.name, fCrossFadeTime);
		} else if (h < 0.0f) {
			_animation.CrossFade (anim.runLeft.name, fCrossFadeTime);
		} else {
			_animation.CrossFade (anim.idle.name, fCrossFadeTime);
		}
	}

    private void OnTriggerEnter( Collider coll )
    {
        if( coll.gameObject.tag == "PUNCH" )
        {
            hp -= 10;
            Debug.Log("Player HP = " + hp.ToString());

            if( hp <= 0 )
            {
                //PlayerDie();

                OnPlayerDie();

                _gameMgr.isGameOver = true;
            }
        }
    }

    //private void PlayerDie()
    //{
    //    Debug.Log("Player Die !!");

    //    GameObject[] monsters = GameObject.FindGameObjectsWithTag( "MONSTER" );

    //    foreach( GameObject monster in monsters )
    //    {
    //        monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
    //    }
    //}
}