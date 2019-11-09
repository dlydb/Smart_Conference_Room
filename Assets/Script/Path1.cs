/*
 Author: Ziyu Liu
 Function: Control the path and movement for the people inside the room.
           The difference between Path1 and Path2 is the direction for person to sit down in their lab.
           The script should be assigned to each person model.
 */
using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;



public class Path1 : MonoBehaviour {

    // Variables: 
    public Animator anim;    // control the animation for each person model
    public Transform office;    // set the target for each person model

    // Six public areas that each person can share saved inside public_area
    private Transform[] public_area = new Transform[6];
    public Transform kitchen1;
    public Transform kitchen2;
    public Transform kitchen3;
    public Transform printer;
    public Transform pc1;
    public Transform pc2;

    // Helper script to set target for each person model
    public NavMeshAgent MeshAgent;

    // Public exit target
    public Transform exit;
    
    // Other variables
    private int tar;    // help to set target of movement 
    private int check = 0;    // helper variable to save the status for the person (0 means moving, 1 means stable, 2 moves to exit)
    private string[] text = File.ReadAllText(@"C:\FALL_2018\research\LESA\Assets\Tests\Time.txt").Split(' ');    // read the waiting time from text file
    private int[] sitting_time = new int[50];    // pass time to an array
    private int ite = 0;    // a variable to calculate random value
    private int timer = 0;    // count frame
    private int time_to_move = -1;    // record the time for the person to move
    private bool start = false;    // bool to save if the person has started to move or not
	
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        // target positions
        public_area[0] = kitchen1;
        public_area[1] = kitchen2;
        public_area[2] = kitchen3;
        public_area[3] = printer;
        public_area[4] = pc1;
        public_area[5] = pc2;

        // pass all the time value
        for (int i = 0; i < 50; i++)
        {
            sitting_time[i] = Int32.Parse(text[i]);
        }

