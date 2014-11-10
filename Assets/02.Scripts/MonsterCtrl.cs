using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {

	public enum MonsterState { idle, trace, attack, die };
	public MonsterState monsterState = MonsterState.idle;

	public float traceDist = 10.0f;
	public float attackDist = 2.0f;

	private bool isDie = false;

    public GameObject bloodEffect;
    public GameObject bloodDecal;

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

    // Update is called once per frame
    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).nameHash == hashGotHit)
        {
            _animator.SetBool("IsHit", false);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.fall"))
        {
            _animator.SetBool( "IsPlayerDie", false );
        }
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
            StartCoroutine(this.CreateBloodEffect(coll.transform.position));
			Destroy (coll.gameObject);
			_animator.SetBool("IsHit", true);
		}
	}

    IEnumerator CreateBloodEffect( Vector3 pos )
    {
        GameObject _blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity );
        Destroy(_blood1, 2.0f);

        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.01f);

        Quaternion decalRot = Quaternion.Euler(0, Random.Range(0,360), 0);

        GameObject _blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);

        float _scale = Random.Range(1.5f, 3.5f);
        _blood2.transform.localScale = new Vector3(_scale, 1, _scale);
        Destroy(_blood2, 5.0f);

        yield return null;
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        nvAgent.Stop();
        _animator.SetBool("IsPlayerDie", true);
    }
}