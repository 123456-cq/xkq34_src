﻿$(document).ready(function () {
    $.each($(".SKUValueClass"), function () {
        $(this).bind("click", function () { SelectSkus(this); });
    });

    $("#buyButton").bind("click", function () { BuyProduct(); }); //立即购买
    $("#spAdd").bind("click", function () { $("#buyNum").val(parseInt($("#buyNum").val()) + 1) });
    $("#spSub").bind("click", function () { var num = parseInt($("#buyNum").val()) - 1; if (num > 0) $("#buyNum").val(parseInt($("#buyNum").val()) - 1) });
    $("#aSubmmitorder").bind("click", function () { submmitorder() }); //确认订单页面点击提交订单
    $("#spcloces").bind("click", function () { $("#divshow").hide() });
});

function disableShoppingBtn(disabled) {//禁用(启用)购买和加入购物车按钮

    var btns = $('button[type=shoppingBtn]');
    if (disabled)
        btns.addClass('disabled');
    else
        btns.removeClass('disabled');
}

function SelectSkus(clt) {
    //禁用购买和加入购物车按钮
    disableShoppingBtn(true);

    // 保存当前选择的规格
    var AttributeId = $(clt).attr("AttributeId");
    var ValueId = $(clt).attr("ValueId");
    $("#skuContent_" + AttributeId).val(AttributeId + ":" + ValueId);
    // 重置样式
    ResetSkuRowClass("skuRow_" + AttributeId, "skuValueId_" + AttributeId + "_" + ValueId);
    // 如果全选，则重置SKU
    var allSelected = IsallSelected();
    var selectedOptions = "";
    if (allSelected) {
        $.each($("input[type='hidden'][name='skuCountname']"), function () {
            selectedOptions += $(this).attr("value") + ",";
        });
        selectedOptions = selectedOptions.substring(0, selectedOptions.length - 1);
        $.ajax({
            url: "/API/VshopProcess.ashx",
            type: 'post', dataType: 'json', timeout: 10000,
            data: { action: "GetSkuByOptions", productId: $("#hiddenProductId").val(), options: selectedOptions },
            success: function (resultData) {
                if (resultData.Status == "OK") {
                    ResetCurrentSku(resultData.SkuId, resultData.SKU, resultData.Weight, resultData.Stock, resultData.SalePrice);
                }
                else {
                    ResetCurrentSku("", "", "", "", "0"); //带服务端返回的结果，函数里可以根据这个结果来显示不同的信息
                }
                disableShoppingBtn(false); //启用购买和加入购物车按钮
            },
            error: function () {
                disableShoppingBtn(false); //启用购买和加入购物车按钮
            }
        });
    }
}

// 是否所有规格都已选
function IsallSelected() {
    var allSelected = true;
    $.each($("input[type='hidden'][name='skuCountname']"), function () {
        if ($(this).val().length == 0) {
            allSelected = false;
        }
    });
    return allSelected;
}

// 重置规格值的样式
function ResetSkuRowClass(skuRowId, skuSelectId) {
    var pvid = skuSelectId.split("_");

    $.each($("#" + skuRowId + " div"), function () {
        $(this).removeClass('active');
    });

    $("#" + skuSelectId).addClass('active');
}

// 重置SKU

function ResetCurrentSku(skuId, sku, weight, stock, salePrice) {
    $("#hiddenSkuId").val(skuId);
    $(".spSalaPrice").html(salePrice);
    if (!isNaN(parseInt(stock))) {
        $("#spStock").html(stock);
    }
    else {
        $("#spStock").html("0");
        alert_h("该规格的产品没有库存，请选择其它的规格！");
    }
}

// 立即购买按钮单击事件
function BuyProduct() {
    if (!ValidateBuyAmount()) {
        return false;
    }
    if (!IsallSelected()) {
        debugger;
        alert_h("请先选择您要购买的商品规格!");
        return false;
    }

    var quantity = parseInt($("#buyNum").val());
    var stock = parseInt($("#spStock").html());
    var limitedTimeDiscountId = parseInt($("#hdLimitedTimeDiscountId").val());
    if (isNaN(stock) || stock == 0) {
        alert_h("该规格的产品没有库存，请选择其它的规格！");
        return false;
    }
    if (quantity > stock) {
        alert_h("商品库存不足 " + quantity + " 件，请修改购买数量!");
        return false;
    }
    //location.href = "/Vshop/UserLogin.aspx?userstatus=1&buyAmount=" + $("#buyNum").val() + "&productSku=" + $("#hiddenSkuId").val() + "&from=signBuy";
    location.href = "/Vshop/SubmmitOrder.aspx?buyAmount=" + $("#buyNum").val() + "&productSku=" + $("#hiddenSkuId").val() + "&limitedTimeDiscountId=" + limitedTimeDiscountId + "&from=signBuy&ReferralId=" + getParam("ReferralId") + "&ShopType=1";
}

// 验证数量输入
function ValidateBuyAmount() {
    var buyNum = $("#buyNum");
    if ($(buyNum).val().length == 0) {
        alert_h("请先填写购买数量!");
        return false;
    }
    if ($(buyNum).val() == "0" || $(buyNum).val().length > 3) {

        alert_h("填写的购买数量必须大于0小于999!");
        var str = $(buyNum).val();
        $(buyNum).val(str.substring(0, 5));
        return;
    }
    var amountReg = /^[1-9]d*|0$/;
    if (!amountReg.test($(buyNum).val())) {
        alert_h("请填写正确的购买数量!");
        return false;
    }

    return true;
}

