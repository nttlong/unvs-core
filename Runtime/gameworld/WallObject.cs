using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;

public class WallObject : MonoBehaviour, IWall
{
    public WallTypeEnum wallType; //<-- enum Left,Right
    public BoxCollider2D coll;
   

    public WallTypeEnum WallType => wallType;

    public BoxCollider2D Coll
    {
        get
        {
            if (coll != null) return coll;
            coll = GetComponent<BoxCollider2D>();
            return coll;
        }
    }

    

    private void Awake()
    {
        this.SetMeOnLayer(Constants.Layers.LIMIT_WALL);
        GetComponent<BoxCollider2D>().isTrigger = false;

    }
    public static IWall GetOrCreateWall(GameObject parent, WallTypeEnum type, Vector2 pos, Vector2 size)
    {
        // 1. Tìm trong các con xem có ai là IWall và đúng WallType không
        var existingWalls = parent.GetComponentsInChildren<WallObject>();
        var targetWall = existingWalls.FirstOrDefault(w => w.WallType == type);

        GameObject wallGo;

        if (targetWall == null)
        {
            // 2. Nếu chưa có thì tạo mới
            wallGo = new GameObject($"Wall_{type}");
            wallGo.transform.SetParent(parent.transform);
            targetWall = wallGo.AddComponent<WallObject>();
            targetWall.wallType = type;
        }
        else
        {
            // 3. Nếu có rồi thì lấy GameObject đó để cập nhật
            wallGo = targetWall.gameObject;
        }

        // 4. Cập nhật thông số vật lý (vị trí và kích thước)
        wallGo.transform.position = pos;

        var box = wallGo.GetComponent<BoxCollider2D>();
        if (box == null) box = wallGo.AddComponent<BoxCollider2D>();

        box.size = size;

        //// Đảm bảo là Static để nhân vật không đẩy được tường
        if (wallGo.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        return targetWall;
    }
    public static void CreateLimitCollider(GameObject goWorld, Collider2D boundingShape2D, out IWall lWall, out IWall rWall)
    {
        if (boundingShape2D == null)
        {
            lWall = null;
            rWall = null;
            return;
        }

        Bounds bounds = boundingShape2D.bounds;
        float centerY = bounds.center.y;
        float height = bounds.size.y;
        float thickness = 1f;

        // Tìm hoặc tạo chốt bên Trái
        lWall = GetOrCreateWall(goWorld, WallTypeEnum.Left,
            new Vector2(bounds.min.x - (thickness / 2f), centerY),
            new Vector2(thickness, height));

        // Tìm hoặc tạo chốt bên Phải
        rWall = GetOrCreateWall(goWorld, WallTypeEnum.Right,
            new Vector2(bounds.max.x + (thickness / 2f), centerY),
            new Vector2(thickness, height));
    }
    
}
