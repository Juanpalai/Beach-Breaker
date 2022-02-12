using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public static BoardManager instance;

    public List<GameObject> prfabs = new List<GameObject>();
    public GameObject currentCandy;
    public int xSize, ySize;
    int idx = -1;

    private GameObject[,] candies;
    public bool isShifting { get; private set; }

    private Candy selectedCandy;

    //Minimum number of neighboring candies to get a match
    public const int MiniCandiesMatch = 2;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Vector2 offset = currentCandy.GetComponent<BoxCollider2D>().size;
        CrateInitialBoard(offset);
    }

    //Initialize the candies array
    private void CrateInitialBoard(Vector2 offset)
    {
        candies = new GameObject[xSize, ySize];

        float starX = this.transform.position.x;
        float starY = this.transform.position.y;



        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                GameObject newCandy = Instantiate(
                    currentCandy,
                    new Vector3(starX + (offset.x * i),
                                starY + (offset.y * j),
                                0),
                    currentCandy.transform.rotation);

                newCandy.name = string.Format("Candy[{0}][{1}]", i, j);

                //conditional to prevent candies appearing together
                do
                {
                    idx = Random.Range(0, prfabs.Count);
                } while ((i > 0 && idx == candies[i - 1, j].GetComponent<Candy>().id) ||
                         (j > 0 && idx == candies[i, j - 1].GetComponent<Candy>().id));


                //obtain a prefab
                GameObject sprite = prfabs[idx];

                //assign the sprite and animator from our pefabs
                newCandy.GetComponent<SpriteRenderer>().sprite = sprite.GetComponent<SpriteRenderer>().sprite;
                newCandy.GetComponent<Animator>().runtimeAnimatorController = sprite.GetComponent<Animator>().runtimeAnimatorController;


                //assign an ID
                newCandy.GetComponent<Candy>().id = idx;

                newCandy.transform.parent = this.transform;

                candies[i, j] = newCandy;
            }
        }
    }

    public void FindNullCandies()
    {
        
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (candies[i, j].GetComponent<SpriteRenderer>().sprite == null)
                {
                    MakeCandies(i, j);
                    
                }
            }
        }
    }

    private void MakeCandies(int x, int y)
    {

        isShifting = true;
        GameObject spriteRenderer = new GameObject();        
        //get a new random id
        idx = Random.Range(0, prfabs.Count);
       
        //obtain a prefab
        GameObject sprite = prfabs[idx];

        //assign the sprite of our pefabs
        spriteRenderer.GetComponent<SpriteRenderer>().sprite = sprite.GetComponent<SpriteRenderer>().sprite;       
        spriteRenderer.GetComponent<Animator>().runtimeAnimatorController = sprite.GetComponent<Animator>().runtimeAnimatorController;
        spriteRenderer.GetComponent<Animator>().enabled = true;



        //assign an ID
        spriteRenderer.GetComponent<Candy>().id = idx;        

        candies[x, y] = spriteRenderer;
        isShifting = false;
    }

}


