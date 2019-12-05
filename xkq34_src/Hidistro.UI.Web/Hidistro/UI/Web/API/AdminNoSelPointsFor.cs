using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using IntegralHelp;

namespace Hidistro.UI.Web.API
{
    public class AdminNoSelPointsFor : System.Web.IHttpHandler
    {
        AdminService service = new AdminService();

        #region 选中方法
        public void ProcessRequest(System.Web.HttpContext context)
        {
            string method = context.Request["Method"];
            switch (method)
            {
                case "NoSelPointsFor":
                    NoSelPointsFor(context);
                    break;
                case "AddSendsIntegral":
                    AddSendsIntegral(context);
                    break;
                case "AddPointsFor":
                    AddPointsFor(context);
                    break;
                case "AddZeroBy":
                    AddZeroBy(context);
                    break;
            }
        } 
        #endregion

        #region 展示未设置购物送积分和未设置积分兑换的商品

        public void NoSelPointsFor(HttpContext context)
        {
            int PageIndex = Convert.ToInt32(context.Request["PageIndex"]);
            int totalCount, totalPage = 0;
            DataTable dt = service.NoSelPointsFor(out totalCount, out totalPage, 5, PageIndex);
            var list = new { dt = dt, totalCount = totalCount, totalPage = totalPage };
            string json = JsonConvert.SerializeObject(list);
            context.Response.Write(json);
        }

        #endregion

        #region 添加购物送积分商品

        public void AddSendsIntegral(HttpContext context)
        {
            bool isSucess = service.AddSendsIntegral(Convert.ToInt32(context.Request["integral"]), Convert.ToInt32(context.Request["ProductId"]));
            context.Response.Write(isSucess);
        }

        #endregion

        #region 添加积分兑换商品

        public void AddPointsFor(HttpContext context)
        {
            bool isSucess = service.AddPointsFor(Convert.ToInt32(context.Request["IntegralCeiling"]), Convert.ToInt32(context.Request["ProductId"]));
            context.Response.Write(isSucess);
        }

        #endregion

        #region 添加零元购商品

        public void AddZeroBy(HttpContext context)
        {
            bool isSucess = service.AddZeroBuy(Convert.ToInt32(context.Request["ZeroBy"]), Convert.ToInt32(context.Request["ProductId"]));
            context.Response.Write(isSucess);
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
