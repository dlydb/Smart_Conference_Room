/*
 Author: Ziyu Liu
 Function: Main function to control the simulation with enabling each human modal and saving desired data to further transfer into image by Matlab.
 */
using UnityEngine;
using System;
using System.IO;

public class Simulation : MonoBehaviour {

    // Variables
    // person models with array
    public GameObject person1;
    public GameObject person2;
    public GameObject person3;
    public GameObject person4;
    public GameObject person5;
    public GameObject person6;
    public GameObject person7;
    private GameObject[] people = new GameObject[7];

    private int timer = 0;    // timer to count frame number
    private int time_to_in = 0;    // time for people to move in
    private int layerMask = ~(1 << 8);    // helper variable with Raycast function
    UnityEngine.Color color = UnityEngine.Color.blue;    // color for rays

    // start and end location for the room
    private float x_axis_start = -9.52f;
    private float x_axis_end = -3.5f;
    private float z_axis_start = 0f;
    private float z_axis_end = 30.26f;

    // different camera position with different height
    private Vector3 camPos = new Vector3(0, (float)2.63, 0);
    private Vector3 camPos2 = new Vector3(0, (float)2.9, 0);

    RaycastHit hit;    // hit point for rays
    private float distanceToGround;    // variable to save distance from ray to ground
    private float heightObj = 0;    // calculated height of object

    // different vector array for cameras with different degree
    private Vector3[] camDirArray30, camDirArray15, camDirArray45;

    // helper variable with float type
    private float r, ang_dif, pos_ang, beam_length15, beam_length30, beam_length45;

    // helper variable with double type
    private double radians15, radians30, radians45;

    // helper variable with int type
    private int b_x, b_z, x_size, z_size, time_ite = 0;

    // background matrix (base)
    private int[,] background;

    // pass all the waiting time to array
    private string[] text = File.ReadAllText(@"C:\FALL_2018\research\LESA\Assets\Tests\Waiting_Time.txt").Split(' ');

    // helper variable with int type
    private int count, max_occupancy;

