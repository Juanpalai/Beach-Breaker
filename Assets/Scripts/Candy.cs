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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                previusSeleted.DeselectCandy();
                //SelecCandy();
            }

        }
    }

}
