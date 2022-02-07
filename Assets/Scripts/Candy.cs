using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    private static Color seletedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Candy previusSeleted = null;

    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    private Vector2[] adjacentDirectios = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    public int id;

    public Vector3 objective;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objective = Vector3.zero;
    } 
    
    // Update is called once per frame
    private void Update()
    {
        if (objective != Vector3.zero)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, objective, 5 * Time.deltaTime);
        }
      
    }
    private void SelecCandy()
    {
        isSelected = true;
        spriteRenderer.color = seletedColor;
        previusSeleted = gameObject.GetComponent<Candy>();
    }

    private void DeselectCandy()
    {
        isSelected=false;
        spriteRenderer.color = Color.white;
        previusSeleted = null;
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
            if(previusSeleted==null)
            {
                SelecCandy();
            }
            else
            {
                if (CanSwipe())
                {
                    SwappingCandy(previusSeleted);
                    previusSeleted.DeselectCandy();
                }
                else
                {
                    previusSeleted.DeselectCandy();
                    SelecCandy();
                }
                //SelecCandy();
            }

        }
    }

    public void SwappingCandy(Candy newCandy)
    {
        //If the carmels are the same, they will not change among themselves.
        if (spriteRenderer.sprite == newCandy.GetComponent<SpriteRenderer>().sprite) return;

        this.objective = newCandy.transform.position;
        newCandy.GetComponent<Candy>().objective = this.transform.position;
    }

    //Method to find the neighboring candy
    private GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);
        if(hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    //Method to find all neighboring candies
    private List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach(Vector2 direction in adjacentDirectios)
        {
            neighbors.Add(GetNeighbor(direction));
        }

        return neighbors;
    }

    //conditional to know if the change is possible

    private bool CanSwipe()
    {
        return GetAllNeighbors().Contains(previusSeleted.gameObject);
    }

    private List<GameObject> FindMatch(Vector2 direction)
    {
        //The query of the neighbors in the direction of the parameter
        List<GameObject> matchingCandies = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position,direction);

        while(hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
        {
            matchingCandies.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position,direction);
        }

        //Neighborhood consultation in the other direction
        hit = Physics2D.Raycast(this.transform.position, -direction);

        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
        {
            matchingCandies.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, -direction);
        }

        return matchingCandies;
    }

    private bool ClearMatch(Vector2[] directions)
    {

    }
}
