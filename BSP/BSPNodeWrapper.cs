using GameEngine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

class BSPNodeWrapper : MonoBehaviour
{
    public BSPNode<IPoly> node;

    public bool draw = true;

    public static BSPNodeWrapper Create(string name, BSPNode<IPoly> node)
    {
        return new GameObject(name).AddComponent<BSPNodeWrapper>().Init(node);
    }

    private BSPNodeWrapper Init(BSPNode<IPoly> node)
    {
        this.node = node;
        if(node.inside == null)
        {
            //var left = new GameObject("left: null");
            //left.transform.parent = transform;
        }
        else
        {
            var left = Create("left", node.inside);
            left.transform.parent = transform;
        }
        if(node.outside == null)
        {
            //var right = new GameObject("right: null");
            //right.transform.parent = transform;
        }
        else
        {
            var right = Create("right", node.outside);
            right.transform.parent = transform;
        }
        return this;
    }

    private void Start()
    {

    }

    private void Update()
    {
#if UNITY_EDITOR
        if(draw)
        {
            if(Selection.Contains(gameObject))
            {
                node.poly.working.Draw(Color.white);
                Draw(node.inside, Color.red);
                Draw(node.outside, Color.blue);
            }
        }
        
#endif
    }

    private void Draw<T>(BSPNode<T> node, Color color) where T : IPoly
    {
        if(node == null || node.poly == null || node.poly.working == null)
        {
            return;
        }
        node.poly.working.Draw(color);
        Draw(node.inside, color);
        Draw(node.outside, color);
    }
}