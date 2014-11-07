using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {

	public enum MonsterState { idle, trace, attack, die };
	public MonsterState monsterState = MonsterState.idle;

	public float traceDist = 10.0f;
	public float attackDist = 2.0f;

	private bool isDie = false;

	private Transform monsterTr;
	private Transform playerTr;
	private NavMeshAgent nvAgent;
	private Animator _animator;

	private int hashGotHit = 0;

	// Use this for initialization
	void Start ()
	{
		monsterTr = this.gameObject.GetComponent<Transform>();
		playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
		nvAgent = this.gameObject.GetComponent<NavMeshAgent> ();

		nvAgent.destination = playerTr.position;

		_animator = this.gameObject.GetComponent<Animator>();

		hashGotHit = Animator.StringToHash ("Base Layer.gothit");

		StartCoroutine (this.CheckMonsterState ());
		StartCoroutine (this.MonsterAction ());
	}

	IEnumerator CheckMonsterState()
	{
		while (!isDie)
		{
			yield return new WaitForSeconds (0.2f);

			float dist = Vector3.Distance (playerTr.position, monsterTr.position);

			if (dist <= attackDist)
			{
				monsterState = MonsterState.attack;
			}
			else if (dist <= traceDist)
			{
				monsterState = MonsterState.trace;
			} 
			else
			{
				monsterState = MonsterState.idle;
			}
		}
	}


	IEnumerator MonsterAction()
	{
		while (!isDie)
		{
			switch(monsterState)
			{
				case MonsterState.idle:
					nvAgent.Stop();
					_animator.SetBool("IsAttack", false);
					_animator.SetBool("IsTrace", false);
					break;

				case MonsterState.trace:
					nvAgent.destination = playerTr.position;
					_animator.SetBool("IsAttack", false);
					_animator.SetBool("IsTrace", true);
					break;

				case MonsterState.attack:
					nvAgent.Stop();
					_animator.SetBool("IsAttack", true);
					break;
			}

			yield return null;
		}
	}

	void OnCollisionEnter( Collision coll )
	{
		if (coll.gameObject.tag == "BULLET")
		{
			Destroy (coll.gameObject);
			_animator.SetBool("IsHit", true);
		}
	}
	// Update is called once per frame
	void Update ()
	{
		if( _animator.GetCurrentAnimatorStateInfo (0).nameHash == hashGotHit )
		{
			_animator.SetBool ("IsHit", false);
		}
	}
}