using UnityEngine;

public class GridPoint : MonoBehaviour
{
    //Note:
    //  since this GridPoint class is a MonoBehaviour, it will have to be attached to a gameobject to be valid
    //  otherwise you will have nullreference exceptions during usage

    #region --- events ---
    public delegate void PointChangeErase(Vector3 chunk);
    public static event PointChangeErase OnPointStayErase;
    public delegate void PointChangeAdd(Vector3 chunk);
    public static event PointChangeAdd OnPointStayAdd;
    #endregion

    private Vector3 _chunk = Vector3.zero;
    private float _value = 0f;

    public Vector3 Chunk
    {
        get
        {
            return _chunk;
        }
        set
        {
            _chunk = new Vector3(value.x, value.y, value.z);
        }
    }
    public Vector3 Position
    {
        get
        {
            return this.transform.localPosition;
        }
        set
        {
            this.transform.localPosition = new Vector3(value.x, value.y, value.z);
        }
    }
    public float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }
    public float Color
    {
        get
        {
            return this.GetComponent<Renderer>().material.color.r;
        }
        set
        {
            this.GetComponent<Renderer>().material.color = new Color(1 - value, 0, 0);
        }
    }
    public bool Visible
    {
        get
        {
            return this.GetComponent<Renderer>().enabled;
        }
        set
        {
            this.GetComponent<Renderer>().enabled = value;
        }
    }

    private void OnEnable()
    {
        try
        {
            this.tag = "GridPoint";
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Erase") == true)
        {
            if (OnPointStayErase != null)
            {
                Value = Mathf.Clamp(Value + (0.1f * Time.deltaTime), 0f, 1f);
                Color = Value;
                OnPointStayErase(Chunk);
            }
                
        }    
        else if (other.gameObject.CompareTag("Add") == true)
        {
            if (OnPointStayAdd != null)
            {
                Value = Mathf.Clamp(Value - (0.1f * Time.deltaTime), 0f, 1f);
                Color = Value;
                OnPointStayAdd(Chunk);
            }
                
        }
    }   
    public override string ToString()
    {
        return string.Format("chunk[{0},{1},{2}] position[{3},{4},{5}] value={6}", Chunk.x, Chunk.y, Chunk.z, Position.x, Position.y, Position.z, Value);
    }
}
