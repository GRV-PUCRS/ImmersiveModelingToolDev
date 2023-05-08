using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesteServer : MonoBehaviour
{
    public struct Message
    {
        public string name;
        public int state;
        public int type;
        public Vector3 center;
        public Vector2 size;
        public Vector3 position;
        public Vector3 rotation;
    }
    Message m = new Message();

    public GameObject wall;
    public Dictionary<string, GameObject> walls = new Dictionary<string, GameObject>();

    public void OnReceiveData(string _data)
    {
        m = JsonUtility.FromJson<Message>(_data);

        switch (m.state)
        {
            case 0:
                walls.Add(m.name, Instantiate(wall, m.position, Quaternion.Euler(m.rotation)));
                break;
            case 1:
                Destroy(walls[m.name].gameObject);
                walls.Remove(m.name);
                break;
            case 2:
                if (m.type == 0)
                {
                    walls[m.name].transform.localScale = new Vector3(m.size.x, 0.01f, m.size.y);
                }
                else
                {
                    walls[m.name].transform.localScale = new Vector3(m.size.x, m.size.y, 0.01f);
                }
                walls[m.name].transform.position = m.position;
                walls[m.name].transform.rotation = Quaternion.Euler(m.rotation);
                break;
            default:
                Debug.Log($"Status {m.state} Desconhecido");
                break;
        }
    }
}
