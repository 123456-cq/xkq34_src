
$(function () {
    switch (window.location.pathname.toLowerCase()) {
        case "/default.aspx":
        case "/productsearch.aspx": //分类查询首页
        case "/productlist.aspx": //所有商品
        case "/branddetail.aspx": //品牌专题
        case "/activities.aspx": //店铺活动
            jQuery.getJSON("/api/Hi_Ajax_NavMenu.ashx", function (settingjson) {
                //底部导航
                menu(settingjson);
                //弹出圆圈菜单
                if (window.location.pathname.toLowerCase() == "/default.aspx") {
                    AddMenu(settingjson);
                }
            });
            break;
        default:
            break;
    }
})




function menu(settingjson) {
    var ispass = false;
    switch (window.location.pathname.toLowerCase()) {
        case "/default.aspx":
            if (settingjson.ShopDefault) {
                ispass = true;
            }
            break;
        case "/productsearch.aspx": //分类查询首页
            if (settingjson.GoodsType) {
                ispass = true;
            }
            break;
        case "/productlist.aspx": //所有商品
            if (settingjson.GoodsListMenu) {
                ispass = true;
            }
            break;
        case "/branddetail.aspx": //品牌专题
            if (settingjson.BrandMenu) {
                ispass = true;
            }
            break;
        case "/activities.aspx": //店铺活动
            if (settingjson.ActivityMenu) {
                ispass = true;
            }
            break;
        default:
            break;
    }
    if (ispass) {
        $(_.template($("#menu").html())(settingjson)).appendTo('body');
        GetUICss();
    }
}


function GetUICss() {
    var oUl = $("#ul");
    var len = parseInt($("#ul > li").length);
    for (i = 0; i < $("#ul > li").length; i++) {
        var width = 100 / len;
        width += "%"
        $("#ul > li").eq(i).css("width", width);
    }


    //    导航栏区分js
    $("#ul>li a").click(function () {
        var name = $(this).text().trim();
        switch (name) {
            case "首页":
                window.localStorage.setItem("navIndex", 1);
                break;
            case "分类":
                window.localStorage.setItem("navIndex", 2);
                break;
            case "我的":
                window.localStorage.setItem("navIndex", 4);
                break;
        }
    });
    var navIndex = window.localStorage["navIndex"];
    switch (navIndex) {
        case "1":
            $("#ul>li:nth-child(1) a").css({ "color": "red", "font-weight": "bolder" });
            break;
        case "2":
            $("#ul>li:nth-child(2) a").css({ "color": "red", "font-weight": "bolder" });
            break;
        case "4":
            $("#ul>li:nth-child(4) a").css({ "color": "red", "font-weight": "bolder" });
            break;
        default:
            $("#ul>li:nth-child(1) a").css({ "color": "red", "font-weight": "bolder"});
            break;
    }


    $("#menuNav  div[data='1']").click(function () {
        if ($(this).find(".childNav").css("display") == "none") {
            $(this).find(".childNav").css("display", "block");
        } else {
            $(this).find(".childNav").css("display", "none");
        }
    });
}

