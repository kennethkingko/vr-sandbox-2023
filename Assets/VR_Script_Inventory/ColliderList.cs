using System.Collections.Generic;
using UnityEngine;

public class ColliderList : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> m_colliders;

    public List<GameObject> getColliderList
    {
        get { return m_colliders; }
    }

    private void Start()
    {
        m_colliders = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "InventorySystem")
        {
            if (!m_colliders.Contains(obj))
            {
                m_colliders.Add(obj);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "InventorySystem")
        {
            if (m_colliders.Contains(obj))
            {
                m_colliders.Remove(obj);
            }

        }

    }

    public void resetColliderList(){
        m_colliders.Clear();
    }
}