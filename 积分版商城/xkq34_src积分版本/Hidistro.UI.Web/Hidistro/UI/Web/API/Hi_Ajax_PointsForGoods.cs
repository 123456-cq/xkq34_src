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
    /// <summary>
    /// 首页积分兑换数据处理类
    /// </summary>
    public class Hi_Ajax_PointsForGoods : System.Web.IHttpHandler
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
            string content = this.GoodGroupJson(context);
            context.Response.Write(content);
        }

        public string GoodGroupJson(System.Web.HttpContext context)
        {
            Hi_Json_GoodGourpContent hi_Json_GoodGourpContent = new Hi_Json_GoodGourpContent();
            try
            {
                string number = context.Request["ShopNumber"];//获取商品显示的数量

                string text = (context.Request["IDs"] != null) ? context.Request["IDs"] : "";

                System.Collections.Generic.List<HiShop_Model_Good> list = new System.Collections.Generic.List<HiShop_Model_Good>();

                if (!string.IsNullOrEmpty(text))
                {
                    System.Collections.Generic.IList<ProductInfo> goods = this.GetGoods(context, text, number);//获取积分兑换商品
                    foreach (ProductInfo current in goods)
                    {
                        if (String.IsNullOrEmpty(current.IntegralCeiling))
                        {
                            current.IntegralCeiling = "0";
                        }
                        list.Add(new HiShop_Model_Good
                        {
                            item_id = current.ProductId.ToString(),
                            title = current.ProductName.ToString(),
                            price = System.Convert.ToDouble(current.MinShowPrice).ToString("f2"),
                            original_price = System.Convert.ToDouble(current.MarketPrice).ToString("f2"),
                            link = Globals.GetWebUrlStart() + "/ProductDetails.aspx?productId=" + current.ProductId.ToString(),
                            pic = current.ThumbnailUrl310.ToString(),
                            IntegralCeiling = current.IntegralCeiling.ToString()
                        });
                    }
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

        public System.Collections.Generic.IList<ProductInfo> GetGoods(System.Web.HttpContext context, string ids, string number)
        {
            System.Collections.Generic.List<int> productIds = (from s in ids.Split(new char[]
			{
				','
			})
                                                               select int.Parse(s)).ToList<int>();
            return service.PointsForGoods(productIds, true, number);//获取积分兑换商品
        }
	}
}
