$(document).ready(function () {
    $("input[name='inputQuantity']").bind("blur", function () { chageQuantity(this); }); //立即购买
    $("[name='iDelete']").bind("click", function () {
        ProductId = $(this).prev().text();
        var obj = this;
        myConfirm('询问', '确定要从购物车里删除该商品吗？', '确认删除', function () {
            deleteCartProduct(obj);
        });
    }); //立即购买
    $("#selectShippingType").bind("change", function () { chageShippingType() });
    $("#selectCoupon").bind("change", function () { chageCoupon() });
   
});

function deleteCartProduct(obj) {
    var type = $(obj).attr("stype");
    if (type == null || type == undefined) {
        type = 0;
    }
    limitedTimeDiscountId = $(obj).attr("limitedTimeDiscountId");
    $.ajax({
        url: "/API/VshopProcess.ashx",
        type: 'post', dataType: 'json', timeout: 10000,
        data: { action: "DeleteCartProduct", skuId: $(obj).attr("skuId"), type: type, limitedTimeDiscountId: limitedTimeDiscountId },
        success: function (resultData) {
            if (resultData.Status == "OK") {
                $.post("/API/ClickIntegralLoad.ashx", { Method: "ClosePointsFor", SkuId: $(obj).attr("skuId") }, function (isSuccess) {
                })
                location.href = "/Vshop/ShoppingCart.aspx";
            }
        }
    });
}

function chageQuantity(obj) {
    $.ajax({
        url: "/API/VshopProcess.ashx",
        type: 'post', dataType: 'json', timeout: 10000,
        data: { action: "ChageQuantity", skuId: $(obj).attr("skuId"), quantity: parseInt($(obj).val()) },
        success: function (resultData) {
            if (resultData.Status == "OK") {
                location.href = "/Vshop/ShoppingCart.aspx";
            }
            else {
                alert_h("最多可购买" + resultData.Status + "件", function () {
                    location.href = "/Vshop/ShoppingCart.aspx";
                });

            }
        }
    });
}


function chageShippingType() {
    var freight = 0;
    if ($("#selectShippingType").val() != "-1") {
        var selectedShippingType = $("#selectShippingType option:selected").text();
        freight = parseFloat(selectedShippingType.substring(selectedShippingType.lastIndexOf("￥") + 1));
    }

    var discountValue = 0;
    if ($("#selectCoupon").val() != undefined && $("#selectCoupon").val() != "") {
        var selectCoupon = $("#selectCoupon option:selected").text();
        discountValue = parseFloat(selectCoupon.substring(selectCoupon.lastIndexOf("￥") + 1));
    }

    var orderTotal = parseFloat($("#vSubmmitOrder_hiddenCartTotal").val()) + freight - discountValue;
    $("#strongTotal").html("¥" + orderTotal.toFixed(2));
}

function chageCoupon() {
    var freight = 0;
    if ($("#selectShippingType").val() != "-1") {
        var selectedShippingType = $("#selectShippingType option:selected").text();
        freight = parseFloat(selectedShippingType.substring(selectedShippingType.lastIndexOf("￥") + 1));
    }

    var discountValue = 0;
    if ($("#selectCoupon").val() != "") {
        var selectCoupon = $("#selectCoupon option:selected").text();
        discountValue = parseFloat(selectCoupon.substring(selectCoupon.lastIndexOf("￥") + 1));
    }

    var orderTotal = parseFloat($("#vSubmmitOrder_hiddenCartTotal").val()) + freight - discountValue;
    $("#strongTotal").html("¥" + orderTotal.toFixed(2));
}









