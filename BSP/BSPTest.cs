using GameEngine.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class BSPTest : MonoBehaviour
{
    public int width = 10;

    BSPTree<IPoly> tree;

    public void Start()
    {
        List<IPoly> polygons = new List<IPoly>();
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(true)
                {
                    polygons.Add(Primitive.MakeBox(new Vector3(i * 1.1f + (j % 2) * 0.5f, 0f, j * 1.1f), Vector2.one));
                }
            }
        }

        tree = new BSPTree<IPoly>(polygons.ToArray(), true);
        BSPNode<IPoly> root = tree.root;
        BSPNodeWrapper.Create("root", root);
    }

    public void Update()
    {
        IPoly poly = tree.GetPoly(transform.position);
        if(poly != null)
        {
            poly.Draw(Color.yellow);
        }
        else
        {
            Debug.DrawLine(Vector3.zero, Vector3.up);
        }
    }
}