﻿// -----------------------------------------------------------------------------
// 让 .NET 开发更简单，更通用，更流行。
// Copyright © 2020-2021 Furion, 百小僧, Baiqian Co.,Ltd.
//
// 框架名称：Furion
// 框架作者：百小僧
// 框架版本：2.14.1
// 源码地址：Gitee： https://gitee.com/dotnetchina/Furion
//          Github：https://github.com/monksoul/Furion
// 开源协议：Apache-2.0（https://gitee.com/dotnetchina/Furion/blob/master/LICENSE）
// -----------------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Furion
{
    /// <summary>
    /// 内部 App 副本
    /// </summary>
    internal static class InternalApp
    {
        /// <summary>
        /// 应用服务
        /// </summary>
        internal static IServiceCollection InternalServices;

        /// <summary>
        /// 根服务
        /// </summary>
        internal static IServiceProvider RootServices;

        /// <summary>
        /// 配置对象
        /// </summary>
        internal static IConfiguration Configuration;

        /// <summary>
        /// 获取Web主机环境
        /// </summary>
        internal static IWebHostEnvironment WebHostEnvironment;

        /// <summary>
        /// 获取泛型主机环境
        /// </summary>
        internal static IHostEnvironment HostEnvironment;

        /// <summary>
        /// 加载自定义 .json 配置文件
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="hostEnvironment"></param>
        internal static void AddJsonFiles(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
        {
            // 获取程序目录下的所有配置文件（只限顶级目标，不递归查找）
            var jsonFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.json", SearchOption.TopDirectoryOnly)
                .Union(
                    Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json", SearchOption.TopDirectoryOnly)
                );

            // 如果没有配置文件，中止执行
            if (!jsonFiles.Any()) return;

            // 获取环境变量名
            var envName = hostEnvironment.EnvironmentName;

            // 读取忽略的配置文件
            var ignoreConfigurationFiles = configurationBuilder.Build()
                    .GetSection("IgnoreConfigurationFiles")
                    .Get<string[]>()
                ?? Array.Empty<string>();

            // 将所有文件进行分组
            var jsonFilesGroups = SplitConfigFileNameToGroups(jsonFiles)
                                                                    .Where(u => !excludeJsonPrefixs.Contains(u.Key, StringComparer.OrdinalIgnoreCase) && !u.Any(c => runtimeJsonSuffixs.Any(z => c.EndsWith(z, StringComparison.OrdinalIgnoreCase)) || ignoreConfigurationFiles.Contains(Path.GetFileName(c), StringComparer.OrdinalIgnoreCase)));

            // 遍历所有配置分组
            foreach (var group in jsonFilesGroups)
            {
                // 限制查找的 json 文件组
                var limitFileNames = new[] { $"{group.Key}.json", $"{group.Key}.{envName}.json" };

                // 查找默认配置和环境配置
                var files = group.Where(u => limitFileNames.Contains(Path.GetFileName(u), StringComparer.OrdinalIgnoreCase))
                                                 .OrderBy(u => Path.GetFileName(u).Length);

                // 循环加载
                foreach (var jsonFile in files)
                {
                    configurationBuilder.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);
                }
            }
        }

        /// <summary>
        /// 排序的配置文件前缀
        /// </summary>
        private static readonly string[] excludeJsonPrefixs = new[] { "appsettings", "bundleconfig", "compilerconfig" };

        /// <summary>
        /// 排除运行时 Json 后缀
        /// </summary>
        private static readonly string[] runtimeJsonSuffixs = new[]
        {
            "deps.json",
            "runtimeconfig.dev.json",
            "runtimeconfig.prod.json",
            "runtimeconfig.json"
        };

        /// <summary>
        /// 对配置文件名进行分组
        /// </summary>
        /// <param name="configFiles"></param>
        /// <returns></returns>
        private static IEnumerable<IGrouping<string, string>> SplitConfigFileNameToGroups(IEnumerable<string> configFiles)
        {
            // 分组
            return configFiles.GroupBy(Function);

            // 本地函数
            static string Function(string file)
            {
                // 根据 . 分隔
                var fileNameParts = Path.GetFileName(file).Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (fileNameParts.Length == 2) return fileNameParts[0];

                return string.Join('.', fileNameParts.Take(fileNameParts.Length - 2));
            }
        }
    }
}