function AddProductToCart() {
    if (!ValidateBuyAmount()) {
        return false;
    }

    if (!IsallSelected()) {
        alert_h("请先选择您要购买的商品规格!");
        return false;
    }
    var quantity = parseInt($("#buyNum").val());
    var stock = parseInt($("#spStock").html());
    if (isNaN(stock) || stock == 0) {
        alert_h("该规格的产品没有库存，请选择其它的规格！");
        return false;
    }
    if (quantity > stock) {
        alert_h("商品库存不足 " + quantity + " 件，请修改购买数量!");
        return false;
    }

    BuyProductToCart(); //添加到购物车
}

function submmitorder() {//确认订单页面点击提交订单
    var ispass = true;
    $(".MemberPointNumber").each(function () {
        if ($(this).html() < 0) {
            ispass = false;
        }
    })
    if (!ispass) {
        alert_h("您的积分不足，请重新选择商品购买数量！", function () { location.href = "shoppingCart.aspx"; });
        return false;
    }
    var shippingType = "";
    var b = false;
    $(".selectShippingTypeValue").each(function () {
        if ($(this).val() == "") {
            b = true;
        }
        else {
            shippingType += $(this).val() + ",";
        }
    });
    if (b) {
        alert_h("请选择配送方式");
        return false;
    }
    if (parseFloat($("#total").html()) < 0) {
        alert_h("实付总额为负数，不能支付！");
        return false;
    }
    var remark = "";
    $("textarea").each(function () {
        remark += $(this).val() + ",";
    });
    remark = remark.substr(0, remark.length - 1);
    var selectCouponValue = "";
    $(".selectCouponValue").each(function () {
        selectCouponValue += $(this).val() + ",";
    });

    selectCouponValue = selectCouponValue.substr(0, selectCouponValue.length - 1);

    shippingType = shippingType.substr(0, shippingType.length - 1);
    var PointNumber = "";
    $(".txtPointNumber").each(function () {
        PointNumber += $(this).val() + ",";
    });
    PointNumber = PointNumber.substr(0, PointNumber.length - 1);

    if (!$("#selectPaymentType").val()) {
        alert_h("请选择支付方式");
        return false;
    }

    if (!$('#selectShipToDate').val()) {
        alert_h("请选择送货上门时间");
        return false;
    }

    var bargainDetialID = parseInt(getParam("bargainDetialId"));
    var limitedTimeDiscountId = parseInt(getParam("limitedTimeDiscountId"));
    maskayer(0);

    $.ajax({//提交订单方法
        url: "/API/VshopProcess.ashx",
        type: 'post', dataType: 'json', timeout: 10000,
        data: {
            action: "Submmitorder", PointNumber: PointNumber, selectCouponValue: selectCouponValue, Shippingcity: $("#Shippingcity").val(), shippingType: shippingType, paymentType: $("#selectPaymentType").val(), couponCode: $("#selectCoupon").val(), redpagerid: $("#selectRedPager").val(), shippingId: $('#selectShipTo').val(),
            productSku: getParam("productSku"), buyAmount: getParam("buyAmount"), from: getParam("from"), shiptoDate: $("#selectShipToDate").val(), groupbuyId: $('#groupbuyHiddenBox').val(), remark: remark, bargainDetialId: bargainDetialID, limitedTimeDiscountId: limitedTimeDiscountId
        },
        success: function (resultData) {
            maskayer(1);
            if (resultData.Status == "OK") {
                location.href = "/Vshop/FinishOrder.aspx?orderId=" + resultData.OrderId;
            }
            else if (resultData.ErrorMsg) {
                alert_h(resultData.ErrorMsg);
            }
        }
    });
}

function isWeiXin() {
    var ua = window.navigator.userAgent.toLowerCase();
    if (ua.match(/MicroMessenger/i) == 'micromessenger') {
        return true;
    } else {
        return false;
    }
}
function BuyProductToCart() {
    $.ajax({
        url: "/API/VshopProcess.ashx",
        type: 'post', dataType: 'json', timeout: 10000,
        data: { action: "AddToCartBySkus", limitedTimeDiscountId: 0, quantity: parseInt($("#buyNum").val()), productSkuId: $("#hiddenSkuId").val(), Templateid: $("#vProductDetails_litTemplate").val(), categoryid: $("#vProductDetails_litCategoryId").val() },
        async: false,
        success: function (resultData) {

            if (resultData.Status == "OK") {
                var xtarget = $("#addcartButton").offset().left;
                var ytarget = $("#addcartButton").offset().top;

                $("#divshow").css("top", "200px");
                $("#divshow").css("left", parseInt(xtarget) + "px");
                myConfirm1('添加成功', '商品已经添加至购物车', '继续逛逛', '现在去购物车', function (data) {
                    if (data) {
                        location.replace('/Vshop/ShoppingCart.aspx');
                    } else {
                        location.replace('/ProductList.aspx');
                    }

                });
                //显示添加购物成功
            } else if (resultData.Status == "0") {
                // 商品已经下架

                alert_h("此商品已经不存在(可能被删除或被下架)，暂时不能购买");
            }
            else if (resultData.Status == "1") {
                // 商品库存不足

                alert_h("商品库存不足 " + parseInt($("#buyNum").val()) + " 件，请修改购买数量!");
            }
            else {
                if (resultData.Status == "2") {
                    var url = window.location.href;
                    //if (!isWeiXin())
                    //location.href = "/UserLogin.aspx?returnUrl=" +encodeURIComponent(url) + "";
                    //else
                    location.href = "/UserLogining.aspx?returnUrl=" + encodeURIComponent(url) + "";
                }
                else {
                    // 抛出异常消息

                    alert_h(resultData.Status);
                }
            }
        }
    });
}

