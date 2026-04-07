
using System;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
using unvs.ui;


namespace unvs.players{
    public abstract class BaseSubPlayer
    {
        public PlayerBase owner;
        public abstract void OnPlayerStart(PlayerBase player);
    }
    [Serializable]
    public class AnimExractor: BaseSubPlayer
    {
        [SerializeField]
        public Animator animator;
       
        [SerializeField]
        public BlendTreeInfo[] motions;
        
        
        public virtual void BaseMotion(string name)
        {
            this.motions.PlayBaseLayer(name);
        }

        public void AddtiveMotion(string name)
        {
            this.motions.PlayAddtiveMotion(name);
        }
        public override void OnPlayerStart(PlayerBase player)
        {
            
        }

#if UNITY_EDITOR
        internal void EditorExtractAllAnim(PlayerBase obj)
        {
           animator = obj.GetComponentInChildren<Animator>();
          
           motions = animator.EditorExtractAllMotions().ToArray();
            this.owner = obj;
        }

        
#endif
    }
    [Serializable]
    public class Physical : BaseSubPlayer
    {
        public Transform ArmRoot;
        public Transform ArmTop;
        public Transform LegRoot;
        public Transform LegTop;
        public float ArmLen;

        public float LegLen;
        public float footStepLen;
        private Vector2 startPointArm;
        private Vector2 endPointArm;
        private Vector2 startPointLeg;
        private Vector2 endPointLeg;
        public float walkAngleDegree;
        
        [SerializeField]
        public Vector3 originalScale;

        public void Direction(float v)
        {
            if (owner != null)
            {
                if (v > 0) owner.transform.localScale = originalScale;
                if (v < 0) owner.transform.localScale = new Vector3(-originalScale.x,originalScale.y,originalScale.z);

            }
        }
        public void Direction(Vector2 v)
        {
            if (owner != null)
            {
                var x = v.x- owner.GetComponent<Collider2D>().bounds.center.x;
                if (x > 0) owner.transform.localScale = originalScale;
                if (x < 0) owner.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

            }
        }
        public void Direction(Vector3 v)
        {
            if (owner != null)
            {
                var x = v.x - owner.GetComponent<Collider2D>().bounds.center.x;
                if (x > 0) owner.transform.localScale = originalScale;
                if (x < 0) owner.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

            }
        }
        public void MoveTo(Vector2 v)
        {
            if(owner==null) return;
            owner.CancellationTokenSourceRefresh();
            owner.anims.BaseMotion("walk");
            owner.transform.MoveStep(v, this.footStepLen, out var dir);
           
        }
        public override void OnPlayerStart(PlayerBase player)
        {
            
        }
#if UNITY_EDITOR
        public void EditorPhysicalCalculate(PlayerBase playerBase)
        {
            owner=playerBase;
            originalScale = owner.transform.localScale.CloneToNew();
            playerBase.GismoxDraw -= PlayerBase_GismoxDraw;
            if (ArmRoot != null && ArmTop != null)
            {
                ArmLen = getLen(ArmTop, ArmRoot);
                startPointArm = ArmRoot.GetSegment().Start;
                endPointArm = ArmTop.GetSegment().End;
            }
            if(LegRoot != null && LegTop != null)
            {
                startPointLeg = LegRoot.GetSegment().Start;
                endPointLeg = LegTop.GetSegment().End;
                LegLen = getLen(LegTop, LegRoot);
                footStepLen = calculateFootStep(LegLen, walkAngleDegree);


            }
            
            playerBase.GismoxDraw += PlayerBase_GismoxDraw;

          


        }
        private float calculateFootStep(float legLen, float angle)
        {
            // Đảm bảo góc luôn dương để tránh tính toán ra khoảng cách âm
            float absoluteAngle = Mathf.Abs(angle);

            // Chuyển đổi độ sang Radian
            float angleInRadians = absoluteAngle * Mathf.Deg2Rad;

            // Tính toán khoảng cách (cạnh đối diện góc trong tam giác vuông)
            float stepDistance = legLen * Mathf.Sin(angleInRadians);

            return stepDistance;
        }