        // reset all the position to 0 (not occupied)
        for (int i = 0; i < 6; i++)
        {
            leave_pos(i);
        }
    }

    // Utility functions
	// function for setting the destination
    private void SetDestination(Transform pos)
    {
        Vector3 targetVector = pos.transform.position;
        MeshAgent.SetDestination(targetVector);
    }

	// read the position information from the txt
    private bool read_pos(int num)
    {
        string[] pos_txt = File.ReadAllText(@"C:\FALL_2018\research\LESA\Assets\Tests\Targets.txt").Split('\n');
        if (Int32.Parse(pos_txt[num]) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	// when one of the chair is occupied, set to 1
    private void write_pos(int num)
    {
        string[] pos_txt = File.ReadAllText(@"C:\FALL_2018\research\LESA\Assets\Tests\Targets.txt").Split('\n');
        pos_txt[num] = "1";
        System.IO.File.WriteAllLines(@"C:\FALL_2018\research\LESA\Assets\Tests\Targets.txt", pos_txt);
    }

	// when someone leave the chair, set to 0
    private void leave_pos(int num)
    {
        string[] pos_txt = File.ReadAllText(@"C:\FALL_2018\research\LESA\Assets\Tests\Targets.txt").Split('\n');
        pos_txt[num] = "0";
        System.IO.File.WriteAllLines(@"C:\FALL_2018\research\LESA\Assets\Tests\Targets.txt", pos_txt);
    }

    // Update is called once per frame
    void Update () {
        timer++;

        // when the person just get into the simulation
        if (this.gameObject.activeSelf == true && start == false)
        {
            // set the initial start position
            transform.position = new Vector3(-10f, 0.0f, 15f);

            // use random value to find target (0, 1, 2) --> office     (3) --> public area
            System.Random num = new System.Random();
            tar = num.Next(0, 4);
            MeshAgent = GetComponent<NavMeshAgent>();

            // when target is office
            if (tar <= 2)
            {
                SetDestination(office);
                tar = -1;
            }

            // when target is public area, randomly choose between all the not occupied public area
            else
            {
                tar = num.Next(0, 6);
                while (read_pos(tar) == false)
                {
                    tar = num.Next(0, 6);
                }
                SetDestination(public_area[tar]);
                write_pos(tar);
            }
            start = true;
        }

        // person goes to office area, check if the person is close enough to his target
        if (tar == -1 && check == 0)
        {
            if ((transform.position.x <= office.position.x + 0.2) && (transform.position.x >= office.position.x - 0.2) && (transform.position.z <= office.position.z + 0.2) && (transform.position.z >= office.position.z - 0.2))
            {
                MeshAgent.enabled = false;
                check = 1;
                System.Random num_time = new System.Random();
                ite = num_time.Next(0, 50);
                time_to_move = timer + sitting_time[ite];
                anim.Play("Stand_to_sit", -1);
                transform.position = new Vector3(office.transform.position.x, office.transform.position.y, office.transform.position.z - 0.35f);
                transform.LookAt(new Vector3(office.transform.position.x, office.transform.position.y, office.transform.position.z - 30f));
            }
        }

        // person is moving to the public area
        else if (tar != -1 && check == 0)
        {
            if ((transform.position.x <= public_area[tar].position.x + 0.2) && (transform.position.x >= public_area[tar].position.x - 0.2) && (transform.position.z <= public_area[tar].position.z + 0.2) && (transform.position.z >= public_area[tar].position.z - 0.2) && check == 0)
            {
                MeshAgent.enabled = false;
                check = 1;

                // different public with different animation
                if (tar == 0)
                {
                    anim.Play("Standing", -1);
                    transform.position = new Vector3(public_area[tar].transform.position.x, public_area[tar].transform.position.y, public_area[tar].transform.position.z);
                    transform.LookAt(new Vector3(public_area[tar].transform.position.x - 30f, public_area[tar].transform.position.y, public_area[tar].transform.position.z));
                    time_to_move = timer + 100;
                }
                else if (tar == 1)
                {
                    anim.Play("Standing", -1);
                    transform.position = new Vector3(public_area[tar].transform.position.x, public_area[tar].transform.position.y, public_area[tar].transform.position.z);
                    transform.LookAt(new Vector3(public_area[tar].transform.position.x , public_area[tar].transform.position.y, public_area[tar].transform.position.z - 30f));
                    time_to_move = timer + 100;
                }
                else if (tar == 2)
                {
                    anim.Play("Standing", -1);
                    transform.position = new Vector3(public_area[tar].transform.position.x, public_area[tar].transform.position.y, public_area[tar].transform.position.z);
                    transform.LookAt(new Vector3(public_area[tar].transform.position.x + 30f, public_area[tar].transform.position.y, public_area[tar].transform.position.z));
                    time_to_move = timer + 100;
                }
                else if (tar == 3)
                {
                    anim.Play("Standing", -1);
                    transform.position = new Vector3(public_area[tar].transform.position.x, public_area[tar].transform.position.y, public_area[tar].transform.position.z);
                    transform.LookAt(new Vector3(public_area[tar].transform.position.x - 30f, public_area[tar].transform.position.y, public_area[tar].transform.position.z));
                    time_to_move = timer + 100;
                }
                else if (tar == 4 || tar == 5)
                {
                    anim.Play("Stand_to_sit", -1);
                    transform.position = new Vector3(public_area[tar].transform.position.x - 0.35f, public_area[tar].transform.position.y, public_area[tar].transform.position.z - 0.1f);
                    transform.LookAt(new Vector3(public_area[tar].transform.position.x - 30f, public_area[tar].transform.position.y, public_area[tar].transform.position.z - 0.1f));
                    time_to_move = timer + 100;
                }
            }
        }

        // check if the person has to move
        if (timer == time_to_move)
        {
            System.Random num = new System.Random();
            int tar_new = num.Next(0, 10);

            // if the person was not at the office, the person has to move to his office
            if (tar != -1)
            {
                leave_pos(tar);
                MeshAgent.enabled = true;
                SetDestination(office);
                tar = -1;
                check = 0;
                transform.Rotate(0, 180, 0);
                anim.Play("Walking", -1);
            }

            // tar_new has 3/10 probabilities that move to another target, otherwise, stay in his lab
            else if (tar_new <= 2)
            {
                num = new System.Random();
                tar_new = num.Next(0, 1);
                // target is exit
                if (tar_new == 0)
                {
                    MeshAgent.enabled = true;
                    SetDestination(exit);
                    check = 2;
                }
                // randomly select from the targets
                else
                {
                    tar_new = num.Next(0, 6);
                    while (read_pos(tar_new) == false)
                    {
                        num = new System.Random();
                        tar_new = num.Next(0, 6);
                    }
                    MeshAgent.enabled = true;
                    SetDestination(public_area[tar_new]);
                    write_pos(tar_new);
                    if (tar != -1)
                    {
                        leave_pos(tar);
                    }
                    check = 0;
                }
                tar = tar_new;
                anim.Play("Walking", -1);
            }
        }
        
        // when the person moves to the exit, disable the person
        if (check == 2 && this.transform.position.x < -9.8f)
        {
            this.gameObject.SetActive(false);
        }
    }
}