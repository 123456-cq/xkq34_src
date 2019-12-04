 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hidistro.Entities.Commodities;
using System.Runtime.InteropServices;
using System.Data;
using Hidistro.Entities;

namespace IntegralHelp
{
    /// <summary>
    /// 首页分类数据类
    /// </summary>
    public class HomePageClassService
    {
        SqlHelper help = new SqlHelper();

        #region 购物送积分商品

        public IList<ProductInfo> ShopSendIntegral(IList<int> productIds, [Optional, DefaultParameterValue(false)] bool Resort, string ShopNumber)
        {
            IList<ProductInfo> list = new List<ProductInfo>();
            try
            {
                string str = "(";
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                for (int i = 0; i < productIds.Count; i++)
                {
                    str = str + productIds[i] + ",";
                    if (!dictionary.ContainsKey(productIds[i]))
                    {
                        dictionary.Add(productIds[i], i);
                    }
                }
                if (str.Length > 1)
                {
                    string sql = "";
                    if (ShopNumber == "four")//展示四个商品
                    {
                        sql = "SELECT top (4) * FROM (SELECT P.*,\r\n                (SELECT MIN(SalePrice) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS SalePrice,\r\n                (SELECT TOP 1 SkuId FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS SkuId,\r\n                (SELECT SUM(Stock) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS Stock,\r\n                (SELECT TOP 1 [Weight] FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS [Weight],\r\n                (SELECT COUNT(*) FROM Taobao_Products WHERE ProductId = p.ProductId) AS IsMakeTaobao\r\n                FROM Hishop_Products p)  as A\r\n         left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]\r\n         WHERE A.ProductId IN " + (str.Substring(0, str.Length - 1) + ")") + " AND A.Stock > 0 AND A.SaleStatus=1 and [integral]!=0";
                    }
                    else//展示所有商品
                    {
                        sql = "SELECT * FROM (SELECT P.*,\r\n                (SELECT MIN(SalePrice) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS SalePrice,\r\n                (SELECT TOP 1 SkuId FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS SkuId,\r\n                (SELECT SUM(Stock) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS Stock,\r\n                (SELECT TOP 1 [Weight] FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS [Weight],\r\n                (SELECT COUNT(*) FROM Taobao_Products WHERE ProductId = p.ProductId) AS IsMakeTaobao\r\n                FROM Hishop_Products p)  as A\r\n       left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]\r\n           WHERE A.ProductId IN " + (str.Substring(0, str.Length - 1) + ")") + " AND A.Stock > 0 AND A.SaleStatus=1 and [integral]!=0";
                    }

                    DataTable table = null;
                    table = help.GetTable(sql);
                    if ((table == null) || (table.Rows.Count <= 0))
                    {
                        return list;
                    }
                    if (Resort)
                    {
                        table.Columns.Add("Sort", typeof(int));
                        foreach (DataRow row in table.Rows)
                        {
                            if (dictionary.ContainsKey((int)row["ProductId"]))
                            {
                                row["Sort"] = dictionary[(int)row["ProductId"]];
                            }
                            else
                            {
                                row["Sort"] = 0x1869f;
                            }
                        }
                        DataView defaultView = table.DefaultView;
                        defaultView.Sort = "Sort Asc";
                        table = defaultView.ToTable();
                    }
                    foreach (DataRow row in table.Rows)
                    {
                        list.Add(DataMapper.PopulateProduct(row));
                    }
                }
            }
            catch (Exception)
            {
                return list;
                throw;
            }
            return list;
        }

        #endregion

        #region 积分兑换商品

