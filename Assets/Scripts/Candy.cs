using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Candy previousSelected = null;

    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    public int id;

    private Vector2[] adjacentDirections = new Vector2[]
    {
        Vector2.down,
        Vector2.left,
        Vector2.up,
        Vector2.right,
    };

    public Vector3 objective;

    GameObject newCandy;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objective = Vector3.zero;
    }

    private void OnMouseDown()
    {
        if (spriteRenderer.sprite == null || BoardManager.instance.isShifting)
        {
            return;
        }

        if (isSelected)
        {
            DeselectCandy();
        }
        else
        {
            if (previousSelected == null)
            {
                SelectCandy();
            }
            else
            {
                if (CanSwipe())
                {
                    SwapSprite(previousSelected.gameObject);
                    previousSelected.Invoke("FindAllMatches", 0.25f);

                    previousSelected.DeselectCandy();
                    Invoke("FindAllMatches", 0.25f);
                }
                else
                {
                    previousSelected.DeselectCandy();
                    SelectCandy();
                }
            }
        }
    }

    private void Update()
    {
        if (objective != Vector3.zero)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, objective, 5 * Time.deltaTime);
        }
    }

    private void SelectCandy()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Candy>();
    }

    private void DeselectCandy()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    public void SwapSprite(GameObject candy)
    {
        if (id == candy.GetComponent<Candy>().id)
        {
            return;
        }

        this.objective = candy.transform.position;
        candy.GetComponent<Candy>().objective = this.transform.position;
    }

    private GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach (Vector2 direction in adjacentDirections)
        {
            neighbors.Add(GetNeighbor(direction));
        }


        return neighbors;
    }

    private bool CanSwipe()
    {
        return GetAllNeighbors().Contains(previousSelected.gameObject);
    }

    //Consulta vecinos en direccion del parametro
    private List<GameObject> FindMatch(Vector2 direction)
    {
        List<GameObject> matchingCandies = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        int i = 0;

        while (hit.collider != null && hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
        {
            matchingCandies.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, direction);
        }

        return matchingCandies;
    }

    private bool ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingCandies = new List<GameObject>();

        foreach (Vector2 direction in directions)
        {
            matchingCandies.AddRange(FindMatch(direction));
        }

        if (matchingCandies.Count >= BoardManager.MiniCandiesMatch)
        {
            foreach (GameObject candy in matchingCandies)
            {
                candy.GetComponent<Animator>().SetBool("destroid", true);
                
                StartCoroutine(DeleteCandy(candy));
            }

            return true;
        }

        return false;
    }

    public void FindAllMatches()
    {
        if (spriteRenderer.sprite == null)
        {
            return;
        }

        bool hMatch = ClearMatch(new Vector2[2]
        {
            Vector2.left,
            Vector2.right,
        });

        bool vMatch = ClearMatch(new Vector2[2]
        {
            Vector2.up,
            Vector2.down,
        });
        if(hMatch || vMatch)
        {
            GetComponent<Animator>().SetBool("destroid", true);
            
        }
    }


    private IEnumerator DeleteCandy(GameObject candy)
    {
        
            yield return new WaitForSeconds(1.2f);
            candy.GetComponent<SpriteRenderer>().sprite = null;
            spriteRenderer.sprite = null;


    }
    
}

