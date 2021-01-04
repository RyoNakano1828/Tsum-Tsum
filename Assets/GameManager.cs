using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public GameObject[] ballPrefabs;

    private bool isDragged;

    private ArrayList removeableBallList = new ArrayList();

    private CommonBall firstBall = new CommonBall();

    private bool isPlaying = true;

    public Text timer;

    private Text timerText;

    public int TIME_LIMIT = 60;

    public int TIME_COUNT = 5;

    private int currentScore;

    public Text scoreText;

    void Start()
    {
        timerText = timer.GetComponent<Text>();

        StartCoroutine("CountDown");

        
    }

    private IEnumerator DropBall()
    {
        while (isPlaying)
        {
            //生成されるボールの種類
            int RANDOM_INDEX = Random.Range(0, ballPrefabs.Length);
            //ボールが生成されるx座標の乱数
            float RANDOM_X = Random.Range(-2.0f, 2.0f);
            //ボールが生成される座標のVector
            Vector3 BALL_INITIAL_POSITION = new Vector3(RANDOM_X, 7.0f, 0.0f);
            //ボールの生成
            GameObject clonedBall = Instantiate(ballPrefabs[RANDOM_INDEX]);

            //ボールの座標を設定
            clonedBall.transform.position = BALL_INITIAL_POSITION;

            //次のボールの生成まで待機
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator CountDown()
    {
        int count = TIME_COUNT;
        while (count > 0)
        {
            timerText.text = count.ToString();

            yield return new WaitForSeconds(1.0f);

            count -= 1;
        }
        timerText.text = "Start!";

        yield return new WaitForSeconds(1.0f);

        StartCoroutine("DropBall");

        StartCoroutine("GameTimer");
    }

    private IEnumerator GameTimer()
    {
        int count = TIME_LIMIT;
        while(count > 0)
        {
            timerText.text = count.ToString();

            yield return new WaitForSeconds(1.0f);

            count -= 1;
        }
        timerText.text = "Finish";
        isPlaying = false;
        foreach(GameObject ball in removeableBallList)
        {
            ball.transform.GetComponent<CommonBall>().ResetColor();
        }
    }

    private void ChangeColor(GameObject obj)
    {
        Material ballMaterial = obj.GetComponent<Renderer>().material;

        ballMaterial.SetFloat("_Metallic", 1.0f);
    }

    private void OnDragStart()
    {
        GameObject targetObject = GetCurrentTarget();

        removeableBallList.Clear();

        if (targetObject)
        {
            if (targetObject.name.IndexOf("Ball") != -1)
            {
                firstBall = targetObject.GetComponent<CommonBall>();

                removeableBallList.Add(targetObject);

                firstBall.isAdd = true;

                ChangeColor(targetObject);
            }
        }

    }

    private void OnDragging()
    {
        GameObject targetObject = GetCurrentTarget();

        if (targetObject)
        {
            if(targetObject.name.IndexOf("Ball") != -1)
            {
                CommonBall targetBall = targetObject.transform.GetComponent<CommonBall>();

                if(targetBall.kindOfId == firstBall.kindOfId)
                {
                    if(targetBall.isAdd == false)
                    {
                        removeableBallList.Add(targetObject);

                        targetBall.isAdd = true;

                        ChangeColor(targetObject);
                    }
                }
            }
        }

    }

    private void OnDragEnd()
    {
        int length = removeableBallList.Count;

        if(length >= 3)
        {
            foreach(GameObject ball in removeableBallList)
            {
                currentScore += length;

                scoreText.text = "Score: " + currentScore.ToString();

                Destroy(ball);
            }
            removeableBallList.Clear(); 
        }
        else
        {
            foreach(GameObject ball in removeableBallList)
            {
                ball.transform.GetComponent<CommonBall>().isAdd = false;

                ball.transform.GetComponent<CommonBall>().ResetColor();
            }
        }

    }

    private GameObject GetCurrentTarget()
    {
        GameObject atDeleteTarget = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        if (hit2d)
        {
            atDeleteTarget = hit2d.transform.gameObject;
        }

        return atDeleteTarget;
    }

    
    
    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetMouseButtonDown(0) && isDragged == false)
            {
                isDragged = true;
                OnDragStart();
            }
            else if (Input.GetMouseButton(0) && isDragged == true)
            {
                OnDragging();
            }
            else
            {
                isDragged = false;
                OnDragEnd();
            }
        }
    }
}
