﻿//namespace Hidistro.UI.SaleSystem.CodeBehind
//{
//    using Hidistro.Core;
//    using Hidistro.Core.Entities;
//    using Hidistro.Entities.Sales;
//    using Hidistro.Entities.VShop;
//    using Hidistro.SaleSystem.Vshop;
//    using Hidistro.UI.Common.Controls;
//    using Newtonsoft.Json;
//    using Newtonsoft.Json.Converters;
//    using System;
//    using System.Collections;
//    using System.Collections.Generic;
//    using System.Data;
//    using System.Linq;
//    using System.Web;
//    using System.Web.UI.HtmlControls;
//    using System.Web.UI.WebControls;

//    public class VMyOneTaoSuccess : VMemberTemplatedWebControl
//    {
//        private Literal litAddAddress;
//        private Literal litAddress;
//        private Literal litCellPhone;
//        private Literal litShipTo;
//        private Literal litShowMes;
//        private HtmlInputHidden regionId;
//        private VshopTemplatedRepeater rptAddress;
//        private HtmlInputHidden selectShipTo;

//        protected override void AttachChildControls()
//        {
//            this.litShipTo = (Literal)this.FindControl("litShipTo");
//            this.litCellPhone = (Literal)this.FindControl("litCellPhone");
//            this.litAddress = (Literal)this.FindControl("litAddress");
//            this.rptAddress = (VshopTemplatedRepeater)this.FindControl("rptAddress");
//            IList<ShippingAddressInfo> shippingAddresses = MemberProcessor.GetShippingAddresses();
//            this.rptAddress.DataSource = from item in shippingAddresses
//                                         orderby item.IsDefault
//                                         select item;
//            this.rptAddress.DataBind();
//            this.litAddAddress = (Literal)this.FindControl("litAddAddress");
//            this.selectShipTo = (HtmlInputHidden)this.FindControl("selectShipTo");
//            this.regionId = (HtmlInputHidden)this.FindControl("regionId");
//            //if (CS$<>9__CachedAnonymousMethodDelegate9 == null)
//            //{
//            //    CS$<>9__CachedAnonymousMethodDelegate9 = new Func<ShippingAddressInfo, bool>(null, (IntPtr) <AttachChildControls>b__7);
//            //}
//            //ShippingAddressInfo info = Enumerable.FirstOrDefault<ShippingAddressInfo>(shippingAddresses, CS$<>9__CachedAnonymousMethodDelegate9);
//            ShippingAddressInfo info = shippingAddresses.FirstOrDefault<ShippingAddressInfo>(item => item.IsDefault);

//            if (info == null)
//            {
//                info = (shippingAddresses.Count > 0) ? shippingAddresses[0] : null;
//            }
//            if (info != null)
//            {
//                this.litShipTo.Text = info.ShipTo;
//                this.litCellPhone.Text = info.CellPhone;
//                this.litAddress.Text = info.Address;
//                this.selectShipTo.SetWhenIsNotNull(info.ShippingId.ToString());
//                this.regionId.SetWhenIsNotNull(info.RegionId.ToString());
//            }
//            this.litAddAddress.Text = " href='/Vshop/AddShippingAddress.aspx?returnUrl=" + Globals.UrlEncode(HttpContext.Current.Request.Url.ToString()) + "'";
//            PageTitle.AddSiteNameTitle("中奖记录");
//        }

