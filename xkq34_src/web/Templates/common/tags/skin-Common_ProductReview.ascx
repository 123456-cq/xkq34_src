﻿<%@ Control Language="C#" %>
<%@ Import Namespace="Hidistro.Core" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<div class="comment-box">
    <div class="name font-m text-muted" id="name-font-m">
     <lable class="replace"> <%# Eval("UserName")%></lable><span class="date"><%# Convert.ToDateTime(Eval("ReviewDate")).ToString("yyyy-MM-dd")%></span></div>
    <div class="detail">
        <%# Eval("ReviewText")%></div>
</div>
