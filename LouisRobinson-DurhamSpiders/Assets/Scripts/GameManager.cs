using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(AudioSource))]

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;

    [HideInInspector]
    public int lives;

    private bool inMenu = true;

    public GameObject myPrefab;
    public GameObject powerUps;
    public GameObject treasure;
    public GameObject terrain;

    public GameObject playerCam;

    public MainMenu mainMenuScreen;
    public DeathMenu theDeathScreen;
    public PausedMenu pauseScreen;
    public CompletedMenu completedMenuScreen;
    public LevelsMenu levelsScreen;
    private List<GameObject> Spiders = new List<GameObject>();
    private List<GameObject> deadSpiders = new List<GameObject>();
    private List<GameObject> ToReassign = new List<GameObject>();
    private int numberOfSpiders = 0;
    private float deleteDistance = 120.0f;

    private int maxSpiders = 150;

    [HideInInspector]
    public int level;
    [HideInInspector]
    public int treasureFound;

    public GameObject nextLevel;

    float spiderRunDistance = 30.0f;
    float spiderWalkSpeed = 4;

    [HideInInspector]
    public int initialPlayerLives = 10;

    [HideInInspector]
    public Vector3 startingPosition = new Vector3(-72.0f, 130.0f, 285.0f);

    public float waterHeight = 15.0f;

    private float spiderGenerationRate = 3.0f;


    private int[] initHistory = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    private int N;
    private int[] livesHistory;
    private int livesIndex;

    public bool canHide;
    //CursorLockMode wantedMode;
    //CrossHairs ch;

    AudioSource audioData;

    void Start()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);

        activateMainMenu();

        N = initHistory.Length;

        //ch = player.GetComponent<CrossHairs>();
    }

    public void DestroyAll(string tag)
    {
        var objects = GameObject.FindGameObjectsWithTag(tag);
        for (var i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);
        }
        if (tag == "Spider")
        {
            Spiders = new List<GameObject>();
        }
        if (tag == "DeadSpider")
        {
            deadSpiders = new List<GameObject>();
        }
    }

    bool SpiderToPlayerRayCheck(Vector3 spiderPos)
    {
        Vector3 direction = player.transform.position - spiderPos;
        // launch a ray from the spider, see if it hits the player
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(spiderPos, direction, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    void ReassignPlayerAsTarget(GameObject spider)
    {
        spider.GetComponent<SpiderMovement>().enabled = true;
        spider.GetComponent<SpiderMovement>().target = player;
        spider.GetComponent<NavMeshAgent>().speed = 3.5f;
    }

    void CheckReassign()
    {
        // Be careful that spiders that are destroyed for being too far away arent included in ToReassign
        if (ToReassign.Count > 0)
        {
            List<int> ToRemove = new List<int>();

            for (int i = 0; i < ToReassign.Count; i++)
            {
                bool visible = SpiderToPlayerRayCheck(ToReassign[i].transform.position);
                if (visible || Vector3.Distance(player.transform.position, ToReassign[i].transform.position) > 5.0f+ ToReassign[i].GetComponent<SpiderMovement>().DetectionDistance)
                {
                    ReassignPlayerAsTarget(ToReassign[i]);
                    ToRemove.Add(i);
                }
            }

            for (int i = ToRemove.Count - 1; i >= 0; i--)
            {
                ToReassign.RemoveAt(ToRemove[i]);
            }

            Invoke("CheckReassign", 5.0f);
        }
    }

    void ResetSpidersTarget()
    {
        // start the timer for reloading the hiding ability

        if (Spiders.Count > 5)
        {
            Danger();
        }

        for (int i = 0; i < Spiders.Count; i++)
        {
            // if they can see the player  -> reassign the player as the target
            bool visible = SpiderToPlayerRayCheck(Spiders[i].transform.position);
            if (visible)
            {
                ReassignPlayerAsTarget(Spiders[i]);
            }
            else// if they cant see the player -> once the player is over the undetectable distance away reassign
            {
                ToReassign.Add(Spiders[i]);// this can be done be recursive invoking over say 5 seconds
            }
        }
        player.GetComponent<Hiding>().enabled = false;

        player.GetComponent<NotHiding>().enabled = true;
        player.GetComponent<NotHiding>().StartTimer(15);
        Invoke("ResetCanHide", 15.0f);
        Invoke("CheckReassign", 5.0f);
    }

    void ResetCanHide()
    {
        canHide = true;
    }

    void RandomSpiderMovement(GameObject spider)
    {
        float radius = UnityEngine.Random.Range(1.0f, 20.0f);
        Vector3 loc = RandomNavmeshLocation(radius, spider.transform.position);
        spider.GetComponent<SpiderMovement>().enabled = false;
        spider.GetComponent<NavMeshAgent>().SetDestination(loc);
        spider.GetComponent<NavMeshAgent>().speed = UnityEngine.Random.Range(0.2f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && canHide)
        {
            player.GetComponent<NotHiding>().enabled = false;

            canHide = false;
            player.GetComponent<Hiding>().enabled = true;
            player.GetComponent<Hiding>().StartTimer();
            // set all spiders target locations using RandomNavmeshLocation(radius, spider.pos)
            for (int i = 0; i < Spiders.Count; i++)
            {
                RandomSpiderMovement(Spiders[i]);
            }

            // once 10 seconds are up, reset them all to the player
            Invoke("ResetSpidersTarget", 10.0f);
        }

        if (Input.GetKeyDown(KeyCode.P) && inMenu==false)
        {
            if (Time.timeScale == 1)
            {
                audioData.Pause();
                Time.timeScale = 0;
                player.GetComponent<CrossHairs>().enabled = false;
                pauseScreen.gameObject.SetActive(true);
            }
            else if (Time.timeScale == 0)
            {
                audioData.UnPause();
                Time.timeScale = 1;
                player.GetComponent<CrossHairs>().enabled = true;
                pauseScreen.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            TeleportPlayer();
        }

        if (player.transform.position.y < waterHeight)
        {
            RestartGame();
        }
    }

    public void RefreshList()//slow
    {
        List<int> toRemove = new List<int>();

        for (int i = 0; i < Spiders.Count; i++)
        {
            if (Spiders[i].tag == "DeadSpider")
            {
                toRemove.Add(i);
                deadSpiders.Add(Spiders[i]);
            }
        }
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            Spiders.RemoveAt(toRemove[i]);
        }
        numberOfSpiders = Spiders.Count;
        Invoke("DestroyDistantSpiders", 10.0f);
    }

    public void DestroyDistantSpiders()//slow
    {
        List<int> toRemove = new List<int>();
        for (int i = 0; i < Spiders.Count; i++)
        {
            if (Vector3.Distance(player.transform.position, Spiders[i].transform.position) > deleteDistance )
            {
                if (ToReassign.Contains(Spiders[i]))
                {
                    ToReassign.Remove(Spiders[i]);// if a spider is in ToReassign, remember to remove it!
                }
                toRemove.Add(i);
            }
        }
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            Destroy(Spiders[toRemove[i]]);
            Spiders.RemoveAt(toRemove[i]);
        }
        toRemove = new List<int>();
        for (int i = 0; i < deadSpiders.Count; i++)
        {
            if (Vector3.Distance(player.transform.position, deadSpiders[i].transform.position) > deleteDistance)
            {
                toRemove.Add(i);
            }
        }
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            Destroy(deadSpiders[toRemove[i]]);
            deadSpiders.RemoveAt(toRemove[i]);
        }
        Invoke("RefreshList", 10.0f);
    }

    public Vector3 RandomNavmeshLocation(float radius, Vector3 target)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += target;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private void AdaptDifficulty()
    {
        // determine how skillful the player is from livesHistory
        // implement some fn of skill, level -> spiderGenerationRate
        livesIndex = (livesIndex + 1) % N;// N items in livesHistory
        livesHistory[livesIndex] = lives;

        int livesNStepsAgo = livesHistory[(livesIndex + 1) % N];
        int livesLost = livesNStepsAgo - livesNStepsAgo;

        string skill = "Average";
        if (livesLost == 0)
        {
            skill = "Good";
        }
        else if (livesLost > 1)
        {
            skill = "Bad";
        }

        spiderGenerationRate = 6.0f*Convert.ToInt32(level == 1) + 3.0f*Convert.ToInt32(level==2)+ Convert.ToInt32(level==3);
        //if (level == 1)
        //{
        //    spiderGenerationRate = 6.0f;
        //}
        //else if (level == 2)
        //{
        //    spiderGenerationRate = 3.0f;
        //}
        //else if (level == 3)
        //{
        //    spiderGenerationRate = 1.0f;
        //}


        if (skill == "Good")
        {
            spiderGenerationRate *= 0.5f;
        }
        if (skill == "Bad")
        {
            spiderGenerationRate *= 1.5f;
        }
    }

    private void RecursiveGeneration()
    {
        AdaptDifficulty();

        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.2 && numberOfSpiders < maxSpiders)
        {
            GenerateSpider();
            //Debug.Log("Number of spiders: " + numberOfSpiders.ToString());
        }
        Invoke("RecursiveGeneration", spiderGenerationRate);
    }

    private void Danger()
    {
        if (!audioData.isPlaying)
        {
            audioData.Stop();
            audioData.Play(0);
        }
    }

    private void GenerateSpider()
    {
        float scale = UnityEngine.Random.Range(0.0f, 1.0f);
        scale = scale * scale * 4 + 1;

        Ray ray = playerCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 start = RandomNavmeshLocation(50.0f, player.transform.position + ray.direction * 50.0f);

        if (start != Vector3.zero)
        {
            GameObject spider = Instantiate(myPrefab, start, Quaternion.identity);
            SpiderMovement sm = spider.GetComponent<SpiderMovement>();
            sm.player = player;

            spider.transform.localScale = new Vector3(scale, scale, scale);

            if (player.GetComponent<Hiding>().enabled)
            {
                RandomSpiderMovement(spider);
            }
            else
            {
                sm.target = player;
            }

            sm.RunDistance = spiderRunDistance;
            sm.WalkSpeed = spiderWalkSpeed;
            Spiders.Add(spider);
            numberOfSpiders += 1;
        }
    }

    private void TeleportPlayer()
    {

        if (player.GetComponent<PlayerCollider>().Boosts > 0)
        {
            player.GetComponent<PlayerCollider>().Boosts -= 1;

            player.GetComponent<CharacterController>().enabled = false;

            Ray ray = playerCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            Vector3 teleportPos = player.transform.position + ray.direction * 100.0f;
            // if the direction we move the player is downwards, add 10m onto the height before sampling the navmesh
            teleportPos.y = teleportPos.y + 10 * Convert.ToInt32(ray.direction.y < 0);
            float rad = 10+50 * Convert.ToInt32(ray.direction.y < 0);
            if (NavMesh.SamplePosition(teleportPos, out hit, rad, 1))
            {
                finalPosition = hit.position;
                Debug.Log("Teleporting player...");
                finalPosition.y = finalPosition.y + 10.0f;
                player.transform.position = finalPosition;
            }
            else
            {
                player.transform.position = player.transform.position + ray.direction * 100.0f;
            }


            player.GetComponent<CharacterController>().enabled = true;
        }


    }



    public void activateMainMenu()
    {
        player.GetComponent<Hiding>().enabled = false;
        player.GetComponent<DisplayLives>().enabled = false;
        player.GetComponent<CrossHairs>().enabled = false;
        player.GetComponent<NotHiding>().enabled = false;

        mainMenuScreen.gameObject.SetActive(true);
        levelsScreen.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(false);
        theDeathScreen.gameObject.SetActive(false);
        completedMenuScreen.gameObject.SetActive(false);
        Time.timeScale = 0;
        inMenu = true;
    }

    public void LostLife()
    {
        if (lives > 0)
        {
            lives -= 1;
        }
        else if (lives == 0)
        {
            Debug.Log("Dead");
            RestartGame();
        }
    }

    public void CompletedLevel()
    {
        player.GetComponent<DisplayLives>().enabled = false;
        player.GetComponent<CrossHairs>().enabled = false;
        player.GetComponent<NotHiding>().enabled = false;
        Time.timeScale = 0;

        completedMenuScreen.gameObject.GetComponent<CompletedMenu>().text.text = "Completed Level " + level.ToString() + "!";

        if (level == 3)
        {
            nextLevel.active = false;
        }
        else
        {
            nextLevel.active = true;
        }

        completedMenuScreen.gameObject.SetActive(true);
        Cursor.visible = true;
        inMenu = true;
    }

    public void NextLevel()
    {
        if (level == 1)
        {
            L2();
        }
        else if (level == 2)
        {
            L3();
        }
    }

    public void FoundTreasure()
    {
        treasureFound += 1;
        //player.GetComponent<DisplayLives>().numTreasure = treasureFound;
        if (treasureFound == 10)
        {
            CompletedLevel();
        }
    }

    public void OpenLevelsMenu()
    {
        player.GetComponent<DisplayLives>().enabled = false;
        player.GetComponent<CrossHairs>().enabled = false;
        player.GetComponent<NotHiding>().enabled = false;

        levelsScreen.gameObject.SetActive(true);
        Cursor.visible = true;
        inMenu = true;
    }

    public void L1()
    {
        // set difficulty
        level = 1;

        spiderGenerationRate = 6.0f;

        fpsController.m_RunSpeed = 20.0f;

        spiderRunDistance = 30.0f;
        spiderWalkSpeed = 4;

        Reset();
    }

    public void L2()
    {
        // set difficulty
        level = 2;

        spiderGenerationRate = 3.0f;

        fpsController.m_RunSpeed = 16.0f;

        spiderRunDistance = 35.0f;
        spiderWalkSpeed = 6;

        Reset();
    }

    public void L3()
    {
        // set difficulty
        level = 3;

        spiderGenerationRate = 1.0f;

        fpsController.m_RunSpeed = 12.0f;

        spiderRunDistance = 45.0f;
        spiderWalkSpeed = 10;

        Reset();
    }

    public void RestartGame()
    {
        player.GetComponent<DisplayLives>().enabled = false;
        player.GetComponent<CrossHairs>().enabled = false;
        player.GetComponent<NotHiding>().enabled = false;
        Time.timeScale = 0;
        theDeathScreen.gameObject.SetActive(true);

        audioData.Play(0);

        Cursor.visible = true;
    }

    public void Reset()
    {
        audioData.Pause();

        livesHistory = initHistory;
        livesIndex = 0;

        treasureFound = 0;
        player.GetComponent<PlayerCollider>().Boosts = 3;

        Cursor.lockState = CursorLockMode.Locked;

        canHide = true;

        Spiders = new List<GameObject>();
        deadSpiders = new List<GameObject>();
        ToReassign = new List<GameObject>();

        DestroyAll("PowerUp");
        DestroyAll("Treasure");
        GameObject powerUpGroup = Instantiate(powerUps, new Vector3(208.8121f, 162.1835f, 77.38285f), Quaternion.identity);
        GameObject coins = Instantiate(treasure, new Vector3(-431.271f, -336.4203f, -1080.188f), Quaternion.identity);

        player.GetComponent<DisplayLives>().enabled = true;
        player.GetComponent<NotHiding>().enabled = true;
        player.GetComponent<Hiding>().enabled = false;
        player.GetComponent<CrossHairs>().enabled = true;

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = startingPosition;
        player.GetComponent<CharacterController>().enabled = true;

        mainMenuScreen.gameObject.SetActive(false);
        theDeathScreen.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(false);
        completedMenuScreen.gameObject.SetActive(false);
        levelsScreen.gameObject.SetActive(false);

        Time.timeScale = 1;
        lives = initialPlayerLives;

        inMenu = false;

        DestroyAll("DeadSpider");
        DestroyAll("Spider");


        Invoke("GenerateSpider", 2.0f);
        Invoke("GenerateSpider", 5.0f);
        if (level == 2) 
        {
            Invoke("GenerateSpider", 10.0f);
            Invoke("GenerateSpider", 15.0f);
        }
        else if (level == 3)
        {
            Invoke("GenerateSpider", 10.0f);
            Invoke("GenerateSpider", 17.0f);
            Invoke("GenerateSpider", 15.0f);
            Invoke("GenerateSpider", 18.0f);
        }
        Invoke("RecursiveGeneration", 20.0f);

        Invoke("RefreshList", 10.0f);
    }
}