        public IList<ProductInfo> PointsForGoods(IList<int> productIds, [Optional, DefaultParameterValue(false)] bool Resort, string ShopNumber)
        {
            IList<ProductInfo> list = new List<ProductInfo>();
            try
            {
                string str = "(";
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                for (int i = 0; i < productIds.Count; i++)
                {
                    str = str + productIds[i] + ",";
                    if (!dictionary.ContainsKey(productIds[i]))
                    {
                        dictionary.Add(productIds[i], i);
                    }
                }
                if (str.Length > 1)
                {
                    string sql = "";
                    if (ShopNumber == "four")//展示四个商品
                    {
                        sql = "SELECT top (4) * FROM (SELECT P.*,\r\n                (SELECT MIN(SalePrice) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS SalePrice,\r\n                (SELECT TOP 1 SkuId FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS SkuId,\r\n                (SELECT SUM(Stock) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS Stock,\r\n                (SELECT TOP 1 [Weight] FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS [Weight],\r\n                (SELECT COUNT(*) FROM Taobao_Products WHERE ProductId = p.ProductId) AS IsMakeTaobao\r\n                FROM Hishop_Products p)  as A\r\n        left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]\r\n                 WHERE A.ProductId IN " + (str.Substring(0, str.Length - 1) + ")") + " AND A.Stock > 0 AND A.SaleStatus=1  and [IntegralToNow]=1";
                    }
                    else//展示所有商品
                    {
                        sql = "SELECT * FROM (SELECT P.*,\r\n                (SELECT MIN(SalePrice) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS SalePrice,\r\n                (SELECT TOP 1 SkuId FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS SkuId,\r\n                (SELECT SUM(Stock) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS Stock,\r\n                (SELECT TOP 1 [Weight] FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS [Weight],\r\n                (SELECT COUNT(*) FROM Taobao_Products WHERE ProductId = p.ProductId) AS IsMakeTaobao\r\n                FROM Hishop_Products p)  as A\r\n         left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]\r\n                 WHERE A.ProductId IN " + (str.Substring(0, str.Length - 1) + ")") + " AND A.Stock > 0 AND A.SaleStatus=1  and [IntegralToNow]=1";
                    }

                    DataTable table = null;
                    table = help.GetTable(sql);
                    if ((table == null) || (table.Rows.Count <= 0))
                    {
                        return list;
                    }
                    if (Resort)
                    {
                        table.Columns.Add("Sort", typeof(int));
                        foreach (DataRow row in table.Rows)
                        {
                            if (dictionary.ContainsKey((int)row["ProductId"]))
                            {
                                row["Sort"] = dictionary[(int)row["ProductId"]];
                            }
                            else
                            {
                                row["Sort"] = 0x1869f;
                            }
                        }
                        DataView defaultView = table.DefaultView;
                        defaultView.Sort = "Sort Asc";
                        table = defaultView.ToTable();
                    }
                    foreach (DataRow row in table.Rows)
                    {
                        list.Add(DataMapper.PopulateProduct(row));
                    }
                }
            }
            catch (Exception)
            {
                return list;
                throw;
            }
            return list;
        }

        #endregion

        #region 零元购商品


        public IList<ProductInfo> ZeroBuyGoods(IList<int> productIds, [Optional, DefaultParameterValue(false)] bool Resort, string ShopNumber)
        {
            IList<ProductInfo> list = new List<ProductInfo>();
            try
            {
                string str = "(";
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                for (int i = 0; i < productIds.Count; i++)
                {
                    str = str + productIds[i] + ",";
                    if (!dictionary.ContainsKey(productIds[i]))
                    {
                        dictionary.Add(productIds[i], i);
                    }
                }
                if (str.Length > 1)
                {
                    string sql = "";
                    if (ShopNumber == "four")//展示四个商品
                    {
                        sql = "SELECT top (4) *  FROM (SELECT P.*,\r\n                (SELECT MIN(SalePrice) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS SalePrice,\r\n                (SELECT TOP 1 SkuId FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS SkuId,\r\n                (SELECT SUM(Stock) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS Stock,\r\n                (SELECT TOP 1 [Weight] FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS [Weight],\r\n                (SELECT COUNT(*) FROM Taobao_Products WHERE ProductId = p.ProductId) AS IsMakeTaobao \r\n     FROM Hishop_Products p)  as A\r\n   left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]\r\n                 WHERE A.ProductId IN " + (str.Substring(0, str.Length - 1) + ")") + " AND A.Stock > 0 AND A.SaleStatus=1  and ZeroBuy=1";
                    }
                    else//展示所有商品
                    {
                        sql = "SELECT  *  FROM (SELECT P.*,\r\n                (SELECT MIN(SalePrice) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS SalePrice,\r\n                (SELECT TOP 1 SkuId FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS SkuId,\r\n                (SELECT SUM(Stock) FROM Hishop_SKUs WHERE ProductId = p.ProductId) AS Stock,\r\n                (SELECT TOP 1 [Weight] FROM Hishop_SKUs WHERE ProductId = p.ProductId ORDER BY SalePrice) AS [Weight],\r\n                (SELECT COUNT(*) FROM Taobao_Products WHERE ProductId = p.ProductId) AS IsMakeTaobao \r\n     FROM Hishop_Products p)  as A\r\n   left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]\r\n                 WHERE A.ProductId IN " + (str.Substring(0, str.Length - 1) + ")") + " AND A.Stock > 0 AND A.SaleStatus=1  and ZeroBuy=1";
                    }

                    DataTable table = null;
                    table = help.GetTable(sql);
                    if ((table == null) || (table.Rows.Count <= 0))
                    {
                        return list;
                    }
                    if (Resort)
                    {
                        table.Columns.Add("Sort", typeof(int));
                        foreach (DataRow row in table.Rows)
                        {
                            if (dictionary.ContainsKey((int)row["ProductId"]))
                            {
                                row["Sort"] = dictionary[(int)row["ProductId"]];
                            }
                            else
                            {
                                row["Sort"] = 0x1869f;
                            }
                        }
                        DataView defaultView = table.DefaultView;
                        defaultView.Sort = "Sort Asc";
                        table = defaultView.ToTable();
                    }
                    foreach (DataRow row in table.Rows)
                    {
                        list.Add(DataMapper.PopulateProduct(row));
                    }
                }
            }
            catch (Exception)
            {
                return list;
                throw;
            }
            return list;
        }

        #endregion
    }
}
