<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wx_Submit.aspx.cs" Inherits="Hidistro.UI.Web.Pay.wx_Submit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript">

    var CheckValue="<%=CheckValue%>";

    if(CheckValue!=""){
        alert(CheckValue);
        //如果出错时，弹出提示
        location.href = "/vshop/MemberCenter.aspx?status=1";
    }
    else
    {
			 pay();
                        function onBridgeReady(){    
                            WeixinJSBridge.invoke(    
                                'getBrandWCPayRequest', <%= pay_json %>,    
                                    
                                function(res){         
                                    if(res.err_msg == "get_brand_wcpay_request:ok" ) {    
                                        alert("支付成功");  // 使用以上方式判断前端返回,微信团队郑重提示：res.err_msg将在用户支付成功后返回    ok，但并不保证它绝对可靠。     
                                        //回到用户订单列表  
					var shareid=<%=shareid%>;
					  if(shareid>0)
                			{
                			    location.href = "/vshop/GetRedShare.aspx?shareid="+shareid;
                			}
               				 else
                			{
                  				  location.href = "/vshop/MemberCenter.aspx?status=3";
               				 }
                                       // window.location.href="http://wx.ooklady.com/wechat/order/orderlist";  
                                    }else if (res.err_msg == "get_brand_wcpay_request:cancel")  {  
                                        alert("支付过程中用户取消");  
					 location.href = "/vshop/MemberOrders.aspx?status=1";
                                    }else{  
                                       //支付失败  
                                       alert(JSON.stringify(res));
					 location.href = "/vshop/MemberCenter.aspx?status=1";
                                    }       
                                }    
                            );     
                         }   
                        //唤起微信支付  
                        function pay(){    
                            if (typeof WeixinJSBridge == "undefined"){    
                               if( document.addEventListener ){    
                                   document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);    
                               }else if (document.attachEvent){    
                                   document.attachEvent('WeixinJSBridgeReady', onBridgeReady);     
                                   document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);    
                               }    
                            }else{    
                               onBridgeReady();    
                            }     
                                
                        }  
                    



      }
   
</script>
</body>
</html>
