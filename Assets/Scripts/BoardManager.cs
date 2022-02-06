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
                //assign an ID
                newCandy.GetComponent<Candy>().id = idx;

                newCandy.transform.parent = this.transform;

                candies[i, j] = newCandy;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
