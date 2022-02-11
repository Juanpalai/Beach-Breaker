using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public static BoardManager instance;

    public List<GameObject> prfabs = new List<GameObject>();
    public GameObject currentCandy;
    public int xSize, ySize;

    private GameObject[,] candies;
    public bool isShifting { get; private set; }

    private Candy selectedCandy;

    //Minimum number of neighboring candies to get a match
    public const int MiniCandiesMatch = 2;


    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
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
    private void CrateInitialBoard( Vector2 offset)
    {
        candies = new GameObject[xSize, ySize];       

        float starX = this.transform.position.x;
        float starY = this.transform.position.y;

        int idx = -1;

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

                //conditional to prevent candies from appearing together
                do
                {
                    idx = Random.Range(0, prfabs.Count);
                } while ((i > 0 && idx == candies[i - 1, j].GetComponent<Candy>().id) ||
                         (j > 0 && idx == candies[i, j - 1].GetComponent<Candy>().id));


                //obtain a prefab
                GameObject sprite = prfabs[idx];

                //assign the sprite of our pefabs
                newCandy.GetComponent<SpriteRenderer>().sprite = sprite.GetComponent<SpriteRenderer>().sprite;
                newCandy.GetComponent<Animator>().runtimeAnimatorController = sprite.GetComponent<Animator>().runtimeAnimatorController;
                

                //assign an ID
                newCandy.GetComponent<Candy>().id = idx;

                newCandy.transform.parent = this.transform;

                candies[i, j] = newCandy;
            }
        }
    } 
    
    public IEnumerator FindNullCandies()
    {
        for(int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(candies[i, j].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MakeCandiesFall(i, j));
                    break;
                }
            }
        }
    }

    private IEnumerator MakeCandiesFall(int x, int yStart,
                                        float shiftDelay = 0.05f)
    {

        isShifting = true;

        List<GameObject> renderes = new List<GameObject>();
        int nullCandies = 0;

        for (int y = yStart; y < ySize; y++)
        {
            GameObject spriteRenderer = candies[x, y];
            if (spriteRenderer.GetComponent<SpriteRenderer>().sprite == null)
            {
                nullCandies++;
            }
            renderes.Add(spriteRenderer);
        }

        for (int i = 0; i < nullCandies; i++)
        {            

            yield return new WaitForSeconds(shiftDelay);
            for (int j = 0; j < renderes.Count - 1; j++)
            {
                renderes[j].GetComponent<SpriteRenderer>().sprite = renderes[j + 1].GetComponent<SpriteRenderer>().sprite;
                renderes[j].GetComponent<Animator>().runtimeAnimatorController = renderes[j + 1].GetComponent<Animator>().runtimeAnimatorController;
                if (renderes.Count == 1)
                {
                    renderes[j + 1].GetComponent<SpriteRenderer>().sprite = GetNewCandy(x, yStart - 1).GetComponent<SpriteRenderer>().sprite;
                    renderes[j + 1].GetComponent<Animator>().runtimeAnimatorController = GetNewCandy(x, yStart - 1).GetComponent<Animator>().runtimeAnimatorController;
                }
            }
        }

        isShifting = false;
    }

    private GameObject GetNewCandy(int x, int y)
    {
        List<GameObject> possibleCandies = new List<GameObject>();
        possibleCandies.AddRange(prfabs);


        if (x > 0)
        {
            possibleCandies.Remove(candies[x - 1, y]);
        }
        if (x < xSize - 1)
        {
            possibleCandies.Remove(candies[x + 1, y]);
        }
        if (y > 0)
        {
            possibleCandies.Remove(candies[x, y - 1]);
        }

        return possibleCandies[Random.Range(0, possibleCandies.Count)];
    }

}
