using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using IntegralHelp;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using System.Web.SessionState;
using Hidistro.Core;

namespace Hidistro.UI.Web.API
{
    public class IntegralHandler : IHttpHandler, IRequiresSessionState
    {
        IntegralService service = new IntegralService();

        #region 选择方法

        public void ProcessRequest(HttpContext context)
        {
            string method = context.Request["Method"];
            switch (method)
            {
                case "ClickIntegralLoad":
                    ClickIntegralLoad(context);
                    break;
                case "SaveContent":
                    SaveContent(context);
                    break;
                case "SelPointsForConten":
                    SelPointsForConten(context);
                    break;
                case "ClosePointsFor":
                    DeletePointsFor(context);
                    break;
                case "GetPointsForByPSU":
                    GetPointsForByPSU(context);
                    break;
                case "SelIntegralToNow":
                    SelIntegralToNow(context);
                    break;
                case "enoughPointFor":
                    enoughPointFor(context);
                    break;
                case "InsertPointsForMoney":
                    InsertPointsForMoney(context);
                    break;
                case "DelPointsFor":
                    DelPointsFor(context);
                    break;
                case "DelShoppointdfor":
                    DelShoppointdfor(context);
                    break;
            }
        }

        #endregion

        #region 删除积分金额记录

        public void DelPointsFor(HttpContext context)
        {
            int UserId = Globals.GetCurrentMemberUserId();//获取用户编号
            bool isSuccess = service.DeletePointsFor(UserId);
            context.Response.Write(isSuccess);
        }

        #endregion

        #region 添加积分兑换金额记录

        public void InsertPointsForMoney(HttpContext context)
        {
            int type = Convert.ToInt32(context.Request["type"]);//获取购物类型
            bool isSuccess = service.InsertPointsForMoney(Convert.ToInt32(context.Request["PointsForMoney"]), Globals.GetCurrentMemberUserId(), type);
            context.Response.Write(isSuccess);
        }


        #endregion

        #region 判断现有积分是否足够兑换

        public void enoughPointFor(HttpContext context)
        {
            DataTable dt = service.SelPointsForConten(Globals.GetCurrentMemberUserId());
            decimal IntegralSet = 0;
            foreach (DataRow item in dt.Rows)
            {
                if (!String.IsNullOrEmpty(item["IntegralSet"].ToString()))
                {
                    IntegralSet += Convert.ToDecimal(item["IntegralSet"]);//购物车兑换总积分
                }
            }
            int Points = service.Points(Globals.GetCurrentMemberUserId());//现有积分
            if (IntegralSet > Points)
            {
                context.Response.Write("false");
            }
            else
            {
                context.Response.Write("true");
            }
        }

        #endregion

        #region 根据商品编号查询该商品是否能积分兑换

        public void SelIntegralToNow(HttpContext context)
        {
            int ProductId = Convert.ToInt32(context.Request["ProductId"]);//获取商品编号
            bool IsSuccess = service.SelIntegralToNow(ProductId);
            context.Response.Write(IsSuccess);
        }

        #endregion

        #region 根据商品编号,商品唯一标识,用户编号,查询积分内容加载模态框
        protected void ClickIntegralLoad(HttpContext context)
        {
            int ProductId = Convert.ToInt32(context.Request["ProductId"]);//获取商品编号
            int UserId = Globals.GetCurrentMemberUserId();//获取用户编号
            string SkuId = context.Request["SkuId"];//商品编号唯一标识
            DataTable dt = service.SelMaxIntegral(UserId, ProductId, SkuId);//查询最大积分值
            var lists = new { dt = dt };
            string json = JsonConvert.SerializeObject(lists);
            context.Response.Write(json);
        }
        #endregion

        #region 根据商品编号，商品唯一标识，用户编号，查询积分内容
        protected void GetPointsForByPSU(HttpContext context)
        {
            int ProductId = Convert.ToInt32(context.Request["ProductId"]);//获取商品编号
            int UserId = Globals.GetCurrentMemberUserId();//获取用户编号
            string SkuId = context.Request["SkuId"];//商品编号唯一标识
            DataTable dt = service.GetPointsForByPSU(UserId, ProductId, SkuId);//查询最大积分值
            var lists = new { dt = dt };
            string json = JsonConvert.SerializeObject(lists);
            context.Response.Write(json);
        }
        #endregion

        #region 添加积分兑换内容

        public void SaveContent(HttpContext context)
        {
            int UserId = Globals.GetCurrentMemberUserId();//获取用户编号
            int ProductId = Convert.ToInt32(context.Request["ProductId"]);//获取商品编号
            string IntegralSet = context.Request["PointsFor"];//兑换积分值
            string PointsForConten = context.Request["content"];//积分兑换内容
            string SkuId = context.Request["SkuId"];//商品唯一标识
            bool isSuccess = service.SaveContent(UserId, ProductId, PointsForConten, IntegralSet, SkuId);//添加积分兑换内容
            context.Response.Write(isSuccess);
        }

        #endregion

        #region 查询积分兑换内容

        public void SelPointsForConten(HttpContext context)
        {
            int UserId = Globals.GetCurrentMemberUserId(); ;//获取用户编号
            DataTable dt = service.SelPointsForConten(UserId);
            var list = new { dt = dt };
            string json = JsonConvert.SerializeObject(list);
            context.Response.Write(json);
        }

        #endregion

        #region 删除积分兑换内容

        public void DeletePointsFor(HttpContext context)
        {
            int UserId = Globals.GetCurrentMemberUserId();//获取用户编号
            int ProductId = Convert.ToInt32(context.Request["ProductId"]);//获取商品编号
            string SkuId = context.Request["SkuId"];//商品唯一标识
            bool isSuccess = service.DeletePointsFor(UserId, SkuId);
            context.Response.Write(isSuccess);
        }

        #endregion

        #region 删除立即购买积分兑换金额

        public void DelShoppointdfor(HttpContext context)
        {
            bool DelShoppointdfor = service.DelShoppointdfor(Globals.GetCurrentMemberUserId());
            context.Response.Write(DelShoppointdfor);
        }

        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
