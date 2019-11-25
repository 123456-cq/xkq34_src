﻿namespace Hidistro.UI.SaleSystem.CodeBehind
{
    using Hidistro.Entities.Sales;
    using Hidistro.SaleSystem.Vshop;
    using Hidistro.UI.Common.Controls;
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using IntegralHelp;
    using System.Data;
    using System.Data.SqlClient;
    using Hidistro.Entities.Members;
    using System.Web;
    using Newtonsoft.Json;

    [ParseChildren(true)]
    public class VShoppingCart : VMemberTemplatedWebControl//购物车
    {
        IntegralService service = new IntegralService();
        MemberInfo currentMember = MemberProcessor.GetCurrentMember();
        //private HtmlAnchor aLink;
        private List<ShoppingCartInfo> cart;
        private List<ShoppingCartInfo> cartPoint;
        private HtmlGenericControl divShowTotal;
        private Literal litTotal;
        private decimal ReductionMoneyALL = 0M;
        private VshopTemplatedRepeater rptCartPointProducts;
        private VshopTemplatedRepeater rptCartProducts;

        protected override void AttachChildControls()
        {
            this.rptCartProducts = (VshopTemplatedRepeater)this.FindControl("rptCartProducts");
            this.rptCartProducts.ItemDataBound += new RepeaterItemEventHandler(this.rptCartProducts_ItemDataBound);
            this.rptCartPointProducts = (VshopTemplatedRepeater)this.FindControl("rptCartPointProducts");
            this.litTotal = (Literal)this.FindControl("litTotal");
            this.divShowTotal = (HtmlGenericControl)this.FindControl("divShowTotal");
            //this.aLink = (HtmlAnchor)this.FindControl("aLink");
            this.Page.Session["stylestatus"] = "0";
            this.cart = ShoppingCartProcessor.GetShoppingCartAviti(0);
            this.cartPoint = ShoppingCartProcessor.GetShoppingCartAviti(1);
            if (this.cart != null)
            {
                this.rptCartProducts.DataSource = this.cart;
                this.rptCartProducts.DataBind();
            }
            else
            {
                Panel panel = (Panel)this.FindControl("products");
                panel.Visible = false;
            }
            if (this.cartPoint != null)
            {
                this.rptCartPointProducts.DataSource = this.cartPoint;
                this.rptCartPointProducts.DataBind();
            }
            else
            {
                Panel panel2 = (Panel)this.FindControl("pointproducts");
                panel2.Visible = false;
            }
            if ((this.cart != null) || (this.cartPoint != null))
            {
                //this.aLink.HRef = "/Vshop/SubmmitOrder.aspx";
            }
            else
            {
                Panel panel3 = (Panel)this.FindControl("divEmpty");
                panel3.Visible = true;
                Panel panel4 = (Panel)this.FindControl("pannelGo");
                panel4.Visible = false;
                HtmlInputHidden hidden = (HtmlInputHidden)this.FindControl("hdIsShow");
                hidden.Value = "1";
            }
            decimal num = 0M;
            if (this.cart != null)
            {
                foreach (ShoppingCartInfo info in this.cart)
                {
                    num += info.GetAmount();
                }
            }
            int num2 = 0;
            if (this.cartPoint != null)
            {
                foreach (ShoppingCartInfo info in this.cartPoint)
                {
                    num2 += info.GetTotalPoint();
                }
            }
            PageTitle.AddSiteNameTitle("购物车");
            string str = string.Empty;
            decimal num3 = num - this.ReductionMoneyALL;

            #region 购物车合计=购物车合计-积分兑换金额

            MemberInfo currentMember = MemberProcessor.GetCurrentMember();
            decimal PointsForMoney = 0;
            int UserId = currentMember.UserId;//获取用户编号
            DataTable dt = service.SelIntegralSetInShopCar(UserId);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!String.IsNullOrEmpty(dt.Rows[i]["IntegralSet"].ToString()))
                {
                    PointsForMoney += Convert.ToDecimal(dt.Rows[i]["IntegralSet"]);
                }
            }
            num3 = num3 - PointsForMoney;

            #endregion

            if (num3 > 0M)
            {
                str = "￥" + num3.ToString("F2");
            }
            if (num2 > 0)
            {
                str = str + "+" + num2.ToString() + "积分";
            }
            this.litTotal.Text = str;
        }



        protected override void OnInit(EventArgs e)
        {
            if (this.SkinName == null)
            {
                this.SkinName = "Skin-VShoppingCart.html";
            }
            base.OnInit(e);
        }

        private void rptCartProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
            }
        }
    }
}

