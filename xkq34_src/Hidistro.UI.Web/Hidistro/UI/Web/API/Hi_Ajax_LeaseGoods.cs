using Hidistro.ControlPanel.Commodities;
using Hidistro.Core;
using Hidistro.Entities.Commodities;
using HiTemplate.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntegralHelp;


namespace Hidistro.UI.Web.API
{
    public class Hi_Ajax_LeaseGoods : System.Web.IHttpHandler
    {
        HomePageClassService service = new HomePageClassService();//首页分类数据类
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(this.GoodGroupJson(context));
        }


        public string GoodGroupJson(System.Web.HttpContext context)
        {
            Hi_Json_GoodGourpContent hi_Json_GoodGourpContent = new Hi_Json_GoodGourpContent();
            try
            {
                string number = context.Request["ShopNumber"];//商品显示的数量

                System.Collections.Generic.List<HiShop_Model_Good> list = new System.Collections.Generic.List<HiShop_Model_Good>();


                System.Collections.Generic.IList<ProductInfo> goods = this.GetGoods(context, number);//获取零元购商品
                foreach (ProductInfo current in goods)
                {
                    if (String.IsNullOrEmpty(current.FristPrice.ToString()))
                    {
                        current.FristPrice = 0;
                    }

                    list.Add(new HiShop_Model_Good
                    {
                        item_id = current.ProductId.ToString(),
                        title = current.ProductName.ToString(),
                        price = System.Convert.ToDouble(current.MinShowPrice).ToString("f2"),
                        original_price = System.Convert.ToDouble(current.MarketPrice).ToString("f2"),
                        link = Globals.GetWebUrlStart() + "/ProductDetails.aspx?productId=" + current.ProductId.ToString(),
                        pic = current.ThumbnailUrl310.ToString(),
                        FristPrice = current.FristPrice
                    });
                }

                hi_Json_GoodGourpContent.goodslist = list;
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(hi_Json_GoodGourpContent);
                throw;
            }
            return JsonConvert.SerializeObject(hi_Json_GoodGourpContent);
        }

        public System.Collections.Generic.IList<ProductInfo> GetGoods(System.Web.HttpContext context, string number)
        {
            return service.LeaseGoods(true, number);
        }
    }
}