    /*
      Set positions for all the cameras with different degree. The function will pass the matrix with 
      each sensor reading and return the updated matrix.
    */
    int[,] cam_cast(int[,] matrix)
    {
        camPos.Set((float)-6, (float)2.63, (float)2.5);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.63);
        camPos.Set((float)-5.5, (float)2.63, (float)5.3);
        matrix = cast(camDirArray15, camPos, matrix, beam_length15, (float)2.63);
        camPos.Set((float)-7, (float)2.63, (float)6.4);
        matrix = cast(camDirArray15, camPos, matrix, beam_length15, (float)2.63);
        camPos.Set((float)-5, (float)2.63, (float)7.8);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.63);
        camPos.Set((float)-5, (float)2.63, (float)11);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.63);
        camPos.Set((float)-5, (float)2.63, (float)15.4);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.63);
        camPos.Set((float)-4, (float)2.63, (float)12.8);
        matrix = cast(camDirArray15, camPos, matrix, beam_length15, (float)2.63);
        camPos.Set((float)-4, (float)2.63, (float)18.2);
        matrix = cast(camDirArray15, camPos, matrix, beam_length15, (float)2.63);
        camPos.Set((float)-5, (float)2.63, (float)20);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.63);
        camPos.Set((float)-5, (float)2.63, (float)23.5);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.63);
        camPos.Set((float)-8, (float)2.9, (float)21.8);
        matrix = cast(camDirArray30, camPos, matrix, beam_length30, (float)2.9);
        camPos.Set((float)-8, (float)2.9, (float)20.8);
        matrix = cast(camDirArray15, camPos, matrix, beam_length15, (float)2.9);
        camPos.Set((float)-4.3, (float)2.63, (float)26);
        matrix = cast(camDirArray15, camPos, matrix, beam_length15, (float)2.63);
        return matrix;
    }

    /*
      Use calculated variables to update matrix with distance.
    */
    int[,] cast(Vector3[] camDirArray, Vector3 camPos, int[,] matrix, float beam_length, float cam_height)
    {
        for (int d = 0; d < 9; d++) // mimicing Trinity, hence 9 measurements
        {
            // when reading from the center ray
            if (d == 0)
            {
                if (Physics.Raycast(camPos, camDirArray[d], out hit, 6.0f, layerMask))    // check if the ray touches an object
                {
                    distanceToGround = hit.distance;
                    heightObj = cam_height - distanceToGround;
                    
                    // delete small error
                    if (Math.Abs(heightObj) < 0.01)
                    {
                        heightObj = 0;
                    }

                    // show the rays on play
                    Debug.DrawLine(camPos, hit.point, color);
                }
                else
                {
                    heightObj = 0;
                }
            }

            // side rays
            else
            {
                if (Physics.Raycast(camPos, camDirArray[d], out hit, 6.0f, layerMask))
                {
                    distanceToGround = hit.distance;
                    heightObj = beam_length - distanceToGround;
                    if (Math.Abs(heightObj) < 0.01)
                    {
                        heightObj = 0;
                    }
                    Debug.DrawLine(camPos, hit.point, color);
                }
                else
                {
                    heightObj = 0;
                }
            }

            // calculate the point with height map onto matrix
            Vector3 point_hit = get_pos(camPos[0], camPos[2], (camDirArray[d][0] * cam_height + camPos[0]), (camDirArray[d][2] * cam_height + camPos[2]), heightObj, beam_length, cam_height);
            int x_f = (int)Math.Floor(point_hit[0] * 5 / 3 + b_x);
            int z_f = (int)Math.Floor(point_hit[2] * 5 / 3 + b_z);
            int y_f = (int)(point_hit[1] / cam_height * 255);
            if (x_f >= 0 && x_f < x_size && z_f >= 0 && z_f < z_size)
            {
                if (y_f > matrix[x_f, z_f])
                {
                    matrix[x_f, z_f] = y_f;
                }
            }
        }
        return matrix;
    }

    // helper function to calculate the matrix
    Vector3 get_pos(float camXPos, float camZPos, float floorXPos, float floorZPos, float cam_dis, float cam_length, float cam_height)
    {
        float ratio = cam_dis / cam_length;
        float y_pos = ratio * cam_height;
        float x_pos = ratio * (camXPos - floorXPos) + floorXPos;
        float z_pos = ratio * (camZPos - floorZPos) + floorZPos;
        return new Vector3(x_pos, y_pos, z_pos);
    }
    
    // Initial values
    void Start () {
        color.a = 0.3f;    // set width for rays

        // initial 30 degree rays variables
        camDirArray30 = new Vector3[9];
        camDirArray30[0] = new Vector3(0, -1, 0);
        ang_dif = 360 / 8;
        ang_dif = ang_dif * (float)(Math.PI / 180);
        pos_ang = 0;
        radians30 = 30 * (Math.PI / 180);
        beam_length30 = (float)2.63 / (float)Math.Cos(radians30);
        r = (float)Math.Tan(radians30);
        for (int i = 1; i < 9; i++)
        {
            float x_pos = r * (float)Math.Cos(pos_ang);
            float z_pos = r * (float)Math.Sin(pos_ang);
            camDirArray30[i] = new Vector3(x_pos, -1, z_pos);
            pos_ang += ang_dif;
        }

        // initial 15 degree rays variables
        camDirArray15 = new Vector3[9];
        camDirArray15[0] = new Vector3(0, -1, 0);
        ang_dif = 360 / 8;
        ang_dif = ang_dif * (float)(Math.PI / 180);
        pos_ang = 0;
        radians15 = 15 * (Math.PI / 180);
        beam_length15 = (float)2.63 / (float)Math.Cos(radians15);
        r = (float)Math.Tan(radians15);
        for (int i = 1; i < 9; i++)
        {
            float x_pos = r * (float)Math.Cos(pos_ang);
            float z_pos = r * (float)Math.Sin(pos_ang);
            camDirArray15[i] = new Vector3(x_pos, -1, z_pos);
            pos_ang += ang_dif;
        }

        // initial 45 degree rays varibles
        camDirArray45 = new Vector3[9];
        camDirArray45[0] = new Vector3(0, -1, 0);
        ang_dif = 360 / 8;
        ang_dif = ang_dif * (float)(Math.PI / 180);
        pos_ang = 0;
        radians45 = 45 * (Math.PI / 180);
        beam_length45 = (float)2.63 / (float)Math.Cos(radians45);
        r = (float)Math.Tan(radians45);
        for (int i = 1; i < 9; i++)
        {
            float x_pos = r * (float)Math.Cos(pos_ang);
            float z_pos = r * (float)Math.Sin(pos_ang);
            camDirArray45[i] = new Vector3(x_pos, -1, z_pos);
            pos_ang += ang_dif;
        }

        // save if the public area has been occupied or not
        string[] txt = new string[6];
        for (int i = 0; i < 6; i++)
        {
            txt[i] = "0";
        }
        System.IO.File.WriteAllLines(@"C:\FALL_2018\research\LESA\Assets\Tests\Targets.txt", txt);

        // initial variables for function
        b_x = 9 - (int)Math.Round(5 / 3 * x_axis_start, 0);
        b_z = (int)Math.Round(5 / 3 * z_axis_start, 0);
        x_size = 2 + (int)Math.Round(5 / 3 * x_axis_end) + b_x;
        z_size = 15 + (int)Math.Round(5 / 3 * z_axis_end) + b_z;

        // initial the matrix
        background = new int[x_size, z_size];
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < z_size; j++)
            {
                background[i, j] = -1;
            }
        }
        background = cam_cast(background);

        // save the background matrix
        String path = Application.dataPath + "/Tests/Data/background.txt";
        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < z_size; j++)
            {
                //sw.Write(Convert.ToString(background[i, j]).PadLeft(5));
                sw.Write(Convert.ToString(background[i, j]) + ",");
                if (j == z_size - 1)
                {
                    sw.Write("\n");
                }
            }
        }
        sw.Close();
        fs.Close();

        // pass models to the array
        people[0] = person1;
        people[1] = person2;
        people[2] = person3;
        people[3] = person4;
        people[4] = person5;
        people[5] = person6;
        people[6] = person7;

        // de-activate all the human models
        for (int i = 0; i < 7; i++)
        {
            people[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
        timer++;
        Debug.Log("timer: " + timer);

        // code here is used to determine if the person is enter or exit any entrance
        /*
        camPos.Set((float)-8, (float)2.63, (float)20.8);
        if (Physics.Raycast(camPos, camDirArray15[7], out hit, 6.0f, layerMask))
        {
            distanceToGround = hit.distance;
            heightObj = beam_length15 - distanceToGround;
            if (Math.Abs(heightObj) > 0.01)
            {
                Debug.Log("Hit Entry1");
            }
        }
        if (Physics.Raycast(camPos, camDirArray15[8], out hit, 6.0f, layerMask))
        {
            distanceToGround = hit.distance;
            heightObj = beam_length15 - distanceToGround;
            if (Math.Abs(heightObj) > 0.01)
            {
                Debug.Log("Hit Entry1");
            }
        }
        if (Physics.Raycast(camPos, camDirArray15[0], out hit, 6.0f, layerMask))
        {
            distanceToGround = hit.distance;
            heightObj = (float)2.63 - distanceToGround;
            if (Math.Abs(heightObj) > 0.01)
            {
                Debug.Log("Hit Entry1");
            }
        }
        camPos.Set((float)-7, (float)2.63, (float)6.4);
        if (Physics.Raycast(camPos, camDirArray15[0], out hit, 6.0f, layerMask))
        {
            distanceToGround = hit.distance;
            heightObj = (float)2.63 - distanceToGround;
            if (Math.Abs(heightObj) > 0.01)
            {
                Debug.Log("Hit Entry2");
            }
        }
        if (Physics.Raycast(camPos, camDirArray15[4], out hit, 6.0f, layerMask))
        {
            distanceToGround = hit.distance;
            heightObj = beam_length15 - distanceToGround;
            if (Math.Abs(heightObj) > 0.01)
            {
                Debug.Log("Hit Entry2");
            }
        }
        if (Physics.Raycast(camPos, camDirArray15[5], out hit, 6.0f, layerMask))
        {
            distanceToGround = hit.distance;
            heightObj = beam_length15 - distanceToGround;
            if (Math.Abs(heightObj) > 0.01)
            {
                Debug.Log("Hit Entry2");
            }
        }*/

        // time to activate different human models
        if (time_ite < 5)
        {
            if (timer == Int32.Parse(text[time_ite]))
            {
                people[time_ite].SetActive(true);
                time_ite++;
            }
        }

        // save the matrix to desired place
        String path = Application.dataPath + "/Tests/Data/time" + Convert.ToString(timer) + ".txt";
        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        int[,] matrix = new int[x_size, z_size];
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < z_size; j++)
            {
                matrix[i, j] = -1;
            }
        }
        matrix = cam_cast(matrix);
        int[] matrix1 = new int[x_size * z_size];
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < z_size; j++)
            {
                matrix1[i + x_size + j] = matrix[i, j];
            }
        }
        int test = 0;
        //int c = 0;
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < z_size; j++)
            {
              //  if (matrix[i, j] > -1)
                //{
                  //  c += 1;
                //}
                if (matrix[i, j] > background[i, j] && matrix[i, j] > 0)
                {
                    test = 1;
                    //sw.Write(Convert.ToString(matrix[i, j]).PadLeft(5));
                    sw.Write(Convert.ToString(matrix[i, j]) + ",");
                }
                else
                {
                    sw.Write(Convert.ToString(0) + ",");
                    //sw.Write(Convert.ToString(0).PadLeft(5));
                }
                if (j == z_size - 1)
                {
                    sw.Write("\n");
                }
            }
        }
        if (test == 1)
        {
            count += 1;
        }
        sw.Close();
        fs.Close();
        String screenshotname = Application.dataPath + "/Tests/ScreenShot/img" + Convert.ToString(timer + 1000) + ".png";
        ScreenCapture.CaptureScreenshot(screenshotname);
    }
}
