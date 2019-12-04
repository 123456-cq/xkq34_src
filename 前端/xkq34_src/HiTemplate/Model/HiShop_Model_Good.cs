﻿using System;
using System.Runtime.CompilerServices;

namespace HiTemplate.Model
{

    /// <summary>
    /// 商品模块
    /// </summary>
    public class HiShop_Model_Good
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string item_id { get; set; }

        /// <summary>
        /// 商品链接
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// 原始价格
        /// </summary>
        public string original_price { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string pic { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 购物送积分
        /// </summary>
        public string integral { get; set; }

        /// <summary>
        /// 积分兑换上限
        /// </summary>
        public string IntegralCeiling { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public int FristPrice { get; set; }
    }

}

