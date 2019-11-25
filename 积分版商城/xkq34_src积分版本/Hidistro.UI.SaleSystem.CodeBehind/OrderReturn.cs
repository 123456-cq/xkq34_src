namespace Hidistro.UI.SaleSystem.CodeBehind
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using IntegralHelp;
    using Hidistro.Core;
    using System.Data;


    [ParseChildren(true)]
    public class OrderReturn : PaymentTemplatedWebControl//支付宝支付回调方法
    {
        IntegralService service = new IntegralService();
        private Literal litMessage;

        public OrderReturn()
            : base(false)
        {
        }

        protected override void AttachChildControls()
        {
            this.litMessage = (Literal)this.FindControl("litMessage");
        }

        protected override void DisplayMessage(string status)
        {
            switch (status)
            {
                case "ordernotfound":
                    this.litMessage.Text = string.Format("没有找到对应的订单信息，订单号：{0}", base.OrderId);
                    break;

                case "gatewaynotfound":
                    this.litMessage.Text = "没有找到与此订单对应的支付方式，系统无法自动完成操作，请联系管理员";
                    break;

                case "verifyfaild":
                    this.litMessage.Text = "支付返回验证失败，操作已停止";
                    break;

                case "success"://订单支付成功

                    #region 积分处理

                    int UserId = Globals.GetCurrentMemberUserId();//获取用户编号

                    int pointsFor = service.SelPointsForMoneyById(UserId);//获取该商品积分兑换值

                    if (!String.IsNullOrEmpty(pointsFor.ToString()))
                    {
                        this.Amount = this.Amount - pointsFor;//商品金额=商品金额-积分兑换金额

                        int Points = service.Points(UserId);//查询现有积分

                        Points = Points - pointsFor;//现有积分=现有积分-积分兑换金额

                        bool UpdatePointByUserId = service.UpdatePointByUserId(Points, UserId);//根据用户编号修改个人积分

                        bool InsertIntegral = service.InsertIntegral(base.OrderId, pointsFor, UserId); //添加积分兑换记录（支出）

                        bool UpdateOrderTotal = service.UpdateOrderTotal(base.OrderId, this.Amount);//修改订单金额

                        bool DeletePointsFor = service.DeletePointsFor(UserId);//删除积分金额记录
                    }
                    int integral = 0;//商品送积分值

                    DataTable dt = service.SelProductInfoByOrderId(base.OrderId);//根据订单号查询商品信息
                    foreach (DataRow item in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(item["ProductId"].ToString()))
                        {
                            integral += service.SelIntegralById(Convert.ToInt32(item["ProductId"]));
                        }
                    }

                    if (integral != 0)
                    {
                        int Points = service.Points(UserId);//查询现有积分

                        Points = Points + integral;//现有积分=现有积分+商品送积分值

                        bool UpdatePointByUserId = service.UpdatePointByUserId(Points, UserId);//根据用户编号修改个人积分

                        bool ShopSendIntegral = service.ShopSendIntegral(integral, UserId);//添加购物送积分记录(收入)
                    }
                    #endregion

                    this.litMessage.Text = string.Format("恭喜您，订单已成功完成支付：{0}</br>支付金额：{1}", base.OrderId, this.Amount.ToString("F"));
                    break;

                case "exceedordermax":
                    this.litMessage.Text = "订单为团购订单，订购数量超过订购总数，支付失败";
                    break;

                case "groupbuyalreadyfinished":
                    this.litMessage.Text = "订单为团购订单，团购活动已结束，支付失败";
                    break;

                case "fail":
                    this.litMessage.Text = string.Format("订单支付已成功，但是系统在处理过程中遇到问题，请联系管理员</br>支付金额：{0}", this.Amount.ToString("F"));
                    break;

                default:
                    this.litMessage.Text = "未知错误，操作已停止";
                    break;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.SkinName == null)
            {
                this.SkinName = "Skin-PaymentReturn.html";
            }
            base.OnInit(e);
        }
    }
}