//        private void DoAction(string Action)
//        {
//            string s = "{\"state\":false,\"msg\":\"未定义操作\"}";
//            int num = Globals.RequestFormNum("pageIndex");
//            if (num > 0)
//            {
//                int userid = Globals.GetCurrentMemberUserId();
//                OneyuanTaoPartInQuery query = new OneyuanTaoPartInQuery {
//                    PageIndex = num,
//                    PageSize = 10,
//                    ActivityId = "",
//                    UserId = Globals.GetCurrentMemberUserId(),
//                    state = 3,
//                    SortBy = "BuyTime",
//                    IsPay = -1
//                };
//                DbQueryResult oneyuanPartInDataTable = OneyuanTaoHelp.GetOneyuanPartInDataTable(query);
//                DataTable data = new DataTable();
//                if (oneyuanPartInDataTable.Data != null)
//                {
//                    data = (DataTable) oneyuanPartInDataTable.Data;
//                    data.Columns.Add("LuckNumList");
//                    data.Columns.Add("PostSate");
//                    data.Columns.Add("PostBtn");
//                    data.Columns.Add("tabid");
//                    using (IEnumerator enumerator = data.Rows.GetEnumerator())
//                    {
//                        while (enumerator.MoveNext())
//                        {
//                            Func<LuckInfo, bool> func = null;
//                            DataRow Item = (DataRow) enumerator.Current;
//                            if (func == null)
//                            {
//                                <>c__DisplayClass4 class2;
//                                func = new Func<LuckInfo, bool>(class2, (IntPtr) this.<DoAction>b__0);
//                            }
//                            IList<LuckInfo> list = Enumerable.Where<LuckInfo>(OneyuanTaoHelp.getLuckInfoList(true, Item["ActivityId"].ToString()), func).ToList<LuckInfo>();
//                            Item["PostBtn"] = "0";
//                            Item["tabid"] = "0";
//                            if (list != null)
//                            {
//                                List<string> list2 = new List<string>();
//                                foreach (LuckInfo info in list)
//                                {
//                                    list2.Add(info.PrizeNum);
//                                }
//                                Item["LuckNumList"] = string.Join(",", list2);
//                                DataTable table2 = OneyuanTaoHelp.PrizesDeliveryRecord(Item["Pid"].ToString());
//                                if ((table2 == null) || (table2.Rows.Count == 0))
//                                {
//                                    Item["PostSate"] = "收货地址未确认";
//                                }
//                                else
//                                {
//                                    Item["PostSate"] = OneyuanTaoHelp.GetPrizesDeliveStatus(table2.Rows[0]["status"].ToString());
//                                    Item["PostBtn"] = table2.Rows[0]["status"].ToString();
//                                    Item["tabid"] = table2.Rows[0]["Id"].ToString();
//                                }
//                            }
//                        }
//                    }
//                }
//                IsoDateTimeConverter converter = new IsoDateTimeConverter {
//                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
//                };
//                string str2 = JsonConvert.SerializeObject(data, new JsonConverter[] { converter });
//                s = "{\"state\":true,\"msg\":\"读取成功\",\"Data\":" + str2 + "}";
//            }
//            else
//            {
//                s = "{\"state\":false,\"msg\":\"参数不正确\"}";
//            }
//            this.Page.Response.ClearContent();
//            this.Page.Response.ContentType = "application/json";
//            this.Page.Response.Write(s);
//            this.Page.Response.End();
//        }

//        protected override void OnInit(EventArgs e)
//        {
//            string str = Globals.RequestFormStr("action");
//            if (!string.IsNullOrEmpty(str))
//            {
//                this.DoAction(str);
//                this.Page.Response.End();
//            }
//            if (this.SkinName == null)
//            {
//                this.SkinName = "Skin-VMyOneTaoSuccess.html";
//            }
//            base.OnInit(e);
//        }
//    }
//}

