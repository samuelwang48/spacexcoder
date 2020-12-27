using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
/*
	成都时代互动科技有限公司 www.epoching.com 
                 
	By Jeremy
*/
public class Mouse_drag_rotate_scale_control_ui : MonoBehaviour
{
    //drag variable
    #region 
    [Header("Does it need to drag")]
    public bool is_dragable;

    //is dragging
    private bool is_dragging;

    //发送射线摄像机到碰撞体 Z 轴上的距离
    private float distance_z;

    //点击拖拽时，鼠标到物体中心的偏差距离
    private Vector3 drag_offset;
    #endregion

    //rotation variable
    #region 
    [Header("Does it need to rotate")]
    public bool is_rotation;

    [Header("Rotational speed 0~1")]
    [Range(0, 1)]
    public float rotation_speed;
    #endregion

    //scale variable
    #region 
    [Header("Does it need to scale")]
    public bool is_scale;

    [Header("max scale and min scale")]
    public float max_scale;
    public float min_scale;
    #endregion

    //the statu if the mouse is down
    private bool is_mouse_down = false;

    //the mouse position
    private float mouse_position_x;
    private float mouse_position_y;

    // Update is called once per frame
    void Update()
    {
        //set the mouse is down,鼠标是否点中物体
        #region 
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {

            this.is_mouse_down = true;
            this.mouse_position_x = Input.mousePosition.x;
            this.mouse_position_y = Input.mousePosition.y;


#if IPHONE || ANDROID
			if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
            {
                if (this.is_dragable && this.is_dragging == false)
                {
                    this.is_dragging = true;
                }              
            }
            else
            {

            }
        }
        #endregion

        //鼠标抬起，mouse is up
        #region 
        else if (Input.GetMouseButtonUp(0))
        {
            this.is_mouse_down = false;

            //鼠标抬起 不再拖动，尺寸大小还原
            if (this.is_dragging == true)
            {
                this.is_dragging = false;
            }
        }
        #endregion

        //drag and rotate
        #region 
        if (this.is_mouse_down)
        {
            //drag
            #region 
            if (this.is_dragable && this.is_dragging)
            {
                this.GetComponent<RectTransform>().position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            #endregion

            //rotation
            #region 
            else if (this.is_rotation == true)
            {
                //if (this.rotation_type == Rotation_type.rotation_along_x)
                //{
                //    transform.Rotate(Vector3.down * (Input.mousePosition.x - this.mouse_position_x) * this.rotation_speed, Space.World);
                //}
                //else if (this.rotation_type == Rotation_type.rotation_along_y)
                //{
                //    transform.Rotate(Vector3.right * (Input.mousePosition.y - this.mouse_position_y) * this.rotation_speed, Space.World);
                //}

                Vector2 deltaPos = new Vector2((Input.mousePosition.x - this.mouse_position_x), (Input.mousePosition.y - this.mouse_position_y));

                if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
                {
                    if (Mathf.Abs(deltaPos.x) > 5)
                    {
                        //transform.Rotate(Vector3.down * deltaPos.x * this.rotation_speed, Space.Self); //todo 下次优化 把0.1提出去
                        this.transform.Rotate(new Vector3(0, 0, deltaPos.x * this.rotation_speed));
                    }


                  
                }
                else
                {
                    if (Mathf.Abs(deltaPos.y) > 5)
                    {
                        //transform.Rotate(Vector3.right * deltaPos.y * this.rotation_speed, Space.World); //todo 下次优化 把0.1提出去
                        this.transform.Rotate(new Vector3(0, 0, deltaPos.y * this.rotation_speed));
                    }
                }

                //float distance = Mathf.Sqrt(Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.y, 2));
                //transform.Rotate(Vector3.right * distance * this.rotation_speed, Space.Self);
                //this.transform.Rotate(new Vector3(0, 0, distance));


                this.mouse_position_x = Input.mousePosition.x;
                this.mouse_position_y = Input.mousePosition.y;
            }
            #endregion
        }
        #endregion

        //scale
        #region 
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (this.transform.localScale.x > this.min_scale)
                this.transform.localScale = this.transform.localScale * 0.98f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (this.transform.localScale.x < this.max_scale)
                this.transform.localScale = this.transform.localScale * 1.02f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") == 0)
        {
        }
        #endregion
    }
}

