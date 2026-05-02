using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIScript : MonoBehaviour
{
    private bool IsEnter = false;
	private bool IsFollow = false;
    public bool IsAtacking = false;


	public bool IsKamikase;
	//public  bool IsSpeach = false;

    public string PlayerTag;
    //public AudioSource AudioPoint;
    public float Volume;
	public AudioClip[] AtackSounds;

	public AudioClip[] BrainsSounds;
    public Animator AIAnimations;
    private Transform PlayerPos;

    public NavMeshAgent Agent;
    public float PlayerDistance;
    public float AtackDistance;

	public int Damage;
    public float AtackRate;
    private float NextAtack = 0;

    void Start ()
    {
		IsEnter = false;
		IsFollow = false;
		IsAtacking = false;
        PlayerPos = GameObject.FindWithTag(PlayerTag).GetComponent<Transform>();
    }

    void Update ()
    {

        if(PlayerPos == null)
        {
			GetComponent<AIScript>().enabled = false;
        }
        if(Vector3.Distance (PlayerPos.position, transform.position) < 15) 
        {
          // BrainsSound ();
        }

        if(Vector3.Distance (PlayerPos.position, transform.position) < PlayerDistance) 
        {
            IsEnter = true;
            IsFollow = true;
        }
        else
        {
            IsEnter = false;
            IsFollow = false;

			AIAnimations.SetBool("IsIdle",true);
			AIAnimations.SetBool("IsWalk",false);
        }
        //==================Shoot Distance =================//

		if(Vector3.Distance (PlayerPos.position, transform.position) < AtackDistance) 
        {
			AIAnimations.SetBool("IsAtack",true);
			IsAtacking = true;
            Agent.isStopped = true;

			if (IsKamikase == true) 
			{
				Kamikaze();
			}
        }

		if(Vector3.Distance (PlayerPos.position, transform.position) > AtackDistance) 
        {
            if(IsEnter)
            {   
				AIAnimations.SetBool("IsAtack",false);
				IsAtacking = false;
                Agent.isStopped = false;
                IsFollow = true;
            }
        }

        //===================Is Follow =======================//
        if(IsFollow == true)
        {
			AIAnimations.SetBool("IsWalk",true);
            Agent.destination = PlayerPos.position;
        }
        else
        {
            Agent.isStopped = true;
        }

        //======================Shoot Action ===================//

		if (IsAtacking == true) 
		{
			if(Time.time > NextAtack)
			{
				NextAtack = Time.time + AtackRate;
                PlayerHealth.Health -= Damage;

				if (GetComponent<AudioSource> ().isPlaying)return;
				GetComponent<AudioSource> ().clip = AtackSounds [Random.Range (0, AtackSounds.Length)];
				GetComponent<AudioSource> ().Play ();
			}
		}
    }

	void Kamikaze ()
	{
		
	}

	void BrainsSound ()
	{
        if(Time.time > NextAtack)
		{
			NextAtack = Time.time + Random.Range(8,15);
            if (GetComponent<AudioSource> ().isPlaying)return;
            GetComponent<AudioSource> ().clip = BrainsSounds [Random.Range (0, BrainsSounds.Length)];
            GetComponent<AudioSource> ().Play ();
        }
	}
}