        private float getLen(Transform tr, Transform armRoot)
        {
            var dx = 0f;
            while (tr != null && tr != ArmRoot)
            {

                dx += tr.GetSegment().Length;
                tr = tr.parent;
            }
            dx += ArmRoot.GetSegment().Length;
            return dx;
        }

        private void PlayerBase_GismoxDraw()
        {
           if(startPointArm!=Vector2.zero) startPointArm.DrawCircle(0.01f, 1, Color.red);
            if (endPointArm != Vector2.zero) endPointArm.DrawCircle(0.01f, 1,Color.green);
            if (startPointLeg != Vector2.zero) startPointLeg.DrawCircle(0.01f, 1, Color.red);
            if (endPointLeg != Vector2.zero) endPointLeg.DrawCircle(0.01f, 1, Color.green);
        }

        


#endif

    }
    [Serializable]
    public class Dialogue:BaseSubPlayer
    {
        
        public Transform container;
        public Canvas canvas;
        public Image panel;
        public TextMeshProUGUI txt;

       

        public override void OnPlayerStart(PlayerBase player)
        {
            this.owner=player;
            if (canvas != null) canvas.FullSize();
            container.gameObject.SetActive(false);
        }

        public void SayText(string v)
        {
            if(this.owner==null) return;
            if(this.owner.GetComponent<Collider2D>()==null) return;
            var bound = this.owner.GetComponent<Collider2D>().bounds;
            var panelSize = panel.GetSize();
            var center = bound.center.ToScreen();
            var max = bound.max.ToScreen();
            this.txt.text = $"{panelSize.x},{v}";
            container.gameObject.SetActive(true);
            canvas.enabled = true;
            canvas.gameObject.SetActive(true);
            canvas.sortingOrder = -1;
            panel.SetPosition(new Vector2(center.x-panelSize.x/2,max.y+10));

        }
        public void Off(string v)
        {
           
            container.gameObject.SetActive(false);
        }
        private void Owner_OnRuntimeStart(PlayerBase obj)
        {
            this.canvas.FullSize();
            this.canvas.sortingOrder = -1;
        }
#if UNITY_EDITOR
        public void EditorGenerateDialogueUI(PlayerBase owner)
        {
            this.owner = owner;
            container = this.owner.transform.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.PLAYER_DIALOGUE_CONATAINER);
            container.gameObject.SetActive(false);
            canvas = container.AddChildComponentIfNotExist<Canvas>("Canvas");
            canvas.AddComponentIfNotExist<GraphicRaycaster>();
            panel = canvas.transform.AddChildComponentIfNotExist<Image>("Panel");
            var layout = panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            layout.FixFullLayoutChildren();
            txt = panel.AddChildComponentIfNotExist<TextMeshProUGUI>("Text");
            this.owner.OnRuntimeStart += Owner_OnRuntimeStart;
        } 
#endif
    }
    [Serializable]
    public class Bagger : BaseSubPlayer
    {
        private SettingsUIInventory bagger;

        public override void OnPlayerStart(PlayerBase player)
        {
            owner = player;
            bagger = owner.GetComponentInChildrenByName<SettingsUIInventory>(Constants.ObjectsConst.INVENTORY_PANEL_BAGGER);
        }
        public virtual void Show()
        {
            bagger.enabled = true;
            bagger.gameObject.SetActive(true);
            bagger.containerCanvas.enabled = true;
            bagger.containerCanvas.gameObject.SetActive(true);
            bagger.bagger.Show();
        }
        public virtual void Hide()
        {
            bagger.enabled = false;
            bagger.gameObject.SetActive(false);
            bagger.bagger.Hide();
        }
        internal void EditorGenerateBagger(PlayerBase playerBase)
        {
            owner = playerBase;
           bagger = owner.AddChildComponentIfNotExist<SettingsUIInventory>(Constants.ObjectsConst.INVENTORY_PANEL_BAGGER);
            bagger.containerCanvas.gameObject.SetActive(false);
        }
    }

}