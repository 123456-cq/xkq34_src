﻿namespace Hishop.Weixin.MP.Request
{
    using Hishop.Weixin.MP;
    using System;
    using System.Runtime.CompilerServices;

    public class VideoRequest : AbstractRequest
    {
        public string MediaId { get; set; }

        public string ThumbMediaId { get; set; }
    }
}

