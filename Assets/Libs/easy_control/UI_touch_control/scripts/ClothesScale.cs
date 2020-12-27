using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClothesScale : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    private float minScale = 0.3f;
    private float maxScale = 3f;
    private int scaleDivisor = 800;

    private Touch oldTouch1;  //上次触摸点1(手指1)  
    private Touch oldTouch2;  //上次触摸点2(手指2)  

    //点击位置与衣服中心点位置的距离差
    private Vector2 Distance;
    //衣服的位置
    private Vector2 ClothesPosition;
    //衣服改变的位置
    private Vector2 ClothesChangePosition;
    //衣服改变的大小
    private Vector2 ClothesChangeScale = new Vector2(1, 1);
    private bool IsDrag;

    void Start()
    {
        this.ClothesPosition = this.GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(this.IsDrag)
        {
            this.ClothesChangePosition = new Vector2(Input.mousePosition.x * 1280 / Screen.width + Distance.x, Input.mousePosition.y * 720 / Screen.height + Distance.y);
            this.GetComponent<RectTransform>().anchoredPosition = ClothesChangePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.transform.localScale = this.ClothesChangeScale * 0.98f;
        Distance = new Vector2(this.GetComponent<RectTransform>().anchoredPosition.x - Input.mousePosition.x * 1280 / Screen.width, 
            this.GetComponent<RectTransform>().anchoredPosition.y - Input.mousePosition.y * 720 / Screen.height);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.transform.localScale = this.ClothesChangeScale * 1f;
    }

    void Update()
    {
        //没有触摸  
        if (Input.touchCount <= 0)
        {
            this.IsDrag = false;
            return;
        }
        if (Input.touchCount == 1)
        {
            this.IsDrag = true;
            //判断衣服是否移出屏幕
            this.ClothesPosition = this.GetComponent<RectTransform>().anchoredPosition;

            if (this.ClothesPosition.x < 0 || this.ClothesPosition.x > 1280 || this.ClothesPosition.y < 0 || this.ClothesPosition.y > 720)
            {
                if (this.ClothesPosition.x < 0)
                {
                    this.ClothesChangePosition.x = 0;
                }
                if (this.ClothesPosition.x > 1280)
                {
                    this.ClothesChangePosition.x = 1280;
                }
                if (this.ClothesPosition.y < 0)
                {
                    this.ClothesChangePosition.y = 0;
                }
                if (this.ClothesPosition.y > 720)
                {
                    this.ClothesChangePosition.y = 720;
                }
                //this.GetComponent<RectTransform>().anchoredPosition = this.ClothesChangePosition;
            }
        }
        //控制衣服缩放
        else
        {
            this.IsDrag = false;
            //多点触摸, 放大缩小  
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);

            //第2点刚开始接触屏幕, 只记录，不做处理  
            if (newTouch2.phase == TouchPhase.Began)
            {
                oldTouch2 = newTouch2;
                oldTouch1 = newTouch1;
                return;
            }

            //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型  
            float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
            float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

            //两个距离之差，为正表示放大手势， 为负表示缩小手势  
            float offset = newDistance - oldDistance;

            //放大因子， 一个像素按 0.01倍来算(100可调整)  
            float scaleFactor = offset / scaleDivisor;
            Vector2 localScale = transform.localScale;
            Vector2 scale = new Vector3(localScale.x + scaleFactor, localScale.y + scaleFactor);

            //最小缩放到 0.3 倍  
            if (scale.x > minScale && scale.x < maxScale && scale.y > minScale && scale.y < maxScale)
            {
                this.transform.localScale = scale;
                this.ClothesChangeScale = this.transform.localScale;
            }
            //记住最新的触摸点，下次使用  
            oldTouch1 = newTouch1;
            oldTouch2 = newTouch2;
        }
    }
}