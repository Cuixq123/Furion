﻿// -----------------------------------------------------------------------------
// 让 .NET 开发更简单，更通用，更流行。
// Copyright © 2020-2021 Furion, 百小僧, Baiqian Co.,Ltd.
//
// 框架名称：Furion
// 框架作者：百小僧
// 框架版本：3.0.0-preview.6.21355.2
// 源码地址：Gitee： https://gitee.com/dotnetchina/Furion
//          Github：https://github.com/monksoul/Furion
// 开源协议：Apache-2.0（https://gitee.com/dotnetchina/Furion/blob/master/LICENSE）
// -----------------------------------------------------------------------------

using Furion.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 虚拟文件服务中间件
    /// </summary>
    [SuppressSniffer]
    public static class VirtualFileServerApplicationBuilderExtensions
    {
        /// <summary>
        /// 虚拟文件系统中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseVirtualFileServer(this IApplicationBuilder app)
        {
            return app;
        }
    }
}