namespace Hidistro.UI.SaleSystem.CodeBehind
{
    using Hidistro.Core;
    using Hidistro.Core.Entities;
    using Hidistro.Entities.Sales;
    using Hidistro.Entities.VShop;
    using Hidistro.SaleSystem.Vshop;
    using Hidistro.UI.Common.Controls;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class VMyOneTaoSuccess : VMemberTemplatedWebControl
    {
        private Literal litAddAddress;
        private Literal litAddress;
        private Literal litCellPhone;
        private Literal litShipTo;
        private Literal litShowMes;
        private HtmlInputHidden regionId;
        private VshopTemplatedRepeater rptAddress;
        private HtmlInputHidden selectShipTo;

        protected override void AttachChildControls()
        {
            this.litShipTo = (Literal)this.FindControl("litShipTo");
            this.litCellPhone = (Literal)this.FindControl("litCellPhone");
            this.litAddress = (Literal)this.FindControl("litAddress");
            this.rptAddress = (VshopTemplatedRepeater)this.FindControl("rptAddress");
            IList<ShippingAddressInfo> shippingAddresses = MemberProcessor.GetShippingAddresses();
            this.rptAddress.DataSource = from item in shippingAddresses
                                         orderby item.IsDefault
                                         select item;
            this.rptAddress.DataBind();
            this.litAddAddress = (Literal)this.FindControl("litAddAddress");
            this.selectShipTo = (HtmlInputHidden)this.FindControl("selectShipTo");
            this.regionId = (HtmlInputHidden)this.FindControl("regionId");
            ShippingAddressInfo info = shippingAddresses.FirstOrDefault<ShippingAddressInfo>(item => item.IsDefault);
            if (info == null)
            {
                info = (shippingAddresses.Count > 0) ? shippingAddresses[0] : null;
            }
            if (info != null)
            {
                this.litShipTo.Text = info.ShipTo;
                this.litCellPhone.Text = info.CellPhone;
                this.litAddress.Text = info.Address;
                this.selectShipTo.SetWhenIsNotNull(info.ShippingId.ToString());
                this.regionId.SetWhenIsNotNull(info.RegionId.ToString());
            }
            this.litAddAddress.Text = " href='/Vshop/AddShippingAddress.aspx?returnUrl=" + Globals.UrlEncode(HttpContext.Current.Request.Url.ToString()) + "'";
            PageTitle.AddSiteNameTitle("中奖记录");
        }

        private void DoAction(string Action)
        {
            string s = "{\"state\":false,\"msg\":\"未定义操作\"}";
            int num = Globals.RequestFormNum("pageIndex");
            if (num > 0)
            {
                int userid = Globals.GetCurrentMemberUserId();
                OneyuanTaoPartInQuery query = new OneyuanTaoPartInQuery
                {
                    PageIndex = num,
                    PageSize = 10,
                    ActivityId = "",
                    UserId = Globals.GetCurrentMemberUserId(),
                    state = 3,
                    SortBy = "BuyTime",
                    IsPay = -1
                };
                DbQueryResult oneyuanPartInDataTable = OneyuanTaoHelp.GetOneyuanPartInDataTable(query);
                DataTable data = new DataTable();
                if (oneyuanPartInDataTable.Data != null)
                {
                    data = (DataTable)oneyuanPartInDataTable.Data;
                    data.Columns.Add("LuckNumList");
                    data.Columns.Add("PostSate");
                    data.Columns.Add("PostBtn");
                    data.Columns.Add("tabid");
                    IEnumerator enumerator = data.Rows.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        Func<LuckInfo, bool> predicate = null;
                        DataRow Item = (DataRow)enumerator.Current;
                        if (predicate == null)
                        {
                            predicate = t => (t.UserId == userid) && (t.Pid == Item["Pid"].ToString());
                        }
                        IList<LuckInfo> list = OneyuanTaoHelp.getLuckInfoList(true, Item["ActivityId"].ToString()).Where<LuckInfo>(predicate).ToList<LuckInfo>();
                        Item["PostBtn"] = "0";
                        Item["tabid"] = "0";
                        if (list != null)
                        {
                            List<string> values = new List<string>();
                            foreach (LuckInfo info in list)
                            {
                                values.Add(info.PrizeNum);
                            }
                            Item["LuckNumList"] = string.Join(",", values);
                            DataTable table2 = OneyuanTaoHelp.PrizesDeliveryRecord(Item["Pid"].ToString());
                            if ((table2 == null) || (table2.Rows.Count == 0))
                            {
                                Item["PostSate"] = "收货地址未确认";
                            }
                            else
                            {
                                Item["PostSate"] = OneyuanTaoHelp.GetPrizesDeliveStatus(table2.Rows[0]["status"].ToString());
                                Item["PostBtn"] = table2.Rows[0]["status"].ToString();
                                Item["tabid"] = table2.Rows[0]["Id"].ToString();
                            }
                        }

                    }
                }
                IsoDateTimeConverter converter = new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                };
                string str2 = JsonConvert.SerializeObject(data, new JsonConverter[] { converter });
                s = "{\"state\":true,\"msg\":\"读取成功\",\"Data\":" + str2 + "}";
            }
            else
            {
                s = "{\"state\":false,\"msg\":\"参数不正确\"}";
            }
            this.Page.Response.ClearContent();
            this.Page.Response.ContentType = "application/json";
            this.Page.Response.Write(s);
            this.Page.Response.End();
        }

        protected override void OnInit(EventArgs e)
        {
            string str = Globals.RequestFormStr("action");
            if (!string.IsNullOrEmpty(str))
            {
                this.DoAction(str);
                this.Page.Response.End();
            }
            if (this.SkinName == null)
            {
                this.SkinName = "Skin-VMyOneTaoSuccess.html";
            }
            base.OnInit(e);
        }
    }
}



