using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hidistro.Entities.Commodities;
using Hidistro.SaleSystem.Vshop;
using Newtonsoft.Json;


namespace Hidistro.UI.Web.API
{
    public class Hi_Ajax_GoodsDetails : System.Web.IHttpHandler
    {
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
            context.Response.Write(this.GetGoodsDetails(context));
        }

        public string GetGoodsDetails(System.Web.HttpContext context)
        {

            try
            {
                int productId = Convert.ToInt32(context.Request["productId"]);

                if (productId == 0 || productId == null)
                {
                    return "商品编号不存在";
                }

                ProductInfo product = ProductBrowser.GetProduct(MemberProcessor.GetCurrentMember(), productId);//获取商品详情

                if (product == null)
                {
                    return "商品不存在";
                }

                if (product.SaleStatus != ProductSaleStatus.OnSale)
                {
                    return "此商品已下架";
                }

                return JsonConvert.SerializeObject(product);

            }
            catch (Exception)
            {
                return "Error";
                throw;
            }

        }

    }
}
